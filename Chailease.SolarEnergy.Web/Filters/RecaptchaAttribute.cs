using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Commons;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Filters
{
    public class RecaptchaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var controllerName = (string)filterContext.RouteData.Values["controller"];
            //var actionName = (string)filterContext.RouteData.Values["action"];
            if (!filterContext.HttpContext.Request.IsLocal)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    string recaptchaToken = string.Empty;
                    var contentType = filterContext.HttpContext.Request.ContentType;
                    if (contentType.Contains("application/json"))
                    {
                        var inputStream = filterContext.HttpContext.Request.InputStream;
                        inputStream.Position = 0;
                        using (var reader = new System.IO.StreamReader(inputStream))
                        {
                            var body = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(body))
                            {
                                JObject jo = JObject.Parse(body);
                                recaptchaToken = ((JValue)jo["g-recaptcha-response"]).Value.ToString();
                            }
                        }
                    }
                    
                    if (Common.IsValid(recaptchaToken))
                    {
                        //TO DO:寫入資料
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Clear();
                        filterContext.HttpContext.Response.StatusCode = 500;
                        filterContext.Result = new JsonResult()
                        {
                            Data = new
                            {
                                errpage = string.Empty,
                                isLoacl = filterContext.HttpContext.Request.IsLocal,
                                isRecaptchaFail = true,
                                message = "Recaptcha chenk failed"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class AuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var controllerName = (string)filterContext.RouteData.Values["controller"];
            //var actionName = (string)filterContext.RouteData.Values["action"];
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var acct = new Services.AccountService();
                if (!acct.IsAuthorized)
                {
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "index" }));

                    // Redirect to external url
                    var back = filterContext.RequestContext.HttpContext.Request.Url.AbsolutePath + filterContext.RequestContext.HttpContext.Request.Url.Query;
                    filterContext.Result = new RedirectResult(string.Format("~/home{0}{1}#login", "?Back=", back));
                }
                else
                {
                    var u = acct.GetUserInfo();
                    filterContext.HttpContext.User = new UserPrincipal()
                    {
                        Identity = new UserInfo()
                        {
                            Name = u.CUST_NAME,
                            IsAuthenticated = true,
                            MemberId = u.MBR_ID,
                            Account = u.MBR_ACCT
                        }
                    };
                }
                base.OnActionExecuting(filterContext);
            }
        }

        public class UserPrincipal : IPrincipal
        {
            public IIdentity Identity { get; set; }

            public bool IsInRole(string role)
            {
                return true;
            }
        }

        public class UserInfo : IIdentity
        {
            public string Name { get; set; }

            public string AuthenticationType { get; set; }

            public bool IsAuthenticated { get; set; }
            public string MemberId { get; internal set; }
            public string Account { get; internal set; }
        }
    }

    public class GlobalHandleError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            new LogServices().Error(ex, string.Format("controller:{0}, action:{1}", controllerName, actionName));

            if (!filterContext.HttpContext.Request.IsLocal & !filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //var result = new ViewResult
                //{
                //    ViewName = "Error",
                //    //MasterName = "_Layout",
                //    TempData = filterContext.Controller.TempData
                //};

                ////uesr temp data pass error message to error page
                //result.TempData["ErrorMsg"] = ex.Message;
                //filterContext.Result = result;
                filterContext.Result = new RedirectResult("~/home/error");

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var url = new UrlHelper(filterContext.RequestContext).Action("error", "home");
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    //Data = new { Message = filterContext.Exception.Message }
                    Data = new
                    {
                        errpage = url,
                        isLoacl = filterContext.HttpContext.Request.IsLocal,
                        isRecaptchaFail = false,
                        message = ex.Message
                    }
                };
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }

            base.OnException(filterContext);
        }
    }
}