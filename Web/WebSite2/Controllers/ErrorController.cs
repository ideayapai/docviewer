using System.Web.Mvc;

namespace WebSite2.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HttpError()
        {
            return View("Error");
        }

        public ActionResult BadRequest()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

    }
}
