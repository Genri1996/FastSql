using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProxy.Executors
{
    public class SqlServerExecutor : IQueryExecutor
    {
        private readonly String _connectionString;
        public SqlServerExecutor(String connectionString)
        {
            _connectionString = connectionString;
        }

        public string ExecuteQuery(string query)
        {
            StringBuilder builder = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.VisibleFieldCount; i++)
                                builder.AppendLine(reader[i] + " ");
                            builder.AppendLine();
                        }
                    else
                        while (reader.Read())
                        {
                            builder.AppendLine(reader[0] + " ");
                            builder.AppendLine();
                        }
                }
                catch (SqlException exception)
                {
                    builder.AppendLine(exception.Message);
                }
                finally
                {
                    reader?.Close();
                }
            }
            return builder.ToString();
        }
    }
}
