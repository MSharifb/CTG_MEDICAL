using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel.Adjustment
{
    public class AdjustmentViewModel : BaseViewModel
    {
        #region Ctor
        public AdjustmentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.AdjustmentItemDetail = new List<AdjustmentDetailViewModel>();

            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        [Required]
        public int Id { get; set; }

        [Display(Name = "Adjustment #")]
        [UIHint("_ReadOnly")]
        [MaxLength(50)]
        [Required]
        public string AdjustmentNo { get; set; }

        [Display(Name = "Adjustment Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? AdjustmentDate { get; set; }

        [Display(Name = "Adjusted By")]
        [Required]
        public int AdjustedByEmpId { get; set; }

        [DisplayName("Adjusted By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string AdjustedBy { get; set; }

        [Display(Name = "Approved By")]
        [Required]
        public int ApprovedByEmpId { get; set; }

        [DisplayName("Approved By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        public int ZoneInfoId { get; set; }

        #endregion

        #region Attachment
        public HttpPostedFileBase File { get; set; }

        [DisplayName("Attachment")]
        public byte[] Attachment { get; set; }

        public string FileName { get; set; }

        #endregion

        #region Other

        public string CheckStatus { get; set; }
        public IList<AdjustmentDetailViewModel> AdjustmentItemDetail { get; set; }


        [DisplayName("Item")]
        public int? ItemId { get; set; }

        [DisplayName("Status")]
        public int? ItemStatusId { get; set; }

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



        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }
        public IList<SelectListItem> StatusList { set; get; }

        #endregion
    }

    public class AdjustmentSearchViewModel
    {
        #region Properties
        public string AdjustedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AdjustmentNo { get; set; }
        public DateTime? AdjustmentDateFrom { get; set; }
        public DateTime? AdjustmentDateTo { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(AdjustmentNo))
                filterExpressionBuilder.Append(String.Format("AdjustmentNo like {0} AND ", AdjustmentNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}