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
    /// Handles actions in private room.
    /// </summary>
    public class UserRoomController : Controller
    {
        /// <summary>
        /// Reference to current user.
        /// </summary>
        private AppUserManager _userManager => System.Web.HttpContext.Current.GetOwinContext()
            .GetUserManager<AppUserManager>();

        /// <summary>
        /// Returns a table with available databases to current user
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult Index()
        {
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            return View(DataBasesManager.GetDbInfos(user.Id));
        }

        /// <summary>
        /// Returns a confirmation reqguest to delete the DB
        /// </summary>
        /// <param name="id">Id of selected Db</param>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult Delete(int id)
        {
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            DataBaseInfo foundDb = DataBasesManager.GetDbInfos(user.Id).Single(db => db.Id == id);
            return View(foundDb);
        }

        /// <summary>
        /// Removes Database after confiramtion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            //Find user
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            //Find DB
            DataBaseInfo foundDb = DataBasesManager.GetDbInfos(user.Id).Single(db => db.Id == id);
            //Remove db info from DbInfos
            DataBasesManager.RemoveDbInfo(foundDb);
            //Delete database
            DataService.DropDataBase(foundDb.DbmsType, foundDb.Name);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ExecuteQuery(int id)
        {
            //Find user
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            //Find DB
            DataBaseInfo foundDb = DataBasesManager.GetDbInfos(user.Id).Single(db => db.Id == id);

            QueryExecutorVm vm = new QueryExecutorVm
            {
                DbId = id,
                DbName = foundDb.Name
            };

            return PartialView("~/Views/Query/QueryExecutor.cshtml", vm);
        }
    }
}