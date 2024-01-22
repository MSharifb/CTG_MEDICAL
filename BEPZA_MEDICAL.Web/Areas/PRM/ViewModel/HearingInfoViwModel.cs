using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class HearingInfoViwModel : BaseViewModel
    {

        #region Ctor
        public HearingInfoViwModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
            this.HearingFixationInfoList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Dept. Proceedings No.")]
        public int ComplaintNoteSheetId { get; set; }

        [Required]
        [DisplayName("Hearing Ref. No.")]
        public int HearingFixationInfoId { get; set; }

        [DisplayName("Hearing Details")]
        [UIHint("_MultiLine")]
        public string HearingDetails { get; set; }
        [Required]
        public int HearingInfoRecordedEmployeeId { get; set; }

        [Required]
        public int HearingFixationInfoDetailId { get; set; }
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

        [Required]
        [DisplayName("Hearing Date")]
        [UIHint("_ReadOnly")]
        public string HearingDate { get; set; }

        public DateTime? HearingDateL { get; set; }

        [DisplayName("Hearing Time")]
        [UIHint("_ReadOnly")]
        public string HearingTime { get; set; }

        [DisplayName("Hearing Location")]
        [UIHint("_ReadOnly")]
        public string HearingLocation { get; set; }

        public string HearingFixationInfoRefNo { get; set; }

        #region Hearing Information Recorded By

        [Required]
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string HearingInfoRecordedByEmpId { get; set; }

        [Required]
        [Display(Name = "Name")]
        [UIHint("_ReadOnly")]
        public string HearingInfoRecordedByEmployeeName { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string HearingInfoRecordedByDesignationName { get; set; }

        #endregion


        [DisplayName("Reference No.")]
        [UIHint("_ReadOnly")]
        public string RefNo { get; set; }

        [DisplayName("Complaint Date")]
        [UIHint("_ReadOnly")]
        public string ComplaintDate { get; set; }


        #region Accused Person

        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string ComplaintEmpId { get; set; }

        [Display(Name = "Name of the Accused Person")]
        [UIHint("_ReadOnly")]
        public string ComplaintEmployeeName { get; set; }

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
        public IList<SelectListItem> HearingFixationInfoList { set; get; }
        #endregion
    }
}