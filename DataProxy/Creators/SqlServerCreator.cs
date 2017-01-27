using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DataProxy.Executors;

namespace DataProxy.Creators
{
    
    class SqlServerCreator : IDbCreator
    {
        private const string  LocalSqlServerName = @"AMDFXPC\SQLEXPRESS";
        private string _masterConnectionString;
        private readonly string _dataBaseName;

        public SqlServerCreator(String dataBaseName)
        {
            _dataBaseName = dataBaseName;
            _masterConnectionString = ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString;
        }

        public string CreateNewDatabase()
        {
            String query = $"Create database {_dataBaseName};";
            SqlConnection myConn = new SqlConnection(_masterConnectionString);
            SqlCommand myCommand = new SqlCommand(query, myConn);

            myConn.Open();
            myCommand.ExecuteNonQuery();

            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }
            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};Integrated security=True";
        }

        
        public string CreateNewDatabaseWithProtection(string login, string password)
        {
            SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString);
            String query1 = @"Create database @databasename;";
            //TODO Дописать
            return null;
        }
    }
}
