using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Chailease.SolarEnergy.Web.Controllers.api
{
    public class HttpNotFoundAwareControllerActionSelector : ApiControllerActionSelector
    {
        public HttpNotFoundAwareControllerActionSelector()
        {
        }
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            HttpActionDescriptor decriptor = null;
            try
            {
                decriptor = base.SelectAction(controllerContext);
            }
            catch (HttpResponseException ex)
            {
                var code = ex.Response.StatusCode;
                if (code != HttpStatusCode.NotFound && code != HttpStatusCode.MethodNotAllowed)
                    throw;
                var routeData = controllerContext.RouteData;
                if (!controllerContext.Request.Method.Equals(HttpMethod.Post))
                {
                    routeData.Values["action"] = "Handle404";
                }
                else
                {
                    routeData.Values["action"] = "Handle404_POST";
                }
                IHttpController httpController = new ApiBaseController(controllerContext,ex.ToString());
                controllerContext.Controller = httpController;
                controllerContext.ControllerDescriptor = new HttpControllerDescriptor(controllerContext.Configuration, "ApiBase", httpController.GetType());
                decriptor = base.SelectAction(controllerContext);
            }
            catch (InvalidOperationException ex)
            {// controller have multiple methods, and can't mapping action => also 404
                var routeData = controllerContext.RouteData;
                routeData.Values["action"] = "Handle404_POST";
                IHttpController invalidController = new ApiBaseController(controllerContext,ex.ToString());
                controllerContext.Controller = invalidController;
                controllerContext.ControllerDescriptor = new HttpControllerDescriptor(controllerContext.Configuration, "ApiBase", invalidController.GetType());
                decriptor = base.SelectAction(controllerContext);
            }
            catch (Exception ex) {
                var routeData = controllerContext.RouteData;
                if (!controllerContext.Request.Method.Equals(HttpMethod.Post))
                {
                    routeData.Values["action"] = "InternalServerError";
                }
                else
                {
                    routeData.Values["action"] = "InternalServerError_POST";
                }
                IHttpController invalidController = new ApiBaseController(controllerContext,ex.ToString());

                controllerContext.Controller = invalidController;
                controllerContext.ControllerDescriptor = new HttpControllerDescriptor(controllerContext.Configuration, "ApiBase", invalidController.GetType());
                decriptor = base.SelectAction(controllerContext);
            }
            return decriptor;
        }
    }
}