using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class NoteOrderInfoViewModel : BaseViewModel
    {
        #region Ctor
        public NoteOrderInfoViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.ComplaintNoteSheetList = new List<SelectListItem>();
            this.OrderTypeInfoList = new List<SelectListItem>();
            this.DisciplinaryActionTypeList = new List<SelectListItem>();
            this.PunishmentTypeInfoList = new List<SelectListItem>();
            this.NoteOrderInfoTypeList = new List<NoteOrderInfoViewModel>();
        }

        #endregion

        #region Standard Property

        [Required]
        [DisplayName("Departmental Proceedings No.")]
        public int ComplaintNoteSheetId { get; set; }


        [DisplayName("Order Date")]
        [UIHint("_Date")]
        public DateTime? OrderDate { get; set; }


        [DisplayName("Order No.")]
        public string OrderNo { get; set; }
        
        [Required]
        [DisplayName("Order Type")]
        public int OrderTypeInfoId { get; set; }

        [DisplayName("Order Details")]
        [MaxLength(500)]
        [UIHint("_MultiLine")]
        public string OrderDetails { get; set; }
      

        public int? OrderByEmployeeId { get; set; }

        [DisplayName("Disciplinary Action Type")]
        public int? DisciplinaryActionTypeId { get; set; }

        [DisplayName("Name of the Punishment")]
        public int? PunishmentTypeInfoId { get; set; }

        [DisplayName("Effective Date From")]
        [UIHint("_Date")]
        public DateTime? EffectiveDateFrom { get; set; }

        [DisplayName("Effective Date To")]
        [UIHint("_Date")]
        public DateTime? EffectiveDateTo { get; set; }

        public bool IsOrder { get; set; }
        #endregion

        #region Others  Property
        public string ComplaintNoteSheetName { get; set; }

        #region Order info
        
        [Display(Name = "Employee ID")]
        [UIHint("_ReadOnly")]
        public string OrderByEmpId { get; set; }

        [Display(Name = "Order By")]
        [UIHint("_ReadOnly")]
        public string OrderByEmployeeName { get; set; }
     
        [Display(Name = "Designation")]
        [UIHint("_ReadOnly")]
        public string OrderByDesignationName { get; set; }
        #endregion

        public string OrderTypeInfoName { get; set; }
       
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

        public int? ComplaintEmployeeId { get; set; }
       
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

        public string FileStatus { get; set; }
        public IList<SelectListItem> ComplaintNoteSheetList { set; get; }
        public IList<SelectListItem> OrderTypeInfoList { set; get; }
        public IList<SelectListItem> DisciplinaryActionTypeList { set; get; }
        public IList<SelectListItem> PunishmentTypeInfoList { set; get; }

        public int? TypeSlNo { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public IList<NoteOrderInfoViewModel> NoteOrderInfoTypeList { set; get; }

        #endregion
    }
}