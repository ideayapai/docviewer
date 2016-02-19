using System;
using System.Web.Caching;
using System.Web.Mvc;
using Common.Logging;
using Infrasturcture.Web;
using Services.Context;
using Services.Ioc;
using Services.Role;
using Services.User;

namespace WebSite2.Filters
{
    /// <summary>
    /// 菜单高亮
    /// </summary>
    public class MenuHightlightAttribute : ActionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 当前菜单
        /// </summary>
        public string CurrentItem { get; set; }
        /// <summary>
        /// 父级菜单
        /// </summary>
        public string CurrentParentItem { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;
            string actionPath = "/" + controller + "/" + action;

            _logger.Info("菜单高亮：" + actionPath + "；访问IP：" + CerCommon.GetIp() + "; 登录账号：" + filterContext.HttpContext.User.Identity.Name);

            filterContext.Controller.ViewData["CurrentMenu"] = CurrentItem;
            filterContext.Controller.ViewData["CurrentParentItem"] = CurrentParentItem;
            var cache = filterContext.HttpContext.Cache;
            string key = string.Format("rolefunction_{0}", ContextService.Current.GetCookieValue("role"));
            if(cache[key] == null)
            {
                Guid rs;
                if(!Guid.TryParse(ContextService.Current.GetCookieValue("role"), out rs))
                {
                    var user = ServiceActivator.Get<UserService>().GetByUserName(filterContext.HttpContext.User.Identity.Name);
                    rs = user.RoleId;
                    ContextService.Current.SetCookie("role", user.RoleId.ToString());
                }
                var menus = ServiceActivator.Get<RoleFunctionService>().Get(rs);
                cache.Add(key, menus, new SqlCacheDependency("MonkeyCacheDependency", "BASE_ROLEFUNCTIONS"), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
           //ContextService.Current.Cache("");

            filterContext.Controller.ViewData["Menus"] = cache[key];
            base.OnActionExecuted(filterContext);
        }
    }
}