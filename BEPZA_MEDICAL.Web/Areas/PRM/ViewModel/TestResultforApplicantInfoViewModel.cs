using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class TestResultforApplicantInfoViewModel : BaseViewModel
    {
        #region Ctor
        public TestResultforApplicantInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.AdvertisementCodeList = new List<SelectListItem>();
            this.SelectionCriteriaList = new List<SelectListItem>();
            this.DesignationNameList = new List<SelectListItem>();

            this.JobPostInformationList = new List<TestResultforApplicantInfoViewModel>();
            this.ApplicantInformationListDetail = new List<TestResultforApplicantInfoViewModel>();
        }
        #endregion

        #region Standard Property

        [Display(Name="Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }
        public IList<SelectListItem> AdvertisementCodeList { get; set; }
        [Display(Name = "Selection Criteria/Exam Type")]
        public int SelectionCriteriaOrExamTypeId { get; set; }
        public string ExamTypeName { get; set; }
        public IList<SelectListItem> SelectionCriteriaList { get; set; }
        public IList<SelectListItem> DesignationNameList { get; set; }
        #endregion

        #region Other
        //Job post Information
        public bool IsChecked { get; set; }
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? NoOfPost { get; set; }
        public IList<TestResultforApplicantInfoViewModel> JobPostInformationList { get; set; }
        //end
        public decimal? FullMark { get; set; }
        public decimal? PassMark { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public string Notes { get; set; }
        public int ApplicantInfoId { get; set; }
        public int? RollNo { get; set; }
        public string ApplicantName { get; set; }
        public string DateOfBirth { get; set; }
        public string AdvertisementCode { get; set; }
        public bool IsCheckedFinal { get; set; }

        public IList<TestResultforApplicantInfoViewModel> ApplicantInformationListDetail { get; set; }
        #endregion
    }
}