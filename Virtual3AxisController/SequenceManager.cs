using OpenCvSharp.Dnn;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Virtual3AxisController
{
    // 장비 운전 상태 정의 (FSM)
    public enum MachineState
    {
        Idle,
        Homing,
        Running,
        Pause,
        Alarm
    }

    public class SequenceManager
    {
        private readonly ModbusManager _modbus;
        private CancellationTokenSource? _ctsAutoRun;

        // PLC 메모리 맵 주소 정의
        private const ushort ADDR_POS_X = 0;
        private const ushort ADDR_POS_Y = 1;
        private const ushort ADDR_POS_Z = 2;

        private const ushort ADDR_ACTUATOR = 20;

        private ushort _targetX = 2000;
        private ushort _targetY = 1500;

        public MachineState CurrentState { get; private set; } = MachineState.Idle;

        // UI 갱신용 이벤트 콜백
        public event Action<MachineState>? OnStateChanged;
        public event Action<string>? OnStepMessageChanged;
        public event Action<string, string, string>? OnAlarmTriggered; // type, code, message
        public event Action<string, long, string>? OnCycleCompleted;

        public SequenceManager(ModbusManager modbus)
        {
            _modbus = modbus;
        }

        public void SetState(MachineState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }

        /// <summary>
        /// 비상 정지 또는 알람 발생 시 시퀀스 즉각 중단
        /// </summary>
        public void TriggerEmergencyStop(string code, string reason)
        {
            _ctsAutoRun?.Cancel();
            SetState(MachineState.Alarm);
            OnAlarmTriggered?.Invoke("ALARM", code, reason);
            OnStepMessageChanged?.Invoke($"[ALARM] {reason}");
        }

        /// <summary>
        /// 원점 복귀(Homing) 시퀀스: Z축 상승 -> X, Y축 복귀
        /// </summary>
        public async Task<bool> HomingAsync(bool isDoorClosed)
        {
            if (!_modbus.IsConnected)
            {
                OnStepMessageChanged?.Invoke("통신이 연결되어 있지 않습니다.");
                return false;
            }

            if (!isDoorClosed)
            {
                OnStepMessageChanged?.Invoke("도어가 열려 있어 원점 복귀를 수행할 수 없습니다.");
                return false;
            }

            SetState(MachineState.Homing);

            try
            {
                OnStepMessageChanged?.Invoke("Homing: Z축 안전 복귀 중...");
                await _modbus.WriteRegisterAsync(ADDR_POS_Z, 0);
                await Task.Delay(800);

                OnStepMessageChanged?.Invoke("Homing: X, Y축 원점 복귀 중...");
                await _modbus.WriteRegisterAsync(ADDR_POS_X, 0);
                await Task.Delay(400);
                await _modbus.WriteRegisterAsync(ADDR_POS_Y, 0);
                await Task.Delay(400);

                OnStepMessageChanged?.Invoke("원점 복귀 완료");
                SetState(MachineState.Idle);
                return true;
            }
            catch (Exception ex)
            {
                OnStepMessageChanged?.Invoke($"Homing 에러: {ex.Message}");
                TriggerEmergencyStop("ALM_HOME", $"원점 복귀 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 알람 리셋 및 초기화 시퀀스
        /// </summary>
        public async Task<bool> ResetAsync(bool isDoorClosed)
        {
            if (!_modbus.IsConnected) return false;

            if (!isDoorClosed)
            {
                OnStepMessageChanged?.Invoke("도어가 열려 있어 장비를 초기화할 수 없습니다.");
                return false;
            }

            try
            {
                SetState(MachineState.Homing);
                OnStepMessageChanged?.Invoke("초기화: Z축 안전 탈출 상승 중...");
                await _modbus.WriteRegisterAsync(ADDR_POS_Z, 0);
                await Task.Delay(800);

                OnStepMessageChanged?.Invoke("초기화: X, Y축 원점 복귀 중...");
                await _modbus.WriteRegisterAsync(ADDR_POS_X, 0);
                await Task.Delay(400);
                await _modbus.WriteRegisterAsync(ADDR_POS_Y, 0);
                await Task.Delay(400);

                OnStepMessageChanged?.Invoke("장비 초기화 완료 (정상 대기 상태)");
                SetState(MachineState.Idle);
                OnAlarmTriggered?.Invoke("RESET", "ALM_CLR", "사용자 초기화 완료: 알람 상태 해제 및 원점 정렬 완료");
                return true;
            }
            catch (Exception ex)
            {
                OnStepMessageChanged?.Invoke($"초기화 실패: {ex.Message}");
                TriggerEmergencyStop("ERR_RST", $"초기화 중 오류 발생: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 자동 운전 시작
        /// </summary>
        public bool StartAutoRun(bool isDoorClosed)
        {
            if (!_modbus.IsConnected || !isDoorClosed || CurrentState == MachineState.Alarm)
            {
                return false;
            }

            SetState(MachineState.Running);
            _ctsAutoRun = new CancellationTokenSource();
            OnAlarmTriggered?.Invoke("INFO", "SEQ_RUN", "자동 운전 시퀀스 가동 시작");

            Task.Run(() => AutoProcessLoopAsync(_ctsAutoRun.Token));
            return true;
        }

        /// <summary>
        /// 자동 운전 정지 지령
        /// </summary>
        public void StopAutoRun()
        {
            if (_ctsAutoRun != null && !_ctsAutoRun.IsCancellationRequested)
            {
                OnStepMessageChanged?.Invoke("정지 신호 전송 중...");
                _ctsAutoRun.Cancel();
                OnAlarmTriggered?.Invoke("INFO", "SEQ_STP", "사용자 정지(STOP) 요청");
            }
        }

        /// <summary>
        /// 백그라운드 자동 공정 루프 (FSM 4단계)
        /// </summary>
        private async Task AutoProcessLoopAsync(CancellationToken token)
        {
            int cycleCount = 0;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    cycleCount++;
                    var sw = Stopwatch.StartNew();

                    OnStepMessageChanged?.Invoke($"[사이클 #{cycleCount}] 1단계: 원점 대기 & 자재 안착");
                    await _modbus.WriteRegisterAsync(ADDR_POS_Z, 0);
                    await _modbus.WriteRegisterAsync(ADDR_POS_X, 0);
                    await _modbus.WriteRegisterAsync(ADDR_POS_Y, 0);
                    await Task.Delay(1000, token);

                    OnStepMessageChanged?.Invoke($"[사이클 #{cycleCount}] 2단계: 가공 위치로 이동 (X: 2000, Y: 1500)");
                    await _modbus.WriteRegisterAsync(ADDR_POS_X, _targetX);
                    await _modbus.WriteRegisterAsync(ADDR_POS_Y, _targetY);
                    await Task.Delay(1500, token);

                    OnStepMessageChanged?.Invoke($"[사이클 #{cycleCount}] 3단계: Z축 하강 (Z: 500) 및 가공/검사 수행");
                    await _modbus.WriteRegisterAsync(ADDR_POS_Z, 500);
                    await Task.Delay(2000, token);

                    await _modbus.WriteRegisterAsync(ADDR_ACTUATOR, 1);
                    await Task.Delay(300, token);
                    await _modbus.WriteRegisterAsync(ADDR_ACTUATOR, 2);
                    await Task.Delay(800, token);

                    OnStepMessageChanged?.Invoke($"[사이클 #{cycleCount}] 4단계: Z축 탈출 상승 (Z: 0)");
                    await _modbus.WriteRegisterAsync(ADDR_POS_Z, 0);
                    await Task.Delay(800, token);

                    await _modbus.WriteRegisterAsync(ADDR_ACTUATOR, 0);
                    await Task.Delay(500, token);

                    sw.Stop(); // 측정 종료
                    string lotId = $"LOT-{DateTime.Now:yyyyMMdd}-{cycleCount:D3}";
                    string result = (cycleCount % 10 == 0) ? "NG" : "OK"; // 10번에 1번꼴로 불량(NG) 판정 모사

                    OnCycleCompleted?.Invoke(lotId, sw.ElapsedMilliseconds, result);
                    
                }
            }
            catch (OperationCanceledException)
            {
                OnStepMessageChanged?.Invoke("자동 운전이 정지되었습니다.");
            }
            catch (Exception ex)
            {
                OnStepMessageChanged?.Invoke($"자동 공정 에러: {ex.Message}");
                TriggerEmergencyStop("ALM_SEQ", $"공정 시퀀스 중단 예외: {ex.Message}");
                return;
            }

            if (CurrentState != MachineState.Alarm)
            {
                SetState(MachineState.Idle);
            }
        }
        public void UpdateTargetFromVision(int cx, int cy)
        {
            _targetX = (ushort)(cx * 15);
            _targetY = (ushort)(cy * 15);
        }
    }
}
