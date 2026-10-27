using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;

namespace Virtual3AxisController
{
    public class ModbusManager
    {
        private TcpClient? _tcpClient;
        private IModbusMaster? _master;
        private CancellationTokenSource? _cts;
        private bool _isPolling = false;

        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        // 새 데이터를 읽어왔을 때 UI로 전달하는 이벤트 (0~2번지 좌표, 10번지 센서 등)
        public event Action<ushort[]>? OnDataReceived;
        // 통신 오류 발생 시 UI로 알리는 이벤트
        public event Action<string>? OnErrorOccurred;

        // 1. 비동기 연결
        public async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ip, port);

                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_tcpClient);

                // 연결 성공 시 백그라운드 폴링(실시간 모니터링) 자동 시작
                StartPolling();
                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke($"연결 실패: {ex.Message}");
                return false;
            }
        }

        // 2. 연결 종료
        public void Disconnect()
        {
            StopPolling();

            _master?.Dispose();
            _master = null;

            _tcpClient?.Close();
            _tcpClient?.Dispose();
            _tcpClient = null;
        }

        // 3. 백그라운드 실시간 모니터링 (UI 프리징 없는 독립 Task)
        private void StartPolling()
        {
            if (_isPolling) return;

            _cts = new CancellationTokenSource();
            _isPolling = true;

            Task.Run(async () =>
            {
                byte slaveId = 1;
                ushort startAddress = 0;
                ushort readCount = 15; // 0번지부터 14번지까지 총 15개 레지스터 일괄 수신

                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (_master != null && IsConnected)
                        {
                            // 15개 주소 일괄 읽기
                            ushort[] data = await _master.ReadHoldingRegistersAsync(slaveId, startAddress, readCount);

                            // UI로 수신된 데이터 이벤트 발행
                            OnDataReceived?.Invoke(data);
                        }

                        // 100ms 주기(초당 10회)로 반복
                        await Task.Delay(100, _cts.Token);
                    }
                    catch (Exception ex)
                    {
                        OnErrorOccurred?.Invoke($"데이터 수신 오류: {ex.Message}");
                        break;
                    }
                }

                _isPolling = false;
            }, _cts.Token);
        }

        private void StopPolling()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            _isPolling = false;
        }

        // 4. 레지스터 값 쓰기 메서드
        public async Task WriteRegisterAsync(ushort address, ushort value)
        {
            if (!IsConnected || _master == null)
                throw new InvalidOperationException("장비와 통신이 연결되어 있지 않습니다.");

            byte slaveId = 1;
            await _master.WriteSingleRegisterAsync(slaveId, address, value);
        }
    }
}