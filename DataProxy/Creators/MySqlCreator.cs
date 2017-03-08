using System;
using System.Configuration;
using System.Text;
using DataProxy.DbManangment;
using DataProxy.Executors;

namespace DataProxy.Creators
{
    public class MySqlCreator:IDbCreator
    {
        private const string LocalSqlServerName = @"SERVERNAME";
        //Full access to server
        private readonly string _masterConnectionString;
        private readonly string _dataBaseName;

        public MySqlCreator(string dataBaseName)
        {
            _dataBaseName = dataBaseName;
            _masterConnectionString = ConfigurationManager.ConnectionStrings["MySqlMaster"].ConnectionString;
        }

        public string CreateNewDatabaseWithRandomLogin(DataBaseInfo dbInfo)
        {
            var login = "login" + DateTime.Now.Ticks;
            var password = "pass" + DateTime.Now.Ticks % 10000 * 37;

            dbInfo.Login = login;
            dbInfo.Password = password;

            return CreateNewDatabaseWithProtection(dbInfo);
        }

        public string CreateNewDatabaseWithProtection(DataBaseInfo dbInfo)
        {
            //errors collector
            StringBuilder result = new StringBuilder();

            var loginTimeStamp = DateTime.Now.Ticks % 100000;
            dbInfo.Login += loginTimeStamp;

            using (QueryExecutor executor = new MySqlExecutor(_masterConnectionString))
            {
                string createLoginQuery = $"CREATE USER '{dbInfo.Login}'@'%' IDENTIFIED BY '{dbInfo.Password}';";
                string grantPermissionsQuery = $"GRANT ALL PRIVILEGES ON {_dataBaseName}.* TO '{dbInfo.Login}'@'%';";
                string flushQuery = "FLUSH PRIVILEGES;";
                string createDbQuery = $"CREATE DATABASE {_dataBaseName}";

                result.Append(executor.ExecuteQueryAsString(createLoginQuery));
                result.Append(executor.ExecuteQueryAsString(createDbQuery));
                result.Append(executor.ExecuteQueryAsString(grantPermissionsQuery));
                result.Append(executor.ExecuteQueryAsString(flushQuery));
            }

            if (result.ToString() != string.Empty)
                throw new Exception(result.ToString());

            return $"Server={LocalSqlServerName}; Database={_dataBaseName}; Uid={dbInfo.Login}; Pwd={dbInfo.Password};";
        }
    }
}
