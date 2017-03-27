using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using DataProxy.DataBaseReaders;
using DataProxy.Executors;
using DataProxy.Helpers;

namespace DataProxy.DbManangment
{
    /// <summary>
    /// Manages the list of user`s databases.
    /// </summary>
    public static class DataBasesManager
    {
        private const string DbInfosTableName = "DbInfos";
        private const string AnonDbInfosTableName = "AnonDbInofs";
        private const string AspNetUserTable = "AspNetUsers";
        private const string DbName = "FastSqlIdentity";

        /// <summary>
        /// Creates table {DbInfosTableName} if it dies nit exists
        /// </summary>
        public static void CreateTablesIfNotExists()
        {
            //TODO: Do not use bicycle. Use real check for tables persistence.
            //Try to read the table. if success, than table exists, else - no
            CreateDbInfosIfNoExists();
            CreateAnonDbInfosIfNoExists();
        }

        /// <summary>
        /// Returns list of user`s database, orienting his ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<DataBaseInfo> GetDbInfos(string userId)
        {
            //TODO Add public, add anonymous
            OdbcDataBaseReader reader =
                new OdbcDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString, DbmsType.SqlServer);
            DataSet set = reader.LoadTables(DbInfosTableName, AspNetUserTable);

            //Selects DBInfos, that belongs to user
            var result = (from dbInfo in set.Tables[DbInfosTableName].AsEnumerable()
                          where dbInfo.Field<string>("USERKEY") == userId
                          select new DataBaseInfo
                          {
                              Id = dbInfo.Field<int>("Id"),
                              Name = dbInfo.Field<string>("NAME"),
                              ConnectionString = dbInfo.Field<string>("CONNECTIONSTRING"),
                              DateOfCreating = (DateTime)dbInfo["DATEOFCREATING"],
                              IsAnonymous = false,
                              DbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), dbInfo.Field<string>("DBMSTYPE")),
                              Login = dbInfo.Field<string>("LOGIN"),
                              Password = dbInfo.Field<string>("PASSWORD"),
                              ForeignKey = dbInfo.Field<string>("USERKEY")
                          }).ToList();

            return result;
        }

        public static List<DataBaseInfo> GetAnonymousDbInfos()
        {
            OdbcDataBaseReader reader =
                new OdbcDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString, DbmsType.SqlServer);
            DataSet set = reader.LoadTables(AnonDbInfosTableName);

            //Selects DBInfos, that belongs to user
            var result = (from dbInfo in set.Tables[DbInfosTableName].AsEnumerable()
                          select new DataBaseInfo
                          {
                              Id = dbInfo.Field<int>("Id"),
                              Name = dbInfo.Field<string>("NAME"),
                              ConnectionString = dbInfo.Field<string>("CONNECTIONSTRING"),
                              DateOfCreating = (DateTime)dbInfo["DATEOFCREATING"],
                              DateOfDeleting = (DateTime)dbInfo["DATEOFDELETING"],
                              Login = dbInfo.Field<string>("LOGIN"),
                              Password = dbInfo.Field<string>("PASSWORD"),
                              IsAnonymous = true,
                              DbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), dbInfo.Field<string>("DBMSTYPE"))
                          }).ToList();

            return result;
        }

        /// <summary>
        /// Adds record about database to DbInfos.
        /// </summary>
        /// <param name="info"></param>
        public static void AddDbInfo(DataBaseInfo info)
        {
            string query = $"USE {DbName} INSERT INTO {DbInfosTableName} "
                           + "(NAME, DATEOFCREATING, CONNECTIONSTRING, DBMSTYPE, LOGIN, PASSWORD, USERKEY) "
                           +
                           $"VALUES('{info.Name}', CONVERT(DATETIME, '{info.DateOfCreating.ToString("yyyy-MM-dd HH:mm:ss")}', 120), "
                           + $"'{info.ConnectionString}','{info.DbmsType}', '{info.Login}', '{info.Password}', '{info.ForeignKey}');";

            using (
                SqlServerExecutor executor =
                    new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
            {
                executor.ExecuteQueryAsString(query);
            }
        }

        public static void AddAnonymousDbInfo(DataBaseInfo info)
        {
            string query = $"USE {DbName} INSERT INTO {AnonDbInfosTableName} "
                           + "(NAME, DATEOFCREATING, DATEOFDELETING, CONNECTIONSTRING, DBMSTYPE, LOGIN, PASSWORD) "
                           +
                           $"VALUES('{info.Name}', CONVERT(DATETIME, '{info.DateOfCreating.ToString("yyyy-MM-dd HH:mm:ss")}', 120), "
                           + $"CONVERT(DATETIME, '{info.DateOfDeleting.ToString("yyyy-MM-dd HH:mm:ss")}', 120),"
                           + $"'{info.ConnectionString}','{info.DbmsType}', '{info.Login}', '{info.Password}');";

            using (
                SqlServerExecutor executor =
                    new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
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
            string query = $"USE {DbName}  DELETE FROM {DbInfosTableName} WHERE DbInfos.Id={info.Id};";

            using (
                SqlServerExecutor executor =
                    new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
            {
                executor.ExecuteQueryAsString(query);
            }
        }

        public static void RemoveAnonymousDbInfo(DataBaseInfo info)
        {
            string query = $"USE {DbName}  DELETE FROM {AnonDbInfosTableName} WHERE Id={info.Id};";

            using (
                SqlServerExecutor executor =
                    new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
            {
                executor.ExecuteQueryAsString(query);
            }
        }

        public static void DropOutdatedDbs()
        {
            try
            {
                OdbcDataBaseReader reader = new OdbcDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString, DbmsType.SqlServer);
                DataSet set = reader.LoadTables(AnonDbInfosTableName);

                var result = from dbRecord in set.Tables[AnonDbInfosTableName].AsEnumerable()
                             let dateOfDeleting = (DateTime)dbRecord["DATEOFDELETING"]
                             where dateOfDeleting < DateTime.Now
                             select new DataBaseInfo
                             {
                                 Name = dbRecord.Field<string>("NAME"),
                                 Id = dbRecord.Field<int>("ID"),
                                 DateOfDeleting = (DateTime)dbRecord["DATEOFDELETING"]
                             };

                //TODO: add additional servers
                IHelper helper = new SqlServerHelper();
                foreach (DataBaseInfo dataBaseInfo in result)
                {
                    RemoveAnonymousDbInfo(dataBaseInfo);
                    helper.DropDataBase(dataBaseInfo);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Checks existance and creates if required
        /// </summary>
        private static void CreateDbInfosIfNoExists()
        {
            OdbcDataBaseReader reader = new OdbcDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString, DbmsType.SqlServer);
            try
            {
                reader.LoadTables(DbInfosTableName);
            }
            // if table DbInfos does not exist
            catch (OdbcException)
            {
                using (
                    SqlServerExecutor executor =
                        new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
                {
                    string query = $"USE {DbName} CREATE TABLE {DbInfosTableName} ("
                                   + "ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY, "
                                   + "NAME NVARCHAR(200) NOT NULL, "
                                   + "DATEOFCREATING DATETIME NOT NULL, "
                                   + "CONNECTIONSTRING NVARCHAR(500) NOT NULL, "
                                   + "USERKEY NVARCHAR(128), "
                                   + "DBMSTYPE NVARCHAR(128), "
                                   + "LOGIN NVARCHAR(128), "
                                   + "PASSWORD NVARCHAR(128), "
                                   //Restrictions
                                   + "FOREIGN KEY (USERKEY) REFERENCES dbo.AspNetUsers(Id)"
                                   + ")";

                    executor.ExecuteQueryAsString(query);
                }
            }
        }
        private static void CreateAnonDbInfosIfNoExists()
        {
            OdbcDataBaseReader reader = new OdbcDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString, DbmsType.SqlServer);
            try
            {
                reader.LoadTables(AnonDbInfosTableName);
            }
            // if table {AnonDbInfosTableName} does not exist
            catch (OdbcException)
            {
                using (
                    SqlServerExecutor executor =
                        new SqlServerExecutor(ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString))
                {
                    string query = $"USE {DbName} CREATE TABLE {AnonDbInfosTableName} ("
                                   + "ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY, "
                                   + "NAME NVARCHAR(200) NOT NULL, "
                                   + "DATEOFCREATING DATETIME NOT NULL, "
                                   + "DATEOFDELETING DATETIME NOT NULL, "
                                   + "CONNECTIONSTRING NVARCHAR(500) NOT NULL, "
                                   + "DBMSTYPE NVARCHAR(128), "
                                   + "LOGIN NVARCHAR(128), "
                                   + "PASSWORD NVARCHAR(128)"
                                   + ")";

                    executor.ExecuteQueryAsString(query);
                }
            }
        }


    }
}