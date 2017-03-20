using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CursachPrototype.ViewModels;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryGenerators
{
    public class MySqlServerTableEditor:TableEditor
    {
        public MySqlServerTableEditor(DataBaseInfo dbInf) : base(dbInf)
        {
        }

        public override string DeleteTable(string tableName)
        { 
            throw new NotImplementedException();
        }

        public override string InsertNewColumn(CreateColumnVm vm)
        {
            throw new NotImplementedException();
        }

        public override string DropColumn(DeleteColumnVm vm)
        {
            throw new NotImplementedException();
        }
    }
}