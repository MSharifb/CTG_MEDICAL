using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class DivisionHeadMapingService:PRMCommonSevice
    {
   
        #region Ctor

        public DivisionHeadMapingService(PRM_UnitOfWork uow)
            : base(uow)
        {
        }

        #endregion

        #region Workflow methods

        public string CheckBusinessLogic(PRM_Designation obj)
        {
            string businessError = string.Empty;

          
            //if (joiningDate != null && obj.DateofBirth > joiningDate)
            //{
            //    businessError = "Date of Birth should be lower than Joining Date(" + joiningDate.ToString("dd-MM-yyyy") + ")";
            //    return businessError;
            //}
            //if (obj.MarriageDate != null && obj.DateofBirth > obj.MarriageDate)
            //{
            //    businessError = "Marrige Date should be greater than Date of Birth";
            //    return businessError;
            //}

            return string.Empty;
        }

        #endregion
    }
}
