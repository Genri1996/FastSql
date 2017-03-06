using System.Data.Common;
using MySql.Data.MySqlClient;

namespace DataProxy.Executors
{
    public sealed class MySqlExecutor : QueryExecutor
    {
        protected override DbConnection Connection { get; set; }
        protected override DbDataAdapter DataAdapter { get; set; }

        public MySqlExecutor(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            DataAdapter = new MySqlDataAdapter();
            Open();
        }
    }
}
