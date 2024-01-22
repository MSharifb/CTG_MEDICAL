using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.ChartOfAccount;
using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.VendorInfo;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.BankInfo;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.FinancialYearInfo;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitWiseBudgetHead;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.ChequeInfo;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitBudgetAllocation;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.ApprovalPathInfo;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.CentralBudgetInformation;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.VoucherInfo;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.RevisionOfCentralBudget;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.SpecialAccountAssignment;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.BSPLReportHeadMapping;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.DivisionUnitBudgetRevision;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class FAMMapper
    {
        public FAMMapper()
        {
            //chart of account
            Mapper.CreateMap<ChartOfAccountModel, FAM_ChartOfAccount>();
            Mapper.CreateMap<FAM_ChartOfAccount, ChartOfAccountModel>(); 

            //Vendor Information
            Mapper.CreateMap<VendorInformationModel, FAM_VendorInformation>();
            Mapper.CreateMap<FAM_VendorInformation, VendorInformationModel>();

            //Bank Info
            Mapper.CreateMap<BankInformationModel, FAM_BankBranchMap>();
            Mapper.CreateMap<FAM_BankBranchMap, BankInformationModel>(); 

            Mapper.CreateMap<BankInfoChild, FAM_BankBranchAccountNo>();
            Mapper.CreateMap<FAM_BankBranchAccountNo, BankInfoChild>();

            //Financial Year
            Mapper.CreateMap<FinancialYearInformationModel, FAM_FinancialYearInformation>();
            Mapper.CreateMap<FAM_FinancialYearInformation, FinancialYearInformationModel>();

            //Division/Unit Wise Budget Head 
            Mapper.CreateMap<DivisionUnitWiseBudgetHeadModel, FAM_DivisionUnitBudgetHead>();
            Mapper.CreateMap<FAM_DivisionUnitBudgetHead, DivisionUnitWiseBudgetHeadModel>();

            Mapper.CreateMap<BudgetHeadAllocation, FAM_DivisionUnitBudgetHead>();
            Mapper.CreateMap<FAM_DivisionUnitBudgetHead, BudgetHeadAllocation>();

            //Cheque Info
            Mapper.CreateMap<ChequeInfoModel, FAM_ChequeBookInfo>();
            Mapper.CreateMap<FAM_ChequeBookInfo, ChequeInfoModel>();

            Mapper.CreateMap<ChequeInfoDetails, FAM_ChequeInfo>();
            Mapper.CreateMap<FAM_ChequeInfo, ChequeInfoDetails>();

            //Division/Unit Budget Allocation
            Mapper.CreateMap<DivisionUnitBudgetAllocationModel, FAM_DivisionUnitBudgetAllocation>();
            Mapper.CreateMap<FAM_DivisionUnitBudgetAllocation, DivisionUnitBudgetAllocationModel>();

            Mapper.CreateMap<BudgetAllocation, FAM_DivisionUnitBudgetAllocationDetails>();
            Mapper.CreateMap<FAM_DivisionUnitBudgetAllocationDetails, BudgetAllocation>();

            //Approval Path Info
            Mapper.CreateMap<ApprovalPathInfoModel, FAM_ApprovalPath>();
            Mapper.CreateMap<FAM_ApprovalPath, ApprovalPathInfoModel>();

            Mapper.CreateMap<ApprovalPathDetails, FAM_ApprovalPathNodeInfo>();
            Mapper.CreateMap<FAM_ApprovalPathNodeInfo, ApprovalPathDetails>();

            //Central Budget Information
            Mapper.CreateMap<CentralBudgetInformationModel, FAM_CentralBudgetAllocation>();
            Mapper.CreateMap<FAM_CentralBudgetAllocation, CentralBudgetInformationModel>();

            Mapper.CreateMap<CentralBudget, FAM_CentralBudgetAllocationDetails>();
            Mapper.CreateMap<FAM_CentralBudgetAllocationDetails, CentralBudget>();

            //Approval Path Info
            Mapper.CreateMap<ApprovalPathInfoModel, FAM_VoucherInfoDetails>();
            Mapper.CreateMap<FAM_VoucherInfoDetails, ApprovalPathInfoModel>();

            //Approval Path Info
            Mapper.CreateMap<VoucherInfoModel, FAM_VoucherInfo>();
            Mapper.CreateMap<FAM_VoucherInfo, VoucherInfoModel>();

            Mapper.CreateMap<VoucherInfoDetails, FAM_VoucherInfoDetails>();
            Mapper.CreateMap<FAM_VoucherInfoDetails, VoucherInfoDetails>();


            //Revision of Central Budget
            Mapper.CreateMap<RevisionOfCentralBudgetModel, FAM_CentralBudgetRevision>();
            Mapper.CreateMap<FAM_CentralBudgetRevision, RevisionOfCentralBudgetModel>();

            Mapper.CreateMap<CentralBudgetRevision, FAM_CentralBudgetRevisionDetails>();
            Mapper.CreateMap<FAM_CentralBudgetRevisionDetails, CentralBudgetRevision>();

            //Special Account Assignment
            Mapper.CreateMap<SpecialAccountAssignmentModel, FAM_PurposeAccountHeadMapping>();
            Mapper.CreateMap<FAM_PurposeAccountHeadMapping, SpecialAccountAssignmentModel>();

            //BSPL/Report Account Head Mapping
            Mapper.CreateMap<BSPLReportHeadMappingModel, FAM_BSPLReportHeadMapping>();
            Mapper.CreateMap<FAM_BSPLReportHeadMapping, BSPLReportHeadMappingModel>();

            Mapper.CreateMap<BSPLReportAccountHead, FAM_BSPLReportHeadMappingDetails>();
            Mapper.CreateMap<FAM_BSPLReportHeadMappingDetails, BSPLReportAccountHead>();

            //Division/Unit Budget Revision
            Mapper.CreateMap<DivisionUnitBudgetRevisionModel, FAM_DivisionUnitBudgetRevision>();
            Mapper.CreateMap<FAM_DivisionUnitBudgetRevision, DivisionUnitBudgetRevisionModel>();

            Mapper.CreateMap<DivisionUnitBudgetRevision, FAM_DivisionUnitBudgetRevisionDetails>();
            Mapper.CreateMap<FAM_DivisionUnitBudgetRevisionDetails, DivisionUnitBudgetRevision>();
        }
    }
}