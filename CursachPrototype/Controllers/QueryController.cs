using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using DataProxy;
using DataProxy.DbManangment;
using DataProxy.Executors;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CursachPrototype.Controllers
{
    /// <summary>
    /// Respones for exucuting user`s queries from special control
    /// </summary>
    public class QueryController : Controller
    {
        /// <summary>
        /// Reference to current user.
        /// </summary>
        private AppUserManager _userManager => System.Web.HttpContext.Current.GetOwinContext()
            .GetUserManager<AppUserManager>();


        [HttpPost, Authorize]
        public ActionResult QueryExecutor(QueryExecutorVm vm)
        {
            //Find user
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            DataBaseInfo foundDb = DataBasesManager.GetDbInfos(user.Id).Single(m => m.Id == vm.DbId);
            vm.DataTable = DataService.ExecuteQueryAsDataTable(vm.Query, foundDb.ConnectionString, foundDb.DbmsType);
            return PartialView("QueryResults", vm.DataTable);
        }

        public ActionResult QueryExecutorWithCs(QueryExecutorVm vm)
        {
            var connectionString = Request["conStr"].Trim();
            vm.DataTable = new DataTable();
            var errorDataColumn = new DataColumn("FastSqlQueryErrMessages", typeof(string));
            vm.DataTable.Columns.Add(errorDataColumn);

            //Check if connection string is empty
            if (string.IsNullOrEmpty(connectionString))
            {
                var row = vm.DataTable.NewRow();
                row[0] = "Ваша строка подключения пуста!";
                vm.DataTable.Rows.Add(row);
                return PartialView("QueryResults", vm.DataTable);
            }
            //Try to establish connection
            QueryExecutor executor = null;
            try
            {
                executor = new SqlServerExecutor(connectionString);
            }
            catch
            {
                var row = vm.DataTable.NewRow();
                row[0] = "Не удалось установить соеденение.";
                vm.DataTable.Rows.Add(row);
                executor?.Dispose();
                return PartialView("QueryResults", vm.DataTable);
            }
            //receive data
            vm.DataTable = executor.ExecuteQueryAsDataTable(vm.Query);
            executor.Dispose();

            return PartialView("QueryResults", vm.DataTable);
        }
    }
}