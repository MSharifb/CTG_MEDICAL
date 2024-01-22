
using AutoMapper;

using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.CommonConfigViewModel;

using BEPZA_MEDICAL.Web.Areas.CPF.Models.BankInformationViewModel;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ForfeitedRuleViewModel;

using BEPZA_MEDICAL.Web.Areas.CPF.Models.OpeningBalance;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Settlement;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Withdraw;


using BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipInformation;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitDistribution;

using BEPZA_MEDICAL.Web.Areas.CPF.Models.FixedParameters;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.GratuityInterestRate;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitInterestRate;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.LoanPolicy;


namespace BEPZA_MEDICAL.Web.Utility
{
    public class CPFMapper
    {
        public CPFMapper()
        {
            // Common Configuration
            Mapper.CreateMap<CommonConfigViewModel, CommonConfigGetResult>();
            Mapper.CreateMap<CommonConfigGetResult, CommonConfigViewModel>();

            //Common configType
            Mapper.CreateMap<CommonConfigTypeViewModel, CommonConfigType>();
            Mapper.CreateMap<CommonConfigType, CommonConfigTypeViewModel>();

            ////Financial Year
            //Mapper.CreateMap<ManagePeriodInformationModel, CPF_Period>();
            //Mapper.CreateMap<CPF_Period, ManagePeriodInformationModel>();

            // Banch Account information            
            Mapper.CreateMap<BankInformationViewModel, CPF_BankAccountInfo>();
            Mapper.CreateMap<CPF_BankAccountInfo, BankInformationViewModel>();
            // Forfeited Rule           
            Mapper.CreateMap<ForfeitedRuleViewModel, CPF_ForfeitedRule>();
            Mapper.CreateMap<CPF_ForfeitedRule, ForfeitedRuleViewModel>();

            // Opening Balance
            Mapper.CreateMap<OpeningBalanceViewModel, CPF_OpeningBalance>();
            Mapper.CreateMap<CPF_OpeningBalance, OpeningBalanceViewModel>();

            // Settlement
            Mapper.CreateMap<SettlementViewModel, CPF_Settlement>();
            Mapper.CreateMap<CPF_Settlement, SettlementViewModel>();

            // Withdraw
            Mapper.CreateMap<WithdrawViewModel, CPF_Withdraw>();
            Mapper.CreateMap<CPF_Withdraw, WithdrawViewModel>();
            
            // Membership Information
            Mapper.CreateMap<MembershipInformationViewModel, CPF_MembershipInfo>();
            Mapper.CreateMap<CPF_MembershipInfo, MembershipInformationViewModel>();
            
           
            // Profit Distribution   
            Mapper.CreateMap<ProfitDistributionViewModel, CPF_ProfitDistribution>();
            Mapper.CreateMap<CPF_ProfitDistribution, ProfitDistributionViewModel>();
            
            //CPF_ContributionRateSetup
            Mapper.CreateMap<CPFContributionRateSetupModel, CPF_ContributionRateSetup>();
            Mapper.CreateMap<CPF_ContributionRateSetup, CPFContributionRateSetupModel>();

            //CPF_GratuityInterestRateSetup
            Mapper.CreateMap<GratuityInterestRateModel, CPF_GratuityInterestRate>();
            Mapper.CreateMap<CPF_GratuityInterestRate, GratuityInterestRateModel>();

            //CPF_ProfitInterestRate
            Mapper.CreateMap<ProfitInterestRateModel, CPF_ProfitInterestRate>();
            Mapper.CreateMap<CPF_ProfitInterestRate, ProfitInterestRateModel>();

            //CPF_LoanPolicy
            Mapper.CreateMap<LoanPolicyViewModel, CPF_LoanPolicy>();
            Mapper.CreateMap<CPF_LoanPolicy, LoanPolicyViewModel>();

        }
    }
}