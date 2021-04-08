using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Commons;
using Chailease.SolarEnergy.Web.Filters;
using Chailease.SolarEnergy.Web.ViewModel;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.IO;
using System.Net.Http;
using Chailease.SolarEnergy.Model;
using Newtonsoft.Json;
using static Chailease.SolarEnergy.Services.MailSender;
using System.Collections.Generic;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class AccountController : Controller
    {
        AccountService service { get; set; }
        public AccountController()
        {
            service = new AccountService();
        }

        [HttpPost]
        [Recaptcha]
        public ActionResult Login(LoginViewModel model, string Back = "")
        {
            if (!service.Verify(new VerifyDto() { Account = model.Email.Trim(), Pwd = model.Password, Ip = Request.UserHostAddress, Browser = Request.Browser.Browser }))
            {
                return Json(new { result = false, message = "登入失敗" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, BackURL = Back }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout(LoginViewModel model)
        {
            service.Logout();
            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        public ActionResult HasRecommend(string id)
        {
            return Json(new { result = service.HasRecommend(id), message = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HasAccount(string id)
        {
            return Json(new { result = service.HasAccount(id), message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Recaptcha]
        public ActionResult TempRegistrationInfo(RegistrationInfoViewModel model)
        {
            service.TempRegistrationInfo(new RegisterDto()
            {
                Account = model.Email.Trim(),
                Pwd = model.Password,
                RecommendId = model.Recommender,
                ClientIP = Request.UserHostAddress,
                HasNotifyLetter = model.HasNotifyLetter ? "Y" : "N"
            },
            new SorceOptionDto()
            {
                otherSorce = model.SourceEtc,
                SorceValue = model.Source
            });
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdTempRegistrationInfo(bool hasNotifyLetter)
        {
            service.UpdTempRegistrationInfo(hasNotifyLetter);
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRegPhoneMessage(string PhoneNo)
        {
            var result = service.GetRegPhoneMessage(PhoneNo);
            return Json(new { result = result.RESULT, message = result.ERRMSG, result.RES_SEC }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRegPhoneMessageForBIND(string PhoneNo)
        {
            var result = service.GetRegPhoneMessage(PhoneNo, "BIND");
            return Json(new { result = result.RESULT, message = result.ERRMSG, result.RES_SEC , errorCode = result.ERRCODE});
        }

        public JsonResult HasInvestmbr(string id, string phoneNo)
        {
            var param = new HasInvestmbrParams
            {
                CustId = id,
                PhoneNo = phoneNo,
            };
            var result = service.HasInvestmbr(param, Request.Browser.Browser);
            return Json(new { result = result.RESULT, message = result.ERRMSG });
        }

        [HttpPost]
        public ActionResult VerifyRegPhoneMessage(string Message)
        {
            var result = service.VerifyRegPhoneMessage(Message);
            return Json(new { result = result.RESULT, message = result.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Register()
        {
            string email;
            var result = service.Register(Request.Browser.Browser, out email);
            if (result.RESULT && !string.IsNullOrEmpty(email))
            {
                //需求單20191107409090 註冊完不要發驗證EMAIL 20200410
                // var r = service.SendEmailValidate("N", Common.RootUrl(Request) + "/AdvancedPurchase/EmailValidata");
            }
            return Json(new { result = result.RESULT, message = result.ERRMSG, email = email }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Recaptcha]
        public ActionResult ForgetPassword(ForgetPwdViewModel model)
        {
            var result = service.ForgetPassword(new ForgotPasswordDto()
            {
                Account = model.Email,
                CustMobile = model.Phone,
                URL = Commons.Common.RootUrl(Request) + "/Account/EditPassword?q="
            });
            return Json(new { result = result.RESULT, message = result.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPassword(string q)
        {
            if (service.EditPassword(q))
                return new RedirectResult(Url.Action("Index", "Home") + "#settingpassword");
            else
            {
                //錯誤頁面
                throw new System.Exception("連結失效!");
            }
        }

        /// <summary>
        /// Json Return Object
        /// </summary>
        public class ReturnObj
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }

            public string Email { get; set; }

            public string UserID { get; set; }
            public string UserName { get; set; }
            public string Phone { get; set; }
        }
        /// <summary>
        /// LoginWithFacebook
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>

        public JsonResult LoginWithFacebook(string accessToken)
        {
            try
            {
                var jsonResult = this.GetWebResponse("https://graph.facebook.com/v3.0/me?access_token=" + accessToken + "&fields=id%2Cname%2Cemail%2Cgender%2Cbirthday%2Cpicture&format=json&method=get&pretty=0&suppress_http_code=1", HttpMethod.Get);
                if (!String.IsNullOrEmpty(jsonResult))
                {
                    var response = JValue.Parse(jsonResult);
                    var error = response.Value<JObject>("error");

                    if (error != null)
                    {
                        Console.WriteLine(error);
                        return Json(new ReturnObj
                        {
                            StatusCode = (int)RegisterStatus.第三方登入失敗,
                            Message = RegisterStatus.第三方登入失敗.ToString(),
                        });
                    }
                    else
                    {
                        var userId = response.Value<string>("id");
                        var userName = response.Value<string>("name");
                        var userEmail = response.Value<string>("email");

                        if (string.IsNullOrWhiteSpace(userEmail))
                        {
                            Console.WriteLine("FB Email is Null");
                            return Json(new ReturnObj
                            {
                                StatusCode = (int)RegisterStatus.第三方授權無法提供您的EMail,
                                Message = "第三方授權無法提供您的E - Mail，請您改用其他方式註冊(登入)",
                            });
                        }

                        var data = service.HasExtauth(userEmail, "FB", userId);

                        if (data.Status == RegisterStatus.第三方登入失敗)
                        {
                            return Json(new ReturnObj
                            {
                                StatusCode = (int)data.Status,
                                Message = data.Status.ToString(),
                            });
                        }

                        if (data.Status == RegisterStatus.有註冊直接登入)
                        {
                            var verifyForSocialData = new VerifyForSocialDto()
                            {
                                Account = userEmail,
                                Pwd = "#",
                                Ip = Request.UserHostAddress,
                                Browser = Request.Browser.Browser,
                                ProvdrType = "FB",
                                UserId = userId
                            };

                            var verifyStatus = service.VerifyForSocial(verifyForSocialData);

                            // Login
                            if (!verifyStatus)
                            {
                                return Json(new ReturnObj
                                {
                                    StatusCode = (int)RegisterStatus.第三方登入失敗,
                                    Message = RegisterStatus.第三方登入失敗.ToString(),
                                });
                            }
                            else
                            {
                                return Json(new ReturnObj
                                {
                                    StatusCode = (int)data.Status,
                                    Message = data.Status.ToString(),
                                });
                            }
                        }
                        else
                        {
                            data.UserId = userId;
                            data.Email = userEmail;
                            data.ProvdrType = "FB";

                            var result = new ReturnObj
                            {
                                StatusCode = (int)data.Status,
                                Message = data.Status.ToString(),
                                Email = userEmail,
                                UserID = userId,
                                UserName = userName,
                                Phone = data.Phone
                            };

                            //sessionRegistrationInfo = result;
                            service.TempRegistrationInfo(new RegisterDto()
                            {
                                Account = userEmail,
                                Pwd = "#",
                                UserId = userId,
                                ProvdrType = "FB",
                                Email = userEmail,
                                ClientIP = Request.UserHostAddress,
                                HasNotifyLetter = "N",
                                CustId = "",
                                RecommendId = "",
                                Mobile = "",
                            },
                           new SorceOptionDto()
                           {
                               otherSorce = "",
                               SorceValue = ""
                           });

                            return Json(result);
                        }
                    }
                }
                else
                {
                    return Json(new ReturnObj
                    {
                        StatusCode = (int)RegisterStatus.第三方登入失敗,
                        Message = RegisterStatus.第三方登入失敗.ToString(),
                    });
                }
            }
             catch (Exception ex)
            {
                return Json(new ReturnObj
                {
                    StatusCode = (int)RegisterStatus.第三方登入失敗,
                    Message = RegisterStatus.第三方登入失敗.ToString(),
                });
            }
        }

        /// <summary>
        /// LoginWithGooglePlus
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public JsonResult LoginWithGooglePlus(string accessToken)
        {
            try
            {
                var clientId = ConfigurationManager.AppSettings["GooglePlusClientId"];

                var jsonResult = this.GetWebResponse("https://people.googleapis.com/v1/people/me?personFields=metadata,names,emailAddresses&access_token=" + accessToken, HttpMethod.Get, null);

                if (!String.IsNullOrEmpty(jsonResult))
                {
                    var response = JValue.Parse(jsonResult);
                    var error = response.Value<JObject>("error");

                    if (error != null)
                    {
                        Console.WriteLine(error.Value<string>("message"));
                        return Json(new ReturnObj
                        {
                            StatusCode = (int)RegisterStatus.第三方登入失敗,
                            Message = RegisterStatus.第三方登入失敗.ToString(),
                        });
                    }
                    else
                    {
                        GoogleAuthModel AuthModel = JsonConvert.DeserializeObject<GoogleAuthModel>(jsonResult);//反序列化

                        var userId = AuthModel.resourceName.Replace("people/", "");

                        Name name_modle = AuthModel.names.Find(x => x.metadata.primary == true);
                        string userName = null;
                        if (name_modle != null)
                        {
                            userName = name_modle.displayName;
                        }
           
                        GoogleAuthEmailAddresses emailAddresses_model = AuthModel.emailAddresses.Find(x => x.metadata.primary == true);
                        string userEmail = null;
                        if (emailAddresses_model != null)
                        {
                            userEmail = emailAddresses_model.value;
                        }

                        // 確認第三方登入的會員有沒有註冊過 沒有的話就讓使用者填寫註冊資料

                        if ( string.IsNullOrWhiteSpace(userEmail))
                        {
                            Console.WriteLine("Google Plus Email is Null");


                            return Json(new ReturnObj
                            {
                                StatusCode = (int)RegisterStatus.第三方授權無法提供您的EMail,
                                Message = "第三方授權無法提供您的E - Mail，請您改用其他方式註冊(登入)",
                            });
                        }

                        var data = service.HasExtauth(userEmail, "GL", userId);
                        
                        if(data.Status == RegisterStatus.第三方登入失敗)
                        {
                            return Json(new ReturnObj
                            {
                                StatusCode = (int)data.Status,
                                Message = data.Status.ToString(),
                            });
                        }

                        if(data.Status == RegisterStatus.有註冊直接登入)
                        {
                            var verifyForSocialData = new VerifyForSocialDto()
                            {
                                Account = userEmail,
                                Pwd = "#",
                                Ip = Request.UserHostAddress,
                                Browser = Request.Browser.Browser,
                                ProvdrType = "GL",
                                UserId = userId
                            };

                            var verifyStatus = service.VerifyForSocial(verifyForSocialData);

                            // Login
                            if (!verifyStatus)
                            {
                                return Json(new ReturnObj
                                {
                                    StatusCode = (int)RegisterStatus.第三方登入失敗,
                                    Message = RegisterStatus.第三方登入失敗.ToString(),
                                });
                            }
                            else
                            {
                                return Json(new ReturnObj
                                {
                                    StatusCode = (int)data.Status,
                                    Message = data.Status.ToString(),
                                });
                            }
                        } 
                        else
                        {
                            data.UserId = userId;
                            data.Email = userEmail;
                            data.ProvdrType = "GL";

                            var result = new ReturnObj
                            {
                                StatusCode = (int)data.Status,
                                Message = data.Status.ToString(),
                                Email = userEmail,
                                UserID = userId,
                                UserName = userName,
                                Phone = data.Phone
                            };

                            //sessionRegistrationInfo = result;
                            service.TempRegistrationInfo(new RegisterDto()
                            {
                                Account = userEmail,
                                Pwd = "#",
                                UserId = userId,
                                ProvdrType = "GL",
                                Email = userEmail,
                                ClientIP = Request.UserHostAddress,
                                HasNotifyLetter = "N",
                                CustId = "",
                                RecommendId = "",
                                Mobile = "",
                            },
                           new SorceOptionDto()
                           {
                               otherSorce = "",
                               SorceValue = ""
                           });

                            return Json(result);
                        }
                    }
                }
                else
                {
                    return Json(new ReturnObj
                    {
                        StatusCode = (int)RegisterStatus.第三方登入失敗,
                        Message = RegisterStatus.第三方登入失敗.ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new ReturnObj
                {
                    StatusCode = (int)RegisterStatus.第三方登入失敗,
                    Message = RegisterStatus.第三方登入失敗.ToString(),
                });
            }
        }

        /// <summary>
        /// 既有會員首次使用第三方授權OTP驗證成功後之處理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BindMember()
        {
            var result = service.Bindextauth(Request.Browser.Browser);
            return Json(new { result = result.RESULT, message = result.ERRMSG });
        }

        [HttpPost]
        public JsonResult RegisterForSocail(string hasNotifyLetter)
        {
            string email;
            var result = service.RegisterForSocial(Request.Browser.Browser, out email, hasNotifyLetter);
            return Json(new { result = result.RESULT, message = result.ERRMSG, email = email });
        }

        [HttpPost]
        public ActionResult EditPassword(EditPasswordViewModel model)
        {
            //取得用戶IP
            string clintIp = "";
            if (string.IsNullOrEmpty(Request.ServerVariables["HTTP_VIA"]))
            {
                //用戶非使用代理伺服器
                clintIp = Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            else
            {
                //用戶使用代理伺服器
                clintIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }

            var result = service.EditPassword(model.Password, clintIp);

            if (!result.RESULT)
                return Json(new { result = result.RESULT, message = result.ERRMSG }, JsonRequestBehavior.AllowGet);
            return Json(new { result = result.RESULT, message = "您的密碼已經更新成功，請繼續瀏覽！", url = Url.Action("Index", "Home") + "#login" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Recaptcha]
        public ActionResult ForgotAcct(ForgetPwdViewModel model)
        {
            var result = service.ForgotAcct(model.Phone);
            if (!result.RESULT)
                result.ERRMSG = "該手機號碼不存在";
            return Json(new { result = result.RESULT, message = result.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComponentsInit()
        {
            var source = service.HasSorce().Data.Select(x => new
            {
                key = x.SorceOptionValue,
                value = x.SorceOptionNME
            }); 
            return Json(new { source }, JsonRequestBehavior.AllowGet);
        }

        #region 共用Function
        private string GetWebResponse(string uri, HttpMethod method, string authorization = null)
        {
            WebRequest request = WebRequest.Create(uri);
            request.Method = method.Method;

            if (authorization != null)
            {
                request.Headers.Add("Authorization", authorization);
            }

            try
            {
                using (var response = request.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return "";
            }
        }

        #endregion

    }
}