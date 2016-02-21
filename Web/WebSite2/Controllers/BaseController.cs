using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Infrasturcture.Web;
using Services.Context;
using Services.Ioc;
using Services.User;
using SSOLib;
using SSOLib.Service;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    public class BaseController : PrivateController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentUser != null && !(CurrentUser is EmptyUserContract))
            {
                ContextService.Current.NickName = CurrentUser.NickName;
                ContextService.Current.DepId = CurrentUser.DepId.ToString();
                ContextService.Current.UserId = CurrentUser.Id.ToString();
                ContextService.Current.UserPhoto = ConfigurationManager.AppSettings["USER_AVATAR"] + CurrentUser.UserInfoPhoto;
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;
            string actionPath = "/" + controller + "/" + action;
            string userName = "";

            if (CurrentUser != null && !(CurrentUser is EmptyUserContract))
            {
                if ((string.IsNullOrEmpty(CurrentUser.NickName) || CurrentUser.DepId == default(Guid) || CurrentUser.Id == default(Guid)))
                {
                    FormsAuthentication.SignOut();
                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                }
                else
                {
                    ContextService.Current.NickName = CurrentUser.NickName;
                    ContextService.Current.DepId = CurrentUser.DepId.ToString();
                    ContextService.Current.UserId = CurrentUser.Id.ToString();
                    ContextService.Current.UserPhoto = ConfigurationManager.AppSettings["USER_AVATAR"] + CurrentUser.UserInfoPhoto;
                }
            }
            base.OnActionExecuted(filterContext);
        }

        //protected override void Initialize(RequestContext requestContext)
        //{
        //    //if (RequireLogin())
        //    //{
        //    //    FormsAuthentication.SignOut();
        //    //    requestContext.HttpContext.Response.Redirect(FormsAuthentication.LoginUrl);
        //    //    return;
        //    //}

        //    //ContextService.Current.NickName = CurrentUser.NickName;
        //    //ContextService.Current.DepId = CurrentUser.DepId.ToString();
        //    //ContextService.Current.UserId = CurrentUser.Id.ToString();

        //    //base.Initialize(requestContext);
        //}

        private bool RequireLogin()
        {
            if (CurrentUser == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(CurrentUser.Id.ToString()) ||
                string.IsNullOrEmpty(CurrentUser.NickName) ||
                CurrentUser.DepId == default(Guid))
            {
                return true;
            }

            return false;
        }



        public MenuType CurrentMenu { get; set; }

        public string IndexDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["segmentPath"];
            }
        }

    }
}