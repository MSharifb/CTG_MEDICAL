using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget
{
    public class FinancialYearViewModel : BaseViewModel
    {
        public FinancialYearViewModel()
        {
            this.FinancialYearList = new List<FinancialYearViewModel>();
        }
        [DisplayName("Financial Year")]
        public string FinancialYearName { get; set; }

        [DisplayName("Start Date")]
        public DateTime FinancialYearStartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime FinancialYearEndDate { get; set; }

        public List<FinancialYearViewModel> FinancialYearList { get; set; }

        public bool IsSelected { get; set; }

    }
}