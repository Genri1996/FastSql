using DataProxy.DbManangment;

namespace DataProxy.Creators
{
    /// <summary>
    /// Helps To Create database
    /// </summary>
    interface IDbCreator
    {
        /// <summary>
        /// Creates database and returns connection string
        /// </summary>
        /// <returns></returns>
        string CreateNewDatabaseWithRandomLogin(DataBaseInfo dbInfo);
        
        /// <summary>
        /// Creates database and returns connection string with preotection
        /// </summary>
        /// <returns></returns>
        string CreateNewDatabaseWithProtection(DataBaseInfo dbInfo);
    }
}
