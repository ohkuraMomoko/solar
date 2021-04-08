using Chailease.SolarEnergy.Web.Controllers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;

namespace Chailease.SolarEnergy.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, HttpConfiguration config)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults:
                new
                {
                    action = RouteParameter.Optional,
                    id = UrlParameter.Optional
                }
            );

          //  routes.MapRoute(name: "Project", url: "Sell/Project/{caseNo}", defaults: new { controller = "Sell", action = "Project", caseNo = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );



            config.Routes.MapHttpRoute(
                name: "Handle404",
                routeTemplate: "api/{*url}"
            );

            config.Services.Replace(typeof(IHttpControllerSelector), new HttpNotFoundAwareDefaultHttpControllerSelector(config));
            config.Services.Replace(typeof(IHttpActionSelector), new HttpNotFoundAwareControllerActionSelector());
            config.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
    }
}
