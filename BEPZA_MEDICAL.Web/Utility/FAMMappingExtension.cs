using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using BEPZA_MEDICAL.Web.Areas.FAM.Models.Configuration;
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
    public static class FAMMappingExtension
    {
        //Chart of Account
        public static ChartOfAccountModel ToModel(this FAM_ChartOfAccount entity)
        {
            return Mapper.Map<FAM_ChartOfAccount, ChartOfAccountModel>(entity);
        }
        public static FAM_ChartOfAccount ToEntity(this ChartOfAccountModel model)
        {
            return Mapper.Map<ChartOfAccountModel, FAM_ChartOfAccount>(model);
        }

        //Vendor Information
        public static VendorInformationModel ToModel(this FAM_VendorInformation entity)
        {
            return Mapper.Map<FAM_VendorInformation, VendorInformationModel>(entity);
        }
        public static FAM_VendorInformation ToEntity(this VendorInformationModel model)
        {
            return Mapper.Map<VendorInformationModel, FAM_VendorInformation>(model);
        }

        //Bank Info
        public static BankInformationModel ToModel(this FAM_BankBranchMap entity)
        {
            return Mapper.Map<FAM_BankBranchMap, BankInformationModel>(entity);
        }
        public static FAM_BankBranchMap ToEntity(this BankInformationModel model)
        {
            return Mapper.Map<BankInformationModel, FAM_BankBranchMap>(model);
        }

        public static BankInfoChild ToModel(this FAM_BankBranchAccountNo entity)
        {
            return Mapper.Map<FAM_BankBranchAccountNo, BankInfoChild>(entity);
        }
        public static FAM_BankBranchAccountNo ToEntity(this BankInfoChild model)
        {
            return Mapper.Map<BankInfoChild, FAM_BankBranchAccountNo>(model);
        }

        //Financial Year
        public static FinancialYearInformationModel ToModel(this FAM_FinancialYearInformation entity)
        {
            return Mapper.Map<FAM_FinancialYearInformation, FinancialYearInformationModel>(entity);
        }
        public static FAM_FinancialYearInformation ToEntity(this FinancialYearInformationModel model)
        {
            return Mapper.Map<FinancialYearInformationModel, FAM_FinancialYearInformation>(model);
        }

        //Division/Unit Wise Budget Head

        public static DivisionUnitWiseBudgetHeadModel ToModel(this FAM_DivisionUnitBudgetHead entity)
        {
            return Mapper.Map<FAM_DivisionUnitBudgetHead, DivisionUnitWiseBudgetHeadModel>(entity);
        }
        public static FAM_DivisionUnitBudgetHead ToEntity(this DivisionUnitWiseBudgetHeadModel model)
        {
            return Mapper.Map<DivisionUnitWiseBudgetHeadModel, FAM_DivisionUnitBudgetHead>(model);
        }

        public static BudgetHeadAllocation ToChildModel(this FAM_DivisionUnitBudgetHead entity)
        {
            return Mapper.Map<FAM_DivisionUnitBudgetHead, BudgetHeadAllocation>(entity);
        }
        public static FAM_DivisionUnitBudgetHead ToEntity(this BudgetHeadAllocation model)
        {
            return Mapper.Map<BudgetHeadAllocation, FAM_DivisionUnitBudgetHead>(model);
        }


        //Cheque Info
        public static ChequeInfoModel ToModel(this FAM_ChequeBookInfo entity)
        {
            return Mapper.Map<FAM_ChequeBookInfo, ChequeInfoModel>(entity);
        }
        public static FAM_ChequeBookInfo ToEntity(this ChequeInfoModel model)
        {
            return Mapper.Map<ChequeInfoModel, FAM_ChequeBookInfo>(model);
        }

        public static ChequeInfoDetails ToModel(this FAM_ChequeInfo entity)
        {
            return Mapper.Map<FAM_ChequeInfo, ChequeInfoDetails>(entity);
        }
        public static FAM_ChequeInfo ToEntity(this ChequeInfoDetails model)
        {
            return Mapper.Map<ChequeInfoDetails, FAM_ChequeInfo>(model);
        }

        //Division Unit Budget Allocation
        public static DivisionUnitBudgetAllocationModel ToModel(this FAM_DivisionUnitBudgetAllocation entity)
        {
            return Mapper.Map<FAM_DivisionUnitBudgetAllocation, DivisionUnitBudgetAllocationModel>(entity);
        }
        public static FAM_DivisionUnitBudgetAllocation ToEntity(this DivisionUnitBudgetAllocationModel model)
        {
            return Mapper.Map<DivisionUnitBudgetAllocationModel, FAM_DivisionUnitBudgetAllocation>(model);
        }

        public static BudgetAllocation ToModelChild(this FAM_DivisionUnitBudgetAllocationDetails entity)
        {
            return Mapper.Map<FAM_DivisionUnitBudgetAllocationDetails, BudgetAllocation>(entity);
        }
        public static FAM_DivisionUnitBudgetAllocationDetails ToEntityChild(this BudgetAllocation model)
        {
            return Mapper.Map<BudgetAllocation, FAM_DivisionUnitBudgetAllocationDetails>(model);
        }

        //Approval Path Info
        public static ApprovalPathInfoModel ToModel(this FAM_ApprovalPath entity)
        {
            return Mapper.Map<FAM_ApprovalPath, ApprovalPathInfoModel>(entity);
        }
        public static FAM_ApprovalPath ToEntity(this ApprovalPathInfoModel model)
        {
            return Mapper.Map<ApprovalPathInfoModel, FAM_ApprovalPath>(model);
        }

        public static ApprovalPathDetails ToModel(this FAM_ApprovalPathNodeInfo entity)
        {
            return Mapper.Map<FAM_ApprovalPathNodeInfo, ApprovalPathDetails>(entity);
        }
        public static FAM_ApprovalPathNodeInfo ToEntity(this ApprovalPathDetails model)
        {
            return Mapper.Map<ApprovalPathDetails, FAM_ApprovalPathNodeInfo>(model);
        }

        //Central Budget Information
        public static CentralBudgetInformationModel ToModel(this FAM_CentralBudgetAllocation entity)
        {
            return Mapper.Map<FAM_CentralBudgetAllocation, CentralBudgetInformationModel>(entity);
        }
        public static FAM_CentralBudgetAllocation ToEntity(this CentralBudgetInformationModel model)
        {
            return Mapper.Map<CentralBudgetInformationModel, FAM_CentralBudgetAllocation>(model);
        }

        public static CentralBudget ToModelChild(this FAM_CentralBudgetAllocationDetails entity)
        {
            return Mapper.Map<FAM_CentralBudgetAllocationDetails, CentralBudget>(entity);
        }
        public static FAM_CentralBudgetAllocationDetails ToEntityChild(this CentralBudget model)
        {
            return Mapper.Map<CentralBudget, FAM_CentralBudgetAllocationDetails>(model);
        }

        //Voucher info
        public static VoucherInfoModel ToModel(this FAM_VoucherInfo entity)
        {
            return Mapper.Map<FAM_VoucherInfo, VoucherInfoModel>(entity);
        }
        public static FAM_VoucherInfo ToEntity(this VoucherInfoModel model)
        {
            return Mapper.Map<VoucherInfoModel, FAM_VoucherInfo>(model);
        }

        public static VoucherInfoDetails ToModel(this FAM_VoucherInfoDetails entity)
        {
            return Mapper.Map<FAM_VoucherInfoDetails, VoucherInfoDetails>(entity);
        }
        public static FAM_VoucherInfoDetails ToEntity(this VoucherInfoDetails model)
        {
            return Mapper.Map<VoucherInfoDetails, FAM_VoucherInfoDetails>(model);
        }

        //Revision of Central Budget 
        public static RevisionOfCentralBudgetModel ToModel(this FAM_CentralBudgetRevision entity)
        {
            return Mapper.Map<FAM_CentralBudgetRevision, RevisionOfCentralBudgetModel>(entity);
        }
        public static FAM_CentralBudgetRevision ToEntity(this RevisionOfCentralBudgetModel model)
        {
            return Mapper.Map<RevisionOfCentralBudgetModel, FAM_CentralBudgetRevision>(model);
        }

        public static CentralBudgetRevision ToModelChild(this FAM_CentralBudgetRevisionDetails entity)
        {
            return Mapper.Map<FAM_CentralBudgetRevisionDetails, CentralBudgetRevision>(entity);
        }
        public static FAM_CentralBudgetRevisionDetails ToEntityChild(this CentralBudgetRevision model)
        {
            return Mapper.Map<CentralBudgetRevision, FAM_CentralBudgetRevisionDetails>(model);
        }

        //Special Account Assignment
        public static SpecialAccountAssignmentModel ToModel(this FAM_PurposeAccountHeadMapping entity)
        {
            return Mapper.Map<FAM_PurposeAccountHeadMapping, SpecialAccountAssignmentModel>(entity);
        }
        public static FAM_PurposeAccountHeadMapping ToEntity(this SpecialAccountAssignmentModel model)
        {
            return Mapper.Map<SpecialAccountAssignmentModel, FAM_PurposeAccountHeadMapping>(model);
        }

        //BSPL/Report Head Mapping 
        public static BSPLReportHeadMappingModel ToModel(this FAM_BSPLReportHeadMapping entity)
        {
            return Mapper.Map<FAM_BSPLReportHeadMapping, BSPLReportHeadMappingModel>(entity);
        }
        public static FAM_BSPLReportHeadMapping ToEntity(this BSPLReportHeadMappingModel model)
        {
            return Mapper.Map<BSPLReportHeadMappingModel, FAM_BSPLReportHeadMapping>(model);
        }

        public static BSPLReportAccountHead ToModelChild(this FAM_BSPLReportHeadMappingDetails entity)
        {
            return Mapper.Map<FAM_BSPLReportHeadMappingDetails, BSPLReportAccountHead>(entity);
        }
        public static FAM_BSPLReportHeadMappingDetails ToEntityChild(this BSPLReportAccountHead model)
        {
            return Mapper.Map<BSPLReportAccountHead, FAM_BSPLReportHeadMappingDetails>(model);
        }

        //Division/Unit Budget Revision
        public static DivisionUnitBudgetRevisionModel ToModel(this FAM_DivisionUnitBudgetRevision entity)
        {
            return Mapper.Map<FAM_DivisionUnitBudgetRevision, DivisionUnitBudgetRevisionModel>(entity);
        }
        public static FAM_DivisionUnitBudgetRevision ToEntity(this DivisionUnitBudgetRevisionModel model)
        {
            return Mapper.Map<DivisionUnitBudgetRevisionModel, FAM_DivisionUnitBudgetRevision>(model);
        }

        public static DivisionUnitBudgetRevision ToModelChild(this FAM_DivisionUnitBudgetRevisionDetails entity)
        {
            return Mapper.Map<FAM_DivisionUnitBudgetRevisionDetails, DivisionUnitBudgetRevision>(entity);
        }
        public static FAM_DivisionUnitBudgetRevisionDetails ToEntityChild(this DivisionUnitBudgetRevision model)
        {
            return Mapper.Map<DivisionUnitBudgetRevision, FAM_DivisionUnitBudgetRevisionDetails>(model);
        }
    }
}