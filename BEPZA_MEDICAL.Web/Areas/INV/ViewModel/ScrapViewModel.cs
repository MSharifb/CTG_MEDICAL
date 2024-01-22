using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval;
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
    public class ScrapViewModel : BaseViewModel
    {
        #region Ctor
        public ScrapViewModel()
        {
            //this.IUser = HttpContext.Current.User.Identity.Name;
            //this.IDate = DateTime.Now;


            this.ScrapDetail = new List<ScrapDetailViewModel>();
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
        public int ZoneInfoId { get; set; }
        [Required]
        public int Id { get; set; }

        [Display(Name = "Return Request #")]
        [MaxLength(50)]
        public string ScrapNo { get; set; }

        [Display(Name = "Return Request Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? ScrapDate { get; set; }

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

        [DisplayName("Forward To")]
        [Required]
        public int ApproverId { get; set; }

        public int? ApprovalStatusId { get; set; }



        #endregion

        #region Other
        public bool IsConfigurableApprovalFlow { get; set; }

        public IList<ScrapDetailViewModel> ScrapDetail { get; set; }
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

        [DisplayName("Status")]
        public int? ItemStatusId { get; set; }

        public int ApprovalStepId { get; set; }
        public IList<SelectListItem> ItemList { set; get; }
        public IList<SelectListItem> CategoryList { set; get; }
        public IList<SelectListItem> ColorList { set; get; }
        public IList<SelectListItem> ItemTypeList { set; get; }
        public IList<SelectListItem> ModelList { set; get; }
        public IList<SelectListItem> UnitList { set; get; }

        public IList<SelectListItem> StatusList { set; get; }

        public IList<SelectListItem> ApproverList { set; get; }

        public List<ApprovalHistoryViewModel> ApprovalHistory { get; set; }

        [Display(Name = "Designation")]
        public string ForwardToEmpDesignation { get; set; }
        public string ForwardToEmpId { get; set; }

        #endregion

    }
    public class ScrapSearchViewModel
    {
        #region Properties
        public string ByEmpID { get; set; }
        public string ToEmpID { get; set; }
        public string ScrapNo { get; set; }
        public DateTime? ScrapDateFrom { get; set; }
        public DateTime? ScrapDateTo { get; set; }
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

            if (!String.IsNullOrWhiteSpace(ScrapNo))
                filterExpressionBuilder.Append(String.Format("IndentNo like {0} AND ", ScrapNo));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}