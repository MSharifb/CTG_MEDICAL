using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using BEPZA_MEDICAL.DAL.CPF.CustomEntities;


namespace BEPZA_MEDICAL.DAL.CPF
{
    public class CPF_UnitOfWork
    {
        #region Fields
        CPF_ExecuteFunctions _functionRepository;
        CPF_GenericRepository<CommonConfigType> _commonConfigType;

        CPF_GenericRepository<CPF_BankAccountInfo> _bankAccountInfo;
        CPF_GenericRepository<CPF_MembershipInfo> _MembershipInfo;
        CPF_GenericRepository<CPF_ForfeitedRule> _forfeitedRule;

        CPF_GenericRepository<CPF_SpecialParameter> _specialAccountHead;

        CPF_GenericRepository<CPF_OpeningBalance> _openingBalance;
        CPF_GenericRepository<CPF_Settlement> _settlement;
        CPF_GenericRepository<CPF_ProfitDistribution> _profitDistribution;
        CPF_GenericRepository<CPF_ProfitDistributionDetail> _profitDistributionDetail;
        CPF_GenericRepository<CPF_Withdraw> _withdraw;

        CPF_GenericRepository<APV_ApprovalStatus> _approvalStatus;

        CPF_GenericRepository<CPF_DocumentType> _documentType;

        CPF_GenericRepository<CPF_PaymentType> _paymentType;
        CPF_GenericRepository<CPF_AccountHeadCategory> _accountHeadCategory;
        CPF_GenericRepository<ProfitDistributionDetailModel> _profitDistributionDetailCustom;
        CPF_GenericRepository<CPF_ContributionRateSetup> _CPF_ContributionRateSetup;
        CPF_GenericRepository<CPF_GratuityInterestRate> _CPF_GratuityInterestRate;
        CPF_GenericRepository<CPF_DefaultMapping> _CPF_DefaultMapping;
        CPF_GenericRepository<vwCPFNominee> _CPF_VwNominee;
        CPF_GenericRepository<CPF_ProfitInterestRate> _CPF_ProfitInterestRate;
        CPF_GenericRepository<CPF_LoanPolicy> _CPF_LoanPolicy;
        CPF_GenericRepository<CPF_EmployeeCPF_WF_FundStatus_Result> _CPF_EmployeeWiseFindStatus;
        CPF_GenericRepository<CPF_GetMyCPFSummary_Result> _CPF_GetMyCpfSummary;
        CPF_GenericRepository<CustomPropertyAttribute> _customPropertyAttribute;

        #endregion

        #region Constactor

        public CPF_UnitOfWork(

            CPF_ExecuteFunctions functionRepository,
            CPF_GenericRepository<CommonConfigType> configTypeRepository,
            CPF_GenericRepository<CPF_BankAccountInfo> bankAccountInfo,

            CPF_GenericRepository<CPF_ForfeitedRule> forfeitedRule,

            CPF_GenericRepository<CPF_SpecialParameter> specialAccountHead,

            CPF_GenericRepository<CPF_MembershipInfo> membershipInfo,
            CPF_GenericRepository<CPF_OpeningBalance> openingBalance,
            CPF_GenericRepository<CPF_Settlement> settlement,
            CPF_GenericRepository<CPF_ProfitDistribution> profitDistribution,
            CPF_GenericRepository<CPF_ProfitDistributionDetail> profitDistributionDetail,
            CPF_GenericRepository<CPF_Withdraw> withdraw,

            CPF_GenericRepository<APV_ApprovalStatus> approvalStatus,

            CPF_GenericRepository<CPF_DocumentType> documentType,

            CPF_GenericRepository<ProfitDistributionDetailModel> profitDistributionDetailCustom,
            CPF_GenericRepository<CPF_PaymentType> paymentType,
            CPF_GenericRepository<CPF_AccountHeadCategory> accountHeadCategory,
            CPF_GenericRepository<CPF_ContributionRateSetup> CPF_ContributionRateSetup,
            CPF_GenericRepository<CPF_GratuityInterestRate> CPF_GratuityInterestRate,
            CPF_GenericRepository<CPF_DefaultMapping> CPF_DefaultMapping,
            CPF_GenericRepository<vwCPFNominee> CPF_VwNominee,
            CPF_GenericRepository<CPF_ProfitInterestRate> CPF_ProfitInterestRate,
            CPF_GenericRepository<CPF_LoanPolicy> CPF_LoanPolicy,
            CPF_GenericRepository<CPF_EmployeeCPF_WF_FundStatus_Result> CPF_EmployeeWiseFindStatus,
            CPF_GenericRepository<CPF_GetMyCPFSummary_Result> CPF_GetMyCpfSummary,
            CPF_GenericRepository<CustomPropertyAttribute> customPropertyAttribute
            )
        {
            _functionRepository = functionRepository;
            _commonConfigType = configTypeRepository;
            _bankAccountInfo = bankAccountInfo;

            _forfeitedRule = forfeitedRule;

            _specialAccountHead = specialAccountHead;

            _MembershipInfo = membershipInfo;
            _openingBalance = openingBalance;
            _settlement = settlement;
            _profitDistribution = profitDistribution;
            _profitDistributionDetail = profitDistributionDetail;
            _withdraw = withdraw;

            _approvalStatus = approvalStatus;

            _documentType = documentType;

            _profitDistributionDetailCustom = profitDistributionDetailCustom;
            _paymentType = paymentType;
            _accountHeadCategory = accountHeadCategory;
            _CPF_ContributionRateSetup = CPF_ContributionRateSetup;
            _CPF_GratuityInterestRate = CPF_GratuityInterestRate;
            _CPF_DefaultMapping = CPF_DefaultMapping;
            _CPF_VwNominee = CPF_VwNominee;
            _CPF_ProfitInterestRate = CPF_ProfitInterestRate;
            _CPF_LoanPolicy = CPF_LoanPolicy;
            _CPF_EmployeeWiseFindStatus = CPF_EmployeeWiseFindStatus;
            _CPF_GetMyCpfSummary = CPF_GetMyCpfSummary;
            _customPropertyAttribute = customPropertyAttribute;
        }

