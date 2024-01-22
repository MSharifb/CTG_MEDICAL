using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class ResourceCategoryService:PRMCommonSevice
    {
   
        #region Ctor

        public ResourceCategoryService(PRM_UnitOfWork uow):base(uow)
        {
        }

        #endregion

        #region Workflow methods

        public List<string> GetBusinessLogicValidation(PRM_ResourceCategory ResourceCategory)
        {
            List<string> errorMessage = new List<string>();

            if (ResourceCategory.ActualRate <=0)
            {
                errorMessage.Add("Actual Rate should be greater than zero.");
            }
            if (ResourceCategory.BudgetRate <= 0)
            {
                errorMessage.Add("Budget Rate should be greater than zero.");
            }
            return errorMessage;

        }
        #endregion
    }
}
