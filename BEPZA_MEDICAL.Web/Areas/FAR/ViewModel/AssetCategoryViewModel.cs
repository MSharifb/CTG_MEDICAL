using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AssetCategoryViewModel : BaseViewModel
    {

        #region Ctor
        public AssetCategoryViewModel()
        {
            this.DepreciationMethodList = new List<SelectListItem>();
        }
        #endregion
       

        #region Standard Porperties
        
        [Required]
        [StringLength(2, ErrorMessage = "Category Code must be within 2 characters.")]
        [DisplayName("Category Code")]
        public string CategoryCode { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Category name must be within 100 characters.")]
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        [Required]
        [UIHint("_Only12LengthNumber")]
        [DisplayName("Min. Cost Allowed")]
        public Decimal? MinimumCost { get; set; }

        [Required]
        [DisplayName("Method of Depreciation")]
        public int DepreciationMethodId { get; set; }

        public string DepreciationMethod { get; set; }

        [Required]
        [UIHint("_Only64BitInteger")]
        [DisplayName("Reserved ID From")]
        public int ReservedIDFrom { get; set; }

        [Required]
        [DisplayName("To")]      
        public int ReservedIDTo { get; set; }

        [StringLength(100, ErrorMessage = "Remarks must be within 100 characters.")]
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        public bool IsAssetExists { get; set; }

        #endregion

        #region Other Properties
        public IList<SelectListItem> DepreciationMethodList { get; set; }

        #endregion
    }
}