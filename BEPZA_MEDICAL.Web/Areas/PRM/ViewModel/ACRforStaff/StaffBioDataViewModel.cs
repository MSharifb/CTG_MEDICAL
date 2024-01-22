using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff
{
    public class StaffBioDataViewModel
    {
        #region Ctor
        public StaffBioDataViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.EducationQualificationList = new List<StaffBioDataViewModel>();
            this.TrainingList = new List<StaffBioDataViewModel>();
            this.LanguageList = new List<StaffBioDataViewModel>();
        }

        #endregion

        #region Standerd Property

        public int StaffInfoId { get; set; }
        public int EmployeeId { get; set; }
        public bool PassInDepExam { get; set; }
        [Display(Name="If Yes, Provide Exam Date")]
        [UIHint("_Date")]
        public DateTime? ExamDate { get; set; }
        [Display(Name="From")]
        [UIHint("_Date")]
        public DateTime? FromDate { get; set; }
        [Display(Name="To")]
        [UIHint("_Date")]
        public DateTime? ToDate { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        public int ApproverId { get; set; }

        [DisplayName("Forward To")]
        [Required]
        public int NextApproverId { get; set; }

        public int ApprovalStatusId { get; set; }

        [DisplayName("Comments")]
        public string ApproverComments { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }

        #region Basic
        [Display(Name="Date of Birth")]
        public string DateofBirth { get; set; }
        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }
        [Display(Name = "Date of JoinIng")]
        public string DateOfJoinIng { get; set; }
        [Display(Name = "Date of Joining In Current Post")]
        public string DateOfJoiningInCurrentPost { get; set; }
        [Display(Name = "Salary Scale Name")]
        public string SalaryScaleName { get; set; }
        [Display(Name = "Current Basic Salary")]
        public decimal CurrentBasicSalary { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }
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
        public string Reading { get;set;}
        public string Writing { get; set; }
        #endregion

        #region Training
        public string TrainingTitle { get; set; }
        public string Institution { get; set; }
        public string TraingType { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string TrainingYear { set; get; }
        #endregion 

        #endregion

        #region Officer's Info
        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name="Designation")]
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

        public IList<StaffBioDataViewModel> EducationQualificationList { get; set; }
        public IList<StaffBioDataViewModel> TrainingList { get; set; }
        public IList<StaffBioDataViewModel> LanguageList { get; set; }
        
        

        #endregion

    }
}