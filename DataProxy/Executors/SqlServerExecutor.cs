using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataProxy.Executors
{
    public class SqlServerExecutor : IQueryExecutor
    {
        private readonly SqlConnection _connection;

        public SqlServerExecutor(String connectionString)
        {
            _connection = new SqlConnection(connectionString);
            Open();
        }

        public void Open()
        {
            try
            {
                _connection.Open();
            }
            catch (SqlException ex)
            {
                _connection.Dispose();
                throw new Exception("Unable to open _connection.", ex);
            }
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public string ExecuteQueryAsString(String command)
        {
            StringBuilder builder = new StringBuilder();
            SqlCommand cmd = new SqlCommand(command, _connection);
            SqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
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
                reader?.Dispose();
                cmd.Dispose();
            }
            return builder.ToString();
        }

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
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                sda.Dispose();
            }
        }

        public String ExecuteCommandAsString(SqlCommand cmd)
        {
            return ExecuteQueryAsString(cmd.CommandText);
        }
    }
}
