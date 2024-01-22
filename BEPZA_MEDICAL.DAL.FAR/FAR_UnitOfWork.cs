using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.FAR
{
    public class FAR_UnitOfWork
    {
        #region Fields
        FAR_ExecuteFunctions _functionRepository;
        FAR_GenericRepository<FAR_Location> _locationRepository;
        FAR_GenericRepository<FAR_Supplier> _supplierRepository;
        FAR_GenericRepository<FAR_DepreciationMethod> _depreciationMethodRepository;
        FAR_GenericRepository<FAR_Catagory> _categroyRepository;
        FAR_GenericRepository<FAR_SubCategory> _subCategroyRepository;
        FAR_GenericRepository<FAR_AssetStatus> _assetStatusRepository;
        FAR_GenericRepository<FAR_AssetCondition> _assetConditionRepository;
        FAR_GenericRepository<FAR_MaintenanceTypeInformation> _maintenanceTypeInformationRepository;
        FAR_GenericRepository<FAR_SparePartInformation> _sparePartInformationRepository;
        FAR_GenericRepository<FAR_FixedAsset> _fixedAssetRepository;
        FAR_GenericRepository<FAR_Repair> _assetRepair;
        FAR_GenericRepository<FAR_SaleDisposal> _saleDisposal;
        FAR_GenericRepository<FAR_Depreciation> _assetDepreciation;
        FAR_GenericRepository<FAR_DepreciationDetail> _assetDepreciationDetail;
        FAR_GenericRepository<FAR_AssetTransfer> _assetTransfer;
        FAR_GenericRepository<FAR_AssetMaintenanceInformation> _assetMaintenanceInformationRepository;
        FAR_GenericRepository<FAR_AssetMaintenanceInformationDetail> _assetMaintenanceInformationDetailRepository;
        FAR_GenericRepository<FAR_AdditionalInfoofVehicle> _additionalInfoofVehicleRepository;
        FAR_GenericRepository<FAR_AdditionalInfoofVehicleDetail> _additionalInfoofVehicleDetailRepository;
        FAR_GenericRepository<FAR_AccidentInformation> _accidentInformationRepository;
        FAR_GenericRepository<acc_Accounting_Period_Information> _financialYearRepository;
        #endregion

        #region Ctor
        public FAR_UnitOfWork(
            FAR_ExecuteFunctions functionRepository,
            FAR_GenericRepository<FAR_Location> locationRepository,
            FAR_GenericRepository<FAR_Supplier> supplierRepository,
            FAR_GenericRepository<FAR_DepreciationMethod> depreciationMethodRepository,
            FAR_GenericRepository<FAR_Catagory> categroyRepository,
            FAR_GenericRepository<FAR_SubCategory> subCategroyRepository,
            FAR_GenericRepository<FAR_AssetStatus> assetStatusRepository,
            FAR_GenericRepository<FAR_AssetCondition> assetConditionRepository,
            FAR_GenericRepository<FAR_MaintenanceTypeInformation> maintenanceTypeInformationRepository,
            FAR_GenericRepository<FAR_SparePartInformation> sparePartInformationRepository,
            FAR_GenericRepository<FAR_FixedAsset> fixedAssetRepository,
            FAR_GenericRepository<FAR_Repair> assetRepair,
            FAR_GenericRepository<FAR_SaleDisposal> saleDisposal,
            FAR_GenericRepository<FAR_Depreciation> assetDepreciation,
            FAR_GenericRepository<FAR_DepreciationDetail> assetDepreciationDetail,
            FAR_GenericRepository<FAR_AssetTransfer> assetTransfer,
            FAR_GenericRepository<FAR_AssetMaintenanceInformation> assetMaintenanceInformationRepository,
            FAR_GenericRepository<FAR_AssetMaintenanceInformationDetail> assetMaintenanceInformationDetailRepository,
            FAR_GenericRepository<FAR_AdditionalInfoofVehicle> additionalInfoofVehicleRepository,
            FAR_GenericRepository<FAR_AdditionalInfoofVehicleDetail> additionalInfoofVehicleDetailRepository,
            FAR_GenericRepository<FAR_AccidentInformation> accidentInformationRepository,
            FAR_GenericRepository<acc_Accounting_Period_Information> financialYearRepository
            )
        {
            this._functionRepository = functionRepository;
            this._locationRepository = locationRepository;
            this._supplierRepository = supplierRepository;
            this._depreciationMethodRepository = depreciationMethodRepository;
            this._categroyRepository = categroyRepository;
            this._subCategroyRepository = subCategroyRepository;
            this._assetStatusRepository = assetStatusRepository;
            this._assetConditionRepository = assetConditionRepository;
            this._maintenanceTypeInformationRepository = maintenanceTypeInformationRepository;
            this._sparePartInformationRepository = sparePartInformationRepository;
            this._fixedAssetRepository = fixedAssetRepository;
            this._assetRepair = assetRepair;
            this._saleDisposal = saleDisposal;
            this._assetDepreciation = assetDepreciation;
            this._assetDepreciationDetail = assetDepreciationDetail;
            this._assetTransfer = assetTransfer;
            this._assetMaintenanceInformationRepository = assetMaintenanceInformationRepository;
            this._assetMaintenanceInformationDetailRepository = assetMaintenanceInformationDetailRepository;
            this._additionalInfoofVehicleRepository = additionalInfoofVehicleRepository;
            this._additionalInfoofVehicleDetailRepository = additionalInfoofVehicleDetailRepository;
            this._accidentInformationRepository = accidentInformationRepository;
            this._financialYearRepository = financialYearRepository;
        }
        #endregion

        #region Properties

        public FAR_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }
        public FAR_GenericRepository<FAR_Location> LocationRepository
        {
            get
            {
                return _locationRepository;
            }
        }

        public FAR_GenericRepository<FAR_Supplier> SupplierRepository
        {
            get
            {
                return _supplierRepository;
            }
        }

        public FAR_GenericRepository<FAR_DepreciationMethod> DepreciationMethodRepository
        {
            get
            {
                return _depreciationMethodRepository;
            }
        }

        public FAR_GenericRepository<FAR_Catagory> AssetCategoryRepository
        {
            get
            {
                return _categroyRepository;
            }
        }

        public FAR_GenericRepository<FAR_SubCategory> AssetSubCategoryRepository
        {
            get
            {
                return _subCategroyRepository;
            }
        }
        public FAR_GenericRepository<FAR_AssetStatus> AssetStatusRepository
        {
            get
            {
                return _assetStatusRepository;
            }
        }
        public FAR_GenericRepository<FAR_AssetCondition> AssetConditionRepository
        {
            get
            {
                return _assetConditionRepository;
            }
        }
        public FAR_GenericRepository<FAR_MaintenanceTypeInformation> MaintenanceTypeInformationRepository
        {
            get
            {
                return _maintenanceTypeInformationRepository;
            }
        }
        public FAR_GenericRepository<FAR_SparePartInformation> SparePartInformationRepository
        {
            get
            {
                return _sparePartInformationRepository;
            }
        }
        public FAR_GenericRepository<FAR_FixedAsset> FixedAssetRepository
        {
            get
            {
                return _fixedAssetRepository;
            }
        }

        public FAR_GenericRepository<FAR_Repair> AssetRepairingRepository
        {
            get { return _assetRepair; }
        }

        public FAR_GenericRepository<FAR_SaleDisposal> SaleDisposalRepository
        {
            get { return _saleDisposal; }
        }

        public FAR_GenericRepository<FAR_Depreciation> AssetDepreciationRepository
        {
            get { return _assetDepreciation; }
        }

        public FAR_GenericRepository<FAR_DepreciationDetail> AssetDepreciationDetailRepository
        {
            get { return _assetDepreciationDetail; }
        }

        public FAR_GenericRepository<FAR_AssetTransfer> AssetTransferRepository
        {
            get { return _assetTransfer; }
        }

        public FAR_GenericRepository<FAR_AssetMaintenanceInformation> AssetMaintenanceInformationRepository
        {
            get { return _assetMaintenanceInformationRepository; }
        }
        public FAR_GenericRepository<FAR_AssetMaintenanceInformationDetail> AssetMaintenanceInformationDetailRepository
        {
            get { return _assetMaintenanceInformationDetailRepository; }
        }
        public FAR_GenericRepository<FAR_AdditionalInfoofVehicle> AdditionalInfoofVehicleRepository
        {
            get { return _additionalInfoofVehicleRepository; }
        }
        public FAR_GenericRepository<FAR_AdditionalInfoofVehicleDetail> AdditionalInfoofVehicleDetailRepository
        {
            get { return _additionalInfoofVehicleDetailRepository; }
        }
        public FAR_GenericRepository<FAR_AccidentInformation> AccidentInformationRepository
        {
            get { return _accidentInformationRepository; }
        }
         public FAR_GenericRepository<acc_Accounting_Period_Information> FinancialYearRepository
        {
            get { return _financialYearRepository; }
        }
        
        #endregion
    }
}
