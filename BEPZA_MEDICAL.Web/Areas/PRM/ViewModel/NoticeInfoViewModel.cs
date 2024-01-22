using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class NoticeInfoViewModel :BaseViewModel
    {
        #region Ctor
        public NoticeInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
            this.NoticeTypeList = new List<SelectListItem>(); 
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Departmental Proceedings No.")]
        public int ComplaintNoteSheetId { get; set; }


        [Required]
        [DisplayName("Notice Date")]
        [UIHint("_RequiredDate")]
        public DateTime? NoticeDate { get; set; }


        [Required]
        [DisplayName("Notice Ref. No.")]
        public string NoticeRefNo { get; set; }
        
        [Required]
        [DisplayName("Notice Type")]
        public int NoticeTypeId { get; set; }

        //[Required]
        [DisplayName("Notice Details")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string NoticeDetails { get; set; }

        [DisplayName("Explanation Last Date")]
        [UIHint("_Date")]
        public DateTime? ExplanationLastDate { get; set; }

        [Required]
        public int NoticeIssueByEmployeeId { get; set; }

        public bool IsIssueNotice { get; set; }

        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        #endregion
        #endregion

        #region Others  Property
        public string ComplaintNoteSheetName { get; set; }
        public string NoticeTypeName { get; set; }
        public string NoticeStatus { get; set; }
      
        #region Notice Issue info
        
        [Required]
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string NoticeIssueByEmpId { get; set; }

        [Required]
        [Display(Name = "Notice Issue By")]
        [UIHint("_ReadOnly")]
        public string NoticeIssueByEmployeeName { get; set; }
     
        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string NoticeIssueByDesignationName { get; set; }

        #endregion

    
        [DisplayName("Reference No.")]
        [UIHint("_ReadOnly")]
        public string RefNo { get; set; }      
       
        [DisplayName("Complaint Date")]
        [UIHint("_ReadOnly")]
        public string ComplaintDate { get; set; }


        [DisplayName("Complaint Details")]
        [UIHint("_ReadOnly")]
        public string ComplaintDetails { get; set; }

        
        #region Accused Person

        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string ComplaintEmpId { get; set; }

        [Display(Name = "Name of the Accused Person")]
        [UIHint("_ReadOnly")]
        public string ComplaintEmployeeName { get; set; }

     
        public int? ComplaintDesignationId { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string ComplaintDesignationName { get; set; }

        [Display(Name = "Department Name")]
        [UIHint("_ReadOnly")]
        public string ComplaintDepartmentName { get; set; }
        #endregion

        #region Complainant Info
       
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string ComplainantEmpId { get; set; }

        [Display(Name = "Name of the Complainant")]
        [UIHint("_ReadOnly")]
        public string ComplainantEmployeeName { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string ComplainantDesignationName { get; set; }
        
        [Display(Name = "Department Name")]
        [UIHint("_ReadOnly")]
        public string ComplainantDepartmentName { get; set; }
      
        #endregion
        public IList<SelectListItem> ComplaintNoteSheetList { set; get; }
        public IList<SelectListItem> NoticeTypeList { set; get; }  
        #endregion
    }
}