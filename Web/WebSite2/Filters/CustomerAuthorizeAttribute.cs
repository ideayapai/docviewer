using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Common.Logging;
using Infrasturcture.Web;
using Ninject;
using Services.Context;
using Services.Contracts;
using Services.Ioc;
using Services.Role;
using Services.User;

namespace WebSite2.Filters
{
    /// <summary>
    /// 用户权限拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomerAuthorizeAttribute : AuthorizeAttribute
    {
        public string UrlReferrer { get; set; }

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var rs = base.AuthorizeCore(httpContext);
            if (rs)
            {

            }
            return rs;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var filters = filterContext.ActionDescriptor.GetCustomAttributes(false);
            if (filters.Any(p => p is AllowAnonymousAttribute))
            {
                return;
            }
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            string actionPath = "/" + controllerName + "/" + actionName;
            // filterContext.Result = new RedirectResult("/Account/AuthorError");
            var identity = filterContext.HttpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                var user = ServiceActivator.Kernel.Get<UserService>().GetByUserName(identity.Name);
                
                _logger.Info("权限判断：" + actionPath + "；访问IP：" + CerCommon.GetIp() + "; 登录账号：" + user.UserName);

                if (user != null)
                {
                    if (filterContext.HttpContext.Session != null && string.IsNullOrEmpty(ContextService.Current.NickName))
                    {
                        ContextService.Current.NickName = user.NickName;
                    }
                    var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"];
                    var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"];
                    if (controller != null && action != null)
                    {
                        //if (controller.ToString().ToLower() == "home") return ;
                        string filePath = "/" + controller + "/" + action;
                        string key = string.Format("rolefunctionView_{0}", user.RoleId);
                        var cache = filterContext.HttpContext.Cache;
                        if (cache[key] == null)
                        {
                            Guid id;
                            if (!Guid.TryParse(ContextService.Current.GetCookieValue("role"), out id))
                            {
                                id = user.RoleId;
                                ContextService.Current.SetCookie("role", user.RoleId.ToString());
                            }
                            var menus = ServiceActivator.Kernel.Get<RoleFunctionService>().GetByRole(id);
                            cache.Add(key, menus, new SqlCacheDependency("MonkeyCacheDependency", "BASE_ROLEFUNCTIONS"),
                                      Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                                      CacheItemPriority.Normal,
                                      null);
                        }
                        var menusContract = cache[key] as List<MenuItemContract>;
                        if (menusContract != null)
                        {
                            try
                            {
                                var rs = menusContract.Exists(p =>
                                        String.Compare(p.Action.ToLower(), filePath.ToLower(),
                                            StringComparison.OrdinalIgnoreCase) == 0);
                                _logger.Info("权限判断：" + filePath + "；是否有权限：" + rs + "; 登录账号：" + user.UserName);
                                if (!rs)
                                {
                                    //  权限默认页跳转
                                    if (filePath.ToLower().Contains("home/index") && menusContract.Count > 0)
                                    {
                                        var roleList =
                                            menusContract.Where(p => p.Action != "/" && !string.IsNullOrWhiteSpace(p.Action))
                                                .OrderBy(p => p.OrderBy).ToList();
                                        string firstAction = roleList[0].Action;
                                        filterContext.Result = new RedirectResult(firstAction);
                                    }
                                    else
                                    {
                                        filterContext.Result = new RedirectResult("/Account/AuthorError");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.Error("权限判断：" + actionPath + "；访问IP：" + CerCommon.GetIp() + "; 登录账号：" + user.UserName, e);
                            }

                        }
                        else
                            filterContext.Result = new RedirectResult("/Account/AuthorError");
                    }
                }
            }
            base.OnAuthorization(filterContext);

        }
    }
}