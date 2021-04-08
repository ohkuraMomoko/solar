using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Controllers;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class NewsController : BaseController
    {
        // GET: News
        [Route("News")]
        public ActionResult Index()
        {
            NewsViewModel newsViewModel = GetNewsViewModel(NewsViewModel.DEFAULT_VIEW_COUNT);
            newsViewModel.ListType = newsViewModel.MapTitle.First().Key.ToString();
            return View(newsViewModel);
        }
        [HttpPost]
        public ActionResult Index(String Type)
        {
            NewsViewModel newsViewModel = GetNewsViewModel(Type,NewsViewModel.DEFAULT_VIEW_COUNT);
            newsViewModel.ListType = Type;
            return View(newsViewModel);
        }

        // News: ArticleList Function
        [HttpPost]
        public PartialViewResult ArticleList(string catagory, int datacount)
        {
            //TODO:: 按鈕觸發後 View More 樣式跑掉
            NewsViewModel newsViewModel = GetNewsViewModel(catagory, datacount);
            if(datacount >= NewsViewModel.VIEW_COUNT_INTERVAL)
                newsViewModel.ArticleItemList = newsViewModel.ArticleItemList.Skip(datacount-NewsViewModel.VIEW_COUNT_INTERVAL).ToList();
            newsViewModel.ListType = catagory;
            return PartialView(newsViewModel);
        }

        [Route("News/{category}/{articleAlias}", Name = "NewsArticle")]
        public ActionResult Article(String category, String articleAlias)
        {
            NewsViewModel viewModel = new NewsViewModel(category);
            List<ArticleItem> articleList = viewModel.ArticleItemList.ToList();
            int idx = -1, min_idx = 0, max_idx = articleList.Count() - 1;
            if ((idx = articleList.FindIndex(x => x.ARTICLE_ALIAS == articleAlias)) != -1)
            {
                viewModel.ArticleItem = articleList[idx];
                if (idx > min_idx)
                    viewModel.Prev_Article_Alias = articleList[idx - 1].ARTICLE_ALIAS;
                if (idx < max_idx) 
                    viewModel.Next_Article_Alias = articleList[idx + 1].ARTICLE_ALIAS;
                viewModel.ListType = category;
            }
            else {
                return RedirectToAction("NotFound", "Home", null);
            }
            return View(viewModel);
        }

    }
}