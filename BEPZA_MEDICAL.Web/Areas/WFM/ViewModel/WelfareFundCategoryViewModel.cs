using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class WelfareFundCategoryViewModel : BaseViewModel
    {
        #region Ctor
        public WelfareFundCategoryViewModel()
        {
            this.BudgetHeadList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Budget Head")]
        public int COAId { get; set; }

        [DisplayName("Is Active?")]
        public bool IsActive { get; set; }

        [MaxLength(250,ErrorMessage = "Maximum length 250.")]
        public string Remarks { get; set; }

        #endregion

        #region Others 

        public string BudgetHeadName { get; set; }

         public IList<SelectListItem> BudgetHeadList { get; set; }
        #endregion

    }
}