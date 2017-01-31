using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models.Accounting;
using DataProxy;
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
            return View(DataBaseInfoManager.GetDbInfos(user));
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
            DataBaseInfo foundDb = DataBaseInfoManager.GetDbInfos(user).Single(db => db.Id == id);
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
            DataBaseInfo foundDb = DataBaseInfoManager.GetDbInfos(user).Single(db => db.Id == id);
            //Remove db info from DbInfos
            DataBaseInfoManager.RemoveDbInfo(foundDb);
            //Delete database
            DataService.DropDataBase(foundDb.DbmsType, foundDb.Name);

            return RedirectToAction("Index");
        }
    }
}