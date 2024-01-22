using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApplicantInterviewCardIssueViewModel : BaseViewModel
    {
        #region Ctor
        public ApplicantInterviewCardIssueViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";

            this.AdvertisementCodeList = new List<SelectListItem>();
            this.SelectionCriteriaList = new List<SelectListItem>();
            this.DesignationNameList = new List<SelectListItem>();

            this.JobPostInformationList = new List<ApplicantInterviewCardIssueViewModel>();
            this.ApplicantInformationListDetail = new List<ApplicantInterviewCardIssueViewModel>();
        }
        #endregion

        #region Standard Property
        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }

        [Display(Name = "Reference Date")]
        [UIHint("_RequiredDate")]
        public DateTime ReferenceDate { get; set; }

        [Display(Name = "Interview Date")]
        [UIHint("_RequiredDate")]
        public DateTime InterviewDate{ get; set; }

        [Display(Name = "Interview Time")]
        public Nullable<System.TimeSpan> InterviewTime { get; set; }

        public string Conditions { get; set; }
        public string Venue { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int SignatoryICNo { get; set; }
        [Display(Name = "Advertisement Code")]
        public int AdvertisementInfoId { get; set; }
        public int DesignationId { get; set; }
        [Display(Name = "Selection Criteria/Exam Type")]
        public int SelectionCriteriaOrExamTypeId { get; set; }
        public bool IsIssue { get; set; }


        public string ExamTypeName { get; set; }
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        [Display(Name = "Roll No. From")]
        public int FromRollNo { get; set; }
        [Display(Name = "Roll No. To")]
        public int ToRollNo { get; set; }
        public IList<SelectListItem> AdvertisementCodeList { get; set; }
        public IList<SelectListItem> SelectionCriteriaList { get; set; }
        public IList<SelectListItem> DesignationNameList { get; set; }

        #endregion

        #region Issue Letter To

        //Job post Information
        public int? DepartmentId { get; set; }
        [UIHint("_ReadOnly")]
        public string DepartmentName { get; set; }
        public int? SectionId { get; set; }
        [UIHint("_ReadOnly")]
        public string SectionName { get; set; }
        public int? NoOfPost { get; set; }
        //end
        public int ApplicantInfoId { get; set; }

        //[Remote("GettingExistingRollNo", "ApplicantInterviewCardIssue", HttpMethod = "POST", ErrorMessage = "Roll No. already exists!")]
        public int? RollNo { get; set; }
        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string DateOfBirth { get; set; }
        public string NID { get; set; }
        public string AdvertisementCode { get; set; }

        public IList<ApplicantInterviewCardIssueViewModel> JobPostInformationList { get; set; }
        public IList<ApplicantInterviewCardIssueViewModel> ApplicantInformationListDetail { get; set; }

        public bool IsChecked { get; set; }
        public bool IsCheckedFinal { get; set; }
        #endregion

        #region Other
        public string Issue { get; set; }
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        //[Display(Name = "Roll No. Start From")]
        //public int RollNoStartFrom { get; set; }
        public bool IsSms { get; set; }
        public bool IsEmail { get; set; }

        public string EmailId { get; set; }
        public string PhoneNo { get; set; }

        #endregion
    }
}