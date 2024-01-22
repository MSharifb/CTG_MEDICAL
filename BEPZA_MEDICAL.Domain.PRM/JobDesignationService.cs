using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class JobDesignationService:PRMCommonSevice
    {
   
        #region Ctor

        public JobDesignationService(PRM_UnitOfWork uow)
            : base(uow)
        {
        }

        #endregion

        #region Workflow methods

        public List<string> GetBusinessLogicValidation(PRM_Designation item)
        {
            List<string> errorMessage = new List<string>();

            //if (jobGrade.NumberOfSteps != jobGrade.PRM_GradeStep.Count())
            //{
            //    errorMessage.Add("Step name will be multiple records based on number of step specified in the grade.");
            //}
            //if ( jobGrade.YearlyIncrement>jobGrade.InitialBasic)
            //{
            //    errorMessage.Add("Increment amount must be less than the initial basic salary");       
            //}
            //if (jobGrade.LastBasic!=(jobGrade.InitialBasic+jobGrade.NumberOfSteps)*jobGrade.YearlyIncrement)
            //{
            //    errorMessage.Add("Invalid LastBasic.");
            //}
            return errorMessage;       
        
        }
        #endregion
    }
}
