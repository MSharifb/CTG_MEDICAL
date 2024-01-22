using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class FixedAssetViewModel : BaseViewModel
    {
        #region Ctor
        public FixedAssetViewModel()
        {
            this.AssetCategoryList = new List<SelectListItem>();
            this.AssetSubCategoryList = new List<SelectListItem>();
            this.AssetStatusList = new List<SelectListItem>();
            this.AssetConditionList = new List<SelectListItem>();
            this.AssetSupplierList = new List<SelectListItem>();
            this.LocationList = new List<SelectListItem>();
            this.ItemTypeList = new List<SelectListItem>();
            this.PurchaseList = new List<SelectListItem>();
            this.ItemList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Porperties
        public int ZoneInfoId { get; set; }

        #region Asset Type Infos---------------

        [Required(ErrorMessage = "Please select asset type New or Existing.")]
        [DisplayName("Asset Type")]
        public string AssetType { get; set; }
        public string SelectedAssetType { get; set; }

        [DisplayName("New Asset")]
        public bool NewAsset { get; set; }

        [DisplayName("Existing Asset")]
        public bool ExistingAsset { get; set; }

        #endregion

        [DisplayName("Refurbishment")]
        public bool IsRefurbishment { get; set; }

        [DisplayName("Calculate Depreciation")]
        public bool IsCalculateDepreciation { get; set; }

        [DisplayName("Organogram Level")]
        public int? OrganogramLevelId { get; set; }

        [UIHint("_ReadOnly"), DisplayName("Organogram Level")]
        public string OrganogramLevelName { get; set; }

        [Required, DisplayName("Item Type")]
        public int ItemTypeId { get; set; }

        [Required, DisplayName("Category")]
        public int CategoryId { get; set; }

        [Required, DisplayName("Sub-Category")]
        public int SubCategoryId { get; set; }

        [DisplayName("Asset Code")]
        [StringLength(150, ErrorMessage = "Asset Code must be within 150 characters.")]
        public string AssetCode { get; set; }

        [Required, DisplayName("Asset Name/Model")]
        [StringLength(250, ErrorMessage = "Asset Name/Model must be within 250 characters.")]
        public string AssetName { get; set; }

        [UIHint("_OnlyCurrency"), DisplayName("Depreciation Rate(%)")]
        [Range(0, 99.99)]
        public Decimal? DepreciationRate { get; set; }

        [DisplayName("Asset Status")]
        public int AssetStatusId { get; set; }

        [DisplayName("Asset Condition")]
        public int? AssetConditionId { get; set; }

        [Required, DisplayName("Supplier")]
        public int SupplierId { get; set; }

        [DisplayName("Supplier")]
        public string SupplierName { get; set; }

        [Required, DisplayName("Purchase date/Establishment date"), UIHint("_ReadOnlyDate")]
        public DateTime? PurchaseDate { get; set; }

        [Required, UIHint("_Date"), DisplayName("Dep. Effective Date")]
        public DateTime? DepreciationEffectiveDate { get; set; }

        [Required, UIHint("_OnlyCurrency"), DisplayName("Asset Cost")]
        [Range(0, 999999999999.99)]
        public Decimal? AssetCost { get; set; }

        [UIHint("_Only12LengthNumber"), DisplayName("Opening Balance of Depreciation")]
        public Decimal? OBDepreciation { get; set; }

        [UIHint("_ReadOnlyAmount"), DisplayName("Opening Balance of Book Value")]
        public Decimal? OBBookValue { get; set; }


        [UIHint("_ReadOnly"), DisplayName("Category Code")]
        public string AssetCategoryCode { get; set; }

        [UIHint("_ReadOnly"), DisplayName("Category Name")]
        public string CategoryName { get; set; }

        [UIHint("_ReadOnly"), DisplayName("Sub-Category Name")]
        public string SubCategoryName { get; set; }

        [DisplayName("Asset Status")]
        public string AssetStatus { get; set; }

        public string AssetSupplier { get; set; }

        public string AssetPurchaseDate { get; set; }
        public DateTime? PurchaseDateBetween { get; set; }
        public DateTime? PurchaseDateAnd { get; set; }

        [Required, DisplayName("Location")]
        public int LocationId { get; set; }

        public int? BeneficiaryEmployeeId { get; set; }
        public string AssetLocation { get; set; }

        [DisplayName("Serial No.")]
        public string SerialNo { get; set; }


        [StringLength(500, ErrorMessage = "Remarks must be within 500 characters.")]
        [DisplayName("Description")]
        public string Remarks { get; set; }


        [UIHint("_ReadOnlyAmount"), DisplayName("Current Book Value")]
        public Decimal? CurrentBookValue { get; set; }

        [DisplayName("Is Allow Depreciation Calculation")]
        [Description("Deteminate further depreciation calculation. During depreciation calculation if value <=1 then it will be 0.")]
        public bool IsAllowDepreciationCal { get; set; }

        public IList<SelectListItem> ItemTypeList { get; set; }

        public bool IsApproved { get; set; }
        public string ApprovalStatus { get; set; }

        [Required, DisplayName("MRR #")]
        public int? PurchaseId { get; set; }

        [Required, DisplayName("Asset Name/Model")]
        public int? ItemId { get; set; }

        [DisplayName("Quantity")]
        public int? Quantity { get; set; }

        #endregion

        #region Other Properties
        public  string ItemTypeName { get; set; }
       
        [UIHint("_ReadOnly")]
        [DisplayName("Asset Custodian")]
        public string EmpID { get; set; }
        
        //[UIHint("_ReadOnly")]
        //[DisplayName("Beneficiary Employee")]
        //public string EmployeeName { get; set; }
        public IList<SelectListItem> AssetCategoryList { get; set; }
        public IList<SelectListItem> AssetSubCategoryList { get; set; }
        public IList<SelectListItem> AssetStatusList { get; set; }
        public IList<SelectListItem> AssetConditionList { get; set; }
        public IList<SelectListItem> AssetSupplierList { get; set; }
        public IList<SelectListItem> LocationList { get; set; }
        public IList<SelectListItem> PurchaseList { get; set; }
        public IList<SelectListItem> ItemList { get; set; }

        public byte[] Attachment { get; set; }
        #endregion
    }

}