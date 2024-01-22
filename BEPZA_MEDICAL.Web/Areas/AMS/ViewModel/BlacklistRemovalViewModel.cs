using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class BlacklistRemovalViewModel : BaseViewModel
    {
        public BlacklistRemovalViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        [Required]
        public int BlacklistId { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Removal Date")]
        public virtual System.DateTime? DateofRemoval { get; set; }

        [DisplayName("Remarks")]
        [MaxLength(200)]
        [Required]
        public string Remarks { get; set; }

        [Display(Name = "Removed By")]
        [Required]
        public int RemovedByEmpId { get; set; }

        [Display(Name = "Approved By")]
        [Required]
        public int ApprovedByEmpId { get; set; }

        [Display(Name = "Is Revoked")]
        public bool? IsRevoked { get; set; }

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
        public string RemovedBy { get; set; }

        [Required]
        [UIHint("_ReadOnly")]
        public string ApprovedBy { get; set; }

    }

}