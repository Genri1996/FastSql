using CursachPrototype.ViewModels;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryHelpers
{
    class SqlServerHelper : IHelper
    {
        private CreateColumnVm _cvm;
        private readonly DataBaseInfo _dbInf;

        public SqlServerHelper(DataBaseInfo dbInf)
        {
            _dbInf = dbInf;
        }

        public string InsertNewColumn(CreateColumnVm vm)
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

            var result = DataProxy.DataService.ExecuteQuery(query, _dbInf.ConnectionString, _dbInf.DbmsType);

            if (!string.IsNullOrWhiteSpace(result))
                return result;

            //Adding default value if neccesary
            var defaultQueryResult = SetDefaultIfRequired();

            if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                return defaultQueryResult;

            return string.Empty;
        }

        private string SetDefaultIfRequired()
        {
            if (!string.IsNullOrWhiteSpace(_cvm.DefaultValue))
            {
                var defaultQuery = $"ALTER TABLE {_cvm.TableName} ADD DEFAULT ";
                if (_cvm.TypeName == "String")
                    defaultQuery += "'" + _cvm.DefaultValue + "'";
                else
                    defaultQuery += _cvm.DefaultValue;

                defaultQuery += $"FOR {_cvm.ColumnName}";

                var defaultQueryResult = DataProxy.DataService.ExecuteQuery(defaultQuery, _dbInf.ConnectionString,
                    _dbInf.DbmsType);
                if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                {
                    DropColumn(new DeleteColumnVm {ColumnName = _cvm.ColumnName, TableName = _dbInf.Name});
                    return defaultQueryResult;
                }
            }
            return string.Empty;
        }

        public string DropColumn(DeleteColumnVm vm)
        {
            string dropQuery = $"DECLARE @ConstraintName nvarchar(200) " +
                               $"SELECT @ConstraintName = Name " +
                               $"FROM SYS.DEFAULT_CONSTRAINTS " +
                               $"WHERE PARENT_OBJECT_ID = OBJECT_ID('{vm.TableName}') " +
                               $"AND PARENT_COLUMN_ID = (SELECT column_id " +
                               $"FROM sys.columns " +
                               $"WHERE NAME = N'{vm.ColumnName}' AND object_id = OBJECT_ID(N'{vm.TableName}')) " +
                               $"IF @ConstraintName IS NOT NULL " +
                               $"EXEC('ALTER TABLE {vm.TableName} DROP CONSTRAINT ' + @ConstraintName) " +
                               $"ALTER TABLE {vm.TableName} DROP COLUMN {vm.ColumnName}";
            return DataProxy.DataService.ExecuteQuery(dropQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }
    }
}