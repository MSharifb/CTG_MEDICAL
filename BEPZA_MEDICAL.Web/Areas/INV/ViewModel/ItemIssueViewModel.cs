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
    public class ItemIssueViewModel : BaseViewModel
    {
        #region Ctor
        public ItemIssueViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            IndentList = new List<SelectListItem>();
            this.IssueItemDetail = new List<ItemIssueDetailViewModel>();

            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
            MRRList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        //[Required]
        //public int Id { get; set; }

        [Display(Name = "Issue #")]
        [UIHint("_ReadOnly")]
        [MaxLength(50)]
        [Required]
        public string IssueNo { get; set; }

        [Display(Name = "Issue Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Indent Date")]
        [UIHint("_Date")]
        public DateTime? IndentDate { get; set; }

        [Display(Name = "Indent #")]
        [Required]
        public int IndentId { get; set; }

        [Display(Name = "Issued To")]
        [Required]
        public int IssuedToId { get; set; }

        [DisplayName("Issued To")]
        [Required]
        [UIHint("_ReadOnly")]
        public string IssuedTo { get; set; }

        [Display(Name = "Issued By")]
        [Required]
        public int IssuedById { get; set; }

        [DisplayName("Issued By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string IssuedBy { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        public int ZoneInfoId { get; set; }

        #endregion

        #region Other

        public string CheckStatus { get; set; }
        public IList<SelectListItem> IndentList { get; set; }
        public IList<ItemIssueDetailViewModel> IssueItemDetail { get; set; }


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

        [Display(Name = "MRR#")]
        public int? PurchaseId { get; set; }

        [DisplayName("Quantity")]
        [Range(0, 99999999)]
        public decimal? IssueQuantity { get; set; }

        [Range(0, 99999999)]
        public decimal? DemandQuantity { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string CommentDetail { get; set; }



        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }
        public IList<SelectListItem> MRRList { get; set; }

        #endregion
    }

    public class ItemIssueSearchViewModel:BaseViewModel
    {
        #region Properties
        public string IssuedTo { get; set; }
        public string IssuedBy { get; set; }
        public string IssueNo { get; set; }
        public string IndentNo { get; set; }
        public DateTime? IssueDateFrom { get; set; }
        public DateTime? IssueDateTo { get; set; }
        public DateTime? IndentDateFrom { get; set; }
        public DateTime? IndentDateTo { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(IssueNo))
                filterExpressionBuilder.Append(String.Format("IssueNo like {0} AND ", IssueNo));

            if (!String.IsNullOrWhiteSpace(IndentNo))
                filterExpressionBuilder.Append(String.Format("INV_RequisitionInfo.IndentNo like {0} AND ", IndentNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}