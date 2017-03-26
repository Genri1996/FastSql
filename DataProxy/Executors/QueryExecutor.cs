using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DataProxy.Executors
{
    /// <summary>
    /// Provides interface for raw queries
    /// </summary>
    public abstract class QueryExecutor : IDisposable
    {
        /// <summary>
        /// Executes string quey and returns datatable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataTable ExecuteQueryAsDataTable(string command)
        {
            DbCommand cmd = GetCommand(command);
            return ExecuteCommandAsDataTable(cmd);
        }

        /// <summary>
        /// Executes string quey and returns string
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string ExecuteQueryAsString(string command)
        {
            DbCommand cmd = GetCommand(command);
            return ExecuteCommandAsString(cmd);
        }

        /// <summary>
        /// Executes SqlCommand and returns string
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string ExecuteCommandAsString(DbCommand command)
        {
            StringBuilder builder = new StringBuilder();
            command.Connection = Connection;

            //Result collector
            DbDataReader reader = null;
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
            catch (Exception exception)
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
        /// Executes SqlCommand and returns DataTable
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataTable ExecuteCommandAsDataTable(DbCommand command)
        {
            DataTable dt = new DataTable();
            command.Connection = Connection;
            DbDataAdapter sda = DataAdapter;
            sda.SelectCommand = command;
            command.CommandType = CommandType.Text;
            try
            {
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
            catch (MySqlException e)
            {
                var errorDataColumn = new DataColumn("FastSqlQueryErrMessages", typeof(string));
                dt.Columns.Add(errorDataColumn);
                var row = dt.NewRow();
                row[0] = e.Message;
                dt.Rows.Add(row);

                return dt;
            }
            finally
            {
                sda.Dispose();
            }
        }

        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }

        protected abstract DbConnection Connection { get; set; }

        protected abstract DbDataAdapter DataAdapter { get; set; }

        protected abstract DbCommand GetCommand(string command);

        protected void Open()
        {
            try
            {
                Connection.Open();
            }
            catch (SqlException ex)
            {
                Connection.Dispose();
                throw new Exception("Unable to open connection.", ex);
            }
        }


    }
}
