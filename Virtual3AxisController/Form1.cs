using System;
using System.Drawing;
using System.Reflection.Metadata;
using System.Windows.Forms;

namespace Virtual3AxisController
{
    public partial class Form1 : Form
    {
        private readonly ModbusManager _modbus = new ModbusManager();
        private readonly AlarmLogger _logger = new AlarmLogger();
        private readonly SequenceManager _sequence;
        private readonly VisionManager _vision = new VisionManager();
        private readonly DatabaseManager _db = new DatabaseManager();

        // JOG 좌표 주소 및 수신 캐시
        private const ushort ADDR_POS_X = 0;
        private const ushort ADDR_POS_Y = 1;
        private const ushort ADDR_POS_Z = 2;
        private ushort[] _currentPositions = new ushort[3];
        private bool _isDoorClosed = true;

        // 수동 JOG 조작 제어 타이머
        private readonly System.Windows.Forms.Timer _jogTimer = new System.Windows.Forms.Timer();
        private ushort _activeJogAddress = 0;
        private int _jogDirection = 0;
        private const ushort JOG_STEP = 20;

        public Form1()
        {
            InitializeComponent();

            _sequence = new SequenceManager(_modbus);

            // 1. 이벤트 바인딩
            _modbus.OnDataReceived += Modbus_OnDataReceived;
            _modbus.OnErrorOccurred += Modbus_OnErrorOccurred;

            _logger.OnAlarmLogged += Logger_OnAlarmLogged;

            _sequence.OnStateChanged += Sequence_OnStateChanged;
            _sequence.OnStepMessageChanged += Sequence_OnStepMessageChanged;
            _sequence.OnAlarmTriggered += (type, code, msg) => _logger.Log(type, code, msg);
            _sequence.OnCycleCompleted += Sequence_OnCycleCompleted;
            _vision.OnFrameReady += Vision_OnFrameReady;
            _vision.OnCameraError += Vision_OnCameraError;
            _vision.OnTargetDetected += _sequence.UpdateTargetFromVision;
            _vision.OnTargetDetected += Vision_OnTargetDetected_UI;

            // 2. JOG 타이머 세팅
            _jogTimer.Interval = 50;
            _jogTimer.Tick += JogTimer_Tick;
            RegisterJogEvents();

            // 3. 초기 상태 갱신
            _sequence.SetState(MachineState.Idle);
            _logger.Log("INFO", "SYS_001", "제어 시스템 프로그램 시작");

            UpdateDashboardUI(); // 프로그램 기동 시 기존 데이터베이스 기록 로드
        }

