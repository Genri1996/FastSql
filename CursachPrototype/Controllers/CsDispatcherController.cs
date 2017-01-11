using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.ViewModels;
using DataProxy;

namespace CursachPrototype.Controllers
{
    public class CsDispatcherController : Controller
    {
        private readonly DataService _dataService = new DataService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(new ServerSelectionVM {AvailableServers = _dataService.AvailableServers});
        }

        [HttpPost]
        public ActionResult CreateDb(ServerSelectionVM s)
        {
            s.AvailableServers = _dataService.AvailableServers;
            if (!ModelState.IsValid)
                return View("Index", s);

            //TODO: Валидация на существование уже этой базы данных

            String connectionString = _dataService.GetConnectionString(s.SelectedServer, s.DataBaseName);
            ViewBag.ConnectionString = connectionString;
            return View("ShowConnectionString", (object) connectionString);
        }
    }
}