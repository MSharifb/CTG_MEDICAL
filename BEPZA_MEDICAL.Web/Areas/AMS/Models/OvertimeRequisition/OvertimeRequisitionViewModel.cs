using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ERP_BEPZA.Web.Areas.AMS.Models.OvertimeRequisition
{
    public class OvertimeRequisitionViewModel : BaseViewModel
    {
        public OvertimeRequisitionViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
            this.OvertimeRequisitionDetail = new List<OvertimeRequisitionDetailViewModel>();
        }

        #region Standard Properties
        [Required]
        [DisplayName("Department Name")]
        public int DepartmentId { get; set; }

        [Required]
        [UIHint("_Date")] 
        [DisplayName("Requisition Date")]
        public System.DateTime RequisitionDate { get; set; }
        [Required]
        [DisplayName("Overtime Date")]
        [UIHint("_Date")] 
        public System.DateTime OvertimeDate { get; set; }
        public bool IsApproved { get; set; }
        public string Description { get; set; }
        public virtual List<OvertimeRequisitionDetailViewModel> OvertimeRequisitionDetail { get; set; }

        #endregion 

        #region Others
        public string Mode { get; set; }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> Department { get; set; }
      
        #endregion 
    }

    public class OvertimeRequisitionDetailViewModel 
    {
        public int Id { get; set; }
        [DisplayName("Requisition No.")]
        public int RequisitionId { get; set; }
        public int EmployeeId { get; set; }
        public System.TimeSpan FromTime { get; set; }
        public System.TimeSpan ToTime { get; set; }
        public decimal TotalHours { get; set; }
        public Nullable<System.DateTime> DateOfApproval { get; set; }
        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }
        public int ApproverId { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        #region Other

        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
      //  public string SectionName { get; set; }
        [DisplayName("Department Name")]
        public int? DepartmentId { get; set; }

        [UIHint("_Date")]
        [DisplayName("Requisition Date")]
        public System.DateTime RequisitionDate { get; set; }
       
        [DisplayName("Overtime Date")]
        [UIHint("_Date")]
        public System.DateTime OvertimeDate { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public IList<SelectListItem> Department { get; set; }

        #endregion 
    }
}