using System.Web.Http;
using System.Web.Mvc;
using WebAPI2.Areas.HelpPage.App_Start;

namespace WebAPI2.Areas.HelpPage
{
    /// <summary>
    /// API自动生成帮助页面
    /// </summary>
    public class HelpPageAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// AreaName
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        /// <summary>
        /// RegisterArea
        /// </summary>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_Default",
                "Help/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}