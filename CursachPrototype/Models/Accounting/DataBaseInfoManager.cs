using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using DataProxy;
using DataProxy.DataBaseReaders;
using DataProxy.Executors;

namespace CursachPrototype.Models.Accounting
{
    public static class DataBaseInfoManager
    {
        private const string TableName = "DbInfos";
        private const string AspNetUserTable = "AspNetUsers";
        private const string DbName = "FastSqlIdentity";

        public static void CreateTableIfNotExists()
        {
            OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);

            try
            {
                reader.LoadTables(TableName);
            }
            // if table DbInfos exists
            catch (OleDbException)
            {
                using (SqlServerExecutor executor = new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
                {

                    string query = $"use {DbName} CREATE TABLE {TableName} ("
                                   + "Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, "
                                   + "NAME NVARCHAR(200) NOT NULL, "
                                   + "DATEOFCREATING DATE NOT NULL, "
                                   + "CONNECTIONSTRING NVARCHAR(500) NOT NULL, "
                                   + "USERKEY NVARCHAR(128), "
                                   + "FOREIGN KEY (USERKEY) REFERENCES dbo.AspNetUsers(Id)"
                                   + ")";

                    executor.ExecuteQueryAsString(query);
                }
            }
        }

        public static List<DataBaseInfo> GetDbInfos(AppUser user)
        {
            OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            DataSet set = reader.LoadTables(TableName, AspNetUserTable);

            var result = (from dbInfo in set.Tables[TableName].AsEnumerable()
                          where dbInfo.Field<string>("USERKEY") == user.Id
                          select new DataBaseInfo
                          {
                              Id = dbInfo.Field<int>("Id"),
                              ConnectionString = dbInfo.Field<string>("CONNECTIONSTRING"),
                              DateOfCreating = dbInfo.Field<DateTime>("DATEOFCREATING"),
                              ForeignKey = dbInfo.Field<string>("USERKEY"),
                              Name = dbInfo.Field<string>("NAME")
                          }).ToList();

            return result;
        }

        public static void AddDbInfo(DataBaseInfo info, AppUser user)
        {
            string query = $"USE {DbName} INSERT INTO {TableName} "
                           + "(NAME, DATEOFCREATING, CONNECTIONSTRING, USERKEY) "
                           + $"VALUES('{info.Name}', CONVERT(DATETIME, '{info.DateOfCreating.ToString("yyyy-MM-dd hh:mm:ss")}', 120), '{info.ConnectionString}', '{user.Id}');";

            using ( SqlServerExecutor executor = new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
            {
                executor.ExecuteQueryAsString(query);
            }
        }

        public static void RemoveDbInfo(DataBaseInfo info) { }
    }
}