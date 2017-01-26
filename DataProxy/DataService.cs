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

namespace DataProxy
{
    public class DataService
    {
        private const string MasterConnectionString =
            "Data Source=AMDFXPC\\SQLEXPRESS;Initial Catalog=master;Integrated security=True";

        public List<DbmsType> AvailableServers { get; } = new List<DbmsType> { DbmsType.SqlServer };

        public String GetConnectionString(DbmsType selectedDbms, String DataBaseName)
        {
            IDbCreator creator=null;
            switch (selectedDbms)
            {
                case DbmsType.SqlServer:
                    creator = new SqlServerCreator(DataBaseName);
                    break;
            }
            return creator.CreateNewDatabase();
        }

        public bool CheckDataBaseExists(DbmsType selectedDbm, String dataBaseName)
        {
            IHelper helper = null;
            switch (selectedDbm)
            {
                case DbmsType.SqlServer:
                    helper = new SqlServerHelper(MasterConnectionString);
                    break;
            }

            return helper.IsDataBaseExists(dataBaseName);
        }
    }
}
