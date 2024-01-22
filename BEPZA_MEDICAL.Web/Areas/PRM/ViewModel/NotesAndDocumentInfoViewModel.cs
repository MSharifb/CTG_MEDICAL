using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class NotesAndDocumentInfoViewModel : BaseViewModel
    {
        #region Ctor
        public NotesAndDocumentInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.NotesAndDocumentInfoAttachmentDetail = new List<NotesAndDocumentInfoAttachmentDetailViewModel>();
            this.TempAttachmentDetail = new List<NotesAndDocumentInfoViewModel>();
        }
        #endregion

        #region Standard Property
       // [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        #endregion

        #region Other

        #region Employee Info
     //   [Required]
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }

        [Display(Name = "Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        public int? DesignationId { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        public int? DivisionId { get; set; }

        [Display(Name = "Department")]
        [UIHint("_ReadOnly")]
        public string DivisionName { get; set; }

        public int? SectionId { get; set; }

        [Display(Name = "Section")]
        [UIHint("_ReadOnly")]
        public string SectionName { get; set; }

        #endregion

        [Display(Name = "Ref. No.")]
        public string RefNo { get; set; }
       
        [UIHint("_Date")]
        public DateTime? Date { get; set; }

        public string Subject { get; set; }

        [UIHint("_Multiline")]
        [MaxLength(500)]
        public string Details { get; set; }
        public IList<NotesAndDocumentInfoAttachmentDetailViewModel> NotesAndDocumentInfoAttachmentDetail { get; set; }

        public IList<NotesAndDocumentInfoViewModel> TempAttachmentDetail { get; set; }

        public int? NotesAndDocumentInfoId { get; set; }
        #region Attachment

        [Display(Name = "Attachment")]
        public bool IsAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int? FileSize { get; set; }
        #endregion
        #endregion
    }
}