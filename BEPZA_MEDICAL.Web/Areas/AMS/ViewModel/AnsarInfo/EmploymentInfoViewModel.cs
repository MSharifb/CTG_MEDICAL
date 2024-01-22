using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class EmploymentInfoViewModel
    {
        public EmploymentInfoViewModel()
        {
            this.GenderList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.CategoryList = new List<SelectListItem>();
            this.StatusList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            this.ReligionList = new List<SelectListItem>();
            this.DistrictList = new List<SelectListItem>();
            this.DateofJoining = DateTime.UtcNow;
        }

        #region Standard Property

        [Required]
        public virtual int Id { get; set; }

        [Required]
        [DisplayName("BEPZA ID")]
        public virtual string BEPZAID { get; set; }

        [Required]
        [DisplayName("Ansar ID")]
        public virtual string AnsarId { get; set; }

        [DisplayName("Title")]
        public int? TitleId { get; set; }
        public IList<SelectListItem> TitleList { get; set; }

        [DisplayName("First Name")]
        [MaxLength(50)]
        public virtual string FirstName { get; set; }

        [DisplayName("Middle Name")]
        [MaxLength(50)]
        public virtual string MiddleName { get; set; }

        [DisplayName("Last Name")]
        [MaxLength(50)]
        public virtual string LastName { get; set; }

        [DisplayName("Full Name (Bangla)")]
        public string FullNameBangla { get; set; }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public virtual string FullName { get; set; }

        [Required]
        [DisplayName("Designation")]
        public virtual int? DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

        [Required]
        [DisplayName("Ansar Category")]
        public virtual int? CategoryId { get; set; }
        public IList<SelectListItem> CategoryList { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date of Joining in BEPZA")]
        public virtual System.DateTime DateofJoining { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date of Birth")]
        public Nullable<System.DateTime> DateofBirth { get; set; }

        [UIHint("_Date")]
        [DisplayName("Date of Joining in Ansar")]
        public Nullable<System.DateTime> AnsarJoiningDate { get; set; }

        [DisplayName("Gross Salary")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Salary must be upto 9999999999999999.99.")]
        public decimal? Salary { get; set; }

        
        [DisplayName("Mobile")]
        [MaxLength(50)]
        public virtual string MobileNo { get; set; }

        [Required]
        [DisplayName("Status")]
        public virtual int StatusId { get; set; }

        [DisplayName("Status")]
        [UIHint("_ReadOnly")]
        public string StatusName { get; set; }

        public IList<SelectListItem> StatusList { get; set; }

        [DisplayName("Inactive Date")]
        [UIHint("_Date")]
        //[UIHint("_ReadOnlyDate")]
        public virtual Nullable<System.DateTime> InactiveDate { get; set; }

        [Required]
        public string Gender { get; set; }
        public IList<SelectListItem> GenderList { get; set; }

        [DisplayName("Home District")]
        public int? DistrictId { get; set; }
        public IList<SelectListItem> DistrictList { get; set; }

        [DisplayName("Religion")]
        public int? ReligionId { get; set; }
        public IList<SelectListItem> ReligionList { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        [DisplayName("Duration")]
        [UIHint("_ReadOnly")]
        public string Duration { get; set; }

        #endregion

        #region Other

        public string DesignationName { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SelectedClass { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }
        public bool IsPhotoExist { get; set; }

        public bool IsAnsarEditDesignation { get; set; }

        #endregion

    }
}