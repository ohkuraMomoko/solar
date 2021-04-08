using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Controllers;
using Chailease.SolarEnergy.Web.Filters;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class SellController : BaseController
    {
        private LoanCaseService loanService { get; set; }
        private AccountService accountService { get; set; }
        private FileService fileService { get; set; }
        private MemberService memberService { get; set; }
        private MemOnSaleService memOnSaleService { get; set; }

        public SellController()
        {
            loanService = new LoanCaseService();
            accountService = new AccountService();
            fileService = new FileService();
            memberService = new MemberService();
            memOnSaleService = new MemOnSaleService();
        }

        public ActionResult Index()
        {
            var user = accountService.GetUserInfo();
            SellViewModel apiResult = new SellViewModel();
            string mbr_id = string.Empty;
            if (user != null)
            {
                mbr_id = user.MBR_ID;
#if DEBUG
                //mbr_id = "M018223C11";
#endif
                apiResult.MemOnSaleList = memOnSaleService.MemOnSaleList("01", mbr_id, 1, SellViewModel.VIEW_COUNT_INTERVAL).Data;
                if (apiResult.MemOnSaleList != null)
                {
                    //foreach (var item in apiResult.MemOnSaleList)
                    //{
                    //    LoanCaseIntroductResponseDto loanCaseIntroduct = loanService.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = "3", CASE_NO = item.CASE_NO });
                    //    if (loanCaseIntroduct != null)
                    //    {
                    //        DownloadFileRDto file = fileService.Download(loanCaseIntroduct.CASELOAN_INTRODCT.CASE_IMG_LIST[0].FILE_UUID);
                    //        item.CASE_IMG = file;
                    //    }

                    //}
                }
            };
            return View(apiResult);
        }

        [HttpPost]
        public JsonResult SellList(string sort_Type, int pageIndex, int page_Num)
        {
            SellViewModel apiResult = new SellViewModel();
            if (apiResult.UserInfoDetail == null)
            {
                UserInfoDto user = new UserInfoDto() { MBR_LVL = "", VAR_RETURN_NOTE = "" };
                apiResult.UserInfoDetail = user;
            }
            string mbr_id = apiResult.UserInfoDetail.MBR_ID == null ? "" : apiResult.UserInfoDetail.MBR_ID;

#if DEBUG
            // mbr_id = "M018223C11";
#endif
            apiResult.MemOnSaleList = memOnSaleService.MemOnSaleList(sort_Type, mbr_id, pageIndex, SellViewModel.VIEW_COUNT_INTERVAL).Data;

            if (apiResult.MemOnSaleList != null)
            {
                foreach (var item in apiResult.MemOnSaleList)
                {
                    LoanCaseIntroductResponseDto loanCaseIntroduct = loanService.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = "3", CASE_NO = item.CASE_NO });
                    if (loanCaseIntroduct != null)
                    {
                        item.CASE_IMG = !string.IsNullOrEmpty(loanCaseIntroduct.CASELOAN_INTRODCT.CASE_IMG_LIST[0].FILE_UUID) ? loanCaseIntroduct.CASELOAN_INTRODCT.CASE_IMG_LIST[0].FILE_UUID : string.Empty;
                    }
                }
            }
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NoticeCaseOpen(string mbr_id, string case_No, string status)
        {
            if (mbr_id == null)
                mbr_id = accountService.GetUserInfo().MBR_ID;
#if DEBUG
            //mbr_id = "M018223C11";
#endif
            var apiResult = memOnSaleService.NoticeCaseOpen(mbr_id, case_No, status);
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProjectList(int status, int page, string orderType)
        {
            try
            {
                SellViewModel model = new SellViewModel();
                string case_status = "01";
                string view_name = "";
                if (page < 0) page = 0;
                // 1 : 募資中 2 : 發電中
                switch (status)
                {
                    case 1: case_status = "01"; view_name = "_IndexProjectContent"; break;
                    case 2: case_status = "02"; view_name = "_OnlineProjectContent"; break;
                    default: return PartialView(); // status亂傳直接回空response
                }

                model.LoanCaseList = loanService.GetLoanCaseList(new LoanCaseListDto() { CASE_TYPE = "3", CASE_STATUS = case_status, PAGE_INDEX = page.ToString(), PAGE_NUM = SellViewModel.VIEW_COUNT_INTERVAL.ToString(), ORDER = orderType });

                #region view more隱藏(使用API的方法)

                //int next_idx = page * SellViewModel.VIEW_COUNT_INTERVAL + 1;
                //if (service.GetLoanCaseList(new LoanCaseListDto() { CASE_TYPE = "3", CASE_STATUS = case_status, PAGE_INDEX = next_idx.ToString(), PAGE_NUM = "1", ORDER = orderType }).Data.Count() <= 0)
                //model.End_Flag = true;

                #endregion view more隱藏(使用API的方法)

                if (model.LoanCaseList.Data == null || model.LoanCaseList.Data.Count() <= 0)
                    model.End_Flag = true;
                // view_name -> 募資中 : "_IndexProjectContent" ; 發電中 : "_OnlineProjectContent"
                return PartialView(view_name, model);
            }
            catch (Exception ex)
            {
                TempData["errorMsg"] = "專案列表載入失敗:請稍後再試或聯絡客服。";
                return Json(new { errorcode = -1, ERRMSG = Url.Action("Error", "Home") }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult NewPrjNotice(string status, string case_no, string notice_type)
        {
            string mbr_id = accountService.GetUserInfo().MBR_ID;
#if DEBUG
            //mbr_id = "M018223C11";
#endif
            NoticeDto model = new NoticeDto()
            {
                MBR_ID = mbr_id,
                STATUS = status,
                CASE_NO = case_no
            };

            NoticeResponseDto response = loanService.SetProjectNotice(model, notice_type);
            return Json(new { result = response.RESULT, msg = response.ReturnMsg });
        }

        public ActionResult Calculate(string id)
        {
            string mbr_id = accountService.GetUserInfo().MBR_ID;
#if DEBUG
            //mbr_id = "M018223C11";
            //id = "P072017100500179";
            //mbr_id = "M01729A305";
#endif
            SellViewModel model = new SellViewModel
            {
                MemOnSalepPreConfirmDto = memOnSaleService.MemOnSalepPreConfirm(id, mbr_id).Data
            };

            model.MemOnSalepPreConfirmDto.CASE_NO = id;
            int power_panel_sell_price = 100;
#if DEBUG
            //id = "P072017100500179";
            //mbr_id = "M017070605";
            //power_panel_sell_price = 100;
#endif
            //model.MemOnSalepPreCalDto = memOnSaleService.MemOnSalepPreCal(id, mbr_id, power_panel_sell_price);
            //if (model.MemOnSalepPreCalDto.Data == null)
            //{
            model.MemOnSalepPreCalDto = new BaseResultDto<MemOnSalepPreCalDto>
            {
                Data = new MemOnSalepPreCalDto()            //}
            };
            model.MemberInfo = memberService.Info();
            return View(model);
        }

        public ActionResult Online()
        {
            SellViewModel model = new SellViewModel();
            return View(model);
        }

        [Auth]
        public ActionResult Project(string id = "")
        {
            // TODO:測試一下，有將變數caseNo改成id
            SellViewModel viewModel = new SellViewModel();
            UserInfoDto user = this.accountService.GetUserInfo();
            if (user != null)
            {
                string mbr_id = user.MBR_ID;
                string sh_trans_inst_cd = id;
#if DEBUG
                //sh_trans_inst_cd = "P072018111400183";
                //mbr_id = "M018413E14";
#endif
                BaseResultDto<MemOnSaleDetailDto> saleDetailResult = this.memOnSaleService.MemOnSaleDetail(mbr_id, sh_trans_inst_cd);
                if (saleDetailResult.ERRCODE == 0)
                {
                    MemOnSaleDetailDto data = saleDetailResult.Data;
                    viewModel = new SellViewModel(data.CASE_TYPE, data.CASE_NO);
                    viewModel.MemberInfo = this.memberService.Info();
                    viewModel.MemOnSaleSellInfo = this.memOnSaleService.MemOnSaleSellInfo(data.SELL_MBR_ID).Data;

                    //if (data.SELL_STATUS == "01" )
                    //{
                       
                    //}
                    if (data.IS_BUYER == "Y")
                    {
                        viewModel.SellMemberInfo = this.memberService.GetSellMemberInfo(data.SELL_MBR_ID);
                    }
                    if (data.IS_SELLER_ACCT == "Y")
                    {
                        viewModel.Payee = this.memberService.GetPayee(data.SELL_MBR_ID);
                    }
                    if (data.LAST_FEE_DIST_AVG != "0")
                    {
                        data.LAST_FEE_DIST_AVG = Math.Round(Double.Parse(data.LAST_FEE_DIST_AVG), 0, MidpointRounding.AwayFromZero).ToString("#,#");
                    }

                    viewModel.MemOnSaleDetail = data;
                }
            }

            return this.View(viewModel);
        }

        [HttpPost]
        public ActionResult GetDiscount(string id, string discountCode, string count, string memid)
        {
            LoanCaseDiscountDto model = new LoanCaseDiscountDto() { CASE_TYPE = "3", CASE_NO = id, DISCOUNT_CODE = discountCode, DISCOUNT_PANEL_NUM = count, MBR_ID = memid };
            LoanCaseDiscountResponseDto discountInfo = new LoanCaseDiscountResponseDto();
            try
            {
                discountInfo = loanService.GetDiscount(model);
            }
            catch (Exception ex)
            {
                discountInfo.DISCOUNT = 0;
            }

            return Json(new { discountInfo });
        }

        [HttpPost]
        public ActionResult LoanCaseInvest(string MBR_ID, string CASE_NO, string INVEST_AMT, string INVEST_NUM,
            string MBR_LVL, string DISCOUNT_CODE, string DISCOUNT_AMT, string CONTRACT_VERSION)
        {
            LoanCaseInvestDto model = new LoanCaseInvestDto()
            {
                CASE_TYPE = "3",
                MBR_ID = MBR_ID,
                CASE_NO = CASE_NO,
                INVEST_AMT = INVEST_AMT,
                INVEST_NUM = INVEST_NUM,
                MBR_LVL = MBR_LVL,
                DISCOUNT_CODE = DISCOUNT_CODE,
                DISCOUNT_AMT = DISCOUNT_AMT,
                CONTRACT_VERSION = CONTRACT_VERSION
            };

            LoanCaseInvestResponseDto response = new LoanCaseInvestResponseDto();
            try
            {
                response = loanService.LoanCaseInvest(model);
            }
            catch (Exception ex)
            {
                return Json(new { response = new { RESULT = false, ERRMSG = ex.Message } });
            }

            return Json(new { response });
        }

        [HttpPost]
        public ActionResult GetContractByVersion(string version)
        {
            var result = new ContractService().GetContractByVersion(version);
            if (result == null)
            {
                result = new Contract();
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult LoadMoneyTable(int buy_cnt, string id)
        {
            SellViewModel viewModel = new SellViewModel();
            try
            {
                var calmodel = Session["Project_LoanCaseCalculate"] as LoanCaseCalculateResponseDto;

                LoanCaseCalculateResponseDto model = new LoanCaseCalculateResponseDto();
                model.CALUATLE_LIST = new List<CALUATLE_LIST>();

                decimal PRE_POWER_GEN = 0;
                decimal PRE_POWER_FEE = 0;
                decimal PRE_APPOR_RENT = 0;
                decimal PRE_APPOR_INSURANCE = 0;
                decimal PRE_OTHER_FEE = 0;
                decimal PRE_ANNUAL_RECEIPT = 0;
                decimal PRE_ACCUMLATE_RECEIPT = 0;

                foreach (var item in calmodel.CALUATLE_LIST)
                {
                    CALUATLE_LIST lists = new CALUATLE_LIST();
                    decimal.TryParse(item.PRE_POWER_GEN, out PRE_POWER_GEN);
                    decimal.TryParse(item.PRE_POWER_FEE, out PRE_POWER_FEE);
                    decimal.TryParse(item.PRE_APPOR_RENT, out PRE_APPOR_RENT);
                    decimal.TryParse(item.PRE_APPOR_INSURANCE, out PRE_APPOR_INSURANCE);
                    decimal.TryParse(item.PRE_OTHER_FEE, out PRE_OTHER_FEE);
                    decimal.TryParse(item.PRE_ANNUAL_RECEIPT, out PRE_ANNUAL_RECEIPT);
                    decimal.TryParse(item.PRE_ACCUMLATE_RECEIPT, out PRE_ACCUMLATE_RECEIPT);
                    PRE_POWER_GEN *= buy_cnt;
                    PRE_POWER_FEE *= buy_cnt;
                    PRE_APPOR_RENT *= buy_cnt;
                    PRE_APPOR_INSURANCE *= buy_cnt;
                    PRE_OTHER_FEE *= buy_cnt;
                    PRE_ANNUAL_RECEIPT *= buy_cnt;
                    PRE_ACCUMLATE_RECEIPT *= buy_cnt;

                    lists.YEAR = item.YEAR;
                    lists.PRE_POWER_PERFORMANCE = item.PRE_POWER_PERFORMANCE;
                    lists.PRE_POWER_GEN = PRE_POWER_GEN.ToString();
                    lists.PRE_POWER_FEE = PRE_POWER_FEE.ToString();
                    lists.PRE_APPOR_RENT = PRE_APPOR_RENT.ToString();
                    lists.PRE_APPOR_INSURANCE = PRE_APPOR_INSURANCE.ToString();
                    lists.PRE_OTHER_FEE = PRE_OTHER_FEE.ToString();
                    lists.PRE_ANNUAL_RECEIPT = PRE_ANNUAL_RECEIPT.ToString();
                    lists.PRE_ACCUMLATE_RECEIPT = PRE_ACCUMLATE_RECEIPT.ToString();

                    model.CALUATLE_LIST.Add(lists);
                }
                viewModel.LoanCaseCalculate = model;

                return PartialView("_ProjectTabMoneyTable", viewModel);
            }
            catch (Exception ex)
            {
                TempData["errorMsg"] = "error";
                return Json(new { errorcode = -1, ERRMSG = Url.Action("Error", "Home") }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 取得轉讓者聯絡資訊
        /// </summary>
        /// <param name="sellMbrId">轉讓者會員編號</param>
        /// <returns>轉讓者會員資料</returns>
        [HttpPost]
        public ActionResult GetSellMemberInfo(string sellMbrId)
        {
            try
            {
                BaseResultDto<MemberInfoResponseDto> result = new BaseResultDto<MemberInfoResponseDto>
                {
                    Data = this.memberService.GetSellMemberInfo(sellMbrId),
                    ERRCODE = 0,
                    RESULT = true,
                    ERRMSG = string.Empty
                };

                return this.Json(result);
            }
            catch (Exception ex)
            {
                return this.Json(new { response = new { RESULT = false, ERRMSG = ex.Message } });
            }
        }
    }
}