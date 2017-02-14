﻿using System.Data;
using System.Linq;
using System.Web.Mvc;
using CursachPrototype.ExtensionMethods;
using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;
using System.Text;

namespace CursachPrototype.Controllers
{
    [Authorize]
    public class DisplayTableController : Controller
    {
        private DataTable _model
        {
            get
            {
                var reader = _modelReader;
                var usersDs = reader.LoadTables(Session["TableName"] as string);
                reader.Dispose();
                return usersDs.Tables[Session["TableName"] as string];
            }
        }

        private OleDbDataBaseReader _modelReader
        {
            get
            {
                //get users db connection string
                string dbCs = _dataBaseInfo.ConnectionString;
                //load neccesary table from there
                return new OleDbDataBaseReader(dbCs);
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
            var dr = GetRecordById(changedRowId) ?? GetNewRow();

            foreach (DataColumn column in dr.Table.Columns)
            {
                if (Request[column.ColumnName] == null || Request[column.ColumnName].ContainsIgnoreCase("Id"))
                    continue;
                ProcesTypes(column, dr);
            }

            //TODO: add saving to database

            ViewBag.StatusMessage = "Ряд " + changedRowId + " был успешно изменён!";
            return RedirectToAction("Index", new { dbId = _dbId, userId = _userId });
        }

        [HttpPost]
        public ActionResult UploadNewRow()
        {
            int changedRowId = int.Parse(Request["ID"]);
            var dr = GetNewRow();

            foreach (DataColumn column in dr.Table.Columns)
            {
                if (Request[column.ColumnName] == null || Request[column.ColumnName].ContainsIgnoreCase("Id"))
                    continue;
                ProcesTypes(column, dr);
            }

            DataTable dt = _model;

            StringBuilder insertStringBuilder = new StringBuilder($"use {_dataBaseInfo.Name} Insert into {_model.TableName} values (");

            bool first = true;
            foreach (DataColumn column in dt.Columns)
            {
                if (!first)
                {
                    insertStringBuilder.Append(",");
                    insertStringBuilder.Append($"@{column.ColumnName}");
                }
                else
                    insertStringBuilder.Append($"@{column.ColumnName}");
            }
            insertStringBuilder.Append(")");
            //TODO: add saving to database

            ViewBag.StatusMessage = "Ряд " + changedRowId + " был успешно добавлен!";
            return RedirectToAction("Index", new { dbId = _dbId, userId = _userId });
        }


        [HttpGet]
        public ActionResult Add()
        {
            var row = GetNewRow();
            return PartialView("AddRow", row);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var dr = GetRecordById(id);
            return PartialView("~/Views/DisplayTable/EditRow.cshtml", dr);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var dr = GetRecordById(id);
            return PartialView("~/Views/DisplayTable/DeleteRow.cshtml", dr);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var dr = GetRecordById(id);
            _model.Rows.Remove(dr);
            //TODO: fixate

            ViewBag.StatusMessage = "Запись была удалена";
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
        private DataRow GetRecordById(int id)
        {
            var columnId = _model.Columns.Cast<DataColumn>()
                .First(column => column.ColumnName.ContainsIgnoreCase("Id")).Ordinal;
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow.ItemArray[columnId].ToString())
                          where idOfCurrentRow == id
                          select dataRow).SingleOrDefault();
            return dr;
        }
        private DataRow GetNewRow()
        {
            var newRow = _model.NewRow();
            var columnId = _model.Columns.Cast<DataColumn>()
              .First(column => column.ColumnName.ContainsIgnoreCase("Id")).Ordinal;
            var maxId = _model.Rows.Cast<DataRow>().Max(row => int.Parse(row.ItemArray[columnId].ToString()));
            newRow[columnId] = maxId + 1;
            return newRow;
        }
    }
}