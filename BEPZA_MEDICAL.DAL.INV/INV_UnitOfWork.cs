using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BEPZA_MEDICAL.DAL.INV
{
    public class INV_UnitOfWork
    {
        #region Fields
        INV_ExecuteFunctions _functionRepository;
        INV_GenericRepository<INV_ItemType> _itemTypeRepository;
        INV_GenericRepository<vwINVBudgetHead> _budgetHeadViewRepository;
        INV_GenericRepository<INV_SupplierInfo> _supplierRepository;
        INV_GenericRepository<CommonConfigType> _configTypeRepository;
        INV_GenericRepository<INV_ItemStatus> _itemStatusRepository;
        INV_GenericRepository<INV_Category> _categoryRepository;
        INV_GenericRepository<INV_Model> _modelRepository;
        INV_GenericRepository<INV_Color> _colorRepository;
        INV_GenericRepository<INV_Manufacturer> _manufacturerRepository;
        INV_GenericRepository<INV_Unit> _unitRepository;
        INV_GenericRepository<INV_PurchaseType> _purchaseTypeRepository;
        INV_GenericRepository<INV_ItemInfo> _itemInfoRepository;
        INV_GenericRepository<INV_PurchaseInfo> _purchaseInfoRepository;
        INV_GenericRepository<INV_PurchaseItem> _purchaseItemRepository;
        INV_GenericRepository<INV_PeriodicAssetDuration> _periodicAssetDurationRepository;
        INV_GenericRepository<INV_AssetQuotaInfo> _assetQuotaInfoRepository;
        INV_GenericRepository<INV_TransferInInfo> _transferInInfoRepository;
        INV_GenericRepository<INV_TransferInItem> _transferInItemRepository; 
        INV_GenericRepository<INV_IssueInfo> _issueInfoRepository;
        INV_GenericRepository<INV_IssueItem> _issueItemRepository;
        INV_GenericRepository<INV_TransferOutInfo> _transferOutInfoRepository;
        INV_GenericRepository<INV_TransferOutItem> _transferOutItemRepository;
        INV_GenericRepository<INV_IssueReturnInfo> _issueReturnInfoRepository;
        INV_GenericRepository<INV_IssueReturnItem> _issueReturnItemRepository;
        INV_GenericRepository<INV_AdjustmentInfo> _adjustmentInfoRepository;
        INV_GenericRepository<INV_AdjustmentItem> _adjustmentItemRepository;
        INV_GenericRepository<INV_RequisitionInfo> _requisitionInfoRepository;
        INV_GenericRepository<INV_RequisitionItem> _requisitionItemRepository;
        INV_GenericRepository<INV_DelegationApprovalInfo> _delegationApprovalInfoRepository;
        INV_GenericRepository<INV_ScrapInfo> _scrapInfoRepository;
        INV_GenericRepository<INV_ScrapItem> _scrapItemRepository;


        #endregion

        #region Ctor
        public INV_UnitOfWork(
            INV_ExecuteFunctions functionRepository,
            INV_GenericRepository<INV_ItemType> itemTypeRepository,
            INV_GenericRepository<vwINVBudgetHead> budgetHeadViewRepository,
            INV_GenericRepository<INV_SupplierInfo> supplierRepository,
            INV_GenericRepository<CommonConfigType> configTypeRepository,
            INV_GenericRepository<INV_ItemStatus> itemStatusRepository,
            INV_GenericRepository<INV_Category> categoryRepository,
            INV_GenericRepository<INV_Model> modelRepository,
            INV_GenericRepository<INV_Color> colorRepository,
            INV_GenericRepository<INV_Manufacturer> manufacturerRepository,
            INV_GenericRepository<INV_Unit> unitRepository,
            INV_GenericRepository<INV_PurchaseType> purchaseTypeRepository,
            INV_GenericRepository<INV_ItemInfo> itemInfoRepository,
            INV_GenericRepository<INV_PurchaseInfo> purchaseInfoRepository,
            INV_GenericRepository<INV_PurchaseItem> purchaseItemRepository,
            INV_GenericRepository<INV_PeriodicAssetDuration> periodicAssetDurationRepository,
            INV_GenericRepository<INV_AssetQuotaInfo> assetQuotaRepository,
            INV_GenericRepository<INV_TransferInInfo> transferInInfoRepository,
            INV_GenericRepository<INV_TransferInItem> transferInItemRepository,
            INV_GenericRepository<INV_IssueInfo> issueInfoRepository,
            INV_GenericRepository<INV_IssueItem> issueItemRepository,
            INV_GenericRepository<INV_TransferOutInfo> transferOutInfoRepository,
            INV_GenericRepository<INV_TransferOutItem> transferOutItemRepository,
            INV_GenericRepository<INV_IssueReturnInfo> issueReturnInfoRepository,
            INV_GenericRepository<INV_IssueReturnItem> issueReturnItemRepository,
            INV_GenericRepository<INV_AdjustmentInfo> adjustmentInfoRepository,
            INV_GenericRepository<INV_AdjustmentItem> adjustmentItemRepository,
            INV_GenericRepository<INV_RequisitionInfo> requisitionInfoRepository,
            INV_GenericRepository<INV_RequisitionItem> requisitionItemRepository,
            INV_GenericRepository<INV_DelegationApprovalInfo> delegationApprovalInfoRepository,
            INV_GenericRepository<INV_ScrapInfo> scrapInfoRepository,
            INV_GenericRepository<INV_ScrapItem> scrapItemRepository

        )
        {
            this._functionRepository = functionRepository;
            this._itemTypeRepository = itemTypeRepository;
            this._budgetHeadViewRepository = budgetHeadViewRepository;
            this._supplierRepository = supplierRepository;
            this._configTypeRepository = configTypeRepository;
            this._itemStatusRepository = itemStatusRepository;
            this._categoryRepository = categoryRepository;
            this._modelRepository = modelRepository;
            this._colorRepository = colorRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._unitRepository = unitRepository;
            this._purchaseTypeRepository = purchaseTypeRepository;
            this._itemInfoRepository = itemInfoRepository;
            this._purchaseInfoRepository = purchaseInfoRepository;
            this._purchaseItemRepository = purchaseItemRepository;
            this._periodicAssetDurationRepository = periodicAssetDurationRepository;
            this._assetQuotaInfoRepository = assetQuotaRepository;
            this._transferInInfoRepository = transferInInfoRepository;
            this._transferInItemRepository = transferInItemRepository;
            this._issueInfoRepository = issueInfoRepository;
            this._issueItemRepository = issueItemRepository;
            this._transferOutInfoRepository = transferOutInfoRepository;
            this._transferOutItemRepository = transferOutItemRepository;
            this._issueReturnInfoRepository = issueReturnInfoRepository;
            this._issueReturnItemRepository = issueReturnItemRepository;
            this._adjustmentInfoRepository = adjustmentInfoRepository;
            this._adjustmentItemRepository = adjustmentItemRepository;
            this._requisitionInfoRepository = requisitionInfoRepository;
            this._requisitionItemRepository = requisitionItemRepository;
            this._delegationApprovalInfoRepository = delegationApprovalInfoRepository;
            this._scrapInfoRepository = scrapInfoRepository;
            this._scrapItemRepository = scrapItemRepository;
        }
        #endregion

        #region Properties
        public INV_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        public INV_GenericRepository<INV_ItemType> ItemTypeRepository
        {
            get
            {
                return _itemTypeRepository;
            }
        }

        public INV_GenericRepository<vwINVBudgetHead> BudgetHeadViewRepository
        {
            get
            {
                return _budgetHeadViewRepository;
            }
        }

        public INV_GenericRepository<INV_SupplierInfo> SupplierRepository
        {
            get
            {
                return _supplierRepository;
            }
        }
        public INV_GenericRepository<CommonConfigType> ConfigTypeRepository
        {
            get
            {
                return _configTypeRepository;
            }
        }
        public INV_GenericRepository<INV_ItemStatus> ItemStatusRepository
        {
            get
            {
                return _itemStatusRepository;
            }
        }
        public INV_GenericRepository<INV_Category> CategoryRepository
        {
            get
            {
                return _categoryRepository;
            }
        }
        public INV_GenericRepository<INV_Model> ModelRepository
        {
            get
            {
                return _modelRepository;
            }
        }
        public INV_GenericRepository<INV_Color> ColorRepository
        {
            get
            {
                return _colorRepository;
            }
        }
        public INV_GenericRepository<INV_Manufacturer> ManufacturerRepository
        {
            get
            {
                return _manufacturerRepository;
            }
        }
        public INV_GenericRepository<INV_Unit> UnitRepository
        {
            get
            {
                return _unitRepository;
            }
        }
        public INV_GenericRepository<INV_PurchaseType> PurchaseTypeRepository
        {
            get
            {
                return _purchaseTypeRepository;
            }
        }

        public INV_GenericRepository<INV_ItemInfo> ItemInfoRepository
        {
            get
            {
                return _itemInfoRepository;
            }
        }

        public INV_GenericRepository<INV_PurchaseInfo> PurchaseInfoRepository
        {
            get
            {
                return _purchaseInfoRepository;
            }
        }

        public INV_GenericRepository<INV_PurchaseItem> PurchaseItemRepository
        {
            get
            {
                return _purchaseItemRepository;
            }
        }

        public INV_GenericRepository<INV_PeriodicAssetDuration> PeriodicAssetDurationRepository
        {
            get
            {
                return _periodicAssetDurationRepository;
            }
        }

        public INV_GenericRepository<INV_AssetQuotaInfo> AssetQuotaInfoRepository
        {
            get
            {
                return _assetQuotaInfoRepository;
            }
        }

        public INV_GenericRepository<INV_TransferInInfo> TransferInInfoRepository
        {
            get
            {
                return _transferInInfoRepository;
            }
        }
        public INV_GenericRepository<INV_TransferInItem> TransferInItemRepository
        {
            get
            {
                return _transferInItemRepository;
            }
        }

        public INV_GenericRepository<INV_IssueInfo> IssueInfoRepository
        {
            get
            {
                return _issueInfoRepository;
            }
        }

        public INV_GenericRepository<INV_IssueItem> IssueItemRepository
        {
            get
            {
                return _issueItemRepository;
            }
        }

        public INV_GenericRepository<INV_TransferOutInfo> TransferOutInfoRepository
        {
            get
            {
                return _transferOutInfoRepository;
            }
        }
        public INV_GenericRepository<INV_TransferOutItem> TransferOutItemRepository
        {
            get
            {
                return _transferOutItemRepository;
            }
        }

        public INV_GenericRepository<INV_IssueReturnInfo> IssueReturnInfoRepository
        {
            get
            {
                return _issueReturnInfoRepository;
            }
        }

        public INV_GenericRepository<INV_IssueReturnItem> IssueReturnItemRepository
        {
            get
            {
                return _issueReturnItemRepository;
            }
        }

        public INV_GenericRepository<INV_AdjustmentInfo> AdjustmentInfoRepository
        {
            get
            {
                return _adjustmentInfoRepository;
            }
        }

        public INV_GenericRepository<INV_AdjustmentItem> AdjustmentItemRepository
        {
            get
            {
                return _adjustmentItemRepository;
            }
        }

        public INV_GenericRepository<INV_RequisitionInfo> RequisitionInfoRepository
        {
            get
            {
                return _requisitionInfoRepository;
            }
        }

        public INV_GenericRepository<INV_RequisitionItem> RequisitionItemRepository
        {
            get
            {
                return _requisitionItemRepository;
            }
        }

        public INV_GenericRepository<INV_DelegationApprovalInfo> DelegationApprovalInfoRepository
        {
            get
            {
                return _delegationApprovalInfoRepository;
            }
        }

        public INV_GenericRepository<INV_ScrapInfo> ScrapInfoRepository
        {
            get
            {
                return _scrapInfoRepository;
            }
        }

        public INV_GenericRepository<INV_ScrapItem> ScrapItemRepository
        {
            get
            {
                return _scrapItemRepository;
            }
        }

        #endregion

    }
}
