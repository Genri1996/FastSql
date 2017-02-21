using System;
using System.Configuration;
using System.Text;
using DataProxy.Executors;
using DataProxy.Helpers;

namespace DataProxy.Creators
{
    /// <summary>
    /// Implementation for SQL Server
    /// </summary>
    class SqlServerCreator : IDbCreator
    {
        //TODO: After publishing to another server should be changed.
        private const string LocalSqlServerName = @".\SQLEXPRESS";
        //Full access to server
        private readonly string _masterConnectionString;
        private readonly string _dataBaseName;

        public SqlServerCreator(string dataBaseName)
        {
            _dataBaseName = dataBaseName;
            _masterConnectionString = ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString;
        }

        /// <summary>
        /// Creates database without ptotection
        /// </summary>
        /// <returns></returns>
        public string CreateNewDatabase()
        {
            string query = $"CREATE DATABASE {_dataBaseName};";
            string error;
            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                error = executor.ExecuteQueryAsString(query);
            }
            if (error != string.Empty)
                throw new Exception("Создание базы данных провалилось.", new Exception(error));

            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};Integrated security=SSPI";
        }

        /// <summary>
        /// Creates private database with selected login and password
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string CreateNewDatabaseWithProtection(string login, string password)
        {
            //errors collector
            StringBuilder result = new StringBuilder();

            var loginTimeStamp = DateTime.Now.Ticks % 100000;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                string createLoginQuery = $"USE MASTER CREATE LOGIN {login + loginTimeStamp} WITH PASSWORD = '{password}'";
                result.Append(executor.ExecuteQueryAsString(createLoginQuery));
                
                string createDbQuery = $"USE MASTER CREATE DATABASE {_dataBaseName}";
                string createUserQuery = $"USE {_dataBaseName} CREATE USER {login + loginTimeStamp} FOR LOGIN {login + loginTimeStamp}";
                //Apply protection rules
                string grantPermissionQuery = $"USE {_dataBaseName} EXEC sp_addrolemember 'db_owner', {login+ loginTimeStamp}";

                result.Append(executor.ExecuteQueryAsString(createDbQuery));
                result.Append(executor.ExecuteQueryAsString(createUserQuery));
                result.Append(executor.ExecuteQueryAsString(grantPermissionQuery));
            }

            if (result.ToString() != string.Empty)
                throw new Exception("Создание базы данных провалилось.", new Exception(result.ToString()));

            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};User Id={login + loginTimeStamp};Password={password}";
        }
    }
}
