using System.Web.Mvc;
using Services.Search;
using WebSite2.Models;

namespace WebSite2.Controllers
{

    public class SettingController : BaseController
    {
        private readonly SearchService _indexService;

        public SettingController(SearchService indexService)
        {
            _indexService = indexService;
        }

        public ActionResult Index()
        {
            var model = new SettingViewModel();
            return View(model);
        }

        public ActionResult Account()
        {
            var model = new SettingViewModel
            {
                ActiveMenuType = MenuType.AccountSetting,
            };
            CurrentMenu = MenuType.AccountSetting;
            return View(model);
        }

        public ActionResult Mail()
        {
            return View();
        }

        public ActionResult Search()
        {
            var model = new SettingViewModel
            {
                ActiveMenuType = MenuType.Setting,
            };
            CurrentMenu = MenuType.Setting;
            return View(model);
        }

        public ActionResult WriteIndex()
        {
            var model = new SettingViewModel
            {
                ActiveMenuType = MenuType.Setting,
            };
            _indexService.Update(IndexDirectory);
            return View("Search", model);
        }

    }
}
