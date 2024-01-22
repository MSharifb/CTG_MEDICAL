using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Project
{
    [Serializable]
    public class BudgetSummaryParamViewModel
    {
        public BudgetSummaryParamViewModel()
        {
            FinancialYearList = new List<SelectListItem>();
            ApprovalTypeList = new List<SelectListItem>();
        }

        [DisplayName("Financial Year")]
        public int FinancialYearId { get; set; }

        [DisplayName("Approval Type")]
        public int ApprovalTypeId { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public IList<SelectListItem> ApprovalTypeList { get; set; }

    }
}