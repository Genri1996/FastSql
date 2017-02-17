using System.Data;
using System.Linq;
using System.Web.Mvc;
using CursachPrototype.ExtensionMethods;
using DataProxy.DataBaseReaders;
using DataProxy.DbManangment;
using CursachPrototype.QueryHelpers;

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

        private string UserId
        {
            get { return Session["UserID"] as string; }
            set { Session["UserID"] = value; }
        }

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
        public ActionResult Index(int dbId, string userId)
        {
            UserId = userId;
            DbId = dbId;

            OleDbDataBaseReader reader = new OleDbDataBaseReader(DataBaseInfo.ConnectionString);
            var tables = reader.GetTableNames();

            var items = tables.Select(tableName => new SelectListItem { Text = tableName, Value = tableName }).ToArray();
            if (items.Length == 0)
                ViewBag.NoElements = (bool?)true;
            ViewBag.DbName = DataBaseInfo.Name;

            return View(items);
        }

        [HttpGet]
        public ActionResult EditTable(string selectedTable)
        {
            Session["TableName"] = selectedTable;
            ViewBag.StatusMessage = "Просмотр таблицы " + Model.TableName;
            return PartialView(Model);
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

        [HttpPost]
        public ActionResult UploadChanges()
        {
            ChangesFixator changesHelper = new ChangesFixator(DataBaseInfo, Model);
            int changedRowId = int.Parse(Request["ID"]);
            DataTable dt = Model;

            foreach (DataColumn column in dt.Columns)
                changesHelper.AddDataColumn(Request[column.ColumnName]);

            var result = changesHelper.FixateChanges(ChangesFixator.QueryType.Update, GetIdOrdinalIndex(), changedRowId);

            if (result == string.Empty)
                result = "Ряд " + changedRowId + " был успешно изменен!";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");
            return RedirectToAction("Index", new { dbId = DbId, userId = UserId });
        }

        [HttpPost]
        public ActionResult UploadNewRow()
        {
            ChangesFixator changesHelper = new ChangesFixator(DataBaseInfo, Model);
            int changedRowId = int.Parse(Request["ID"]);
            DataTable dt = Model;

            foreach (DataColumn column in dt.Columns)
                changesHelper.AddDataColumn(Request[column.ColumnName]);

            var result = changesHelper.FixateChanges(ChangesFixator.QueryType.Insert);

            if (result == string.Empty)
                result = "Ряд " + changedRowId + " был успешно добавлен!";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");
            return RedirectToAction("Index", new { dbId = DbId, userId = UserId });
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ChangesFixator cf = new ChangesFixator(DataBaseInfo, Model);
            var result = cf.FixateChanges(ChangesFixator.QueryType.Delete, GetIdOrdinalIndex(), id);
            if (result == string.Empty)
                result = "Ряд " + id + "успешно удалён.";

            TempData["StatusMessage"] = result;
            TempData.Keep("StatusMessage");

            return RedirectToAction("Index", new { dbId = DbId, userId = UserId });
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