using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class ApprovalWelfareFundInfoViewModel : BaseViewModel
    {
        #region Ctor
        public ApprovalWelfareFundInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.CycleInfoList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.WelfareFundCategoryList = new List<SelectListItem>();
            this.ActiveStatusList = new List<SelectListItem>();
            this.ApprovalWelfareFundInfoCommittee = new List<ApprovalWelfareFundInfoCommitteeViewModel>();
            this.EmployeeList = new List<ApprovalWelfareFundInfoEmployeeDetailsViewModel>();
        }
        #endregion

        #region Standard Properties

        [Required]
        [DisplayName("Ref. No.")]
        public string RefNo { get; set; }

        [Required]
        [DisplayName("Committee Name")]
        public string CommitteeName { get; set; }

        [Required]
        [DisplayName("Meeting Date")]
        [UIHint("_RequiredDate")]
        public DateTime? MeetDate { get; set; }

        [Required]
        [DisplayName("Meeting Time")]
        public string MeetTime { get; set; }

        [Required]
        [DisplayName("Meeting Place")]
        public string MeetPlace { get; set; }

        [Required]
        [DisplayName("Cycle Name")]
        public int CycleInfoId { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        [DisplayName("Fund Category")]
        public int WelfareFundCategoryId { get; set; }


        [Required]
        [DisplayName("Meeting Agenda")]
        [MaxLength(500, ErrorMessage = "Maximum length is 500.")]
        public string MeetAgenda { get; set; }
        #endregion

        #region Other Properties

        [DisplayName("Cycle Name")]
        public string CycleName { get; set; }

        public IList<SelectListItem> CycleInfoList { get; set; }

        public IList<SelectListItem> YearList { get; set; }

        [DisplayName("Welfare Fund Category")]
        public string WelfareFundCategoryName { get; set; }

        public IList<SelectListItem> WelfareFundCategoryList { get; set; }
        public IList<ApprovalWelfareFundInfoCommitteeViewModel> ApprovalWelfareFundInfoCommittee { get; set; }

        public IList<ApprovalWelfareFundInfoEmployeeDetailsViewModel> EmployeeList { get; set; }
        public string ShowRecord { get; set; }
        #endregion

        #region Committee Member

        [DisplayName("Is External")]
        public bool IsExternal { get; set; }

        public int? MemberEmployeeId { get; set; }

        [DisplayName("Employee ID")]
        public string MemberEmpId { get; set; }

        [DisplayName("Member Name")]
        public string MemberName { get; set; }

        [DisplayName("Member Designation")]
        public string MemberDesignation { get; set; }

        [DisplayName("Member Role")]
        public string MemberRole { get; set; }

        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }
        public IList<SelectListItem> ActiveStatusList { get; set; }

        [DisplayName("Sort Order")]
        public int? SortOrder { get; set; }
        #endregion

        #region budget Info

        [UIHint("_ReadOnly"), DisplayName("Financial Year")]
        public string FinancialYear { get; set; }

        [UIHint("_ReadOnly"), DisplayName("Paid Amout")]
        public decimal? PaidAmout { get; set; }

        [UIHint("_ReadOnly"),DisplayName("Balance Amout")]
        public decimal? BalanceAmout { get; set; }
        #endregion
        public string EmployeeName { get; set; }
    }

}