using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            this.CONFIG = new SolarConfigService().GetAll().ToDictionary(x=>x.KEY,y=>y.VALUE);
            this.BannerInfoList = new BannerService().GetByOpenDate();
            var userInfo = new Services.AccountService().GetUserInfo();
            this.MenuVwModel = new MenuViewModel()
            {
                FaqList = new PubRefServices().GetByRefType("FAQ_TYPE")
            };
            if (userInfo != null)
            {
                this.MenuVwModel.LoginUserInfo = new LoginUser()
                {
                    CustName = userInfo.CUST_NAME,
                    Lvl = userInfo.MBR_LVL,
                    Greet = "午安"
                };
            }
        }
        public List<LOAN_CASE_LIST> LoadCaseList { get; set; }

        public IEnumerable<BannerInfo> BannerInfoList { get; set; }

        public IEnumerable<LogoInfo> LogoInfoList { get; set; }

        public ArticleItem ArticleItem { get; set; }

        public MenuViewModel MenuVwModel { get; set; }

        public Dictionary<String, String> CONFIG { get; set; }

        public const int VIEW_COUNT_INTERVAL = 6;

        public PoweGenrInfo PoweGenrInfo { get; set; }        
    }
}