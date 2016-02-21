using System.Configuration;
using System.Web.Mvc;
using Documents;
using Documents.Converter;
using Services.Documents;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// PluginController
    /// </summary>
    public class PluginController : Controller
    {
        private const int CacheTime = 3600; //一小时

        /// <summary>
        /// 浏览组件
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = CacheTime)]
        public ActionResult Browser()
        {
            ViewBag.Title = "文档管理系统SDK|浏览组件";

            return View();
        }

        /// <summary>
        /// 上传组件
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = CacheTime)]
        public ActionResult Upload2()
        {
            ViewBag.Title = "文档管理系统SDK|上传组件";

            return View();
        }

        /// <summary>
        /// 上传组件
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 0)]
        public ActionResult Upload()
        {
            ViewBag.Title = "文档管理系统SDK|上传组件2";

            ViewData["ImageUpload_API"] = ConfigurationManager.AppSettings["IMAGEUPLOAD_API"];
            return View();
        }

        /// <summary>
        /// 返回脚本页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Stripts()
        {
            return View();
        }
    }
}
