using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.AnnualProcurementPlan
{
    public class AnnualProcurementPlanMasterViewModel : BaseViewModel
    {
        public AnnualProcurementPlanMasterViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            AnnualProcurementPlanDetailList = new List<AnnualProcurementPlanDetailViewModel>();
            BudgetDetailHeadList = new List<BudgetDetailsHeadViewModel>();
            ApproverList = new List<ApprovalFlowViewModel>();
            EmployeeList = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
            ApprovalStatusList = new List<SelectListItem>();
            FinancialYearList = new List<SelectListItem>();
            DivisionOrMinistryList = new List<SelectListItem>();
            BudgetList = new List<SelectListItem>();
            BudgetStatusList = new List<SelectListItem>();
        }
        #region Standard Property
        [DisplayName("Ministry")]
        [Required]
        public int MinistryOrDivisionId { get; set; }

        [DisplayName("Financial Year")]
        [Required]
        public int FinancialYearId { get; set; }

        [DisplayName("Agency")]
        public string Agency { get; set; }

        [DisplayName("Procuring Entity Name")]
        public string ProcuringEntityName { get; set; }

        [DisplayName("Procuring Entity Code")]
        public string ProcuringEntityCode { get; set; }

        [DisplayName("Project/Programme Name")]
        public string ProjectName { get; set; }

        [DisplayName("Project/Programme Code")]
        public string ProjectCode { get; set; }
        [DisplayName("APP Status")]
        public int APPStatusId { get; set; }

        [DisplayName("Zone")]
        public int[] BudgetZones { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        public int ZoneInfoId { get; set; }

        [DisplayName("Budget Status")]
        public int? BudgetStatusId { get; set; }
        public int ProjectForId { get; set; }
        public bool IsConfirm { get; set; }

        #endregion
        #region List


        public IList<SelectListItem> BudgetStatusList { get; set; }

        public IList<SelectListItem> DivisionOrMinistryList { get; set; }

        public IList<SelectListItem> ApprovalAuthorityList { get; set; }

        [DisplayName("Zone")]
        public IList<SelectListItem> ProjectOrZoneList { get; set; }

        public IList<SelectListItem> ProjectStatusList { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public IList<SelectListItem> APPStatusList { get; set; }

        [DisplayName("Budget")]
        public int? BudgetId { get; set; }
        public IList<SelectListItem> BudgetList { get; set; }
        #endregion 

        #region Other

        public List<AnnualProcurementPlanDetailViewModel> AnnualProcurementPlanDetailList { get; set; }

        public List<BudgetDetailsHeadViewModel> BudgetDetailHeadList { get; set; }

        public int APPDetailId { get; set; }
        public string AppHeadName { get; set; }
        public string FinancialYearName { get; set; }

        public string NameOfWorks { get; set; }

        public string projectTypeName { get; set; }

        public decimal BudgetAmount { get; set; }

        public decimal ApprovedAmount { get; set; }

        public string BudgetStatusName { get; set; }

        public string BudgetZonesString { get; set; }

        [DisplayName("Rec./Approve By")]
        public int EmployeeId { get; set; }

        public IList<SelectListItem> EmployeeList { get; set; }

        public int DesignationId { get; set; }

        public IList<SelectListItem> DesignationList { get; set; }


        public List<ApprovalFlowViewModel> ApproverList { get; set; }

        public string DepartmentName { get; set; }

        public string strCreateDate { get; set; }
        public string strLastUpdateDate { get; set; }
        public string Remarks { get; set; }
        public string ApprovalStatus { get; set; }

        [DisplayName("Total Project Cost(in Lac Taka)")]
        public decimal? TotalAmount { get; set; }

        [DisplayName("submission status")]
        public int? ApprovalStatusId { get; set; }
        public int? ApprovalSelectionId { get; set; }
        public IList<SelectListItem> ApprovalStatusList { get; set; }
        #endregion
    }

    public class BudgetDetailsHeadViewModel
    {
        public BudgetDetailsHeadViewModel()
        {
            BudgetHeadList = new List<SelectListItem>();
        }

        public int APPHeadId { get; set; }
        public IList<SelectListItem> BudgetHeadList { get; set; }
    }
}