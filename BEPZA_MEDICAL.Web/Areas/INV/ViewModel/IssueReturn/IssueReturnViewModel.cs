using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel.IssueReturn
{
    public class IssueReturnViewModel : BaseViewModel
    {
        #region Ctor
        public IssueReturnViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            ScrapList = new List<SelectListItem>();
            this.IssueReturnItemDetail = new List<IssueReturnDetailViewModel>();

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

        [Display(Name = "Return #")]
        [UIHint("_ReadOnly")]
        [MaxLength(50)]
        [Required]
        public string ReturnNo { get; set; }

        [Display(Name = "Return Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Scrap #")]
        [Required]
        public int ScrapId { get; set; }

        [Display(Name = "Received From")]
        [Required]
        public int ReceivedFromEmpId { get; set; }

        [DisplayName("Received From")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ReceivedFrom { get; set; }

        [Display(Name = "Received By")]
        [Required]
        public int ReceivedByEmpId { get; set; }

        [DisplayName("Received By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ReceivedBy { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        public int ZoneInfoId { get; set; }

        #endregion

        #region Other

        public string CheckStatus { get; set; }
        public IList<IssueReturnDetailViewModel> IssueReturnItemDetail { get; set; }


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

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string CommentDetail { get; set; }


        public IList<SelectListItem> ScrapList { get; set; }
        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }
        public IList<SelectListItem> StatusList { set; get; }

        #endregion
    }

    public class IssueReturnSearchViewModel : BaseViewModel
    {
        #region Properties
        public string ReceivedFrom { get; set; }
        public string ReceivedBy { get; set; }
        public string ReturnNo { get; set; }
        public DateTime? ReturnDateFrom { get; set; }
        public DateTime? ReturnDateTo { get; set; }

        public string ScrapNo { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(ReturnNo))
                filterExpressionBuilder.Append(String.Format("ReturnNo like {0} AND ", ReturnNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}