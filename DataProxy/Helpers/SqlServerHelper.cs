using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProxy.Executors;

namespace DataProxy.Helpers
{
    /// <summary>
    /// Required stored procedures: DatabaseExists
    /// </summary>
    public class SqlServerHelper:IHelper
    {
        private string _masterConnectionString;

        public SqlServerHelper(string masterConnectionString)
        {
            _masterConnectionString = masterConnectionString;
        }

        public bool IsDataBaseExists(string dbName)
        {
            SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString);
            string strQuery = @"select [dbo].[DatabaseExists]('@databasename') as [exists]";

            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@databasename", dbName);
            DataTable dt = executor.ExecuteQuery(cmd);

            return (bool)dt.Rows[0]["exists"];
        }

        public bool DropDataBase(string dbName)
        {
            SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString);
            string strQuery = @" USE master; ALTER DATABASE[@databasename] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE[@databasename]";

            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@databasename", dbName);
            executor.ExecuteQuery(cmd);

            return false;
        }
    }
}
