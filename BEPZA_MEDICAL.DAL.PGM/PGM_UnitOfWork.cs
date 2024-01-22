using BEPZA_MEDICAL.DAL.PGM.CustomEntities;

namespace BEPZA_MEDICAL.DAL.PGM
{
    public class PGM_UnitOfWork
    {
        #region fields

        private readonly PGM_ExecuteFunctions _functionRepository;

        private readonly PGM_GenericRepository<PGM_TaxRule> _taxRule;
        private readonly PGM_GenericRepository<PGM_BonusType> _bonusType;
        private readonly PGM_GenericRepository<PGM_Bonus> _bonus;
        private readonly PGM_GenericRepository<PGM_BonusDetail> _bonusDetails;

        private readonly PGM_GenericRepository<PRM_Religion> _religion;

        private readonly PGM_GenericRepository<PRM_Designation> _designation;
        private readonly PGM_GenericRepository<PRM_Division> _division;
        private readonly PGM_GenericRepository<PRM_Region> _region;
        private readonly PGM_GenericRepository<PRM_EmpSalary> _empSalary;
        private readonly PGM_GenericRepository<PRM_EmpSalaryDetail> _empSalaryDetails;
        private readonly PGM_GenericRepository<PRM_SalaryStructure> _salaryStructureRepository;
        private readonly PGM_GenericRepository<PRM_SalaryStructureDetail> _salaryStructureDetailRepository;
        private readonly PGM_GenericRepository<PRM_SalaryHeadGroup> _salaryHeadGroup;
        private readonly PGM_GenericRepository<PRM_SalaryHead> _salaryHead;
        private readonly PGM_GenericRepository<PRM_SalaryScale> _salaryScale;
        private readonly PGM_GenericRepository<PRM_JobGrade> _grade;
        private readonly PGM_GenericRepository<PRM_GradeStep> _gradeStep;
        private readonly PGM_GenericRepository<PRM_EmploymentType> _employmentType;
        private readonly PGM_GenericRepository<PRM_Section> _section;
        private readonly PGM_GenericRepository<PRM_StaffCategory> _staffCategory;
        private readonly PGM_GenericRepository<PRM_EmpStatusChange> _empStatusChange;
        private readonly PGM_GenericRepository<PRM_BankName> _bankName;
        private readonly PGM_GenericRepository<PRM_BankBranch> _bankBranch;
        private readonly PGM_GenericRepository<FAM_BankBranchMap> _bankMaster;
        private readonly PGM_GenericRepository<FAM_BankBranchAccountNo> _bankDetails;
        private readonly PGM_GenericRepository<PRM_EmpSeperation> _empSeperation;
        private readonly PGM_GenericRepository<PRM_EmploymentStatus> _employmentStatus;

        private readonly PGM_GenericRepository<PGM_BankAccount> _bankaccount;
        private readonly PGM_GenericRepository<PGM_BankAdviceLetterTemplate> _bankLetterTemplate;
        private readonly PGM_GenericRepository<PGM_BankAdviceLetterSubjectBody> _bankLetterText;
        private readonly PGM_GenericRepository<PGM_BankAdviceLetterVariables> _bankLetterVariables;

