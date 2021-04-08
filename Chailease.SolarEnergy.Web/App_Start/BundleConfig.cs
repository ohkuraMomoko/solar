using System.Web.Optimization;

namespace Chailease.SolarEnergy.Web
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-3.3.1.min.js"
                       , "~/Scripts/jquery.validate.min.js"
                       , "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/popper.min.js",
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Content/library/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/layout.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/plugin").Include(
                       "~/Scripts/slick.min.js",
                       "~/Scripts/TweenMax.min.js",
                       "~/Scripts/aos.js",
                       "~/Scripts/noty.min.js",
                       "~/Scripts/lazyload.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/menu").Include(
                       "~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                       "~/Scripts/common.js",
                       "~/Scripts/Utility.js"));

            bundles.Add(new ScriptBundle("~/bundles/Components").Include(
             "~/Scripts/Components/_ModalForgetEmail.js",
             "~/Scripts/Components/_ModalForgetPassword.js",
             "~/Scripts/Components/_ModalSettingPassword.js",
             "~/Scripts/Components/_ModalSignup.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                       "~/Scripts/Chart.bundle.min.js",
                       "~/Scripts/chartjs-plugin-annotation.js"));

            bundles.Add(new ScriptBundle("~/bundles/temp").Include(
                       "~/Scripts/jquery.tmpl.min.js"));
        }
    }
}
