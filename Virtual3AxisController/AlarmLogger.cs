using System;
using System.IO;
using System.Threading.Tasks;

namespace Virtual3AxisController
{
    // 알람 데이터를 담는 DTO(Data Transfer Object)
    public class AlarmItem
    {
        public string Timestamp { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;       // ALARM, ERROR, RESET, INFO, WARN
        public string Code { get; set; } = string.Empty;       // ALM_EMO, NET_ERR 등
        public string Message { get; set; } = string.Empty;
    }

    public class AlarmLogger
    {
        private readonly object _fileLock = new object();
        private readonly string _logFilePath;

        // 알람이 기록되었을 때 UI(Form1)로 알리는 이벤트
        public event Action<AlarmItem>? OnAlarmLogged;

        public AlarmLogger(string? customFileName = null)
        {
            string fileName = customFileName ?? "alarm_log.txt";
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        /// <summary>
        /// 알람/이벤트를 기록하고 파일 저장 및 UI 이벤트를 발행합니다.
        /// </summary>
        public void Log(string type, string code, string message)
        {
            var item = new AlarmItem
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Type = type,
                Code = code,
                Message = message
            };

            // 1. UI 갱신을 위해 구독자(Form1)에게 알림 이벤트 발행
            OnAlarmLogged?.Invoke(item);

            // 2. 백그라운드 스레드에서 파일 동기화 쓰기 (UI 랙 방지)
            Task.Run(() =>
            {
                lock (_fileLock)
                {
                    try
                    {
                        string line = $"[{item.Timestamp}] [{item.Type}] [{item.Code}] {item.Message}{Environment.NewLine}";
                        File.AppendAllText(_logFilePath, line);
                    }
                    catch
                    {
                        // 디스크 I/O 실패 시 메인 제어 루프 영향 차단
                    }
                }
            });
        }
    }
}