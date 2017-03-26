using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;

namespace CursachPrototype.ModelHelpers
{
    public interface IModelProvider
    {
        OdbcDataBaseReader GetDataBaseReader(DataBaseInfo dbInfo);
    }
}