using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        /// Reference to current user.
        /// </summary>
        private AppUserManager _userManager => System.Web.HttpContext.Current.GetOwinContext()
            .GetUserManager<AppUserManager>();

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

            //Receives info
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            CreateDatabaseObject tempObj = vm.ToCreateDatabaseObject(user.UserNickName);

            //Check Db Exists
            DbmsType selectedDbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer);
            if (DataService.CheckDataBaseExists(selectedDbmsType, tempObj.DataBaseName))
            {
                string errorMessage = "База данных с таким именем уже существует в этой СУБД.";
                ModelState.AddModelError("err", errorMessage);
                return View("Index", vm);
            }

            //Try to create database
            String connectionString;
            try
            {
                connectionString = DataService.CreateDatabase(vm.ToCreateDatabaseObject(user.UserNickName));
            }
            catch (Exception)
            {
                string errorMessage = "Не удалось создать базу данных.";
                return View("CustomError", (object)errorMessage);
            }

            //Save action to database
            //Add new info to user. No ID and foreighn Key (!)
            DataBaseInfoManager.AddDbInfo(new DataBaseInfo { Name = tempObj.DataBaseName, DateOfCreating = DateTime.Now, ConnectionString = connectionString }, user);
            _userManager.Update(user);

            return View("ShowConnectionString", (object)connectionString);
        }

        //TODO: Use Enum List instead
        /// <summary>
        /// Returns all available Dbms As String List.
        /// </summary>
        /// <returns></returns>
        private List<string> GetAvailableDbmsAsListString()
        {
            return DataService.AvailableServers.Select(dbmsType => dbmsType.ToString()).ToList();
        }
    }
}