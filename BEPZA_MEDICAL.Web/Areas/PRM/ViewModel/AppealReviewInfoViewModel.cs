using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class AppealReviewInfoViewModel : BaseViewModel
    {
        #region Ctor
        public AppealReviewInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Departmental Proceedings No.")]
        public int ComplaintNoteSheetId { get; set; }


        [Required]
        [DisplayName("Appeal Date")]
        [UIHint("_RequiredDate")]
        public DateTime? AppealDate { get; set; }


        [DisplayName("Appeal Details")]
        [UIHint("_MultiLine")]
        public string AppealDetails { get; set; }

        [Required]
        public int ApplyByEmployeeId { get; set; }

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

        #region Apply By

        [Required]
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string ApplyByEmpId { get; set; }

        [Required]
        [Display(Name = "Name")]
        [UIHint("_ReadOnly")]
        public string ApplyByEmployeeName { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string ApplyByDesignationName { get; set; }

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
        #endregion
    }
}