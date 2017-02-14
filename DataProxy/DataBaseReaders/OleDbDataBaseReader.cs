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
            connectionString = CheckProvider(connectionString);
            _connection = new OleDbConnection(connectionString);
            _connection.Open();
        }

        private string CheckProvider(string connectionString)
        {
            if (connectionString.Contains("SQLOLEDB"))
                return connectionString;
            return connectionString + ";Provider=SQLOLEDB;";
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
        public string[] GetTableNames()
        {
            // Get the data table containing the schema
            DataTable dt = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] tableNames = (from DataRow row in dt.Rows
                                   let strSheetTableName = row["TABLE_NAME"].ToString()
                                   where row["TABLE_TYPE"].ToString() == "TABLE"
                                   select row["TABLE_NAME"].ToString()).ToArray();

            return tableNames;
        }

        public void BatchInsertUpdate(DataTable table, OleDbCommand insertCommand)
        {
            int batchSize = 1;
            insertCommand.Connection = _connection;

            using (OleDbDataAdapter adapter = new OleDbDataAdapter())
            {
                adapter.InsertCommand = insertCommand;
                // Gets or sets the number of rows that are processed in each round-trip to the server.  
                // Setting it to 1 disables batch updates, as rows are sent one at a time.  
                adapter.UpdateBatchSize = batchSize;
                adapter.Update(table);
            }
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}