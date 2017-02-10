//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DataProxy.Executors;
//using DataProxy.Model;

//namespace DataProxy.DataPickers
//{
//    class SqlServerDataPicker : IDataPicker
//    {
//        private SqlServerExecutor _executor;
//        private string _dataBaseName;

//        public SqlServerDataPicker(string connectionString, string dataBaseName)
//        {
//            _dataBaseName = dataBaseName;
//            _executor = new SqlServerExecutor(connectionString);
//        }

//        public List<ColumnMetadata> GetTableMetadata()
//        {
//            string query = $"use {_dataBaseName} ";
//            query += @"SELECT TABLE_CATALOG as DataBaseName,
//                                            TABLE_NAME as TableName,
//                                            COLUMN_NAME as ColumnName,
//                                            ORDINAL_POSITION as OrdinalPosition,
//                                            COLUMN_DEFAULT as ColumnDefaultValue,
//                                            IS_NULLABLE as IsNullable,
//                                            DATA_TYPE as ColumnType,
//                                            CHARACTER_MAXIMUM_LENGTH as MaximumCharacters
//                                            FROM INFORMATION_SCHEMA.COLUMNS";
//            DataTable result;

//            using (_executor)
//            {
//                result = _executor.ExecuteQueryAsDataTable(query);
//            }

//            result.
//        }
//    }
//}
