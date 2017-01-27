using System;

namespace DataProxy.Creators
{
    interface IDbCreator
    {
        /// <summary>
        /// Creates database and returns connection string
        /// </summary>
        /// <returns></returns>
        String CreateNewDatabase();

        String CreateNewDatabaseWithProtection(String login, String password);
    }
}
