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
        string CreateNewDatabaseWithRandomLogin();
        
        /// <summary>
        /// Creates database and returns connection string with preotection
        /// </summary>
        /// <returns></returns>
        string CreateNewDatabaseWithProtection(string login, string password);
    }
}
