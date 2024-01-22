using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipInformation
{
    public class MembershipInformationViewModel : BaseViewModel
    {
        #region Master

        #region Ctor
        public MembershipInformationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.Mode = "Create";
            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.ApproverList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Employee ID")]
        public string EmployeeCode { set; get; }

        //[Required]
        [DisplayName("Employee Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { set; get; }

        [DisplayName("Office")]
        [UIHint("_readOnly")]
        public string OfficeName { get; set; }

        [DisplayName("Department")]
        [UIHint("_readOnly")]
        public string DepartmentName { get; set; }

        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { set; get; }

        [DisplayName("Section")]
        [UIHint("_readOnly")]
        public string SectionName { get; set; }

        public string ApplicationStatus { get; set; }

        public int ToAuthorId { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        [Required]
        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { set; get; }

        [DisplayName("Joining Date")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? JoiningDate { set; get; }

        [Required]
        [DisplayName("PF Active Date")]
        [UIHint("_Date")]
        public DateTime? PermanentDate { set; get; }

        [DisplayName("Employee Category")]
        [UIHint("_readOnly")]
        public string EmployeeCategory { get; set; }

        [DisplayName("Present Pay Scale")]
        [UIHint("_readOnly")]
        public string PresentPayScale { get; set; }

        [DisplayName("Present Basic Pay")]
        [UIHint("_readOnly")]
        public decimal? PresentBasicPay { get; set; }

        [DisplayName("Nationality")]
        [UIHint("_readOnly")]
        public string Nationality { get; set; }

        [DisplayName("Date of Birth")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? DateOfBirth { get; set; }


        [DisplayName("Membership ID")]
        [Required]
        public string MembershipID { set; get; }

        [DisplayName("Present Address")]
        public string PresentAddress { set; get; }

        [DisplayName("Permanent Address")]
        public string PermanentAddress { set; get; }

        [DisplayName("Witness")]
        [UIHint("_ReadOnly")]
        public string WitnessName { set; get; }

        [DisplayName("Witness Address")]
        public string WitnessAddress { set; get; }

        [DisplayName("Witness Designation")]
        [UIHint("_ReadOnly")]
        public string WitnessDesignation { set; get; }

        [DisplayName("Membership No")]
        [UIHint("_20LengthString")]
        public string WitnessMembershipID { set; get; }


        [DisplayName("Witness")]
        [UIHint("_ReadOnly")]
        public string Witness2Name { set; get; }

        [DisplayName("Witness Address")]
        public string Witness2Address { set; get; }

        [DisplayName("Witness Designation")]
        [UIHint("_ReadOnly")]
        public string Witness2Designation { set; get; }

        [DisplayName("Membership No")]
        [UIHint("_20LengthString")]
        public string Witness2MembershipID { set; get; }

        [DisplayName("Membership Status")]
        public string MembershipStatus { get; set; }

        [DisplayName("Approval Status")]
        public int ApprovalStatusId { get; set; }

        [DisplayName("Approved Date")]
        [UIHint("_Date")]
        public DateTime? ApproveDate { get; set; }

        [DisplayName("Forward To")]
        public int? ApprovedById { get; set; }

        [DisplayName("Approval Path")]
        public int? ApprovalPathId { get; set; }

        public string PermanentPeriod { get; set; }
        public string And { get; set; }

        public string ApprovalAction { get; set; }

        public int ApprovalActionId { get; set; }

        public int PreviousStepId { get; set; }

        public string ApprovalStatus { get; set; }

        [DisplayName("Membership Applied By")]
        public string MembershipAppliedByInOffline { get; set; }

        [DisplayName("Membership Applied Date")]
        public string MembershipAppliedDateInOffline { get; set; }

        [Required]
        [DisplayName("Application Date")]
        [UIHint("_date")]
        public DateTime? ApplicationDate { get; set; }

        [DisplayName("Application Receipt Date")]
        [UIHint("_date")]
        public DateTime? ApplicationReceiptDate { get; set; }

        [DisplayName("Trustee Meeting Held On")]
        [UIHint("_date")]
        public DateTime? TrusteeMeetingHeldDate { get; set; }

        [DisplayName("PF ID")]
        public string PfId { get; set; }

        [DisplayName("Date of Membership")]
        [UIHint("_date")]
        public DateTime? DateOfMemberShip { get; set; }


        [DisplayName("A/C No.")]
        public string AccountNumber { get; set; }

        [DisplayName("Bank Name")]
        public int? BankId { get; set; }

        [DisplayName("Bank Branch")]
        public int? BankBranchId { get; set; }

        public IList<SelectListItem> BankList { get; set; }

        public IList<SelectListItem> BankBranchList { get; set; }

        [DisplayName("Bank")]
        [UIHint("_ReadOnly")]
        public string BankName { get; set; }

        [DisplayName("Bank Branch")]
        [UIHint("_ReadOnly")]
        public string BankBranchName { get; set; }

        public string SalaryWithdrawFromZoneName { get; set; }

        [DisplayName("PF Inactive Date")]
        [UIHint("_date")]
        public DateTime? InactiveDate { get; set; }

        #endregion

        #region Other properties
        public bool IsConfigurableApprovalFlow { get; set; }

        public string Mode { get; set; }
        
        public virtual List<ApplicationStatusViewModel> ApprovalPathList { get; set; }

        public string ApproverEmpId { get; set; }
        [DisplayName("Name")]
        public string ApproverName { get; set; }
        [DisplayName("Designation")]
        public string ApproverDesignation { get; set; }

        #endregion

        #endregion Master

        public IList<SelectListItem> MembershipStatusList
        {
            get
            {
                IList<SelectListItem> list = new List<SelectListItem>();
                SelectListItem li = null;

                li = new SelectListItem() { Value = BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active.ToString(), Text = BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Active.ToString() };
                list.Add(li);

                li = new SelectListItem() { Value = BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Inactive.ToString(), Text = BEPZA_MEDICAL.Web.MvcApplication.CPFMembershipStatus.Inactive.ToString() };
                list.Add(li);
                return list;

            }
            set { MembershipStatusList = value; }
        }

        public IList<SelectListItem> YearList { get; set; }

        public IList<SelectListItem> MonthList { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }

    }

    public class ApplicationStatusViewModel
    {
        public ApplicationStatusViewModel() { }

        public int StatusId { get; set; }
        public int ApprovedId { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string Comments { get; set; }
    }
}