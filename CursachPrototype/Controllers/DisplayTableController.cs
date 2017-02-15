using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CursachPrototype.ExtensionMethods;
using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;
using System.Text;
using CursachPrototype.QueryHelpers;
using DataProxy;

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
            ChangesFixator changesHelper = new ChangesFixator(_dataBaseInfo, _model);
            int changedRowId = int.Parse(Request["ID"]);
            changesHelper.AddDataColumn(changedRowId.ToString());
            DataTable dt = _model;

            foreach (DataColumn column in dt.Columns)
                changesHelper.AddDataColumn(Request[column.ColumnName]);

            var result = changesHelper.FixateChanges(ChangesFixator.QueryType.Update, GetIdOrdinalIndex(), changedRowId);

            if (result == string.Empty)
                result = "Ряд " + changedRowId + " был успешно изменен!";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");
            return RedirectToAction("Index", new { dbId = _dbId, userId = _userId });
        }

        [HttpPost]
        public ActionResult UploadNewRow()
        {
            ChangesFixator changesHelper = new ChangesFixator(_dataBaseInfo, _model);
            int changedRowId = int.Parse(Request["ID"]);
            changesHelper.AddDataColumn(changedRowId.ToString());
            DataTable dt = _model;

            foreach (DataColumn column in dt.Columns)
                changesHelper.AddDataColumn(Request[column.ColumnName]);

            var result = changesHelper.FixateChanges(ChangesFixator.QueryType.Insert);

            if (result == string.Empty)
                result = "Ряд " + changedRowId + " был успешно добавлен!";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");
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
            return PartialView("~/Views/DisplayTable/DeleteRow.cshtml", id);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ChangesFixator cf = new ChangesFixator(_dataBaseInfo, _model);
            var result = cf.FixateChanges(ChangesFixator.QueryType.Delete, GetIdOrdinalIndex(), id);
            if (result == string.Empty)
                result = "Ряд " + id + "успешно удалён.";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");

            return RedirectToAction("Index", new { dbId = _dbId, userId = _userId });
        }

        private int GetIdOrdinalIndex()
        {
            return _model.Columns.Cast<DataColumn>()
              .First(column => column.ColumnName.ContainsIgnoreCase("Id")).Ordinal;
        }

        private DataRow GetNewRow()
        {
            var dt = _model;
            var newRow = dt.NewRow();
            var columnId = GetIdOrdinalIndex();
            int newRowId;
            if (dt.Rows.Count == 0)
                newRowId = 1;
            else
                newRowId = _model.Rows.Cast<DataRow>().Max(row => int.Parse(row.ItemArray[columnId].ToString())) + 1;
            newRow[columnId] = newRowId;
            return newRow;

        }
        private DataRow GetRecordById(int id)
        {
            var columnId = GetIdOrdinalIndex();
            DataRow dr = (from DataRow dataRow in _model.Rows
                          let idOfCurrentRow = int.Parse(dataRow.ItemArray[columnId].ToString())
                          where idOfCurrentRow == id
                          select dataRow).Single();
            return dr;
        }
    }
}