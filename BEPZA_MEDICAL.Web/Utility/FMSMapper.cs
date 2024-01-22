using AutoMapper;
using BEPZA_MEDICAL.DAL.FMS;
using BEPZA_MEDICAL.Web.Areas.FMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class FMSMapper
    {
        public FMSMapper()
        {
            //Fixed Deposit Type Info
            Mapper.CreateMap<FixedDepositTypeInfoViewModel, FMS_FixedDepositTypeInfo>();
            Mapper.CreateMap<FMS_FixedDepositTypeInfo, FixedDepositTypeInfoViewModel>();

            //Bank Info
            Mapper.CreateMap<BankInfoViewModel, FMS_BankInfo>();
            Mapper.CreateMap<FMS_BankInfo, BankInfoViewModel>();

            Mapper.CreateMap<BankInfoBranchDetailViewModel, FMS_BankInfoBranchDetail>();
            Mapper.CreateMap<FMS_BankInfoBranchDetail, BankInfoBranchDetailViewModel>();

            //FDR Installment Info
            Mapper.CreateMap<FDRInstallmentInformationViewModel, FMS_FDRInstallmentInfo>();
            Mapper.CreateMap<FMS_FDRInstallmentInfo, FDRInstallmentInformationViewModel>();

            //Fixed Deposit Info
            Mapper.CreateMap<FixedDepositInfoViewModel, FMS_FixedDepositInfo>();
            Mapper.CreateMap<FMS_FixedDepositInfo, FixedDepositInfoViewModel>();

            Mapper.CreateMap<FixedDepositInfoInstallmentScheduleViewModel, FMS_FixedDepositInfoInstallmentSchedule>();
            Mapper.CreateMap<FMS_FixedDepositInfoInstallmentSchedule, FixedDepositInfoInstallmentScheduleViewModel>();

            //FDR Closing
            Mapper.CreateMap<FDRClosingViewModel, FMS_FDRClosingInfo>();
            Mapper.CreateMap<FMS_FDRClosingInfo, FDRClosingViewModel>();

            //Source of Fund
            Mapper.CreateMap<SourceofFundViewModel, FMS_SourceofFund>();
            Mapper.CreateMap<FMS_SourceofFund, SourceofFundViewModel>();

            //Bank Wise Offer Rate
            Mapper.CreateMap<BankWiseOfferRateViewModel, FMS_BankWiseOfferRate>();
            Mapper.CreateMap<FMS_BankWiseOfferRate, BankWiseOfferRateViewModel>();

            //CPF
            Mapper.CreateMap<CPFInterestReceivableViewModel, FMS_CPFInterestReceivable>();
            Mapper.CreateMap<FMS_CPFInterestReceivable, CPFInterestReceivableViewModel>();
        }
    }
}