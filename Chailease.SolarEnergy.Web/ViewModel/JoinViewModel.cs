using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class JoinViewModel : HomeViewModel
    {

        public JoinViewModel()
        {
            End_Flag = false;
            if (UserInfoDetail == null)
            {
                UserInfoDetail = new UserInfoDto() { MBR_LVL = "", VAR_RETURN_NOTE = "" };
            }
        }
      
        public Boolean End_Flag { get; set; }
        public LoanCaseListResponseDto LoanCaseList { get; set; }
        public LoanCaseIntroductResponseDto LoanCaseIntroduct { get; set; }

        public LoanCaseScheduleDtlResponseDto LoanCaseSchedule { get; set; }
        public LoanCaseCalculateResponseDto LoanCaseCalculate { get; set; }

        public LoanCaseAttachResponseDto LoanCaseFile { get; set; }

        public UserInfoDto UserInfoDetail = new AccountService().GetUserInfo();
    }
}