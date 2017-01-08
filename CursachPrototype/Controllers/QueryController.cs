using System;
using System.Web.Mvc;
using DataProxy.Executors;

namespace CursachPrototype.Controllers
{
    public class QueryController : Controller
    {
        public ActionResult ExecuteQuery()
        {
            String connectionString = Request.Form["connectionString"];
            String query = Request.Form["query"];

            String queryResult = new SqlServerExecutor(connectionString).ExecuteQuery(query);
            return View((object)queryResult);
        }
    }
}