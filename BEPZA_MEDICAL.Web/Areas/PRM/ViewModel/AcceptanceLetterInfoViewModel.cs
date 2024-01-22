using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class AcceptanceLetterInfoViewModel:BaseViewModel
    {
        #region Ctor
        public AcceptanceLetterInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

        }
        #endregion

        #region Standard Property

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ApplicantInfoId { get; set; }

        [Required]
        [DisplayName("Selected Id")]
        public int? SelectedId { get; set; }

        [Required]
        [UIHint("_RequiredDate")]
        [DisplayName("Date of Acceptance")]
        public DateTime? DateOfAcceptance { get; set; }

        [Required]
        [UIHint("_RequiredDate")]
        [DisplayName("Date of Joining")]
        public DateTime? DateOfJoining { get; set; }

        [Required]
        [DisplayName("Ref. No.")]
        public string RefNo { get; set; }

        [DisplayName("Is Medical Statement Verified?")]
        public bool IsMedicalVerified { get; set; }

        [DisplayName("Is Wealth Statement Verified?")]
        public bool IsWealthVerified { get; set; }

        [DisplayName("Is Police Verification Document Verified?")]
        public bool IsPoliceVerified { get; set; }

        [MaxLength(200)]
        [UIHint("_Multiline")]
        public string Remarks { get; set; }

        [Required]
        [MaxLength(500)]
        [UIHint("_Multiline")]
        public string Subject { get; set; }


        [Required]
        [UIHint("_Multiline")]
        public string Body { get; set; }

        [UIHint("_Multiline")]
        public string CC { get; set; }
        #endregion

        #region Other

        [Required]
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }

        [Required]
        [Display(Name = "Signature By")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        public int? DesignationId { get; set; }

        [Display(Name = "Status Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        [Display(Name = "Applicant Name")]
        public string ApplicantName { get; set; }
       
        [Display(Name = "Position")]
        public string ApplicantPosition { get; set; }
        #endregion

    }
}