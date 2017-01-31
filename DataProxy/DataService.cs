using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProxy.Creators;
using DataProxy.Executors;
using DataProxy.Helpers;
using DataProxy.Models;

namespace DataProxy
{
    public static class DataService
    {
        public static List<DbmsType> AvailableServers { get; } = new List<DbmsType> { DbmsType.SqlServer };

        public static String CreateDatabase(CreateDatabaseObject obj)
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
