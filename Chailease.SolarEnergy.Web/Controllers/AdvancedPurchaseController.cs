using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Commons;
using Chailease.SolarEnergy.Web.Filters;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class AdvancedPurchaseController : Controller
    {
        AccountService acct { get; set; }
        MemberService member { get; set; }
        PubRefServices pubRef { get; set; }
        AdvancedPurchaseService service { get; set; }

        public AdvancedPurchaseController()
        {
            acct = new AccountService();
            member = new MemberService();
            pubRef = new PubRefServices();
            service = new AdvancedPurchaseService();
        }

        [Auth]
        public ActionResult Index(string Back = "")
        {
            var u = acct.GetUserInfo(true);
            ViewBag.Email = u.MBR_ACCT;

            var first = Session["AdvancedPurchaseFirst"]?.ToString();
            if (string.IsNullOrEmpty(first))
            {
                Session["AdvancedPurchaseFirst"] = '1';
                ViewBag.FirstInto = 1;
            }
            else
                ViewBag.FirstInto = string.Empty;

            if (u.VRF_EMAIL?.ToUpper() != "Y")
            {
                return View(new HomeViewModel());
            }
            if (u.VRF_EMAIL?.ToUpper() == "Y" & u.VRF_BASEINFO_NOW == "01")
            {
                return View("RequestForm", new HomeViewModel());
            }
            if (u.VRF_EMAIL?.ToUpper() == "Y" & u.VRF_ACCTINFO_NOW == "01")
            {
                ViewBag.name = member.Info()?.CUST_NAME;
                return View("RequestFormBank", new HomeViewModel());
            }
            ViewBag.VAR_RETURN_NOTE = u.VAR_RETURN_NOTE;
            if (string.IsNullOrEmpty(Back))
                ViewBag.Back = Url.Action("index", "join");
            else
            {
                if (Back.StartsWith("~"))
                    Back = Back.Substring(1);
                if (Back.StartsWith("/"))
                    Back = Back.Substring(1);
                ViewBag.Back = Back;
            }
            return View("FormResult", new HomeViewModel());
        }

        [HttpPost]
        public ActionResult SendEmailValidata()
        {
            var r = acct.SendEmailValidate("N", Common.RootUrl(Request) + "/AdvancedPurchase/EmailValidata");
            return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmailValidata(string s, string a)
        {
            var r = acct.EmailValidate(s, a);
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

        public ActionResult GetUserAuth()
        {
            var u = acct.GetUserInfo(true);
            return Json(new { u.VRF_EMAIL }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestFormInit()
        {
            var pubref = pubRef.GetAll();
            var county = pubref.Where(x => x.REF_TYPE == "COUNTY").Select(x => new { value = x.NME, key = x.CD });
            var dist = pubref.Where(x => x.REF_TYPE == "DIST").Select(x => new { value = x.NME, key = x.CD });
            return Json(new { county, dist }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RequestFormImg(HttpPostedFileBase Front, HttpPostedFileBase Back)
        {
            try
            {
                var u = new UploadHelpers();
                var FrontPath = u.Register(Front);
                var BackPath = u.Register(Back);
                service.Save2Session(BackPath, FrontPath);
                //var result = service.Ocr(FrontPath, BackPath, Front, Back);
                //return Json(result, JsonRequestBehavior.AllowGet);
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw new Exception("檔案上傳失敗", e);
            }
        }

        [HttpPost]
        public ActionResult RequestForm(RequestFormViewModel model)
        {
            var PSN_BIRTHDAY = new DateTime(int.Parse(model.BirthdayYear), int.Parse(model.BirthdayMonth), int.Parse(model.BirthdayDay), 0, 0, 0, DateTimeKind.Local);
            var diff = DateTime.Now.AddYears(-20) - PSN_BIRTHDAY;
            if (diff.TotalDays < 0)
                return Json(new { result = false, message = "生日有誤，申請會員年齡不得小於20歲" }, JsonRequestBehavior.AllowGet);

            
            var u = acct.GetUserInfo();

            if (acct.HasCustId(model.Id,u.MBR_ID))
                return Json(new { result = false, message = "身分證帳號已存在" }, JsonRequestBehavior.AllowGet);

            var r1 = service.RealRegisterIdcardApply();
            if (!r1.RESULT)
            {
                return Json(new { result = false, message = r1.ERRMSG }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var r = member.Modify(new MemberModifyDto()
                {
                    MBR_ID = u.MBR_ID,
                    MBR_ACCT = u.MBR_ACCT,
                    CUST_ID = model.Id,
                    CUST_KIND = "2",
                    CUST_NAME = model.Name,
                    EMAIL = u.MBR_ACCT,
                    CONTACT_ZIP_CODE = model.ContactZipCode,
                    CONTACT_COUNTY = model.ContactCounty,
                    CONTACT_DIST = model.ContactDist,
                    CONTACT_ADDR = model.ContactAddr,
                    PSN_BIRTHDAY = PSN_BIRTHDAY.ToString("yyyyMMdd"),
                    PSN_GENDER = model.Gender,
                    PSN_REG_ZIP_CODE = model.RegZipCode,
                    PSN_REG_COUNTY = model.RegCounty,
                    PSN_REG_DIST = model.RegDist,
                    PSN_REG_ADDR = model.RegAddr,
                });
                return Json(new { result = r.RESULT, message = r.ERRMSG }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RequestFormBank(RequestFormBankViewModel model)
        {
            var fileName = new UploadHelpers().Register(model.File);
            var result = service.Finish(new Chailease.SolarEnergy.Model.AdvancedPurchaseDto()
            {
                BankImg = fileName,
                PAYEE_ACCT = model.BankAccount,
                PAYEE_BANK = model.BankName + (String.IsNullOrEmpty(model.BankBranchName)?"": model.BankBranchName),
                PAYEE_BANK_ID = model.BankId,
                PAYEE_BRANCH_ID = model.BankBranchId
            });
            return Json(new { result = result.RESULT, message = result.ERRMSG, url = Url.Action("FormResult") }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult RequestForm()
        //{
        //    return View(new HomeViewModel());
        //}
        //public ActionResult RequestFormBank()
        //{
        //    return View(new HomeViewModel());
        //}
        //[Auth]
        //public ActionResult FormResult()
        //{
        //    return View(new HomeViewModel());
        //}
    }
}