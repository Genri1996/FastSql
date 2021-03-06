﻿using System.Data;
using System.Linq;
using System.Web.Mvc;
using CursachPrototype.ExtensionMethods;
using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;
using CursachPrototype.QueryHelpers;
using CursachPrototype.ViewModels;
using Microsoft.AspNet.Identity;

namespace CursachPrototype.Controllers
{
    [Authorize]
    public class DisplayTableController : Controller
    {
        private OleDbDataBaseReader ModelReader
        {
            get
            {
                //get users db connection string
                string dbCs = DataBaseInfo.ConnectionString;
                //load neccesary table from there
                return new OleDbDataBaseReader(dbCs);
            }
        }

        private DataTable Model
        {
            get
            {
                var reader = ModelReader;
                var usersDs = reader.LoadTables(Session["TableName"] as string);
                reader.Dispose();
                return usersDs.Tables[Session["TableName"] as string];
            }
        }

        private string UserId => User.Identity.GetUserId();

        private int DbId
        {
            get { return (int)Session["DbID"]; }
            set { Session["DbID"] = value; }
        }

        private DataBaseInfo DataBaseInfo
        {
            get
            {
                return DataBasesManager.GetDbInfos(UserId).
                    Single(dbInf => dbInf.Id == DbId);
            }
        }

        [HttpGet]
        public ActionResult Index(int dbId, string defaultTableName = null)
        {
            Session["TableName"] = defaultTableName;
            DbId = dbId;
            var tables = ModelReader.GetTableNames();

            var items = tables.Select(tableName => new SelectListItem { Text = tableName, Value = tableName }).ToArray();
            if (items.Length == 0)
                ViewBag.NoElements = (bool?)true;
            else
                ViewBag.NoElements = (bool?)false;
            ViewBag.DbName = DataBaseInfo.Name;

            if (Session["TableName"] != null)
            {
                ViewBag.dataOfTheTable = Model;
            }

            return View(items);
        }

        [HttpGet]
        public ActionResult EditTable(string selectedTable)
        {
            Session["TableName"] = selectedTable;
            return PartialView(Model);
        }

        [HttpGet]
        public ActionResult AddRecord()
        {
            var row = GetNewRow();
            return PartialView("AddRow", row);
        }

        [HttpGet]
        public ActionResult AddColumn(string tablename)
        {
            return PartialView(new CreateColumnVm { TableName = tablename });
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
            return PartialView("~/Views/DisplayTable/DeleteRow.cshtml", id);
        }

        [HttpGet]
        public ActionResult DeleteRowConfirmed(int id)
        {
            ChangesFixator cf = new ChangesFixator(DataBaseInfo, Model);
            var result = cf.FixateChanges(ChangesFixator.QueryType.Delete, GetIdOrdinalIndex(), id);
            if (result == string.Empty)
                result = "Ряд " + id + " успешно удалён.";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");

            return RedirectToAction("Index", new { dbId = DbId, defaultTableName = Model.TableName });
        }

        [HttpGet]
        public ActionResult CreateTable()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult DeleteColumn(string tablename)
        {
            DeleteColumnVm vm = new DeleteColumnVm { TableName = tablename };
            var dt = Model;
            foreach (DataColumn dataColumn in dt.Columns)
                //Skip Id Row. We won't delete it
                if (string.Compare(dataColumn.ColumnName, dt.Columns[GetIdOrdinalIndex()].ColumnName) == 0)
                    continue;
                else
                    vm.AvailableColumns.Add(new SelectListItem { Text = dataColumn.ColumnName, Value = dataColumn.ColumnName });

            return PartialView(vm);
        }

        [HttpGet]
        public ActionResult DeleteTable()
        {
            TableVm vm = new TableVm();
            OleDbDataBaseReader reader = new OleDbDataBaseReader(DataBaseInfo.ConnectionString);
            var tables = reader.GetTableNames();

            var items = tables.Select(tableName => new SelectListItem { Text = tableName, Value = tableName }).ToArray();
            vm.AvailableColumns.AddRange(items);

            return PartialView(vm);
        }

        [HttpPost]
        public ActionResult EditRow()
        {
            DataTable dt = Model;
            ChangesFixator changesHelper = new ChangesFixator(DataBaseInfo, dt);
            int changedRowId = int.Parse(Request[dt.Columns[GetIdOrdinalIndex()].ColumnName]);

            foreach (DataColumn column in dt.Columns)
                changesHelper.AddDataColumn(Request[column.ColumnName]);

            var result = changesHelper.FixateChanges(ChangesFixator.QueryType.Update, GetIdOrdinalIndex(), changedRowId);

            if (result == string.Empty)
                result = "Ряд " + changedRowId + " был успешно изменен!";

            TempData["StatusMessage"] = result;
            return RedirectToAction("Index", new { dbId = DbId, defaultTableName = dt.TableName });
        }

