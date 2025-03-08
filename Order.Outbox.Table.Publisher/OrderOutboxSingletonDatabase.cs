

using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Order.Outbox.Table.Publisher
{
    public static class OrderOutboxSingletonDatabase
    {
        static IDbConnection _connection;
        static bool _dataReaderState = true;
        static OrderOutboxSingletonDatabase()
        => _connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=OutboxInboxOrderDB;User Id=sa;Password=123;Integrated Security=true;Trusted_Connection=Yes;TrustServerCertificate=True;");

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql) => await _connection.QueryAsync<T>(sql);

        public static IDbConnection Connection { 
            get 
            {
                if(_connection.State == ConnectionState.Closed)
                    _connection.Open();

                return _connection;
            } 
        }
        public static async Task<int> ExecuteAsync(string sql) => await _connection.ExecuteAsync(sql);

        public static void DataReaderReady() => _dataReaderState = true;
        public static void DataReaderBusy() => _dataReaderState = false;  
        
        public static bool DataReaderState => _dataReaderState;
    }
}
