using System;
using System.Collections.Generic;
using DataProxy.Creators;
using DataProxy.DbManangment;
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
        public static string CreateDatabase(DataBaseInfo obj, string login = null, string password = null)
        {
            IDbCreator creator = null;
            switch (obj.DbmsType)
            {
                case DbmsType.SqlServer:
                    creator = new SqlServerCreator(obj.Name);
                    break;
            }
            if (obj.IsPublic)
                return creator.CreateNewDatabase();
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                return creator.CreateNewDatabaseWithProtection(login, password);
            throw new ArgumentException("Lack of arguments.");
        }

        public static bool CheckDataBaseExists(DbmsType selectedDbms, String dataBaseName)
        {
            IHelper helper = null;
            switch (selectedDbms)
            {
                case DbmsType.SqlServer:
                    helper = new SqlServerHelper();
                    break;
            }

            return helper.IsDataBaseExists(dataBaseName);
        }

        public static bool DropDataBase(DbmsType selectedDbms, String dataBaseName)
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


    }
}
