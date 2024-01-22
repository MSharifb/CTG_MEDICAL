using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ComplaintNoteSheetViewModel : BaseViewModel
    {
        #region Ctor
        public ComplaintNoteSheetViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion

        #region Standard Property
        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        [DisplayName("Departmental Proceedings No.")]
        public string DeptProceedingNo { get; set; }

        [DisplayName("Reference No.")]
        public string RefNo { get; set; }

        [DisplayName("Complaint Date")]
        [UIHint("_Date")]
        public DateTime? ComplaintDate { get; set; }

        [Required]
        public int ComplaintEmployeeId { get; set; }

        [Required]
        [DisplayName("Complaint Details")]
        [UIHint("_MultiLine")]
        [MaxLength(500)]
        public string ComplaintDetails { get; set; }

        public int? ComplainantEmployeeId { get; set; }

        #endregion

        #region Others  Property

        #region Complaint Info

        [Required]
        [Display(Name = "Employee ID")]
        public string ComplaintEmpId { get; set; }

        [Required]
        [Display(Name = "Name of the Accused Person")]
        [UIHint("_ReadOnly")]
        public string ComplaintEmployeeName { get; set; }

        public int? ComplaintDesignationId { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string ComplaintDesignationName { get; set; }

        public int? ComplaintDepartmentId { get; set; }

        [Display(Name = "Department Name")]
        [UIHint("_ReadOnly")]
        public string ComplaintDepartmentName { get; set; }
        #endregion

        #region Complainant Info

        [Display(Name = "Employee ID")]
        public string ComplainantEmpId { get; set; }

        [Display(Name = "Name of the Complainant")]
        [UIHint("_ReadOnly")]
        public string ComplainantEmployeeName { get; set; }

        public int? ComplainantDesignationId { get; set; }

        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string ComplainantDesignationName { get; set; }


        public int? ComplainantDepartmentId { get; set; }

        [Display(Name = "Department Name")]
        [UIHint("_ReadOnly")]
        public string ComplainantDepartmentName { get; set; }

        #endregion

        #endregion
    }
}