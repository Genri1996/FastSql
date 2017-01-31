using System.Web.Mvc;
using DataProxy.Executors;

namespace CursachPrototype.Controllers
{
    /// <summary>
    /// Respones for exucuting user`s queries from special control
    /// </summary>
    public class QueryController : Controller
    {
        [HttpPost]
        public ActionResult ExecuteQuery()
        {
            //Collect data from Request
            string connectionString = Request.Form["connectionString"];
            string query = Request.Form["query"];
            string queryResult;

            //TODO: Think, if i need different executors for each DBMS or not
            using (IQueryExecutor executor = new SqlServerExecutor(connectionString))
            {
                queryResult = executor.ExecuteQueryAsString(query);
            }

            return View((object)queryResult);
        }
    }
}