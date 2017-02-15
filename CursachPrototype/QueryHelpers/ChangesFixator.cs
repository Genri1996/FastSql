using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using CursachPrototype.ExtensionMethods;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryHelpers
{
    public class ChangesFixator
    {
        private readonly DataBaseInfo _dbInfo;
        private readonly DataTable _dataTable;
        private readonly IDictionary<string, string> _parametrsList = new Dictionary<string, string>();
        private int _columnIndex;
        private const string InvalidValue = "INVALID";

        public enum QueryType
        {
            Insert, Update, Delete
        }

        public ChangesFixator(DataBaseInfo dbInfo, DataTable dataTable)
        {
            _dbInfo = dbInfo;
            _dataTable = dataTable;
        }

        public string FixateChanges(QueryType queryType, int primaryKeyRowName = 0, int rowId = 0)
        {
            string query = null;
            switch (queryType)
            {
                case QueryType.Insert:
                    query = GenerateInsertQuery();
                    break;
                case QueryType.Update:
                    query = GenerateUpdateQuery(primaryKeyRowName, rowId);
                    break;
                case QueryType.Delete:
                    query = GenerateDeleteQuery(primaryKeyRowName, rowId);
                    break;
            }
            return DataProxy.DataService.ExecuteQuery(query, _dbInfo.ConnectionString, _dbInfo.DbmsType);
        }

        private string GenerateDeleteQuery(int primaryKeyRowId, int rowId)
        {
            return $"use {_dbInfo.Name} delete from {_dataTable.TableName} where {_dataTable.Columns[primaryKeyRowId].ColumnName} = {rowId}";
        }

        private string GenerateUpdateQuery(int primaryKeyRowId, int rowId)
        {
            StringBuilder updateQueryBuilder = new StringBuilder($"use {_dbInfo.Name} update {_dataTable.TableName} set ");
            bool first = true;
            foreach (KeyValuePair<string, string> keyValuePair in _parametrsList)
            {
                if (keyValuePair.Value == InvalidValue)
                    continue;
                if (!first)
                {
                    updateQueryBuilder.Append(",");
                    updateQueryBuilder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                }
                else
                    updateQueryBuilder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                first = false;
            }
            updateQueryBuilder.Append($" where {_dataTable.Columns[primaryKeyRowId].ColumnName} = {rowId}");
            return updateQueryBuilder.ToString();
        }

        private string GenerateInsertQuery()
        {
            StringBuilder insertQueryBuilder = new StringBuilder($"use {_dbInfo.Name} Insert into {_dataTable.TableName} ");

            StringBuilder columnsBuilder = new StringBuilder("(");
            StringBuilder valuesBuilder = new StringBuilder("(");

            bool first = true;
            foreach (var keyValuePair in _parametrsList)
            {
                if(keyValuePair.Value==InvalidValue)
                    continue;

                if (!first)
                {
                    columnsBuilder.Append(",");
                    valuesBuilder.Append(",");
                    columnsBuilder.Append(keyValuePair.Key);
                    valuesBuilder.Append(keyValuePair.Value);
                }
                else
                {
                    columnsBuilder.Append(keyValuePair.Key);
                    valuesBuilder.Append(keyValuePair.Value);
                }
                first = false;
            }
            columnsBuilder.Append(")");
            valuesBuilder.Append(")");

            insertQueryBuilder.Append(columnsBuilder);
            insertQueryBuilder.Append(" values ");
            insertQueryBuilder.Append(valuesBuilder);

            return insertQueryBuilder.ToString();
        }

        public void AddDataColumn(string requestValue)
        {
            if (requestValue == null)
                return;

            requestValue = requestValue.Trim();

            if (_columnIndex == _dataTable.Columns.Count)
                _columnIndex = 0;

            var column = _dataTable.Columns[_columnIndex];
            var columnDataType = column.DataType;

          
                

            if (columnDataType == typeof(int) || columnDataType == typeof(double) || columnDataType == typeof(float))
            {
                if (requestValue == string.Empty)
                {
                    _parametrsList.Add(column.ColumnName, InvalidValue);
                    _columnIndex++;
                    return;
                }

                _parametrsList.Add(column.ColumnName, requestValue.Replace(',','.'));
            }
            else if (columnDataType == typeof(DateTime))
            {
                if (requestValue == string.Empty || !requestValue.Match(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{2})?$"))
                {
                    _parametrsList.Add(column.ColumnName, InvalidValue);
                    _columnIndex++;
                    return;
                }

                var dateTime = XmlConvert.ToDateTime(requestValue, XmlDateTimeSerializationMode.Utc);
                string val = $"CONVERT(DATETIME, '{dateTime.ToString("yyyy-MM-dd hh:mm:ss")}')";//TODO: SQLServerSpecific
                _parametrsList.Add(column.ColumnName, val);
            }
            else//Unrecognized type or string, send as string
            {
                _parametrsList.Add(column.ColumnName, "'" + requestValue + "'");
            }

            _columnIndex++;
        }
    }
}