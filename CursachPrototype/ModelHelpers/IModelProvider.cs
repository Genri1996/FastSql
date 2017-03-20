using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;

namespace CursachPrototype.ModelHelpers
{
    public interface IModelProvider
    {
        OleDbDataBaseReader GetDataBaseReader(DataBaseInfo dbInfo);
    }
}