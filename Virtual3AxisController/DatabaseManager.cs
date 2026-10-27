using System;
using System.Data.SQLite;
using System.IO;

namespace Virtual3AxisController
{
    // 대시보드 UI에 전달할 통계용 데이터 구조체 (DTO)
    public class ProductionStats
    {
        public int TotalCount { get; set; }
        public int OkCount { get; set; }
        public int NgCount { get; set; }
        // 양품률 계산 (소수점 1자리까지)
        public double YieldRate => TotalCount == 0 ? 0 : Math.Round((double)OkCount / TotalCount * 100, 1);
    }

    public class DatabaseManager
    {
        private readonly string _dbFilePath;
        private readonly string _connectionString;

        public DatabaseManager(string dbFileName = "ProductionLog.db")
        {
            // 프로그램 실행 폴더(bin/Debug/net8.0-windows) 경로에 DB 파일 경로 설정
            _dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);

            // SQLite 접속을 위한 연결 문자열 세팅
            _connectionString = $"Data Source={_dbFilePath};Version=3;";

            // 객체 생성 시 DB 파일과 테이블 자동 셋업
            InitializeDatabase();
        }

        /// <summary>
        /// DB 파일이 없으면 만들고, 기록용 테이블이 없으면 생성합니다.
        /// </summary>
        private void InitializeDatabase()
        {
            if (!File.Exists(_dbFilePath))
            {
                SQLiteConnection.CreateFile(_dbFilePath);
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                // MES 생산 이력 테이블 설계
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS ProductionHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Timestamp DATETIME DEFAULT (datetime('now', 'localtime')),
                        LotId TEXT,
                        CycleTimeMs INTEGER,
                        Result TEXT
                    )";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery(); // 쿼리 실행
                }
            }
        }

        /// <summary>
        /// 1사이클 완료 시 생산 데이터를 INSERT 합니다.
        /// </summary>
        public void InsertProductionRecord(string lotId, long cycleTimeMs, string result)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO ProductionHistory (LotId, CycleTimeMs, Result) 
                    VALUES (@LotId, @CycleTimeMs, @Result)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    // SQL 인젝션 방지 및 타입 안정을 위한 Parameter 사용
                    command.Parameters.AddWithValue("@LotId", lotId);
                    command.Parameters.AddWithValue("@CycleTimeMs", cycleTimeMs);
                    command.Parameters.AddWithValue("@Result", result);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 오늘(오늘 날짜 기준)의 총 생산량, 양품, 불량 개수를 조회하여 UI 대시보드용으로 반환합니다.
        /// </summary>
        public ProductionStats GetTodayStats()
        {
            var stats = new ProductionStats();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        COUNT(*) as Total,
                        SUM(CASE WHEN Result = 'OK' THEN 1 ELSE 0 END) as OkCount,
                        SUM(CASE WHEN Result = 'NG' THEN 1 ELSE 0 END) as NgCount
                    FROM ProductionHistory
                    WHERE date(Timestamp) = date('now', 'localtime')";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader()) // SELECT 결과를 읽기 위한 Reader
                {
                    if (reader.Read())
                    {
                        // DBNull 체크 후 데이터 매핑
                        stats.TotalCount = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0;
                        stats.OkCount = reader["OkCount"] != DBNull.Value ? Convert.ToInt32(reader["OkCount"]) : 0;
                        stats.NgCount = reader["NgCount"] != DBNull.Value ? Convert.ToInt32(reader["NgCount"]) : 0;
                    }
                }
            }
            return stats;
        }
    }
}