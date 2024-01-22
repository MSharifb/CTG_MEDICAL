using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.OvertimePaymentApproval
{
    public class OvertimePaymentApprovalViewModel : LongBaseViewModel
    {
        public OvertimePaymentApprovalViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
        }

        #region Standard Properties

        [Required]
        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("Salary Year")]
        public string SalaryYear { get; set; }
        [Required]
        [DisplayName("Salary Month")]
        public string SalaryMonth { get; set; }
        public int OverTimeRuleId { get; set; }
        [Required]
        [DisplayName("Salary Head")]
        public int SalaryHeadId { get; set; }
        [Required]
        [DisplayName("Overtime Rate")]
        public decimal Rate { get; set; }
        public decimal RateMultiplier { get; set; }
        public decimal RevenueStamp { get; set; }
        [Required]
        [DisplayName("Total Overtime Hours")]
        public int TotalOverTimeHours { get; set; }
        [Required]
        [DisplayName("Total Approved Overtime Hours")]
        public decimal TotalApprovedOvertimeHours { get; set; }
        [Required]
        [DisplayName("Total Granted Overtime Hours")]
        public decimal TotalGrantedOvertimeHours { get; set; }
        [Required]
        [DisplayName("Total Granted Amounts")]
        public decimal GrantedAmount { get; set; }
      
        [DisplayName("Payment Date")]
        [UIHint("_Date")]
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public bool IsPayment { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }

        #endregion 

        #region Others
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }
        
        [DisplayName("Department Name")]
        public int DepartmentId { get; set; }
        public string DesignationName { get; set; }

        public string Mode { get; set; }
        public string strRate { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }

        public IList<SelectListItem> DepartmentList { get; set; }

        #endregion

    }
}