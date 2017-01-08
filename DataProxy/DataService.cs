using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProxy.Creators;

namespace DataProxy
{
    public class DataService
    {
        private const string SqlServer = "MS SQL Server";

        public List<String> AvailableServers { get; } = new List<string> { SqlServer };

        public String GetConnectionString(String selectedServer, String DataBaseName)
        {
            IDbCreator creator=null;
            switch (selectedServer)
            {
                case SqlServer:
                    creator = new SqlServerCreator(DataBaseName);
                    break;
            }
            return creator.CreateNewDatabase();
        }
    }
}
