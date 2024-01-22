using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class RecruitmentQualificationDetailsViewModel : BaseViewModel
    {
        #region Ctor
        public RecruitmentQualificationDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.DegreeLevelList = new List<SelectListItem>();
            this.DegreeTypeList = new List<SelectListItem>();
            this.AcademicGradeList = new List<SelectListItem>();
            this.SubjectGroupList = new List<SelectListItem>();
            this.RecruitmentQualificationEducationList = new List<RecruitmentQualificationEducationInfoViewModel>();
            this.RecruitmentQualificationJobExpList = new List<RecruitmentQualificationJobExpInfoViewModel>();

            this.PayScaleList = new List<SelectListItem>();
            this.PostNameList = new List<SelectListItem>();
            this.OnList = new List<SelectListItem>();
            this.ProfessionalCertificateList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        public int RecruitmentId { get; set; }
        public string Condition { get; set; }
        public string OrganogramLevelName { get; set; }
        public string DesignationName { get; set; }
        #endregion

        #region Other
        [Display(Name = "Degree Type")]
        public int? DegreeTypeId { get; set; }
        [Display(Name = "Degree Level")]
        public int? DegreeLevelId { get; set; }
        [Display(Name = "Division/Grade")]
        public int? DivisionOrGradeId { get; set; }
        [Display(Name = "GPA/CGPA")]
        public decimal? GPA { get; set; }
        public int[] SubjectOrGroupId { get; set; }

        [Display(Name = "Pay Scale")]
        public int? JobGradeId { get; set; }
        [Display(Name = "Post")]
        public int? PostId { get; set; }
        [Display(Name = "Year of Experience")]
        public decimal? YearOfExp { get; set; }
        [Display(Name = "Professional Certificate")]
        public int? ProfessionalCertificateId { get; set; }
        [Display(Name = "Total Year of Experience")]
        public decimal? TotalYearOfExp { get; set; }
        [Display(Name = "On")]
        public string OnBy { get; set; }
        public string Remarks { get; set; }


        public IList<SelectListItem> DegreeTypeList { get; set; }
        public IList<SelectListItem> DegreeLevelList { get; set; }
        public IList<SelectListItem> AcademicGradeList { get; set; }
        [Display(Name = "Subject/Group")]
        public IList<SelectListItem> SubjectGroupList { get; set; }
        public IList<RecruitmentQualificationEducationInfoViewModel> RecruitmentQualificationEducationList { get; set; }
        public IList<RecruitmentQualificationJobExpInfoViewModel> RecruitmentQualificationJobExpList { get; set; }

        public IList<SelectListItem> PayScaleList { get; set; }
        public IList<SelectListItem> PostNameList { get; set; }
        public IList<SelectListItem> OnList { get; set; }
        public IList<SelectListItem> ProfessionalCertificateList { get; set; }
        #endregion
    }
}