using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProxy.Executors
{
    /// <summary>
    /// Provides interface for raw queries
    /// </summary>
    public interface IQueryExecutor:IDisposable
    {
        /// <summary>
        /// Executes string quey and returns datatable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        DataTable ExecuteQueryAsDataTable(String command);
        /// <summary>
        /// Executes string quey and returns string
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        String ExecuteQueryAsString(String command);
        /// <summary>
        /// Executes SqlCommand and returns string
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        String ExecuteCommandAsString(SqlCommand command);
        /// <summary>
        /// Executes SqlCommand and returns DataTable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        DataTable ExecuteCommandAsDataTable(SqlCommand command);
    }
}
