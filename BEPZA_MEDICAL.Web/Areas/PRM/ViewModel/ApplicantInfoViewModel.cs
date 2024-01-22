using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApplicantInfoViewModel : BaseViewModel
    {

        #region Ctor
        public ApplicantInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.JobAdvertisementInfoList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.PresentThanaList = new List<SelectListItem>();
            this.PresentDistictList = new List<SelectListItem>();
            this.PermanentThanaList = new List<SelectListItem>();
            this.PermanentDistictList = new List<SelectListItem>();
            this.NationalityList = new List<SelectListItem>();
            this.GenderList = new List<SelectListItem>();
            this.ReligionList = new List<SelectListItem>();
            this.QuotaNameList = new List<SelectListItem>();
            this.BankNameList = new List<SelectListItem>();
            this.BankBranchList = new List<SelectListItem>();
            this.DegreeLevelList = new List<SelectListItem>();
            this.UniversityAndBoardList = new List<SelectListItem>();
            this.SubjectGroupList = new List<SelectListItem>();
            this.AcademicGradeList = new List<SelectListItem>();
            this.PassingYearList = new List<SelectListItem>();
            this.ApplicantInfoQualification = new List<ApplicantInfoQualificationViewModel>();
        }
        #endregion

        #region Standard Property
        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        [DisplayName("Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }
        public IList<SelectListItem> JobAdvertisementInfoList { get; set; }

        [Required]
        [DisplayName("Name of the Post")]
        public int DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

        [Required]
        [DisplayName("Department")]
        public int DivisionId { get; set; }

        [Required]
        [DisplayName("Applicant Name In Bengali")]
        public string ApplicantNameB { get; set; }

        [Required]
        [DisplayName("Applicant Name In English")]
        public string ApplicantNameE { get; set; }

        [DisplayName("National ID")]
        public string NationalID { get; set; }

        [DisplayName("Birth Reg. No.")]
        public string BirthRegNo { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        [UIHint("_RequiredDate")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [DisplayName("Place of Birth")]
        public string PlaceOfBirth { get; set; }

        [Required]
        [DisplayName("Age (According to Advertisement)")]
        public int Age { get; set; }

        [Required]
        [DisplayName("Father's Name")]
        public string FatherName { get; set; }

        [Required]
        [DisplayName("Mother's Name")]
        public string MotherName { get; set; }

        [Required]
        [DisplayName("Mobile No.")]
        public string MobNo { get; set; }

        [DisplayName("Telephone No.")]
        public string TelNo { get; set; }

        [DisplayName("E-mail (If any)")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Nationality")]
        public int NationalityId { get; set; }
        public IList<SelectListItem> NationalityList { get; set; }


        [Required]
        public string Gender { get; set; }
        public IList<SelectListItem> GenderList { get; set; }

        [Required]
        [DisplayName("Religion")]
        public int ReligionId { get; set; }
        public IList<SelectListItem> ReligionList { get; set; }

        public string Profession { get; set; }

        [DisplayName("Additional Qualification (If any)")]
        public string AddQualafication { get; set; }

        [DisplayName("Total Year of Experience")]
        public decimal? YearOfExperience { get; set; }

        [DisplayName("Experience Details")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string ExperienceDetails { get; set; }

        [DisplayName("Quota")]
        public int? QuotaNameId { get; set; }
        public IList<SelectListItem> QuotaNameList { get; set; }

        [DisplayName("Challan/Bank Draft/Pay Order No.")]
        public string BankDraftNo { get; set; }

        [UIHint("_Date")]
        [DisplayName("Date")]
        public DateTime? BankDraftDate { get; set; }

        [DisplayName("Bank")]
        public int? BankNameId { get; set; }
        public IList<SelectListItem> BankNameList { get; set; }

        [DisplayName("Branch")]
        public int? BankBranchId { get; set; }
        public IList<SelectListItem> BankBranchList { get; set; }

        [DisplayName("Departmental Candidate ?")]
        public string CandidateType { get; set; }

        [Required]
        [DisplayName("Application Date")]
        [UIHint("_RequiredDate")]
        public DateTime? ApplicationDate { get; set; }

        public bool IsSubmit { get; set; }

        #region Address

        [Required]
        [DisplayName("House & Road (Name/No.)")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string PresentAddress { get; set; }

        [DisplayName("Village/Area")]
        public string PresentVillage { get; set; }

        [DisplayName("Union/Ward")]
        public string PresentUnion { get; set; }

        [DisplayName("Post Office")]
        public string PresentPostName { get; set; }

        [DisplayName("Post Code")]
        [MaxLength(20)]
        public string PresentPostCode { get; set; }

        [DisplayName("Upazila")]
        public int? PresentThanaId { get; set; }
        public IList<SelectListItem> PresentThanaList { get; set; }

        [DisplayName("District")]
        public int? PresentDistictId { get; set; }

        public IList<SelectListItem> PresentDistictList { get; set; }


        [Required]
        [DisplayName("House & Road (Name/No.)")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string PermanentAddress { get; set; }

        [DisplayName("Village/Area")]
        public string PermanentVillage { get; set; }

        [DisplayName("Union/Ward")]
        public string PermanentUnion { get; set; }

        [DisplayName("Post Office")]
        public string PermanentPostName { get; set; }

        [DisplayName("Post Code")]
        [MaxLength(20)]
        public string PermanentPostCode { get; set; }


        [DisplayName("Upazila")]
        public int? PermanentThanaId { get; set; }
        public IList<SelectListItem> PermanentThanaList { get; set; }

        [DisplayName("District")]
        public int? PermanentDistictId { get; set; }
        public IList<SelectListItem> PermanentDistictList { get; set; }

        #endregion

        #region Applicant Photo

        [Display(Name = "Picture")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        #endregion

        #region Applicant Signature

        [Display(Name = "Applicant e-Singnature")]
        public bool IsAddSingnatureAttachment { set; get; }
        public byte[] SingnatureAttachment { set; get; }

        public HttpPostedFileBase SingnatureFile { get; set; }
        public string SingnatureFileName { get; set; }
        public string SingnatureFilePath { get; set; }
        #endregion        

        #endregion

        #region Others

        [DisplayName("Application Start Date")]
        public string StartDateOfApplication { get; set; }

        [DisplayName("Application End Date")]
        public string LastDateOfApplication { get; set; }

        public IList<ApplicantInfoQualificationViewModel> ApplicantInfoQualification { get; set; }

        public int? ApplicantInfoId { get; set; }

        [DisplayName("Examination")]
        public int? DegreeLevelId { get; set; }
        public IList<SelectListItem> DegreeLevelList { get; set; }

        [DisplayName("Board/University")]
        public int? UniversityAndBoardId { get; set; }
        public IList<SelectListItem> UniversityAndBoardList { get; set; }

        [DisplayName("Principal Subject")]
        public int? SubjectGroupId { get; set; }
        public IList<SelectListItem> SubjectGroupList { get; set; }

        [DisplayName("Grade/Class/Division")]
        public int? AcademicGradeId { get; set; }
        public IList<SelectListItem> AcademicGradeList { get; set; }

        [DisplayName("Passing Year")]
        public int? PassingYear { get; set; }
        public IList<SelectListItem> PassingYearList { get; set; }

        [DisplayName("Academic Institution")]
        public string AcademicInstName { get; set; }

        [DisplayName("GPA/CGPA")]
        public decimal? CGPA { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }

        [DisplayName("Advertisement Date")]
        public string AdJobPostAgeCalDate { get; set; }

        public string AdCode { get; set; }
        public string DesignationName { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Status { get; set; }
        public string ZoneName { get; set; }
        public string ZoneAddress { get; set; }
        public string RollNo { get; set; }
        #endregion

        #region For e-Recruitment
        public bool SSC { get; set; }
        public string SscExamName { get; set; }
        public int SscBoardId { get; set; }
        public int SscGroupId { get; set; }
        public string SscResult { get; set; }

        public bool HSC { get; set; }
        public string HscExamName { get; set; }
        public int HscBoardId { get; set; }
        public int HscGroupId { get; set; }
        public string HscResult { get; set; }
        #endregion
    }
}