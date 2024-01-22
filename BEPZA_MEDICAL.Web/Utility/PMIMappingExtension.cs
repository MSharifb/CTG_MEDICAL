using AutoMapper;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.AnnualProcurementPlan;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ProgressReport;
using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class PMIMappingExtension
    {

        //PRM_HumanResourceRateAssignMaster
        public static BudgetHeadViewModel ToModel(this PMI_BudgetHead budgetHeads)
        {
            return Mapper.Map<PMI_BudgetHead, BudgetHeadViewModel>(budgetHeads);
        }
        public static PMI_BudgetHead ToEntity(this BudgetHeadViewModel budgetHeadViewModel)
        {
            return Mapper.Map<BudgetHeadViewModel, PMI_BudgetHead>(budgetHeadViewModel);
        }

        public static List<CommonConfigTypeViewModel> ToModelList(this List<BEPZA_MEDICAL.DAL.PMI.CommonConfigType> objlist)
        {
            List<CommonConfigTypeViewModel> list = new List<CommonConfigTypeViewModel>();
            foreach (var item in objlist)
            {
                list.Add((Mapper.Map<BEPZA_MEDICAL.DAL.PMI.CommonConfigType, CommonConfigTypeViewModel>(item)));
            }
            return list;
        }
        public static List<BEPZA_MEDICAL.DAL.PMI.CommonConfigType> ToEntityList(this List<CommonConfigTypeViewModel> modellist)
        {
            List<BEPZA_MEDICAL.DAL.PMI.CommonConfigType> list = new List<BEPZA_MEDICAL.DAL.PMI.CommonConfigType>();
            foreach (var item in modellist)
            {
                list.Add((Mapper.Map<CommonConfigTypeViewModel, BEPZA_MEDICAL.DAL.PMI.CommonConfigType>(item)));
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

        //Project Master
        public static ProjectViewModel ToModel(this PMI_ProjectMaster obj)
        {
            return Mapper.Map<PMI_ProjectMaster, ProjectViewModel>(obj);
        }
        public static PMI_ProjectMaster ToEntity(this ProjectViewModel model)
        {
            return Mapper.Map<ProjectViewModel, PMI_ProjectMaster>(model);
        }


        //Project Details
        public static ProjectDetailsViewModel ToModel(this PMI_ProjectDetails obj)
        {
            return Mapper.Map<PMI_ProjectDetails, ProjectDetailsViewModel>(obj);
        }
        public static PMI_ProjectDetails ToEntity(this ProjectDetailsViewModel model)
        {
            return Mapper.Map<ProjectDetailsViewModel, PMI_ProjectDetails>(model);
        }

        //Tender Details
        public static TenderDetailsViewModel ToModel(this PMI_TenderDetails obj)
        {
            return Mapper.Map<PMI_TenderDetails, TenderDetailsViewModel>(obj);
        }
        public static PMI_TenderDetails ToEntity(this TenderDetailsViewModel model)
        {
            return Mapper.Map<TenderDetailsViewModel, PMI_TenderDetails>(model);
        }

        //Estimation Unit Setup
        public static EstimationUnitViewModel ToModel(this PMI_EstimationUnit obj)
        {
            return Mapper.Map<PMI_EstimationUnit, EstimationUnitViewModel>(obj);
        }
        public static PMI_EstimationUnit ToEntity(this EstimationUnitViewModel model)
        {
            return Mapper.Map<EstimationUnitViewModel, PMI_EstimationUnit>(model);
        }

        //Estimation Head Setup
        public static EstimationHeadViewModel ToModel(this PMI_EstimationHead obj)
        {
            return Mapper.Map<PMI_EstimationHead, EstimationHeadViewModel>(obj);
        }
        public static PMI_EstimationHead ToEntity(this EstimationHeadViewModel model)
        {
            return Mapper.Map<EstimationHeadViewModel, PMI_EstimationHead>(model);
        }

        //Estimation Setup
        public static EstimationSetupViewModel ToModel(this PMI_EstimationSetup obj)
        {
            return Mapper.Map<PMI_EstimationSetup, EstimationSetupViewModel>(obj);
        }
        public static PMI_EstimationSetup ToEntity(this EstimationSetupViewModel model)
        {
            return Mapper.Map<EstimationSetupViewModel, PMI_EstimationSetup>(model);
        }

        //Budget Master
        public static BudgetMasterViewModel ToModel(this PMI_BudgetMaster obj)
        {
            return Mapper.Map<PMI_BudgetMaster, BudgetMasterViewModel>(obj);
        }
        public static PMI_BudgetMaster ToEntity(this BudgetMasterViewModel model)
        {
            return Mapper.Map<BudgetMasterViewModel, PMI_BudgetMaster>(model);
        }

        //Budget Details
        public static BudgetDetailViewModel ToModel(this PMI_BudgetDetails obj)
        {
            return Mapper.Map<PMI_BudgetDetails, BudgetDetailViewModel>(obj);
        }
        public static PMI_BudgetDetails ToEntity(this BudgetDetailViewModel model)
        {
            return Mapper.Map<BudgetDetailViewModel, PMI_BudgetDetails>(model);
        }

        //Budget Details Yearly Cost
        public static BudgetDetailYearlyCostViewModel ToModel(this PMI_BudgetDetailsYearlyCost obj)
        {
            return Mapper.Map<PMI_BudgetDetailsYearlyCost, BudgetDetailYearlyCostViewModel>(obj);
        }
        public static PMI_BudgetDetailsYearlyCost ToEntity(this BudgetDetailYearlyCostViewModel model)
        {
            return Mapper.Map<BudgetDetailYearlyCostViewModel, PMI_BudgetDetailsYearlyCost>(model);
        }

        //Budget Details Yearly Billed
        public static BudgetDetailYearlyBilledViewModel ToModel(this PMI_BudgetDetailsYearlyBilled obj)
        {
            return Mapper.Map<PMI_BudgetDetailsYearlyBilled, BudgetDetailYearlyBilledViewModel>(obj);
        }
        public static PMI_BudgetDetailsYearlyBilled ToEntity(this BudgetDetailYearlyBilledViewModel model)
        {
            return Mapper.Map<BudgetDetailYearlyBilledViewModel, PMI_BudgetDetailsYearlyBilled>(model);
        }

        //Project Estimation
        public static ProjectEstimationViewModel ToModel(this PMI_EstimationMaster obj)
        {
            return Mapper.Map<PMI_EstimationMaster, ProjectEstimationViewModel>(obj);
        }
        public static PMI_EstimationMaster ToEntity(this ProjectEstimationViewModel model)
        {
            return Mapper.Map<ProjectEstimationViewModel, PMI_EstimationMaster>(model);
        }

        //Project Estimation Details
        public static ProjectEstimationDetailsViewModel ToModel(this PMI_EstimationDetails obj)
        {
            return Mapper.Map<PMI_EstimationDetails, ProjectEstimationDetailsViewModel>(obj);
        }
        public static PMI_EstimationDetails ToEntity(this ProjectEstimationDetailsViewModel model)
        {
            return Mapper.Map<ProjectEstimationDetailsViewModel, PMI_EstimationDetails>(model);
        }


        //Project Zone
        public static ZoneListViewModel ToModel(this PMI_ProjectZone obj)
        {
            return Mapper.Map<PMI_ProjectZone, ZoneListViewModel>(obj);
        }
        public static PMI_ProjectZone ToEntity(this ZoneListViewModel model)
        {
            return Mapper.Map<ZoneListViewModel, PMI_ProjectZone>(model);
        }

        //Budget Zone
        public static BugdetZoneListViewModel ToModel(this PMI_BudgetZoneOrProject obj)
        {
            return Mapper.Map<PMI_BudgetZoneOrProject, BugdetZoneListViewModel>(obj);
        }
        public static PMI_BudgetZoneOrProject ToEntity(this BugdetZoneListViewModel model)
        {
            return Mapper.Map<BugdetZoneListViewModel, PMI_BudgetZoneOrProject>(model);
        }

        //Estimation Zone
        public static EstimationZoneListViewModel ToModel(this PMI_EstimationZone obj)
        {
            return Mapper.Map<PMI_EstimationZone, EstimationZoneListViewModel>(obj);
        }
        public static PMI_EstimationZone ToEntity(this EstimationZoneListViewModel model)
        {
            return Mapper.Map<EstimationZoneListViewModel, PMI_EstimationZone>(model);
        }


        //Estimation View
        public static EstimationListViewModel ToModel(this vwPMIEstimation obj)
        {
            return Mapper.Map<vwPMIEstimation, EstimationListViewModel>(obj);
        }
        public static vwPMIEstimation ToEntity(this EstimationListViewModel model)
        {
            return Mapper.Map<EstimationListViewModel, vwPMIEstimation>(model);
        }

        //Project Time Extension
        public static ProjectTimeExtensionViewModel ToModel(this PMI_ProjectTimeExtension obj)
        {
            return Mapper.Map<PMI_ProjectTimeExtension, ProjectTimeExtensionViewModel>(obj);
        }
        public static PMI_ProjectTimeExtension ToEntity(this ProjectTimeExtensionViewModel model)
        {
            return Mapper.Map<ProjectTimeExtensionViewModel, PMI_ProjectTimeExtension>(model);
        }

        //Estimation Head
        public static EstimationHeadDesciptionViewModel ToModel(this PMI_EstimationHeadDescription obj)
        {
            return Mapper.Map<PMI_EstimationHeadDescription, EstimationHeadDesciptionViewModel>(obj);
        }
        public static PMI_EstimationHeadDescription ToEntity(this EstimationHeadDesciptionViewModel model)
        {
            return Mapper.Map<EstimationHeadDesciptionViewModel, PMI_EstimationHeadDescription>(model);
        }

        //APP Master
        public static AnnualProcurementPlanMasterViewModel ToModel(this PMI_AnnualProcurementPlanMaster obj)
        {
            return Mapper.Map<PMI_AnnualProcurementPlanMaster, AnnualProcurementPlanMasterViewModel>(obj);
        }
        public static PMI_AnnualProcurementPlanMaster ToEntity(this AnnualProcurementPlanMasterViewModel model)
        {
            return Mapper.Map<AnnualProcurementPlanMasterViewModel, PMI_AnnualProcurementPlanMaster>(model);
        }
        //APP Detail
        public static AnnualProcurementPlanDetailViewModel ToModel(this PMI_AnnualProcurementPlanDetails obj)
        {
            return Mapper.Map<PMI_AnnualProcurementPlanDetails, AnnualProcurementPlanDetailViewModel>(obj);
        }
        public static PMI_AnnualProcurementPlanDetails ToEntity(this AnnualProcurementPlanDetailViewModel model)
        {
            return Mapper.Map<AnnualProcurementPlanDetailViewModel, PMI_AnnualProcurementPlanDetails>(model);
        }

        //Progress Report Master
        public static ProgressReportMasterViewModel ToModel(this PMI_ProgressReportMaster obj)
        {
            return Mapper.Map<PMI_ProgressReportMaster, ProgressReportMasterViewModel>(obj);
        }
        public static PMI_ProgressReportMaster ToEntity(this ProgressReportMasterViewModel model)
        {
            return Mapper.Map<ProgressReportMasterViewModel, PMI_ProgressReportMaster>(model);
        }

        //Progress Report Details
        public static ProgressReportDetailsViewModel ToModel(this PMI_ProgressReportDetails obj)
        {
            return Mapper.Map<PMI_ProgressReportDetails, ProgressReportDetailsViewModel>(obj);
        }
        public static PMI_ProgressReportDetails ToEntity(this ProgressReportDetailsViewModel model)
        {
            return Mapper.Map<ProgressReportDetailsViewModel, PMI_ProgressReportDetails>(model);
        }

        //Progress Report Details Billed
        public static ProgressReportDetailYearlyBilledViewModel ToModel(this PMI_ProgressReportDetailsYearlyBilled obj)
        {
            return Mapper.Map<PMI_ProgressReportDetailsYearlyBilled, ProgressReportDetailYearlyBilledViewModel>(obj);
        }
        public static PMI_ProgressReportDetailsYearlyBilled ToEntity(this ProgressReportDetailYearlyBilledViewModel model)
        {
            return Mapper.Map<ProgressReportDetailYearlyBilledViewModel, PMI_ProgressReportDetailsYearlyBilled>(model);
        }

        //Heads
        public static HeadViewModel ToModel(this PMI_Head obj)
        {
            return Mapper.Map<PMI_Head, HeadViewModel>(obj);
        }
        public static PMI_Head ToEntity(this HeadViewModel model)
        {
            return Mapper.Map<HeadViewModel, PMI_Head>(model);
        }

    }

}