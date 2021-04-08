using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    /// <summary>
    /// Landing Page Dto Model
    /// </summary>
    public class LandingPageItemViewModel
    {
        /// <summary>
        /// Key 值
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 自定義網址
        /// </summary>
        public string UrlPart { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 背景圖片
        /// </summary>
        public HttpPostedFileBase BannerImg { get; set; }

        /// <summary>
        /// 背景圖片
        /// </summary>
        public string BannerImgName { get; set; }

        /// <summary>
        /// 主題圖片描述
        /// </summary>
        public string ImageAlt { get; set; }

        /// <summary>
        /// 手機圖片
        /// </summary>
        public HttpPostedFileBase PhoneImg { get; set; }

        /// <summary>
        /// 手機圖片
        /// </summary>
        public string PhoneImgName { get; set; }

        /// <summary>
        /// Youtube 網址
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// 影片縮圖
        /// </summary>
        public HttpPostedFileBase VideoImg { get; set; }

        /// <summary>
        /// 影片縮圖
        /// </summary>
        public string VideoImgName { get; set; }

        /// <summary>
        /// 再生能源資訊隱藏
        /// </summary>
        public int IsBlockHide { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public int ItemStatus { get; set; }

        //public LandingStatusEnum ItemStatusEnum
        //{
        //    get => (LandingStatusEnum)this.ItemStatus;
        //    set => this.ItemStatus = (int)value;
        //}

        /// <summary>
        /// 文字上稿
        /// </summary>
        public string ZoneTxt { get; set; }

        /// <summary>
        /// 按鈕文字上稿
        /// </summary>
        public string ZoneButton { get; set; }

        /// <summary>
        /// 按鈕超連接上稿
        /// </summary>
        public string ZoneUrl { get; set; }

        /// <summary>
        /// SEO 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// SEO 關鍵字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 修改人員
        /// </summary>
        public string User { get; set; }
    }
}