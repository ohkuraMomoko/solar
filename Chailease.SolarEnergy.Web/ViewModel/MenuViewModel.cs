using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class MenuViewModel
    {
        public MenuViewModel() {
            this.FaqList = new List<PubRef>();
        }
        public LoginUser LoginUserInfo { get; set; }

        public IEnumerable<PubRef> FaqList { get; set; }
    }
}