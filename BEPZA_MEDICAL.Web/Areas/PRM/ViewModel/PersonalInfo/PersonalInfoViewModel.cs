using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;
using BEPZA_MEDICAL.Web.Utility;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalInfoViewModel
    {
        public PersonalInfoViewModel()
        {
            //this.GenderList = new List<SelectListItem>();
            this.MaritalStatusList = new List<SelectListItem>();
            this.CountryofBirthList = new List<SelectListItem>();
            this.BloodGroupList = new List<SelectListItem>();
            this.NationalityList = new List<SelectListItem>();
            this.PresentCountryList = new List<SelectListItem>();
            this.PresentDistictList = new List<SelectListItem>();
            this.PresentThanaList = new List<SelectListItem>();
            this.PermanentCountryList = new List<SelectListItem>();
            this.PermanentDistictList = new List<SelectListItem>();
            this.PermanentThanaList = new List<SelectListItem>();

            this.EmpTop = new EmpTop();

            //this.DateofBirth = DateTime.Now;
        }

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public string EmpID { get; set; }

        [UIHint("_ReadOnly")]
        public string Name { get; set; }

        [UIHint("_ReadOnly")]
        public string Designation { get; set; }

        [DisplayName("Initial")]
        [UIHint("_ReadOnly")]
        public string EmployeeInitial { get; set; }
        public byte[] Picture { get; set; }

        public int EmployeeId { get; set; }
        
        [Required]
        [DisplayName("Father's Name")]
        [MaxLength(100)]
        public string FatherName { get; set; }
        
        [Required]
        [DisplayName("Mother's Name")]
        [MaxLength(100)]
        public string MotherName { get; set; }
        
        [Required]
        [DisplayName("Date of Birth")]
        [UIHint("_ReadOnlyDate")]
        public Nullable<System.DateTime> DateofBirth { get; set; }
        
        [DisplayName("Place of Birth")]
        public string PlaceofBirth { get; set; }
        //[Required]
        //public string Gender { get; set; }

        public IList<SelectListItem> GenderList { get; set; }

        [DisplayName("Country of Birth")]
        public int CountryofBirthId { get; set; }

        public IList<SelectListItem> CountryofBirthList { get; set; }

        [DisplayName("Blood Group")]
        public int? BloodGroupId { get; set; }

        public IList<SelectListItem> BloodGroupList { get; set; }

        [DisplayName("Nationality")]
        public int NationalityId { get; set; }

        public IList<SelectListItem> NationalityList { get; set; }

        [DisplayName("National ID")]
        [MaxLength(20, ErrorMessage = "National ID will be 20 characters only.")]
        public string NationalID { get; set; }

        [DisplayName("Marital Status")]
        public int MaritalStatusId { get; set; }

        public IList<SelectListItem> MaritalStatusList { get; set; }

        [DisplayName("Marriage Date")]
        [UIHint("_Date")]
        public DateTime? MarriageDate { get; set; }

        [DisplayName("Spouse Name")]
        [MaxLength(100)]
        public string SpouseName { get; set; }

        [DisplayName("Weight (kg)")]
        //[Range(1, 500)]
        public int? Weight { get; set; }

        [DisplayName("Height (cm)")]
        //[Range(1,500)]
        public int? Hieght { get; set; }

        [MaxLength(20)]
        public string SSN { get; set; }

        [MaxLength(20)]
        public string TIN { get; set; }

        [Required]
        [DisplayName("Village/House No. & Road")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string PresentAddress1 { get; set; }

        [DisplayName("Other Address")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string PresentAddress2 { get; set; }

        [DisplayName("Country")]
        public int PresentCountryId { get; set; }

        public IList<SelectListItem> PresentCountryList { get; set; }

        [DisplayName("District")]
        public int PresentDistictId { get; set; }

        public IList<SelectListItem> PresentDistictList { get; set; }

        [DisplayName("Upazila")]
        public int? PresentThanaId { get; set; }

        public IList<SelectListItem> PresentThanaList { get; set; }

        [DisplayName("Post Office")]
        public string PresentPostName { get; set; }

        [DisplayName("Post Code")]
        [MaxLength(10)]
        public string PresentPostCode { get; set; }

        [DisplayName("Telephone No.")]
        [MaxLength(20)]
        [UIHint("_Phone")]
        public string PresentPhone { get; set; }

        [DisplayName("Ext.")]
        [MaxLength(10)]
        [UIHint("_PhoneExt")]
        public string PresentPhoneExtention { get; set; }

        [DisplayName("Mobile Phone No.")]
        [UIHint("_ReadOnly")]
        public string PresentMobNo { get; set; }

        [DisplayName("E-mail Address")]
       // [RegularExpression(@"^\w+([-+.]*[\w-]+)*@(\w+([-.]?\w+)){1,}\.\w{2,4}$")]
        [Email]
        [UIHint("_ReadOnly")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Village/House No. & Road")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string PermanentAddress1 { get; set; }

        [DisplayName("Other Address")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string PermanentAddress2 { get; set; }

        [DisplayName("Country")]
        public int PermanentCountryId { get; set; }

        public IList<SelectListItem> PermanentCountryList { get; set; }

        [DisplayName("District")]
        public int PermanentDistictId { get; set; }

        public IList<SelectListItem> PermanentDistictList { get; set; }

        [DisplayName("Upazila")]
        public int? PermanentThanaId { get; set; }

        public IList<SelectListItem> PermanentThanaList { get; set; }

        [DisplayName("Post Office")]
        public string PermanentPostName { get; set; }

        [DisplayName("Post Code")]
        [MaxLength(10)]
        public string PermanentPostCode { get; set; }

        [DisplayName("Telephone No.")]
        [MaxLength(20)]
        [UIHint("_Phone")]
        public string PermanentPhone { get; set; }

        [DisplayName("Ext.")]
        [MaxLength(10)]
        [UIHint("_PhoneExt")]
        public string PermanentPhoneExtention { get; set; }

        [DisplayName("Mobile Phone No.")]
        [MaxLength(20)]
        public string PermanentMobNo { get; set; }

        [MaxLength(200)]
        [UIHint("_MultiLine")]
        public string Specialization { get; set; }

        [DisplayName("Extracurricular Activities")]
        [MaxLength(200)]
        [UIHint("_MultiLine")]
        public string Extracurricular { get; set; }

        [UIHint("_MultiLine")]
        [DisplayName("Physical Fitness")]
        [MaxLength(200)]
        public string PhysicalFitness { get; set; }

        [MaxLength(200)]
        [UIHint("_MultiLine")]
        public string Habits { get; set; }

        [DisplayName("EOBI No.")]
        [MaxLength(20)]
        public string EOBINo { get; set; }

        [UIHint("_Date")]
        [DisplayName("EOBI Reg. Date.")]
        [UIHint("_Date")]
        public DateTime? EOBIRegDate { get; set; }

        [DisplayName("Is Smoker?")]
        public bool isSmoke { get; set; }

        [DisplayName("Electric Meter No.")]
        [MaxLength(20)]
        public string ElectricMeterNo { get; set; }

        #region New fields

        [DisplayName("Father's Profession")]
        public int? FatherProfessionId { get; set; }
        public IList<SelectListItem> FatherProfessionList { get; set; }

        [DisplayName("Mother's Profession")]
        public int? MotherProfessionId { get; set; }
        public IList<SelectListItem> MotherProfessionList { get; set; }

        [DisplayName("Identification Mark")]
        [MaxLength(100)]
        public string IdentificationMark { get; set; }

        [DisplayName("Is Police Verified")]
        public bool IsPoliceVerified { get; set; }

        [DisplayName("Is Freedom Fighter")]
        public bool IsFreedomFighter { get; set; }

        [DisplayName("Is Grand Child of Freedom Fighter")]
        public bool IsGrandChildOfFreedomFighter { get; set; }

        #endregion


        public string strMode { get; set; }

        public EmpTop EmpTop { get; set; }

        public string Message { get; set; }

        public string errClass { get; set; }

        public bool DeleteEnable { get; set; }

        public string ButtonText { get; set; }

        public string SideBarClassName { get; set; }
    }
}