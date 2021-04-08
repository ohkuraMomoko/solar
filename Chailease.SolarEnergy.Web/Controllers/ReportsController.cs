using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class ReportsController : BaseController
    {
        // GET: Reports
        [Route("Reports")]
        public ActionResult Index()
        {
            ReportsViewModel reportViewModel = GetReportsViewModel(ReportsViewModel.DEFAULT_VIEW_COUNT);
            reportViewModel.LogoInfoList = new LogoInfoServices().GetAllUsingLogo();
            return View(reportViewModel);
        }

        [HttpPost]
        public PartialViewResult ArticleList(int datacount) {

            ReportsViewModel reportViewModel =  GetReportsViewModel(datacount);
            if (datacount >= ReportsViewModel.VIEW_COUNT_INTERVAL)
                reportViewModel.ReportsList = reportViewModel.ReportsList.Skip(datacount - ReportsViewModel.VIEW_COUNT_INTERVAL).ToList();
            return PartialView(reportViewModel);
        }

        [Route("Reports/All/{articleAlias}", Name = "ReportsArticle")]
        public ActionResult Article(string articleAlias)
        {
            //TODO:麵包屑只有兩層
            ReportsViewModel viewModel;

            List<ArticleItem> articleList = new ReportsViewModel().ReportsList.ToList();
            int idx = -1, min_idx = 0, max_idx = articleList.Count() - 1;
            
            if ((idx = articleList.FindIndex(x => x.ARTICLE_ALIAS == articleAlias)) != -1)
            {
                viewModel = new ReportsViewModel()
                {
                    ArticleItem = articleList[idx]
                };
                if (idx > min_idx)
                    viewModel.Prev_Article_Alias = articleList[idx - 1].ARTICLE_ALIAS;
                if (idx < max_idx)
                    viewModel.Next_Article_Alias = articleList[idx + 1].ARTICLE_ALIAS;
            }
            else
            {
                return RedirectToAction("NotFound", "Home", null);
            }
            return View(viewModel);
        }
    }
}