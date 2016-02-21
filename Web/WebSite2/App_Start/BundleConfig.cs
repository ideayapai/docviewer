using System.Web.Optimization;

namespace WebSite2
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //界面css自定义
            bundles.Add(new StyleBundle("~/Content/base").Include(
                     "~/Resource/css/reset.css",
                     "~/Resource/css/style.css"));

            //树型css自定义
            bundles.Add(new StyleBundle("~/Content/tree").Include(
                     "~/Resource/css/zTreeStyle/zTreeStyle.css"));

            bundles.Add(new StyleBundle("~/Content/flexpaper").Include(
                "~/css/flexpaper.css"));

            bundles.Add(new StyleBundle("~/Content/uploadify").Include(
              "~/css/uploadify.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Resource/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerytree").Include(
                    "~/Resource/js/jquery.ztree.core-3.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryeasyui").Include(
                        "~/Resource/easyui/jquery.easyui.tiny.js"));

            bundles.Add(new ScriptBundle("~/bundles/yapai").Include(
                  "~/Resource/js/yapai.documentManagement.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Resource/js/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Resource/bootstrap/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/flexpaper").Include(
                  "~/js/flexpaper.js",
                  "~/js/flexpaper_handlers.js"));

            bundles.Add(new ScriptBundle("~/bundles/uploadify").Include(
                    "~/js/uploadify.min.js",
                    "~/js/prettify.js",
                    "~/js/swfobject.js"));
        }
    }
}