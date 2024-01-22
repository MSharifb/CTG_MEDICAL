using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AssetRepairingViewModel : BaseViewModel
    {
        #region Ctor
        public AssetRepairingViewModel()
        {
            //this.AssetCategoryList = new List<SelectListItem>();
            //this.AssetStatusList = new List<SelectListItem>();
            this.AssetConditionList = new List<SelectListItem>();
        }
        #endregion


        #region Standard Porperties

        public int FixedAssetId { get; set; }

        [Required, DisplayName("Asset Code")]
        public string AssetCode { get; set; }

        [Required, UIHint("_ReadOnly"), DisplayName("Asset Name/Model")]

        public string AssetName { get; set; }

        public int CategoryId { get; set; }

        [Required, UIHint("_ReadOnly"), DisplayName("Category Name")]
        public string CategoryName { get; set; }

        public int SubCategoryId { get; set; }

        [Required, UIHint("_ReadOnly"), DisplayName("Sub-Category Name")]
        public string SubCategoryName { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Depreciation Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        [Required]
        [UIHint("_OnlyFiveLengthDecimalNumber")]
        [DisplayName("Depreciation Rate")]
        public Decimal? DepreciationRate { get; set; }

        [Required]
        [DisplayName("Asset Status")]
        public int AssetStatusId { get; set; }

        [Required, UIHint("_ReadOnly")]
        public string strAssetStatusName { get; set; }

        //[Required]
        [DisplayName("Asset Condition")]
        public int? AssetConditionId { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Current Book Value")]
        public Decimal CurrentBookValue { get; set; }

        [DisplayName("Is Impacted on depreciation")]
        public bool IsImpactWithDep { get; set; }

        [Required]
        [UIHint("_Only12LengthNumber")]
        [DisplayName("Repair Cost")]
        public Decimal RepairCost { get; set; }

        [Required]
        [UIHint("_Only12LengthNumber")]
        [DisplayName("Appreciated Cost")]
        public Decimal AppreciatedCost { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Updated Book Value")]
        public Decimal UpdatedBookValue { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Total Cost")]
        public Decimal TotalCost { get; set; }

        public DateTime? RepairDateFrom { get; set; }
        public DateTime? RepairDateTo { get; set; }

        [StringLength(500, ErrorMessage = "Description must be within 500 characters.")]
        [DisplayName("Description")]
        public string Remarks { get; set; }

        #endregion

        #region Other Properties
        //public IList<SelectListItem> AssetCategoryList { get; set; }
        //public IList<SelectListItem> AssetStatusList { get; set; }      
        public IList<SelectListItem> AssetConditionList { get; set; }

        #endregion
    }
}