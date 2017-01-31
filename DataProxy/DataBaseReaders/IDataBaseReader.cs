using System;
using System.Data;

namespace DataProxy.DataBaseReaders
{
    /// <summary>
    /// DBMS Independent OLEDB Connection
    /// </summary>
    public interface IDataBaseReader:IDisposable
    {
        DataSet LoadWholeDataBase();
        DataSet LoadTables(params string[] tableNames);
    }
}