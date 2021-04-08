using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Services;
using Chailease.SolarEnergy.Web.ViewModel;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class ComponentsController : Controller
    {
        // GET: Components
        public PartialViewResult _Menu()
        {
            var userInfo = new AccountService().GetUserInfo();
            var menuViewModel = new MenuViewModel();
            if (userInfo != null)
            {
                if (Request.IsAjaxRequest())
                {
                    if (new AccountService().IsAuthorized)
                    {

                        var name = string.IsNullOrEmpty(userInfo.CUST_NAME) ? "親愛的用戶" : userInfo.CUST_NAME;
                        menuViewModel = new MenuViewModel()
                        {
                            LoginUserInfo = new LoginUser() { CustName = name, Lvl = userInfo.MBR_LVL, Greet = "午安" },
                            FaqList = new PubRefServices().GetByRefType("FAQ_TYPE")
                        };
                        return PartialView("_Menu", menuViewModel);
                    }
                }
            }

            return PartialView(menuViewModel);
        }

        public PartialViewResult _Footer()
        {
            return PartialView("_Footer", new MenuViewModel() { FaqList = new Services.PubRefServices().GetByRefType("FAQ_TYPE") });
        }
    }
}