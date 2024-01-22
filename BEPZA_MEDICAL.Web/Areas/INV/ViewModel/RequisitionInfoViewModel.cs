using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
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
    public class RequisitionInfoViewModel : BaseViewModel
    {
        #region Ctor
        public RequisitionInfoViewModel()
        {
            //this.IUser = HttpContext.Current.User.Identity.Name;
            //this.IDate = DateTime.Now;

            
            this.RequisitionDetail = new List<RequisitionDetailViewModel>();
            CategoryList = new List<SelectListItem>();
            ColorList = new List<SelectListItem>();
            ItemTypeList = new List<SelectListItem>();
            ModelList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
            this.ForwardToList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property
        public int ZoneInfoId { get; set; }

        [Display(Name = "Indent #")]
        [MaxLength(50)]
        public string IndentNo { get; set; }

        [Display(Name = "Indent Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? IndentDate { get; set; }

        [Display(Name = "Requisition By")]
        [Required]
        public int IssuedByEmpId { get; set; }

        [DisplayName("Requisition By")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ByEmpId { get; set; }
        [Display(Name = "Employee ID")]
        [Required]
        public int IssuedToEmpId { get; set; }
        [DisplayName("Name")]
        [Required]
        [UIHint("_ReadOnly")]
        public string ToEmpId { get; set; }

        [Display(Name = "Employee ID")]
        public string EmpID { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string Designation { get; set; }

        [Display(Name = "Department")]
        [UIHint("_ReadOnly")]
        public string Department { get; set; }
        [Display(Name = "Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }
        [DisplayName("Total Quantity")]
        public decimal TotalQuantity { get; set; }
        public bool IsOnline { get; set; }
        [Display(Name = "Joining Date")]
        [UIHint("_ReadOnly")]
        public string JoiningDate { get; set; }
        [Display(Name = "Confirmation Date")]
        [UIHint("_ReadOnly")]
        public string ConfirmationDate { get; set; }
        [Display(Name = "Photograph")]
        [UIHint("_ReadOnly")]
        public byte[] EmpImg { get; set; }
        public bool IsPhotoExist { get; set; }
        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        public EmployeePhotoGraphViewModel EmployeePhotograph { get; set; }

        [Display(Name = "Forward To")]
        public int ForwardToEmpId { get; set; }

        public string ForwardToEmployeeId { get; set; }
        [Display(Name = "Name")]
        public string ForwardToEmpName { get; set; }
        [Display(Name = "Designation")]
        public string ForwardToEmpDesignation { get; set; }

        public int ApprovalStatusId { get; set; }
        [DisplayName("Is Issued")]
        public bool IsProcessed { get; set; }
        #endregion

        #region Other
        public bool IsConfigurableApprovalFlow { get; set; }

        public IList<RequisitionDetailViewModel> RequisitionDetail { get; set; }
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
        public IList<SelectListItem> ForwardToList { set; get; }

        public string IssuedTo { get; set; }
        public string Status { get; set; }
        public DateTime? IndentDateFrom { get; set; }
        public DateTime? IndentDateTo { get; set; }
        public string FullName { get; set; }

        #endregion

    }
    public class RequisitionSearchViewModel
    {
        #region Properties
        public string ByEmpID { get; set; }
        public string ToEmpID { get; set; }
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

            if (!String.IsNullOrWhiteSpace(ToEmpID))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", ToEmpID));

            if (!String.IsNullOrWhiteSpace(IndentNo))
                filterExpressionBuilder.Append(String.Format("IndentNo like {0} AND ", IndentNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}