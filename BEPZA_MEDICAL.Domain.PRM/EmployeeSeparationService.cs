using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class EmployeeSeperationService:PRMCommonSevice
    {
   
        #region Ctor

        public EmployeeSeperationService(PRM_UnitOfWork uow):base(uow)
        {
        }

        #endregion

        #region Workflow methods

        public List<string> GetBusinessLogicValidation(PRM_EmpSeperation obj)
        {
            List<string> errorMessage = new List<string>();
            PRM_EmploymentInfo empObj = this.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId);
            List<PRM_EmpContractInfo> empContactLst = this.PRMUnit.EmploymentContractInfoRepository.GetAll().Where(q=>q.EmpoyeeId == obj.EmployeeId).ToList();

            var objConIncrementPro = (from c in this.PRMUnit.EmpStatusChangeRepository.Fetch()
                                 where c.EmployeeId == obj.EmployeeId
                                 select c).FirstOrDefault();

            if (objConIncrementPro != null)
            {
                if (obj.EffectiveDate <= objConIncrementPro.EffectiveDate)
                {
                    errorMessage.Add("Please remove the promotion/Increment/Confirmation at first then separation.");
                }
            }

            if (obj.EDate == null)
            {
                if (empObj.DateofInactive != null)
                {
                    errorMessage.Add("Employee must be Active.");
                }
            }
            if (empObj.DateofJoining != null && obj.ApplicationDate!=null)
            {
                if (obj.ApplicationDate < empObj.DateofJoining)
                {
                    errorMessage.Add("Application Date must be greater than date of joining.");                    
                }               
            }
            //if (empContactLst.Count > 0)
            //{
            //    DateTime contactEndDate = empContactLst.Max(q => q.ContractEndDate);
            //    if (obj.EffectiveDate < contactEndDate)
            //    {
            //        // **** send and email to user *****//
            //        errorMessage.Add("Separation Date should be greater than Contract End Date.");
            //    }
            //}
            
            if (obj.ApplicationDate!=null && obj.EffectiveDate < obj.ApplicationDate)
            {
                errorMessage.Add("Separation Date must be equal or grater than the Application Date.");
            }
            if (obj.isAddAttachment && obj.Attachment.Length == 0)
            {
                errorMessage.Add("Attachment not found.");
            }
            
            return errorMessage;

        }
        #endregion
    }
}
