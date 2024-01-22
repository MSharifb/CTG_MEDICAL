using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class SaleDisposalViewModel : BaseViewModel
    {
        #region Ctor
        public SaleDisposalViewModel()
        {
            this.AssetConditionList = new List<SelectListItem>();

        }
        #endregion
       
        #region Standard Porperties
        

        #region Type

        [Required(ErrorMessage = "Please select type Disposal or Sale.")]
        [DisplayName("Type")]
        public string strType { get; set; }  
        #endregion

        [DisplayName("Category ID")]
        public int CategoryId { get; set; }       

        [Required]
        [DisplayName("Category Name")]
        [UIHint("_ReadOnly")]
        public string CategoryName { get; set; }

        [DisplayName("Sub-Category ID")]
        public int SubCategoryId { get; set; }
       
        [Required]
        [UIHint("_ReadOnly")]
        [DisplayName("Sub-Category Name")]
        public string SubCategoryName { get; set; }

        [Required]
        [DisplayName("Fixed Asset Id")]
        public int FixedAssetId { get; set; }

        [DisplayName("Asset Code")]
        public string AssetCode { get; set; }

        [Required]
        [UIHint("_ReadOnly")]
        [StringLength(100, ErrorMessage = "Asset Name/Model must be within 100 characters.")]
        [DisplayName("Asset Name/Model")]
        public string AssetName { get; set; }

        [DisplayName("Status")]
        public int AssetStatusId { get; set; }

        [Required]
        [DisplayName("Status")]
        [UIHint("_ReadOnly")]
        public string strAssetStatusName { get; set; }

        public int PreviousStatusId { get; set; }

        public int? PreviousConditionId { get; set; }

        //[Required]
        [DisplayName("Asset Condition")]
        public int? AssetConditionId { get; set; }

        [Required]
        [DisplayName("Supplier")]
        public int SupplierId { get; set; }

        public string AssetSupplier { get; set; }

        public string AssetPurchaseDate { get; set; }
        public DateTime? EffectiveDateBetween { get; set; }
        public DateTime? EffectiveDateBetweenAnd { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        [Required]
        [UIHint("_Only12LengthNumber")]
        [DisplayName("Sale Value")]
        public Decimal SalValue { get; set; }

        [UIHint("_ReadOnlyAmount")]
        public Decimal DisposedValue { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Asset Cost")]
        public Decimal? AssetCost { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Current Book Value")]
        public Decimal CurrentBookValue { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Accumulated Dep.")]
        public Decimal AccumulatedDepreciation { get; set; }

        [Required]
        [UIHint("_ReadOnlyAmount")]
        [DisplayName("Capital Gain/Loss")]
        public Decimal CapitalGain { get; set; }

        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }

        public string AssetLocation { get; set; }

        //[Required]
        //[DisplayName("Project No.")]
        //public int ProjectId { get; set; }

        //public string ProjectNo { get; set; }

        [StringLength(500, ErrorMessage = "Remarks must be within 500 characters.")]
        [DisplayName("Description")]
        public string Remarks { get; set; }

        #endregion

        #region Other Properties
        public IList<SelectListItem> AssetConditionList { get; set; }

        #endregion
    }
}