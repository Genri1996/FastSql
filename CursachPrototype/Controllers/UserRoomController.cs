using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models;
using CursachPrototype.Models.Accounting;
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
        /// Returns a table with available databases to current user
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult Index()
        {
            AppUser user =
                System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<AppUserManager>()
                    .FindById(User.Identity.GetUserId());

      
            return View(user.UserDbs);
        }

        /// <summary>
        /// Returns a confirmation reqguest to delete the DB
        /// </summary>
        /// <param name="id">Id of selected Db</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult Delete(int id)
        {

            AppUser user =
                System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<AppUserManager>()
                    .FindById(User.Identity.GetUserId());

            DataBaseInfo foundDb = user.UserDbs.Single(db => db.Id == id);
            return View(foundDb);
        }

        /// <summary>
        /// Removes Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            AppUser user =
               System.Web.HttpContext.Current.GetOwinContext()
                   .GetUserManager<AppUserManager>()
                   .FindById(User.Identity.GetUserId());

            DataBaseInfo foundDb = user.UserDbs.Single(db => db.Id == id);
            user.UserDbs.Remove(foundDb);



            return RedirectToAction("Index");
        }
    }
}