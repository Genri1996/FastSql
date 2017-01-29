using System;
using System.Collections.Generic;
using System.Configuration;
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

        public SqlServerHelper()
        {
            _masterConnectionString = ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString;
        }

        public bool IsDataBaseExists(string dbName)
        {
            string strQuery = $"use master select [dbo].[DatabaseExists]('{dbName}') as [exists]";
            DataTable dt;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                 dt = executor.ExecuteQueryAsDataTable(strQuery);
            }
            return (bool)dt.Rows[0]["exists"];
        }

        public bool DropDataBase(string dbName)
        {
            string strQuery = @" USE master; ALTER DATABASE[@databasename] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE[@databasename]";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@databasename", dbName);

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                executor.ExecuteCommandAsDataTable(cmd);
            }   
            //TODO Доделать

            return false;
        }

        public bool IsLoginExists(string login)
        {
            string strQuery = $"use master select [dbo].[LoginExists]('{login}') as [exists]";
            DataTable dt;

            using (SqlServerExecutor executor = new SqlServerExecutor(_masterConnectionString))
            {
                dt = executor.ExecuteQueryAsDataTable(strQuery);
            }
            return (bool)dt.Rows[0]["exists"];

        }
    }
}
