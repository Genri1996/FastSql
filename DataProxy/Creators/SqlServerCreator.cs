using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
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
            StringBuilder result = new StringBuilder();
            
            //Create Database
            String query1 =$"use master create database {_dataBaseName}";

            //Create Login TODO: Добавить проверку на существование логина
            String query2 = $"create login {login} with password = '{password}'";

            //Create User for login
            String query3 = $"use {_dataBaseName} create user {login} for login {login}";

            //Grant permission for user
            String query4 = $"EXEC sp_addrolemember 'db_owner', {login}";

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                result.Append(executor.ExecuteQueryAsString(query1));
                result.Append(executor.ExecuteQueryAsString(query2));
                result.Append(executor.ExecuteQueryAsString(query3));
                result.Append(executor.ExecuteQueryAsString(query4));
            }

            if(result.ToString()!=string.Empty)
                throw new Exception("Создание базы данных провалилось.", new Exception(result.ToString()));

            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};User Id={login};Password={password}";
        }
    }
}
