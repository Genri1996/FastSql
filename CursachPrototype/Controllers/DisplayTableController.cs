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
    [Authorize]
    public class DisplayTableController : Controller
    {
        private DataTable _model
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["IdentityDbOleDb"].ConnectionString;
                //get DbInfos
                var dbInfoTable = new OleDbDataBaseReader(cs).LoadTables("DbInfos").Tables["DbInfos"];
                //get users db connection string
                string dbCs = (from DataRow dataRow in dbInfoTable.Rows
                               where (int)dataRow["ID"] == _dbId
                               select dataRow["CONNECTIONSTRING"] as string).FirstOrDefault();
                //load neccesary table from there

                var usersDs = new OleDbDataBaseReader(dbCs).LoadTables(Session["TableName"] as string);
                return usersDs.Tables[Session["TableName"] as string];
            }
        }

        private string _userId
        {
            get { return Session["UserID"] as string; }
            set { Session["UserID"] = value; }
        }

        private int _dbId
        {
            get { return (int)Session["DbID"]; }
            set { Session["DbID"] = value; }
        }

        private DataBaseInfo _dataBaseInfo
        {
            get
            {
                return DataBasesManager.GetDbInfos(_userId).
                    Single(dbInf => dbInf.Id == _dbId);
            }
        }

        [HttpGet]
        public ActionResult Index(int dbId, string userId)
        {
            _userId = userId;
            _dbId = dbId;

            OleDbDataBaseReader reader = new OleDbDataBaseReader(_dataBaseInfo.ConnectionString);
            var tables = reader.GetTableNames();

            var items = tables.Select(tableName => new SelectListItem { Text = tableName, Value = tableName }).ToArray();
            if (items.Length == 0)
                ViewBag.NoElements = (bool?)true;
            ViewBag.DbName = _dataBaseInfo.Name;

            return View(items);
        }

        [HttpGet]
        public ActionResult EditTable(string selectedTable)
        {
            Session["TableName"] = selectedTable;
            ViewBag.StatusMessage = "Просмотр таблицы " + _model.TableName;
            return PartialView(_model);
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
                if (Request[column.ColumnName] == null
                    || (string.Compare(Request[column.ColumnName], "ID") == 0))
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
            if (column.DataType == typeof(int))
            {
                int val = int.Parse(Request[column.ColumnName]);
                dr[column] = val;
            }

            if (column.DataType == typeof(string))
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
            //TODO: fixate

            ViewBag.StatusMessage = "Запись была удалена";
            return RedirectToAction("Index");
        }
    }
}