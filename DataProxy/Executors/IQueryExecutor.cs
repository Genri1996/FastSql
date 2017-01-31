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
        DataTable ExecuteQueryAsDataTable(String command);
        String ExecuteQueryAsString(String command);
        String ExecuteCommandAsString(SqlCommand command);
        DataTable ExecuteCommandAsDataTable(SqlCommand command);
    }
}
