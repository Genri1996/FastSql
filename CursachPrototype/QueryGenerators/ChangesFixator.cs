using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using CursachPrototype.ExtensionMethods;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryGenerators
{
    public abstract class ChangesFixator
    {
        protected readonly DataBaseInfo _dbInfo;
        protected readonly DataTable _dataTable;
        private readonly IDictionary<string, string> _parametrsList = new Dictionary<string, string>();
        private int _columnIndex;
        private const string InvalidValue = "INVALID";

        protected ChangesFixator(DataBaseInfo dbInfo, DataTable dataTable)
        {
            _dbInfo = dbInfo;
            _dataTable = dataTable;
        }

        public string FixateChanges(QueryType queryType, int rowId = 0)
        {
            var primaryKeyRowName = GetIdOrdinalIndex();
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
            return DataProxy.DataService.ExecuteQueryAsString(query, _dbInfo.ConnectionString, _dbInfo.DbmsType);
        }

        public void AddDataRowColumnValue(string value)
        {
            if (value == null)
                return;

            value = value.Trim();

            if (_columnIndex == _dataTable.Columns.Count)
                _columnIndex = 0;

            var column = _dataTable.Columns[_columnIndex];
            var columnDataType = column.DataType;

            if (string.IsNullOrWhiteSpace(value) && HasColumnDefaultValue(column))
            {
                _parametrsList.Add(column.ColumnName, InvalidValue);
                _columnIndex++;
                return;
            }

            if (columnDataType == typeof(int) || columnDataType == typeof(double) || columnDataType == typeof(float))
            {
                if (value == string.Empty)
                {
                    _parametrsList.Add(column.ColumnName, InvalidValue);
                    _columnIndex++;
                    return;
                }

                _parametrsList.Add(column.ColumnName, value.Replace(',', '.'));
            }
            else if (columnDataType == typeof(DateTime))
            {
                if (value == string.Empty || !value.Match(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{2,})?$"))
                {
                    _parametrsList.Add(column.ColumnName, InvalidValue);
                    _columnIndex++;
                    return;
                }

                var dateTime = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
                string val = GetFormattedDateForQuery(dateTime);
                _parametrsList.Add(column.ColumnName, val);
            }
            else//Unrecognized type or string, send as string
            {
                _parametrsList.Add(column.ColumnName, "'" + value + "'");
            }

            _columnIndex++;
        }

        protected abstract string GetFormattedDateForQuery(DateTime dateTime);

        private string GenerateDeleteQuery(int primaryKeyRowId, int rowId)
        {
            return $"USE {_dbInfo.Name}; DELETE FROM {_dataTable.TableName} WHERE {_dataTable.Columns[primaryKeyRowId].ColumnName} = {rowId}";
        }

        private string GenerateUpdateQuery(int primaryKeyRowId, int rowId)
        {
            StringBuilder updateQueryBuilder = new StringBuilder($"USE {_dbInfo.Name}; UPDATE {_dataTable.TableName} SET ");
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
            updateQueryBuilder.Append($" WHERE {_dataTable.Columns[primaryKeyRowId].ColumnName} = {rowId}");
            return updateQueryBuilder.ToString();
        }

        private string GenerateInsertQuery()
        {
            StringBuilder insertQueryBuilder = new StringBuilder($"USE {_dbInfo.Name}; INSERT INTO {_dataTable.TableName} ");

            StringBuilder columnsBuilder = new StringBuilder("(");
            StringBuilder valuesBuilder = new StringBuilder("(");

            bool first = true;
            foreach (var keyValuePair in _parametrsList)
            {
                if (keyValuePair.Value == InvalidValue)
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
            insertQueryBuilder.Append(" VALUES ");
            insertQueryBuilder.Append(valuesBuilder);

            return insertQueryBuilder.ToString();
        }

        protected abstract bool HasColumnDefaultValue(DataColumn column);
        
        private int GetIdOrdinalIndex()
        {
            return _dataTable.Columns.Cast<DataColumn>()
              .First(column => column.ColumnName.ContainsIgnoreCase("Id")).Ordinal;
        }

    }
}
