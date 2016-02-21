using System.Web.Optimization;

namespace WebAPI2
{
    /// <summary>
    /// css,js绑定
    /// </summary>
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        /// <summary>
        /// 注册绑定css/js
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.9.0.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            //添加flexpaper js
            bundles.Add(new ScriptBundle("~/bundles/flexpaper").Include(
                 "~/Scripts/swfobject.js",
                 "~/Scripts/flexpaper.js",
                 "~/Scripts/flexpaper_handlers.js"));

            //添加uploadify js
            bundles.Add(new ScriptBundle("~/bundles/uploadify").Include(
                    "~/Scripts/uploadify.js",
                    "~/Scripts/prettify.js",
                    "~/Scripts/swfobject.js"));

            //添加flexpaper css
            bundles.Add(new StyleBundle("~/Content/flexpaper").Include(
                "~/Content/flexpaper.css"));

            //添加uploadify css
            bundles.Add(new StyleBundle("~/Content/uploadify").Include(
              "~/Content/uploadify.css"));

           
        }
    }
}
