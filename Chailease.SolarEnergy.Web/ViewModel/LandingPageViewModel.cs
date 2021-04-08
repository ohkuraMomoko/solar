using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    /// <summary>
    /// Landing Page ViewModel
    /// </summary>
    public class LandingPageViewModel: JoinViewModel
    {
        /// <summary>
        /// Landing Page Dto Model
        /// </summary>
        public LandingPageItemViewModel LandingPage { get; set; }
    }
}