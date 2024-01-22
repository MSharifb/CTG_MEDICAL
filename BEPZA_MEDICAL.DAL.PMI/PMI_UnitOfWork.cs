using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.DAL.PMI
{
    public class PMI_UnitOfWork
    {
        #region Fields
        PMI_ExecuteFunctions _functionRepository;

        PMI_GenericRepository<PMI_BudgetHead> _budgetHeadRepository;
        PMI_GenericRepository<vwPMIBudgetHead> _budgetHeadViewRepository;
        PMI_GenericRepository<CommonConfigType> _configTypeRepository;
        PMI_GenericRepository<PMI_Ministry> _ministryRepository;
        PMI_GenericRepository<PMI_SourceOfFund> _sourceofFundRepository;
        PMI_GenericRepository<PMI_ApprovalAuthority> _approvalAuthorityRepository;
        PMI_GenericRepository<PMI_ProcurementType> _procurementTypeRepository;
        PMI_GenericRepository<PMI_ProjectStatus> _projectStatusRepository;
        PMI_GenericRepository<PMI_Agency> _agencyRepository;
        PMI_GenericRepository<PMI_ProcurementMethod> _procurementMethodRepository;
        PMI_GenericRepository<PMI_ConstructionType> _constructionTypeRepository;
        PMI_GenericRepository<PMI_ConstructionCategory> _constructionCategoryRepository;
        PMI_GenericRepository<PMI_PaymentStatus> _paymentStatusRepository;
        PMI_GenericRepository<PMI_ProjectMaster> _projectMasterRepository;
        PMI_GenericRepository<PMI_TenderDetails> _tenderDetailsRepository;
        PMI_GenericRepository<PMI_EstimationUnit> _estimationUnitRepository;
        PMI_GenericRepository<PMI_EstimationHead> _estimationHeadRepository;
        PMI_GenericRepository<PMI_EstimationSetup> _estimationSetupRepository;
        PMI_GenericRepository<PMI_BudgetMaster> _budgetMasterRepository;
        PMI_GenericRepository<PMI_BudgetDetails> _budgetDetailsRepository;
        PMI_GenericRepository<PMI_BudgetDetailsYearlyCost> _budgetDetailsYearlyCostRepository;
        PMI_GenericRepository<vwPMIStatusInformation> _StatusInformationRepository;
        PMI_GenericRepository<PMI_EstimationMaster> _projectEstimationRepository;
        PMI_GenericRepository<PMI_EstimationDetails> _projectEstimationDetailsRepository;
        PMI_GenericRepository<PMI_ProjectDetails> _projectDetailsRepository;
        PMI_GenericRepository<PMI_ProjectZone> _projectZoneRepository;
        PMI_GenericRepository<PMI_BudgetZoneOrProject> _budgetZoneRepository;
        PMI_GenericRepository<PMI_EstimationZone> _estimationZoneRepository;
        PMI_GenericRepository<vwPMIEstimation> _estimationViewRepository;
        PMI_GenericRepository<vwPMIProjectSummary> _projectViewRepository;
        PMI_GenericRepository<vwPMIBudgetInfo> _budgetViewRepository;
        PMI_GenericRepository<PMI_ProjectTimeExtension> _projectTimeExtensionRepository;
        PMI_GenericRepository<acc_Accounting_Period_Information> _accountingPeriodRepository;
        PMI_GenericRepository<PMI_EstimationHeadDescription> _estimationHeadDescriptionRepository;
        PMI_GenericRepository<PMI_ApprovalSection> _approvalSectionRepository;
        PMI_GenericRepository<PMI_ApprovalFlowMaster> _approvalFlowMasterRepository;
        PMI_GenericRepository<PMI_ApprovalFlowDetails> _approvalFlowDetailsRepository;
        PMI_GenericRepository<PMI_ProjectApproverStatus> _projectApproverStatus;
        PMI_GenericRepository<PMI_AnnualProcurementPlanMaster> _annualProcurementPlanMasterRepository;
        PMI_GenericRepository<PMI_AnnualProcurementPlanDetails> _annualProcurementPlanDetailsRepository;
        PMI_GenericRepository<PMI_AppZones> _appZonesRepository;
        PMI_GenericRepository<acc_Cost_Centre_or_Institute_Information> _subLedgerRepository;
        PMI_GenericRepository<PMI_WorkStatus> _workStatusRepository;
        PMI_GenericRepository<PMI_BudgetDetailsYearlyBilled> _budgetDetailsYearlyBilledRepository;
        PMI_GenericRepository<PMI_ProgressReportMaster> _progressReportMasterRepository;
        PMI_GenericRepository<PMI_ProgressReportDetails> _progressReportDetailsRepository;
        PMI_GenericRepository<PMI_ProgressReportDetailsYearlyBilled> _progressReportDetailsYearlyBilledRepository;
        PMI_GenericRepository<PMI_ProjectFor> _projectForRepository;
        PMI_GenericRepository<PMI_Head> _headRepository;
        PMI_GenericRepository<PMI_ProgressReportAttachment> _progressReportAttachmentRepository;
        PMI_GenericRepository<vwPMINewBudgetInfo> _newBudgetInfoRepository;
        PMI_GenericRepository<PMI_ProgressReportSignature> _progressReportSignatureRepository;

        #endregion

        #region Ctor
        public PMI_UnitOfWork(
            PMI_ExecuteFunctions functionRepository,
            PMI_GenericRepository<PMI_BudgetHead> budgetHeadRepository,
            PMI_GenericRepository<vwPMIBudgetHead> budgetHeadViewRepository,
            PMI_GenericRepository<CommonConfigType> configTypeRepository,
            PMI_GenericRepository<PMI_Ministry> ministryRepository,
            PMI_GenericRepository<PMI_SourceOfFund> sourceOfFundRepository,
            PMI_GenericRepository<PMI_ApprovalAuthority> approvalAuthorityRepository,
            PMI_GenericRepository<PMI_ProcurementType> procurementTypeRepository,
            PMI_GenericRepository<PMI_ProjectStatus> projectStatusRepository,
            PMI_GenericRepository<PMI_Agency> agencyRepository,
            PMI_GenericRepository<PMI_ProcurementMethod> procurementMethodRepository,
            PMI_GenericRepository<PMI_ConstructionType> constructionTypeRepository,
            PMI_GenericRepository<PMI_ConstructionCategory> constructionCategoryRepository,
            PMI_GenericRepository<PMI_PaymentStatus> paymentStatusRepository,
            PMI_GenericRepository<PMI_ProjectMaster> projectMasterRepository,
            PMI_GenericRepository<PMI_TenderDetails> tenderDetailsRepository,
            PMI_GenericRepository<PMI_EstimationUnit> estimationUnitRepository,
            PMI_GenericRepository<PMI_EstimationHead> estimationHeadRepository,
            PMI_GenericRepository<PMI_EstimationSetup> estimationSetupRepository,
            PMI_GenericRepository<PMI_BudgetMaster> budgetMasterRepository,
            PMI_GenericRepository<PMI_BudgetDetails> budgetDetailsRepository,
            PMI_GenericRepository<PMI_BudgetDetailsYearlyCost> budgetDetailsYearlyCostRepository,
            PMI_GenericRepository<vwPMIStatusInformation> statusInformationRepository,
            PMI_GenericRepository<PMI_EstimationMaster> projectEstimationRepository,
            PMI_GenericRepository<PMI_EstimationDetails> projectEstimationDetailsRepository,
            PMI_GenericRepository<PMI_ProjectDetails> projectDetailsRepository,
            PMI_GenericRepository<PMI_ProjectZone> projectZoneRepository,
            PMI_GenericRepository<PMI_BudgetZoneOrProject> budgetZoneRepository,
            PMI_GenericRepository<PMI_EstimationZone> estimationZoneRepository,
            PMI_GenericRepository<vwPMIEstimation> _estimationViewRepository,
            PMI_GenericRepository<vwPMIProjectSummary> _projectViewRepository,
            PMI_GenericRepository<vwPMIBudgetInfo> _budgetViewRepository,
            PMI_GenericRepository<PMI_ProjectTimeExtension> projectTimeExtensionRepository,
            PMI_GenericRepository<acc_Accounting_Period_Information> accountingPeriodRepository,
            PMI_GenericRepository<PMI_EstimationHeadDescription> estimationHeadDescriptionRepository,
            PMI_GenericRepository<PMI_ApprovalSection> approvalSectionRepository,
            PMI_GenericRepository<PMI_ApprovalFlowMaster> approvalFlowMasterRepository,
            PMI_GenericRepository<PMI_ApprovalFlowDetails> approvalFlowDetailsRepository,
            PMI_GenericRepository<PMI_ProjectApproverStatus> projectApproverStatus,
            PMI_GenericRepository<PMI_AnnualProcurementPlanMaster> annualProcurementPlanMasterRepository,
            PMI_GenericRepository<PMI_AnnualProcurementPlanDetails> annualProcurementPlanDetailsRepository,
            PMI_GenericRepository<PMI_AppZones> appZonesRepository,
            PMI_GenericRepository<acc_Cost_Centre_or_Institute_Information> subLedgerRepository,
            PMI_GenericRepository<PMI_WorkStatus> workStatusRepository,
            PMI_GenericRepository<PMI_BudgetDetailsYearlyBilled> budgetDetailsYearlyBilledRepository,
            PMI_GenericRepository<PMI_ProgressReportMaster> progressReportMasterRepository,
            PMI_GenericRepository<PMI_ProgressReportDetails> progressReportDetailsRepository,
            PMI_GenericRepository<PMI_ProgressReportDetailsYearlyBilled> progressReportDetailsYearlyBilledRepository,
            PMI_GenericRepository<PMI_ProjectFor> projectForRepository,
            PMI_GenericRepository<PMI_Head> headRepository,
            PMI_GenericRepository<PMI_ProgressReportAttachment> progressReportAttachmentRepository,
            PMI_GenericRepository<vwPMINewBudgetInfo> newBudgetInfoRepository,
            PMI_GenericRepository<PMI_ProgressReportSignature> progressReportSignatureRepository
            )
        {
            this._functionRepository = functionRepository;
            this._budgetHeadRepository = budgetHeadRepository;
            this._budgetHeadViewRepository = budgetHeadViewRepository;
            this._configTypeRepository = configTypeRepository;
            this._ministryRepository = ministryRepository;
            this._sourceofFundRepository = sourceOfFundRepository;
            this._approvalAuthorityRepository = approvalAuthorityRepository;
            this._procurementTypeRepository = procurementTypeRepository;
            this._projectStatusRepository = projectStatusRepository;
            this._agencyRepository = agencyRepository;
            this._procurementMethodRepository = procurementMethodRepository;
            this._constructionTypeRepository = constructionTypeRepository;
            this._constructionCategoryRepository = constructionCategoryRepository;
            this._paymentStatusRepository = paymentStatusRepository;
            this._projectMasterRepository = projectMasterRepository;
            this._tenderDetailsRepository = tenderDetailsRepository;
            this._estimationUnitRepository = estimationUnitRepository;
            this._estimationHeadRepository = estimationHeadRepository;
            this._estimationSetupRepository = estimationSetupRepository;
            this._budgetMasterRepository = budgetMasterRepository;
            this._budgetDetailsRepository = budgetDetailsRepository;
            this._budgetDetailsYearlyCostRepository = budgetDetailsYearlyCostRepository;
            this._StatusInformationRepository = statusInformationRepository;
            this._projectEstimationRepository = projectEstimationRepository;
            this._projectEstimationDetailsRepository = projectEstimationDetailsRepository;
            this._projectDetailsRepository = projectDetailsRepository;
            this._projectZoneRepository = projectZoneRepository;
            this._budgetZoneRepository = budgetZoneRepository;
            this._estimationZoneRepository = estimationZoneRepository;
            this._estimationViewRepository = _estimationViewRepository;
            this._projectViewRepository = _projectViewRepository;
            this._budgetViewRepository = _budgetViewRepository;
            this._projectTimeExtensionRepository = projectTimeExtensionRepository;
            this._accountingPeriodRepository = accountingPeriodRepository;
            this._estimationHeadDescriptionRepository = estimationHeadDescriptionRepository;
            this._approvalSectionRepository = approvalSectionRepository;
            this._approvalFlowMasterRepository = approvalFlowMasterRepository;
            this._approvalFlowDetailsRepository = approvalFlowDetailsRepository;
            this._projectApproverStatus = projectApproverStatus;
            this._annualProcurementPlanMasterRepository = annualProcurementPlanMasterRepository;
            this._annualProcurementPlanDetailsRepository = annualProcurementPlanDetailsRepository;
            this._appZonesRepository = appZonesRepository;
            this._subLedgerRepository = subLedgerRepository;
            this._workStatusRepository = workStatusRepository;
            this._budgetDetailsYearlyBilledRepository = budgetDetailsYearlyBilledRepository;
            this._progressReportMasterRepository = progressReportMasterRepository;
            this._progressReportDetailsRepository = progressReportDetailsRepository;
            this._progressReportDetailsYearlyBilledRepository = progressReportDetailsYearlyBilledRepository;
            this._projectForRepository = projectForRepository;
            this._headRepository = headRepository;
            this._progressReportAttachmentRepository = progressReportAttachmentRepository;
            this._newBudgetInfoRepository = newBudgetInfoRepository;
            this._progressReportSignatureRepository = progressReportSignatureRepository;
        }
        #endregion

        #region Properties
        public PMI_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        public PMI_GenericRepository<PMI_BudgetHead> BudgetHeadRepository
        {
            get
            {
                return _budgetHeadRepository;
            }
        }

        public PMI_GenericRepository<vwPMIBudgetHead> BudgetHeadViewRepository
        {
            get
            {
                return _budgetHeadViewRepository;
            }
        }


        public PMI_GenericRepository<CommonConfigType> ConfigTypeRepository
        {
            get
            {
                return _configTypeRepository;
            }
        }

        public PMI_GenericRepository<PMI_Ministry> MinistryRepository
        {
            get
            {
                return _ministryRepository;
            }
        }

        public PMI_GenericRepository<PMI_SourceOfFund> SourceOfFundRepository
        {
            get
            {
                return _sourceofFundRepository;
            }
        }

        public PMI_GenericRepository<PMI_ApprovalAuthority> ApprovalAuthorityRepository
        {
            get
            {
                return _approvalAuthorityRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProcurementType> ProcurementTypeRepository
        {
            get
            {
                return _procurementTypeRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProjectStatus> ProjectStatusRepository
        {
            get
            {
                return _projectStatusRepository;
            }
        }

        public PMI_GenericRepository<PMI_Agency> AgencyRepository
        {
            get
            {
                return _agencyRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProcurementMethod> ProcurementMethodRepository
        {
            get
            {
                return _procurementMethodRepository;
            }
        }

        public PMI_GenericRepository<PMI_ConstructionType> ConstructionTypeRepository
        {
            get
            {
                return _constructionTypeRepository;
            }
        }

        public PMI_GenericRepository<PMI_ConstructionCategory> ConstructionCategoryRepository
        {
            get
            {
                return _constructionCategoryRepository;
            }
        }

        public PMI_GenericRepository<PMI_PaymentStatus> PaymentStatusRepository
        {
            get
            {
                return _paymentStatusRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProjectMaster> ProjectMasterRepository
        {
            get
            {
                return _projectMasterRepository;
            }
        }

        public PMI_GenericRepository<PMI_TenderDetails> TenderDetailsRepository
        {
            get
            {
                return _tenderDetailsRepository;
            }
        }

        public PMI_GenericRepository<PMI_EstimationUnit> EstimationUnitRepository
        {
            get
            {
                return _estimationUnitRepository;
            }
        }

        public PMI_GenericRepository<PMI_EstimationHead> EstimationHeadRepository
        {
            get
            {
                return _estimationHeadRepository;
            }
        }

        public PMI_GenericRepository<PMI_EstimationSetup> EstimationSetupRepository
        {
            get
            {
                return _estimationSetupRepository;
            }
        }



        public PMI_GenericRepository<PMI_BudgetMaster> BudgetMasterRepository
        {
            get
            {
                return _budgetMasterRepository;
            }
        }

        public PMI_GenericRepository<PMI_BudgetDetails> BudgetDetailsRepository
        {
            get
            {
                return _budgetDetailsRepository;
            }
        }

        public PMI_GenericRepository<PMI_BudgetDetailsYearlyCost> BudgetDetailsYearlyCostRepository
        {
            get
            {
                return _budgetDetailsYearlyCostRepository;
            }
        }

        public PMI_GenericRepository<vwPMIStatusInformation> StatusInformationRepository
        {
            get
            {
                return _StatusInformationRepository;
            }
        }

        public PMI_GenericRepository<PMI_EstimationMaster> ProjectEstimationRepository
        {
            get
            {
                return _projectEstimationRepository;
            }
        }

        public PMI_GenericRepository<PMI_EstimationDetails> ProjectEstimationDetailsRepository
        {
            get
            {
                return _projectEstimationDetailsRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProjectDetails> ProjectDetailsRepository
        {
            get
            {
                return _projectDetailsRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProjectZone> ProjectZoneRepository
        {
            get
            {
                return _projectZoneRepository;
            }
        }

        public PMI_GenericRepository<PMI_BudgetZoneOrProject> BudgetZoneRepository
        {
            get
            {
                return _budgetZoneRepository;
            }
        }

        public PMI_GenericRepository<PMI_EstimationZone> EstimationZoneRepository
        {
            get
            {
                return _estimationZoneRepository;
            }
        }

        public PMI_GenericRepository<vwPMIEstimation> EstimationViewRepository
        {
            get
            {
                return _estimationViewRepository;
            }
        }

        public PMI_GenericRepository<vwPMIProjectSummary> ProjectViewRepository
        {
            get
            {
                return _projectViewRepository;
            }
        }

        public PMI_GenericRepository<vwPMIBudgetInfo> BudgetViewRepository
        {
            get
            {
                return _budgetViewRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProjectTimeExtension> ProjectTimeExtensionRepository
        {
            get
            {
                return _projectTimeExtensionRepository;
            }
        }

        public PMI_GenericRepository<acc_Accounting_Period_Information> AccountingPeriodRepository
        {
            get
            {
                return _accountingPeriodRepository;
            }
        }


        public PMI_GenericRepository<PMI_EstimationHeadDescription> EstimationHeadDescriptionRepository
        {
            get
            {
                return _estimationHeadDescriptionRepository;
            }
        }

        public PMI_GenericRepository<PMI_ApprovalSection> ApprovalSectionRepository
        {
            get
            {
                return _approvalSectionRepository;
            }
        }
        public PMI_GenericRepository<PMI_ApprovalFlowMaster> ApprovalFlowMasterRepository
        {
            get
            {
                return _approvalFlowMasterRepository;
            }
        }
        public PMI_GenericRepository<PMI_ApprovalFlowDetails> ApprovalFlowDetailsRepository
        {
            get
            {
                return _approvalFlowDetailsRepository;
            }
        }
        public PMI_GenericRepository<PMI_ProjectApproverStatus> ProjectApproverStatusRepository
        {
            get
            {
                return _projectApproverStatus;
            }
        }
        public PMI_GenericRepository<PMI_AnnualProcurementPlanMaster> AnnualProcurementPlanMasterRepository
        {
            get
            {
                return _annualProcurementPlanMasterRepository;
            }
        }
        public PMI_GenericRepository<PMI_AnnualProcurementPlanDetails> AnnualProcurementPlanDetailsRepository
        {
            get
            {
                return _annualProcurementPlanDetailsRepository;
            }
        }
        public PMI_GenericRepository<PMI_AppZones> AppZonesRepository
        {
            get
            {
                return _appZonesRepository;
            }
        }
        public PMI_GenericRepository<acc_Cost_Centre_or_Institute_Information> SubLedgerRepository
        {
            get
            {
                return _subLedgerRepository;
            }
        }
        public PMI_GenericRepository<PMI_WorkStatus> WorkStatusRepository
        {
            get
            {
                return _workStatusRepository;
            }
        }
        public PMI_GenericRepository<PMI_BudgetDetailsYearlyBilled> BudgetDetailsYearlyBilledRepository
        {
            get
            {
                return _budgetDetailsYearlyBilledRepository;
            }
        }
        public PMI_GenericRepository<PMI_ProgressReportMaster> ProgressReportMasterRepository
        {
            get
            {
                return _progressReportMasterRepository;
            }
        }
        public PMI_GenericRepository<PMI_ProgressReportDetails> ProgressReportDetailsRepository
        {
            get
            {
                return _progressReportDetailsRepository;
            }
        }
        public PMI_GenericRepository<PMI_ProgressReportDetailsYearlyBilled> ProgressReportDetailsYearlyBilledRepository
        {
            get
            {
                return _progressReportDetailsYearlyBilledRepository;
            }
        }

        public PMI_GenericRepository<PMI_ProjectFor> ProjectForRepository
        {
            get
            {
                return _projectForRepository;
            }
        }

        public PMI_GenericRepository<PMI_Head> HeadRepository
        {
            get
            {
                return _headRepository;
            }
        }
        public PMI_GenericRepository<PMI_ProgressReportAttachment> ProgressReportAttachmentRepository
        {
            get
            {
                return _progressReportAttachmentRepository;
            }
        }
        public PMI_GenericRepository<vwPMINewBudgetInfo> NewBudgetInfoRepository
        {
            get
            {
                return _newBudgetInfoRepository;
            }
        }
        public PMI_GenericRepository<PMI_ProgressReportSignature> ProgressReportSignatureRepository
        {
            get
            {
                return _progressReportSignatureRepository;
            }
        }
        #endregion
    }
}
