using AutoMapper;
using BEPZA_MEDICAL.DAL.INV;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.ItemType;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.IssueReturn;
using BEPZA_MEDICAL.Web.Areas.INV.ViewModel.Adjustment;
using BEPZA_MEDICAL.DAL.PRM;


namespace BEPZA_MEDICAL.Web.Utility
{
    public class INVMapper
    {
        public INVMapper()
        {
            //Supplier
            Mapper.CreateMap<SupplierInfoViewModel, INV_SupplierInfo>();
            Mapper.CreateMap<INV_SupplierInfo, SupplierInfoViewModel>();

            //INV_ItemType
            Mapper.CreateMap<ItemTypeViewModel, INV_ItemType>();
            Mapper.CreateMap<INV_ItemType, ItemTypeViewModel>();

            //Common configType
            Mapper.CreateMap<CommonConfigTypeViewModel, BEPZA_MEDICAL.DAL.INV.CommonConfigType>();
            Mapper.CreateMap<BEPZA_MEDICAL.DAL.INV.CommonConfigType, CommonConfigTypeViewModel>();

            //Common config
            Mapper.CreateMap<CommonConfigViewModel, CommonConfigGetResult>();
            Mapper.CreateMap<CommonConfigGetResult, CommonConfigViewModel>();

            //INV_ItemInfo
            Mapper.CreateMap<ItemInfoViewModel, INV_ItemInfo>();
            Mapper.CreateMap<INV_ItemInfo, ItemInfoViewModel>();

            //INV_PurchaseInfo
            Mapper.CreateMap<ItemPurchaseViewModel, INV_PurchaseInfo>();
            Mapper.CreateMap<INV_PurchaseInfo, ItemPurchaseViewModel>();

            //INV_PurchaseItem
            Mapper.CreateMap<ItemPurchaseDetailViewModel, INV_PurchaseItem>();
            Mapper.CreateMap<INV_PurchaseItem, ItemPurchaseDetailViewModel>();

            //INV_PerodicAssetDuration
            Mapper.CreateMap<PeriodicAssetDurationViewModel, INV_PeriodicAssetDuration>();
            Mapper.CreateMap<INV_PeriodicAssetDuration, PeriodicAssetDurationViewModel>();

            //INV_AssetQuotaInformation
            Mapper.CreateMap<AssetQuotaInfoViewModel, INV_AssetQuotaInfo>();
            Mapper.CreateMap<INV_AssetQuotaInfo, AssetQuotaInfoViewModel>();

            //INV_TransferInInformation
            Mapper.CreateMap<TransferInViewModel, INV_TransferInInfo>();
            Mapper.CreateMap<INV_TransferInInfo, TransferInViewModel>();

            //INV_TransferInItem
            Mapper.CreateMap<TransferInDetailViewModel, INV_TransferInItem>();
            Mapper.CreateMap<INV_TransferInItem, TransferInDetailViewModel>();

            //INV_IssueInfo
            Mapper.CreateMap<ItemIssueViewModel, INV_IssueInfo>();
            Mapper.CreateMap<INV_IssueInfo, ItemIssueViewModel>();

            //INV_IssueItem
            Mapper.CreateMap<ItemIssueDetailViewModel, INV_IssueItem>();
            Mapper.CreateMap<INV_IssueItem, ItemIssueDetailViewModel>();

            //INV_TransferOutInformation
            Mapper.CreateMap<TransferOutViewModel, INV_TransferOutInfo>();
            Mapper.CreateMap<INV_TransferOutInfo, TransferOutViewModel>();

            //INV_TransferOutItem
            Mapper.CreateMap<TransferOutDetailViewModel, INV_TransferOutItem>();
            Mapper.CreateMap<INV_TransferOutItem, TransferOutDetailViewModel>();

            //INV_IssueReturnInfo
            Mapper.CreateMap<IssueReturnViewModel, INV_IssueReturnInfo>();
            Mapper.CreateMap<INV_IssueReturnInfo, IssueReturnViewModel>();

            //INV_IssueReturnItem
            Mapper.CreateMap<IssueReturnDetailViewModel, INV_IssueReturnItem>();
            Mapper.CreateMap<INV_IssueReturnItem, IssueReturnDetailViewModel>();

            //INV_AdjustmentInfo
            Mapper.CreateMap<AdjustmentViewModel, INV_AdjustmentInfo>();
            Mapper.CreateMap<INV_AdjustmentInfo, AdjustmentViewModel>();

            //INV_AdjustmentItem
            Mapper.CreateMap<AdjustmentDetailViewModel, INV_AdjustmentItem>();
            Mapper.CreateMap<INV_AdjustmentItem, AdjustmentDetailViewModel>();

            //INV_OfflineRequisitionInfo
            Mapper.CreateMap<RequisitionInfoViewModel, INV_RequisitionInfo>();
            Mapper.CreateMap<INV_RequisitionInfo, RequisitionInfoViewModel>();

            //INV_OfflineRequisitionItem
            Mapper.CreateMap<RequisitionDetailViewModel, INV_RequisitionItem>();
            Mapper.CreateMap<INV_RequisitionItem, RequisitionDetailViewModel>();

            //INV_DelegationApprovalInfo
            Mapper.CreateMap<DelegationApprovalInfoViewModel, INV_DelegationApprovalInfo>();
            Mapper.CreateMap<INV_DelegationApprovalInfo, DelegationApprovalInfoViewModel>();

            //INV_ScrapInfo
            Mapper.CreateMap<ScrapViewModel, INV_ScrapInfo>();
            Mapper.CreateMap<INV_ScrapInfo, ScrapViewModel>();

            //INV_ScrapItem
            Mapper.CreateMap<ScrapDetailViewModel, INV_ScrapItem>();
            Mapper.CreateMap<INV_ScrapItem, ScrapDetailViewModel>();

        }

    }
}