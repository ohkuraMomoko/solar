using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Repository;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    /// <summary>
    /// LandingPage 相關功能
    /// </summary>
    public class LandingPageController : BaseController
    {
        /// <summary>
        /// LandingPage 相關服務
        /// </summary>
        LandingPageService landingPageService { get; set; }

        /// <summary>
        /// 建構子
        /// </summary>
        public LandingPageController()
        {
            landingPageService = new LandingPageService();
        }

        /// <summary>
        /// 檢視 Landing Page 頁面
        /// </summary>
        /// <param name="urlPart">自定義網址</param>
        /// <returns> HomeViewMode 基本資料l </returns>
        [Route("LandingPage/{urlPart}")]
        public ActionResult Index(string urlPart)
        {
            // 如果沒有輸入自訂義網址，直接導向404
            if (string.IsNullOrEmpty(urlPart))
            {
                return new RedirectResult(Url.Action("NotFound", "Home"));
            }

            // 取得 Landing Page資料
            LandingPage landingPage = this.GetLandingPage(urlPart);
            if (landingPage == null)
            {
                // 找不到資料則直接導向404
                return new RedirectResult(Url.Action("NotFound", "Home"));
            }

            ViewBag.Title = landingPage.LandingPageTitle;

            // 取得 HomeViewModel 基本資料
            HomeViewModel viewModel = this.GetHomeViewModel();
            LandingPageViewModel model = new LandingPageViewModel
            {
                LogoInfoList = viewModel.LogoInfoList,
                ArticleItem = viewModel.ArticleItem,
                LandingPage = new LandingPageItemViewModel
                {
                    UrlPart = landingPage.LandingPageAlias,
                    Title = landingPage.LandingPageTitle,
                    StartDate = landingPage.OpenStart.ToString("yyyy/MM/dd HH:mm"),
                    EndDate = landingPage.OpenEnd?.ToString("yyyy/MM/dd HH:mm"),
                    BannerImgName = landingPage.BannerImg,
                    ImageAlt = landingPage.ImageAlt,
                    PhoneImgName = landingPage.PhoneImg,
                    VideoUrl = landingPage.VideoUrl,
                    VideoImgName = landingPage.VideoImg,
                    IsBlockHide = landingPage.IsBlockHide,
                    ItemStatus = landingPage.ItemStatus,
                    ZoneTxt = landingPage.ZoneTxt,
                    ZoneButton = landingPage.ZoneButton,
                    ZoneUrl = landingPage.ZoneUrl,
                    Description = landingPage.SeoDesc,
                    Keywords = landingPage.SeoKeyword
                }
            };

            // 取得參與募資資料 
            string caseType = "3";
            string caseStatus = "01";
            string pageIndex = "1";
            string order = "01";
            LoanCaseService loanCaseService = new LoanCaseService();

            // 取得電廠資訊
            model.LoanCaseList = loanCaseService.GetLoanCaseList(new LoanCaseListDto
            {
                CASE_TYPE = caseType,
                CASE_STATUS = caseStatus,
                PAGE_INDEX = pageIndex,
                PAGE_NUM = JoinViewModel.VIEW_COUNT_INTERVAL.ToString(),
                ORDER = order
            });

            return View(model);
        }

        /// <summary>
        /// 取得 LandingPage 詳細資料
        /// </summary>
        /// <param name="urlPart">自定義網址</param>
        /// <returns>Landing Page entity model</returns>
        public LandingPage GetLandingPage(string urlPart)
        {
            LandingPage landingPage = new LandingPage();
            if (!string.IsNullOrEmpty(urlPart))
            {
                landingPage = landingPageService.GetLandingPageData(urlPart);
            }

            return landingPage;
        }

        public ActionResult GetMeta(LandingPageItemViewModel model)
        {
            MetaModel meta = new MetaModel
            {
                DESC = model.Description,
                KEYWORD = model.Keywords,
                TITLE = model.Title
            };

            if (meta == null)
            {
                meta = new MetaModel();
            }

            return PartialView("~/Views/Shared/_Meta.cshtml", meta);
        }
    }
}