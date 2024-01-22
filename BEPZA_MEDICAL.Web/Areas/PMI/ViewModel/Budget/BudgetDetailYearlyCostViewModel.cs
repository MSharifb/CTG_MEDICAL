using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget
{
    public class BudgetDetailYearlyCostViewModel : BaseViewModel
    {
        public BudgetDetailYearlyCostViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public int BudgetDetailsId { get; set; }

        public int FinancialYearId { get; set; }

        [DisplayName("Status")]

        public int BudgetStatusId { get; set; }

        [DisplayName("Estimated Cost")]
        public decimal? EstematedCost { get; set; }



        #region Other

        public int? BudgetSubHeadId { get; set; }

        public string PreviousFieldId { get; set; }

        public string StatusName { get; set; }

        public string FinancialYearName { get; set; }

        #endregion
    }
}