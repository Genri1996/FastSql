using System;
using System.Collections.Generic;
using DataProxy.Creators;
using DataProxy.Helpers;
using DataProxy.Models;

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
        public static string CreateDatabase(CreateDatabaseObject obj)
        {
            IDbCreator creator = null;
            switch (obj.SelectedDbms)
            {
                case DbmsType.SqlServer:
                    creator = new SqlServerCreator(obj.DataBaseName);
                    break;
            }
            if (!obj.IsProtectionRequired)
                return creator.CreateNewDatabase();
            else
                return creator.CreateNewDatabaseWithProtection(obj.DataBaseLogin, obj.DataBasePassword);
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
