using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;

namespace CursachPrototype.ModelHelpers
{
    public class SqlServerResolver:IModelProvider
    {
        public OleDbDataBaseReader GetDataBaseReader(DataBaseInfo dbInfo)
        {
            throw new NotImplementedException();
        }
    }
}