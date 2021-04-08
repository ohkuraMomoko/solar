using Chailease.SolarEnergy.Model.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class MemberViewModel : HomeViewModel
    {       
        public MemOnSaleCenterDto MemOnSaleCenter { get; set; }

        public List<MemOnSaleSellManageDto> MemOnSaleSellManageList { get; set; }

        public List<List<MemOnSaleSellManageBuyerDto>> MemOnSaleSellManageBuyerList { get; set; }

        public MemberViewModel() : base()
        {
            MemOnSaleSellManageBuyerList = new List<List<MemOnSaleSellManageBuyerDto>>();
        }
        public List<MemOnSaleTransBuyRecDto> MemOnSaleTransBuyRecList { get; set; }

        public List<MemOnSaleTransSellRecDto> MemOnSaleTransSellRecList { get; set; }

    }
}