using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipApplication
{
    public class MembershipApplicationViewModel
    {
        #region Master
        #region Ctor
        public MembershipApplicationViewModel()
        { 
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            //this.MemberNomineeList = new List<MemberNominee>();      

            this.Mode = "Create";
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Employee Code")]
        public string EmployeeCode { set; get; }

        [Required]
        [DisplayName("Employee Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { set; get; }

        [Required]
        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string Designation { set; get; }


        [Required]
        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { set; get; }

        [Required]
        [DisplayName("Father's Name")]
        public string FatherName { set; get; }

        [Required]
        [DisplayName("Mother's Name")]
        public string MotherName { set; get; }

        [Required]
        [DisplayName("Joining Date")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? JoiningDate { set; get; }

        [Required]
        [DisplayName("Permanent Date")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? PermanentDate { set; get; }

        [Required]
        [DisplayName("Contract No")]
        [UIHint("_20LengthString")]
        public string ContractNo { set; get; }

        [Required]
        [DisplayName("National ID")]
        [UIHint("_20LengthString")]
        public string NationalID { set; get; }

        [Required]
        [DisplayName("TIN")]
        [UIHint("_20LengthString")]
        public string TIN { set; get; }

        //[Required]
        [DisplayName("Membership ID")]
        [UIHint("_20LengthString")]
        [UIHint("_ReadOnly")]
        public string MembershipID { set; get; }

        [Required]
        [DisplayName("Present Address")]
        public string PresentAddress { set; get; }

        [Required]
        [DisplayName("Permanent Address")]
        public string PermanentAddress { set; get; }

        [Required]
        [DisplayName("Name")]
        public string WitnessName { set; get; }

        [Required]
        [DisplayName("Witness Address")]
        public string WitnessAddress { set; get; }

        [Required]
        [DisplayName("Membership No")]
        [UIHint("_20LengthString")]
        public string WitnessMembershipID { set; get; }

        [Required]
        [DisplayName("Persion-1")]
        public string NomimneeWitnessName1 { set; get; }

        [Required]
        [DisplayName("Address")]
        public string NomineeWitnessAddress1 { set; get; }

        [Required]
        [DisplayName("Membership No")]
        [UIHint("_20LengthString")]
        public string NomineeWitnessMembershipID { set; get; }

        [Required]
        [DisplayName("Persion-2")]
        public string NomimneeWitnessName2 { set; get; }

        [Required]
        [DisplayName("Address")]
        public string NomineeWitnessAddress2 { set; get; }

        [Required]
        [DisplayName("Membership No")]
        [UIHint("_20LengthString")]
        public string NomineeWitnessMembershipID2 { set; get; }

        [DisplayName("Membership Status")]
        public string MembershipStatus { get; set; }

        [Required]
        [DisplayName("Approval Status")]
        public int ApprovalStatusId { get; set; }

        [DisplayName("Approved Date")]
        public DateTime? ApproveDate { get; set; }

        [DisplayName("Approval Path")]
        public int? ApprovalPathId { get; set; }

        public string PermanentPeriod { get; set; }
        public string And { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion 

        #region Other properties

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        public string strMessage { get; set; }
        public string errClass { get; set; }

        [Required(ErrorMessage = "Please Provide at least nominee.")]
        public virtual ICollection<MemberNomineeApplication> MemberNomineeApplicationList { get; set; }

        public virtual List<ApplicationStatusViewModel> ApprovalPathList { get; set; }

        #endregion 

        #endregion Master         
    }

    public class ApplicationStatusViewModel
    {
        public ApplicationStatusViewModel()
        {

        }
        public int StatusId { get; set; }
        public int ApprovedId { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }

        public string ApprovedDate { get; set; }

        public string Comments { get; set; }

       

    }
}