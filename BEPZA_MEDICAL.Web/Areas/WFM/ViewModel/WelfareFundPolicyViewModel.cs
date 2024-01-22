using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class WelfareFundPolicyViewModel : BaseViewModel
    {
        #region Ctor
        public WelfareFundPolicyViewModel()
        {
            this.WelfareFundCategoryList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Properties

        [Required]
        [DisplayName("Category Name")]
        public int WelfareFundCategoryId { get; set; }

        [DisplayName("Order/Ref. No.")]
        public string OrderNo { get; set; }

        [Required]
        [DisplayName("Effective From Date")]
        [UIHint("_RequiredDate")]
        public DateTime? EffectiveFromDate { get; set; }

        [DisplayName("Effective To Date")]
        [UIHint("_RequiredDate")]
        public DateTime? EffectiveToDate { get; set; }

        [Required]
        [DisplayName("Minimum Service Year")]
        public decimal? MinServiceYear { get; set; }

        [Required]
        [DisplayName("Maximum Amount")]
        public decimal? MaxAmount { get; set; }

        [Required]
        [DisplayName("For Family Members, Maximum Amount ")]
        public decimal? OtherMaxAmount { get; set; }

        [DisplayName("Is Continuous")]
        public bool IsContinuous { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Reference Date")]
        [UIHint("_Date")]
        public DateTime? ReferenceDate { get; set; }
        #endregion

        #region Others

        public string WelfareFundCategoryName { get; set; }
        public IList<SelectListItem> WelfareFundCategoryList { get; set; }
        public string EffectDateView { get; set; }
        #endregion
    }
}