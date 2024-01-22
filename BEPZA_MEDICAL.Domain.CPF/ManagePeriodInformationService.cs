using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Domain.PGM;
using BEPZA_MEDICAL.Domain.PRM;


namespace BEPZA_MEDICAL.Domain.CPF
{
    public class ManagePeriodInformationService : CPFCommonService
    {
        #region Constructor

        public ManagePeriodInformationService(CPF_UnitOfWork uow)
            : base(uow) { }

        public ManagePeriodInformationService(CPF_UnitOfWork uow, PGMCommonService pgmCommon, PRMCommonSevice prmCommon)
            : base(uow, pgmCommon, prmCommon) { }

        #endregion

        #region Business Logic Validation

        //public string CheckBusinessLogicManagePeriodInformation(CPF_Period obj)
        //{
        //    string businessError = string.Empty;

        //    if (obj.StartDate >= obj.EndDate)
        //    {
        //        businessError = "End Date must be greater than Start Date.";
        //        return businessError;
        //    }

        //    //Check for multiple active 

        //    //if (obj.IsActive == true)
        //    //{
        //    //    bool? active = (from c in CPFUnit.ManagePeriodInformationRepository.Fetch()
        //    //                    where c.IsActive == true
        //    //                    select c.IsActive).FirstOrDefault();
        //    //    if (active.HasValue && (active.Value == true))
        //    //    {
        //    //        businessError = "Multiple Period Information can not be Active at the same time";
        //    //        return businessError;
        //    //    }
        //    //}


        //    return string.Empty;
        //}

        //public string CheckBusinessLogicManagePeriodInformationEdit(CPF_Period obj)
        //{
        //    string businessError = string.Empty;

        //    // TODO: Validation with voucher
        //    //var voucher = (from v in this.CPFUnit.VoucherRepository.GetAll() where v.PeriodId == obj.Id select v).ToList();
        //    //if (voucher.Count > 0)
        //    //{
        //    //    businessError = "This period can not be updated, beacause voucher already exist.";
        //    //}

        //    if (obj.IsActive == false)
        //    {
        //        bool? active = (from c in CPFUnit.ManagePeriodInformationRepository.Fetch()
        //                        where c.IsActive == true && c.Id != obj.Id
        //                        select c.IsActive).FirstOrDefault();
        //        if (active.HasValue && (active.Value == false))
        //        {
        //            businessError = "One period must be active.";
        //            return businessError;
        //        }
        //    }

        //    return businessError;
        //}

        //public string CheckBusinessLogicForDelete(CPF_Period obj)
        //{
        //    string businessError = string.Empty;
        //    // TODO: Validation with voucher
        //    //var voucher = (from v in this.CPFUnit.VoucherRepository.GetAll() where v.PeriodId == obj.Id select v).ToList();
        //    //if (voucher.Count > 0)
        //    //{
        //    //    businessError = "This period can not be deleted, beacause voucher already exist.";
        //    //}

        //    if (obj != null && obj.IsActive == true)
        //    {
        //        businessError = "Active period can not be deleted.Please active and another then try.";
        //    }

        //    return businessError;
        //}


        #endregion
    }
}
