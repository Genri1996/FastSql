using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using DataProxy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CursachPrototype.ExtensionMethods;

namespace CursachPrototype.Controllers
{
    /// <summary>
    /// Handles databases actions
    /// </summary>
    public class CsDispatcherController : Controller
    {
        /// <summary>
        /// Proxy to work with databases indirectly
        /// </summary>
        private readonly DataService _dataService = new DataService();
        /// <summary>
        /// Reference to current user.
        /// </summary>
        private AppUserManager _userManager
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<AppUserManager>();
            }
        }

        /// <summary>
        /// Returns menu for creating database in some selected DBMS
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult Index()
        {
            ViewBag.FromInfo = "Необходимо войти для создания персональной базы данных";

            ServerSelectionVm vm = new ServerSelectionVm
            {
                AvailableServers = GetAvailableDbmsAsListString()
            };
         
            return View(vm);
        }

        /// <summary>
        /// Creates Database according to user`vm selection
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult CreateDb(ServerSelectionVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();//Почему не приходит из представления??

            if (!ModelState.IsValid)
                return View("Index", vm);

            //Add prefix
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            vm.DataBaseName += "_" + user.UserNickName;

            //Check Db Exists
            DbmsType selectedDbm = (DbmsType)Enum.Parse(typeof (DbmsType), vm.SelectedServer);
            if (_dataService.CheckDataBaseExists(selectedDbm, vm.DataBaseName))
            {
                string errorMessage = "База данных с таким именем уже существует в вашем профиле.";
                ModelState.AddModelError("err", errorMessage);
                return View("Index", vm);
            }

            //Add new info to user
            user.UserDbs.Add(new DataBaseInfo { Name = vm.DataBaseName, DateOfCreating = DateTime.Now});
            _userManager.Update(user);

            String connectionString = _dataService.CreateDatabase(vm.ToCreateDatabaseObject());
            ViewBag.ConnectionString = connectionString;
            return View("ShowConnectionString", (object)connectionString);
        }

        private List<String> GetAvailableDbmsAsListString()
        {
            var list = new List<String>();
            foreach (var dbmsType in _dataService.AvailableServers)
                list.Add(dbmsType.ToString());

            return list;
        }
    }
}