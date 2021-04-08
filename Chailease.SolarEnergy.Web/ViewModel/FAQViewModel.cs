using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class FAQViewModel : HomeViewModel
    {
        public FAQViewModel()
        {
            FAQList = new FAQService().GetAll();
        }

        public IEnumerable<FAQ> FAQList { get; set; }
    }
}