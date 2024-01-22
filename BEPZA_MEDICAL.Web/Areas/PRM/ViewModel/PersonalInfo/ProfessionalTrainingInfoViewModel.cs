using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Objects.DataClasses;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ProfessionalTrainingInfoViewModel
    {
        public ProfessionalTrainingInfoViewModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.TrainingYearList = new List<SelectListItem>();
            this.TrainingTypeList = new List<SelectListItem>();
            this.AcademicGradeList = new List<SelectListItem>();
            this.LocationList = new List<SelectListItem>();

            //this.FromDate = DateTime.Now;
            //this.ToDate = DateTime.Now;
            this.EmpTop = new EmpTop();
        }

        #region Standard Property

        public int Id { get; set; }
        public int EmployeeId { get; set; }
     

        [Required]
        [MaxLength(250)]
        [DisplayName("Course/Training Title")]
        public string TrainingTitle { get; set; }

        [DisplayName("Organized By")]
        public string OrganizedBy { get; set; }

        [Required]
        [DisplayName("Country")]
        public int CountryId { get; set; }

        [DisplayName("Period From")]
        [UIHint("_Date")]
        public DateTime? FromDate { get; set; }

        [DisplayName("Period To")]
        [UIHint("_Date")]
        public DateTime? ToDate { get; set; }

        public decimal? Duration { get; set; }

        [MaxLength(500)]
        [DisplayName("Course Detail")]
        [UIHint("_MultiLineBig")]
        public string CourseDetail { get; set; }

        [DisplayName("Training Year")]
        public string TrainingYear { get; set; }

        [DisplayName("Training Type")]
        public int? TrainingTypeId { get; set; }

        [DisplayName("Grade")]
        public int? AcademicGradeId { get; set; }

        [DisplayName("Location")]
        public int? LocationId { get; set; }

        [DisplayName("Finance by")]
        public string Financeby { get; set; }

        public string Position { get; set; }
        #endregion

        public IList<SelectListItem> CountryList { get; set; }
        public IList<SelectListItem> TrainingYearList { get; set; }
        public IList<SelectListItem> TrainingTypeList { get; set; }
        public IList<SelectListItem> AcademicGradeList { get; set; }
        public IList<SelectListItem> LocationList { get; set; }

        public EmpTop EmpTop { get; set; }       
        public string strMode { get; set; }
        public string Message { get; set; }
        public string errClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ButtonText { get; set; }
        public string SideBarClassName { get; set; }
    }
}