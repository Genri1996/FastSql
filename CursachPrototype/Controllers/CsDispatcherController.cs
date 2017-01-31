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
using DataProxy.Models;

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
        /// Creates Database according to user`s vm selection
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult CreateDb(ServerSelectionVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();

            if (!ModelState.IsValid)
                return View("Index", vm);

            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            CreateDatabaseObject tempObj = vm.ToCreateDatabaseObject(user.UserNickName);
            //Check Db Exists
            DbmsType selectedDbm = (DbmsType)Enum.Parse(typeof (DbmsType), vm.SelectedServer);
            if (_dataService.CheckDataBaseExists(selectedDbm, tempObj.DataBaseName))
            {
                string errorMessage = "База данных с таким именем уже существует в вашем профиле.";
                ModelState.AddModelError("err", errorMessage);
                return View("Index", vm);
            }

            String connectionString;
            try
            {
                connectionString = _dataService.CreateDatabase(vm.ToCreateDatabaseObject(user.UserNickName));
            }
            catch (Exception e)
            {
                string errorMessage = "Не удалось создать базу данных.";
                return View("CustomError", (object)errorMessage);
            }

            //Add new info to user
            DataBaseInfoManager.AddDbInfo(new DataBaseInfo { Name = tempObj.DataBaseName, DateOfCreating = DateTime.Now, ConnectionString = connectionString}, user);
            _userManager.Update(user);

         
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