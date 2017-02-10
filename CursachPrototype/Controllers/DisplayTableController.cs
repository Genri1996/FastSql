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
            _model.Rows.Add(66, "TestString2");

        }

        [HttpGet]
        public ActionResult Index()
        {
            //var cs = ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString;
            //_model = new OleDbDataBaseReader(cs).LoadTables("DbInfos").Tables["DbInfos"];

            ViewBag.StatusMessage = "Просмотр таблицы " + _model.TableName;
            return View(_model);
        }

        [HttpPost]
        public ActionResult UploadChanges()
        {
            int changedRowId = int.Parse(Request["ID"]);
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow[_model.Columns["ID"]].ToString())
                          where idOfCurrentRow == changedRowId
                          select dataRow).SingleOrDefault();

            dr.BeginEdit();
            foreach (DataColumn column in dr.Table.Columns)
            {
                if (Request[column.ColumnName] == null || (String.Compare(Request[column.ColumnName], "ID")==0))
                {
                    continue;
                }

                ProcesTypes(column, dr);
            }

            //TODO: add saving to database
            dr.AcceptChanges();

            ViewBag.StatusMessage = "Ряд " + changedRowId + " был успешно изменён!";
            return RedirectToAction("Index");
        }

        private void ProcesTypes(DataColumn column, DataRow dr)
        {
            if (column.DataType == typeof (int))
            {
                int val = int.Parse(Request[column.ColumnName]);
                dr[column] = val;
            }

            if (column.DataType == typeof (string))
            {
                string val = Request[column.ColumnName];
                dr[column] = val;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow[_model.Columns["ID"]].ToString())
                          where idOfCurrentRow == id
                          select dataRow).SingleOrDefault();

            return PartialView("~/Views/DisplayTable/EditRow.cshtml", dr);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow[_model.Columns["ID"]].ToString())
                          where idOfCurrentRow == id
                          select dataRow).SingleOrDefault();

            return PartialView("~/Views/DisplayTable/DeleteRow.cshtml", dr);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow[_model.Columns["ID"]].ToString())
                          where idOfCurrentRow == id
                          select dataRow).SingleOrDefault();

            _model.Rows.Remove(dr);

            ViewBag.StatusMessage = "Запись была удалена";
            return RedirectToAction("Index");
        }
    }
}