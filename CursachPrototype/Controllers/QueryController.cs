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
            String queryResult;

            using (IQueryExecutor executor = new SqlServerExecutor(connectionString))
            {
                queryResult = executor.ExecuteQueryAsString(query);
            }

            return View((object)queryResult);
        }
    }
}