using BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.ApprovalFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget
{
    public class BudgetMasterViewModel : BaseViewModel
    {
        public BudgetMasterViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            BudgetDetailList = new List<BudgetDetailViewModel>();
            YearWiseBudgetStatusList = new List<YearWiseBudgetStatusViewModel>();
            YearWiseBilledList = new List<YearWiseBilledViewModel>();
            BudgetDetailHeadList = new List<BudgetDetailsHeadViewModel>();
            ApproverList = new List<ApprovalFlowViewModel>();
            EmployeeList = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
            ApprovalStatusList = new List<SelectListItem>();
        }

        [DisplayName("Budget Type")]
        public string BudgetType { get; set; }

        [DisplayName("Ministry")]
        [Required]
        public int MinistryOrDivisionId { get; set; }

        [DisplayName("Zone")]
        [Required]
        public int[] BudgetZones { get; set; }

        [DisplayName("Source of Fund")]
        public int SourceOfFundId { get; set; }

        [DisplayName("Approval Authority")]
        [Required]
        public int ApprovalAuthorityId { get; set; }

        [DisplayName("Approval Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime ApprovalDate { get; set; }

        [DisplayName("Procurement Type")]
        [Required]
        public int ProcurementTypeId { get; set; }

        [DisplayName("Create Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Last Update Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime LastUpdateDate { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        public int ZoneInfoId { get; set; }
        public int ProjectForId { get; set; }
        public bool IsConfirm { get; set; }
        #region List

        public IList<SelectListItem> DivisionOrMinistryList { get; set; }

        public IList<SelectListItem> SourceOfFundList { get; set; }

        public IList<SelectListItem> ApprovalAuthorityList { get; set; }

        public IList<SelectListItem> ProcurementTypeList { get; set; }

        [DisplayName("Zone")]
        public IList<SelectListItem> ProjectOrZoneList { get; set; }

        public IList<SelectListItem> ProjectStatusList { get; set; }

        #endregion

        #region Other

        public List<BudgetDetailViewModel> BudgetDetailList { get; set; }

        public List<YearWiseBudgetStatusViewModel> YearWiseBudgetStatusList { get; set; }

        public List<YearWiseBilledViewModel> YearWiseBilledList { get; set; }

        public List<BudgetDetailsHeadViewModel> BudgetDetailHeadList { get; set; }

        public int BudgetDetailId { get; set; }

        public int FinancialYearId { get; set; }

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

        [DisplayName("submission status")]
        public int? ApprovalStatusId { get; set; }
        public int? ApprovalSelectionId { get; set; }
        public IList<SelectListItem> ApprovalStatusList { get; set; }
        #endregion
    }

    public class YearWiseBudgetStatusViewModel
    {
        public YearWiseBudgetStatusViewModel()
        {
            FinancialYearList = new List<SelectListItem>();
            BudgetStatusList = new List<SelectListItem>();
        }
        public int FinancialYearId { get; set; }

        public int BudgetStatusId { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public IList<SelectListItem> BudgetStatusList { get; set; }

        public string StatusName { get; set; }

        public string FinancialYearName { get; set; }

        public decimal EstematedCost { get; set; }

    }

    public class BudgetDetailsHeadViewModel
    {
        public BudgetDetailsHeadViewModel()
        {
            BudgetHeadList = new List<SelectListItem>();
        }

        public int BudgetHeadId { get; set; }
        public IList<SelectListItem> BudgetHeadList { get; set; }
    }
    public class YearWiseBilledViewModel
    {
        public YearWiseBilledViewModel()
        {
            FinancialYearList = new List<SelectListItem>();
        }
        public int? FinancialYearId { get; set; }


        public IList<SelectListItem> FinancialYearList { get; set; }

        public string FinancialYearName { get; set; }

        public decimal BilledAmount { get; set; }
    }
}