using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProxy.Creators;
using DataProxy.Executors;

namespace DataProxy
{
    public class DataService
    {
        private const string SqlServer = "MS SQL Server";

        private const string MasterConnectionString =
            "Data Source=AMDFXPC\\SQLEXPRESS;Initial Catalog=master;Integrated security=True";

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

        public bool CheckDataBaseExists(String selectedServer, String dataBaseName)
        {
            IQueryExecutor executor = null;
            switch (selectedServer)
            {
                case SqlServer:
                    executor = new SqlServerExecutor(MasterConnectionString);
                    break;
            }

            string strQuery = @"select [dbo].[DatabaseExists]('@databasename') as [exists]";

            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@databasename", dataBaseName);
            DataTable dt = executor.ExecuteQuery(cmd);

            return (bool)dt.Rows[0]["exists"];
        }
    }
}
