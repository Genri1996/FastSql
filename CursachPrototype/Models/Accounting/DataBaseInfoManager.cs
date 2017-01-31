using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using DataProxy;
using DataProxy.DataBaseReaders;
using DataProxy.Executors;

namespace CursachPrototype.Models.Accounting
{
    /// <summary>
    /// Manages the list of user`s databases.
    /// </summary>
    public static class DataBaseInfoManager
    {
        private const string TableName = "DbInfos";
        private const string AspNetUserTable = "AspNetUsers";
        private const string DbName = "FastSqlIdentity";

        /// <summary>
        /// Creates table {TableName} if it dies nit exists
        /// </summary>
        public static void CreateTableIfNotExists()
        {
            OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            //TODO: Do not use bicycle. Use real check for tavle persistence.
            //Try to read the table. if success, than table exists, else - no
            try
            {
                reader.LoadTables(TableName);
            }
            // if table DbInfos does not exist
            catch (OleDbException)
            {
                using (SqlServerExecutor executor = new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
                {

                    string query = $"USE {DbName} CREATE TABLE {TableName} ("
                                   + "ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY, "
                                   + "NAME NVARCHAR(200) NOT NULL, "
                                   + "DATEOFCREATING DATETIME NOT NULL, "
                                   + "CONNECTIONSTRING NVARCHAR(500) NOT NULL, "
                                   + "USERKEY NVARCHAR(128), "
                                   + "DBMSTYPE NVARCHAR(128), "
                                   //Restrictions
                                   + "FOREIGN KEY (USERKEY) REFERENCES dbo.AspNetUsers(Id)"
                                   + ")";

                    executor.ExecuteQueryAsString(query);
                }
            }
        }

        /// <summary>
        /// Returns list of user`s database, orienting his ID.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<DataBaseInfo> GetDbInfos(AppUser user)
        {
            OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            DataSet set = reader.LoadTables(TableName, AspNetUserTable);

            //Selects DBInfos, that belongs to user
            var result = (from dbInfo in set.Tables[TableName].AsEnumerable()
                          where dbInfo.Field<string>("USERKEY") == user.Id
                          select new DataBaseInfo
                          {
                              Id = dbInfo.Field<int>("Id"),
                              Name = dbInfo.Field<string>("NAME"),
                              ConnectionString = dbInfo.Field<string>("CONNECTIONSTRING"),
                              DateOfCreating = dbInfo.Field<DateTime>("DATEOFCREATING"),
                              DbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), dbInfo.Field<string>("DBMSTYPE")),
                              ForeignKey = dbInfo.Field<string>("USERKEY")
                          }).ToList();

            return result;
        }

        /// <summary>
        /// Adds record about database to DbInfos.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        public static void AddDbInfo(DataBaseInfo info, AppUser user)
        {
            string query = $"USE {DbName} INSERT INTO {TableName} "
                           + "(NAME, DATEOFCREATING, CONNECTIONSTRING, DBMSTYPE, USERKEY) "
                           + $"VALUES('{info.Name}', CONVERT(DATETIME, '{info.DateOfCreating.ToString("yyyy-MM-dd hh:mm:ss")}', 120), "
                           + $"'{info.ConnectionString}','{info.DbmsType}', '{user.Id}');";

            using (SqlServerExecutor executor = new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
            {
                executor.ExecuteQueryAsString(query);
            }
        }

        /// <summary>
        /// Removes record about user`s database from DbInfoes, usually when dropping table
        /// </summary>
        /// <param name="info"></param>
        public static void RemoveDbInfo(DataBaseInfo info)
        {
            string query = $"USE {DbName}  DELETE FROM {TableName} WHERE DbInfos.Id={info.Id};";

            using (SqlServerExecutor executor = new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
            {
                executor.ExecuteQueryAsString(query);
            }
        }
    }
}