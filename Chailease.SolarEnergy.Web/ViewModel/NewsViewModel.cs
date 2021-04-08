using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class NewsViewModel : HomeViewModel
    {

        protected  void Initialize() {

            MapTitle = new Dictionary<string, string>();
            MapRef = new Dictionary<int, PubRef>();
            List<PubRef> pubRefList = new PubRefServices().GetByRefType("SUB_TYPE").ToList();
            pubRefList.Insert(0,new PubRef { ID = 0,CD="ALL",NME="全部類別" });
            foreach (PubRef pbf in pubRefList) {
                MapRef.Add(pbf.ID,pbf);
            }
            SubTypeSelectList = pubRefList
                .Select(x => new SelectListItem() { Text = x.NME, Value = x.CD.ToString() }).ToList();
            foreach (var subType in SubTypeSelectList)
            {
                MapTitle.Add(subType.Value, subType.Text);
            }
        }

        public NewsViewModel()
        {
            Initialize();
            ArticleItemList = new ArticleItemServices().GetNewsList();
            Article_Count = ArticleItemList.Count();
            ListType = MapTitle.First().Key.ToString();
        }
        public NewsViewModel(string subType)
        {
            Initialize();
            String subTypeId = MapRef.Select(x=>x.Value).Where(x=> x.CD == subType ).First().ID.ToString();
            ArticleItemList = String.Equals(subType, MapTitle.First().Key.ToString()) ? 
                new ArticleItemServices().GetNewsList() : new ArticleItemServices().GetNewsList(subTypeId);
            Article_Count = ArticleItemList.Count();
            ListType = subType;
        }

        public const int DEFAULT_VIEW_COUNT = 6; // 初始三筆資料
       
        public readonly int Article_Count = 0;
        public readonly string SubType_SelectList;
        public string ListType { get; set; }
        public string Prev_Article_Alias { get; set; }
        public string Next_Article_Alias { get; set; }
        public IEnumerable<SelectListItem> SubTypeSelectList { get; set; }
        public Dictionary<string, string> MapTitle { get; set; }
        public Dictionary<int, PubRef> MapRef { get; set; }
        public IEnumerable<ArticleItem> ArticleItemList { get; set; }
    }
}