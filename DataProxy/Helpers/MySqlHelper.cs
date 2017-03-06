namespace DataProxy.Helpers
{
    public class MySqlHelper : IHelper
    {
        public bool IsDataBaseExists(string dbName)
        {
            string checkExistanceQuery =
                $"SELECT count(*) as 'exists' FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{dbName}'";
            throw new System.NotImplementedException();
        }

        public bool DropDataBase(string dbName)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLoginExists(string login)
        {
            throw new System.NotImplementedException();
        }
    }
}