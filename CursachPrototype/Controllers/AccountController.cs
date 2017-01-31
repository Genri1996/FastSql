using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace CursachPrototype.Controllers
{
    /// <summary>
    /// Perfoms actions with accounting. Mostly from metanit.com
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Returns user manager
        /// </summary>
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

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
                //Check if nick name is also unic. (Along with email)
                if (!IsNickNameUnic(vm.NickName))
                {
                    ModelState.AddModelError("", "Такой логин уже зарегестрирован.");
                }
                AppUser user = new AppUser { UserName = vm.Email, Email = vm.Email, UserNickName = vm.NickName };
                IdentityResult result = UserManager.Create(user, vm.Password);
                if (result.Succeeded)
                {
                    //Immediate sign in
                    SignIn(user);
                    return RedirectToAction("Index", "Home");
                }

                foreach (string error in result.Errors)
                {
                    //Russian translation. Defaul is english
                    if (error.Contains("is already taken"))
                        ModelState.AddModelError("", "Такой email уже зарегестрирован.");
                    else
                        ModelState.AddModelError("", error);
                }
            }
            return View(vm);
        }



        /// <summary>
        /// Check if nick name is unic
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        private bool IsNickNameUnic(string nickName)
        {
            if (UserManager.Users != null)
                return true;
            return !UserManager.Users.Select(user => user.UserNickName).Any(nick => string.Compare(nickName, nick) == 0);
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
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
                    SignIn(user);
                    if (string.IsNullOrEmpty(returnUrl))
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

        private void SignIn(AppUser user)
        {
            ClaimsIdentity claim = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignOut();
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, claim);
        }
    }
}