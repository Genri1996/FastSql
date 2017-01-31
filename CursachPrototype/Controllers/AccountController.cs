using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;

namespace CursachPrototype.Controllers
{
    public class AccountController : Controller
    {
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterVm vm)
        {
            if (ModelState.IsValid)
            {
                if (!IsNickNameUnic(vm.NickName))
                {
                    ModelState.AddModelError("", "Такой логин уже зарегестрирован.");
                }
                AppUser user = new AppUser { UserName = vm.Email, Email = vm.Email, UserNickName = vm.NickName};
                IdentityResult result = UserManager.Create(user, vm.Password);
                if (result.Succeeded)
                    return RedirectToAction("Login", "Account");
                else
                    foreach (string error in result.Errors)
                    {
                        if (error.Contains("is already taken"))
                            ModelState.AddModelError("", "Такой email уже зарегестрирован.");
                        else
                            ModelState.AddModelError("", error);
                    }
            }
            return View(vm);
        }

        private bool IsNickNameUnic(string nickName)
        {
            if (UserManager.Users != null)
                return true;
            return !UserManager.Users.Select(user => user.UserNickName).Any(nick => string.Compare(nickName, nick)==0);
        }

        /// <summary>
        /// ///////////////////
        /// </summary>

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVm model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = UserManager.Find(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = UserManager.CreateIdentity(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Home");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }
    }
}