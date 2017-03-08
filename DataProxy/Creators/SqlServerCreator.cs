using System;
using System.Configuration;
using System.Text;
using DataProxy.DbManangment;
using DataProxy.Executors;

namespace DataProxy.Creators
{
    /// <summary>
    /// Implementation for SQL Server
    /// </summary>
    class SqlServerCreator : IDbCreator
    {
        private const string LocalSqlServerName = @"SERVERNAME";
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
        public string CreateNewDatabaseWithRandomLogin(DataBaseInfo dbInfo)
        {
            //errors collector
            StringBuilder result = new StringBuilder();

            dbInfo.Login = "login" + DateTime.Now.Ticks;
            dbInfo.Password = "pass" + DateTime.Now.Ticks % 10000 * 37;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                string createLoginQuery = $"USE MASTER CREATE LOGIN {dbInfo.Login} WITH PASSWORD = '{dbInfo.Password}'";
                result.Append(executor.ExecuteQueryAsString(createLoginQuery));

                string createDbQuery = $"USE MASTER CREATE DATABASE {_dataBaseName}";
                string createUserQuery = $"USE {_dataBaseName} CREATE USER {dbInfo.Login} FOR LOGIN {dbInfo.Login}";
                //Apply protection rules
                string grantPermissionQuery = $"USE {_dataBaseName} EXEC sp_addrolemember 'db_owner', {dbInfo.Login}";

                result.Append(executor.ExecuteQueryAsString(createDbQuery));
                result.Append(executor.ExecuteQueryAsString(createUserQuery));
                result.Append(executor.ExecuteQueryAsString(grantPermissionQuery));
            }

            if (result.ToString() != string.Empty)
                throw new Exception(result.ToString());

            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};User Id={dbInfo.Login};Password={dbInfo.Password}";
        }

        /// <summary>
        /// Creates private database with selected login and password
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string CreateNewDatabaseWithProtection(DataBaseInfo dbInfo)
        {
            //errors collector
            StringBuilder result = new StringBuilder();

            var loginTimeStamp = DateTime.Now.Ticks % 100000;
            dbInfo.Login += loginTimeStamp;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                string createLoginQuery = $"USE MASTER CREATE LOGIN {dbInfo.Login} WITH PASSWORD = '{dbInfo.Password}'";
                result.Append(executor.ExecuteQueryAsString(createLoginQuery));

                string createDbQuery = $"USE MASTER CREATE DATABASE {_dataBaseName}";
                string createUserQuery = $"USE {_dataBaseName} CREATE USER {dbInfo.Login} FOR LOGIN {dbInfo.Login}";
                //Apply protection rules
                string grantPermissionQuery = $"USE {_dataBaseName} EXEC sp_addrolemember 'db_owner', {dbInfo.Login}";

                result.Append(executor.ExecuteQueryAsString(createDbQuery));
                result.Append(executor.ExecuteQueryAsString(createUserQuery));
                result.Append(executor.ExecuteQueryAsString(grantPermissionQuery));
            }

            if (result.ToString() != string.Empty)
                throw new Exception(result.ToString());

            return $"Data Source={LocalSqlServerName};Initial Catalog={_dataBaseName};User Id={dbInfo.Login};Password={dbInfo.Password}";
        }
    }
}
