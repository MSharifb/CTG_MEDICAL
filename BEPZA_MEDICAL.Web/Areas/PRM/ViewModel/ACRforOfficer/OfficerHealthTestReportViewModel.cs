using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class OfficerHealthTestReportViewModel
    {
        #region Ctor
        public OfficerHealthTestReportViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion

        #region Standerd Property

        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "1. Height")]
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Eyesight { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [Display(Name = "Blood Pressure")]
        public string BloodPressure { get; set; }
        [Display(Name = "X-Ray Report")]
        public string XRayReport { get; set; }
        [Display(Name = "ECG Report")]
        public string ECGReport { get; set; }
        [Display(Name = "2. Classification of Treatment")]
        public string ClassificationofTreatment { get; set; }
        [Display(Name = "3. Physical Weakness (In Summary)")]
        public string PhysicalWeekness { get; set; }

        public int MedicalOfficerId { get; set; }
        [Display(Name = "Name Of the Madical Officer")]
        public string NameOftheMadicalOfficer { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        [UIHint("_Date")]
        public DateTime? Date { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

        #endregion

        #region Officer's Info

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Designation")]
        public string EmployeeDesignation { get; set; }
        [Display(Name = "Department")]
        public string EmployeeDepartment { get; set; }
        [Display(Name = "Seniority Serial Number")]
        public string SeniorityNumber { get; set; }
        [Display(Name = "Annual Confidential Report Date")]
        public string ACRDate { get; set; }
        [Display(Name = "ACR Period From")]
        public string ACRPeriodFrom { get; set; }
        [Display(Name = "ACR Period To")]
        public string ACRPeriodTo { get; set; }

        #endregion

        #region Other

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public string SelectedClass { get; set; }
        public bool DeleteEnable { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }
        public bool IsPhotoExist { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }

        [Display(Name = "Comments")]
        public string ApproverComments { get; set; }

        #endregion

    }
}