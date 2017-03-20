using System;
using System.Data;
using DataProxy.DbManangment;
using DataProxy.Executors;

namespace CursachPrototype.QueryGenerators
{
    public class SqlServerChangesFixator : ChangesFixator
    {
        public SqlServerChangesFixator(DataBaseInfo dbInfo) : base(dbInfo)
        {
        }

        protected override string GetFormattedDateForQuery(DateTime dateTime)
        {
            return $"CONVERT(DATETIME, '{dateTime.ToString("yyyy-MM-dd hh:mm:ss")}')";
        }

        protected override bool HasColumnDefaultValue(DataColumn column)
        {
            string query =
                $"SELECT COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{_dataTable.TableName}' AND COLUMN_NAME = '{column.ColumnName}'";
            DataTable dtResult;
            QueryExecutor exe;
            using (exe = new SqlServerExecutor(_dbInfo.ConnectionString))
            {
                dtResult = exe.ExecuteQueryAsDataTable(query);
            }
            return dtResult.Rows[0][0].GetType() != typeof(DBNull);
        }
    }
}