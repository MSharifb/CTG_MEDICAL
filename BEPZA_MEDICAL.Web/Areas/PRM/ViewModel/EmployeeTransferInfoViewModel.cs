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
    public class EmployeeTransferInfoViewModel : BaseViewModel
    {
        public EmployeeTransferInfoViewModel()
        {
            this.ToDesignationList = new List<SelectListItem>();
        }

        public int EmployeeId { set; get; }

        [DisplayName("Employee ID")]
        public string EmpId { set; get; }

        [DisplayName("Order No.")]
        public string OrderNo { set; get; }

        [DisplayName("Order Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? OrderDate { set; get; }

        [DisplayName("Transfer Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? TransferDate { set; get; }

        [DisplayName("From Organogram Level")]
        [Required]
        public int? FromOrganogramLevelId { set; get; }
        public string FromOrganogramLevelName { set; get; }

        [DisplayName("To Organogram Level")]
        //[Required]
        public int? ToOrganogramLevelId { set; get; }
        //public string ToOrganogramLevelName { set; get; }

        
        [UIHint("_MultiLine")]
        public string Remarks { set; get; }

        [DisplayName("From Status Designation")]
        public int FromDesignationId { get; set; }

        [DisplayName("To Status Designation")]
        public int? ToDesignationId { get; set; }

        public int FromZoneInfoId { get; set; }
        public int? ToZoneInfoId { get; set; }
        [DisplayName("Transfer Type")]
        public bool IsOutofOfficeOrDeputaion { set; get; }

        [DisplayName("Organization Name")]
        public string OrganizationName { set; get; }

        [DisplayName("Department Name")]
        public string DepartmentName { set; get; }

        [DisplayName("Section Name")]
        public string SectionName { set; get; }

        [DisplayName("Address")]
        public string Address { set; get; }
        #region Otehrs

        [DisplayName("Joining Date")]
        [UIHint("_ReadOnlyDate")]
        [ReadOnly(true)]
        public DateTime? JoiningDate { set; get; }

        [DisplayName("Status Designation")]
        [ReadOnly(true)]
        public virtual string Designation { set; get; }

        [DisplayName("Employee Name")]
        [ReadOnly(true)]
        public string EmployeeName { set; get; }      

        public int? GradeId { get; set; }

        [DisplayName("Grade")]
        public string GradeName { get; set; }

        public string ToDesignationName { get; set; }
        public IList<SelectListItem> ToDesignationList { get; set; }
        public int? ToGradeId { get; set; }

        [DisplayName("To Grade")]
        public string ToGradeName { get; set; }

        //public int IsError { set; get; }
        //public string ErrMsg { set; get; }     

        #endregion


    }
}