        private readonly PGM_GenericRepository<PGM_GratuityRule> _gratuityrule;
        private readonly PGM_GenericRepository<LMS_tblLeaveType> _leavetype;
        private readonly PGM_GenericRepository<PGM_TaxRateRule> _taxrate;
        private readonly PGM_GenericRepository<PGM_TaxRateRuleDetail> _taxratedetail;
        private readonly PGM_GenericRepository<OverTimeSearch> _overTimeSearch;
        private readonly PGM_GenericRepository<PGM_LeaveEncashment> _leaveEncashment;
        private readonly PGM_GenericRepository<LeaveEncashmentSearchModel> _leaveEncashmentSearch;
        private readonly PGM_GenericRepository<SalaryRateCalculation> _salaryRateCalculation;
        private readonly PGM_GenericRepository<LMS_tblLeaveType> _leaveType;
        private readonly PGM_GenericRepository<LMS_tblLeaveLedger> _leaveInformation;
        private readonly PGM_GenericRepository<LMS_tblLeaveApplication> _leaveApplication;
        private readonly PGM_GenericRepository<LMS_tblLeaveYear> _leaveYear;
        private readonly PGM_GenericRepository<ClosingBalanceSearchModel> _closingBalance;
        private readonly PGM_GenericRepository<PGM_BankLetter> _bankLetter;
        private readonly PGM_GenericRepository<PGM_BankLetterDetail> _bankLetterDetails;
        private readonly PGM_GenericRepository<BankAdviceLetterSearchModel> _bankAdviceLetterSearch;
        private readonly PGM_GenericRepository<BankAdviceLetterDetailCustomModel> _bankDetailCustom;
        private readonly PGM_GenericRepository<WithheldSalaryPaymentSearchModel> _withheldSalarySearch;
        private readonly PGM_GenericRepository<PGM_WithheldSalaryPayment> _withheldSalary;
        private readonly PGM_GenericRepository<PGM_Salary> _salaryMaster;
        private readonly PGM_GenericRepository<PGM_SalaryDetail> _salaryDetail;
        private readonly PGM_GenericRepository<PGM_TaxOpening> _taxopening;
        private readonly PGM_GenericRepository<PGM_TaxOpeningDetail> _taxopeningdetail;
        private readonly PGM_GenericRepository<TaxOpeningDetail> _taxopeningdetailcustom;
        private readonly PGM_GenericRepository<PGM_GratuityPayment> _gratuitySettlement;
        private readonly PGM_GenericRepository<GratuitySettlementSearchModel> _gratuitySettlementSearch;
        private readonly PGM_GenericRepository<EmployeeBasicSalary> _empBasicSalary;
        private readonly PGM_GenericRepository<PGM_FinalSettlement> _finalSettlement;
        private readonly PGM_GenericRepository<FinalSettlementSearchModel> _finalSettlementSearch;
        private readonly PGM_GenericRepository<OtherAdjustDeductSearchModel> _otherAdjustDeductSearch;
        private readonly PGM_GenericRepository<PGM_OtherAdjustDeduct> _otheradjustdeduct;
        private readonly PGM_GenericRepository<PGM_OtherAdjustDeductAll> _otheradjustdeductAll;
        private readonly PGM_GenericRepository<PGM_VehicleDeduction> _vehicleDeduction;
        private readonly PGM_GenericRepository<VehicleDeductionBillList> _vehicleDeductionList;
        private readonly PGM_GenericRepository<SalaryMonthInfo> _salaryMonth;
        private readonly PGM_GenericRepository<MonthlySalaryDetail> _salaryDetailInfo;
        private readonly PGM_GenericRepository<BonusProcessSearchModel> _bonusSearch;
        private readonly PGM_GenericRepository<BonusDetailsSearchModel> _bonusDetailsSearch;
        private readonly PGM_GenericRepository<PGM_SalaryDistribution> _salaryDistribution;
        private readonly PGM_GenericRepository<MonthlySalaryDistributionDetaiCustomModell> _salaryDistributionSearch;
        private readonly PGM_GenericRepository<IncomeTaxComputationCustomModel> _incomeTaxComputation;
        private readonly PGM_GenericRepository<PGM_EmpTax> _tax;
        private readonly PGM_GenericRepository<BonusTypeCustomModel> _bonusTypeSearch;
        private readonly PGM_GenericRepository<PGM_SalaryWithheld> _salaryWithheld;
        private readonly PGM_GenericRepository<PGM_HouseRentRule> _houseRentRule;
        private readonly PGM_GenericRepository<PGM_HouseRentRuleDetail> _houseRentRuleDetail;
        private readonly PGM_GenericRepository<PGM_HouseMaintenanceCharge> _houseMaintenanceCharge;
        private readonly PGM_GenericRepository<PGM_HouseMaintenanceChargeDetail> _houseMaintenanceChargeDetail;
        private readonly PGM_GenericRepository<PGM_ElectricBill> _electricBill;
        private readonly PGM_GenericRepository<PGM_NightBillPayment> _nightBillPayment;
        private readonly PGM_GenericRepository<PGM_NightBillPaymentDetail> _nightBillPaymentDetail;
        private readonly PGM_GenericRepository<PGM_ChargeAllowanceRule> _chargeAllowanceRule;
        private readonly PGM_GenericRepository<PGM_GroupInsurancePaymentType> _groupInsurancePaymentType;
        private readonly PGM_GenericRepository<PGM_EmpChargeAllowance> _empChargeAllowance;
        private readonly PGM_GenericRepository<PGM_GroupInsurancePayment> _groupInsurancePayment;
        private readonly PGM_GenericRepository<PGM_HouseRentWaterBillDeduct> _houseRentWaterBillDeduct;
        private readonly PGM_GenericRepository<IncentiveBonusProcessSearchModel> _incentiveBonusSearchModel;
        private readonly PGM_GenericRepository<IncentiveBonusDetailProcessSearchModel> _incentiveBonusDetailProcessSearchModel;
        private readonly PGM_GenericRepository<PGM_IncentiveBonus> _incentiveBonus;
        private readonly PGM_GenericRepository<PGM_IncentiveBonusDetail> _incentiveBonusDetail;

        private readonly PGM_GenericRepository<PGM_ArrearAdjustment> _arrearAdjustment;
        private readonly PGM_GenericRepository<PGM_ArrearAdjustmentDetail> _arrearAdjustmentDetail;
        private readonly PGM_GenericRepository<PGM_Overtime> _overtime;
        private readonly PGM_GenericRepository<AmountInWords> _amountInWords;
        private readonly PGM_GenericRepository<PGM_TaxRegionRule> _taxRegionRule;
        private readonly PGM_GenericRepository<PGM_TaxRegionWiseMinTax> _taxRegionWiseMinTax;
        private readonly PGM_GenericRepository<PGM_TaxOtherInvestAndAdvPaid> _taxOtherInvAndAdvPaid;
        private readonly PGM_GenericRepository<PGM_TaxOtherInvestAndAdvPaidDetail> _taxOtherInvAndAdvPaidDetail;
        private readonly PGM_GenericRepository<PGM_Document> _document;
        private readonly PGM_GenericRepository<PGM_TaxExemptionRule> _taxExemptionRule;
        private readonly PGM_GenericRepository<PGM_TaxExemptionRuleDetail> _taxExemptionRuleDetail;
        private readonly PGM_GenericRepository<PGM_EntityDocument> _entityDocument;
        private readonly PGM_GenericRepository<PGM_TaxOtherInvestmentType> _taxOtherInvestmentType;
        private readonly PGM_GenericRepository<PGM_TaxInvestmentRebateRule> _taxInvestmentRebateRule;
        private readonly PGM_GenericRepository<PGM_TaxInvestmentRebateRuleMaster> _taxInvestmentRebateRuleMaster;
        private readonly PGM_GenericRepository<PGM_TaxInvestmentRebateRuleDetail> _taxInvestmentRebateRuleDetail;
        private readonly PGM_GenericRepository<PGM_TaxWithheld> _taxWithheld;
        private readonly PGM_GenericRepository<acc_ChartofAccounts> _accChartOfAccount;
        private readonly PGM_GenericRepository<acc_FundControlInformation> _accFundControlInfo;
        private readonly PGM_GenericRepository<PGM_Configuration> _configuration;

