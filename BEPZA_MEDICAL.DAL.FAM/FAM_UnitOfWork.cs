using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.DAL.FAM.CustomEntities;

namespace BEPZA_MEDICAL.DAL.FAM
{
    public class FAM_UnitOfWork
    {
        #region Fields

        FAM_ExecuteFunctions _functionRepository;
        FAM_GenericRepository<CommonConfigType> _configType;
        FAM_GenericRepository<FAM_ChartOfAccount> _coa;
        private readonly FAM_GenericRepository<FAM_BankBranchMap> _bankMaster;
        FAM_GenericRepository<FAM_BankBranchAccountNo> _bankDetails;
        FAM_GenericRepository<FAM_VendorInformation> _vendorInformation;
        FAM_GenericRepository<FAM_FinancialYearInformation> _financialYearInformation;
        private readonly FAM_GenericRepository<BankSearch> _bankSearch;
        private readonly FAM_GenericRepository<FAM_ChequeInfo> _chequeInfoDetails;
        private readonly FAM_GenericRepository<FAM_ChequeBookInfo> _chequeInfoMaster;
        FAM_GenericRepository<FAM_DivisionUnitBudgetHead> _divisionUnitBudgetHead;
        private readonly FAM_GenericRepository<DivisionSearch> _divisionSearch;
        private readonly FAM_GenericRepository<ChequeSearch> _chequeSearch;
        private readonly FAM_GenericRepository<FAM_ApprovalPath> _approvalPathMaster;
        private readonly FAM_GenericRepository<FAM_ApprovalPathNodeInfo> _approvalPathDetails;
        private readonly FAM_GenericRepository<FAM_DivisionUnitBudgetAllocation> _divisionUnitBudgetAllocation;
        private readonly FAM_GenericRepository<FAM_DivisionUnitBudgetAllocationDetails> _budgetAllocation;
        private readonly FAM_GenericRepository<DivisionFinancialYearSearch> _divisionFinancialYearSearch;
        private readonly FAM_GenericRepository<DivisionUnitFinancialYearRevisionSearch> _divisionUnitFinancialYearRevisionSearch;
        private readonly FAM_GenericRepository<FAM_CentralBudgetAllocation> _centralBudgetInformation;
        private readonly FAM_GenericRepository<FAM_CentralBudgetAllocationDetails> _centralBudgetInformationDetail;
        private readonly FAM_GenericRepository<CentralBudgetFinancialYearSearch> _centralBudgetFinancialYearSearch;
        private readonly FAM_GenericRepository<CentralBudgetAllocationFYSearch> _centralBudgetAllocationFYSearch;
        private readonly FAM_GenericRepository<FAM_VoucherInfo> _voucherMaster;
        private readonly FAM_GenericRepository<FAM_VoucherInfoDetails> _voucherDetails;
        private readonly FAM_GenericRepository<FAM_CentralBudgetRevision> _revisionCentralBudgetInformation;
        private readonly FAM_GenericRepository<FAM_CentralBudgetRevisionDetails> _revisionCentralBudgetInformationDetail;
        private readonly FAM_GenericRepository<RevisionOfCentralBudgetFYSearch> _revisionOfCentralBudgetFYSearch;
        private readonly FAM_GenericRepository<RevisionCentralBudgetByFYSearch> _revisionCentralBudgetAllocationFYSearch;
        private readonly FAM_GenericRepository<FAM_PurposeAccountHeadMapping> _purposeAccountHeadMapping;
        private readonly FAM_GenericRepository<FAM_Purpose> _purpose;
        private readonly FAM_GenericRepository<SpecialAccountAssignmentSearch> _specialAccountAssignmentSearch;
        private readonly FAM_GenericRepository<FAM_BSPLReportHeadMapping> _bsplReportHeadMapping;
        private readonly FAM_GenericRepository<FAM_BSPLReportHeadMappingDetails> _bsplReportHeadMappingDetails;
        private readonly FAM_GenericRepository<FAM_BSPLReportHeadName> _bsplReportHeadName;
        private readonly FAM_GenericRepository<FAM_ReportName> _reportName;
        private readonly FAM_GenericRepository<BSPLReportHeadMappingSearch> _bsplReportHeadMappingSearch;
        private readonly FAM_GenericRepository<FAM_ApprovalComments> _approvalComments;
        private readonly FAM_GenericRepository<FAM_DivisionUnitBudgetRevision> _divisionUnitBudgetRevision;
        private readonly FAM_GenericRepository<FAM_DivisionUnitBudgetRevisionDetails> _divisionUnitBudgetRevisionDetail;
        private readonly FAM_GenericRepository<DivisionUnitBudgetByDUFYSearch> _divisionUnitBudgetByDUFYSearch;
        private readonly FAM_GenericRepository<VoucherSearch> _voucherSearch;

