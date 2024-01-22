using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.SMS.ViewModel
{
    public class SecurityPersonnelEmpInfoViewModel
    {
        public SecurityPersonnelEmpInfoViewModel()
        {
            this.TitleList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            this.DistrictList = new List<SelectListItem>();
            this.StatusList = new List<SelectListItem>();
            //this.BloodGroupList = new List<SelectListItem>();
        }

        #region Standard Property

        [Required]
        public virtual int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Identity Code")]
        public virtual string BEPZAID { get; set; }

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

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public virtual string FullName { get; set; }

        [DisplayName("Father's Name")]
        [MaxLength(100)]
        public string FatherName { get; set; }

        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        [Required]
        [UIHint("_ReadOnlyDate")]
        [DisplayName("Date of Joining")]
        public Nullable<System.DateTime> DateofJoining { get; set; }

        [Required]
        [UIHint("_ReadOnlyDate")]
        [DisplayName("Date of Birth")]
        public Nullable<System.DateTime> DateofBirth { get; set; }

        [UIHint("_Date")]
        [DisplayName("Date of Promotion")]
        public Nullable<System.DateTime> DateofPromotion { get; set; }

        [DisplayName("Status")]
        [Required]
        public bool IsActive { get; set; }

        public IList<SelectListItem> StatusList { get; set; }

        [DisplayName("Inactive Date")]
        [UIHint("_Date")]
        //[UIHint("_ReadOnlyDate")]
        public virtual Nullable<System.DateTime> InactiveDate { get; set; }

        [DisplayName("Home District")]
        public int? DistrictId { get; set; }
        public IList<SelectListItem> DistrictList { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        //[DisplayName("Blood Group")]
        //public int? BloodGroupId { get; set; }

        //public IList<SelectListItem> BloodGroupList { get; set; }

        [DisplayName("Blood Group")]
        public string BloodGroup { get; set; }

        #endregion

        #region Other

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SelectedClass { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }
        public bool IsPhotoExist { get; set; }

        #endregion

    }
}