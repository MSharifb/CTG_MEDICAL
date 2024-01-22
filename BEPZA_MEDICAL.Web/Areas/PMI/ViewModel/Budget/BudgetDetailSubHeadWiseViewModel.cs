using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.PMI.ViewModel.Budget
{
    public class BudgetDetailSubHeadWiseViewModel
    {


        [DisplayName("Budget Head")]
        [Required]
        public int BudgetHeadId { get; set; }

        [DisplayName("Budget Sub-Head")]
        [Required]
        public int BudgetSubHeadId { get; set; }

        [DisplayName("Name of Works/Goods/Services")]
        [Required]
        public string NameOfWorks { get; set; }

        [DisplayName("Budget Amount (Taka in Lakh)")]
        [Required]
        public decimal BudgetAmount { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        public IList<SelectListItem> BudgetSubHeadList { get; set; }


        public List<BudgetDetailYearlyCostViewModel> YearlyCostList { get; set; }
    }
}