        // 3. 클래스 하단 빈 공간에 DB 저장 및 대시보드 갱신 헬퍼 메서드 추가
        private void Sequence_OnCycleCompleted(string lotId, long cycleTime, string result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Sequence_OnCycleCompleted(lotId, cycleTime, result)));
                return;
            }

            _db.InsertProductionRecord(lotId, cycleTime, result);
            _logger.Log("INFO", "PRD_END", $"[{lotId}] 사이클 완료 (결과: {result}, 소요시간: {cycleTime}ms)");
            UpdateDashboardUI();
        }

        private void UpdateDashboardUI()
        {
            var stats = _db.GetTodayStats();
            lblTotal.Text = $"총 생산: {stats.TotalCount} 개";
            lblOk.Text = $"양품: {stats.OkCount} 개";
            lblNg.Text = $"불량: {stats.NgCount} 개";
            lblYield.Text = $"수율: {stats.YieldRate} %";
        }

        // ==========================================
        // UI 상태 및 화면 갱신 이벤트 핸들러
        // ==========================================
        private void Sequence_OnStateChanged(MachineState state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Sequence_OnStateChanged(state)));
                return;
            }

            lblMachineState.Text = $"상태: {state.ToString().ToUpper()}";
            lblMachineState.ForeColor = (state == MachineState.Alarm) ? Color.Red : Color.Blue;

            bool isIdle = (state == MachineState.Idle);
            grpJog.Enabled = isIdle;
            btnHome.Enabled = isIdle;
            btnAutoStart.Enabled = isIdle;
            btnAutoStop.Enabled = (state == MachineState.Running);
            btnReset.Enabled = (state == MachineState.Alarm || state == MachineState.Idle);
        }

        private void Sequence_OnStepMessageChanged(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Sequence_OnStepMessageChanged(message)));
                return;
            }
            lblStatus.Text = message;
        }

        private void Logger_OnAlarmLogged(AlarmItem item)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Logger_OnAlarmLogged(item)));
                return;
            }

            dgvAlarmLog.Rows.Insert(0, item.Timestamp, item.Code, item.Type, item.Message);
            var targetRow = dgvAlarmLog.Rows[0];

            if (item.Type == "ALARM")
            {
                targetRow.DefaultCellStyle.BackColor = Color.MistyRose;
                targetRow.DefaultCellStyle.ForeColor = Color.DarkRed;
            }
            else if (item.Type == "RESET")
            {
                targetRow.DefaultCellStyle.BackColor = Color.Honeydew;
                targetRow.DefaultCellStyle.ForeColor = Color.DarkGreen;
            }
        }

        // ==========================================
        // 상단 버튼 조작부 (단순 호출 위임)
        // ==========================================
        private async void btnHome_Click(object sender, EventArgs e)
        {
            await _sequence.HomingAsync(_isDoorClosed);
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            await _sequence.ResetAsync(_isDoorClosed);
        }

        private void btnAutoStart_Click(object sender, EventArgs e)
        {
            if (!_sequence.StartAutoRun(_isDoorClosed))
            {
                if (!_modbus.IsConnected) MessageBox.Show("통신을 먼저 연결해주세요.");
                else if (!_isDoorClosed) MessageBox.Show("도어가 열려 있어 가동할 수 없습니다.");
                else if (_sequence.CurrentState == MachineState.Alarm) MessageBox.Show("알람 상태입니다. 리셋을 먼저 수행하세요.");
            }
        }

        private void btnAutoStop_Click(object sender, EventArgs e)
        {
            _sequence.StopAutoRun();
        }

        private void btnEMO_Click(object sender, EventArgs e)
        {
            StopJog();
            _sequence.TriggerEmergencyStop("ALM_EMO", "사용자 EMO(비상정지) 버튼 작동");
        }

        // ==========================================
        // JOG 제어 로직
        // ==========================================
        private void RegisterJogEvents()
        {
            btnJogXPlus.MouseDown += (s, e) => StartJog(ADDR_POS_X, +1);
            btnJogXPlus.MouseUp += (s, e) => StopJog();
            btnJogXMinus.MouseDown += (s, e) => StartJog(ADDR_POS_X, -1);
            btnJogXMinus.MouseUp += (s, e) => StopJog();

            btnJogYPlus.MouseDown += (s, e) => StartJog(ADDR_POS_Y, +1);
            btnJogYPlus.MouseUp += (s, e) => StopJog();
            btnJogYMinus.MouseDown += (s, e) => StartJog(ADDR_POS_Y, -1);
            btnJogYMinus.MouseUp += (s, e) => StopJog();

            btnJogZPlus.MouseDown += (s, e) => StartJog(ADDR_POS_Z, +1);
            btnJogZPlus.MouseUp += (s, e) => StopJog();
            btnJogZMinus.MouseDown += (s, e) => StartJog(ADDR_POS_Z, -1);
            btnJogZMinus.MouseUp += (s, e) => StopJog();
        }

        private void StartJog(ushort axisAddress, int direction)
        {
            if (!_modbus.IsConnected || _sequence.CurrentState != MachineState.Idle) return;
            if (!_isDoorClosed)
            {
                MessageBox.Show("도어가 열려 있어 수동 조작을 할 수 없습니다.");
                return;
            }

            _activeJogAddress = axisAddress;
            _jogDirection = direction;
            _jogTimer.Start();
        }

        private void StopJog()
        {
            _jogTimer.Stop();
        }

        private async void JogTimer_Tick(object? sender, EventArgs e)
        {
            if (!_modbus.IsConnected)
            {
                StopJog();
                return;
            }

            int nextVal = _currentPositions[_activeJogAddress] + (_jogDirection * JOG_STEP);
            if (nextVal < 0) nextVal = 0;
            if (nextVal > 5000) nextVal = 5000;

            try
            {
                await _modbus.WriteRegisterAsync(_activeJogAddress, (ushort)nextVal);
            }
            catch (Exception ex)
            {
                StopJog();
                _logger.Log("ERROR", "ERR_JOG", $"JOG 쓰기 실패: {ex.Message}");
            }
        }

        // ==========================================
        // 통신 및 인터락 감시부
        // ==========================================
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            lblStatus.Text = "연결 시도 중...";
            lblStatus.ForeColor = Color.Orange;

            bool success = await _modbus.ConnectAsync(txtIp.Text.Trim(), int.Parse(txtPort.Text.Trim()));
            if (success)
            {
                lblStatus.Text = "통신 연결 성공";
                lblStatus.ForeColor = Color.Green;
                _logger.Log("INFO", "NET_001", $"Modbus TCP 연결 수립 성공 ({txtIp.Text}:{txtPort.Text})");
            }
            else
            {
                lblStatus.Text = "연결 실패";
                lblStatus.ForeColor = Color.Red;
                btnConnect.Enabled = true;
                _logger.Log("ERROR", "NET_ERR", $"Modbus TCP 연결 시도 실패 ({txtIp.Text}:{txtPort.Text})");
            }
        }

        private void Modbus_OnDataReceived(ushort[] data)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Modbus_OnDataReceived(data)));
                return;
            }

            _currentPositions[0] = data[0];
            _currentPositions[1] = data[1];
            _currentPositions[2] = data[2];

            lblPosX.Text = $"X축: {data[0]} mm";
            lblPosY.Text = $"Y축: {data[1]} mm";
            lblPosZ.Text = $"Z축: {data[2]} mm";

            if (data.Length > 10)
            {
                bool prevDoor = _isDoorClosed;
                _isDoorClosed = (data[10] == 1);

                if (_isDoorClosed)
                {
                    lblDoorSensor.BackColor = Color.Lime;
                    lblDoorSensor.Text = "도어 닫힘";
                }
                else
                {
                    lblDoorSensor.BackColor = Color.Red;
                    lblDoorSensor.Text = "도어 열림";

                    if (_sequence.CurrentState == MachineState.Running || _sequence.CurrentState == MachineState.Homing)
                    {
                        StopJog();
                        _sequence.TriggerEmergencyStop("ALM_DOOR", "가동 중 세이프티 도어 열림 감지 (인터락 차단)");
                    }
                    else if (prevDoor && !_isDoorClosed)
                    {
                        _logger.Log("WARN", "WRN_DOOR", "대기 중 세이프티 도어 개방 감지");
                    }
                }
            }
        }

        private void Modbus_OnErrorOccurred(string errorMsg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Modbus_OnErrorOccurred(errorMsg)));
                return;
            }

            lblStatus.Text = errorMsg;
            lblStatus.ForeColor = Color.Red;
            btnConnect.Enabled = true;
            _sequence.TriggerEmergencyStop("NET_LOST", $"통신 두절: {errorMsg}");
        }

        // [카메라 켜기]
        private void btnCameraOn_Click(object sender, EventArgs e)
        {
            _vision.StartCamera();
        }

        // [카메라 끄기]
        private void btnCameraOff_Click(object sender, EventArgs e)
        {
            _vision.StopCamera();
            picWebcam.Image = null; // 화면 지우기
        }

        // 백그라운드 비전 스레드에서 이미지가 넘어올 때 호출됨
        private void Vision_OnFrameReady(Bitmap bitmap)
        {
            // UI 크로스 스레드 방지
            if (picWebcam.InvokeRequired)
            {
                picWebcam.BeginInvoke(new Action(() => Vision_OnFrameReady(bitmap)));
                return;
            }

            // 메모리 누수(Memory Leak)를 막기 위해 이전 이미지는 해제 후 덮어쓰기
            var oldImage = picWebcam.Image;
            picWebcam.Image = bitmap;
            oldImage?.Dispose();
        }

        private void Vision_OnTargetDetected_UI(int cx, int cy)
        {
            if (this.InvokeRequired)
            {
                // UI 스레드 안전 호출
                this.BeginInvoke(new Action(() => Vision_OnTargetDetected_UI(cx, cy)));
                return;
            }

            // 그룹박스 텍스트를 실시간 중심 좌표로 갱신
            grpVision.Text = $"머신 비전 (X: {cx}, Y: {cy})";
        }

        private void Vision_OnCameraError(string errorMsg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Vision_OnCameraError(errorMsg)));
                return;
            }
            MessageBox.Show(errorMsg, "비전 시스템 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _vision.StopCamera();
            _sequence.StopAutoRun();
            StopJog();
            _modbus.Disconnect();
            _logger.Log("INFO", "SYS_002", "제어 시스템 프로그램 종료");
            base.OnFormClosing(e);
        }
    }
}