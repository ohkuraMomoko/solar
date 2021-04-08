using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Chailease.SolarEnergy.Web.Controllers.api
{
    public class HttpNotFoundAwareDefaultHttpControllerSelector : DefaultHttpControllerSelector
    {
        public HttpNotFoundAwareDefaultHttpControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            HttpControllerDescriptor decriptor = null;
            try
            {
                decriptor = base.SelectController(request);
            }
            catch (HttpResponseException ex)
            {
                var code = ex.Response.StatusCode;
                if (code != HttpStatusCode.NotFound)
                    throw;
                var routeValues = request.GetRouteData().Values;
                routeValues["controller"] = "ApiBase";
                if (!request.Method.Equals("POST"))
                {
                    routeValues["action"] = "Handle404";
                }
                else
                {
                    routeValues["action"] = "Handle404_POST";
                }
                decriptor = base.SelectController(request);
            }
            catch (Exception ex) {
                var routeValues = request.GetRouteData().Values;
                routeValues["controller"] = "ApiBase";
                if (!request.Method.Equals("POST"))
                {
                    routeValues["action"] = "InternalServerError";
                }
                else
                {
                    routeValues["action"] = "InternalServerError_POST";
                }
                decriptor = base.SelectController(request);

            }
            return decriptor;
        }
    }
}