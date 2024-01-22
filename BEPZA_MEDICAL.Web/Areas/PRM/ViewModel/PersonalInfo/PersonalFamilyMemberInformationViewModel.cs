using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class PersonalFamilyMemberInformationViewModel
    {
        public PersonalFamilyMemberInformationViewModel()
        {
            this.BloodGroupList = new List<SelectListItem>();
            this.DegreeLevelList = new List<SelectListItem>();
            this.MaritalStatusList = new List<SelectListItem>();
            this.ProfessionList = new List<SelectListItem>();
            this.RelationList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            this.GenderList = new List<SelectListItem>();
            this.NationalityList = new List<SelectListItem>();
            this.PersonOnBehalfList = new List<SelectListItem>();
            EmpTop = new EmpTop();
        }
        public EmpTop EmpTop { get; set; }

        #region Basic informaition

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

        #endregion

        public int Id
        {
            get;
            set;
        }

        public int EmployeeId
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Relation")]
        public int RelationId
        {
            get;
            set;
        }
        public IList<SelectListItem> RelationList { get; set; }

        [DisplayName("Is Dependent?")]
        public bool isDependent
        {
            get;
            set;
        }

        [DisplayName("Title")]
        public int? TitleId
        {
            get;
            set;
        }
        public IList<SelectListItem> TitleList { get; set; }

        [DisplayName("First Name")]
        [Required]
        public string FirstName
        {
            get;
            set;
        }

        [DisplayName("Middle Name")]
        public string MiddleName
        {
            get;
            set;
        }

        [DisplayName("Last Name")]
        public string LastName
        {
            get;
            set;
        }

        [DisplayName("Full Name")]
        [Required]
        [UIHint("_ReadOnly")]
        public string FullName
        {
            get;
            set;
        }

        public decimal Age
        {

            get
            {
                if (this.DateofBirth != null)
                {
                    // get the difference in years
                    int years = DateTime.Now.Year - ((DateTime)(this.DateofBirth)).Year;
                    // subtract another year if we're before the
                    // birth day in the current year
                    if (DateTime.Now.Month < ((DateTime)(this.DateofBirth)).Month ||
                        (DateTime.Now.Month == ((DateTime)(this.DateofBirth)).Month &&
                        DateTime.Now.Day < ((DateTime)(this.DateofBirth)).Day))
                        years--;
                    return years;
                }
                else
                    return 0;
            }

        }

        [DisplayName("Date of Birth")]
        [Required]
        [UIHint("_Date")]
        public DateTime? DateofBirth
        {
            get;
            set;
        }

        [Required]
        public string Gender
        {
            get;
            set;
        }
        public IList<SelectListItem> GenderList { get; set; }


        [DisplayName("Marital Status")]
        //[Required]
        public int? MaritalStatusId
        {
            get;
            set;

        }
        public IList<SelectListItem> MaritalStatusList { get; set; }

        [DisplayName("Occupation")]
        //[Required]
        public int? ProfessionId { get; set; }

        public IList<SelectListItem> ProfessionList { get; set; }

        [DisplayName("Academic Qualification")]
        public int? DegreeLevelId
        {
            get;
            set;
        }
        public IList<SelectListItem> DegreeLevelList { get; set; }

        [DisplayName("Blood Group")]
        public int? BloodGroupId
        {
            get;
            set;
        }
        public IList<SelectListItem> BloodGroupList { get; set; }

        [DisplayName("Nationality")]
        [Required]
        public int? NationalityId
        {
            get;
            set;
        }
        public IList<SelectListItem> NationalityList { get; set; }

        [DisplayName("Contact No.")]
        public string ContractNo
        {
            get;
            set;
        }

        [DisplayName("Fax No.")]
        public string FaxNo
        {
            get;
            set;
        }

        [DisplayName("National ID")]
        public string NationalID
        {
            get;
            set;
        }

        [DisplayName("Email Address")]
        [RegularExpression(@"^\w+([-+.]*[\w-]+)*@(\w+([-.]?\w+)){1,}\.\w{2,4}$")]
        public string EmailAddress
        {
            get;
            set;
        }

        [DisplayName("Present Address")]
        [MaxLength(200)]
        public string PresentAddress
        {
            get;
            set;
        }

        [DisplayName("Permanent Address")]
        [MaxLength(200)]
        public string PermanentAddress
        {
            get;
            set;
        }

        [DisplayName("Add Photo?")]
        public bool isAddPhoto
        {
            get;
            set;
        }

        public byte[] Photo
        {
            get;
            set;
        }

        public string ImageUrl { get; set; }
        public string ImageAltText { get; set; }
        [DisplayName("Nominee Bank Name")]
        public string NomineeBankName { get; set; }

        [DisplayName("Nominee Branch Name")]
        public string NomineeBranchName { get; set; }

        [DisplayName("Nominee Account No.")]
        public string NomineeAccountNo { get; set; }

        [DisplayName("Is Nominee Immature")]
        public bool isNomineeImmature { get; set; }
        [DisplayName("Is Nominee Suffering from legal disability")]
        public bool isNomineeDisable { get; set; }
        [DisplayName("Person on Behalf")]
        public int? PersonOnBehalfId { get; set; }
        public List<SelectListItem> PersonOnBehalfList { get; set; }

        [DisplayName("Nominee Address")]
        [MaxLength(200)]
        [UIHint("_MultiLine")]
        public string NomineeAddress { get; set; }
        //[MaxLength(200)]
        //[UIHint("_MultiLine")]
        //public string Witness { get; set; }
        public int? Witness1EmpId { get; set; }
        public int? Witness2EmpId { get; set; }
        [DisplayName("Witness ID")]
        public string Witness1Id { get; set; }
        [DisplayName("Witness ID")]
        public string Witness2Id { get; set; }
        [DisplayName("Name")]
        public string Witness1Name { get; set; }
        [DisplayName("Name")]
        public string Witness2Name { get; set; }
        [DisplayName("Address")]
        public string Witness1Address { get; set; }
        [DisplayName("Address")]
        public string Witness2Address { get; set; }
        [DisplayName("Designation")]
        public string Witness1Designation { get; set; }
        [DisplayName("Designation")]
        public string Witness2Designation { get; set; }
        [MaxLength(200)]
        [UIHint("_MultiLine")]
        public string Remarks { get; set; }

        public string Message { get; set; }
        public bool IsSuccessful { get; set; }

        //public string Action { get; set; }

        public string SideBarClassName { get; set; }
        public string strMode { get; set; }

        public string ActionType { get; set; }
    }
}