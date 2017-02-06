using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net.Sockets;
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
            OleDbDataBaseReader reader =
                new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            DataSet set = reader.LoadTables(DbInfosTableName, AspNetUserTable);

            //Selects DBInfos, that belongs to user
            var result = (from dbInfo in set.Tables[DbInfosTableName].AsEnumerable()
                          where dbInfo.Field<string>("USERKEY") == userId
                          select new DataBaseInfo
                          {
                              Id = dbInfo.Field<int>("Id"),
                              Name = dbInfo.Field<string>("NAME"),
                              ConnectionString = dbInfo.Field<string>("CONNECTIONSTRING"),
                              DateOfCreating = dbInfo.Field<DateTime>("DATEOFCREATING"),
                              IsAnonymous = false,
                              DbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), dbInfo.Field<string>("DBMSTYPE")),
                              ForeignKey = dbInfo.Field<string>("USERKEY")
                          }).ToList();

            return result;
        }

        public static List<DataBaseInfo> GetAnonymousDbInfos()
        {
            OleDbDataBaseReader reader =
                new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            DataSet set = reader.LoadTables(AnonDbInfosTableName);

            //Selects DBInfos, that belongs to user
            var result = (from dbInfo in set.Tables[DbInfosTableName].AsEnumerable()
                          select new DataBaseInfo
                          {
                              Id = dbInfo.Field<int>("Id"),
                              Name = dbInfo.Field<string>("NAME"),
                              ConnectionString = dbInfo.Field<string>("CONNECTIONSTRING"),
                              DateOfCreating = dbInfo.Field<DateTime>("DATEOFCREATING"),
                              DateOfDeleting = dbInfo.Field<DateTime>("DATEOFDELETING"),
                              IsAnonymous = true,
                              DbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), dbInfo.Field<string>("DBMSTYPE"))
                          }).ToList();

            return result;
        }

        /// <summary>
        /// Adds record about database to DbInfos.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        public static void AddDbInfo(DataBaseInfo info)
        {
            string query = $"USE {DbName} INSERT INTO {DbInfosTableName} "
                           + "(NAME, DATEOFCREATING, CONNECTIONSTRING, DBMSTYPE, USERKEY) "
                           +
                           $"VALUES('{info.Name}', CONVERT(DATETIME, '{info.DateOfCreating.ToString("yyyy-MM-dd hh:mm:ss")}', 120), "
                           + $"'{info.ConnectionString}','{info.DbmsType}', '{info.ForeignKey}');";

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
                           + "(NAME, DATEOFCREATING, DATEOFDELETING, CONNECTIONSTRING, DBMSTYPE) "
                           +
                           $"VALUES('{info.Name}', CONVERT(DATETIME, '{info.DateOfCreating.ToString("yyyy-MM-dd hh:mm:ss")}', 120), "
                           + $"CONVERT(DATETIME, '{info.DateOfDeleting.ToString("yyyy-MM-dd hh:mm:ss")}', 120),"
                           + $"'{info.ConnectionString}','{info.DbmsType}');";

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
            string query = $"USE {DbName}  DELETE FROM {AnonDbInfosTableName} WHERE DbInfos.Id={info.Id};";

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
                OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
                DataSet set = reader.LoadTables(AnonDbInfosTableName);

                var result = from dbRecord in set.Tables[AnonDbInfosTableName].AsEnumerable()
                             let dateOfDeleting = dbRecord.Field<DateTime>("DATEOFDELETEING")
                             where dateOfDeleting < DateTime.Now
                             select new DataBaseInfo
                             {
                                 Name = dbRecord.Field<string>("NAME"),
                                 Id = dbRecord.Field<int>("ID")
                             };

                //TODO: add additional servers
                IHelper helper = new SqlServerHelper();
                foreach (DataBaseInfo dataBaseInfo in result)
                {
                    RemoveAnonymousDbInfo(dataBaseInfo);
                    helper.DropDataBase(dataBaseInfo.Name);
                }
            }
            catch
            {

            }
        }

        public static string GetAutoName()
        {
            return $"db{DateTime.Now.Ticks}";
        }

        /// <summary>
        /// Checks existance and creates if required
        /// </summary>
        /// <param name="reader"></param>
        private static void CreateDbInfosIfNoExists()
        {
            OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            try
            {
                reader.LoadTables(DbInfosTableName);
            }
            // if table DbInfos does not exist
            catch (OleDbException)
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
                                   //Restrictions
                                   + "FOREIGN KEY (USERKEY) REFERENCES dbo.AspNetUsers(Id)"
                                   + ")";

                    executor.ExecuteQueryAsString(query);
                }
            }
        }
        private static void CreateAnonDbInfosIfNoExists()
        {
            OleDbDataBaseReader reader = new OleDbDataBaseReader(ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString);
            try
            {
                reader.LoadTables(AnonDbInfosTableName);
            }
            // if table {AnonDbInfosTableName} does not exist
            catch (OleDbException)
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
                                   + ")";

                    executor.ExecuteQueryAsString(query);
                }
            }
        }


    }
}