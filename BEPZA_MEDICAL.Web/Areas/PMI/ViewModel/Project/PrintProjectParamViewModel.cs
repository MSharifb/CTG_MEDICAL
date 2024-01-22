using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project
{
    public class PrintProjectParamViewModel
    {
        public PrintProjectParamViewModel()
        {
            FinancialYearList = new List<SelectListItem>();
            ApprovalTypeList = new List<SelectListItem>();
            ProcurementTypeList = new List<SelectListItem>();
            ProjectList = new List<SelectListItem>();
            ZoneList = new List<ZoneListViewModel>();
            TenderNoticeFormatList = new List<string>();
            ParamList = new List<BudgetSummaryParamViewModel>();
            ProjectStatusList = new List<SelectListItem>();
        }

        [DisplayName("Financial Year")]
        public int FinancialYearId { get; set; }

        [DisplayName("Approval Type")]
        public int ApprovalTypeId { get; set; }

        [DisplayName("Procurement Type")]
        public int ProcurementTypeId { get; set; }

        [DisplayName("Name of Work")]
        public int ProjectId { get; set; }

        [DisplayName("Date")]
        [UIHint("_Date")]
        public DateTime ProjectDate { get; set; }

        [DisplayName("Zone")]
        public List<ZoneListViewModel> ZoneList { get; set; }

        public string ReportName { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public IList<SelectListItem> ApprovalTypeList { get; set; }

        public IList<SelectListItem> ProcurementTypeList { get; set; }

        public IList<SelectListItem> ProjectList { get; set; }

        #region Others

        [DisplayName("Financial Year")]
        [UIHint("_readOnly")]
        public string FinancialYearName { get; set; }

        [DisplayName("Tender Publication Date")]
        [UIHint("_readOnly")]
        public string ProjectDateStr { get; set; }

        [DisplayName("Name of Work")]
        [UIHint("_readOnly")]
        public string NameOfWork { get; set; }

        public List<string> TenderNoticeFormatList { get; set; }

        public List<BudgetSummaryParamViewModel> ParamList { get; set; }

        public string ActionType { get; set; }

        public int ProjectStatusId { get; set; }

        public IList<SelectListItem> ProjectStatusList { get; set; }


        #endregion
    }
}