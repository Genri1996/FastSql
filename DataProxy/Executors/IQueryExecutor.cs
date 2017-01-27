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
        String ExecuteQueryAsString(String command);
        DataTable ExecuteCommandAsDataTable(SqlCommand command);

        void Open();
    }
}
