using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using NLog;

namespace DataProxy.DataBaseReaders
{
    /// <summary>
    /// Provides an ability to load several tables from ANY Dbms.
    /// </summary>
    public class OdbcDataBaseReader : IDataBaseReader
    {
        private readonly OdbcConnection _connection;

        /// <summary>
        /// Opens connection immediately
        /// </summary>
        /// <param name="connectionString"></param>
        public OdbcDataBaseReader(string connectionString, DbmsType dbType)
        {
            connectionString = CheckProvider(connectionString, dbType);
            _connection = new OdbcConnection(connectionString);

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(connectionString);

            _connection.Open();
        }

        private string CheckProvider(string connectionString, DbmsType dbType)
        {
            //For localhost conenctions to identityDb
            if (connectionString.Contains("Driver={SQL Server}"))
                return connectionString;
            if (dbType == DbmsType.SqlServer)
            {
                var temp = "Driver={SQL Server};";
                temp += connectionString;
                temp = temp.Replace("Data Source", "Server");
                temp = temp.Replace("Initial Catalog", "Database");
                temp = temp.Replace("User Id", "Uid");
                temp = temp.Replace("Password", "Pwd");
                return temp;
            }
            if (dbType == DbmsType.MySql)
            {
                var temp = "Driver={MySQL ODBC 5.3 ANSI Driver};";
                temp += "DSN=MySql;";
                temp += connectionString;
                temp += "Option=3;";
                temp = temp.Replace("Uid", "User");
                temp = temp.Replace("Pwd", "Password");
                return temp;
            }
            return null;
        }

        /// <summary>
        /// Loads whole database.
        /// </summary>
        /// <returns></returns>
        public DataSet LoadWholeDataBase()
        {
            string[] tableNames = GetTableNames();
            return LoadTables(tableNames);
        }

        /// <summary>
        /// Loads selecteed tables.
        /// </summary>
        /// <param name="tableNames">Names of tables to load</param>
        /// <returns></returns>
        public DataSet LoadTables(params string[] tableNames)
        {
            return LoadTablesWithAdapter(tableNames).Key;
        }

        public KeyValuePair<DataSet, OdbcDataAdapter> LoadTablesWithAdapter(params string[] tableNames)
        {
            DataSet ds = new DataSet();
            StringBuilder query = new StringBuilder();

            //Form query
            foreach (var tableName in tableNames)
                query.Append($" SELECT * FROM {tableName};");

            OdbcDataAdapter adapter = new OdbcDataAdapter(query.ToString(), _connection);

            //Adding mapping with table names 
            for (int i = 0; i < tableNames.Length; i++)
                adapter.TableMappings.Add("Table" + (i != 0 ? i.ToString() : ""), tableNames[i]);

            adapter.Fill(ds);

            return new KeyValuePair<DataSet, OdbcDataAdapter>(ds, adapter);
        }

        /// <summary>
        /// Returns names of all tables from selected Db.
        /// </summary>
        /// <returns></returns>
        public string[] GetTableNames()
        {
            // Get the data table containing the schema
            DataTable dt = _connection.GetSchema("Tables");
            string[] tableNames = (from DataRow row in dt.Rows
                                   let strSheetTableName = row["TABLE_NAME"].ToString()
                                   where row["TABLE_TYPE"].ToString() == "TABLE"
                                   select row["TABLE_NAME"].ToString()).ToArray();

            return tableNames
                .Where(
                    str => !(string.Equals(str, "trace_xe_event_map") || string.Equals(str, "trace_xe_action_map")))
                .ToArray();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}