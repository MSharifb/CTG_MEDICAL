using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class JobGradeService : PRMCommonSevice
    {
        #region Ctor

        public JobGradeService(PRM_UnitOfWork uow): base(uow)
        {

        }

        #endregion

        #region Workflow methods

        public string GetBusinessLogicValidation(PRM_JobGrade jobGrade)
        {
            string errorMessage = "";

            if (jobGrade.NumberOfSteps != jobGrade.PRM_GradeStep.Count())
            {
                errorMessage = "Step name will be multiple records based on number of step specified in the grade.";

                return errorMessage;
            }
            if (jobGrade.YearlyIncrement > jobGrade.InitialBasic)
            {
                errorMessage = "Increment amount must be less than the initial basic salary";

                return errorMessage;
            }
            if (jobGrade.LastBasic != (jobGrade.InitialBasic + ((jobGrade.NumberOfSteps - 1) * jobGrade.YearlyIncrement)))
            {
                errorMessage = "Invalid last basic.";

                return errorMessage;
            }
            return errorMessage;
        }

        private bool CheckDuplicate(int CategoryId, int GradeId, string GradeName)
        {
            bool isDuplicate = false;

            var grade = (from tr in base.PRMUnit.JobGradeRepository.GetAll()
                         where tr.Id != GradeId 
                         && tr.GradeName.Trim().ToUpper() == GradeName.Trim().ToUpper()
                         select tr).ToList();
            if (grade.Count > 0)
            {
                isDuplicate = true;
            }

            return isDuplicate;
        }
        #endregion

        
        #region Public Method

        public IList<PRM_JobGrade> GetLatestJobGrade()
        {
            var maxSalaryScale = base.PRMUnit.SalaryScaleRepository.Get(t => t.DateOfEffective <= DateTime.Now).OrderByDescending(t => t.DateOfEffective).FirstOrDefault();

            if (maxSalaryScale == null) return null;

            var q = PRMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == maxSalaryScale.Id);
            return q.ToList();
        }

        #endregion
    }
}
