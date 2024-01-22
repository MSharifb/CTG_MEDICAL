using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class RecruitmentQualificationInfoViewModel
    {
        #region Ctor
        public RecruitmentQualificationInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.DegreeLevelList = new List<SelectListItem>();
            this.DegreeTypeList = new List<SelectListItem>();
            this.AcademicGradeList = new List<SelectListItem>();
            this.PrincipleSubjectList = new List<SelectListItem>();

            this.PayScaleList = new List<SelectListItem>();
            this.PostNameList = new List<SelectListItem>();
            this.OnList = new List<SelectListItem>();

            this.DesignationList = new List<SelectListItem>();
            this.RecruitmentQualificationListDetails = new List<RecruitmentQualificationDetailsViewModel>();
            this.Mode = "Create";
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        public int OrganogramLevelId { get; set; }

        public int DepartmentId { get; set; }

        public int? SectionId { get; set; }

        [Required]
        [Display(Name="Post Name")]
        public int DesignationId { get; set; }

        public bool IsNewRecruitment { get; set; }

        [Display(Name = "Minimum Age Limit")]
        public int MinAge { get; set; }

        [Display(Name = "Maximum Age Limit")]
        public int MaxAge { get; set; }

        [Display(Name = "Percentage")]
        public decimal? Percentage { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        public IList<SelectListItem> DesignationList { get; set; }
        public IList<RecruitmentQualificationDetailsViewModel> RecruitmentQualificationListDetails { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        #endregion

        #region Other

        #region Details
        public string Condition { get; set; }
        #endregion

        #region Education
        [Display(Name="Degree Type")]
        public int? RecruDegreeTypeId { get; set; }
        [Display(Name = "Degree Level")]
        public int? RecruDegreeLevelId { get; set; }
        [Display(Name = "Division/Grade")]
        public int? RecruAcademicGradeId { get; set; }
        [Display(Name = "Remarks")]
        public string RecruRemarks { get; set; }
        [Display(Name="Principle Subject")]
        public int? RecruPrincipleSubjectId { get; set; }

        [Display(Name = "Degree Type")]
        public int? ProDegreeTypeId { get; set; }
        [Display(Name = "Degree Level")]
        public int? ProDegreeLevelId { get; set; }
        [Display(Name = "Division/Grade")]
        public int? ProAcademicGradeId { get; set; }
        [Display(Name = "Remarks")]
        public string ProRemarks { get; set; }
        [Display(Name = "Principle Subject")]
        public int? ProPrincipleSubjectId { get; set; }

        public IList<SelectListItem> DegreeTypeList { get; set; }
        public IList<SelectListItem> DegreeLevelList { get; set; }
        public IList<SelectListItem> AcademicGradeList { get; set; }
        public IList<SelectListItem> PrincipleSubjectList { get; set; }


        [Display(Name = "GPA/CGPA")]
        public decimal RecruCGPA { get; set; }

        [Display(Name = "GPA/CGPA")]
        public decimal ProCGPA { get; set; }
        #endregion

        #region Job Exp
        [Display(Name="Year Of Experience")]
        public string RecruYearOfExp { get; set; }
        [Display(Name = "Pay Scale")]
        public int? RecruPayScale { get; set; }
        [Display(Name = "Post Name")]
        public int? RecruPostName { get; set; }
        [Display(Name = "On")]
        public string RecruOn { get; set; }
        [Display(Name = "Total Year Of Experience")]
        public decimal? RecruTotalYearOfExp { get; set; }

        [Display(Name = "Year Of Experience")]
        public string ProYearOfExp { get; set; }
        [Display(Name = "Pay Scale")]
        public int? ProPayScale { get; set; }
        [Display(Name = "Post Name")]
        public int? ProPostName { get; set; }
        [Display(Name = "On")]
        public string ProOn { get; set; }
        [Display(Name = "Total Year Of Experience")]
        public decimal? ProTotalYearOfExp { get; set; }

        public IList<SelectListItem> PayScaleList { get; set; }
        public IList<SelectListItem> PostNameList { get; set; }
        public IList<SelectListItem> OnList { get; set; }

        #endregion

        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string Recruitment { get; set; }
        public string Qualification { get; set; }
        public string OfficeOrSectionName { get; set; }

        public bool NewRec { get; set; }
        public bool Pro { get; set; }
        [Display (Name="Not Applicable")]
        public bool MinAgeC { get; set; }
        public bool MaxAgeC { get; set; }

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string errClass { get; set; }
        #endregion

        #region Organogram

        [Display(Name = "Organogram Level")]
        [UIHint("_ReadOnly")]
        public string OrganogramLevelName { get; set; }
        [Display(Name = "...")]
        public string OrganogramLevelDetail { get; set; }

        #endregion

    }
}