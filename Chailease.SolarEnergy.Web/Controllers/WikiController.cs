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
    public class WikiController : BaseController
    {
        [Route("Wiki")]
        public ActionResult Index()
        {
            //TODO 5.要判斷取回資料的有效日期  
            WikiViewModel wikiViewModel = GetWikiViewModel(WikiViewModel.DEFAULT_VIEW_COUNT);
            wikiViewModel.ListType = wikiViewModel.MapTitle.First().Key.ToString();
            return View(wikiViewModel);
        }
        [HttpPost]
        public ActionResult Index(String Type)
        {
            WikiViewModel wikiViewModel = GetWikiViewModel(Type, WikiViewModel.DEFAULT_VIEW_COUNT);
            wikiViewModel.ListType = Type;
            return View(wikiViewModel);
        }

        [HttpPost]
        public PartialViewResult ArticleList(string category, int datacount)
        {
            WikiViewModel wikiViewModel = GetWikiViewModel(category,datacount);

            if (datacount >= WikiViewModel.VIEW_COUNT_INTERVAL)
                wikiViewModel.WikiList = wikiViewModel.WikiList.Skip(datacount - WikiViewModel.VIEW_COUNT_INTERVAL).ToList();
            wikiViewModel.ListType = category;

            return PartialView(wikiViewModel);
        }

        [Route("Wiki/{category}/{articleAlias}", Name = "WikiArticle")]
        public ActionResult Article(string category, string articleAlias)
        {
            WikiViewModel viewModel;
            List<ArticleItem> articleList = new WikiViewModel(category).WikiList.ToList();
            int idx = -1, min_idx = 0, max_idx = articleList.Count() - 1;
            if ((idx = articleList.FindIndex(x => x.ARTICLE_ALIAS == articleAlias)) != -1)
            {

                viewModel = new WikiViewModel()
                {
                    ArticleItem = articleList[idx]
                };
                if (idx > min_idx)
                    viewModel.Prev_Article_Alias = articleList[idx - 1].ARTICLE_ALIAS;
                if (idx < max_idx)
                    viewModel.Next_Article_Alias = articleList[idx + 1].ARTICLE_ALIAS;
                viewModel.ListType = category;
            }
            else{
                return RedirectToAction("NotFound", "Home", null);
            }
            return View(viewModel);
        }

    }
}