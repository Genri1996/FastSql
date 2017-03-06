using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace DataProxy.Executors
{
    /// <summary>
    /// Provides an ability to execute query to SQL server
    /// </summary>
    public sealed class SqlServerExecutor : QueryExecutor
    {
        protected override DbConnection Connection { get; set; }
        protected override DbDataAdapter DataAdapter { get; set; }

        public SqlServerExecutor(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            DataAdapter = new SqlDataAdapter();
            Open();
        }
    }
}
