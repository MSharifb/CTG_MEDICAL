using AutoMapper;
using BEPZA_MEDICAL.DAL.FAR;
using BEPZA_MEDICAL.Web.Areas.FAR.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class FARMapper
    {
        public FARMapper()
        {
            //Location
            Mapper.CreateMap<LocationViewModel, FAR_Location>();
            Mapper.CreateMap<FAR_Location, LocationViewModel>();

            //Location
            Mapper.CreateMap<SupplierViewModel, FAR_Supplier>();
            Mapper.CreateMap<FAR_Supplier, SupplierViewModel>();

            //Asset Category
            Mapper.CreateMap<AssetCategoryViewModel, FAR_Catagory>();
            Mapper.CreateMap<FAR_Catagory, AssetCategoryViewModel>();

            //Asset Sub Category
            Mapper.CreateMap<AssetSubCategoryViewModel, FAR_SubCategory>();
            Mapper.CreateMap<FAR_SubCategory, AssetSubCategoryViewModel>();

            //Asset Condition
            Mapper.CreateMap<AssetConditionViewModel, FAR_AssetCondition>();
            Mapper.CreateMap<FAR_AssetCondition, AssetConditionViewModel>();

            //Maintenance Type Information
            Mapper.CreateMap<MaintenanceTypeInformationViewModel, FAR_MaintenanceTypeInformation>();
            Mapper.CreateMap<FAR_MaintenanceTypeInformation, MaintenanceTypeInformationViewModel>();

            //Spare Part Information
            Mapper.CreateMap<SparePartInformationViewModel, FAR_SparePartInformation>();
            Mapper.CreateMap<FAR_SparePartInformation, SparePartInformationViewModel>();

            //Fixed Asset
            Mapper.CreateMap<FixedAssetViewModel, FAR_FixedAsset>();
            Mapper.CreateMap<FAR_FixedAsset, FixedAssetViewModel>();

            //Asset Maintenance Type
            Mapper.CreateMap<AssetMaintenanceInformationViewModel, FAR_AssetMaintenanceInformation>();
            Mapper.CreateMap<FAR_AssetMaintenanceInformation, AssetMaintenanceInformationViewModel>();

            Mapper.CreateMap<AssetMaintenanceInformationDetailViewModel, FAR_AssetMaintenanceInformationDetail>();
            Mapper.CreateMap<FAR_AssetMaintenanceInformationDetail, AssetMaintenanceInformationDetailViewModel>();

            //Asset Repairing
            Mapper.CreateMap<AssetRepairingViewModel, FAR_Repair>();
            Mapper.CreateMap<FAR_Repair, AssetRepairingViewModel>();

            //Additional Information of Vehicle 
            Mapper.CreateMap<AdditionalInformationofVehicleViewModel, FAR_AdditionalInfoofVehicle>();
            Mapper.CreateMap<FAR_AdditionalInfoofVehicle, AdditionalInformationofVehicleViewModel>();

            Mapper.CreateMap<AdditionalInformationofVehicleDetailViewModel, FAR_AdditionalInfoofVehicleDetail>();
            Mapper.CreateMap<FAR_AdditionalInfoofVehicleDetail, AdditionalInformationofVehicleDetailViewModel>();

            //Accident Information 
            Mapper.CreateMap<AccidentInformationViewModel, FAR_AccidentInformation>();
            Mapper.CreateMap<FAR_AccidentInformation, AccidentInformationViewModel>();

            //Sale Disposal
            Mapper.CreateMap<SaleDisposalViewModel, FAR_SaleDisposal>();
            Mapper.CreateMap<FAR_SaleDisposal, SaleDisposalViewModel>();

            //Asset Transfer           
            Mapper.CreateMap<AssetTransferViewModel, FAR_AssetTransfer>();
            Mapper.CreateMap<FAR_AssetTransfer, AssetTransferViewModel>();

        }
    }
}