        private readonly FAM_GenericRepository<FAM_VoucherType> _voucherType;
        

        #endregion

        #region Constactor

        public FAM_UnitOfWork(
            FAM_ExecuteFunctions functionRepository,
            FAM_GenericRepository<CommonConfigType> configType,
            FAM_GenericRepository<FAM_ChartOfAccount> coa,
            FAM_GenericRepository<FAM_BankBranchMap> bankMaster,
            FAM_GenericRepository<FAM_BankBranchAccountNo> bankDetails,
            FAM_GenericRepository<FAM_VendorInformation> vendorInformation,
            FAM_GenericRepository<FAM_FinancialYearInformation> financialYearInformation,
            FAM_GenericRepository<BankSearch> bankSearch,
            FAM_GenericRepository<FAM_ChequeBookInfo> chequeInfoMaster,
            FAM_GenericRepository<FAM_ChequeInfo> chequeInfoDetails,
            FAM_GenericRepository<FAM_DivisionUnitBudgetHead> divisionUnitBudgetHead,
            FAM_GenericRepository<DivisionSearch> divisionSearch,
            FAM_GenericRepository<ChequeSearch> chequeSearch,
            FAM_GenericRepository<FAM_ApprovalPath> approvalPathMaster,
            FAM_GenericRepository<FAM_ApprovalPathNodeInfo> approvalPathDetails,
            FAM_GenericRepository<FAM_DivisionUnitBudgetAllocation> divisionUnitBudgetAllocation,
            FAM_GenericRepository<FAM_DivisionUnitBudgetAllocationDetails> budgetAllocation,
            FAM_GenericRepository<DivisionFinancialYearSearch> divisionFinancialYearSearch,
            FAM_GenericRepository<DivisionUnitFinancialYearRevisionSearch> divisionUnitFinancialYearRevisionSearch,
            FAM_GenericRepository<FAM_CentralBudgetAllocation> centralBudgetInformation,
            FAM_GenericRepository<FAM_CentralBudgetAllocationDetails> centralBudgetInformationDetail,
            FAM_GenericRepository<CentralBudgetFinancialYearSearch> centralBudgetFinancialYearSearch,
            FAM_GenericRepository<CentralBudgetAllocationFYSearch> centralBudgetAllocationFYSearch,
            FAM_GenericRepository<FAM_VoucherInfo> voucherMaster,
            FAM_GenericRepository<FAM_VoucherInfoDetails> voucherDetails,
            FAM_GenericRepository<FAM_CentralBudgetRevision> revisionCentralBudgetInformation,
            FAM_GenericRepository<FAM_CentralBudgetRevisionDetails> revisionCentralBudgetInformationDetail,
            FAM_GenericRepository<RevisionOfCentralBudgetFYSearch> revisionOfCentralBudgetFYSearch,
            FAM_GenericRepository<RevisionCentralBudgetByFYSearch> revisionCentralBudgetAllocationFYSearch,
            FAM_GenericRepository<FAM_PurposeAccountHeadMapping> purposeAccountHeadMapping,
            FAM_GenericRepository<FAM_Purpose> purpose,
            FAM_GenericRepository<SpecialAccountAssignmentSearch> specialAccountAssignmentSearch,
            FAM_GenericRepository<FAM_BSPLReportHeadMapping> bsplReportHeadMapping,
            FAM_GenericRepository<FAM_BSPLReportHeadMappingDetails> bsplReportHeadMappingDetails,
            FAM_GenericRepository<FAM_BSPLReportHeadName> bsplReportHeadName,
            FAM_GenericRepository<FAM_ReportName> reportName,
            FAM_GenericRepository<BSPLReportHeadMappingSearch> bsplReportHeadMappingSearch,
            FAM_GenericRepository<FAM_ApprovalComments> approvalComments,
            FAM_GenericRepository<FAM_DivisionUnitBudgetRevision> divisionUnitBudgetRevision,
            FAM_GenericRepository<FAM_DivisionUnitBudgetRevisionDetails> divisionUnitBudgetRevisionDetail,
            FAM_GenericRepository<DivisionUnitBudgetByDUFYSearch> divisionUnitBudgetByDUFYSearch,
            FAM_GenericRepository<VoucherSearch> voucherSearch,

            FAM_GenericRepository<FAM_VoucherType> voucherType
            )
        {
            _functionRepository = functionRepository;
            _configType = configType;
            _coa = coa;
            _bankMaster = bankMaster;
            _bankDetails = bankDetails;
            _vendorInformation = vendorInformation;
            _financialYearInformation = financialYearInformation;
            _bankSearch = bankSearch;
            _chequeInfoMaster = chequeInfoMaster;
            _chequeInfoDetails = chequeInfoDetails;
            _divisionUnitBudgetHead = divisionUnitBudgetHead;
            _divisionSearch = divisionSearch;
            _chequeSearch = chequeSearch;
            _approvalPathMaster = approvalPathMaster;
            _approvalPathDetails = approvalPathDetails;
            _divisionUnitBudgetAllocation = divisionUnitBudgetAllocation;
            _budgetAllocation = budgetAllocation;
            _divisionFinancialYearSearch = divisionFinancialYearSearch;
            _divisionUnitFinancialYearRevisionSearch = divisionUnitFinancialYearRevisionSearch;
            _centralBudgetInformation = centralBudgetInformation;
            _centralBudgetInformationDetail = centralBudgetInformationDetail;
            _voucherMaster = voucherMaster;
            _voucherDetails = voucherDetails;
            _centralBudgetFinancialYearSearch = centralBudgetFinancialYearSearch;
            _centralBudgetAllocationFYSearch = centralBudgetAllocationFYSearch;
            _revisionCentralBudgetInformation = revisionCentralBudgetInformation;
            _revisionCentralBudgetInformationDetail = revisionCentralBudgetInformationDetail;
            _revisionOfCentralBudgetFYSearch = revisionOfCentralBudgetFYSearch;
            _revisionCentralBudgetAllocationFYSearch = revisionCentralBudgetAllocationFYSearch;
            _purposeAccountHeadMapping = purposeAccountHeadMapping;
            _purpose = purpose;
            _specialAccountAssignmentSearch = specialAccountAssignmentSearch;
            _bsplReportHeadMapping = bsplReportHeadMapping;
            _bsplReportHeadMappingDetails = bsplReportHeadMappingDetails;
            _bsplReportHeadName = bsplReportHeadName;
            _reportName = reportName;
            _bsplReportHeadMappingSearch = bsplReportHeadMappingSearch;
            _approvalComments = approvalComments;
            _voucherSearch = voucherSearch;
            _divisionUnitBudgetRevision = divisionUnitBudgetRevision;
            _divisionUnitBudgetRevisionDetail = divisionUnitBudgetRevisionDetail;
            _divisionUnitBudgetByDUFYSearch = divisionUnitBudgetByDUFYSearch;

            _voucherType = voucherType;
        }

