using System;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using DataProxy;
using DataProxy.DbManangment;

namespace CursachPrototype.ExtensionMethods
{
    public static class FromServerSelectionVmToCreateDbObj
    {
        /// <summary>
        /// Converts CreateProtectbleDbVm to ToCreateDatabaseObject. 
        /// For easier DB creation.
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="userNickName">User nickname. Need for unic db name</param>
        /// <returns></returns>
        public static DataBaseInfo ToCreateDatabaseObject(this DbVm vm, AppUser user = null)
        {
            DataBaseInfo obj = new DataBaseInfo
            {
                //TODO: get rid of strings
                DbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer),
                DateOfCreating = DateTime.Now,
                IsPublic = vm.IsPublic
            };

            //if user asks for privacy
            if (vm is ProtectedDbVm)
            {
                obj.IsPublic = false;
            }

            if (user != null)
            {
                obj.Name = vm.DataBaseName + "_" + user.UserNickName;
                obj.ForeignKey = user.Id;
            }

            //if user creates anonymous
            if (vm is AnonymousDbVm)
            {
                var anonVm = (AnonymousDbVm)vm;
                obj.IsAnonymous = true;
                obj.DateOfDeleting = DateTime.Now + new TimeSpan(anonVm.StoreHours, 0, 0);
                obj.Name = vm.DataBaseName + "_" + "Anon";

            }
            return obj;
        }
    }
}