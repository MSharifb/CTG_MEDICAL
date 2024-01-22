using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FAR.ViewModel
{
    public class AssetTransferViewModel : BaseViewModel
    {
        #region Ctor
        public AssetTransferViewModel()
        {
            this.ToAssetConditionList = new List<SelectListItem>();
            this.ToLocationList = new List<SelectListItem>();
            this.ToZoneList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Porperties

        [Required]
        public int FixedAssetId { get; set; }

        [Required]
        public int AssetStatusId { get; set; }
       
        [Required,UIHint("_Date"),DisplayName("Transfer Date")]
        public DateTime? TransferDate { get; set; }

        //[Required]
        public int? FromAssetConditionId { get; set; }
       
        //[DisplayName("To Asset Condition")]
        //public int? ToAssetConditionId { get; set; }

        [Required]
        public int FromLocationId { get; set; }

        [Required,DisplayName("To Location")]
        public int ToLocationId { get; set; }

        public int? FromBeneficiaryEmployeeId { get; set; }

        public int? ToBeneficiaryEmployeeId { get; set; }

        [DisplayName("Accessories"),StringLength(500, ErrorMessage = "Accessories must be within 500 characters.")]       
        public string Accessories { get; set; }

        [DisplayName("Description"),StringLength(500, ErrorMessage = "Description must be within 500 characters.")]       
        public string Remarks { get; set; }

        [UIHint("_Date"),DisplayName("Expected Date of Return")]
        public DateTime? ExpectedDateOfReturn { get; set; }
        
        [UIHint("_Date"),DisplayName("Return Date")]
        public DateTime? ReturnDate { get; set; }

        [Required]
        [DisplayName("Issued By"), UIHint("_ReadOnly")]
        public int IssuedBy { get; set; }
        
        [Required,DisplayName("Received By"), UIHint("_ReadOnly")]
        public int ReceivedBy { get; set; }

        [DisplayName("Return By"), UIHint("_ReadOnly")]
        public int? ReturnBy { get; set; }

        [Required, DisplayName("Zone/Executive Office")]
        public int ToZoneInfoId { get; set; }

        [DisplayName("Organogram Level")]
        public int? ToOrganogramLevelId { get; set; }

        [UIHint("_ReadOnly")]
        public string ToOrganogramLevelName { get; set; }

        #endregion

        #region Other Properties
        
        [Required,DisplayName("Asset Code")]
        public string AssetCode { get; set; }
       
        [Required,DisplayName("Asset Name/Model"),UIHint("_ReadOnly")]
        public string AssetName { get; set; }

        [Required,DisplayName("Category Name"),UIHint("_ReadOnly")]       
        public string CategoryName { get; set; }
        
        [Required,UIHint("_ReadOnly"),DisplayName("Sub-Category Name")]
        public string SubCategoryName { get; set; }        
        
        [UIHint("_ReadOnly"),DisplayName("Asset Status")]
        public string AssetStatusName { get; set; }
       
        [DisplayName("From Asset Condition"),UIHint("_ReadOnly")]       
        public string FromAssetCondition { get; set; }
       
        [Required,DisplayName("From Location"),UIHint("_ReadOnly")]       
        public string FromLocation { get; set; }

        [DisplayName("From Beneficiary Employee"),UIHint("_ReadOnly")]       
        public string FromBeneficiaryEmployee { get; set; }

        [DisplayName("To Beneficiary Employee"),UIHint("_ReadOnly")]       
        public string ToBeneficiaryEmployee { get; set; }

        [Required, DisplayName("Issued By"), UIHint("_ReadOnly")]
        public string IssuedEmployeeBy { get; set; }

        [Required, DisplayName("Received By"), UIHint("_ReadOnly")]
        public string ReceivedEmployeeBy { get; set; }

        [Required, DisplayName("Return By"), UIHint("_ReadOnly")]
        public string ReturnEmployeeBy { get; set; }

        public string ToLocation { get; set; }

        public DateTime? TransferDateBetween { get; set; }
        public DateTime? TransferDateAnd { get; set; }
        public IList<SelectListItem> ToAssetConditionList { get; set; }
        public IList<SelectListItem> ToLocationList { get; set; }
        public IList<SelectListItem> ToZoneList { get; set; }
        #endregion
    }
}