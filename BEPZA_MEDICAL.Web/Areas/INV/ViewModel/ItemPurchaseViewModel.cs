using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class ItemPurchaseViewModel : BaseViewModel
    {
        #region Ctor
        public ItemPurchaseViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            PurchaseTypeList = new List<SelectListItem>();
            SupplierList = new List<SelectListItem>();
            this.PurchaseItemDetail = new List<ItemPurchaseDetailViewModel>();

            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        [Required]
        public int Id { get; set; }

        [Display(Name = "MRR #")]
        [UIHint("_ReadOnly")]
        [MaxLength(50)]
        [Required]
        public string MRR { get; set; }

        [Display(Name = "P.O. #")]
        [MaxLength(50)]
        public string PO { get; set; }

        [Display(Name = "Challan #")]
        [MaxLength(50)]
        public string Challan { get; set; }

        [Display(Name = "Reference #")]
        [MaxLength(50)]
        public string Reference { get; set; }

        [Display(Name = "Purchase Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "MRR Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? MRRDate { get; set; }

        [Display(Name = "P.O. Date")]
        [UIHint("_Date")]
        public DateTime? PODate { get; set; }

        [Display(Name = "Challan Date")]
        [UIHint("_Date")]
        public DateTime? ChallanDate { get; set; }

        [Display(Name = "Purchase Type")]
        [Required]
        public int PurchaseTypeId { get; set; }

        [Display(Name = "Supplier")]
        [Required]
        public int SupplierId { get; set; }

        [Display(Name = "Received By")]
        [Required]
        public int ReceivedBy { get; set; }

        [DisplayName("Received By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string EmpId { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        public int ZoneInfoId { get; set; }

        #endregion

        #region Other

        public IList<SelectListItem> PurchaseTypeList { get; set; }
        public IList<SelectListItem> SupplierList { get; set; }
        public IList<ItemPurchaseDetailViewModel> PurchaseItemDetail { get; set; }


        [DisplayName("Item")]
        public int? ItemId { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { set; get; }

        [DisplayName("Color")]
        public int? ColorId { set; get; }

        [DisplayName("Type")]
        public int? TypeId { set; get; }

        [DisplayName("Model")]
        public int? ModelId { set; get; }

        [DisplayName("Unit")]
        public int? UnitId { set; get; }

        //[Required]
        [DisplayName("Quantity")]
        [Range(0, 99999999)]
        public decimal? Quantity { get; set; }

        //[Required]
        [DisplayName("Rate")]
        [Range(0, 9999999999999999)]
        public decimal? Rate { get; set; }

        //[Required]
        [DisplayName("Total")]
        [Range(0, 9999999999999999)]
        public decimal? TotalCost { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string CommentDetail { get; set; }

        #region Attachment
        public HttpPostedFileBase File { get; set; }

        [DisplayName("Attachment")]
        public byte[] Attachment { get; set; }

        public string FileName { get; set; }

        #endregion


        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }

        #endregion
    }

    public class ItemPurchaseSearchViewModel
    {
        #region Properties
        public string EmpID { get; set; }
        public string MRR { get; set; }
        public string PO { get; set; }
        public string Challan { get; set; }
        public string PurchaseType { get; set; }
        public string Supplier { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }
        public DateTime? PODateFrom { get; set; }
        public DateTime? PODateTo { get; set; }
        public DateTime? ChallanDateFrom { get; set; }
        public DateTime? ChallanDateTo { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
 
            if (!String.IsNullOrWhiteSpace(MRR))
                filterExpressionBuilder.Append(String.Format("MRR like {0} AND ", MRR));

            if (!String.IsNullOrWhiteSpace(PO))
                filterExpressionBuilder.Append(String.Format("PO like {0} AND ", PO));

            if (!String.IsNullOrWhiteSpace(Challan))
                filterExpressionBuilder.Append(String.Format("Challan like {0} AND ", Challan));

            if (!String.IsNullOrWhiteSpace(PurchaseType))
                filterExpressionBuilder.Append(String.Format("INV_PurchaseType.Name like {0} AND ", PurchaseType));

            if (!String.IsNullOrWhiteSpace(Supplier))
                filterExpressionBuilder.Append(String.Format("INV_SupplierInfo.SupplierName like {0} AND ", Supplier));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}