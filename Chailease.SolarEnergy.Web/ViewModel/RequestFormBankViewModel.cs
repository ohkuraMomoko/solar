using System.Web;

namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class RequestFormBankViewModel
    {
        public HttpPostedFileBase File { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string BankBranchId { get; set; }
        public string BankBranchName { get; set; }
        public string BankAccount { get; set; }
    }
}