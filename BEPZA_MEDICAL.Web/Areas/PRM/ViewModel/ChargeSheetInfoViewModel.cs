using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ChargeSheetInfoViewModel : BaseViewModel
    {
        #region Ctor
        public ChargeSheetInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Dept. Proceedings No.")]
        public int ComplaintNoteSheetId { get; set; }


        [Required]
        [DisplayName("Date of Charge-Sheet")]
        [UIHint("_RequiredDate")]
        public DateTime? ChargeSheetDate { get; set; }

        [Required]
        [DisplayName("Description of the Charge-Sheet")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string ChargeSheetDescription { get; set; }
      

        [Required]
        public int SignatoryEmployeeId { get; set; }
      

        #endregion

        #region Others  Property
        public string ComplaintNoteSheetName { get; set; }

        #region Signatory info
        
        [Required]
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string SignatoryEmpId { get; set; }

        [Required]
        [Display(Name = "Name of the Signatory")]
        [UIHint("_ReadOnly")]
        public string SignatoryEmployeeName { get; set; }
     
        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string SignatoryDesignationName { get; set; }
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
       
        #endregion
    }
}