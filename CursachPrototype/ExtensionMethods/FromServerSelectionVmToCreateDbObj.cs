using System;
using CursachPrototype.ViewModels;
using DataProxy;
using DataProxy.Models;

namespace CursachPrototype.ExtensionMethods
{
    public static class FromServerSelectionVmToCreateDbObj
    {
        /// <summary>
        /// Converts CreateDbVm to ToCreateDatabaseObject. 
        /// For easier DB creation.
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="userNickName">User nickname. Need for unic db name</param>
        /// <returns></returns>
        public static CreateDatabaseObject ToCreateDatabaseObject(this CreateDbVm vm, string userNickName = null)
        {
            CreateDatabaseObject obj = new CreateDatabaseObject
            {
                //TODO: get rid of strings
                SelectedDbms = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer),
                DataBaseName = vm.DataBaseName + "_" + userNickName,//Adds suffix
                DataBaseLogin = userNickName
            };

            //if user wants private database
            if (!vm.IsDataBasePublic)
            {
                obj.IsProtectionRequired = true;
                obj.DataBasePassword = vm.DataBasePassword;
            }
            return obj;
        }
        public static CreateDatabaseObject ToAnonymousCreateDatabaseObject(this CreateDbVm vm)
        {
            CreateDatabaseObject obj = new CreateDatabaseObject
            {
                //TODO: get rid of strings
                SelectedDbms = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer),
                DataBaseName = vm.DataBaseName + "_" + "Anon",//Adds suffix
            };
            return obj;
        }

    }
}