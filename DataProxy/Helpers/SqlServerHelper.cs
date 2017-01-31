using System.Configuration;
using System.Data;
using DataProxy.Executors;

namespace DataProxy.Helpers
{
    /// <summary>
    /// Required stored procedures: DatabaseExists
    /// </summary>
    public class SqlServerHelper:IHelper
    {
        private readonly string _masterConnectionString;

        public SqlServerHelper()
        {
            _masterConnectionString = ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString;
        }

        /// <summary>
        /// Checks if database exists
        /// </summary>
        /// <param name="dbName">name of database</param>
        /// <returns></returns>
        public bool IsDataBaseExists(string dbName)
        {
            string strQuery = $"USE MASTER SELECT [dbo].[DatabaseExists]('{dbName}') AS [exists]";
            DataTable dt;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                 dt = executor.ExecuteQueryAsDataTable(strQuery);
            }
            return (bool)dt.Rows[0]["exists"];
        }

        /// <summary>
        /// Drops database from SQL server
        /// </summary>
        /// <param name="dbName">Name of database</param>
        /// <returns></returns>
        public bool DropDataBase(string dbName)
        {
            string strQuery = $" USE MASTER ALTER DATABASE[{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE[{dbName}];";

            string queryResult;
            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                queryResult=executor.ExecuteQueryAsString(strQuery);
            }
            if (queryResult != string.Empty)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if login exists
        /// </summary>
        /// <param name="login">login name</param>
        /// <returns></returns>
        public bool IsLoginExists(string login)
        {
            string strQuery = $"USE MASTER SELECT [dbo].[LoginExists]('{login}') AS [exists]";
            DataTable dt;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                dt = executor.ExecuteQueryAsDataTable(strQuery);
            }
            return (bool)dt.Rows[0]["exists"];
        }
    }
}
