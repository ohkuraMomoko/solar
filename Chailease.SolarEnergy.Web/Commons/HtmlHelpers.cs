using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Commons
{
    public static class HtmlHelpers
    {
        /*public static IHtmlString RecaptchaV2(this HtmlHelper helper)
        {
            var htmlString = $"<div class='g-recaptcha' data-sitekey='{ Common.RecaptchaSite}' data-callback='RecaptchCallBack'></div>";
            return new HtmlString(htmlString);
        }*/

        public static IHtmlString v2CheckecBox(this HtmlHelper helper)
        {
            return v2CheckecBox(helper, string.Empty);
        }

        public static IHtmlString v2CheckecBox(this HtmlHelper helper, string region)
        {
            string html = "";
            //string html = string.Format("<script src =\"https://www.google.com/recaptcha/api.js?hl={0}\" async defer></script>", region);
            //string html = string.Format("<script src =\"https://www.google.com/recaptcha/api.js?hl={0}&onload=onloadCallback&render=explicit\" async defer></script>", region);
            //if (region == string.Empty)
            //{
                //html = "<script src =\"https://www.google.com/recaptcha/api.js\" async></script>";
                //html = "<script src =\"https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit\" async></script>";
            //}
            if (!Common.RecaptchaTestMode)
                html += string.Format("<div class=\"g-recaptcha\" data-sitekey=\"{0}\"  data-callback='RecaptchCallBack'  ></div>", Common.RecaptchaSite);
            else
                html += "<div class=\"g-recaptcha\" data-sitekey=\"6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI\"  data-callback='RecaptchCallBack' ></div>";
            //style='display: none'
            return new HtmlString(html);
        }

    }
}