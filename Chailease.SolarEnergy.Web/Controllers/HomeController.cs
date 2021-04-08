using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Commons;
using Chailease.SolarEnergy.Web.Controllers;
using Chailease.SolarEnergy.Web.ViewModel;
using GoogleRecaptcha;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chailease.SolarEnergy.Web.Filters;
using Chailease.SolarEnergy.Repository;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class HomeController : BaseController
    {
        //protected Boolean TestMode = System.Configuration.ConfigurationManager.AppSettings["testMode"].Equals("N") ? false : true;
        public ActionResult Index()
        {
            // 取得 HomeViewModel 基本資料
            var viewModel = GetHomeViewModel();
            JoinViewModel model = new JoinViewModel
            {
                LogoInfoList = viewModel.LogoInfoList,
                ArticleItem = viewModel.ArticleItem
            };

            // 取得參與募資資料 
            string caseType = "3";
            string caseStatus = "01";
            string pageIndex = "1";
            string order = "01";
            LoanCaseService service = new LoanCaseService();
            model.LoanCaseList = service.GetLoanCaseList(new LoanCaseListDto
            {
                CASE_TYPE = caseType,
                CASE_STATUS = caseStatus,
                PAGE_INDEX = pageIndex,
                PAGE_NUM = JoinViewModel.VIEW_COUNT_INTERVAL.ToString(),
                ORDER = order
            });

            return View(model);
        }

        public ActionResult Guidelines()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult Comments()
        {
            return View(new CommentsViewModel());
        }

        public ActionResult CommentsSuccess()
        {
            return View(new CommentsViewModel());
        }

        [HttpPost]
        public ActionResult Comments(CommentsViewModel viewModel)
        {
                //Recaptcha驗證欄位是否通過
                if (Common.IsValid())
                {
                    //表單驗證欄位是否通過
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var result = new CommentsServices().Add(
                                new CommentsDto()
                                {
                                    CUST_NAME = viewModel.CustName,
                                    CUST_TEL = viewModel.Tel,
                                    CUST_EMAIL = viewModel.Email,
                                    CUST_DATA_TYPE = viewModel.QuestionType,
                                    CUST_MESSAGE = viewModel.Content
                                });
                            if (result.ERRCODE == 0)
                            {
                                return Json(new Result() { ReturnCode = 0, ReturnMsg = Url.Action("CommentsSuccess", "Home") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                TempData["errorMsg"] = "發送異常:請稍後再試或聯絡客服。";
                                return Json(new Result() { ReturnCode = 2, ReturnMsg = Url.Action("Error", "Home") }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch
                        {
                            TempData["errorMsg"] = "發送異常:請稍後再試或聯絡客服。";
                            return Json(new Result() { ReturnCode = 2, ReturnMsg = Url.Action("Error", "Home") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new Result() { ReturnCode = 3, ReturnMsg = "Recaptcha valid but modul not valide" }, JsonRequestBehavior.AllowGet);
                }
            return Json(new Result() { ReturnCode = 1, ReturnMsg = "Failed Validation Error" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult NotFound()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult Maintain()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult Privacy()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult Membership()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult About()
        {
            LoanCaseService service = new LoanCaseService();
            HomeViewModel homeViewModel = GetHomeViewModel();
            homeViewModel.PoweGenrInfo = service.GetPowegenrInfo();
            return View(homeViewModel);
        }

        public ActionResult OperationModel()
        {
            return View(GetHomeViewModel());
        }

        public ActionResult TransferNotice()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "PDF", "轉讓通知書.pdf");
            if (System.IO.File.Exists(path))
                return File(path, "application/pdf");
            throw new Exception("file not found");
        }

        [Auth]
        public ActionResult GetFile(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("file id is empty");

            var f = new Repository.FileRepository().Download(id);
            if (f != null)
                return File(f.File, "application/pdf");
            throw new Exception("file not found");
        }
        [HttpGet]
        public ActionResult Share(String invite)
        {
            ViewBag.shareMember = invite;
            return View(GetHomeViewModel());
        }
    }
}