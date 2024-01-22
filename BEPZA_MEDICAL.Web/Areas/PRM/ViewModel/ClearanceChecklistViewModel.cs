using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class ClearanceChecklistViewModel : BaseViewModel
    {

        #region Ctor
        public ClearanceChecklistViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ClearanceFormList = new List<SelectListItem>();
            this.ClearanceChecklistDetails = new List<ClearanceChecklistDetailsViewModel>();
        }
        #endregion

        #region Standard Property
        public int IdT { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }

        [Required]

        public int ClearanceFormId { get; set; }

        [Display(Name = "Clearance Form")]
        public IList<SelectListItem> ClearanceFormList { set; get; }
        
        [MaxLength(250, ErrorMessage = "Maximum length is 250.")]
        public string Remarks { get; set; }

        #endregion

        #region Other
    
        [Required]
        [Display(Name = "Responsible Person")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        public string ClearanceFormName { get; set; }
        public virtual ICollection<ClearanceChecklistDetailsViewModel> ClearanceChecklistDetails { get; set; }

        #endregion
    }
}