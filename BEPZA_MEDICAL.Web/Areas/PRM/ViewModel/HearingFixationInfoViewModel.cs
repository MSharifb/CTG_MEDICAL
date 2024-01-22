using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class HearingFixationInfoViewModel : BaseViewModel
    {
        #region Ctor
        public HearingFixationInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
            this.HearingStatusList = new List<SelectListItem>();
            this.HearingFixationInfoDetail = new List<HearingFixationInfoDetailViewModel>();
        }

        #endregion

        #region Standard Property
        [Required]
        public int ZoneInfoId { get; set; }
        
        [Required]
        [DisplayName("Dept. Proceedings No.")]
        public int ComplaintNoteSheetId { get; set; }

        [Required]
        [DisplayName("Hearing Ref. No.")]
        public string HearingRefNo { get; set; }

        [Required]
        public int HearingfixationEmployeeId { get; set; }


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

        [DisplayName("Hearing Date")]
        [UIHint("_Date")]
        public DateTime? HearingDate { get; set; }


        [DisplayName("Hearing Time")]
        public TimeSpan? HearingTime { get; set; }

        [DisplayName("Hearing Location")]
        public string HearingLocation { get; set; }

        [DisplayName("Comments")]
        [UIHint("_MultiLine")]
        public string HearingComments { get; set; }

        [DisplayName("Status")]
        public string HearingStatus { get; set; }


        #region Hearing fixation By  info

        [Required]
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string HearingfixationByEmpId { get; set; }

        [Required]
        [Display(Name = "Explanation Received By")]
        [UIHint("_ReadOnly")]
        public string HearingfixationByEmployeeName { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string HearingfixationByDesignationName { get; set; }

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
        public IList<SelectListItem> HearingStatusList { set; get; }
        public IList<HearingFixationInfoDetailViewModel> HearingFixationInfoDetail { set; get; }
        #endregion
    }
}