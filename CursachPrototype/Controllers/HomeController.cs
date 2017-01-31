using System.Web.Mvc;

namespace CursachPrototype.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}