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
            return View(new ServerSelectionVm { AvailableServers = _dataService.AvailableServers });
        }

        /// <summary>
        /// Creates Database according to user`s selection
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult CreateDb(ServerSelectionVm s)
        {
            s.AvailableServers = _dataService.AvailableServers;

            if (!ModelState.IsValid)
                return View("Index", s);

            //Add prefix
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            s.DataBaseName += "_" + user.UserDbSuffix;


            if (_dataService.CheckDataBaseExists(s.SelectedServer, s.DataBaseName))
            {
                string errorMessage = "База данных с таким именем уже существует в вашем профиле.";
                ModelState.AddModelError("err", errorMessage);
                return View("Index", s);
            }

            user.UserDbs.Add(new DataBaseInfo { Name = s.DataBaseName, DateOfCreating = DateTime.Now});
            
            _userManager.Update(user);

            String connectionString = _dataService.GetConnectionString(s.SelectedServer, s.DataBaseName);
            ViewBag.ConnectionString = connectionString;
            return View("ShowConnectionString", (object)connectionString);
        }
    }
}