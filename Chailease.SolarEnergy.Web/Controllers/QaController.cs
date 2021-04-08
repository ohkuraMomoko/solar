
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class QaController : BaseController
    {
        // GET: Qa
        [HttpGet]
        [Route("QA/")]
        [Route("QA/{CD}", Name = "QAIndex")]
        public ActionResult Index(String CD = "")
        {
            return View(GetFAQViewModel());
        }

        [HttpPost]
        [Route("QA/GetInstantUserInfo")]
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

    }
}