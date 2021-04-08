using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.Api;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.Commons;
using Chailease.SolarEnergy.Web.Filters;
using Chailease.SolarEnergy.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class RentController : BaseController
    {
        // GET: Rent
        [Route("Rent")]
        public ActionResult Index()
        {
            return View(new RentViewModel());
        }
        public ActionResult Landlord()
        {
            return View(new RentViewModel());
        }
        public ActionResult Success()
        {
            return View(new RentViewModel());
        }
        [HttpPost]
        public ActionResult Landlord(RentViewModel viewModel)
        {
            if (Common.IsValid())
            {
                List<string> ModelErrors = new List<string>();
                if (ModelState.IsValid)
                {
                    try
                    {
                        var result = new RentServices().ContractBuilding(
                            new RentDto()
                            {
                                CUST_NAME = viewModel.CustName,
                                CUST_TEL = viewModel.Tel,
                                CUST_EMAIL = viewModel.Email,
                                CUST_REGION = viewModel.BuildRegion,
                                CUST_BUILD_TYPE = viewModel.BuildType,
                                CUST_BUILD_YEARS = viewModel.BuildYears,
                                //CUST_BUILD_HEIGHT = viewModel.BuildHeight,
                                CUST_BUILD_AREA = viewModel.BuildArea,
                                CUST_BUILD_MEMO = viewModel.BuildMemo
                            });
                        if (result.ERRCODE == 0)
                        {
                            return Json(new Result() { ReturnCode = 0, ReturnMsg = Url.Action("Success", "Rent") });
                        }
                        else
                        {
                            TempData["errorMsg"] = "發送異常:請稍後再試或聯絡客服。";
                            return Json(new Result() { ReturnCode = 2, ReturnMsg = Url.Action("Error", "Home") });
                        }
                    }
                    catch
                    {
                        TempData["errorMsg"] = "發送異常:請稍後再試或聯絡客服。";
                        return Json(new Result() { ReturnCode = 2, ReturnMsg = Url.Action("Error", "Home") });
                    }
                }
                else {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var modelError in modelState.Errors)
                        {
                            ModelErrors.Add(modelError.ErrorMessage);
                        }
                    }
                }
                return Json(new Result() { ReturnCode = 3, ReturnMsg = String.Join(", ", ModelErrors.ToArray()) });
            }

            return Json(new Result() { ReturnCode = 1, ReturnMsg = "Failed Validation Error" });
        }
    }
}