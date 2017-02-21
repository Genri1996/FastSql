using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CursachPrototype.ViewModels;
using DataProxy.DbManangment;

namespace CursachPrototype.QueryHelpers
{
    public interface IHelper
    {
        string InsertNewColumn(CreateColumnVm vm);
        string DropColumn(DeleteColumnVm vm);
        string CreateTable(string name);
        string DeleteTable(string tableName);
    }
}