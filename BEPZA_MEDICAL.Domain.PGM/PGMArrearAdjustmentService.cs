using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.Utility;
using System;
using System.Linq;


namespace BEPZA_MEDICAL.Domain.PGM
{
    public class PGMArrearAdjustmentService : PGMCommonService
    {
        #region Ctor

        public PGMArrearAdjustmentService(PGM_UnitOfWork uow) : base(uow)
        {
        }

        #endregion

        #region Public Methods

        public void CreateArrearAdjustment(int statusChangeId)
        {
            var empStatusChange = base.PGMUnit.EmpStatusChangeRepository.GetByID(statusChangeId);

            var salaries = base.PGMUnit.SalaryMasterRepository
                .Get(q => q.EmployeeId == empStatusChange.EmployeeId)
                .DefaultIfEmpty()
                .ToList();

            var arrearToDate = DateTime.Now;

            if (salaries != null && salaries.Count > 0)
            {
                var lastSalaryInfo = salaries.OrderByDescending(q => q.SalaryYear).ThenByDescending(q => q.SalaryMonth).FirstOrDefault();
                arrearToDate = UtilCommon.GetMonthLastDate(lastSalaryInfo.SalaryYear, lastSalaryInfo.SalaryMonth);
            }

            if (salaries != null && salaries.Count > 0)
            {
                var arrearFromDate = Convert.ToDateTime(empStatusChange.EffectiveDate);

                if (empStatusChange.EffectiveDate < arrearToDate)
                {
                    var arrearAdjustment = new PGM_ArrearAdjustment
                    {
                        EmpStatusChangeId = empStatusChange.Id,
                        EmployeeId = empStatusChange.EmployeeId,
                        DesignationId = empStatusChange.FromDesignationId,
                        OrderDate = Convert.ToDateTime(empStatusChange.OrderDate),
                        AdjustmentType = empStatusChange.Type,
                        PaymentDate = null,

                        ArrearFromDate = arrearFromDate,
                        ArrearToDate = arrearToDate,
                        EffectiveDate = empStatusChange.EffectiveDate,

                        IsAdjustWithSalary = false,
                        AdjustmentYear = null,
                        AdjustmentMonth = null,

                        IsAdjustmentPaid = false,
                        Remarks = null,
                        IUser = empStatusChange.IUser,
                        IDate = empStatusChange.IDate,
                    };

                    base.PGMUnit.ArrearAdjustmentRepository.Add(arrearAdjustment);
                    base.PGMUnit.ArrearAdjustmentRepository.SaveChanges();
                }
            }
        }


        #endregion
    }
}
