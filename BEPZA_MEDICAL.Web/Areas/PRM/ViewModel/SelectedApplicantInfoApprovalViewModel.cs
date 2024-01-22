using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SelectedApplicantInfoApprovalViewModel : BaseViewModel
    {
        #region Ctor
        public SelectedApplicantInfoApprovalViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.AdvertisementCodeList = new List<SelectListItem>();
            this.JobPostInformationList = new List<SelectedApplicantInfoApprovalViewModel>();
            this.ApplicantInfoApprovalListDetail = new List<SelectedApplicantInfoApprovalViewModel>();
        }

        #endregion

        #region Standard Property

        [Display(Name = "Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }
        public IList<SelectListItem> AdvertisementCodeList { get; set; }
        [Display(Name = "Candidate Type")]
        public bool CandidateType { get; set; }

        [Required]
        public int EmployeeId { get; set; }

      //  [Required]
        [DisplayName("Approve Date")]
        [UIHint("_RequiredDate")]
        public DateTime? Date { get; set; }
        public bool IsSubmit { get; set; }

        public bool IsReject { get; set; }

        [UIHint("_Multiline")]
        public string Comments { get; set; }

        #endregion

        #region Other
        //Job post Information
        public bool IsChecked { get; set; }
        public int? DesignationId { get; set; }
        [Display(Name = "Status Designation")]
        public string DesignationName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? NoOfPost { get; set; }
        public IList<SelectedApplicantInfoApprovalViewModel> JobPostInformationList { get; set; }
        //end
        public decimal? FullMark { get; set; }
        public decimal? PassMark { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public string Notes { get; set; }
        public int ApplicantInfoId { get; set; }
        public int? RollNo { get; set; }
        public string ApplicantName { get; set; }
        public string DateOfBirth { get; set; }
        public string FinallySelected { get; set; }
        public string Quota { get; set; }
        public string FinalSelectedId { get; set; }
        public string AdvertisementCode { get; set; }
        public bool IsCheckedFinal { get; set; }
        public IList<SelectedApplicantInfoApprovalViewModel> ApplicantInfoApprovalListDetail { get; set; }

        [UIHint("_ReadOnly")]
        public string JobAdvertisementCode { get; set; }
         
        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        public string Status { get; set; }
        #endregion

        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion
    }
}