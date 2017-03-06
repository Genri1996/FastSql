using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataProxy.Executors
{
    /// <summary>
    /// Provides an ability to execute query to SQL server
    /// </summary>
    public class SqlServerExecutor : QueryExecutor
    {
        /// <summary>
        /// Opens connection imeddiately.
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlServerExecutor(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            Open();
        }

        /// <summary>
        /// Executes string query and return result as datatable.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override DataTable ExecuteQueryAsDataTable(string command)
        {
            SqlCommand cmd = new SqlCommand(command);
            return ExecuteCommandAsDataTable(cmd);
        }

        /// <summary>
        /// Executes string query and return result as string.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string ExecuteQueryAsString(string command)
        {
            SqlCommand cmd = new SqlCommand(command);
            return ExecuteCommandAsString(cmd);
        }

        /// <summary>
        /// Executes SqlCommand query and return result as string.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override string ExecuteCommandAsString(SqlCommand command)
        {
            StringBuilder builder = new StringBuilder();
            command.Connection = _connection as SqlConnection;

            //Result collector
            SqlDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.VisibleFieldCount; i++)
                        builder.AppendLine(reader[i] + " ");
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
                reader?.Dispose();
                command.Dispose();
            }
            return builder.ToString();
        }

        /// <summary>
        /// Executes SqlCommand query and return result as datatable.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override DataTable ExecuteCommandAsDataTable(SqlCommand command)
        {
            DataTable dt = new DataTable();
            command.Connection = _connection as SqlConnection;
            SqlDataAdapter sda = new SqlDataAdapter();
            command.CommandType = CommandType.Text;
            try
            {
                sda.SelectCommand = command;
                sda.Fill(dt);
                return dt;
            }
            catch (SqlException e)
            {
                var errorDataColumn = new DataColumn("FastSqlQueryErrMessages", typeof(string));
                dt.Columns.Add(errorDataColumn);
                foreach (SqlError sqlError in e.Errors)
                {
                    var row = dt.NewRow();
                    row[0] = sqlError.Message;
                    dt.Rows.Add(row);
                }

                return dt;
            }
            finally
            {
                sda.Dispose();
            }
        }
    }
}