        private readonly PGM_GenericRepository<acc_BankInformation> _accBankInfo;
        private readonly PGM_GenericRepository<acc_BankBranchInformation> _accBankBranchInfo;
        private readonly PGM_GenericRepository<acc_BankAccountInformation> _accBankAccountInfo;
        private readonly PGM_GenericRepository<acc_ChequeRegister> _accChequeRegister;
        private readonly PGM_GenericRepository<acc_Accounting_Period_Information> _accAccountingPeriod;

        private readonly PGM_GenericRepository<PGM_Attendance> _attendance;
        private readonly PGM_GenericRepository<PGM_Refreshment> _refreshment;

        private readonly PGM_GenericRepository<ELMAH_Error> _elmahError;
        private readonly PGM_GenericRepository<CustomPropertyAttribute> _customPropertyAttr;
        private readonly PGM_GenericRepository<CPF_Settlement> _cpfSettlement;

        #endregion

        #region Ctor
        public PGM_UnitOfWork(
            PGM_ExecuteFunctions functionRepository,

            PGM_GenericRepository<PGM_TaxRule> taxRule,
            PGM_GenericRepository<PGM_BonusType> bonusType,
            PGM_GenericRepository<PGM_Bonus> bonus,
            PGM_GenericRepository<PGM_BonusDetail> bonusDetails,

            PGM_GenericRepository<PRM_Religion> religion,

            PGM_GenericRepository<PRM_Designation> designation,
            PGM_GenericRepository<PRM_Division> division,
            PGM_GenericRepository<PRM_Region> region,
            PGM_GenericRepository<PRM_EmpSalary> empSalary,
            PGM_GenericRepository<PRM_EmpSalaryDetail> empSalaryDetails,
            PGM_GenericRepository<PRM_SalaryStructure> salaryStructureRepository,
            PGM_GenericRepository<PRM_SalaryStructureDetail> salaryStructureDetailRepository,
            PGM_GenericRepository<PRM_SalaryHeadGroup> salaryHeadGroup,
            PGM_GenericRepository<PRM_SalaryHead> salaryHead,
            PGM_GenericRepository<PRM_SalaryScale> salaryScale,
            PGM_GenericRepository<PRM_JobGrade> grade,
            PGM_GenericRepository<PRM_GradeStep> gradeStep,
            PGM_GenericRepository<PRM_EmploymentType> employmentType,
            PGM_GenericRepository<PRM_Section> section,
            PGM_GenericRepository<PRM_StaffCategory> staffCategory,
            PGM_GenericRepository<PRM_EmpStatusChange> empStatusChange,
            PGM_GenericRepository<PRM_BankName> bankName,
            PGM_GenericRepository<PRM_BankBranch> bankBranch,
            PGM_GenericRepository<FAM_BankBranchMap> bankMaster,
            PGM_GenericRepository<FAM_BankBranchAccountNo> bankDetails,
            PGM_GenericRepository<PRM_EmpSeperation> empSeperation,
            PGM_GenericRepository<PRM_EmploymentStatus> employmentStatus,


            PGM_GenericRepository<PGM_BankAccount> bankaccount,
            PGM_GenericRepository<PGM_BankAdviceLetterTemplate> bankLetterTemplate,
            PGM_GenericRepository<PGM_BankAdviceLetterVariables> bankLetterVariables,
            PGM_GenericRepository<PGM_BankAdviceLetterSubjectBody> bankLetterText,

            PGM_GenericRepository<PGM_GratuityRule> gratuityrule,
            PGM_GenericRepository<LMS_tblLeaveType> leavetype,
            PGM_GenericRepository<PGM_TaxRateRule> taxrate,
            PGM_GenericRepository<PGM_TaxRateRuleDetail> taxratedetail,
            PGM_GenericRepository<OverTimeSearch> overTimeSearch,
            PGM_GenericRepository<PGM_LeaveEncashment> leaveEncashment,
            PGM_GenericRepository<LeaveEncashmentSearchModel> leaveEncashmentSearch,
            PGM_GenericRepository<SalaryRateCalculation> salaryRateCalculation,
            PGM_GenericRepository<LMS_tblLeaveType> leaveType,
            PGM_GenericRepository<LMS_tblLeaveApplication> leaveApplication,
            PGM_GenericRepository<LMS_tblLeaveLedger> leaveInfo,
            PGM_GenericRepository<LMS_tblLeaveYear> leaveYear,
            PGM_GenericRepository<ClosingBalanceSearchModel> closingBalance,
            PGM_GenericRepository<PGM_BankLetter> bankLetter,
            PGM_GenericRepository<PGM_BankLetterDetail> bankLetterDetails,
            PGM_GenericRepository<BankAdviceLetterSearchModel> bankAdviceLetter,
            PGM_GenericRepository<BankAdviceLetterDetailCustomModel> bankLetterDetailsCustom,
            PGM_GenericRepository<WithheldSalaryPaymentSearchModel> withheldSalarySearch,
            PGM_GenericRepository<PGM_WithheldSalaryPayment> withheldSalary,
            PGM_GenericRepository<PGM_Salary> salaryMaster,
            PGM_GenericRepository<PGM_SalaryDetail> salaryDetail,
            PGM_GenericRepository<PGM_TaxOpening> taxOpening,
            PGM_GenericRepository<PGM_TaxOpeningDetail> taxOpeningDetail,
            PGM_GenericRepository<TaxOpeningDetail> taxOpeningDetailcustom,
            PGM_GenericRepository<PGM_GratuityPayment> gratuitySettlement,
            PGM_GenericRepository<GratuitySettlementSearchModel> gratuitySettlementSearch,
            PGM_GenericRepository<EmployeeBasicSalary> empBasicSalary,
            PGM_GenericRepository<PGM_FinalSettlement> finalSettlements,
            PGM_GenericRepository<FinalSettlementSearchModel> finalSettlementSearchM,
            PGM_GenericRepository<OtherAdjustDeductSearchModel> otherAdjustDeductSearchM,
            PGM_GenericRepository<PGM_OtherAdjustDeduct> otherAdjustDeduction,
            PGM_GenericRepository<PGM_OtherAdjustDeductAll> otherAdjustDeductionAll,
            PGM_GenericRepository<PGM_VehicleDeduction> vehicleDeduction,
            PGM_GenericRepository<VehicleDeductionBillList> vehicleDeductions,
            PGM_GenericRepository<SalaryMonthInfo> salaryMonth,
            PGM_GenericRepository<MonthlySalaryDetail> salaryDetailInfo,
            PGM_GenericRepository<BonusProcessSearchModel> bonusSearch,
            PGM_GenericRepository<BonusDetailsSearchModel> bonusDetailsSearch,
            PGM_GenericRepository<PGM_SalaryDistribution> salaryDist,
            PGM_GenericRepository<MonthlySalaryDistributionDetaiCustomModell> salaryDistSearch,
            PGM_GenericRepository<IncomeTaxComputationCustomModel> incomeTaxComp,
            PGM_GenericRepository<PGM_EmpTax> tax,
            PGM_GenericRepository<BonusTypeCustomModel> bonusTypeSearch,
            PGM_GenericRepository<PGM_SalaryWithheld> salaryWithheld,
            PGM_GenericRepository<PGM_HouseRentRule> houseRentRule,
            PGM_GenericRepository<PGM_HouseRentRuleDetail> houseRentRuleDetail,
            PGM_GenericRepository<PGM_HouseMaintenanceCharge> houseMaintenanceCharge,
            PGM_GenericRepository<PGM_HouseMaintenanceChargeDetail> houseMaintenanceChargeDetail,
            PGM_GenericRepository<PGM_ElectricBill> electricBill,
            PGM_GenericRepository<PGM_NightBillPayment> nightBillPayment,
            PGM_GenericRepository<PGM_NightBillPaymentDetail> nightBillPaymentDetail,
            PGM_GenericRepository<PGM_ChargeAllowanceRule> chargeAllowanceRule,
            PGM_GenericRepository<PGM_GroupInsurancePaymentType> groupInsurancePaymentType,
            PGM_GenericRepository<PGM_EmpChargeAllowance> empChargeAllowance,
            PGM_GenericRepository<PGM_GroupInsurancePayment> groupInsurancePayment,
            PGM_GenericRepository<PGM_HouseRentWaterBillDeduct> houseRentWaterBillDeduct,
            PGM_GenericRepository<IncentiveBonusProcessSearchModel> incentiveBonusProcessSearchModel,
            PGM_GenericRepository<IncentiveBonusDetailProcessSearchModel> incentiveBonusDetailProcessSearchModel,
            PGM_GenericRepository<PGM_IncentiveBonus> incentiveBonus,
            PGM_GenericRepository<PGM_IncentiveBonusDetail> incentiveBonusDetail,

            PGM_GenericRepository<PGM_ArrearAdjustment> arrearAdjustment,
            PGM_GenericRepository<PGM_ArrearAdjustmentDetail> arrearAdjustmentDetail,
            PGM_GenericRepository<PGM_Overtime> overtime,
            PGM_GenericRepository<AmountInWords> amountInWords,
            PGM_GenericRepository<PGM_TaxRegionRule> taxRegionRule,
            PGM_GenericRepository<PGM_TaxRegionWiseMinTax> taxRegionWiseMinTax,
            PGM_GenericRepository<PGM_TaxOtherInvestAndAdvPaid> taxOtherInvAndAdvPaid,
            PGM_GenericRepository<PGM_TaxOtherInvestAndAdvPaidDetail> taxOtherInvAndAdvPaidDetails,
            PGM_GenericRepository<PGM_Document> document,
            PGM_GenericRepository<PGM_TaxExemptionRule> taxExemptionRule,
            PGM_GenericRepository<PGM_TaxExemptionRuleDetail> taxExemptionRuleDetail,
            PGM_GenericRepository<PGM_EntityDocument> entityDocument,
            PGM_GenericRepository<PGM_TaxOtherInvestmentType> taxOtherInvestmentType,
            PGM_GenericRepository<PGM_TaxInvestmentRebateRule> taxInvestmentRebateRule,
            PGM_GenericRepository<PGM_TaxInvestmentRebateRuleMaster> taxInvestmentRebateRuleMaster,
            PGM_GenericRepository<PGM_TaxInvestmentRebateRuleDetail> taxInvestmentRebateRuleDetail,
            PGM_GenericRepository<PGM_TaxWithheld> taxWithheld,
            PGM_GenericRepository<acc_ChartofAccounts> accChartOfAccount,
            PGM_GenericRepository<acc_FundControlInformation> accFundControlInfo,
            PGM_GenericRepository<PGM_Configuration> configuration,

            PGM_GenericRepository<acc_BankInformation> accBankInfo,
            PGM_GenericRepository<acc_BankBranchInformation> accBankBranchInfo,
            PGM_GenericRepository<acc_BankAccountInformation> accBankAccountInfo,
            PGM_GenericRepository<acc_ChequeRegister> accChequeRegister,
            PGM_GenericRepository<acc_Accounting_Period_Information> accAccountingPeriod,


            PGM_GenericRepository<PGM_Attendance> attendance,
            PGM_GenericRepository<PGM_Refreshment> refreshment,
            PGM_GenericRepository<ELMAH_Error> elmahError,
            PGM_GenericRepository<CustomPropertyAttribute> customPropertyAttr,
            PGM_GenericRepository<CPF_Settlement> cpfSettlement
            )
        {
            _functionRepository = functionRepository;
            _taxRule = taxRule;
            _bonusType = bonusType;
            _bonus = bonus;
            _bonusDetails = bonusDetails;
            _salaryWithheld = salaryWithheld;

            _religion = religion;

            _designation = designation;
            _division = division;
            _region = region;
            _empSalary = empSalary;
            _empSalaryDetails = empSalaryDetails;
            _salaryStructureRepository = salaryStructureRepository;
            _salaryStructureDetailRepository = salaryStructureDetailRepository;
            _salaryHeadGroup = salaryHeadGroup;
            _salaryHead = salaryHead;
            _salaryScale = salaryScale;
            _grade = grade;
            _gradeStep = gradeStep;
            _employmentType = employmentType;
            _section = section;
            _staffCategory = staffCategory;
            _empStatusChange = empStatusChange;
            _bankName = bankName;
            _bankBranch = bankBranch;
            _bankMaster = bankMaster;
            _bankDetails = bankDetails;
            _empSeperation = empSeperation;
            _employmentStatus = employmentStatus;

            _bankaccount = bankaccount;
            _bankLetterTemplate = bankLetterTemplate;
            _bankLetterVariables = bankLetterVariables;
            _bankLetterText = bankLetterText;

            _gratuityrule = gratuityrule;
            _leavetype = leavetype;
            _taxrate = taxrate;
            _taxratedetail = taxratedetail;
            _overTimeSearch = overTimeSearch;
            _leaveEncashment = leaveEncashment;
            _leaveEncashmentSearch = leaveEncashmentSearch;
            _salaryRateCalculation = salaryRateCalculation;
            _leaveType = leaveType;
            _leaveApplication = leaveApplication;
            _leaveInformation = leaveInfo;
            _leaveYear = leaveYear;
            _closingBalance = closingBalance;
            _bankLetter = bankLetter;
            _bankLetterDetails = bankLetterDetails;
            _bankAdviceLetterSearch = bankAdviceLetter;
            _bankDetailCustom = bankLetterDetailsCustom;
            _withheldSalarySearch = withheldSalarySearch;
            _withheldSalary = withheldSalary;
            _salaryMaster = salaryMaster;
            _salaryDetail = salaryDetail;
            _taxopening = taxOpening;
            _taxopeningdetail = taxOpeningDetail;
            _taxopeningdetailcustom = taxOpeningDetailcustom;
            _gratuitySettlement = gratuitySettlement;
            _gratuitySettlementSearch = gratuitySettlementSearch;
            _empBasicSalary = empBasicSalary;
            _finalSettlement = finalSettlements;
            _finalSettlementSearch = finalSettlementSearchM;
            _otherAdjustDeductSearch = otherAdjustDeductSearchM;
            _otheradjustdeduct = otherAdjustDeduction;
            _otheradjustdeductAll = otherAdjustDeductionAll;
            _vehicleDeduction = vehicleDeduction;
            _vehicleDeductionList = vehicleDeductions;
            _salaryMonth = salaryMonth;
            _salaryDetailInfo = salaryDetailInfo;
            _bonusSearch = bonusSearch;
            _bonusDetailsSearch = bonusDetailsSearch;
            _salaryDistribution = salaryDist;
            _salaryDistributionSearch = salaryDistSearch;
            _incomeTaxComputation = incomeTaxComp;
            _tax = tax;
            _bonusTypeSearch = bonusTypeSearch;
            _houseRentRule = houseRentRule;
            _houseRentRuleDetail = houseRentRuleDetail;
            _houseMaintenanceCharge = houseMaintenanceCharge;
            _houseMaintenanceChargeDetail = houseMaintenanceChargeDetail;
            _electricBill = electricBill;
            _nightBillPayment = nightBillPayment;
            _nightBillPaymentDetail = nightBillPaymentDetail;
            _chargeAllowanceRule = chargeAllowanceRule;
            _groupInsurancePaymentType = groupInsurancePaymentType;
            _empChargeAllowance = empChargeAllowance;
            _groupInsurancePayment = groupInsurancePayment;
            _houseRentWaterBillDeduct = houseRentWaterBillDeduct;
            _incentiveBonusSearchModel = incentiveBonusProcessSearchModel;
            _incentiveBonusDetailProcessSearchModel = incentiveBonusDetailProcessSearchModel;
            _incentiveBonus = incentiveBonus;
            _incentiveBonusDetail = incentiveBonusDetail;

            _arrearAdjustment = arrearAdjustment;
            _arrearAdjustmentDetail = arrearAdjustmentDetail;
            _overtime = overtime;
            _amountInWords = amountInWords;
            _taxRegionRule = taxRegionRule;
            _taxRegionWiseMinTax = taxRegionWiseMinTax;
            _taxOtherInvAndAdvPaid = taxOtherInvAndAdvPaid;
            _taxOtherInvAndAdvPaidDetail = taxOtherInvAndAdvPaidDetails;
            _document = document;
            _taxExemptionRule = taxExemptionRule;
            _taxExemptionRuleDetail = taxExemptionRuleDetail;
            _entityDocument = entityDocument;
            _taxOtherInvestmentType = taxOtherInvestmentType;
            _taxInvestmentRebateRule = taxInvestmentRebateRule;
            _taxInvestmentRebateRuleMaster = taxInvestmentRebateRuleMaster;
            _taxInvestmentRebateRuleDetail = taxInvestmentRebateRuleDetail;
            _taxWithheld = taxWithheld;
            _accChartOfAccount = accChartOfAccount;
            _accFundControlInfo = accFundControlInfo;
            _configuration = configuration;
            _accBankInfo = accBankInfo;
            _accBankBranchInfo = accBankBranchInfo;
            _accBankAccountInfo = accBankAccountInfo;
            _accChequeRegister = accChequeRegister;
            _accAccountingPeriod = accAccountingPeriod;
            _attendance = attendance;
            _refreshment = refreshment;
            _elmahError = elmahError;
            _customPropertyAttr = customPropertyAttr;
            _cpfSettlement = cpfSettlement;
        }
        #endregion

