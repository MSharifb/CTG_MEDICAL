using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class ResourceInfoService:PRMCommonSevice
    {
   
        #region Ctor

        public ResourceInfoService(PRM_UnitOfWork uow):base(uow)
        {
        }

        #endregion

        #region Workflow methods

        public List<string> GetBusinessLogicValidation(PRM_ResourceInfo ResourceInfo)
        {
            List<string> errorMessage = new List<string>();

            if (ResourceInfo.ActualRate <=0)
            {
                errorMessage.Add("Actual Rate should be greater than zero.");
            }
            if (ResourceInfo.BudgetRate <= 0)
            {
                errorMessage.Add("Budget Rate should be greater than zero.");
            }
            
            return errorMessage;

        }
        #endregion
    }
}
