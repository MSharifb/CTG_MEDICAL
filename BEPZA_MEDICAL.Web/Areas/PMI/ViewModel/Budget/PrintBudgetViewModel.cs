using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget
{
    public class PrintBudgetViewModel : BaseViewModel
    {
        public PrintBudgetViewModel()
        {
            this.FinancialYearList = new List<SelectListItem>();
            this.ProjectList = new List<SelectListItem>();
        }

        [DisplayName("Financial Year")]
        public int FinancialYearId { get; set; }

        [DisplayName("Name of Work/Goods/Services")]
        public int ProjectId { get; set; }

        [DisplayName("Status")]
        public int BudgetStatusId { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }

        public IList<SelectListItem> ProjectList { get; set; }

        public IList<SelectListItem> BudgetStatusList { get; set; }
    }
}