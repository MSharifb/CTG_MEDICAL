using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SelectedApplicantInfoViewModel : BaseViewModel
    {
        #region Ctor
        public SelectedApplicantInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.AdvertisementCodeList = new List<SelectListItem>();
            this.SelectionCriteriaExamTypeList = new List<SelectListItem>();
            this.IsFinallySelectedList = new List<SelectListItem>();
            this.SelectedForNextExamList = new List<SelectListItem>();

            this.JobPostInformationList = new List<SelectedApplicantInfoViewModel>();
            this.ApplicantInformationListDetail = new List<SelectedApplicantInfoViewModel>();
            this.TestResultList = new List<SelectedApplicantInfoViewModel>();
        }
        #endregion

        #region Standard Property

        [Display(Name="Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }
        public IList<SelectListItem> AdvertisementCodeList { get; set; }

        [Display(Name = "Selection Criteria/Exam. Type")]
        public int SelectionCriteriaExamTypeId {get; set;}
        public IList<SelectListItem> SelectionCriteriaExamTypeList { get; set; }

        public bool? IsFinalExam { get; set; }

        [Display(Name = "Candidate Type")]
        public bool CandidateType { get; set; }
        public string Comments { get; set; }
        public int ZoneInfoId { get; set; }
        public string ExamTypeName { get; set; }
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
        public IList<SelectedApplicantInfoViewModel> JobPostInformationList { get; set; }
        //end
        public int ExamTypeId { get; set; }
        public decimal? FullMark { get; set; }
        public decimal? PassMark { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public int? QuotaId { get; set; }
        public string Quota { get; set; }
        public bool IsFinallySelected { get; set; }
        public IList<SelectListItem> IsFinallySelectedList { get; set; }
        public int? SelectedId { get; set; }
        public string Notes { get; set; }
        public int ApplicantInfoId { get; set; }
        public int? RollNo { get; set; }
        public string ApplicantName { get; set; }
        public string DateOfBirth { get; set; }
        public string AdvertisementCode { get; set; }
        public bool IsCheckedFinal { get; set; }
        public int? SelectedForNextExamId { get; set; }
        public IList<SelectListItem> SelectedForNextExamList { get; set; }
        //public 

        public IList<SelectedApplicantInfoViewModel> ApplicantInformationListDetail { get; set; }

        // Test Result
        public string TestName { get; set; }
        public IList<SelectedApplicantInfoViewModel> TestResultList { get; set; }

        #endregion

        #region Attachment

        public byte[] Attachment { set; get; }
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion
    }
}