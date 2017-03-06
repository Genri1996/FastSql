using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string CreateNewDatabaseWithRandomLogin()
        {
            var login = "login" + DateTime.Now.Ticks;
            var password = "pass" + DateTime.Now.Ticks % 10000 * 37;

            return CreateNewDatabaseWithProtection(login, password);
        }

        public string CreateNewDatabaseWithProtection(string login, string password)
        {
            //errors collector
            StringBuilder result = new StringBuilder();

            using (QueryExecutor executor = new MySqlExecutor(_masterConnectionString))
            {
                string createLoginQuery = $"CREATE USER '{login}'@'{LocalSqlServerName}' IDENTIFIED BY '{password}';";
                string grantPermissionsQuery = $"GRANT ALL PRIVILEGES ON {_dataBaseName} . * TO '{login}'@'{LocalSqlServerName}';";
                string flushQuery = "FLUSH PRIVILEGES;";
                string createDbQuery = $"CREATE DATABASE {_dataBaseName}";

                result.Append(executor.ExecuteQueryAsString(createLoginQuery));
                result.Append(executor.ExecuteQueryAsString(grantPermissionsQuery));
                result.Append(executor.ExecuteQueryAsString(flushQuery));
                result.Append(executor.ExecuteQueryAsString(createDbQuery));
            }

            if (result.ToString() != string.Empty)
                throw new Exception(result.ToString());

            return $"Server={LocalSqlServerName};Database={_dataBaseName};Uid={login};Pwd={password};";
        }
    }
}
