using AutoMapper;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.IssueReturn;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.Adjustment;
using System.Collections.Generic;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.ItemType;
using BEPZA_MEDICAL.DAL.PRM;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class INVMappingExtension
    {
        //Supplier Info Mapping Extension
        public static SupplierInfoViewModel ToModel(this INV_SupplierInfo entity)
        {
            return Mapper.Map<INV_SupplierInfo, SupplierInfoViewModel>(entity);
        }
        public static INV_SupplierInfo ToEntity(this SupplierInfoViewModel model)
        {
            return Mapper.Map<SupplierInfoViewModel, INV_SupplierInfo>(model);
        }

        // INV_ItemType mapping extension
        public static ItemTypeViewModel ToModel(this INV_ItemType entity)
        {
            return Mapper.Map<INV_ItemType, ItemTypeViewModel>(entity);
        }

        public static INV_ItemType ToEntity(this ItemTypeViewModel model)
        {
            return Mapper.Map<ItemTypeViewModel, INV_ItemType>(model);
        }

        //Common Config Type
        public static List<CommonConfigTypeViewModel> ToModelList(this List<BEPZA_MEDICAL.DAL.INV.CommonConfigType> objlist)
        {
            List<CommonConfigTypeViewModel> list = new List<CommonConfigTypeViewModel>();
            foreach (var item in objlist)
            {
                list.Add((Mapper.Map<BEPZA_MEDICAL.DAL.INV.CommonConfigType, CommonConfigTypeViewModel>(item)));
            }
            return list;
        }
        public static List<BEPZA_MEDICAL.DAL.INV.CommonConfigType> ToEntityList(this List<CommonConfigTypeViewModel> modellist)
        {
            List<BEPZA_MEDICAL.DAL.INV.CommonConfigType> list = new List<BEPZA_MEDICAL.DAL.INV.CommonConfigType>();
            foreach (var item in modellist)
            {
                list.Add((Mapper.Map<CommonConfigTypeViewModel, BEPZA_MEDICAL.DAL.INV.CommonConfigType>(item)));
            }
            return list;
        }
        //Common Config
        public static CommonConfigViewModel ToModel(this CommonConfigGetResult obj)
        {
            return Mapper.Map<CommonConfigGetResult, CommonConfigViewModel>(obj);
        }
        public static CommonConfigGetResult ToEntity(this CommonConfigViewModel model)
        {
            return Mapper.Map<CommonConfigViewModel, CommonConfigGetResult>(model);
        }


        // INV_ItemInfo mapping extension
        public static ItemInfoViewModel ToModel(this INV_ItemInfo entity)
        {
            return Mapper.Map<INV_ItemInfo, ItemInfoViewModel>(entity);
        }

        public static INV_ItemInfo ToEntity(this ItemInfoViewModel model)
        {
            return Mapper.Map<ItemInfoViewModel, INV_ItemInfo>(model);
        }

        // INV_PurchaseInfo mapping extension
        public static ItemPurchaseViewModel ToModel(this INV_PurchaseInfo entity)
        {
            return Mapper.Map<INV_PurchaseInfo, ItemPurchaseViewModel>(entity);
        }

        public static INV_PurchaseInfo ToEntity(this ItemPurchaseViewModel model)
        {
            return Mapper.Map<ItemPurchaseViewModel, INV_PurchaseInfo>(model);
        }

        // INV_PurchaseItem mapping extension
        public static ItemPurchaseDetailViewModel ToModel(this INV_PurchaseItem entity)
        {
            return Mapper.Map<INV_PurchaseItem, ItemPurchaseDetailViewModel>(entity);
        }

        public static INV_PurchaseItem ToEntity(this ItemPurchaseDetailViewModel model)
        {
            return Mapper.Map<ItemPurchaseDetailViewModel, INV_PurchaseItem>(model);
        }

        //INV_PeriodicAssetDuration mapping Extension
        public static PeriodicAssetDurationViewModel ToModel(this INV_PeriodicAssetDuration entity)
        {
            return Mapper.Map<INV_PeriodicAssetDuration, PeriodicAssetDurationViewModel>(entity);
        }
        public static INV_PeriodicAssetDuration ToEntity(this PeriodicAssetDurationViewModel model)
        {
            return Mapper.Map<PeriodicAssetDurationViewModel, INV_PeriodicAssetDuration>(model);
        }

        //INV_AssetQuotaInformation mapping Extension
        public static AssetQuotaInfoViewModel ToModel(this INV_AssetQuotaInfo entity)
        {
            return Mapper.Map<INV_AssetQuotaInfo, AssetQuotaInfoViewModel>(entity);
        }
        public static INV_AssetQuotaInfo ToEntity(this AssetQuotaInfoViewModel model)
        {
            return Mapper.Map<AssetQuotaInfoViewModel, INV_AssetQuotaInfo>(model);
        }

        //INV_TransferIn Information mapping Extension
        public static TransferInViewModel ToModel(this INV_TransferInInfo entity)
        {
            return Mapper.Map<INV_TransferInInfo, TransferInViewModel>(entity);
        }
        public static INV_TransferInInfo ToEntity(this TransferInViewModel model)
        {
            return Mapper.Map<TransferInViewModel, INV_TransferInInfo>(model);
        }

        //INV_TransferInItem Information mapping Extension
        public static TransferInDetailViewModel ToModel(this INV_TransferInItem entity)
        {
            return Mapper.Map<INV_TransferInItem, TransferInDetailViewModel>(entity);
        }
        public static INV_TransferInItem ToEntity(this TransferInDetailViewModel model)
        {
            return Mapper.Map<TransferInDetailViewModel, INV_TransferInItem>(model);
        }

        // INV_IssueInfo mapping extension
        public static ItemIssueViewModel ToModel(this INV_IssueInfo entity)
        {
            return Mapper.Map<INV_IssueInfo, ItemIssueViewModel>(entity);
        }

        public static INV_IssueInfo ToEntity(this ItemIssueViewModel model)
        {
            return Mapper.Map<ItemIssueViewModel, INV_IssueInfo>(model);
        }

        // INV_IssueItem mapping extension
        public static ItemIssueDetailViewModel ToModel(this INV_IssueItem entity)
        {
            return Mapper.Map<INV_IssueItem, ItemIssueDetailViewModel>(entity);
        }

        public static INV_IssueItem ToEntity(this ItemIssueDetailViewModel model)
        {
            return Mapper.Map<ItemIssueDetailViewModel, INV_IssueItem>(model);
        }

        //INV_TransferOut Information mapping Extension
        public static TransferOutViewModel ToModel(this INV_TransferOutInfo entity)
        {
            return Mapper.Map<INV_TransferOutInfo, TransferOutViewModel>(entity);
        }
        public static INV_TransferOutInfo ToEntity(this TransferOutViewModel model)
        {
            return Mapper.Map<TransferOutViewModel, INV_TransferOutInfo>(model);
        }

        //INV_TransferOutItem Information mapping Extension
        public static TransferOutDetailViewModel ToModel(this INV_TransferOutItem entity)
        {
            return Mapper.Map<INV_TransferOutItem, TransferOutDetailViewModel>(entity);
        }
        public static INV_TransferOutItem ToEntity(this TransferOutDetailViewModel model)
        {
            return Mapper.Map<TransferOutDetailViewModel, INV_TransferOutItem>(model);
        }

        // INV_IssueReturnInfo mapping extension
        public static IssueReturnViewModel ToModel(this INV_IssueReturnInfo entity)
        {
            return Mapper.Map<INV_IssueReturnInfo, IssueReturnViewModel>(entity);
        }

        public static INV_IssueReturnInfo ToEntity(this IssueReturnViewModel model)
        {
            return Mapper.Map<IssueReturnViewModel, INV_IssueReturnInfo>(model);
        }

        // INV_IssueReturnItem mapping extension
        public static IssueReturnDetailViewModel ToModel(this INV_IssueReturnItem entity)
        {
            return Mapper.Map<INV_IssueReturnItem, IssueReturnDetailViewModel>(entity);
        }

        public static INV_IssueReturnItem ToEntity(this IssueReturnDetailViewModel model)
        {
            return Mapper.Map<IssueReturnDetailViewModel, INV_IssueReturnItem>(model);
        }

        // INV_AdjustmentInfo mapping extension
        public static AdjustmentViewModel ToModel(this INV_AdjustmentInfo entity)
        {
            return Mapper.Map<INV_AdjustmentInfo, AdjustmentViewModel>(entity);
        }

        public static INV_AdjustmentInfo ToEntity(this AdjustmentViewModel model)
        {
            return Mapper.Map<AdjustmentViewModel, INV_AdjustmentInfo>(model);
        }

        // INV_AdjustmentItem mapping extension
        public static AdjustmentDetailViewModel ToModel(this INV_AdjustmentItem entity)
        {
            return Mapper.Map<INV_AdjustmentItem, AdjustmentDetailViewModel>(entity);
        }

        public static INV_AdjustmentItem ToEntity(this AdjustmentDetailViewModel model)
        {
            return Mapper.Map<AdjustmentDetailViewModel, INV_AdjustmentItem>(model);
        }

        //INV_RequisitionInfo mapping Extension
        public static RequisitionInfoViewModel ToModel(this INV_RequisitionInfo entity)
        {
            return Mapper.Map<INV_RequisitionInfo, RequisitionInfoViewModel>(entity);
        }
        public static INV_RequisitionInfo ToEntity(this RequisitionInfoViewModel model)
        {
            return Mapper.Map<RequisitionInfoViewModel, INV_RequisitionInfo>(model);
        }

        //INV_RequisitionItem mapping Extension
        public static RequisitionDetailViewModel ToModel(this INV_RequisitionItem entity)
        {
            return Mapper.Map<INV_RequisitionItem, RequisitionDetailViewModel>(entity);
        }
        public static INV_RequisitionItem ToEntity(this RequisitionDetailViewModel model)
        {
            return Mapper.Map<RequisitionDetailViewModel, INV_RequisitionItem>(model);
        }


        //INV_DelegationApprovalInfo mapping Extension
        public static DelegationApprovalInfoViewModel ToModel(this INV_DelegationApprovalInfo entity)
        {
            return Mapper.Map<INV_DelegationApprovalInfo, DelegationApprovalInfoViewModel>(entity);
        }
        public static INV_DelegationApprovalInfo ToEntity(this DelegationApprovalInfoViewModel model)
        {
            return Mapper.Map<DelegationApprovalInfoViewModel, INV_DelegationApprovalInfo>(model);
        }

        //INV_ScrapInfo mapping Extension
        public static ScrapViewModel ToModel(this INV_ScrapInfo entity)
        {
            return Mapper.Map<INV_ScrapInfo, ScrapViewModel>(entity);
        }
        public static INV_ScrapInfo ToEntity(this ScrapViewModel model)
        {
            return Mapper.Map<ScrapViewModel, INV_ScrapInfo>(model);
        }

        //INV_ScrapItem mapping Extension
        public static ScrapDetailViewModel ToModel(this INV_ScrapItem entity)
        {
            return Mapper.Map<INV_ScrapItem, ScrapDetailViewModel>(entity);
        }
        public static INV_ScrapItem ToEntity(this ScrapDetailViewModel model)
        {
            return Mapper.Map<ScrapDetailViewModel, INV_ScrapItem>(model);
        }
    }
}