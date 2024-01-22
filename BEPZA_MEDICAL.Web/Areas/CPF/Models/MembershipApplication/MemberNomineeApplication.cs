using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Models.MembershipApplication
{
    public class MemberNomineeApplication
    {

        public MemberNomineeApplication()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.Mode = "Create";
        }

        #region Standard Property

        public int Id { get; set; }


        public int MembershipId { get; set; } 

        [Required]
        [DisplayName("NomineeName")]
        public string NomineeName { set; get; }

        [Required]
        [DisplayName("Nominee Address")]
        public string NomineeAddress { set; get; }

        [Required]
        [DisplayName("Relationship")]
        public string Relationship { set; get; }

        [Required]
        [DisplayName("Date Of Birth")]
        [UIHint("_Date")]
        public DateTime? DateOfBirth { set; get; }

        [Required]
        [DisplayName("Percent Of Share")]
        [UIHint("_OnlyFiveLengthDecimalNumber")]
        public Decimal? PercentOfShare { set; get; }

        [DisplayName("Guardian Name")]
        public string GuardianName { set; get; }

        [DisplayName("Guardian Address")]
        public string GuardianAddress { set; get; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public string Mode { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion
    }
}