        #region Properties

        public PGM_ExecuteFunctions FunctionRepository { get { return _functionRepository; } }

        public PGM_GenericRepository<PGM_HouseMaintenanceChargeDetail> HouseMaintenanceChargeDetailsRepository { get { return _houseMaintenanceChargeDetail; } }
        public PGM_GenericRepository<PGM_HouseMaintenanceCharge> HouseMaintenanceChargeMasterRepository { get { return _houseMaintenanceCharge; } }
        public PGM_GenericRepository<PGM_NightBillPayment> NightBillPaymentMasterRepository { get { return _nightBillPayment; } }
        public PGM_GenericRepository<PGM_NightBillPaymentDetail> NightBillPaymentDetailsRepository { get { return _nightBillPaymentDetail; } }
        public PGM_GenericRepository<PGM_ElectricBill> ElectricBillRepository { get { return _electricBill; } }
        public PGM_GenericRepository<PGM_GroupInsurancePaymentType> GroupInsurancePaymentTypeRepository { get { return _groupInsurancePaymentType; } }
        public PGM_GenericRepository<PGM_GroupInsurancePayment> GroupInsurancePaymentRepository { get { return _groupInsurancePayment; } }
        public PGM_GenericRepository<PGM_SalaryWithheld> SalaryWithheldRepository { get { return _salaryWithheld; } }
        public PGM_GenericRepository<PGM_OtherAdjustDeduct> OtherAdjustDeductionRepository { get { return _otheradjustdeduct; } }
        public PGM_GenericRepository<PGM_OtherAdjustDeductAll> OtherAdjustmentDeductionAllRepository { get { return _otheradjustdeductAll; } }
        public PGM_GenericRepository<PGM_TaxOpening> TaxOpeningMasterRepository { get { return _taxopening; } }
        public PGM_GenericRepository<PGM_TaxOpeningDetail> TaxOpeningDetailRepository { get { return _taxopeningdetail; } }
        public PGM_GenericRepository<PGM_TaxRateRule> TaxRateMasterRepository { get { return _taxrate; } }
        public PGM_GenericRepository<PGM_TaxRateRuleDetail> TaxRateDetailsRepository { get { return _taxratedetail; } }
        public PGM_GenericRepository<PGM_TaxRule> TaxRule { get { return _taxRule; } }
        public PGM_GenericRepository<PGM_BonusType> BonusTypeRepository { get { return _bonusType; } }
        public PGM_GenericRepository<PGM_Bonus> BonusMasterRepository { get { return _bonus; } }
        public PGM_GenericRepository<PGM_BonusDetail> BonusDetailsRepository { get { return _bonusDetails; } }

