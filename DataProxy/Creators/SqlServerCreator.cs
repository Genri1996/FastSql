using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DataProxy.Executors;
using DataProxy.Helpers;

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
            StringBuilder result = new StringBuilder();      

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                if (new SqlServerHelper().IsLoginExists(login) == false)
                {
                    String createLoginQuery = $"use master create login {login} with password = '{password}'";
                    result.Append(executor.ExecuteQueryAsString(createLoginQuery));
                }
                String createDbQuery = $"use master create database {_dataBaseName}";
                String createUserQuery = $"use {_dataBaseName} create user {login} for login {login}";
                String grantPermissionQuery = $"use {_dataBaseName} EXEC sp_addrolemember 'db_owner', {login}";

                result.Append(executor.ExecuteQueryAsString(createDbQuery));
                result.Append(executor.ExecuteQueryAsString(createUserQuery));
                result.Append(executor.ExecuteQueryAsString(grantPermissionQuery));
            }

            if(result.ToString()!=string.Empty)
                throw new Exception("Создание базы данных провалилось.", new Exception(result.ToString()));

            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};User Id={login};Password={password}";
        }
    }
}
