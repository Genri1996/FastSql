using CursachPrototype.ViewModels;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryGenerators
{
    public abstract class TableEditor
    {
        protected CreateColumnVm _cvm;
        protected readonly DataBaseInfo _dbInf;

        protected TableEditor(DataBaseInfo dbInf)
        {
            _dbInf = dbInf;
        }

        public string CreateTable(string tableName)
        {
            string createTableQuery = $"CREATE TABLE {tableName} (ID int NOT NULL PRIMARY KEY)";
            return DataProxy.DataService.ExecuteQueryAsString(createTableQuery, _dbInf.ConnectionString, _dbInf.DbmsType);
        }

        public abstract string DeleteTable(string tableName);
        public abstract string InsertNewColumn(CreateColumnVm vm);
        public abstract string DropColumn(DeleteColumnVm vm);
    }
}