using AutoMapper;
using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class FMSMappingExtension
    {
        //Fixed Deposit Type Info
        public static FixedDepositTypeInfoViewModel ToModel(this FMS_FixedDepositTypeInfo entity)
        {
            return Mapper.Map<FMS_FixedDepositTypeInfo, FixedDepositTypeInfoViewModel>(entity);
        }
        public static FMS_FixedDepositTypeInfo ToEntity(this FixedDepositTypeInfoViewModel model)
        {
            return Mapper.Map<FixedDepositTypeInfoViewModel, FMS_FixedDepositTypeInfo>(model);
        }

        //Bank Info


        public static BankInfoViewModel ToModel(this FMS_BankInfo entity)
        {
            return Mapper.Map<FMS_BankInfo, BankInfoViewModel>(entity);
        }
        public static FMS_BankInfo ToEntity(this BankInfoViewModel model)
        {
            return Mapper.Map<BankInfoViewModel, FMS_BankInfo>(model);
        }

        public static BankInfoBranchDetailViewModel ToModel(this FMS_BankInfoBranchDetail entity)
        {
            return Mapper.Map<FMS_BankInfoBranchDetail, BankInfoBranchDetailViewModel>(entity);
        }
        public static FMS_BankInfoBranchDetail ToEntity(this BankInfoBranchDetailViewModel model)
        {
            return Mapper.Map<BankInfoBranchDetailViewModel, FMS_BankInfoBranchDetail>(model);
        }

        //FDR Installment Info
        public static FDRInstallmentInformationViewModel ToModel(this FMS_FDRInstallmentInfo entity)
        {
            return Mapper.Map<FMS_FDRInstallmentInfo, FDRInstallmentInformationViewModel>(entity);
        }
        public static FMS_FDRInstallmentInfo ToEntity(this FDRInstallmentInformationViewModel model)
        {
            return Mapper.Map<FDRInstallmentInformationViewModel, FMS_FDRInstallmentInfo>(model);
        }

        //Fixed Deposit Info

        public static FixedDepositInfoViewModel ToModel(this FMS_FixedDepositInfo entity)
        {
            return Mapper.Map<FMS_FixedDepositInfo, FixedDepositInfoViewModel>(entity);
        }
        public static FMS_FixedDepositInfo ToEntity(this FixedDepositInfoViewModel model)
        {
            return Mapper.Map<FixedDepositInfoViewModel, FMS_FixedDepositInfo>(model);
        }

        public static FixedDepositInfoInstallmentScheduleViewModel ToModel(this FMS_FixedDepositInfoInstallmentSchedule entity)
        {
            return Mapper.Map<FMS_FixedDepositInfoInstallmentSchedule, FixedDepositInfoInstallmentScheduleViewModel>(entity);
        }
        public static FMS_FixedDepositInfoInstallmentSchedule ToEntity(this FixedDepositInfoInstallmentScheduleViewModel model)
        {
            return Mapper.Map<FixedDepositInfoInstallmentScheduleViewModel, FMS_FixedDepositInfoInstallmentSchedule>(model);
        }


        //FDR Closing

        public static FDRClosingViewModel ToModel(this FMS_FDRClosingInfo entity)
        {
            return Mapper.Map<FMS_FDRClosingInfo, FDRClosingViewModel>(entity);
        }
        public static FMS_FDRClosingInfo ToEntity(this FDRClosingViewModel model)
        {
            return Mapper.Map<FDRClosingViewModel, FMS_FDRClosingInfo>(model);
        }

        //Source of Fund
        public static SourceofFundViewModel ToModel(this FMS_SourceofFund entity)
        {
            return Mapper.Map<FMS_SourceofFund, SourceofFundViewModel>(entity);
        }
        public static FMS_SourceofFund ToEntity(this SourceofFundViewModel model)
        {
            return Mapper.Map<SourceofFundViewModel, FMS_SourceofFund>(model);
        }

        //Bank Wise Offer Rate
        public static BankWiseOfferRateViewModel ToModel(this FMS_BankWiseOfferRate entity)
        {
            return Mapper.Map<FMS_BankWiseOfferRate, BankWiseOfferRateViewModel>(entity);
        }
        public static FMS_BankWiseOfferRate ToEntity(this BankWiseOfferRateViewModel model)
        {
            return Mapper.Map<BankWiseOfferRateViewModel, FMS_BankWiseOfferRate>(model);
        }

        //CPF
        public static CPFInterestReceivableViewModel ToModel(this FMS_CPFInterestReceivable entity)
        {
            return Mapper.Map<FMS_CPFInterestReceivable, CPFInterestReceivableViewModel>(entity);
        }
        public static FMS_CPFInterestReceivable ToEntity(this CPFInterestReceivableViewModel model)
        {
            return Mapper.Map<CPFInterestReceivableViewModel, FMS_CPFInterestReceivable>(model);
        }
    }
}