        [HttpPost]
        public ActionResult UpdateWithNewColumn(CreateColumnVm vm)
        {
            if (ModelState.IsValid)
            {

                IHelper helper = new SqlServerHelper(DataBaseInfo);
                var result = helper.InsertNewColumn(vm);

                if (result == string.Empty)
                    TempData["StatusMessage"] = $"Колонка {vm.ColumnName} была создана.";
                else
                    TempData["StatusMessage"] = result;

                return RedirectToAction("Index", new { dbId = DbId, defaultTableName = vm.TableName });
            }
            TempData["StatusMessage"] = "Были введены недопустимые данные.";
            return RedirectToAction("Index", new { dbId = DbId });
        }

        [HttpPost]
        public ActionResult UploadNewRow()
        {
            DataTable dt = Model;
            ChangesFixator changesHelper = new ChangesFixator(DataBaseInfo, dt);
            int changedRowId = int.Parse(Request[dt.Columns[GetIdOrdinalIndex()].ColumnName]);

            foreach (DataColumn column in dt.Columns)
                changesHelper.AddDataColumn(Request[column.ColumnName]);

            var result = changesHelper.FixateChanges(ChangesFixator.QueryType.Insert);

            if (result == string.Empty)
                result = "Ряд " + changedRowId + " был успешно добавлен!";

            TempData["StatusMessage"] = result;
            return RedirectToAction("Index", new { dbId = DbId, defaultTableName = dt.TableName });
        }

        [HttpPost]
        public ActionResult DeleteColumnConfirmed(DeleteColumnVm vm)
        {
            if (ModelState.IsValid)
            {
                IHelper helper = new SqlServerHelper(DataBaseInfo);
                var result = helper.DropColumn(vm);

                if (string.IsNullOrEmpty(result))
                    result = $"Колонка {vm.ColumnName} удалена успешно!";

                TempData["StatusMessage"] = result;
                return RedirectToAction("Index", new { dbId = DbId, defaultTableName = vm.TableName });
            }
            TempData["StatusMessage"] = "Были введены недопустимые данные.";
            return RedirectToAction("Index", new { dbId = DbId, defaultTableName = vm.TableName });
        }

        [HttpPost]
        public ActionResult CreateTableConfirmed(TableVm vm)
        {
            if (ModelState.IsValid)
            {
                IHelper helper = new SqlServerHelper(DataBaseInfo);
                var result = helper.CreateTable(vm.TableName);

                if (string.IsNullOrEmpty(result))
                    result = $"Таблица {vm.TableName} создана успешно!";

                TempData["StatusMessage"] = result;
                return RedirectToAction("Index", new { dbId = DbId, defaultTableName = vm.TableName });
            }
            TempData["StatusMessage"] = "Были введены недопустимые данные.";
            return RedirectToAction("Index", new { dbId = DbId });
        }

        [HttpPost]
        public ActionResult DeleteTableConfirmed(TableVm vm)
        {
            if (ModelState.IsValid)
            {
                IHelper helper = new SqlServerHelper(DataBaseInfo);
                var result = helper.DeleteTable(vm.TableName);

                if (string.IsNullOrEmpty(result))
                    result = $"Таблица {vm.TableName} удалена успешно!";

                TempData["StatusMessage"] = result;
                return RedirectToAction("Index", new { dbId = DbId });
            }
            TempData["StatusMessage"] = "Были введены недопустимые данные.";
            return RedirectToAction("Index", new { dbId = DbId });
        }

        private int GetIdOrdinalIndex()
        {
            return Model.Columns.Cast<DataColumn>()
              .First(column => column.ColumnName.ContainsIgnoreCase("Id")).Ordinal;
        }

        private DataRow GetNewRow()
        {
            var dt = Model;
            var newRow = dt.NewRow();
            var columnId = GetIdOrdinalIndex();
            int newRowId;
            if (dt.Rows.Count == 0)
                newRowId = 1;
            else
                newRowId = Model.Rows.Cast<DataRow>().Max(row => int.Parse(row.ItemArray[columnId].ToString())) + 1;
            newRow[columnId] = newRowId;
            return newRow;

        }

        private DataRow GetRecordById(int id)
        {
            var columnId = GetIdOrdinalIndex();
            DataRow dr = (from DataRow dataRow in Model.Rows
                          let idOfCurrentRow = int.Parse(dataRow.ItemArray[columnId].ToString())
                          where idOfCurrentRow == id
                          select dataRow).Single();
            return dr;
        }


    }
}