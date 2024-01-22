using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Utility;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeSeperationViewModel
    {

        #region Ctor
        public EmployeeSeperationViewModel()
        {
            this.TypeList = new List<SelectListItem>();
           // this.EffectiveDate = DateTime.Now;
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [Required]
        public string EmpId { set; get; }

        [DisplayName("Separation Type")]
        [Required]
        public string Type { set; get; }
        public IList<SelectListItem> TypeList { set; get; }

        //ShortageDays
        [DisplayName("Shortage of Notice Period")]
        [UIHint("_ReadOnlyInteger")]
        public int ShortageDays { set; get; }

        [DisplayName("Application Date")]
        [UIHint("_Date")]
        public DateTime? ApplicationDate { set; get; }

        [DisplayName("Effective Date")]
        [Required]
        [UIHint("_RequiredDate")]
        public DateTime? EffectiveDate { set; get; }

        [UIHint("_MultiLine")]
        public string Reason { set; get; }

        [UIHint("_MultiLine")]
        public string Remarks { set; get; }


        [DisplayName("Add Attachment")]
        public bool isAddAttachment { set; get; }

        public byte[] Attachment { set; get; }


        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsContractual { set; get; }
        public int PreviousEmploymentStatusId { set; get; }
        public string PreviousEmploymentStatus { set; get; }

        #region new field

        [DisplayName("Order No./ Ref No.")]
        public string OrderOrRefNo { get; set; }

        [UIHint("_MultiLine")]
        public string Action { get; set; }

        [UIHint("_MultiLine")]
        public string Condition { get; set; }

        public int ApprovalEmployeeId { get; set; }
        #endregion


        #endregion


        #region Other

        [Display(Name = "Employee Name")]
        [Required]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        [UIHint("_ReadOnly")]
        public string Designation { get; set; }



        [Display(Name = "Joining Date")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? DateofJoining { get; set; }

        [DisplayName("Employee ID")]
        public string ApprovalEmpId { get; set; }

        [Display(Name = "Employee Name")]
        //[Required]
        [UIHint("_ReadOnly")]
        public string ApprovalEmpName { get; set; }


        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string ApprovalEmpDesignation { get; set; }

        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string errClass { get; set; }
        public string ErrMsg { set; get; }

        public String NotifyTo { get; set; }

        #endregion
    }

}