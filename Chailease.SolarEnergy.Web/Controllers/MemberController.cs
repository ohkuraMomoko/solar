using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Commons;
using Chailease.SolarEnergy.Web.Controllers;
using Chailease.SolarEnergy.Web.Filters;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class MemberController : BaseController
    {
        AccountService accountService { get; set; }
        MemberService memberService { get; set; }

        MemOnSaleService memOnSaleService { get; set; }

        public MemberController()
        {
            accountService = new AccountService();
            memberService = new MemberService();
            memOnSaleService = new MemOnSaleService();
        }
        [Route("Member")]
        [Auth]
        public ActionResult Index()
        {
            MemberViewModel viewModel = new MemberViewModel();
            var userInfo = accountService.GetUserInfo(true);
            viewModel.MemOnSaleCenter = memOnSaleService.MemOnSaleCenter(userInfo.MBR_ID).Data;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult IndexInit()
        {
            if (accountService.IsAuthorized)
            {
                var userInfo = accountService.GetUserInfo(true);
                var auth = new
                {
                    //MemberStatus = (int)member.GetSataus(userInfo.MBR_ID),
                    userInfo.VRF_SMS,
                    userInfo.VRF_EMAIL,
                    userInfo.MBR_LVL,
                    userInfo.VRF_BASEINFO_NOW,
                    userInfo.VRF_ACCTINFO_NOW,
                    VRF_ACCTINFO_UPDATE = userInfo.VRF_ACCTINFO_UPDATE == "Y" ? true : false
                };
                var loan = memberService.LoanListByMember(userInfo.MBR_ID);
                return Json(new { auth, loan }, JsonRequestBehavior.AllowGet);
            }
            else
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLoanListByMember(string MemberId)
        {
            return Json(memberService.LoanListByMember(MemberId), JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Dashboard()
        {
            return View(new HomeViewModel());
        }

        [HttpPost]
        public ActionResult PlantList()
        {
            var r = memberService.InvestLoanCaseList();
            if (r.RESULT)
            {
                var plant = r.Data?.Select(x => new { key = x.MAJ_SEQ_ID, value = x.LOAN_CASE_NAME });
                return Json(new { result = r.RESULT, plant }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = r.RESULT }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PropertySolarPanel(string MAJ_SEQ_ID, string PAGE_INDEX)
        {
            var plant = memberService.PropertySolarPanel(MAJ_SEQ_ID, PAGE_INDEX);
            return Json(new { plant }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Interest(string id = "")
        {
            ViewBag.id = id;
            return View(new HomeViewModel());
        }

        public ActionResult _InterestDesktop()
        {
            return PartialView();
        }

        public ActionResult _InterestMobile()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult MyInvestSum(string MajSeqId, string StartDate, string EndDate)
        {
            var r = memberService.MyInvestSum(MajSeqId, StartDate.Replace("/", ""), EndDate.Replace("/", "")).Data;
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DetailSolarPanel(string MajSeqId, string StartDate, string EndDate, string PageIndex)
        {
            var r = memberService.DetailSolarPanel(MajSeqId, StartDate.Replace("/", ""), EndDate.Replace("/", ""), PageIndex);
            return Json(new { data = r.Data, result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DetailSolarPanelContent(string MajSeqId, string PrdNum)
        {
            var r = memberService.DetailSolarPanelContent(MajSeqId, PrdNum);
            return Json(r.Data.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DetailSolarPanel_EINV(string CompId, string INVNO)
        {
            if (string.IsNullOrEmpty(INVNO))
                return Json(new { data = new { }, result = false, message = "發票號碼不存在" }, JsonRequestBehavior.AllowGet);
            var r = memberService.DetailSolarPanel_EINV(CompId, INVNO);
            return Json(new { data = r.Data.FirstOrDefault(), result = r.RESULT, message = r.RESULT ? string.Empty : "發票號碼不存在" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DetailSolarPanel_POWER(string MAJSEQID)
        {
            var r = memberService.DetailSolarPanel_POWER(MAJSEQID);
            return Json(new { data = r.Data, result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Transaction()
        {
            return View(new HomeViewModel());
        }

        [HttpPost]
        public ActionResult InvestList(string StatusId)
        {
            var investList = memberService.InvestList(StatusId);
            var u = accountService.GetUserInfo(true);
            if (u.MBR_LVL != "03")
            {
                investList.ForEach(x =>
                {
                    x.BNK_VR_ACCNTNO = string.Empty;
                    x.PAY_DUE_DT = string.Empty;
                });
            }
            return Json(new { investList }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult InvestContract(string id)
        {
            var contract = new ContractService().GetContractByVersion(id);
            if (contract != null)
                return File(new PdfHalpers().ToPdf(contract.CONTENT), "application/pdf");
            throw new Exception("file not found");
        }

        [HttpPost]
        public ActionResult InvestCancel(string OrderNo, string OrderSeqId)
        {
            var r = memberService.InvestCancel(OrderNo, OrderSeqId);
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _TransactionDesktop()
        {
            var u = accountService.GetUserInfo(true);
            ViewBag.isMember03 = u.MBR_LVL == "03" ? true : false;
            return PartialView();
        }

        public ActionResult _TransactionMobile()
        {
            var u = accountService.GetUserInfo(true);
            ViewBag.isMember03 = u.MBR_LVL == "03" ? true : false;
            return PartialView();
        }

        [Auth]
        public new ActionResult Profile()
        {
            var u = accountService.GetUserInfo(true);
            var isShow = u.MBR_LVL == "03" | u.MBR_LVL == "02";
            if (isShow)
            {
                return View(new HomeViewModel());
            }
            return RedirectToAction("Event");
        }
        [HttpPost]
        public ActionResult ProfileInit()
        {
            return Json(memberService.ProfileInit(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ModifyAddress(RequestFormViewModel model)
        {
            var u = accountService.GetUserInfo();
            var r = memberService.ModifyAddress(new MemberModifyDto()
            {
                MBR_ID = u.MBR_ID,
                CONTACT_ZIP_CODE = model.ContactZipCode,
                CONTACT_COUNTY = model.ContactCounty,
                CONTACT_DIST = model.ContactDist,
                CONTACT_ADDR = model.ContactAddr,
            });
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendEmailValidata(string NewMemberAcct)
        {
            var r = memberService.SendEmailValidate(NewMemberAcct, Common.RootUrl(Request) + "/Member/EmailValidata");
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmailValidata(string s, string a)
        {
            var r = memberService.EmailValidate(s, a);
            if (r.RESULT)
            {
                return RedirectToAction("MailVerifySuccess", "Member");
            }
            else
            {
                TempData["errorMsg"] = r.ERRMSG;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult GetRegPhoneMessage(string PhoneNo)
        {
            var result = memberService.GetRegPhoneMessage(PhoneNo);
            return Json(new { result = result.RESULT, message = result.ERRMSG, result.RES_SEC }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VerifyRegPhoneMessage(string Message, string PhoneNo)
        {
            var result = memberService.VerifyRegPhoneMessage(Message, PhoneNo);
            return Json(new { result = result.RESULT, message = result.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadIdCard(HttpPostedFileBase Front, HttpPostedFileBase Back)
        {
            var u = new UploadHelpers();
            var result = memberService.RealRegisterApply(u.Register(Front), u.Register(Back), true);
            var status = string.Empty;
            if (result.RESULT)
            {
                var auth = accountService.GetUserInfo(true);
                status = auth.VRF_BASEINFO_NOW;
            }
            return Json(new { result = result.RESULT, status = status, message = result.RESULT ? string.Empty : "送出資料發生異常，請稍後再試或聯絡客服" }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Password()
        {
            var u = accountService.GetUserInfo(true);
            ViewBag.IsShow = u.MBR_LVL == "03" | u.MBR_LVL == "02";
            return View(new HomeViewModel());
        }
        [HttpPost]
        public ActionResult Password(string Old, string New)
        {
            var r = memberService.ModifyPassword(Old, New, Request.UserHostAddress, Request.Browser.Browser);
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult BankInfo()
        {
            var u = accountService.GetUserInfo(true);
            var isShow = u.MBR_LVL == "03" | u.MBR_LVL == "02";
            if (isShow)
            {
                return View(new HomeViewModel());
            }
            return RedirectToAction("Event");
        }

        [HttpPost]
        public ActionResult BankInfoInit()
        {
            return Json(memberService.BankInfoInit(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBankName()
        {
            var bankCode = memberService.GetBankCode().Select(x => new { key = x.Key, value = x.Value });
            return Json(new { bankCode }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankBranchName(string code)
        {
            var bankNames = memberService.GetBankBranchName(code).Select(x => new { key = x.Key, value = x.Value });
            return Json(new { bankNames }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BankInfo(RequestFormBankViewModel model)
        {
            bool rresult;
            var u = accountService.GetUserInfo(true);

            if (u.VRF_ACCTINFO_NOW == "02")
                return Json(new { result = false, message = "資料審查中，暫不接受更新資料" }, JsonRequestBehavior.AllowGet);

            var fileName = new UploadHelpers().Register(model.File);
            if (u.VRF_ACCTINFO_NOW == "03")
            {
                //更換銀行帳戶
                var result = memberService.PayeeApplyModify(new PayeeApplyModifyDto()
                {
                    IMG = fileName,
                    PAYEE_ACCT = model.BankAccount,
                    PAYEE_BANK = model.BankName + (String.IsNullOrEmpty(model.BankBranchName) ? "" : model.BankBranchName),
                    PAYEE_BANK_ID = model.BankId,
                    PAYEE_BRANCH_ID = model.BankBranchId
                });
                rresult = result.RESULT;
            }
            else
            {
                //退件，重新送審
                if (rresult = memberService.RealRegisterApplyBankAddDoc(fileName, model.BankAccount, model.BankName, model.BankId, model.BankBranchId).RESULT)
                {
                    //存則資料更新
                    rresult = memberService.Modify(new MemberModifyDto()
                    {
                        MBR_ACCT = u.MBR_ACCT,
                        MBR_ID = u.MBR_ID,
                        PAYEE_ACCT = model.BankAccount,
                        PAYEE_BANK = model.BankName + (String.IsNullOrEmpty(model.BankBranchName) ? "" : model.BankBranchName),
                        PAYEE_BANK_ID = model.BankId,
                        PAYEE_BRANCH_ID = model.BankBranchId
                    }).RESULT;
                }
            }
            return Json(new { result = rresult, message = rresult ? string.Empty : "送出資料發生異常，請稍後再試或聯絡客服" }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Event()
        {
            var u = accountService.GetUserInfo(true);
            ViewBag.id = u.MBR_ID;
            ViewBag.recommendId = (u.RecommendUser == null) ? "" : u.RecommendUser;
            ViewBag.baseUrl = Common.RootUrl(Request) + "?invite=" + u.MBR_ID + "#signup";
            ViewBag.lineBaseUrl = Common.RootUrl(Request) + "/Home/Share?invite=" + u.MBR_ID;
            ViewBag.IsShow = u.MBR_LVL == "03" | u.MBR_LVL == "02";
            return View(new HomeViewModel());
        }

        [HttpPost]
        public ActionResult Event(string id)
        {
            var r = accountService.UpdateRecommend(id);
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult NewsletterSetting()
        {
            return View(new HomeViewModel());
        }

        [HttpPost]
        public ActionResult NewsNoticeClose()
        {
            var r = memberService.NewsNoticeClose();
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MailVerifySuccess()
        {
            return View(new HomeViewModel());
        }

        #region 二手交易轉讓紀錄
        [Auth]
        public ActionResult Transfer()
        {
            MemberViewModel viewModel = new MemberViewModel();
            return View(viewModel);
        }
        [Auth]
        public ActionResult _TransferDesktop()
        {
            MemberViewModel viewModel = new MemberViewModel();
            UserInfoDto userInfo = this.accountService.GetUserInfo(true);
            LoanCaseService service = new LoanCaseService();
            viewModel.MemOnSaleTransSellRecList = this.memOnSaleService.MemOnSaleTransSellRecList(userInfo.MBR_ID, 1, MemberViewModel.VIEW_COUNT_INTERVAL).Data;

            // 取得 MAJ_SEQ_ID
            viewModel.MemOnSaleTransSellRecList?.ForEach(x => x.MAJ_SEQ_ID = service.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = "3", CASE_NO = x.SELL_CASE_NO }).CASELOAN_INTRODCT.MAJ_SEQ_ID);

            return this.PartialView(viewModel);
        }
        [Auth]
        public ActionResult _TransferMobile()
        {
            MemberViewModel viewModel = new MemberViewModel();
            UserInfoDto userInfo = this.accountService.GetUserInfo(true);
            LoanCaseService service = new LoanCaseService();
            viewModel.MemOnSaleTransSellRecList = this.memOnSaleService.MemOnSaleTransSellRecList(userInfo.MBR_ID, 1, MemberViewModel.VIEW_COUNT_INTERVAL).Data;

            // 取得 MAJ_SEQ_ID
            viewModel.MemOnSaleTransSellRecList?.ForEach(x => x.MAJ_SEQ_ID = service.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = "3", CASE_NO = x.SELL_CASE_NO }).CASELOAN_INTRODCT.MAJ_SEQ_ID);

            return this.PartialView(viewModel);
        }

        #endregion

        #region 二手交易受讓紀錄
        [Auth]
        public ActionResult Accept()
        {
            MemberViewModel viewModel = new MemberViewModel();
            return View(viewModel);
        }
        [Auth]
        public ActionResult _AcceptDesktop()
        {
            return PartialView(_AcceptViewModel());
        }
        [Auth]
        public ActionResult _AcceptMobile()
        {
            return PartialView(_AcceptViewModel());
        }

        private MemberViewModel _AcceptViewModel()
        {
            MemberViewModel viewModel = new MemberViewModel();
            UserInfoDto userInfo = this.accountService.GetUserInfo(true);

            viewModel.MemOnSaleTransBuyRecList = this.memOnSaleService.MemOnSaleTransBuyRecList(userInfo.MBR_ID, 1, MemberViewModel.VIEW_COUNT_INTERVAL).Data;
            LoanCaseService service = new LoanCaseService();

            // 取得 MAJ_SEQ_ID
            viewModel.MemOnSaleTransBuyRecList?.ForEach(x => x.MAJ_SEQ_ID = service.GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_TYPE = "3", CASE_NO = x.SELL_CASE_NO }).CASELOAN_INTRODCT.MAJ_SEQ_ID);

            if (viewModel.MemOnSaleTransBuyRecList == null)
            {
                viewModel.MemOnSaleTransBuyRecList = new List<MemOnSaleTransBuyRecDto>();
            }

            return viewModel;
        }
        #endregion

        #region  二手交易出售公告管理
        [Auth]
        public ActionResult Annouce()
        {
            MemberViewModel viewModel = new MemberViewModel();
            return View(viewModel);
        }

        public ActionResult _AnnouceDesktop(int page_index, string sort_type = "01")
        {
            var viewModel = _AnnouceViewModel(page_index, sort_type);
            return PartialView(viewModel);
        }

        public ActionResult _AnnouceMobile(int page_index, string sort_type = "01")
        {
            var viewModel = _AnnouceViewModel(page_index, sort_type);
            return PartialView(viewModel);
        }

        private MemberViewModel _AnnouceViewModel(int page_index, string sort_type)
        {
            MemberViewModel viewModel = new MemberViewModel();
            var userInfo = accountService.GetUserInfo(true);
            viewModel.MemOnSaleSellManageList = memOnSaleService.MemOnSaleSellManage(userInfo.MBR_ID, sort_type, 1, MemberViewModel.VIEW_COUNT_INTERVAL * page_index).Data;

            if (viewModel.MemOnSaleSellManageList != null)
            {
                foreach (var m in viewModel.MemOnSaleSellManageList)
                {
                    List<MemOnSaleSellManageBuyerDto> lsData = memOnSaleService.MemOnSaleSellManageBuyer(m.SELL_INST_CD).Data;
                    if (lsData == null)
                    {
                        viewModel.MemOnSaleSellManageBuyerList.Add(new List<MemOnSaleSellManageBuyerDto>());
                    }
                    else
                        viewModel.MemOnSaleSellManageBuyerList.Add(lsData);
                    if (m.LAST_FEE_DIST_AVG != "0") {
                        m.LAST_FEE_DIST_AVG = Math.Round(Double.Parse(m.LAST_FEE_DIST_AVG), 0, MidpointRounding.AwayFromZero).ToString("#,#");
                    }
                    
                }
            }

            return viewModel;
        }
        #endregion

        public ActionResult CancelCaseOpen(string mbrId)
        {
            return View(new HomeViewModel());
        }
    }
}