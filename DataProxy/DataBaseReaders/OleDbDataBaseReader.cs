using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace DataProxy.DataBaseReaders
{
    public class OleDbDataBaseReader : IDataBaseReader
    {
        private readonly OleDbConnection _connection;

        public OleDbDataBaseReader(string connectionString)
        {
            _connection = new OleDbConnection(connectionString);
            _connection.Open();
        }

        public DataSet LoadWholeDataBase()
        {
            string[] tableNames = GetTableNames();
            return LoadTables(tableNames);
        }

        public DataSet LoadTables(params string[] tableNames)
        {
            DataSet ds = new DataSet();
            StringBuilder query = new StringBuilder();

            foreach (var tableName in tableNames)
            {
                query.Append(" SELECT * FROM " + tableName + ";");
            }

            OleDbDataAdapter adapter = new OleDbDataAdapter(query.ToString(), _connection);
            adapter.Fill(ds);
            
            return ds;
        }

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