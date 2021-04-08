using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class MemOnSaleController : BaseController
    {
        MemberService memberService { get; set; }
        MemOnSaleService memonSaleService { get; set; }
        AccountService accountService { get; set; }

        public MemOnSaleController()
        {
            accountService = new AccountService();
            memonSaleService = new MemOnSaleService();
            memberService = new MemberService();
        }

        // GET: MemOnSale
        public JsonResult MemOnSaleLoanCaseChk(string case_No)
        {
            var apiResult = memonSaleService.MemOnSaleLoanCaseChk(case_No);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        public JsonResult MemOnSaleDetailCal(string sh_trans_inst_cd)
        {
#if DEBUG
            //sh_trans_inst_cd = "TEST20191203";
#endif
            var apiResult = memonSaleService.MemOnSaleDetailCal(sh_trans_inst_cd);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        public JsonResult MemOnSaleBuyerWant(string sh_trans_inst_cd, int buy_Num)
        {
            var user = accountService.GetUserInfo(true);
#if DEBUG
            //user.MBR_ID = "M018413E14";
#endif
            var apiResult = memonSaleService.MemOnSaleBuyerWant(user.MBR_ID, sh_trans_inst_cd, buy_Num);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 二手交易受讓人取消關注
        /// </summary>  
        public JsonResult MemOnSaleBuyerNoWant(string sh_trans_inst_cd)
        {
            var user = accountService.GetUserInfo();
#if DEBUG
            //user.MBR_ID = "M018413E14";
#endif
            var apiResult = memonSaleService.MemOnSaleBuyerNoWant(user.MBR_ID, sh_trans_inst_cd);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 二手交易關注狀態更新(更新為已讀)
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult MemOnSaleSellManageWantStatus(string sell_inst_cd)
        {
            var user = accountService.GetUserInfo();
            var apiResult = memonSaleService.MemOnSaleSellManageWantStatus(sell_inst_cd);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 二手交易出售公告管理狀態
        /// </summary>
        public JsonResult MemOnSaleSellManageStatus(string sell_inst_cd, string sell_status)
        {
            var user = accountService.GetUserInfo();
            var apiResult = memonSaleService.MemOnSaleSellManageStatus(sell_inst_cd, sell_status);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 二手交易出售公告管理關注清單_提供帳號
        /// </summary>  
        public JsonResult MemOnSaleSellManageBuyerAcct(string sell_inst_cd, string want_inst_cd, string is_seller_acct)
        {
            var user = accountService.GetUserInfo();
            var apiResult = memonSaleService.MemOnSaleSellManageBuyerAcct(sell_inst_cd, want_inst_cd, is_seller_acct);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        public JsonResult MemOnSalepPreCal(string id, int power_panel_sell_price)
        {
            string mbr_id = accountService.GetUserInfo().MBR_ID;
#if DEBUG
            //mbr_id = "M018223C11";
            //id = "P072017100500179";
            //mbr_id = "M01729A305";
#endif
            var apiResult = memonSaleService.MemOnSalepPreCal(id, mbr_id, power_panel_sell_price);
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        public JsonResult MemOnSalepConfirm(string cal_uuid, string sell_num, string sell_cust_name, string sell_cust_title)
        {
            var apiResult = memonSaleService.GetBaseResult(new { cal_uuid, sell_num, sell_cust_name, sell_cust_title }, "MemOnSalepConfirm");
            return Json(apiResult, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetLoanCaseIntroList(string case_No)
        {
            var model = new LoanCaseService().GetLoanCaseIntroList(new LoanCaseIntroductDto() { CASE_NO = case_No, CASE_TYPE = "3" });
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 會員網頁瀏覽紀錄
        /// </summary>
        /// <param name="sh_trans_inst_cd"></param>
        /// <returns></returns>
        public JsonResult MemOnSaleBuyerView(string sh_trans_inst_cd)
        {
            string mbr_id = this.accountService.GetUserInfo().MBR_ID;
            BaseResultDto model = this.memonSaleService.MemOnSaleBuyerView(mbr_id, sh_trans_inst_cd);
            return this.Json(model, JsonRequestBehavior.DenyGet);
        }
    }
}