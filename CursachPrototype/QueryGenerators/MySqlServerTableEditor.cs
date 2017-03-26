using System;
using CursachPrototype.ViewModels;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryGenerators
{
    public class MySqlServerTableEditor:TableEditor
    {
        public MySqlServerTableEditor(DataBaseInfo dbInf) : base(dbInf)
        {
        }

        public override string DeleteTable(string tableName)
        {
           string dropTableQuery = $"DROP TABLE {tableName}";
            return DataProxy.DataService.ExecuteQueryAsString(dropTableQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }

        public override string InsertNewColumn(CreateColumnVm vm)
        {
            _cvm = vm;
            string query = $"ALTER TABLE {_cvm.TableName} ADD {_cvm.ColumnName} ";
            string type = null;
            switch (_cvm.TypeName)
            {
                case "String":
                    type = $"nvarchar({_cvm.TypeLength})";
                    break;
                case "Integer":
                    type = $"int";
                    break;
                case "Double":
                    type = $"float({_cvm.TypeLength})";
                    break;
                case "DateTime":
                    type = "datetime";
                    break;
            }
            query += type;

            var result = DataProxy.DataService.ExecuteQueryAsString(query, _dbInf.ConnectionString, _dbInf.DbmsType);

            if (!string.IsNullOrWhiteSpace(result))
                return result;

            //Adding default value if neccesary
            var defaultQueryResult = SetDefaultIfRequired();

            if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                return defaultQueryResult;

            //Adding unique constraint
            var uniqueQueryResult = SetUniqueIfRequired();

            if (!string.IsNullOrWhiteSpace(uniqueQueryResult))
                return uniqueQueryResult;

            //Adding not null
            var notNullQueryResult = SetNotNullIfRequired();

            if (!string.IsNullOrWhiteSpace(notNullQueryResult))
                return notNullQueryResult;

            return string.Empty;
        }

        public override string DropColumn(DeleteColumnVm vm)
        {
            string dropQuery = $"ALTER TABLE {vm.TableName} DROP COLUMN {vm.ColumnName}";
            return DataProxy.DataService.ExecuteQueryAsString(dropQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }

        private string SetDefaultIfRequired()
        {
            if (_cvm.IsDefaultValueEnabled)
            {
                var defaultQuery = $"ALTER TABLE {_cvm.TableName} ALTER COLUMN {_cvm.ColumnName} SET DEFAULT ";
                if (_cvm.TypeName == "String")
                    defaultQuery += "'" + _cvm.DefaultValue + "'";
                else
                    defaultQuery += _cvm.DefaultValue;

                defaultQuery += ";";

                var defaultQueryResult = DataProxy.DataService.ExecuteQueryAsString(defaultQuery, _dbInf.ConnectionString,
                    _dbInf.DbmsType);
                if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                {
                    DropColumn(new DeleteColumnVm { ColumnName = _cvm.ColumnName, TableName = _cvm.TableName });
                    return defaultQueryResult;
                }
            }
            return string.Empty;
        }

        private string SetUniqueIfRequired()
        {
            if (_cvm.IsUnique)
            {
                var defaultQuery = $"ALTER TABLE {_cvm.TableName} ADD UNIQUE ({_cvm.ColumnName})";

                var defaultQueryResult = DataProxy.DataService.ExecuteQueryAsString(defaultQuery, _dbInf.ConnectionString,
                    _dbInf.DbmsType);

                if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                {
                    DropColumn(new DeleteColumnVm { ColumnName = _cvm.ColumnName, TableName = _cvm.TableName });
                    return defaultQueryResult;
                }
            }
            return string.Empty;
        }

        private string SetNotNullIfRequired()
        {
            if (_cvm.IsNotNull)
            {
                var defaultQuery = $"ALTER TABLE {_cvm.TableName} MODIFY ({_cvm.ColumnName}) ";

                if (string.Compare(_cvm.TypeName, "String") == 0)
                {
                    defaultQuery += $"NVARCHAR({_cvm.TypeLength})";
                }
                else if (string.Compare(_cvm.TypeName, "String") == 0)
                {
                    defaultQuery += "INT";
                }
                else if (string.Compare(_cvm.TypeName, "Double") == 0)
                {
                    defaultQuery += $"FLOAT({_cvm.TypeLength})";
                }
                else if (string.Compare(_cvm.TypeName, "DateTime") == 0)
                {
                    defaultQuery += "DATETIME";
                }

                defaultQuery += " NOT NULL";
                var defaultQueryResult = DataProxy.DataService.ExecuteQueryAsString(defaultQuery, _dbInf.ConnectionString,
                    _dbInf.DbmsType);
                if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                {
                    DropColumn(new DeleteColumnVm { ColumnName = _cvm.ColumnName, TableName = _cvm.TableName });
                    return defaultQueryResult;
                }
            }
            return string.Empty;
        }
    }
}