        public PGM_GenericRepository<PRM_Religion> Religion { get { return _religion; } }

        public PGM_GenericRepository<PRM_Designation> DesignationRepository { get { return _designation; } }
        public PGM_GenericRepository<PRM_Division> DivisionRepository { get { return _division; } }
        public PGM_GenericRepository<PRM_Region> RegionRepository { get { return _region; } }
        public PGM_GenericRepository<PRM_EmpSalary> EmpSalaryRepository { get { return _empSalary; } }
        public PGM_GenericRepository<PRM_EmpSalaryDetail> EmpSalaryDetailRepository { get { return _empSalaryDetails; } }
        public PGM_GenericRepository<PRM_SalaryStructure> SalaryStructureRepository { get { return _salaryStructureRepository; } }
        public PGM_GenericRepository<PRM_SalaryStructureDetail> SalaryStructureDetailRepository { get { return _salaryStructureDetailRepository; } }
        public PGM_GenericRepository<PRM_SalaryHeadGroup> SalaryHeadGroupRepository { get { return _salaryHeadGroup; } }
        public PGM_GenericRepository<PRM_SalaryHead> SalaryHeadRepository { get { return _salaryHead; } }
        public PGM_GenericRepository<PRM_SalaryScale> SalaryScaleRepository { get { return _salaryScale; } }
        public PGM_GenericRepository<PRM_JobGrade> JobGradeRepository { get { return _grade; } }
        public PGM_GenericRepository<PRM_GradeStep> JobGradeStepRepository { get { return _gradeStep; } }
        public PGM_GenericRepository<PRM_EmploymentType> EmploymentTypeRepository { get { return _employmentType; } }
        public PGM_GenericRepository<PRM_Section> SectionRepository { get { return _section; } }
        public PGM_GenericRepository<PRM_StaffCategory> StaffCategoryRepository { get { return _staffCategory; } }
        public PGM_GenericRepository<PRM_EmpStatusChange> EmpStatusChangeRepository { get { return _empStatusChange; } }
        public PGM_GenericRepository<PRM_BankName> BankNameRepository { get { return _bankName; } }
        public PGM_GenericRepository<PRM_BankBranch> BankBranchRepository { get { return _bankBranch; } }
        public PGM_GenericRepository<FAM_BankBranchMap> BankMaster { get { return _bankMaster; } }
        public PGM_GenericRepository<FAM_BankBranchAccountNo> BankDetails { get { return _bankDetails; } }
        public PGM_GenericRepository<PRM_EmpSeperation> EmpSeperationRepository { get { return _empSeperation; } }
        public PGM_GenericRepository<PRM_EmploymentStatus> EmploymentStatusRepository { get { return _employmentStatus; } }

