using CursachPrototype.ViewModels;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryHelpers
{
    class SqlServerHelper : IHelper
    {
        private CreateColumnVm _vm;
        private DataBaseInfo _dbInf;

        public SqlServerHelper(CreateColumnVm vm, DataBaseInfo dbInf)
        {
            _vm = vm;
            _dbInf = dbInf;
        }

        public string InsertNewColumn()
        {
            string query = $"ALTER TABLE {_vm.TableName} ADD {_vm.ColumnName} ";
            string type = null;
            switch (_vm.TypeName)
            {
                case "String":
                    type = $"nvarchar({_vm.TypeLength})";
                    break;
                case "Integer":
                    type = $"int({_vm.TypeLength})";
                    break;
                case "Double":
                    type = $"float({_vm.TypeLength})";
                    break;
                case "DateTime":
                    type = $"datetime";
                    break;
            }
            query += type;

            if (!string.IsNullOrEmpty(_vm.Constraints))
                query += _vm.DefaultValue != string.Empty ? "," : "" + _vm.Constraints;

            var result = DataProxy.DataService.ExecuteQuery(query, _dbInf.ConnectionString, _dbInf.DbmsType);

            if (!string.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            if (!string.IsNullOrWhiteSpace(_vm.DefaultValue))
            {
                var defaultQuery = $"ALTER TABLE {_vm.TableName} ADD DEFAULT ";
                if (_vm.TypeName == "String")
                    defaultQuery += "N'" + _vm.DefaultValue + "'";
                else
                    defaultQuery += _vm.DefaultValue;

                defaultQuery += $"FOR {_vm.ColumnName}";

                var defaultQueryResult = DataProxy.DataService.ExecuteQuery(defaultQuery, _dbInf.ConnectionString,
                    _dbInf.DbmsType);
                if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                {
                    DropColumn();
                    return defaultQueryResult;
                }
            }
            return string.Empty;
        }

        public string DropColumn()
        {
            string dropQuery = $"ALTER TABLE {_vm.TableName} DROP COLUMN {_vm.ColumnName}";
            return DataProxy.DataService.ExecuteQuery(dropQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }
    }
}