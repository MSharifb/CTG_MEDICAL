using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget
{
    public class BudgetDetailYearlyBilledViewModel : BaseViewModel
    {
        public BudgetDetailYearlyBilledViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public int BudgetDetailsId { get; set; }

        public int? FinancialYearId { get; set; }

        [DisplayName("Billed Amount")]
        public decimal? BilledAmount { get; set; }



        #region Other

        public int? BudgetSubHeadId { get; set; }

        public string PreviousFieldId { get; set; }

        public string StatusName { get; set; }

        public string FinancialYearName { get; set; }

        #endregion
    }
}