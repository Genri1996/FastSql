using System;
using System.Collections.Generic;
using System.Data;
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
        public static List<DbmsType> AvailableServers { get; } = new List<DbmsType> { DbmsType.SqlServer, DbmsType.MySql };

        /// <summary>
        /// Creates DB according CDBObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DataBaseInfo CreateDatabase(DataBaseInfo obj, string connectionString)
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
                cs = creator.CreateNewDatabaseWithRandomLogin(obj);
            else if (!string.IsNullOrEmpty(obj.Login) && !string.IsNullOrEmpty(obj.Password))
                cs = creator.CreateNewDatabaseWithProtection(obj);
            else
                throw new ArgumentException("Lack of arguments.");


            cs = cs.Replace("SERVERNAME", string.Equals(connectionString, "LOCALHOST") ? localHostName : connectionString);
            obj.ConnectionString = cs;
            return obj;
        }

        public static bool CheckDataBaseExists(DataBaseInfo dbInfo)
        {
            IHelper helper = null;
            switch (dbInfo.DbmsType)
            {
                case DbmsType.SqlServer:
                    helper = new SqlServerHelper();
                    break;
                case DbmsType.MySql:
                    helper = new MySqlHelper();
                    break;
            }

            return helper.IsDataBaseExists(dbInfo);
        }

        public static bool DropDataBase(DataBaseInfo dbInfo)
        {
            IHelper helper = null;
            switch (dbInfo.DbmsType)
            {
                case DbmsType.SqlServer:
                    helper = new SqlServerHelper();
                    break;
                case DbmsType.MySql:
                    helper = new MySqlHelper();
                    break;
            }

            return helper.DropDataBase(dbInfo);
        }

        public static string ExecuteQueryAsString(string query, string connectionString, DbmsType type)
        {
            QueryExecutor executor = null;
            switch (type)
            {
                case DbmsType.SqlServer:
                    executor = new SqlServerExecutor(connectionString);
                    break;
                case DbmsType.MySql:
                    executor = new MySqlExecutor(connectionString);
                    break;
            }

            return executor.ExecuteQueryAsString(query);
        }

        public static DataTable ExecuteQueryAsDataTable(string query, string connectionString, DbmsType type)
        {
            QueryExecutor executor = null;
            switch (type)
            {
                case DbmsType.SqlServer:
                    executor = new SqlServerExecutor(connectionString);
                    break;
                case DbmsType.MySql:
                    executor = new MySqlExecutor(connectionString);
                    break;
            }
             
            return executor.ExecuteQueryAsDataTable(query);
        }

    }
}
