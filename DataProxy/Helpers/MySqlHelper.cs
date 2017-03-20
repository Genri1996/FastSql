using System.Configuration;
using System.Data;
using DataProxy.DbManangment;
using DataProxy.Executors;

namespace DataProxy.Helpers
{
    public class MySqlHelper : IHelper
    {
        private readonly string _masterConnectionString;

        public MySqlHelper()
        {
            _masterConnectionString = ConfigurationManager.ConnectionStrings["MySqlMaster"].ConnectionString;
        }

        public bool IsDataBaseExists(DataBaseInfo dbInf)
        {
            string checkExistanceQuery =
                $"SELECT count(*) as 'exists' FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{dbInf.Name}'";

            DataTable dt;
            using (QueryExecutor executor = new MySqlExecutor(_masterConnectionString))
            {
                dt = executor.ExecuteQueryAsDataTable(checkExistanceQuery);
            }

            return (long)dt.Rows[0][0] != 0;
        }

        public bool DropDataBase(DataBaseInfo dbInfo)
        {
            string strQuery = $"DROP DATABASE {dbInfo.Name}";
            string deleteUser = $"DROP USER '{dbInfo.Login}'@'%';";
            string queryResult;
            using (MySqlExecutor executor = new MySqlExecutor(_masterConnectionString))
            {
                queryResult = executor.ExecuteQueryAsString(strQuery);
                executor.ExecuteQueryAsString(deleteUser);
            }
            if (queryResult != string.Empty)
                return false;

            return true;
        }

        public bool IsLoginExists(string login)
        {
            throw new System.NotImplementedException();
        }
    }
}