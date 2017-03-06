using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace DataProxy.Executors
{
    class MySqlExecutor:QueryExecutor
    {
        private readonly MySqlConnection _connection;

        /// <summary>
        /// Opens connection imeddiately.
        /// </summary>
        /// <param name="connectionString"></param>
        public MySqlExecutor(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            Open();
        }

        public override DataTable ExecuteQueryAsDataTable(string command)
        {
            throw new System.NotImplementedException();
        }

        public override string ExecuteQueryAsString(string command)
        {
            throw new System.NotImplementedException();
        }

        public override string ExecuteCommandAsString(SqlCommand command)
        {
            throw new System.NotImplementedException();
        }

        public override DataTable ExecuteCommandAsDataTable(SqlCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}
