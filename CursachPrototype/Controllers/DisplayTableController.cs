using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;
using Microsoft.AspNet.Identity;

namespace CursachPrototype.Controllers
{
    public class DisplayTableController : Controller
    {
        private DataTable _model;

        public DisplayTableController()
        {
            _model = new DataTable("Table");
            _model.Columns.Add(new DataColumn("ID", typeof(int)));
            _model.Columns.Add(new DataColumn("TestStr", typeof(string)));
            _model.Rows.Add(55, "TestString");


        }

        [HttpGet]
        public ActionResult Index()
        {
            //var cs = ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString;
            //_model = new OleDbDataBaseReader(cs).LoadTables("DbInfos").Tables["DbInfos"];

            return View(_model);
        }

        [HttpPost]
        public ActionResult UploadChanges()
        {

            return PartialView("_Message", "Успех");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow[_model.Columns["ID"]].ToString())
                          where idOfCurrentRow == id
                          select dataRow).FirstOrDefault();

            return PartialView("~/Views/DisplayTable/EditRow.cshtml", dr);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return null;
        }
    }
}