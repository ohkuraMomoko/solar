using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Model.WebApi;
using Chailease.SolarEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chailease.SolarEnergy.Web.Controllers.api
{
    public class LoanCaseController : ApiBaseController
    {

        [HttpPost]
        public WebApiResponse Projectdocument(LoanCaseProjectDocumentRequestApi Requestmodel)
        {
            try
            {
                LoanCaseProjectDocumentApi api = new LoanCaseProjectDocumentApi();
                Contract contract = new ContractService().GetContractByVersion(Requestmodel.Version);
                if (contract != null)
                {
                    api.Version = contract.VERSION;
                    api.DocumentHtml = contract.CONTENT;
                }
                response.setResponse(api);
            }
            catch(Exception ex) {
                response.setErrorResponse();
                if (testMode)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }
        [HttpPost]
        public WebApiResponse Versionlist()
        {
            try
            {
                LoanCaseVersionListApi api = new LoanCaseVersionListApi();
                List<Contract> contractList = new ContractService().GetAllContract();
                List<LoanCaseProjectDocumentApi> list = new List<LoanCaseProjectDocumentApi>();
                foreach (Contract contract in contractList)
                {
                    LoanCaseProjectDocumentApi ele = new LoanCaseProjectDocumentApi();
                    ele.Version = contract.VERSION;
                    ele.DocumentHtml = contract.CONTENT;
                    list.Add(ele);
                }
                api.VersionList = list;
                response.setResponse(api);
            }
            catch(Exception ex) {
                response.setErrorResponse();
                if (testMode)
                {
                    response.Message = ex.ToString();
                }
            }
            return response;
        }

    }
}