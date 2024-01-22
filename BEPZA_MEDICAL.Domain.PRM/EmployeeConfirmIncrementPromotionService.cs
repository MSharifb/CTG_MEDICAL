using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.Utility;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class EmployeeConfirmIncrementPromotionService : PRMCommonSevice
    {
        #region Ctor

        public EmployeeConfirmIncrementPromotionService(PRM_UnitOfWork uow)
            : base(uow)
        {

        }

        #endregion

        #region Workflow methods

        //  public List<string> GetBusinessLogicValidation(PRM_EmpStatusChange obj)
        public string GetBusinessLogicValidation(PRM_EmpStatusChange obj)
        {
            //  List<string> errorMessage = new List<string>();

            string errorMessage = string.Empty;

            var enty = (from c in this.PRMUnit.EmploymentInfoRepository.Fetch()
                        where c.Id == obj.EmployeeId
                        select c).FirstOrDefault();

            var fromStep = (from c in this.PRMUnit.JobGradeStepRepository.Fetch()
                            where c.Id == obj.FromStepId
                            select c).FirstOrDefault();

            var toStep = (from c in this.PRMUnit.JobGradeStepRepository.Fetch()
                          where c.Id == obj.ToStepId
                          select c).FirstOrDefault();

            if (enty.DateofJoining > obj.EffectiveDate)
            {
                //errorMessage.Add("Effective Date must be greater than date of joining.");
                return errorMessage = "Effective Date must be greater than date of joining.";
            }

            //var objSeparation = (from c in this.PRMUnit.EmpSeperationRepository.Fetch()
            //                     where c.EmployeeId == obj.EmployeeId
            //                     select c).FirstOrDefault();

            //if (objSeparation != null)
            //{
            //    if (obj.EffectiveDate >= objSeparation.EffectiveDate)
            //    {
            //        //  errorMessage.Add("Employee Confirmation / Increment/ Promotion date must be less than separation date.");
            //        return errorMessage = "Employee Confirmation / Increment/ Promotion date must be less than separation date.";
            //    }
            //}

            if (obj.Type.Equals(PRMEnum.EmployeeStatusChange.Increment.ToString()))
            {
               
                if (!base.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId).IsContractual)
                {
                    if (obj.FromGradeId == obj.ToGradeId)
                    {
                        if (fromStep.StepName >= toStep.StepName)
                        {
                            //errorMessage.Add("During Increment, To Step must be greater than From Step.");
                            return errorMessage = "During Increment, To Step must be greater than From Step.";
                        }
                    }

                }

                if (obj.FromBasicSalary >= obj.ToBasicSalary)
                {
                    //     errorMessage.Add("To Basic should be greater than From Basic.");
                    return errorMessage = "To Basic should be greater than From Basic.";
                }

                if (obj.FromGrossSalary >= obj.ToGrossSalary)
                {
                    // errorMessage.Add("To Gross should be greater than From Gross.");
                    return errorMessage = "To Gross should be greater than From Gross.";
                }

                dynamic ECPE;
                if (obj.Id == 0)
                {
                    ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                            where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type
                            select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                }
                else
                {
                    ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                            where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type && ie.Id != obj.Id
                            select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                }

                if (ECPE != null && ECPE.EffectiveDate >= obj.EffectiveDate)
                {
                    //errorMessage.Add("Last annual increment date must be greater than previous increment date.");
                    return errorMessage = "Last annual increment date must be greater than previous increment date.";
                }
            }

            if (obj.Type.Equals(PRMEnum.EmployeeStatusChange.Promotion.ToString()))
            {
                if (obj.FromDesignationId == obj.ToDesignationId)
                {
                    //  errorMessage.Add("During Promotion, From Designation and To Designation cannot be same.");
                }

                if (obj.FromGradeId == obj.ToGradeId)
                {
                    // errorMessage.Add("During Promotion, From Grade and To Grade cannot be same.");
                    return errorMessage = "During Promotion, From Grade and To Grade cannot be same.";
                }

                if (obj.FromBasicSalary >= obj.ToBasicSalary)
                {
                    // errorMessage.Add("To Basic should be greater than From Basic.");
                    return errorMessage = "To Basic should be greater than From Basic.";
                }

                if (obj.FromGrossSalary >= obj.ToGrossSalary)
                {
                    // errorMessage.Add("To Gross should be greater than From Gross.");
                    return errorMessage = "To Gross should be greater than From Gross.";
                }

                dynamic ECPE;

                if (obj.Id == 0)
                {
                    ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                            where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type
                            select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                }
                else
                {
                    ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                            where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type && ie.Id != obj.Id
                            select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                }
                if (ECPE != null && ECPE.EffectiveDate >= obj.EffectiveDate)
                {
                    // errorMessage.Add("Last promotion date must be greater than previous promotion date.");
                    return errorMessage = "Last promotion date must be greater than previous promotion date.";
                }
            }
            else
            {
                if (obj.Type.Equals(PRMEnum.EmployeeStatusChange.Confirmation.ToString()))
                {
                    dynamic ECPE;

                    if (obj.Id == 0)
                    {
                        ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll() where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                    }
                    else
                    {
                        ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                                where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type && ie.Id != obj.Id
                                select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                    }

                    if (ECPE != null)
                    {
                        //  errorMessage.Add("Couldn't be duplicate entry for employee confirmation.");
                        return errorMessage = "Couldn't be duplicate entry for employee confirmation.";
                    }
                }
            }

            if (obj.Type.Equals(PRMEnum.EmployeeStatusChange.Promotion.ToString())
                || obj.Type.Equals(PRMEnum.EmployeeStatusChange.Demotion.ToString())
                )
            {
                if (obj.ToOrganogramLevelId == null)
                {
                    //  errorMessage.Add("To Organogram level is rquired.");
                    return errorMessage = "To Organogram level is rquired.";
                }
            }

            #region check Punishmnet
            if (obj.Type == "Increment" || obj.Type == "Promotion")
            {
                var list = PRMUnit.FunctionRepository.PRM_FN_GetFinalComplaintEmployee(obj.EmployeeId, obj.Type);
                if (list != null && list.Count > 0)
                {
                    list = list.Where(d => d.EffectiveDateFrom <= obj.EffectiveDate && d.EffectiveDateTo >= obj.EffectiveDate).ToList();
                }

                if (list.Count > 0)
                {
                    // errorMessage.Add("Transfer not applicable because of Punishment.");
                    return errorMessage = "Transfer not applicable because of Punishment.";
                }
            }

            #endregion

            if (obj.Type.Equals(PRMEnum.EmployeeStatusChange.SelectionGrade.ToString()))
            {
                dynamic ECPE;
                if (obj.Id == 0)
                {
                    ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                            where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type
                            select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                }
                else
                {
                    ECPE = (from ie in base.PRMUnit.EmpStatusChangeRepository.GetAll()
                            where ie.EmployeeId == obj.EmployeeId && ie.Type == obj.Type && ie.Id != obj.Id
                            select ie).OrderBy(x => x.EffectiveDate).LastOrDefault();
                }

                if (ECPE != null && ECPE.EffectiveDate >= obj.EffectiveDate)
                {
                    return errorMessage = "Effective date must be greater than previous Effective date.";
                }
            }
            return errorMessage;
        }
        #endregion
    }
}
