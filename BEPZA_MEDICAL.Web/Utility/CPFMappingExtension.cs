using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using BEPZA_MEDICAL.Web.Areas.CPF.Models;
using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.CommonConfigViewModel;

using BEPZA_MEDICAL.Web.Areas.CPF.Models.BankInformationViewModel;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ForfeitedRuleViewModel;

using BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipInformation;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipApplication;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.OpeningBalance;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Settlement;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Withdraw;


using BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitDistribution;

using BEPZA_MEDICAL.Web.Areas.CPF.Models.FixedParameters;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.GratuityInterestRate;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.ProfitInterestRate;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.LoanPolicy;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class CPFMappingExtension
    {
        //Common Config
        public static CommonConfigViewModel ToModel(this CommonConfigGetResult obj)
        {
            return Mapper.Map<CommonConfigGetResult, CommonConfigViewModel>(obj);
        }
        public static CommonConfigGetResult ToEntity(this CommonConfigViewModel model)
        {
            return Mapper.Map<CommonConfigViewModel, CommonConfigGetResult>(model);
        }

        //Common Config Type
        public static List<CommonConfigTypeViewModel> ToModelList(this List<CommonConfigType> objlist)
        {
            List<CommonConfigTypeViewModel> list = new List<CommonConfigTypeViewModel>();
            foreach (var item in objlist)
            {
                list.Add((Mapper.Map<CommonConfigType, CommonConfigTypeViewModel>(item)));
            }
            return list;
        }
        public static List<CommonConfigType> ToEntityList(this List<CommonConfigTypeViewModel> modellist)
        {
            List<CommonConfigType> list = new List<CommonConfigType>();
            foreach (var item in modellist)
            {
                list.Add((Mapper.Map<CommonConfigTypeViewModel, CommonConfigType>(item)));
            }
            return list;
        }

        // Bank Information

        public static BankInformationViewModel ToModel(this CPF_BankAccountInfo obj)
        {
            return Mapper.Map<CPF_BankAccountInfo, BankInformationViewModel>(obj);
        }
        public static CPF_BankAccountInfo ToEntity(this BankInformationViewModel obj)
        {
            return Mapper.Map<BankInformationViewModel, CPF_BankAccountInfo>(obj);
        }

        //Membership Information

        public static MembershipInformationViewModel ToModel(this CPF_MembershipInfo obj)
        {
            return Mapper.Map<CPF_MembershipInfo, MembershipInformationViewModel>(obj);
        }
        public static CPF_MembershipInfo ToEntity(this MembershipInformationViewModel obj)
        {
            return Mapper.Map<MembershipInformationViewModel, CPF_MembershipInfo>(obj);
        }

        // Forfeited Rule information    

        public static ForfeitedRuleViewModel ToModel(this CPF_ForfeitedRule obj)
        {
            return Mapper.Map<CPF_ForfeitedRule, ForfeitedRuleViewModel>(obj);
        }
        public static CPF_ForfeitedRule ToEntity(this ForfeitedRuleViewModel obj)
        {
            return Mapper.Map<ForfeitedRuleViewModel, CPF_ForfeitedRule>(obj);
        }

        // Opening Balance
        public static OpeningBalanceViewModel ToModel(this CPF_OpeningBalance obj)
        {
            return Mapper.Map<CPF_OpeningBalance, OpeningBalanceViewModel>(obj);
        }
        public static CPF_OpeningBalance ToEntity(this OpeningBalanceViewModel obj)
        {
            return Mapper.Map<OpeningBalanceViewModel, CPF_OpeningBalance>(obj);
        }

        // Settlement 

        public static SettlementViewModel ToModel(this CPF_Settlement obj)
        {
            return Mapper.Map<CPF_Settlement, SettlementViewModel>(obj);
        }
        public static CPF_Settlement ToEntity(this SettlementViewModel obj)
        {
            return Mapper.Map<SettlementViewModel, CPF_Settlement>(obj);
        }

        // Withdraw 
        public static WithdrawViewModel ToModel(this CPF_Withdraw obj)
        {
            return Mapper.Map<CPF_Withdraw, WithdrawViewModel>(obj);
        }
        public static CPF_Withdraw ToEntity(this WithdrawViewModel obj)
        {
            return Mapper.Map<WithdrawViewModel, CPF_Withdraw>(obj);
        }

        // Profit Distribution
        public static ProfitDistributionViewModel ToModel(this CPF_ProfitDistribution obj)
        {
            return Mapper.Map<CPF_ProfitDistribution, ProfitDistributionViewModel>(obj);
        }
        public static CPF_ProfitDistribution ToEntity(this ProfitDistributionViewModel obj)
        {
            return Mapper.Map<ProfitDistributionViewModel, CPF_ProfitDistribution>(obj);
        }

        // CPF_ContributionRateSetup
        public static CPFContributionRateSetupModel ToModel(this CPF_ContributionRateSetup obj)
        {
            return Mapper.Map<CPF_ContributionRateSetup, CPFContributionRateSetupModel>(obj);
        }
        public static CPF_ContributionRateSetup ToEntity(this CPFContributionRateSetupModel obj)
        {
            return Mapper.Map<CPFContributionRateSetupModel, CPF_ContributionRateSetup>(obj);
        }

        // CPF_GratuityInterestRate
        public static GratuityInterestRateModel ToModel(this CPF_GratuityInterestRate obj)
        {
            return Mapper.Map<CPF_GratuityInterestRate, GratuityInterestRateModel>(obj);
        }
        public static CPF_GratuityInterestRate ToEntity(this GratuityInterestRateModel obj)
        {
            return Mapper.Map<GratuityInterestRateModel, CPF_GratuityInterestRate>(obj);
        }

        // CPF_ProvidendFundInterestRate
        public static ProfitInterestRateModel ToModel(this CPF_ProfitInterestRate obj)
        {
            return Mapper.Map<CPF_ProfitInterestRate, ProfitInterestRateModel>(obj);
        }
        public static CPF_ProfitInterestRate ToEntity(this ProfitInterestRateModel obj)
        {
            return Mapper.Map<ProfitInterestRateModel, CPF_ProfitInterestRate>(obj);
        }

        // CPF_LoanPolicy
        public static LoanPolicyViewModel ToModel(this CPF_LoanPolicy obj)
        {
            return Mapper.Map<CPF_LoanPolicy, LoanPolicyViewModel>(obj);
        }
        public static CPF_LoanPolicy ToEntity(this LoanPolicyViewModel obj)
        {
            return Mapper.Map<LoanPolicyViewModel, CPF_LoanPolicy>(obj);
        }
    }
}