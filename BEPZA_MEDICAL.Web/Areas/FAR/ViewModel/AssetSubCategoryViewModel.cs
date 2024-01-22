using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AssetSubCategoryViewModel : BaseViewModel
    {
        #region Ctor
        public AssetSubCategoryViewModel()
        {
            this.AssetCategoryList = new List<SelectListItem>();
        }
        #endregion


        #region Standard Porperties

        public string AssetCategoryCode { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Sub-Category Code must be within 20 characters.")]
        [DisplayName("Sub-Category Code")]
        public string SubCategoryCode { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Sub-Category name must be within 100 characters.")]
        [DisplayName("Sub-Category Name")]
        public string SubCategoryName { get; set; }

        [Required]
        [UIHint("_OnlyCurrency")]
        [DisplayName("Depreciation Rate(%)")]
        [Range(0, 99.99)]
        public Decimal? DepreciationRate { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        //[Required]
        //[UIHint("_ReadOnly")]
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        [StringLength(100, ErrorMessage = "Remarks must be within 100 characters.")]
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        #endregion


        #region Other Properties

        public IList<SelectListItem> AssetCategoryList { get; set; }

        #endregion
    }
}