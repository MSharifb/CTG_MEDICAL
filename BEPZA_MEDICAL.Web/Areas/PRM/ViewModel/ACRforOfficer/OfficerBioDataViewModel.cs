using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class OfficerBioDataViewModel
    {
        #region Ctor
        public OfficerBioDataViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.EducationQualificationList = new List<OfficerBioDataViewModel>();
            this.TrainingList = new List<TrainingViewModel>();
            this.LanguageList = new List<OfficerBioDataViewModel>();
        }

        #endregion

        #region Standerd Property

        public int OfficerInfoId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "6. Service/Cadre Name (If Any)")]
        public string ServiceOrCadreName { get; set; }
        [Display(Name = "From")]
        [UIHint("_Date")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "To")]
        [UIHint("_Date")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "14. Work Description within Reporting Duration")]
        public string WorkingDescription { get; set; }
        public int ReportingOfficerId { get; set; }
        [Display(Name = "Officer Reported Under (ORU)")]
        public string NameOfReportingOfficer { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        [Display(Name = "Date")]
        [UIHint("_Date")]
        public DateTime? ReportingDate { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        public int ApprovalStatusId { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

        #region Basic
        [Display(Name = "3. Date of Birth")]
        public string DateofBirth { get; set; }
        [Display(Name = "4. Father's Name")]
        public string FatherName { get; set; }
        [Display(Name = "5. (A) Marital Status")]
        public string MaritalStatus { get; set; }
        [Display(Name = "5. (B) Number of Children")]
        public int? NumberofChildren { get; set; }
        [Display(Name = "7. Date of JoinIng")]
        public string DateOfJoinIng { get; set; }
        [Display(Name = "8. Date of Joining In Current Post")]
        public string DateOfJoiningInCurrentPost { get; set; }
        [Display(Name = "9. (A)Salary Scale Name")]
        public string SalaryScaleName { get; set; }
        [Display(Name = "9. (B)Current Basic Salary")]
        public decimal CurrentBasicSalary { get; set; }
        #endregion

        #region Education
        public string ExaminationName { get; set; }
        public string AcademicInstitution { get; set; }
        public string PassingYear { get; set; }
        public string DivisionOrGrade { get; set; }
        public string BoardOrUniversity { get; set; }
        public string Subject { get; set; }
        #endregion

        #region Language
        public string Language { get; set; }
        public string Speaking { get; set; }
        public string Reading { get; set; }
        public string Writing { get; set; }
        #endregion

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
        [Display(Name = "Annual Confidential Report Date")]
        public string ACRDate { get; set; }
        [Display(Name = "ACR Period From")]
        public string ACRPeriodFrom { get; set; }
        [Display(Name = "ACR Period To")]
        public string ACRPeriodTo { get; set; }
        [Display(Name = "Seniority Serial Number")]
        public string SeniorityNumber { get; set; }

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

        public IList<OfficerBioDataViewModel> EducationQualificationList { get; set; }
        public IList<TrainingViewModel> TrainingList { get; set; }
        public IList<OfficerBioDataViewModel> LanguageList { get; set; }
        public IList<SelectListItem> ApproverList { get; set; }

        [Display(Name = "Comments")]
        public string ApproverComments { get; set; }

        #endregion
    }
}