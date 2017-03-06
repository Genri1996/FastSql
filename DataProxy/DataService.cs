using System;
using System.Collections.Generic;
using DataProxy.Creators;
using DataProxy.DbManangment;
using DataProxy.Executors;
using DataProxy.Helpers;

namespace DataProxy
{
    /// <summary>
    /// Mediator between some functions and client.
    /// Used for DBMS independence.
    /// </summary>
    public static class DataService
    {
        public static List<DbmsType> AvailableServers { get; } = new List<DbmsType> { DbmsType.SqlServer };

        /// <summary>
        /// Creates DB according CDBObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string CreateDatabase(DataBaseInfo obj, string connectionString, string login = null, string password = null)
        {
            IDbCreator creator = null;
            string localHostName = null;
            switch (obj.DbmsType)
            {
                case DbmsType.SqlServer:
                    creator = new SqlServerCreator(obj.Name);
                    localHostName = ".\\SQLEXPRESS";
                    break;
                case DbmsType.MySql:
                    creator = new MySqlCreator(obj.Name);
                    localHostName = "localhost";
                    break;
            }
            string cs;
            if (obj.IsPublic)
                cs = creator.CreateNewDatabaseWithRandomLogin();
            else if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                cs = creator.CreateNewDatabaseWithProtection(login, password);
            else
                throw new ArgumentException("Lack of arguments.");


            cs = cs.Replace("SERVERNAME", string.Equals(connectionString, "LOCALHOST") ? localHostName : connectionString);
            return cs;
        }

        public static bool CheckDataBaseExists(DbmsType selectedDbms, string dataBaseName)
        {
            IHelper helper = null;
            switch (selectedDbms)
            {
                case DbmsType.SqlServer:
                    helper = new SqlServerHelper();
                    break;
                case DbmsType.MySql:
                    helper = new MySqlHelper();
                    break;
            }

            return helper.IsDataBaseExists(dataBaseName);
        }

        public static bool DropDataBase(DbmsType selectedDbms, string dataBaseName)
        {
            IHelper helper = null;
            switch (selectedDbms)
            {
                case DbmsType.SqlServer:
                    helper = new SqlServerHelper();
                    break;
            }

            return helper.DropDataBase(dataBaseName);
        }

        public static string ExecuteQuery(string query, string connectionString, DbmsType type)
        {
            QueryExecutor executor = null;
            switch (type)
            {
                case DbmsType.SqlServer:
                    executor = new SqlServerExecutor(connectionString);
                    break;
            }

            return executor.ExecuteQueryAsString(query);
        }
    }
}
