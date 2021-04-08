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
    public class SellViewModel : HomeViewModel
    {
        LoanCaseService loanService { get; set; }

        public SellViewModel()
        {
            End_Flag = false;
            if (UserInfoDetail == null)
            {
                UserInfoDetail = new UserInfoDto() { MBR_LVL = "", VAR_RETURN_NOTE = "" };
            }

            loanService = new LoanCaseService();
        }

        public SellViewModel(string case_Type, string caseNo) : this()
        {
            MemOnSaleSellInfo = new MemOnSaleSellInfoDto() { SELL_NUM = "0", TRANS_NUM = "0" };
            LoanCaseAttach = loanService.LoanCaseAttach(new LoanCaseAttachDto() { CASE_TYPE = case_Type, CASE_NO = caseNo });
            LoanCaseIntroduct = loanService.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = case_Type, CASE_NO = caseNo });
        }

      
        public Boolean End_Flag { get; set; }
        public LoanCaseListResponseDto LoanCaseList { get; set; }
        public LoanCaseIntroductResponseDto LoanCaseIntroduct { get; set; }

        public LoanCaseScheduleDtlResponseDto LoanCaseSchedule { get; set; }
        public LoanCaseCalculateResponseDto LoanCaseCalculate { get; set; }

        public LoanCaseAttachResponseDto LoanCaseFile { get; set; }

        public UserInfoDto UserInfoDetail = new AccountService().GetUserInfo();

        public List<MemOnSaleListDto> MemOnSaleList { get; set; }

        public MemOnSaleDetailDto MemOnSaleDetail { get; set; }

        public LoanCaseAttachResponseDto LoanCaseAttach { get; set; }

        public PayeeResponseDto Payee { get; set; }

        public MemberInfoResponseDto MemberInfo { get; set; }

        public MemOnSaleSellInfoDto MemOnSaleSellInfo { get; set; }
        public MemOnSalepPreConfirmDto MemOnSalepPreConfirmDto { get; internal set; }
        public BaseResultDto<MemOnSalepPreCalDto> MemOnSalepPreCalDto { get; internal set; }
        //轉讓人資訊
        public MemberInfoResponseDto SellMemberInfo { get; set; }
    }
}