        public PGM_GenericRepository<PGM_BankAccount> BankAccount { get { return _bankaccount; } }
        public PGM_GenericRepository<PGM_BankAdviceLetterTemplate> BankAdviceLetterTemplateRepository { get { return _bankLetterTemplate; } }
        public PGM_GenericRepository<PGM_BankAdviceLetterVariables> BankAdviceLetterVariablesRepository { get { return _bankLetterVariables; } }
        public PGM_GenericRepository<PGM_BankAdviceLetterSubjectBody> BankAdviceLetterBodyTextRepository { get { return _bankLetterText; } }
        public PGM_GenericRepository<PGM_GratuityRule> GratuityRule { get { return _gratuityrule; } }
        public PGM_GenericRepository<LMS_tblLeaveType> LeaveType { get { return _leavetype; } }
        public PGM_GenericRepository<OverTimeSearch> OverTimeSearch { get { return _overTimeSearch; } }
        public PGM_GenericRepository<PGM_LeaveEncashment> LeaveEncashment { get { return _leaveEncashment; } }
        public PGM_GenericRepository<LMS_tblLeaveType> LeaveTypeInfo { get { return _leaveType; } }
        public PGM_GenericRepository<LMS_tblLeaveLedger> LeaveInformation { get { return _leaveInformation; } }
        public PGM_GenericRepository<LMS_tblLeaveApplication> LeaveApplication { get { return _leaveApplication; } }
        public PGM_GenericRepository<LMS_tblLeaveYear> LeaveYear { get { return _leaveYear; } }
        public PGM_GenericRepository<PGM_BankLetter> BankAdviceLetters { get { return _bankLetter; } }
        public PGM_GenericRepository<PGM_BankLetterDetail> BankAdviceLetterDetails { get { return _bankLetterDetails; } }
        public PGM_GenericRepository<PGM_WithheldSalaryPayment> WithheldSalaryPayment { get { return _withheldSalary; } }
        public PGM_GenericRepository<PGM_Salary> SalaryMasterRepository { get { return _salaryMaster; } }
        public PGM_GenericRepository<PGM_SalaryDetail> SalaryDetailsRepository { get { return _salaryDetail; } }
        public PGM_GenericRepository<PGM_GratuityPayment> GratuitySettlement { get { return _gratuitySettlement; } }
        public PGM_GenericRepository<PGM_FinalSettlement> FinalSettlementRepository { get { return _finalSettlement; } }
        public PGM_GenericRepository<PGM_VehicleDeduction> VehicleDeductionRepository { get { return _vehicleDeduction; } }
        public PGM_GenericRepository<PGM_EmpTax> EmpTaxRepository { get { return _tax; } }
        public PGM_GenericRepository<PGM_HouseRentRule> HouseRentRuleRepositoty { get { return _houseRentRule; } }
        public PGM_GenericRepository<PGM_HouseRentRuleDetail> HouseRentRuleDetailRepositoty { get { return _houseRentRuleDetail; } }
        public PGM_GenericRepository<PGM_ChargeAllowanceRule> ChargeAllowanceRuleRepositoty { get { return _chargeAllowanceRule; } }
        public PGM_GenericRepository<PGM_EmpChargeAllowance> EmpChargeAllowanceRepositoty { get { return _empChargeAllowance; } }
        public PGM_GenericRepository<PGM_HouseRentWaterBillDeduct> HouseRentWaterBillDeductRepositoty { get { return _houseRentWaterBillDeduct; } }
        public PGM_GenericRepository<PGM_SalaryDistribution> SalaryDistributionMasterRepository { get { return _salaryDistribution; } }
        public PGM_GenericRepository<PGM_IncentiveBonus> IncentiveBonusRepository { get { return _incentiveBonus; } }
        public PGM_GenericRepository<PGM_IncentiveBonusDetail> IncentiveBonusDetailRepository { get { return _incentiveBonusDetail; } }
        public PGM_GenericRepository<PGM_ArrearAdjustment> ArrearAdjustmentRepository { get { return _arrearAdjustment; } }
        public PGM_GenericRepository<PGM_ArrearAdjustmentDetail> ArrearAdjustmentDetailRepository { get { return _arrearAdjustmentDetail; } }
        public PGM_GenericRepository<PGM_Overtime> OvertimeRepository { get { return _overtime; } }
        public PGM_GenericRepository<AmountInWords> AmountInBengaliWords { get { return _amountInWords; } }
        public PGM_GenericRepository<PGM_TaxRegionRule> TaxRegionRuleRepository { get { return _taxRegionRule; } }
        public PGM_GenericRepository<PGM_TaxRegionWiseMinTax> TaxRegionWiseMinRuleRepository { get { return _taxRegionWiseMinTax; } }
        public PGM_GenericRepository<PGM_TaxOtherInvestAndAdvPaid> TaxOtherInvestAndAdvPaidRepository { get { return _taxOtherInvAndAdvPaid; } }
        public PGM_GenericRepository<PGM_TaxOtherInvestAndAdvPaidDetail> TaxOtherInvestAndAdvPaidDetailsRepository { get { return _taxOtherInvAndAdvPaidDetail; } }
        public PGM_GenericRepository<PGM_Document> DocumentRepository { get { return _document; } }
        public PGM_GenericRepository<PGM_TaxExemptionRule> TaxExemptionRuleRepository { get { return _taxExemptionRule; } }
        public PGM_GenericRepository<PGM_TaxExemptionRuleDetail> TaxExemptionRuleDetailRepository { get { return _taxExemptionRuleDetail; } }
        public PGM_GenericRepository<PGM_EntityDocument> EntityDocumentRepository { get { return _entityDocument; } }
        public PGM_GenericRepository<PGM_TaxOtherInvestmentType> OtherInvestmentTypeRepository { get { return _taxOtherInvestmentType; } }
        public PGM_GenericRepository<PGM_TaxInvestmentRebateRule> TaxInvestmentRebateRuleRepository { get { return _taxInvestmentRebateRule; } }
        public PGM_GenericRepository<PGM_TaxInvestmentRebateRuleMaster> TaxInvestmentRebateRuleMasterRepository { get { return _taxInvestmentRebateRuleMaster; } }
        public PGM_GenericRepository<PGM_TaxInvestmentRebateRuleDetail> TaxInvestmentRebateRuleDetailRepository { get { return _taxInvestmentRebateRuleDetail; } }

