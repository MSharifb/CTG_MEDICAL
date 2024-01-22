using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class SalaryScaleService : PRMCommonSevice
    {
        #region Ctor

        public SalaryScaleService (PRM_UnitOfWork uow) : base(uow)
        {
        }

        #endregion

        #region Workflow methods

        public string GetBusinessLogicValidation(PRM_SalaryScale jobGrade)
        {
            string errorMessage = "";

            //if (jobGrade.NumberOfSteps != jobGrade.PRM_GradeStep.Count())
            //{
            //    errorMessage = "Step name will be multiple records based on number of step specified in the grade.";

            //    return errorMessage;
            //}
            //if (jobGrade.YearlyIncrement > jobGrade.InitialBasic)
            //{
            //    errorMessage = "Increment amount must be less than the initial basic salary";

            //    return errorMessage;
            //}
            //if (jobGrade.LastBasic != (jobGrade.InitialBasic + ((jobGrade.NumberOfSteps - 1) * jobGrade.YearlyIncrement)))
            //{
            //    errorMessage = "Invalid last basic.";

            //    return errorMessage;
            //}

            //if (CheckDuplicate(jobGrade.StaffCategoryId, jobGrade.Id, jobGrade.GradeName))
            //{
            //    errorMessage ="Job grade name exists for the staff category.";
            //    return errorMessage;
            //}          

            return errorMessage;
        }

        private bool CheckDuplicate(int CategoryId, int GradeId, string GradeName)
        {
            bool isDuplicate = false;

            //var grade = (from tr in base.PRMUnit.JobGradeRepository.GetAll()
            //             where tr.Id != GradeId && tr.StaffCategoryId == CategoryId && tr.GradeName.Trim().ToUpper() == GradeName.Trim().ToUpper()
            //             select tr).ToList();
            //if (grade.Count > 0)
            //{
            //    isDuplicate = true;
            //}

            return isDuplicate;
        }
        #endregion
    }
}
