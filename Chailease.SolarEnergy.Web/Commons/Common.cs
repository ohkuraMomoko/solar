using GoogleRecaptcha;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Chailease.SolarEnergy.Web.Commons
{
    public class Common
    {
        public static bool RecaptchaTestMode => ConfigurationManager.AppSettings["Google-Recaptcha-TestMode"] == "Y";
        public static string RecaptchaSite => ConfigurationManager.AppSettings["RecaptchaSite"]?.ToString();
        private static string GoogleRecaptchaUrl => ConfigurationManager.AppSettings["Google-Recaptcha-Url"]?.ToString();
        protected static string RecaptchaSecret => ConfigurationManager.AppSettings["RecaptchaSecret"]?.ToString();

        /*public static bool VerifyRecaptcha(string Response)
        {
            IRecaptcha<RecaptchaV2Result> recaptcha = new RecaptchaV2(new RecaptchaV2Data()
            {
                Secret = RecaptchaSecret,
                Response = Response
            });
            var result = recaptcha.Verify();
            return result.Success;
        }

        public static bool VerifyRecaptcha()
        {
            IRecaptcha<RecaptchaV2Result> recaptcha = new RecaptchaV2(new RecaptchaV2Data()
            {
                Secret = RecaptchaSecret
            });
            var result = recaptcha.Verify();
            return result.Success;
        }*/

        public static string RootUrl(System.Web.HttpRequestBase request)
        {
            var r = ConfigurationManager.AppSettings["WebsiteRootUrl"]?.ToString();
            return r;
            //return request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
        }
        public static bool IsValid()
        {
            string recaptchaToken = string.Empty;
            return IsValid(recaptchaToken);
        }
        public static bool IsValid(string Response)
        {
            string response = string.IsNullOrEmpty(Response) ?
                    HttpContext.Current.Request["g-recaptcha-response"] :
                    Response;
            bool valid = false;
            /*正式設定*/
            HttpWebRequest req;
            if (!Common.RecaptchaTestMode)
                req = (HttpWebRequest)WebRequest.Create(string.Format("{0}?response={1}", GoogleRecaptchaUrl, response));
            else
                req = (HttpWebRequest)WebRequest.Create(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe", response));
            try
            {
                //Google recaptcha Response
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        dynamic result = new { Success = false };
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var data = js.Deserialize<dynamic>(jsonResponse);
                        valid = data[@"success"];
                    }
                }

                return valid;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

    }
}