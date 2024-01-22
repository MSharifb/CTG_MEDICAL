using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BEPZA_MEDICAL.DAL.FAM;


namespace BEPZA_MEDICAL.Domain.FAM
{
    public class FAMFinancialYearService:FAMCommonService
    {
        #region Constructor

        public FAMFinancialYearService(FAM_UnitOfWork uow)
            : base(uow) { }

        #endregion

        #region Business Logic Validation

        public string CheckBusinessLogicFinancialYear(FAM_FinancialYearInformation obj)
        {
            string businessError = string.Empty;

            if (obj.FinancialYearStartDate >= obj.FinancialYearEndDate)
            {
                businessError = "Financial Year End Date must be greater than Financial Year Start Date.";
                return businessError;
            }

            DateTime? financialYearStart = (from c in FAMUnit.FinancialYearInformationRepository.Fetch()
                                     where c.FinancialYearStartDate >= obj.FinancialYearStartDate
                                            select c.FinancialYearStartDate).FirstOrDefault();

            DateTime? financialYearEnd = (from c in FAMUnit.FinancialYearInformationRepository.Fetch()
                                            where c.FinancialYearEndDate >= obj.FinancialYearStartDate
                                            select c.FinancialYearEndDate).FirstOrDefault();

            if (financialYearStart.HasValue && (financialYearStart.Value >= obj.FinancialYearStartDate))
            {
                businessError = "Financial Year Start Date must be greater than all saved Financial Year Start Date .";
                return businessError;
            }

            if (financialYearEnd.HasValue && (financialYearEnd.Value >= obj.FinancialYearStartDate))
            {
                businessError = "Financial Year Start Date must be greater than all saved Financial Year End Date .";
                return businessError;
            }


            //Check for multiple active 
            if (obj.IsActive == true)
            {
                bool? active = (from c in FAMUnit.FinancialYearInformationRepository.Fetch()
                                where c.IsActive == true
                                select c.IsActive).FirstOrDefault();
                if (active.HasValue && (active.Value == true))
                {
                    businessError = "Multiple Financial Year cannot be Active at the same time";
                    return businessError;
                }
            }
            

            return string.Empty;
        }

        public string CheckBusinessLogicFinancialYearEdit(FAM_FinancialYearInformation obj)
        {
            string businessError = string.Empty;

            //if (obj.FinancialYearStartDate >= obj.FinancialYearEndDate)
            //{
            //    businessError = "Financial Year End Date must be greater than Financial Year Start Date.";
            //    return businessError;
            //}

            //DateTime? financialYearStart = (from c in FAMUnit.FinancialYearInformationRepository.Fetch()
            //                                where c.FinancialYearStartDate >= obj.FinancialYearStartDate && c.Id != obj.Id
            //                                select c.FinancialYearStartDate).FirstOrDefault();


            //if (financialYearStart.HasValue && (financialYearStart.Value >= obj.FinancialYearStartDate))
            //{
            //    businessError = "Financial Year Start Date must be greater than all saved Financial Year Start Date .";
            //    return businessError;
            //}



            //Check for multiple active 
            //if (obj.IsActive == true)
            //{
            //    bool? active = (from c in FAMUnit.FinancialYearInformationRepository.Fetch()
            //                    where c.IsActive == true && c.Id != obj.Id
            //                    select c.IsActive).FirstOrDefault();
            //    if (active.HasValue && (active.Value == true))
            //    {
            //        businessError = "Multiple Financial Year cannot be Active at the same time";
            //        return businessError;
            //    }
            //}

            return string.Empty;
        }

        

        #endregion
    }
}
