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
        private readonly List<string> _parametrsList = new List<string>();
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

        public string FixateChanges(QueryType queryType)
        {
            string query=null;
            switch (queryType)
            {
                case QueryType.Insert:
                    query = GenerateInsertQuery();
                    break;
                case QueryType.Update:
                    query = GenerateUpdateQuery();
                    break;
                case QueryType.Delete:
                    query = GenerateDeleteQuery();
                    break;
            }

            return DataProxy.DataService.ExecuteQuery(query, _dbInfo.ConnectionString, _dbInfo.DbmsType);
        }

        private string GenerateDeleteQuery()
        {
            throw new NotImplementedException();
        }

        private string GenerateUpdateQuery()
        {
            throw new NotImplementedException();
        }

        private string GenerateInsertQuery()
        {
            StringBuilder insertStringBuilder = new StringBuilder($"use {_dbInfo.Name} Insert into {_dataTable.TableName} values (");

            bool first = true;
            foreach (string s in _parametrsList)
            {
                if (!first)
                {
                    insertStringBuilder.Append(",");
                    insertStringBuilder.Append(s);
                }
                else
                    insertStringBuilder.Append(s);
                first = false;
            }
            insertStringBuilder.Append(")");
            return insertStringBuilder.ToString();
        }

        public void AddDataColumn(string requestValue)
        {
            if (requestValue == null)
                return;

            requestValue = requestValue.Trim();

            if (_columnIndex == _dataTable.Columns.Count)
                _columnIndex = 0;

            var columnDataType = _dataTable.Columns[_columnIndex].DataType;

            if (columnDataType == typeof(int))
            {
                _parametrsList.Add(requestValue);
            }
            else//Unrecognized type, send as string
            {
                _parametrsList.Add("'" + requestValue + "'");
            }

            _columnIndex++;
        }
    }
}