using System;
using System.Data;
using System.Data.SqlClient;

namespace DataProxy.Creators
{
    class SqlServerCreator : IDbCreator
    {
        private const string LocalSqlServerName = "AMDFXPC\\SQLEXPRESS";
        private readonly string _dataBaseName;

        public SqlServerCreator(String dataBaseName)
        {
            _dataBaseName = dataBaseName;
        }

        public string CreateNewDatabase()
        {
            String str = $"Create database {_dataBaseName};";
            SqlConnection myConn =
                new SqlConnection($"Server={LocalSqlServerName};Integrated security=True;database=master");
            SqlCommand myCommand = new SqlCommand(str, myConn);

            myConn.Open();
            myCommand.ExecuteNonQuery();

            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};Integrated security=True";
        }
    }
}
