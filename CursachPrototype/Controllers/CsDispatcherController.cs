using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.CustomAttributes;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using DataProxy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CursachPrototype.ExtensionMethods;
using DataProxy.DbManangment;

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
        public ActionResult CreateProtectedDb()
        {
            ProtectedDbVm vm = new ProtectedDbVm
            {
                AvailableServers = GetAvailableDbmsAsListString()
            };
            return View(vm);
        }

        /// <summary>
        /// /// <summary>
        /// Returns menu for creating anonymous database in some selected DBMS
        /// </summary>
        /// <returns></returns>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateAnonymousDb()
        {
            AnonymousDbVm vm = new AnonymousDbVm
            {
                AvailableServers = GetAvailableDbmsAsListString()
            };
            return View(vm);
        }

        [Authorize]
        public ActionResult ShowConnectionString()
        {
            string cs =
                DataBasesManager.GetDbInfos(User.Identity.GetUserId())
                    .OrderByDescending(db => db.DateOfCreating)
                    .First()
                    .ConnectionString;

            return PartialView("ShowConnectionString", cs);
        }

        [Authorize]
        public ActionResult GoToEditingProtected()
        {
            var id =
               DataBasesManager.GetDbInfos(User.Identity.GetUserId())
                   .OrderByDescending(db => db.DateOfCreating)
                   .First()
                   .Id;

            return RedirectToAction("Index","DisplayTable", new { dbId = id});
        }

        /// <summary>
        /// Creates Database according to user`s vm selection
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult CreateProtectedDb(ProtectedDbVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();

            if (!ModelState.IsValid)
                return View(vm);

            //Receives info
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            var tempDbInfo = vm.ToCreateDatabaseObject(user);

            //Check Db Exists
            DbmsType selectedDbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer);
            if (DataService.CheckDataBaseExists(selectedDbmsType, tempDbInfo.Name))
            {
                string errorMessage = "База данных с таким именем уже существует в этой СУБД.";
                ModelState.AddModelError("err", errorMessage);
                return View(vm);
            }

            //Try to create database
            try
            {
                tempDbInfo.ConnectionString = DataService.CreateDatabase(tempDbInfo, GetHostAddress(), user.UserNickName, vm.DataBasePassword);
            }
            catch (Exception e)
            {
                return View("CustomError", (object)e.ToString());
            }
            //Save action to database
            //AddRecord new info to user. No ID and foreighn Key (!)
            DataBasesManager.AddDbInfo(tempDbInfo);
            _userManager.Update(user);

            return View("RequestConnectionString");
        }

        [HttpPost]
        public ActionResult CreateAnonymousDb(AnonymousDbVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();

            if (!ModelState.IsValid)
                return View(vm);

            var tempObj = vm.ToCreateDatabaseObject();
            //Check Db Exists
            DbmsType selectedDbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer);
            if (DataService.CheckDataBaseExists(selectedDbmsType, tempObj.Name))
            {
                string errorMessage = "База данных с таким именем уже существует в этой СУБД.";
                ModelState.AddModelError("err", errorMessage);
                return View(vm);
            }

            //Try to create database
            String connectionString;
            try
            {
                connectionString = DataService.CreateDatabase(tempObj, GetHostAddress());
            }
            catch (Exception e)
            {
                return View("CustomError", (object)e.ToString());
            }
            tempObj.ConnectionString = connectionString;
            DataBasesManager.AddAnonymousDbInfo(tempObj);

            ViewBag.IsAnon = true;
            return View("RequestConnectionString", (object)connectionString);
        }

        //TODO: Use Enum List instead
        /// <summary>
        /// Returns all available Dbms As String List.
        /// </summary>
        /// <returns></returns>
        private List<string> GetAvailableDbmsAsListString()
        {   var availableServers = DataService.AvailableServers.Select(dbmsType => dbmsType.ToString()).ToList();
            availableServers.Add("MySql");
            return availableServers;
        }

        private string GetHostAddress()
        {
            var req = $"{Request.Url.Authority}";
            if (req.Contains("localhost"))
                return @"LOCALHOST";
            return
                req;
        }
    }
}