        #endregion

        #region Properties

        public FAM_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        public FAM_GenericRepository<CommonConfigType> ConfigType
        {
            get { return _configType; }
        }

        public FAM_GenericRepository<FAM_ChartOfAccount> ChartOfAccount
        {
            get { return _coa; }
        }

        public FAM_GenericRepository<FAM_BankBranchMap> BankMaster
        {
            get { return _bankMaster; }
        }

        public FAM_GenericRepository<FAM_BankBranchAccountNo> BankDetails
        {
            get { return _bankDetails; }
        }

        public FAM_GenericRepository<FAM_VendorInformation> VendorInformationRepository
        {
            get { return _vendorInformation; }
        }

        public FAM_GenericRepository<FAM_FinancialYearInformation> FinancialYearInformationRepository
        {
            get { return _financialYearInformation; }
        }

        public FAM_GenericRepository<BankSearch> BankSearch
        {
            get { return _bankSearch; }
        }

        public FAM_GenericRepository<FAM_ChequeBookInfo> ChequeInfoMaster
        {
            get { return _chequeInfoMaster; }
        }

        public FAM_GenericRepository<FAM_ChequeInfo> ChequeInfoDetails
        {
            get { return _chequeInfoDetails; }
        }

        public FAM_GenericRepository<FAM_DivisionUnitBudgetHead> DivisionUnitBudgetHeadRepository
        {
            get { return _divisionUnitBudgetHead; }
        }

        public FAM_GenericRepository<DivisionSearch> DivisionSearch
        {
            get { return _divisionSearch; }
        }

        public FAM_GenericRepository<ChequeSearch> ChequeSearch
        {
            get { return _chequeSearch; }
        }

        public FAM_GenericRepository<FAM_ApprovalPath> ApprovalPathMaster
        {
            get { return _approvalPathMaster; }
        }

        public FAM_GenericRepository<FAM_ApprovalPathNodeInfo> ApprovalPathDetails
        {
            get { return _approvalPathDetails; }
        }

        public FAM_GenericRepository<FAM_DivisionUnitBudgetAllocation> DivisionUnitBudgetAllocationRepository
        {
            get { return _divisionUnitBudgetAllocation; }
        }

