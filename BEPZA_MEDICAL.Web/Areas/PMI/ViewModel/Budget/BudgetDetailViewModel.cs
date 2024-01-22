using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Budget
{
    public class BudgetDetailViewModel : BaseViewModel
    {
        public BudgetDetailViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            BudgetSubHeadList = new List<SelectListItem>();
            BudgetHeadList = new List<SelectListItem>();
            ConstructionTypeList = new List<SelectListItem>();
            NameOfWorksList = new List<SelectListItem>();
            YearlyCostList = new List<BudgetDetailYearlyCostViewModel>();
            YearlyBilledList = new List<BudgetDetailYearlyBilledViewModel>();
            SubLedgerList = new List<SelectListItem>();
            WorkStatusList = new List<SelectListItem>();
        }

        public int BudgetMasterId { get; set; }

        [DisplayName("Budget Head")]
        [Required]
        public int BudgetHeadId { get; set; }

        [DisplayName("Sub-Head")]
        public int? BudgetSubHeadId { get; set; }

        [Required]
        [DisplayName("Construction Type")]
        public int ConstructionTypeId { get; set; }

        [DisplayName("Project")]
        [Required]
        public string NameOfWorks { get; set; }

        [DisplayName("Amount")]
        [Required]
        public decimal? BudgetAmount { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [DisplayName("Sub-Ledger")]
        public int? SubledgerId { get; set; }

        public string SerialNo { get; set; }

        [DisplayName("Amount")]
        public decimal? EstimatCost { get; set; }

        [DisplayName("Amount")]
        public decimal? ContractAmount { get; set; }

        [DisplayName("Amount")]
        public decimal? BillAmount { get; set; }

        [DisplayName("Amount")]
        public decimal? BillAmount2 { get; set; }

        public int? WorkStatusId { get; set; }

        public string WorkIdentificationNumber { get; set; }

        public string HiddenSlNo { get; set; }

        #region List
        public IList<SelectListItem> WorkStatusList { get; set; }

        public IList<SelectListItem> BudgetSubHeadList { get; set; }

        public IList<SelectListItem> BudgetHeadList { get; set; }

        public List<BudgetDetailYearlyCostViewModel> YearlyCostList { get; set; }

        public List<BudgetDetailYearlyBilledViewModel> YearlyBilledList { get; set; }

        public IList<SelectListItem> ConstructionTypeList { get; set; }

        public IList<SelectListItem> NameOfWorksList { get; set; }

        public IList<SelectListItem> SubLedgerList { get; set; }

        #endregion

        #region Others

        public string BudgetHeadName { get; set; }
        public string BudgetSubHeadName { get; set; }
        public string ConstructionTypeName { get; set; }

        #endregion
    }


}