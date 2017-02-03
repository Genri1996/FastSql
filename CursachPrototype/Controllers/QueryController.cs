using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
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

        [HttpPost]
        public ActionResult QueryExecutor(QueryExecutorVm vm)
        {
            //Find user
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            DataBaseInfo foundDb = DataBaseInfoManager.GetDbInfos(user.Id).Single(m => m.Id == vm.DbId);

            //TODO: Think, if i need different executors for each DBMS or not

            using (IQueryExecutor executor = new SqlServerExecutor(foundDb.ConnectionString))
            {
                vm.QueryResult = executor.ExecuteQueryAsString(vm.Query);
            }

            return PartialView("QueryResults",vm);
        }
    }
}