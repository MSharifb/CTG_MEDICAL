using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class BlacklistViewModel : BaseViewModel
    {
        public BlacklistViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date")]
        public virtual System.DateTime? Date { get; set; }

        [DisplayName("Reason")]
        [MaxLength(200)]
        [Required]
        public string Reason { get; set; }

        [Display(Name = "Blacklisted By")]
        [Required]
        public int BlacklistedByEmpId { get; set; }

        [Display(Name = "Approved By")]
        //[Required]
        public int? ApprovedByEmpId { get; set; }

        [Required]
        [Display(Name = "Is Revoked")]
        public bool IsRevoked { get; set; }

        [Display(Name = "BEPZA ID")]
        [UIHint("_readonly")]
        [Required]
        public string BEPZAID { get; set; }

        [Display(Name = "Ansar ID")]
        [UIHint("_readonly")]
        public string AnsarId { get; set; }

        [Display(Name = "Name")]
        [UIHint("_readonly")]
        public string Name { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_readonly")]
        public string Designation { get; set; }

        [Display(Name = "Date of Joining in BEPZA")]
        [UIHint("_ReadOnlyDate")]
        public virtual System.DateTime? DateOfJoining { get; set; }

        [Display(Name = "Date of Joining in Ansar")]
        [UIHint("_ReadOnlyDate")]
        public virtual System.DateTime? AnsarJoiningDate { get; set; }

        public string CheckStatus { get; set; }

        [Required]
        [UIHint("_ReadOnly")]
        public string BlacklistedBy { get; set; }

        //[Required]
        [UIHint("_ReadOnly")]
        public string ApprovedBy { get; set; }

    }

}