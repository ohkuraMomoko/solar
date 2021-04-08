using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class WikiViewModel : HomeViewModel
    {
        protected void Initialize()
        {
            MapTitle = new Dictionary<string, string>();
            MapRef = new Dictionary<int, PubRef>();

            List<PubRef> pubRefList = new PubRefServices().GetByRefType("WIKI_TYPE").ToList();
            pubRefList.Insert(0, new PubRef { ID = 0, CD = "ALL", NME = "全部類別" });
            foreach (PubRef pbf in pubRefList)
            {
                MapRef.Add(pbf.ID, pbf);
            }
            WikiTypeSelectList = pubRefList
                .Select(x => new SelectListItem() { Text = x.NME, Value = x.CD.ToString() }).ToList();
            foreach (var wikiType in WikiTypeSelectList)
            {
                MapTitle.Add(wikiType.Value, wikiType.Text);
            }

        }

        public WikiViewModel()
        {
            Initialize();
            WikiList = new ArticleItemServices().GetWikiList();
            Article_Count = WikiList.Count();
            ListType = MapTitle.First().Key.ToString();
        }

        public WikiViewModel(string wikiType){
            Initialize();
            String wikiTypeId = MapRef.Select(x => x.Value).Where(x => x.CD == wikiType).First().ID.ToString();
            WikiList = String.Equals(wikiType, MapTitle.First().Key.ToString()) ?
                new ArticleItemServices().GetWikiList() : new ArticleItemServices().GetWikiList(wikiTypeId);
            Article_Count = WikiList.Count();
            ListType = wikiType;
        }

        public const int DEFAULT_VIEW_COUNT = 6;
      
        public readonly int WikiType_SelectList;
        public readonly int Article_Count = 0;

        public string ListType { get; set; }
        public string Prev_Article_Alias { get; set; }
        public string Next_Article_Alias { get; set; }
        public IEnumerable<ArticleItem> WikiList { get; set; }

        public IEnumerable<SelectListItem> WikiTypeSelectList { get; set; }

        public Dictionary<string, string> MapTitle { get; set; }
        public Dictionary<int, PubRef> MapRef { get; set; }
    }
}