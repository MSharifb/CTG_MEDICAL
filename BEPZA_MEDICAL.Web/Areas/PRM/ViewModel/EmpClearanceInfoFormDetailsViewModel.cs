using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmpClearanceInfoFormDetailsViewModel : BaseViewModel
    {
        #region Ctor
        public EmpClearanceInfoFormDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ClearanceCheckList = new List<SelectListItem>();
            this.EmpChecklistDetails = new List<EmpClearanceInfoChecklistDetailsViewModel>();
        }
        #endregion

        #region Standard Property
        [Required]
        public int ClearanceFormId { get; set; }

        [Display(Name = "Clearance Date")]
        [UIHint("_Date")]
        public DateTime ClearanceDate { get; set; }
        
        [Required]
        public int ResponsibleEmployeeId { get; set; }
        #endregion
        
        #region Other

        [Display(Name = "Clearance Form ")]
        public string ClearanceFormName { get; set; }

        [Display(Name = "Responsible Person")]
        [UIHint("_ReadOnly")]
        public string ResponsibleEmployeeName { get; set; }

        [UIHint("_ReadOnly")]
        [Display(Name = "Designation")]
        public string ResponsibleEmployeeDesignation { get; set; }
        public int EmpClearanceInfoId { get; set; }

        public int? ClearanceChecklistId { get; set; }
     

        [Display(Name = "Clearance Status")]
        public bool Status { get; set; }

        [Display(Name = "If ‘No’, Description")]
        [UIHint("_MultiLine")]
        public string Description { get; set; }

        [Display(Name = "Name of Checklist")]
        public IList<SelectListItem> ClearanceCheckList { set; get; }
        public IList<EmpClearanceInfoChecklistDetailsViewModel> EmpChecklistDetails { get; set; }
        #endregion
    }
}