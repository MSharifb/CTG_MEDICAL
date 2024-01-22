using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class RecruitmentQualificationEducationInfoViewModel : BaseViewModel
    {
        #region Ctor
        public RecruitmentQualificationEducationInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }
        #endregion

        #region Standard Property

        public int? RecruitmentQualificationDetailsId { get; set; }
        public int? DegreeTypeId { get; set; }
        public int? DegreeLevelId { get; set; }
        public int? DivisionOrGradeId { get; set; }
        public decimal? GPAOrCGPA { get; set; }

        #endregion

        #region Other
        public string SubjectOrGroupId { get; set; }
        public string DegreeType { get; set; }
        public string DegreeLevel { get; set; }
        public string DivisionOrGrade { get; set; }
        public string SubjectOrGroup { get; set; }
        #endregion
    }
}