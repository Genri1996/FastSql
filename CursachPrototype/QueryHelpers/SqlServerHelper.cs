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

        private string SetDefaultIfRequired()
        {
            if (_cvm.IsDefaultValueEnabled)
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

                var defaultQueryResult = DataProxy.DataService.ExecuteQuery(defaultQuery, _dbInf.ConnectionString,
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
                var defaultQuery = $"ALTER TABLE {_cvm.TableName} ALTER COLUMN ({_cvm.ColumnName}) ";

                if (string.Compare(_cvm.TypeName,"String")==0)
                {
                    defaultQuery += $"NVARCHAR({_cvm.TypeLength})";
                }else if (string.Compare(_cvm.TypeName, "String") == 0)
                {
                    defaultQuery +="INT"; 
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
                var defaultQueryResult = DataProxy.DataService.ExecuteQuery(defaultQuery, _dbInf.ConnectionString,
                    _dbInf.DbmsType);
                if (!string.IsNullOrWhiteSpace(defaultQueryResult))
                {
                    DropColumn(new DeleteColumnVm { ColumnName = _cvm.ColumnName, TableName = _cvm.TableName });
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

        public string CreateTable(string tableName)
        {
            string createTableQuery = $"CREATE TABLE {tableName} (ID int NOT NULL PRIMARY KEY)";
            return DataProxy.DataService.ExecuteQuery(createTableQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }

        public string DeleteTable(string tableName)
        {
            string dropConstraintQueryGenerator =
                $"SELECT " +
                $"'ALTER TABLE ' + OBJECT_SCHEMA_NAME(parent_object_id) " +
                $"+ '.[' + OBJECT_NAME(parent_object_id) + ']" +
                $" DROP CONSTRAINT ' + name FROM sys.foreign_keys" +
                $" WHERE referenced_object_id = object_id('{tableName}')";


            var dropConstraintQuery = DataProxy.DataService.ExecuteQuery(dropConstraintQueryGenerator, _dbInf.ConnectionString, _dbInf.DbmsType);
            if(!string.IsNullOrEmpty(dropConstraintQuery))
                DataProxy.DataService.ExecuteQuery(dropConstraintQuery, _dbInf.ConnectionString, _dbInf.DbmsType);

            string dropTableQuery = $"DROP TABLE {tableName}";
            return DataProxy.DataService.ExecuteQuery(dropTableQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }
    }
}