using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataProxy.Executors
{
    /// <summary>
    /// Provides an ability to execute query to SQL server
    /// </summary>
    public class SqlServerExecutor : IQueryExecutor
    {
        private readonly SqlConnection _connection;

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
        public DataTable ExecuteQueryAsDataTable(string command)
        {
            SqlCommand cmd = new SqlCommand(command);
            return ExecuteCommandAsDataTable(cmd);
        }

        /// <summary>
        /// Executes string query and return result as string.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string ExecuteQueryAsString(string command)
        {
            SqlCommand cmd = new SqlCommand(command);
            return ExecuteCommandAsString(cmd);
        }

        /// <summary>
        /// Executes SqlCommand query and return result as string.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string ExecuteCommandAsString(SqlCommand command)
        {
            StringBuilder builder = new StringBuilder();
            command.Connection = _connection;

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
        public DataTable ExecuteCommandAsDataTable(SqlCommand command)
        {
            DataTable dt = new DataTable();
            command.Connection = _connection;
            SqlDataAdapter sda = new SqlDataAdapter();
            command.CommandType = CommandType.Text;
            try
            {
                sda.SelectCommand = command;
                sda.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                sda.Dispose();
            }
        }

        private void Open()
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
