using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Controllers;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WebApplication1.Controllers
{
    public class JoinController : BaseController
    {
        LoanCaseService service { get; set; }
        AccountService accountService { get; set; }
     
        public JoinController()
        {
            service = new LoanCaseService();
            accountService = new AccountService();          
        }
        [Route("Join")]
        public ActionResult Index()
        {
            JoinViewModel model = new JoinViewModel();
            if (model.UserInfoDetail == null)
            {
                UserInfoDto user = new UserInfoDto() { MBR_LVL = "", VAR_RETURN_NOTE = "" };
                model.UserInfoDetail = user;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ProjectList(int status, int page, string orderType)
        {
            try
            {
                JoinViewModel model = new JoinViewModel();
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

                model.LoanCaseList = service.GetLoanCaseList(new LoanCaseListDto() { CASE_TYPE = "3", CASE_STATUS = case_status, PAGE_INDEX = page.ToString(), PAGE_NUM = JoinViewModel.VIEW_COUNT_INTERVAL.ToString(), ORDER = orderType });

                #region view more隱藏(使用API的方法) 
                //int next_idx = page * JoinViewModel.VIEW_COUNT_INTERVAL + 1;
                //if (service.GetLoanCaseList(new LoanCaseListDto() { CASE_TYPE = "3", CASE_STATUS = case_status, PAGE_INDEX = next_idx.ToString(), PAGE_NUM = "1", ORDER = orderType }).Data.Count() <= 0)
                //model.End_Flag = true;
                #endregion

                if (model.LoanCaseList.Data == null|| model.LoanCaseList.Data.Count()<=0)
                    model.End_Flag = true;
                // view_name -> 募資中 : "_IndexProjectContent" ; 發電中 : "_OnlineProjectContent"
                return PartialView(view_name, model);
            }
            catch (Exception ex) {
                TempData["errorMsg"] = "專案列表載入失敗:請稍後再試或聯絡客服。";
                return Json(new { errorcode = -1, ERRMSG=Url.Action("Error","Home")},JsonRequestBehavior.AllowGet);
            }
            
            
        }

        [HttpPost]
        public ActionResult NewPrjNotice(string MBR_ID, string STATUS, string CASE_NO, string NOTICE_TYPE)
        {
            NoticeDto model = new NoticeDto()
            {
                MBR_ID = MBR_ID,
                STATUS = STATUS,
                CASE_NO = CASE_NO
            };

            NoticeResponseDto response = service.SetProjectNotice(model, NOTICE_TYPE);
            return Json(new { result = response.RESULT, msg = response.ReturnMsg });
        }


        public ActionResult Calculate()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult Online()
        {
            JoinViewModel model = new JoinViewModel();
            
            return View(model);
        }

        public ActionResult Project(string id = "")
        {
            JoinViewModel model = new JoinViewModel();

            try
            {
                model.LoanCaseIntroduct = service.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = "3", CASE_NO = id });
                model.LoanCaseSchedule = service.GetProjectSchedule(new LoanCaseScheduleDtlDto() { CASE_TYPE = "3", CASE_NO = id });
                model.LoanCaseCalculate = service.GetProjectCalculate(new LoanCaseCalculateDto() { CASE_NO = id });
                Session["Project_LoanCaseCalculate"] = model.LoanCaseCalculate;
            }
            catch (Exception ex)
            {
                TempData["errorMsg"] = ex.Message.Substring(0, ex.Message.IndexOf("statusCode"));
                return RedirectToAction("Error", "Home");
            }

            if (model.UserInfoDetail == null)
            {
                UserInfoDto user = new UserInfoDto() { MBR_LVL = "00", VAR_RETURN_NOTE = "" };
                model.UserInfoDetail = user;
            }
            
            #region 專案附件
            LoanCaseAttachResponseDto attach = new LoanCaseAttachResponseDto();
            try
            {
                attach = service.LoanCaseAttach(new LoanCaseAttachDto() { CASE_TYPE = "3", CASE_NO = id });
            }
            catch (Exception ex)
            {
                TempData["errorMsg"] = ex.Message.Substring(0, ex.Message.IndexOf("statusCode"));
                return RedirectToAction("Error", "Home");
            }
            
            model.LoanCaseFile = attach;
            #endregion

            return View(model);

        }
        
        [HttpPost]
        public ActionResult GetDiscount(string id, string discountCode,string count,string memid)
        {
            LoanCaseDiscountDto model = new LoanCaseDiscountDto() { CASE_TYPE = "3", CASE_NO = id, DISCOUNT_CODE = discountCode, DISCOUNT_PANEL_NUM = count, MBR_ID = memid };
            LoanCaseDiscountResponseDto discountInfo = new LoanCaseDiscountResponseDto();
            try
            {
                discountInfo = service.GetDiscount(model);
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
                response = service.LoanCaseInvest(model);
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
            JoinViewModel viewModel = new JoinViewModel();
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

    }
}