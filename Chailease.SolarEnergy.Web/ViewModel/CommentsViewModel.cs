using Chailease.SolarEnergy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class CommentsViewModel : HomeViewModel
    {
        //[StringLength(30, MinimumLength = 6, ErrorMessage = "姓名的長度")]
        [Display(Name = "姓名"), Required(ErrorMessage = "姓名為必填")]
       
        public string CustName { get; set; }

        [RegularExpression("^09[0-9]{8}$", ErrorMessage = "電話格式不正確")]
        public string Tel { get; set; }

        [EmailAddress(ErrorMessage = "E-mail格式不正確")]
        [Display(Name = "E-mail"), Required(ErrorMessage = "E-mail必填")]
        public string Email { get; set; }

        [Display(Name = "問題分類"), Required(ErrorMessage = "問題分類必填")]
        public string QuestionType { get; set; }

        [Display(Name = "問題內容"), Required(ErrorMessage = "問題內容為必填")]
        public string Content { get; set; }

        public IEnumerable<SelectListItem> QuestionTypeSelectList { get; set; }

        public CommentsViewModel()
        {           
            List<SelectListItem> itemList = new List<SelectListItem>
            {
                new SelectListItem() { Value = "0", Text = "請選擇問題分類", Disabled = true, Selected = true },
                new SelectListItem() { Value = "參與出資", Text = "參與出資" },
                new SelectListItem() { Value = "出租屋頂", Text = "出租屋頂" },
                new SelectListItem() { Value = "會員申辦", Text = "會員申辦" },
                new SelectListItem() { Value = "其他建議", Text = "其他建議" }
            };
            QuestionTypeSelectList = itemList;
        }
        public string GoogleRecaptchaValid { get; set; }
    }
}