        public FAM_GenericRepository<FAM_DivisionUnitBudgetAllocationDetails> BudgetAllocationRepository
        {
            get { return _budgetAllocation; }
        }

        public FAM_GenericRepository<DivisionFinancialYearSearch> DivisionFinancialYearSearch
        {
            get { return _divisionFinancialYearSearch; }
        }

        public FAM_GenericRepository<DivisionUnitFinancialYearRevisionSearch> DivisionUnitFinancialYearRevisionSearch
        {
            get { return _divisionUnitFinancialYearRevisionSearch; }
        }
        
        public FAM_GenericRepository<FAM_CentralBudgetAllocation> CentralBudgetInformationRepository
        {
            get { return _centralBudgetInformation; }
        }

        public FAM_GenericRepository<FAM_CentralBudgetAllocationDetails> CentralBudgetInformationDetailRepository
        {
            get { return _centralBudgetInformationDetail; }
        }

        public FAM_GenericRepository<CentralBudgetFinancialYearSearch> CentralBudgetFinancialYearSearch
        {
            get { return _centralBudgetFinancialYearSearch; }
        }

        public FAM_GenericRepository<CentralBudgetAllocationFYSearch> CentralBudgetAllocationFYSearch
        {
            get { return _centralBudgetAllocationFYSearch; }
        }
        
        public FAM_GenericRepository<FAM_VoucherInfo> voucherMaster
        {
            get { return _voucherMaster; }
        }

        public FAM_GenericRepository<FAM_VoucherInfoDetails> voucherDetails
        {
            get { return _voucherDetails; }
        }

        public FAM_GenericRepository<FAM_CentralBudgetRevision> RevisionCentralBudgetInformationRepository
        {
            get { return _revisionCentralBudgetInformation; }
        }

        public FAM_GenericRepository<FAM_CentralBudgetRevisionDetails> RevisionCentralBudgetInformationDetailRepository
        {
            get { return _revisionCentralBudgetInformationDetail; }
        }

        public FAM_GenericRepository<RevisionOfCentralBudgetFYSearch> RevisionOfCentralBudgetFYSearch
        {
            get { return _revisionOfCentralBudgetFYSearch; }
        }

        public FAM_GenericRepository<RevisionCentralBudgetByFYSearch> RevisionCentralBudgetByFYSearch
        {
            get { return _revisionCentralBudgetAllocationFYSearch; }
        }

        public FAM_GenericRepository<DivisionUnitBudgetByDUFYSearch> DivisionUnitBudgetByDUFYSearch
        {
            get { return _divisionUnitBudgetByDUFYSearch; }
        }

        public FAM_GenericRepository<FAM_PurposeAccountHeadMapping> PurposeAccountHeadMappingRepository
        {
            get { return _purposeAccountHeadMapping; }
        }

        public FAM_GenericRepository<FAM_Purpose> Purpose
        {
            get { return _purpose; }
        }

        public FAM_GenericRepository<SpecialAccountAssignmentSearch> SpecialAccountAssignmentSearch
        {
            get { return _specialAccountAssignmentSearch; }
        }

        public FAM_GenericRepository<FAM_BSPLReportHeadMapping> BSPLReportHeadMappingRepository
        {
            get { return _bsplReportHeadMapping; }
        }

        public FAM_GenericRepository<FAM_BSPLReportHeadMappingDetails> BSPLReportHeadMappingDetailsRepository
        {
            get { return _bsplReportHeadMappingDetails; }
        }

        public FAM_GenericRepository<FAM_BSPLReportHeadName> BSPLReportHeadNameRepository
        {
            get { return _bsplReportHeadName; }
        }

        public FAM_GenericRepository<FAM_ReportName> ReportNameRepository
        {
            get { return _reportName; }
        }

        public FAM_GenericRepository<BSPLReportHeadMappingSearch> BSPLReportHeadMappingSearchRepository
        {
            get { return _bsplReportHeadMappingSearch; }
        }

        public FAM_GenericRepository<VoucherSearch> VoucherSearch
        {
            get { return _voucherSearch; }
        }

        public FAM_GenericRepository<FAM_ApprovalComments> ApprovalComments
        {
            get { return _approvalComments; }
        }

        public FAM_GenericRepository<FAM_DivisionUnitBudgetRevision> DivisionUnitBudgetRevisionRepository
        {
            get { return _divisionUnitBudgetRevision; }
        }

        public FAM_GenericRepository<FAM_DivisionUnitBudgetRevisionDetails> DivisionUnitBudgetRevisionDetailRepository
        {
            get { return _divisionUnitBudgetRevisionDetail; }
        }

        public FAM_GenericRepository<FAM_VoucherType> VoucherTypeRepository
        {
            get { return _voucherType; }
        }

        #endregion
    }


}