        #endregion

        #region Properties

        public CPF_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        public CPF_GenericRepository<CommonConfigType> ConfigTypeRepository
        {
            get { return _commonConfigType; }
        }

        public CPF_GenericRepository<CPF_BankAccountInfo> BankAccountInfoRepository
        {
            get { return _bankAccountInfo; }
        }

        public CPF_GenericRepository<CPF_MembershipInfo> MembershipInformationRepository
        {
            get { return _MembershipInfo; }
        }


        public CPF_GenericRepository<CPF_ForfeitedRule> ForfeitedRuleRepository
        {
            get { return _forfeitedRule; }
        }

        public CPF_GenericRepository<CPF_SpecialParameter> SpecialAccountHeadRepository
        {
            get { return _specialAccountHead; }
        }

        public CPF_GenericRepository<CPF_OpeningBalance> OpeningBalanceRepository
        {
            get { return _openingBalance; }
        }
        public CPF_GenericRepository<CPF_Settlement> SettlementRepository
        {
            get { return _settlement; }
        }
        public CPF_GenericRepository<CPF_ProfitDistribution> ProfitDistributionRepository
        {
            get { return _profitDistribution; }
        }
        public CPF_GenericRepository<CPF_ProfitDistributionDetail> ProfitDistributionDetailRepository
        {
            get { return _profitDistributionDetail; }
        }

        public CPF_GenericRepository<CPF_Withdraw> WithdrawRepository
        {
            get { return _withdraw; }
        }

        public CPF_GenericRepository<APV_ApprovalStatus> ApprovalStatusRepository
        {
            get { return _approvalStatus; }
        }

        public CPF_GenericRepository<CPF_DocumentType> DocumentTypeRepository
        {
            get { return _documentType; }
        }

        public CPF_GenericRepository<ProfitDistributionDetailModel> ProfitDistributionDetailInfo
        {
            get { return _profitDistributionDetailCustom; }
        }

        public CPF_GenericRepository<CPF_PaymentType> PaymentType
        {
            get { return _paymentType; }
        }

        public CPF_GenericRepository<CPF_AccountHeadCategory> AccountHeadCategoryRepository
        {
            get { return _accountHeadCategory; }
        }

        public CPF_GenericRepository<CPF_ContributionRateSetup> CPF_ContributionRateSetupRepository
        {
            get { return _CPF_ContributionRateSetup; }
        }

        public CPF_GenericRepository<CPF_GratuityInterestRate> CPF_GratuityInterestRateRepository
        {
            get { return _CPF_GratuityInterestRate; }
        }

        public CPF_GenericRepository<CPF_DefaultMapping> CPF_DefaultMappingRepository
        {
            get { return _CPF_DefaultMapping; }
        }

        public CPF_GenericRepository<vwCPFNominee> CPF_VwNomineeRepository
        {
            get { return _CPF_VwNominee; }
        }

        public CPF_GenericRepository<CPF_ProfitInterestRate> CPF_ProfitInterestRateRepository
        {
            get { return _CPF_ProfitInterestRate; }
        }

        public CPF_GenericRepository<CPF_LoanPolicy> LoanPolicyRepository
        {
            get { return _CPF_LoanPolicy; }
        }

        public CPF_GenericRepository<CPF_EmployeeCPF_WF_FundStatus_Result> EmployeeWiseFundStatusRepository
        {
            get { return _CPF_EmployeeWiseFindStatus; }
        }
        
        public CPF_GenericRepository<CPF_GetMyCPFSummary_Result> MyCpfSummaryRepository
        {
            get { return _CPF_GetMyCpfSummary; }
        }

        public CPF_GenericRepository<CustomPropertyAttribute> CustomPropertyAttributeRepository
        {
            get { return _customPropertyAttribute; }
        }

        #endregion

    }
}
