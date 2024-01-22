using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class AccademicQlfnInfoViewModel
    {
        public AccademicQlfnInfoViewModel()
        {
            this.ExamLavelList = new List<SelectListItem>();
            this.YearOfPassingList = new List<SelectListItem>();
            this.SubjectGroupList = new List<SelectListItem>();
            this.ResultList = new List<SelectListItem>();
            this.UniversityAndBoardList = new List<SelectListItem>();
            this.CountryList = new List<SelectListItem>();
            EmpTop = new EmpTop();
        }

        public EmpTop EmpTop { get; set; }

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [DisplayName("Education")]
        public int DegreeLevelId { get; set; }

        public IList<SelectListItem> ExamLavelList { get; set; }

        [Required]
        [UIHint("_DropDownList")]
        [DisplayName("Division/Grade")]
        public int AcademicGradeId { get; set; }
        public IList<SelectListItem> ResultList { get; set; }

        [DisplayName("Obtained Marks (%)")]
        public decimal? ObtainMarks { get; set; }

        [DisplayName("GPA/CGPA")]
        [Range(0, 5)]
        public decimal? CGPA { get; set; }

        [Required]
        [DisplayName("Board/University ")]
        [UIHint("_DropDownList")]
        public int UniversityAndBoardId { get; set; }

        public IList<SelectListItem> UniversityAndBoardList { get; set; }

       
        //[Required]      
        [DisplayName("Principal Subject")]
        [UIHint("_DropDownList")]
        public int? SubjectGroupId { get; set; }

        public IList<SelectListItem> SubjectGroupList { get; set; }

        [DisplayName("Passing Year")]
        public string YearOfPassing { get; set; }
        public IList<SelectListItem> YearOfPassingList { get; set; }

        [DisplayName("Degree Duration (year)")]
        public decimal DegreeDuration { get; set; }


        public decimal? Scale { get; set; }

        [DisplayName("Country")]
        [UIHint("_DropDownList")]
        public int DegreeCountryId { get; set; }

        public IList<SelectListItem> CountryList { get; set; }

        [MaxLength(150)]
        [DisplayName("Academic Institution")]
        public string InstituteName { get; set; }

        [DisplayName("Special Achievement")]
        [MaxLength(200)]
        //[UIHint("_MultiLineBig")]
        public string SpecialAchievement { get; set; }

        public virtual Nullable<System.DateTime> InactiveDate { get; set; }

        public string strMode { get; set; }

        public int IsError { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }
    }
}