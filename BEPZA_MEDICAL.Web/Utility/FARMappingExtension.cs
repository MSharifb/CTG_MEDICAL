using AutoMapper;
using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class FARMappingExtension
    {
        //Location
        public static LocationViewModel ToModel(this FAR_Location gradeStep)
        {
            return Mapper.Map<FAR_Location, LocationViewModel>(gradeStep);
        }
        public static FAR_Location ToEntity(this LocationViewModel gradeStepModel)
        {
            return Mapper.Map<LocationViewModel, FAR_Location>(gradeStepModel);
        }


        //Supplier
        public static SupplierViewModel ToModel(this FAR_Supplier gradeStep)
        {
            return Mapper.Map<FAR_Supplier, SupplierViewModel>(gradeStep);
        }
        public static FAR_Supplier ToEntity(this SupplierViewModel gradeStepModel)
        {
            return Mapper.Map<SupplierViewModel, FAR_Supplier>(gradeStepModel);
        }

        //Asset Category
        public static AssetCategoryViewModel ToModel(this FAR_Catagory gradeStep)
        {
            return Mapper.Map<FAR_Catagory, AssetCategoryViewModel>(gradeStep);
        }
        public static FAR_Catagory ToEntity(this AssetCategoryViewModel gradeStepModel)
        {
            return Mapper.Map<AssetCategoryViewModel, FAR_Catagory>(gradeStepModel);
        }


        //Asset Sub Category
        public static AssetSubCategoryViewModel ToModel(this FAR_SubCategory gradeStep)
        {
            return Mapper.Map<FAR_SubCategory, AssetSubCategoryViewModel>(gradeStep);
        }
        public static FAR_SubCategory ToEntity(this AssetSubCategoryViewModel gradeStepModel)
        {
            return Mapper.Map<AssetSubCategoryViewModel, FAR_SubCategory>(gradeStepModel);
        }

        //Asset Condition

        public static AssetConditionViewModel ToModel(this FAR_AssetCondition gradeStep)
        {
            return Mapper.Map<FAR_AssetCondition, AssetConditionViewModel>(gradeStep);
        }
        public static FAR_AssetCondition ToEntity(this AssetConditionViewModel gradeStepModel)
        {
            return Mapper.Map<AssetConditionViewModel, FAR_AssetCondition>(gradeStepModel);
        }

        //Maintenance Type Information
        public static MaintenanceTypeInformationViewModel ToModel(this FAR_MaintenanceTypeInformation gradeStep)
        {
            return Mapper.Map<FAR_MaintenanceTypeInformation, MaintenanceTypeInformationViewModel>(gradeStep);
        }
        public static FAR_MaintenanceTypeInformation ToEntity(this MaintenanceTypeInformationViewModel gradeStepModel)
        {
            return Mapper.Map<MaintenanceTypeInformationViewModel, FAR_MaintenanceTypeInformation>(gradeStepModel);
        }

        //Spare Part Information
        public static SparePartInformationViewModel ToModel(this FAR_SparePartInformation gradeStep)
        {
            return Mapper.Map<FAR_SparePartInformation, SparePartInformationViewModel>(gradeStep);
        }
        public static FAR_SparePartInformation ToEntity(this SparePartInformationViewModel gradeStepModel)
        {
            return Mapper.Map<SparePartInformationViewModel, FAR_SparePartInformation>(gradeStepModel);
        }

        //Fixed Asset
        public static FixedAssetViewModel ToModel(this FAR_FixedAsset gradeStep)
        {
            return Mapper.Map<FAR_FixedAsset, FixedAssetViewModel>(gradeStep);
        }
        public static FAR_FixedAsset ToEntity(this FixedAssetViewModel gradeStepModel)
        {
            return Mapper.Map<FixedAssetViewModel, FAR_FixedAsset>(gradeStepModel);
        }
        //Asset Maintenance Type Info
        public static AssetMaintenanceInformationViewModel ToModel(this FAR_AssetMaintenanceInformation gradeStep)
        {
            return Mapper.Map<FAR_AssetMaintenanceInformation, AssetMaintenanceInformationViewModel>(gradeStep);
        }
        public static FAR_AssetMaintenanceInformation ToEntity(this AssetMaintenanceInformationViewModel gradeStepModel)
        {
            return Mapper.Map<AssetMaintenanceInformationViewModel, FAR_AssetMaintenanceInformation>(gradeStepModel);
        }
        //Asset Maintenance Type Info Detail
        public static AssetMaintenanceInformationDetailViewModel ToModel(this FAR_AssetMaintenanceInformationDetail gradeStep)
        {
            return Mapper.Map<FAR_AssetMaintenanceInformationDetail, AssetMaintenanceInformationDetailViewModel>(gradeStep);
        }
        public static FAR_AssetMaintenanceInformationDetail ToEntity(this AssetMaintenanceInformationDetailViewModel gradeStepModel)
        {
            return Mapper.Map<AssetMaintenanceInformationDetailViewModel, FAR_AssetMaintenanceInformationDetail>(gradeStepModel);
        }

        //Asset Repairing
        public static AssetRepairingViewModel ToModel(this FAR_Repair gradeStep)
        {
            return Mapper.Map<FAR_Repair, AssetRepairingViewModel>(gradeStep);
        }
        public static FAR_Repair ToEntity(this AssetRepairingViewModel gradeStepModel)
        {
            return Mapper.Map<AssetRepairingViewModel, FAR_Repair>(gradeStepModel);
        }
        //Additional Information of Vehicle 
        public static AdditionalInformationofVehicleViewModel ToModel(this FAR_AdditionalInfoofVehicle gradeStep)
        {
            return Mapper.Map<FAR_AdditionalInfoofVehicle, AdditionalInformationofVehicleViewModel>(gradeStep);
        }
        public static FAR_AdditionalInfoofVehicle ToEntity(this AdditionalInformationofVehicleViewModel gradeStepModel)
        {
            return Mapper.Map<AdditionalInformationofVehicleViewModel, FAR_AdditionalInfoofVehicle>(gradeStepModel);
        }
        //Additional Information of Vehicle  Details
        public static AdditionalInformationofVehicleDetailViewModel ToModel(this FAR_AdditionalInfoofVehicleDetail gradeStep)
        {
            return Mapper.Map<FAR_AdditionalInfoofVehicleDetail, AdditionalInformationofVehicleDetailViewModel>(gradeStep);
        }
        public static FAR_AdditionalInfoofVehicleDetail ToEntity(this AdditionalInformationofVehicleDetailViewModel gradeStepModel)
        {
            return Mapper.Map<AdditionalInformationofVehicleDetailViewModel, FAR_AdditionalInfoofVehicleDetail>(gradeStepModel);
        }
        //Accident Information 
        public static AccidentInformationViewModel ToModel(this FAR_AccidentInformation gradeStep)
        {
            return Mapper.Map<FAR_AccidentInformation, AccidentInformationViewModel>(gradeStep);
        }
        public static FAR_AccidentInformation ToEntity(this AccidentInformationViewModel gradeStepModel)
        {
            return Mapper.Map<AccidentInformationViewModel, FAR_AccidentInformation>(gradeStepModel);
        }

        //Sale Disposal
        public static SaleDisposalViewModel ToModel(this FAR_SaleDisposal gradeStep)
        {
            return Mapper.Map<FAR_SaleDisposal, SaleDisposalViewModel>(gradeStep);
        }
        public static FAR_SaleDisposal ToEntity(this SaleDisposalViewModel gradeStepModel)
        {
            return Mapper.Map<SaleDisposalViewModel, FAR_SaleDisposal>(gradeStepModel);
        }

        //Asset Transfer

        public static AssetTransferViewModel ToModel(this FAR_AssetTransfer gradeStep)
        {
            return Mapper.Map<FAR_AssetTransfer, AssetTransferViewModel>(gradeStep);
        }
        public static FAR_AssetTransfer ToEntity(this AssetTransferViewModel gradeStepModel)
        {
            return Mapper.Map<AssetTransferViewModel, FAR_AssetTransfer>(gradeStepModel);
        }


    }
}