using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.ViewModel;
using System.Linq;
using System.Web.Mvc;
using Chailease.SolarEnergy.Model;
using System;
using Chailease.SolarEnergy.Model.Api;
using System.Web;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class BaseController : Controller
    {
        LoanCaseService loanService { get; set; }
        FileService fileService { get; set; } 

        #region HomeViewModel Get-Method 
        protected HomeViewModel GetHomeViewModel()
        {
            return new HomeViewModel
            {
                LogoInfoList = new LogoInfoServices().GetHomeLogo(),
                ArticleItem = new ArticleItemServices().GetNewsItem()
            };
        }
        #endregion

        #region ReportsViewModel Get-Method
        protected ReportsViewModel GetReportsViewModel(int dataCount)
        {
            ReportsViewModel reportsViewModel = new ReportsViewModel();
            if (reportsViewModel.ReportsList.Count() >= dataCount)
            {
                reportsViewModel.ReportsList = reportsViewModel.ReportsList.Take(dataCount);
            }
            return reportsViewModel;
        }
        #endregion

        #region WikiViewModel Get-Method 
        protected WikiViewModel GetWikiViewModel(int dataCount)
        {
            WikiViewModel wikiViewModel = new WikiViewModel();

            if (wikiViewModel.WikiList.Count() >= dataCount)
                wikiViewModel.WikiList = wikiViewModel.WikiList.Take(dataCount);

            return wikiViewModel;
        }

        protected WikiViewModel GetWikiViewModel(string dataType, int dataCount)
        {

            WikiViewModel wikiViewModel = new WikiViewModel(dataType);
            if (wikiViewModel.WikiList.Count() >= dataCount)
            {
                wikiViewModel.WikiList = wikiViewModel.WikiList.Take(dataCount);
            }
            return wikiViewModel;
        }
        #endregion

        #region FAQViewModel Get-Method
        protected FAQViewModel GetFAQViewModel()
        {
            return new FAQViewModel();
        }
        #endregion

        #region NewsViewModel Get-Method

        protected NewsViewModel GetNewsViewModel(int dataCount)
        {

            NewsViewModel newsViewModel = new NewsViewModel();

            if (newsViewModel.ArticleItemList.Count() >= dataCount)
                newsViewModel.ArticleItemList = newsViewModel.ArticleItemList.Take(dataCount);

            return newsViewModel;
        }

        protected NewsViewModel GetNewsViewModel(string dataType, int dataCount)
        {

            NewsViewModel newsViewModel = new NewsViewModel(dataType);

            if (newsViewModel.ArticleItemList.Count() >= dataCount)
            {
                newsViewModel.ArticleItemList = newsViewModel.ArticleItemList.Take(dataCount);
            }
            return newsViewModel;
        }
        #endregion

        protected JsonResult GetConfig()
        {
            SolarConfigService solarConfigService = new SolarConfigService();
            solarConfigService.GetAll();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region meta
        public ActionResult GetMetaByType(string id = "0")
        {
            MetaModel meta = new MetaServices().GetMetaByType(id);
            if (meta == null)
            {
                meta = new MetaModel();
            }
            return PartialView("~/Views/Shared/_Meta.cshtml", meta);
        }
        public ActionResult GetArticleMetaByType(ArticleItem model)
        {
            return PartialView("~/Views/Shared/_ArticleMeta.cshtml", model);
        }

        #endregion

        [HttpPost]
        public ActionResult GetUserInfo()
        {
            var result = new AccountService().GetUserInfo(true);
            if (result == null)
            {
                result = new UserInfoDto();
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetInstantUserInfo()
        {
            var userInfo = new AccountService().GetUserInfo(true);
            // 沒有的 LEVEL 要不要給00 ? 
            var instantInfo =
                userInfo == null ? new InstantUserInfoDto() : new InstantUserInfoDto
                {
                    // 會員代碼
                    ID = userInfo.MBR_ID,
                    // 會員權限 00:非會員 01:一般會員 02:出資會員(審核中) 03:出資會員(完成實名認證)
                    LEVEL = userInfo.MBR_LVL,
                    // 基本資料是否送審狀態(01：從未送過, 02：送審中, 03：審核完畢 ,04：退件)
                    BASEINFO = userInfo.VRF_BASEINFO_NOW,
                    //帳戶資料是否送審狀態(01：從未送過, 02：送審中, 03：審核完畢 ,04：退件)
                    ACCTION = userInfo.VRF_ACCTINFO_NOW,
                    //是否被曾被退件(Y/N)
                    RETURN_NOTE = userInfo.VAR_RETURN_NOTE
                };
            return Json(instantInfo);
        }

        public ActionResult LoanCaseFile(string FILE_UUID)
        {
            String contentType = "";
            String fileName = "";           
            DownloadFileRDto file = new DownloadFileRDto();
            try
            {
                fileService = new FileService();
                file = fileService.Download(FILE_UUID);
                fileName = file.FileName;
                contentType = MimeMapping.GetMimeMapping(fileName);
            }
            catch (Exception ex)
            {
                return null;
            }
            //return new FileContentResult(file.File, contentType);
            return File(file.File, contentType, fileName);
        }
    }  

}