using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.INV.ViewModel
{
    public class OfflineRequisitionInfoViewModel : BaseViewModel
    {
        #region Ctor
        public OfflineRequisitionInfoViewModel()
        {
            //this.IUser = HttpContext.Current.User.Identity.Name;
            //this.IDate = DateTime.Now;

            IndentToList = new List<SelectListItem>();
            IndentByList = new List<SelectListItem>();
            this.OfflineRequisitionDetail = new List<OfflineRequisitionDetailViewModel>();
            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        public int ZoneInfoId { get; set; }
        [Required]
        public int Id { get; set; }

        [Display(Name = "Indent #")]
        [MaxLength(50)]
        public string IndentNo { get; set; }

        [Display(Name = "Indent Date")]
        [UIHint("_Date")]
        public DateTime? IndentDate { get; set; }

        [Display(Name = "Issued By")]
        [Required]
        public int IssuedByEmpId { get; set; }

        [DisplayName("Issued By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ByEmpId { get; set; }
        [Display(Name = "Name")]
        [Required]
        public int IssuedToEmpId { get; set; }
        [DisplayName("Name")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ToEmpId { get; set; }
        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        [UIHint("_ReadOnly")]
        public string Department { get; set; }
        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        #endregion

        #region Other

        public IList<SelectListItem> IndentToList { get; set; }
        public IList<SelectListItem> IndentByList { get; set; }
        public IList<OfflineRequisitionDetailViewModel> OfflineRequisitionDetail { get; set; }
        //public IList<ItemPurchaseViewModel> TempItemDetail { get; set; }


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

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string CommentDetail { get; set; }


        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }

        #endregion

    }
    public class OfflineRequisitionSearchViewModel
    {
        #region Properties
        public string ByEmpID { get; set; }
        public string IndentNo { get; set; }
        public DateTime? IndentDateFrom { get; set; }
        public DateTime? IndentDateTo { get; set; }
        public decimal TotalQuantity { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(ByEmpID))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", ByEmpID));

            if (!String.IsNullOrWhiteSpace(IndentNo))
                filterExpressionBuilder.Append(String.Format("IndentNo like {0} AND ", IndentNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}