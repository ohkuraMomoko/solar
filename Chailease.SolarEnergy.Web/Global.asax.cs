using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Chailease.SolarEnergy.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes, GlobalConfiguration.Configuration);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Register global filter
            GlobalFilters.Filters.Add(new Filters.GlobalHandleError());
        }
    }
}
