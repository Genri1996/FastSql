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
    interface IQueryExecutor
    {
        String ExecuteQuery(String query);
        DataTable ExecuteQuery(SqlCommand command);
    }
}
