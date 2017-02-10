using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataProxy.DataBaseReaders;

namespace CursachPrototype.Controllers
{
    public class DisplayTableController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var cs = ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString;
            var dt = new OleDbDataBaseReader(cs).LoadTables("DbInfos").Tables["DbInfos"];

            return View(dt);
        }

        [HttpPost]
        public ActionResult UploadChanges(DataTable dt)
        {
            return null;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return null;
        }
    }
}