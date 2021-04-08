using Chailease.SolarEnergy.Model.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Chailease.SolarEnergy.Web.Controllers.api
{
    public class ApiBaseController : ApiController
    {
            public WebApiResponse response;
            public String errorMessage { get; set; }
            public Boolean testMode { get; set; }

            #region Constructor
            protected override void Initialize(HttpControllerContext controllerContext) {
                response = new WebApiResponse();
                testMode = System.Configuration.ConfigurationManager.AppSettings["testMode"].Equals("N") ? false : true;
                errorMessage = "";
                ControllerContext = controllerContext;
            }
            protected ApiBaseController() { }
            public ApiBaseController(HttpControllerContext controllerContext) {
                Initialize(controllerContext);
            }
            public ApiBaseController(HttpControllerContext controllerContext,String errmsg) : this(controllerContext)
            {
                errorMessage = errmsg;
            }
            #endregion
            #region HTTP_DEFAULT_404
            [HttpGet, HttpPut, HttpDelete, HttpHead, HttpOptions, AcceptVerbs("PATCH")]
            public HttpResponseMessage Handle404()
            {
            
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                String baseUrl =System.Configuration.ConfigurationManager.AppSettings["WebsiteRootUrl"];
                //Request.RequestUri.GetLeftPart(UriPartial.Authority);
                String url = baseUrl + Url.Route("Default", new { controller = "Home", action = "NotFound" });
                response.Headers.Location =  new Uri(url);
                return response;
            }
            #endregion

            #region API Response Define
            [HttpPost]
            public WebApiResponse Handle404_POST()
            {
                response.Code = 1;
                if (testMode)
                {
                    response.Message = errorMessage;
                }
                else
                {
                    response.Message = "ALERT:HTTP NOT FOUND";
                }

                return response;
            }

            [HttpPost]
            public WebApiResponse InternalServerError_POST()
            {
                response.Code = 1;
                if (testMode)
                {
                    response.Message = errorMessage;
                }
                else
                {
                    response.Message = "Internal Server Error";
                }
                return response;
            }
            #endregion

        
    }
}