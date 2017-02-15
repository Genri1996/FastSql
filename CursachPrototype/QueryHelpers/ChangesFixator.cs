using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CursachPrototype.ExtensionMethods;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryHelpers
{
    public class ChangesFixator
    {
        private readonly DataBaseInfo _dbInfo;
        private readonly DataTable _dataTable;
        private readonly IDictionary<string, string> _parametrsList = new Dictionary<string, string>();
        private int _columnIndex = 0;

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
            foreach (KeyValuePair<string, string> s in _parametrsList)
            {
                if (!first)
                {
                    updateQueryBuilder.Append(",");
                    updateQueryBuilder.Append(s.Key + "=" + s.Value);
                }
                else
                    updateQueryBuilder.Append(s.Key + "=" + s.Value);
                first = false;
            }
            updateQueryBuilder.Append($" where {_dataTable.Columns[primaryKeyRowId].ColumnName} = {rowId}");
            return updateQueryBuilder.ToString();
        }

        private string GenerateInsertQuery()
        {
            StringBuilder insertQueryBuilder = new StringBuilder($"use {_dbInfo.Name} Insert into {_dataTable.TableName} values (");

            bool first = true;
            foreach (string s in _parametrsList.Values)
            {
                if (!first)
                {
                    insertQueryBuilder.Append(",");
                    insertQueryBuilder.Append(s);
                }
                else
                    insertQueryBuilder.Append(s);
                first = false;
            }
            insertQueryBuilder.Append(")");
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

            if (columnDataType == typeof(int))
            {
                _parametrsList.Add(column.ColumnName, requestValue);
            }
            else//Unrecognized type, send as string
            {
                _parametrsList.Add(column.ColumnName, "'" + requestValue + "'");
            }

            _columnIndex++;
        }
    }
}