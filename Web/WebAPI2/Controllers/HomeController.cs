using System.Web.Mvc;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// HomeController
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewBag.Title = "文档管理系统SDK";

            return View();
        }
    }
}
