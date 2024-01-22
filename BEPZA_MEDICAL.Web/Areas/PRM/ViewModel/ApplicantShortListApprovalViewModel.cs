using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ApplicantShortListApprovalViewModel : BaseViewModel
    {
        
        #region Ctor
        public ApplicantShortListApprovalViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.JobAdvertisementInfoList = new List<SelectListItem>();
            this.ApplicantShortListApprovalDetail = new List<ApplicantShortListApprovalDetailViewModel>();
            this.ApplicantShortListApproval = new List<ApplicantShortListApprovalViewModel>();
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Advertisement Code")]
        public int JobAdvertisementInfoId { get; set; }


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

        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion

        #region Other

        public int? ApplicantShortListApprovalId { get; set; }      
        public int? ApplicantInfoId { get; set; }

        [UIHint("_ReadOnly")]
        public string JobAdvertisementCode { get; set; }

      //  [Required]
        [Display(Name = "Short Listed By")]
        [UIHint("_ReadOnly")]        
        public string ShortListedEmpName { get; set; }

        //[Required]
        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string ShortListedDesignation { get; set; }

        //[Required]
        [Display(Name = "Short List Date")]
        [UIHint("_ReadOnly")]
        public string ShortListDate { get; set; }

       // [Required]
        [Display(Name = "Name")]
        [UIHint("_ReadOnly")]      
        public string EmployeeName { get; set; }

        public int? DesignationId { get; set; }
        
      //  [Required]
        [Display(Name = "Status Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        public int?  DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }        
        public string DateOfBirth { get; set; }
        public string NID { get; set; }
        public IList<SelectListItem> JobAdvertisementInfoList { set; get; }

        public IList<ApplicantShortListApprovalDetailViewModel> ApplicantShortListApprovalDetail { get; set; }
        public IList<ApplicantShortListApprovalViewModel> ApplicantShortListApproval { get; set; }

        public bool IsChecked { get; set; }

        public bool IsCheckedFinal { get; set; }  

        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? NoOfPost { get; set; }
        public string Status { get; set; }
        #endregion
    }
}