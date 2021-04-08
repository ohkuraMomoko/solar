using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class ForgetPwdViewModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Captcha { get; set; }
    }
}