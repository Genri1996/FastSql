using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CursachPrototype.ViewModels;
using DataProxy;
using DataProxy.Models;

namespace CursachPrototype.ExtensionMethods
{
    public static class FromServerSelectionVmToCreateDbObj
    {
        public static CreateDatabaseObject ToCreateDatabaseObject(this ServerSelectionVm vm)
        {
            CreateDatabaseObject obj = new CreateDatabaseObject
            {
                SelectedDbms = (DbmsType) Enum.Parse(typeof (DbmsType), vm.SelectedServer),
                DataBaseName = vm.DataBaseName,
                IsProtectionRequired = false
            };
            return obj;
        }
    }
}