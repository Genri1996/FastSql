using System.Web.Mvc;
using CursachPrototype.ViewModels;

namespace CursachPrototype.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MakeQuery()
        {
            return PartialView("~/Views/Query/QueryExecutorWithCs.cshtml", new QueryExecutorVm());
        }
    }
}