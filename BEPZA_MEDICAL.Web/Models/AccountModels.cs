using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
//using System.Web.Mvc;
using System.Web.Security;

namespace BEPZA_MEDICAL.Web.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Enter Your Authentication Code")]
        public string AuthenticationCode { get; set; }

        public bool IsVerificationEnable { get; set; }
    }

    public class LogOnModel
    {
        #region Ctor
        public LogOnModel()
        {
            this.ZoneList = new List<PRM_ZoneInfo>();
        }
        #endregion

        [Required]
        [Display(Name = "Login ID")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool ISMultiZone { get; set; }
        public int ZoneInfoId { get; set; }
        public string ZoneName { get; set; }
        public List<PRM_ZoneInfo> ZoneList { get; set; }

        [Display(Name = "Enter Your Authentication Code")]
        public string TwoFactorCode { get; set; }
        public bool IsVerified { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMeforVerification { get; set; }


    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
