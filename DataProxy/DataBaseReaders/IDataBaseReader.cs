using System;
using System.Data;

namespace DataProxy.DataBaseReaders
{
    public interface IDataBaseReader:IDisposable
    {
        DataSet LoadWholeDataBase();
        DataSet LoadTables(params string[] tableNames);
    }
}