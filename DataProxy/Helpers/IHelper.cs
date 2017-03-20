using DataProxy.DbManangment;

namespace DataProxy.Helpers
{
    /// <summary>
    /// Different additional  functions
    /// </summary>
    public interface IHelper
    {
        bool IsDataBaseExists(DataBaseInfo dbInfo);

        bool DropDataBase(DataBaseInfo dbInfo);

        bool IsLoginExists(string login);
    }
}
