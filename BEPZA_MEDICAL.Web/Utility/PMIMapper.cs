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
    public class PMIMapper
    {
        public PMIMapper()
        {
            //Bank Letter Body
            Mapper.CreateMap<BudgetHeadViewModel, PMI_BudgetHead>();
            Mapper.CreateMap<PMI_BudgetHead, BudgetHeadViewModel>();

            //Common configType
            Mapper.CreateMap<CommonConfigTypeViewModel, BEPZA_MEDICAL.DAL.PMI.CommonConfigType>();
            Mapper.CreateMap<BEPZA_MEDICAL.DAL.PMI.CommonConfigType, CommonConfigTypeViewModel>();

            //Common config
            Mapper.CreateMap<CommonConfigViewModel, CommonConfigGetResult>();
            Mapper.CreateMap<CommonConfigGetResult, CommonConfigViewModel>();

            //Project Master
            Mapper.CreateMap<ProjectViewModel, PMI_ProjectMaster>();
            Mapper.CreateMap<PMI_ProjectMaster, ProjectViewModel>();

            //Project Details
            Mapper.CreateMap<ProjectDetailsViewModel, PMI_ProjectDetails>();
            Mapper.CreateMap<PMI_ProjectDetails, ProjectDetailsViewModel>();

            //Tender Details
            Mapper.CreateMap<TenderDetailsViewModel, PMI_TenderDetails>();
            Mapper.CreateMap<PMI_TenderDetails, TenderDetailsViewModel>();

            //Estimation Unit Setup
            Mapper.CreateMap<EstimationUnitViewModel, PMI_EstimationUnit>();
            Mapper.CreateMap<PMI_EstimationUnit, EstimationUnitViewModel>();

            //Estimation Head Setup
            Mapper.CreateMap<EstimationHeadViewModel, PMI_EstimationHead>();
            Mapper.CreateMap<PMI_EstimationHead, EstimationHeadViewModel>();

            //Estimation Setup
            Mapper.CreateMap<EstimationSetupViewModel, PMI_EstimationSetup>();
            Mapper.CreateMap<PMI_EstimationSetup, EstimationSetupViewModel>();

            //Budget Master
            Mapper.CreateMap<BudgetMasterViewModel, PMI_BudgetMaster>();
            Mapper.CreateMap<PMI_BudgetMaster, BudgetMasterViewModel>();

            //Budget Detail
            Mapper.CreateMap<BudgetDetailViewModel, PMI_BudgetDetails>();
            Mapper.CreateMap<PMI_BudgetDetails, BudgetDetailViewModel>();

            //Budget Detail
            Mapper.CreateMap<BudgetDetailYearlyCostViewModel, PMI_BudgetDetailsYearlyCost>();
            Mapper.CreateMap<PMI_BudgetDetailsYearlyCost, BudgetDetailYearlyCostViewModel>();

            //Budget Detail Billed
            Mapper.CreateMap<BudgetDetailYearlyBilledViewModel, PMI_BudgetDetailsYearlyBilled>();
            Mapper.CreateMap<PMI_BudgetDetailsYearlyBilled, BudgetDetailYearlyBilledViewModel>();

            //Project Estimation
            Mapper.CreateMap<ProjectEstimationViewModel, PMI_EstimationMaster>();
            Mapper.CreateMap<PMI_EstimationMaster, ProjectEstimationViewModel>();

            //Project Estimation Details
            Mapper.CreateMap<ProjectEstimationDetailsViewModel, PMI_EstimationDetails>();
            Mapper.CreateMap<PMI_EstimationDetails, ProjectEstimationDetailsViewModel>();

            //Project Zone Info
            Mapper.CreateMap<ZoneListViewModel, PMI_ProjectZone>();
            Mapper.CreateMap<PMI_ProjectZone, ZoneListViewModel>();

            //Budget Zone Info
            Mapper.CreateMap<BugdetZoneListViewModel, PMI_BudgetZoneOrProject>();
            Mapper.CreateMap<PMI_BudgetZoneOrProject, BugdetZoneListViewModel>();

            //Estimation Zone Info
            Mapper.CreateMap<EstimationZoneListViewModel, PMI_EstimationZone>();
            Mapper.CreateMap<PMI_EstimationZone, EstimationZoneListViewModel>();

            //Estimation View
            Mapper.CreateMap<EstimationListViewModel, vwPMIEstimation>();
            Mapper.CreateMap<vwPMIEstimation, EstimationListViewModel>();

            //Project Time Extension
            Mapper.CreateMap<ProjectTimeExtensionViewModel, PMI_ProjectTimeExtension>();
            Mapper.CreateMap<PMI_ProjectTimeExtension, ProjectTimeExtensionViewModel>();

            //Estimation Head
            Mapper.CreateMap<EstimationHeadDesciptionViewModel, PMI_EstimationHeadDescription>();
            Mapper.CreateMap<PMI_EstimationHeadDescription, EstimationHeadDesciptionViewModel>();

            //APP Master
            Mapper.CreateMap<AnnualProcurementPlanMasterViewModel, PMI_AnnualProcurementPlanMaster>();
            Mapper.CreateMap<PMI_AnnualProcurementPlanMaster, AnnualProcurementPlanMasterViewModel>();

            //APP Details
            Mapper.CreateMap<AnnualProcurementPlanDetailViewModel, PMI_AnnualProcurementPlanDetails>();
            Mapper.CreateMap<PMI_AnnualProcurementPlanDetails, AnnualProcurementPlanDetailViewModel>();

            //Progress Report Master
            Mapper.CreateMap<ProgressReportMasterViewModel, PMI_ProgressReportMaster>();
            Mapper.CreateMap<PMI_ProgressReportMaster, ProgressReportMasterViewModel>();

            //Progress Report Details
            Mapper.CreateMap<ProgressReportDetailsViewModel, PMI_ProgressReportDetails>();
            Mapper.CreateMap<PMI_ProgressReportDetails, ProgressReportDetailsViewModel>();

            //Progress Report Details Yearly Billed
            Mapper.CreateMap<ProgressReportDetailYearlyBilledViewModel, PMI_ProgressReportDetailsYearlyBilled>();
            Mapper.CreateMap<PMI_ProgressReportDetailsYearlyBilled, ProgressReportDetailYearlyBilledViewModel>();

            //Heads
            Mapper.CreateMap<HeadViewModel, PMI_Head>();
            Mapper.CreateMap<PMI_Head, HeadViewModel>();

        }
    }
}