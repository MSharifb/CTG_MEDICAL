using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApplicantInfoQualificationViewModel:BaseViewModel
    {
        #region Ctor
        public ApplicantInfoQualificationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        #endregion

        #region Standard Property
        //public int Id { get; set; }
        public int ApplicantInfoId { get; set; }
        public int DegreeLevelId { get; set; }
        public int UniversityAndBoardId { get; set; }
        public int SubjectGroupId { get; set; }
        public int AcademicGradeId { get; set; }
        public int PassingYear { get; set; }
        public string AcademicInstName { get; set; }
        public decimal? CGPA { get; set; }
        public string DegreeLevelName { get; set; }
        public string AcademicGradeName { get; set; }
        public string UniversityAndBoardName { get; set; }
        public string SubjectGroupName { get; set; }
        #endregion

    }

}