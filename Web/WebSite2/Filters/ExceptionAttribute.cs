using System;
using System.Web.Mvc;
using Common.Logging;

namespace WebSite2.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        private readonly ILog _log = LogManager.GetCurrentClassLogger();

        public virtual void OnException(ExceptionContext filterContext)
        {
            string message = string.Format("消息类型：{0}<br>消息内容：{1}<br>引发异常的方法：{2}<br>引发异常源：{3}"
                , filterContext.Exception.GetType().Name
                , filterContext.Exception.Message
                 , filterContext.Exception.TargetSite
                 , filterContext.Exception.Source + filterContext.Exception.StackTrace
                 );

            //记录日志
            _log.Error(message);

            //抛出异常信息
            filterContext.Controller.TempData["ExceptionAttributeMessages"] = message;

            ////转向
            //filterContext.ExceptionHandled = true;
            //filterContext.Result = new RedirectResult("/Error/ErrorDetail/");
        }
    }
}