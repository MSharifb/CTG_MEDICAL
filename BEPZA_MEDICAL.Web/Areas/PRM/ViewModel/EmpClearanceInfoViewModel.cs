using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmpClearanceInfoViewModel : BaseViewModel
    {
        #region Ctor
        public EmpClearanceInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ClearanceFormList = new List<SelectListItem>();          
            this.EmpClearanceFormDetails = new List<EmpClearanceInfoFormDetailsViewModel>();
            this.ClearanceDate = DateTime.Now;
            
        }
        #endregion

        #region Standard Property
      
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        //[Required]
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
       
        [UIHint("_MultiLine")]
        public string Remarks { get; set; }

        #endregion
        
        #region Other
        [Required]
        [Display(Name = "Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        [UIHint("_ReadOnly")]
        public string Department { get; set; }

        public int? DesignationId { get; set; }

        [UIHint("_ReadOnly")]
        public string Designation { get; set; }

        [UIHint("_ReadOnly")]
        public string Section { get; set; }

        public int? ClearanceFormId { get; set; }

        [Display(Name = "Clearance Form")]
        public IList<SelectListItem> ClearanceFormList { set; get; }


        [Display(Name = "Clearance Date")]
        [UIHint("_Date")]
        public DateTime ClearanceDate { get; set; }

        public int ResponsibleEmployeeId { get; set; }
       
        [Display(Name = "Responsible Person")]
        [UIHint("_ReadOnly")]
        public string ResponsibleEmployeeName { get; set; }
       
        [UIHint("_ReadOnly")]
        [Display(Name = "Designation")]
        public string ResponsibleEmployeeDesignation { get; set; }

        public IList<EmpClearanceInfoFormDetailsViewModel> EmpClearanceFormDetails { get; set; }

        #endregion

        #region Attachment
        [Display(Name="Add Attachment")]
        public bool isAddAttachment { set; get; }
        public byte[] Attachment { set; get; }

        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        #endregion
    }
}