using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProxy.Executors
{
    /// <summary>
    /// Provides interface for raw queries
    /// </summary>
    public abstract class QueryExecutor:IDisposable
    {
        protected DbConnection _connection;

        /// <summary>
        /// Executes string quey and returns datatable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract DataTable ExecuteQueryAsDataTable(String command);
        /// <summary>
        /// Executes string quey and returns string
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract String ExecuteQueryAsString(String command);
        /// <summary>
        /// Executes SqlCommand and returns string
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract String ExecuteCommandAsString(SqlCommand command);
        /// <summary>
        /// Executes SqlCommand and returns DataTable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract DataTable ExecuteCommandAsDataTable(SqlCommand command);

        protected void Open()
        {
            try
            {
                _connection.Open();
            }
            catch (SqlException ex)
            {
                _connection.Dispose();
                throw new Exception("Unable to open connection.", ex);
            }
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
