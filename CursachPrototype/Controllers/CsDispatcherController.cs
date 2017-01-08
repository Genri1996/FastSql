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
        private readonly DataService ds = new DataService();

        [HttpGet]
        public ActionResult Index()
        {
            return View(new ServerSelectionVM {AvailableServers = ds.AvailableServers});
        }

        [HttpPost]
        public ActionResult CreateDb(ServerSelectionVM s)
        {
            String connectionString = ds.GetConnectionString(s.SelectedServer, s.DataBaseName);
            ViewBag.ConnectionString = connectionString;
            return View("ShowConnectionString", (object)connectionString);
        }


    }
}