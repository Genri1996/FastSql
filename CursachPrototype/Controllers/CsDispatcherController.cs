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
    public class CsDispatcherController : Controller
    {
        private readonly DataService _dataService = new DataService();

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.FromInfo = "Необходимо войти для создания персональной базы данных";
            return View(new ServerSelectionVm {AvailableServers = _dataService.AvailableServers});
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateDb(ServerSelectionVm s)
        {
            s.AvailableServers = _dataService.AvailableServers;
            AppUser user =
                System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<AppUserManager>()
                    .FindById(User.Identity.GetUserId());
            s.DataBaseName += "_" + user.UserDbSuffix;

            if (!ModelState.IsValid)
                return View("Index", s);

           

            if (_dataService.CheckDataBaseExists(s.SelectedServer, s.DataBaseName))//!Переписать
            {
                string errorMessage = "База данных с таким именем уже существует в вашем профиле.";
                return View("CustomError", errorMessage);
            }

            user.UserDbs.Add(new DataBaseInfo { Name = s.DataBaseName});

            String connectionString = _dataService.GetConnectionString(s.SelectedServer, s.DataBaseName);
            ViewBag.ConnectionString = connectionString;
            return View("ShowConnectionString", (object) connectionString);
        }
    }
}