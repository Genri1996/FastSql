namespace DataProxy.Helpers
{
    /// <summary>
    /// Different additional  functions
    /// </summary>
    public interface IHelper
    {
        bool IsDataBaseExists(string dbName);

        bool DropDataBase(string dbName);

        bool IsLoginExists(string login);
    }
}