        public PGM_GenericRepository<EmployeeBasicSalary> EmployeeBasicSalarySearch { get { return _empBasicSalary; } }
        public PGM_GenericRepository<FinalSettlementSearchModel> FinalSettlementSearch { get { return _finalSettlementSearch; } }
        public PGM_GenericRepository<OtherAdjustDeductSearchModel> OtehrAdjustDeductSearch { get { return _otherAdjustDeductSearch; } }
        public PGM_GenericRepository<VehicleDeductionBillList> VehicleDeductionBillList { get { return _vehicleDeductionList; } }
        public PGM_GenericRepository<SalaryMonthInfo> SalaryMonthList { get { return _salaryMonth; } }
        public PGM_GenericRepository<MonthlySalaryDetail> MonthlySalaryDetailInfo { get { return _salaryDetailInfo; } }
        public PGM_GenericRepository<BonusProcessSearchModel> BonusProcessSearch { get { return _bonusSearch; } }
        public PGM_GenericRepository<IncentiveBonusProcessSearchModel> IncentiveBonusProcessSearch { get { return _incentiveBonusSearchModel; } }
        public PGM_GenericRepository<BonusDetailsSearchModel> BonusDetailsSearch { get { return _bonusDetailsSearch; } }
        public PGM_GenericRepository<MonthlySalaryDistributionDetaiCustomModell> SalaryDistributionSearch { get { return _salaryDistributionSearch; } }
        public PGM_GenericRepository<IncomeTaxComputationCustomModel> IncomeTaxComputationSearch { get { return _incomeTaxComputation; } }
        public PGM_GenericRepository<BonusTypeCustomModel> BonusTypeCustopmSearch { get { return _bonusTypeSearch; } }
        public PGM_GenericRepository<IncentiveBonusDetailProcessSearchModel> IncentiveBonusDetailProcessSearchRepository { get { return _incentiveBonusDetailProcessSearchModel; } }
        public PGM_GenericRepository<TaxOpeningDetail> TaxopeningDatailCustom { get { return _taxopeningdetailcustom; } }
        public PGM_GenericRepository<BankAdviceLetterSearchModel> BankAdviceLetterSearch { get { return _bankAdviceLetterSearch; } }
        public PGM_GenericRepository<BankAdviceLetterDetailCustomModel> BankAdviceLetterDetailsCustom { get { return _bankDetailCustom; } }
        public PGM_GenericRepository<WithheldSalaryPaymentSearchModel> WithheldSalaryPaymentSearch { get { return _withheldSalarySearch; } }
        public PGM_GenericRepository<ClosingBalanceSearchModel> ClosingBalance { get { return _closingBalance; } }
        public PGM_GenericRepository<GratuitySettlementSearchModel> GratuitySettlementSearch { get { return _gratuitySettlementSearch; } }
        public PGM_GenericRepository<LeaveEncashmentSearchModel> LeaveEncashmentSearch { get { return _leaveEncashmentSearch; } }
        public PGM_GenericRepository<SalaryRateCalculation> SalaryRateCalculation { get { return _salaryRateCalculation; } }
        public PGM_GenericRepository<PGM_TaxWithheld> TaxWithheldRepository { get { return _taxWithheld; } }
        public PGM_GenericRepository<acc_ChartofAccounts> AccChartOfAccountRepository { get { return _accChartOfAccount; } }
        public PGM_GenericRepository<acc_FundControlInformation> AccFundControlInfoRepository { get { return _accFundControlInfo; } }
        public PGM_GenericRepository<PGM_Configuration> PgmConfiguration { get { return _configuration; } }

        public PGM_GenericRepository<acc_BankInformation> AccBankInfo { get { return _accBankInfo; } }
        public PGM_GenericRepository<acc_BankBranchInformation> AccBankBranchInfo { get { return _accBankBranchInfo; } }
        public PGM_GenericRepository<acc_BankAccountInformation> AccBankAccountInfo { get { return _accBankAccountInfo; } }
        public PGM_GenericRepository<acc_ChequeRegister> AccChequeRegister { get { return _accChequeRegister; } }
        public PGM_GenericRepository<acc_Accounting_Period_Information> AccAccountingPeriod { get { return _accAccountingPeriod; } }

        public PGM_GenericRepository<PGM_Attendance> AttendanceRepository { get { return _attendance; } }
        public PGM_GenericRepository<PGM_Refreshment> RefreshmentRepository { get { return _refreshment; } }

        public PGM_GenericRepository<ELMAH_Error> ELMAHErrorRepository { get { return _elmahError; } }
        public PGM_GenericRepository<CustomPropertyAttribute> CustomPropertyAttributeRepository { get { return _customPropertyAttr; } }
        public PGM_GenericRepository<CPF_Settlement> CpfSettlementRepository{get { return _cpfSettlement; }}

        #endregion
    }
}