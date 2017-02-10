using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataProxy.DataBaseReaders
{
    /// <summary>
    /// Provides an ability to load several tables from ANY Dbms.
    /// </summary>
    public class OleDbDataBaseReader : IDataBaseReader
    {
        private readonly OleDbConnection _connection;

        /// <summary>
        /// Opens connection immediately
        /// </summary>
        /// <param name="connectionString"></param>
        public OleDbDataBaseReader(string connectionString)
        {
            _connection = new OleDbConnection(connectionString);
            _connection.Open();
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

        public KeyValuePair<DataSet, OleDbDataAdapter> LoadTablesWithAdapter(params string[] tableNames)
        {
            DataSet ds = new DataSet();
            StringBuilder query = new StringBuilder();

            //Form query
            foreach (var tableName in tableNames)
                query.Append($" SELECT * FROM {tableName};");

            OleDbDataAdapter adapter = new OleDbDataAdapter(query.ToString(), _connection);

            //Adding mapping with table names
            for (int i = 0; i < tableNames.Length; i++)
                adapter.TableMappings.Add("Table" + (i != 0 ? i.ToString() : ""), tableNames[i]);

            adapter.Fill(ds);

            return new KeyValuePair<DataSet, OleDbDataAdapter>(ds, adapter);
        } 

        /// <summary>
        /// Returns names of all tables from selected Db.
        /// </summary>
        /// <returns></returns>
        private string[] GetTableNames()
        {
            // Get the data table containing the schema
            DataTable dt = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] tableNames = (from DataRow row in dt.Rows
                                   let strSheetTableName = row["TABLE_NAME"].ToString()
                                   where row["TABLE_TYPE"].ToString() == "TABLE"
                                   select row["TABLE_NAME"].ToString()).ToArray();

            return tableNames;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}