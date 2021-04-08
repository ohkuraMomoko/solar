using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class RentViewModel : HomeViewModel
    {
        protected SmefpRepository smefpRepository;
        //[StringLength(30, MinimumLength = 1, ErrorMessage = "請輸入正確姓名")]
        [Display(Name = "姓名"), Required(ErrorMessage = "姓名為必填")]
        public string CustName { get; set; }
        [Display(Name = "Tel"), Required(ErrorMessage = "電話為必填")]
        [RegularExpression("^09[0-9]{8}$", ErrorMessage = "電話格式不正確，請輸入手機號碼")]
        public string Tel { get; set; }

        [EmailAddress(ErrorMessage = "E-mail格式不正確")]
        public string Email { get; set; }
        [Display(Name = "BuildRegion"), Required(ErrorMessage = "建築物所在地區為必填")]
        public string BuildRegion { get; set; }
        [Display(Name = "BuildType"), Required(ErrorMessage = "建物型式為必填")]
        public string BuildType { get; set; }

        public IEnumerable<SelectListItem> RegionSelectList { get; set; }

        public IEnumerable<SelectListItem> BuildTypeSelectList { get; set; }

        public string BuildYears { get; set; }

        /*[Display(Name = "BuildHeight"), Required(ErrorMessage = "建物樓層為必填")]
        public string BuildHeight { get; set; }*/
        [Display(Name = "BuildArea"), Required(ErrorMessage = "屋頂面積(坪)為必填")]
        [StringLength(8, MinimumLength = 1, ErrorMessage = "超過限制長度")]
        [RegularExpression("^[2-9][0-9][0-9]$|^[1-9][0-9][0-9][0-9]$|^[1-9][0-9][0-9][0-9][0-9]$|^[1-9][0-9][0-9][0-9][0-9][0-9]$|^[1-9][0-9][0-9][0-9][0-9][0-9][0-9]$|^[1-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$", ErrorMessage = "屋頂面積(坪)需大於200坪，請輸入阿拉伯數字")]
        public string BuildArea { get; set; }

        public string BuildMemo { get; set; }
        public string GoogleRecaptchaValid { get; set; }

        public RentViewModel()
        {
            smefpRepository = new SmefpRepository();
            List<SelectListItem> buildTypeList = new List<SelectListItem>
            {
                new SelectListItem() { Value = "0", Text = "請選擇", Disabled = true, Selected = true },
                new SelectListItem() { Value = "3", Text = "廠房/倉儲" },
                new SelectListItem() { Value = "6", Text = "畜(禽)舍" },
                //new SelectListItem() { Value = "5", Text = "大樓/公寓" },
                //new SelectListItem() { Value = "1", Text = "透天" },
                new SelectListItem() { Value = "7", Text = "土地" },
                new SelectListItem() { Value = "8", Text = "其他" }
            };

            List<SelectListItem> regionList = new List<SelectListItem>
            {
                new SelectListItem() { Value = "0", Text = "請選擇", Disabled = true, Selected = true },
            };

            var pubref = smefpRepository.GetPubRefList("LOAN_REGION")
                .Select(x => new SelectListItem() { Text = x.NME, Value = x.ID.ToString() }).ToList();
            regionList.AddRange(pubref);

            BuildTypeSelectList = buildTypeList;
            RegionSelectList = regionList;
        }
    }
}