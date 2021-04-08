using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class ReportsViewModel : HomeViewModel
    {
        public ReportsViewModel()
        {
            ReportsList = new ArticleItemServices().GetReportsList();
            MapMediaTitle = new LogoInfoServices().GetAll().Select(x=>new { ID=x.LOGO_SEQ,NME=x.LOGO_TITLE })
            .ToDictionary(t => t.ID.ToString(), t => t.NME);
            Article_Count = ReportsList.Count();
        }

        public const int DEFAULT_VIEW_COUNT = 6;
       
        public readonly int Article_Count = 0;
        public string Prev_Article_Alias { get; set; }
        public string Next_Article_Alias { get; set; }
        //public readonly string ReportsType_SelectList;
        public Dictionary<string, string> MapMediaTitle { get; set; }
        public IEnumerable<ArticleItem> ReportsList { get; set; }
    }
}