using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmploymentInfoViewModel
    {
        public EmploymentInfoViewModel()
        {
            this.GenderList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            this.BankList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
            this.StatusDesignationList = new List<SelectListItem>();
            this.DisciplineList = new List<SelectListItem>();
            this.DivisionList = new List<SelectListItem>();
            this.EmploymentStatusList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.JobLocationList = new List<SelectListItem>();
            this.ResourceLevelList = new List<SelectListItem>();
            this.StaffCategoryList = new List<SelectListItem>();
            this.TitleList = new List<SelectListItem>();
            this.BankBranchList = new List<SelectListItem>();
            this.ReligionList = new List<SelectListItem>();
            this.JobGradeList = new List<SelectListItem>();
            this.ContractualTypeList = new List<SelectListItem>();



            this.QuotaList = new List<SelectListItem>();
            this.EmploymentProcessList = new List<SelectListItem>();

            this.AssesseTypeList = new List<SelectListItem>();
            this.SalaryWithdrawFromList = new List<SelectListItem>();
            this.DateofJoining = DateTime.UtcNow;
            this.DateofPosition = DateTime.UtcNow;
            this.EmployeeInitial = "";
        }

        #region Standard Property

        [Required]
        public virtual int Id { get; set; }

        [Required]
        [DisplayName("Employee ID")]
        public virtual string EmpID { get; set; }

        [DisplayName("Employee Initial")]
        [StringLength(3)]
        public virtual string EmployeeInitial { get; set; }

        [DisplayName("Title")]
        public int? TitleId { get; set; }
        public IList<SelectListItem> TitleList { get; set; }

        [DisplayName("Religion")]
        public int? ReligionId { get; set; }

        public IList<SelectListItem> ReligionList { get; set; }

        [Required]
        [DisplayName("Job Grade")]
        public int JobGradeId { get; set; }
        public IList<SelectListItem> JobGradeList { get; set; }

        [Required]
        [DisplayName("Salary Scale")]
        public int SalaryScaleId { get; set; }
        //public IList<SelectListItem> SalaryScaleList { get; set; }

        [DisplayName("First Name")]
        [MaxLength(50)]
        public virtual string FirstName { get; set; }

        [DisplayName("Middle Name")]
        [MaxLength(50)]
        public virtual string MiddleName { get; set; }

        [DisplayName("Last Name")]
        [MaxLength(50)]
        public virtual string LastName { get; set; }

        [DisplayName("Full Name (Bangla)")]
        public string FullNameBangla { get; set; }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public virtual string FullName { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date of Joining")]
        public virtual System.DateTime DateofJoining
        {
            get;
            set;
        }

        [Required]
        [UIHint("_OnlyInteger")]
        [DisplayName("Probationary Period (Month)")]
        public virtual int ProvisionMonth
        {
            get;
            set;
        }

        //[UIHint("_ReadOnlyDate")]
        [UIHint("_Date")]
        [DisplayName("Date of Confirmation")]
        public virtual Nullable<System.DateTime> DateofConfirmation
        {
            get;
            set;
        }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date of Present Position")]
        public virtual System.DateTime DateofPosition
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Status Designation")]
        public virtual int? DesignationId
        {
            get;
            set;
        }
        public IList<SelectListItem> DesignationList
        {
            get;
            set;
        }

        [Required]
        [DisplayName("Working Designation")]
        public int? StatusDesignationId { get; set; }
        public IList<SelectListItem> StatusDesignationList { get; set; }

        [DisplayName("Department")]
        public virtual int? DivisionId { get; set; }

        public IList<SelectListItem> DivisionList { get; set; }

        [Required]
        [DisplayName("Joining Place")]
        public virtual int? JobLocationId { get; set; }

        public IList<SelectListItem> JobLocationList { get; set; }

        [DisplayName("Discipline")]
        public virtual int? DisciplineId { get; set; }

        public IList<SelectListItem> DisciplineList { get; set; }

        [DisplayName("Resource Level")]
        public virtual int? ResourceLevelId { get; set; }
        public IList<SelectListItem> ResourceLevelList { get; set; }

        [Required]
        [DisplayName("Officer / Staff Category")]
        public virtual int StaffCategoryId { get; set; }
        public IList<SelectListItem> StaffCategoryList { get; set; }

        [DisplayName("Is General Shifted")]
        [Required]
        public bool IsGeneralShifted { get; set; }

        [Required]
        [DisplayName("Employment Type")]
        public virtual int EmploymentTypeId { get; set; }


        public string EmploymentType { get; set; }

        //[Required]
        [DisplayName("Contractual Type")]
        public virtual int? ContractType { get; set; }

        public IList<SelectListItem> EmploymentTypeList { get; set; }

        public IList<SelectListItem> ContractualTypeList { get; set; }


        [DisplayName("Is Contractual Employee?")]
        [UIHint("_ReadOnly")]
        public virtual bool IsContractual { get; set; }

        [DisplayName("Is Consultant / External Resource?")]
        public virtual bool IsConsultant { get; set; }

        [DisplayName("Is Bonus Eligible?")]
        public virtual bool IsBonusEligible { get; set; }

        [DisplayName("Mobile")]
        [MaxLength(50)]
        public virtual string MobileNo { get; set; }


        [DisplayName("Email")]
        [MaxLength(50)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Format.")]
        public virtual string EmialAddress { get; set; }

        [DisplayName("Telephone(Office)")]
        [MaxLength(50)]
        public virtual string TelephoneOffice { get; set; }


        [DisplayName("Intercom")]
        [MaxLength(50)]
        public virtual string Intercom { get; set; }


        [DisplayName("Bank")]
        public virtual Nullable<int> BankId { get; set; }

        public IList<SelectListItem> BankList { get; set; }

        [DisplayName("Branch")]
        public virtual Nullable<int> BankBranchId { get; set; }

        public IList<SelectListItem> BankBranchList { get; set; }


        [DisplayName("Account No.")]
        [MaxLength(50)]
        public virtual string BankAccountNo { get; set; }

        [Required]
        [DisplayName("Employment Status")]
        public virtual int EmploymentStatusId { get; set; }

        [DisplayName("Employment Status")]
        [UIHint("_ReadOnly")]
        public string EmploymentStatusName { get; set; }

        public IList<SelectListItem> EmploymentStatusList { get; set; }

        [DisplayName("Inactive Date")]
        [UIHint("_ReadOnlyDate")]
        public virtual Nullable<System.DateTime> DateofInactive { get; set; }

        [DisplayName("Is Eligible for Overtime?")]
        public bool IsOvertimeEligible { get; set; }

        [DisplayName("Overtime Rate")]
        [Range(0, 999, ErrorMessage = "Overtime Rate must be upto 999.")]
        public decimal OvertimeRate { get; set; }

        [Required]
        public string Gender { get; set; }
        public IList<SelectListItem> GenderList { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date of Birth")]
        public Nullable<DateTime> DateofBirth { get; set; }

        [UIHint("_ReadOnlyDate")]
        [DisplayName("Date of Retirement")]
        public Nullable<DateTime> DateOfRetirement { get; set; }

        [UIHint("_Date")]
        [DisplayName("Contract End Date")]
        public DateTime? ContractExpireDate { get; set; }

        [DisplayName("Contract Duration(Month)")]
        public decimal? ContractDuration { get; set; }



        [DisplayName("Organogram")]
        public int? OrganogramLevelId { get; set; }

        [Required]
        [UIHint("_Date")]
        [DisplayName("Date of Appointment")]
        public DateTime? DateofAppointment { get; set; }

        [DisplayName("Office Order No.")]
        public string OrderNo { get; set; }

        [DisplayName("Quota")]
        public int? QuotaId { get; set; }
        public IList<SelectListItem> QuotaList { get; set; }

        [DisplayName("Employee Class")]
        public int? EmployeeClassId { get; set; }
        public string EmployeeClassName { get; set; }

        [DisplayName("Employment Process")]
        public int? EmploymentProcessId { get; set; }
        public IList<SelectListItem> EmploymentProcessList { get; set; }

        [DisplayName("Seniority / Merit Position")]
        public string SeniorityPosition { get; set; }

        [UIHint("_Date")]
        [DisplayName("Date of Seniority")]
        public DateTime? DateofSeniority { get; set; }


        [DisplayName("PRL Date")]
        [UIHint("_ReadOnlyDate")]
        public DateTime? PRLDate { get; set; }

        [DisplayName("Is Pension Eligible")]
        public bool IsPensionEligible { get; set; }

        [DisplayName("Is Leverage Eligible")]
        public bool IsLeverageEligible { get; set; }


        [DisplayName("Card No.")]
        [MaxLength(10)]
        public string CardNo { get; set; }

        [DisplayName("Finger Print Identity No.")]
        [MaxLength(50)]
        public string FingerPrintIdentiyNo { get; set; }

        [DisplayName("Effective Date")]
        [UIHint("_Date")]
        public DateTime? AttendanceEffectiveDate { get; set; }

        [DisplayName("Status")]
        public bool AttendanceStatus { get; set; }

        [DisplayName("Select Organogram Level")]
        [UIHint("_ReadOnly")]
        public string OrganogramLevelName { get; set; }

        [DisplayName("...")]
        public string OrganogramLevelDetail { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        [DisplayName("Honorary Degree")]
        public string HonoraryDegree { get; set; }


        #region Tax
        [DisplayName("Tax Region")]
        public int? TaxRegionId { get; set; }
        public IList<SelectListItem> TaxRegionList { get; set; }

        [DisplayName("Assessee Type")]
        public byte? TaxAssesseeType { get; set; }
        public IList<SelectListItem> AssesseTypeList { get; set; }

        [DisplayName("Have children with disability")]
        public bool HavingChildWithDisability { get; set; }

        [DisplayName("Salary Withdraw From")]
        public int SalaryWithdrawFromZoneId { get; set; }
        public IList<SelectListItem> SalaryWithdrawFromList { get; set; }
        #endregion

        [DisplayName("e-TIN")]
        [MaxLength(20)]
        public string ETIN { get; set; }
        #endregion

        #region Other

        public string DesignationName { get; set; }

        //[Required]
        public string JobGradeName { get; set; }

        [Required]
        public string SalaryScaleName { get; set; }


        [UIHint("_ReadOnly")]
        public string DivisionName { get; set; }
        public string DivisionNameForLabel { get; set; }

        [UIHint("_ReadOnly")]
        public string DisciplineName { get; set; }
        public string DisciplineForLabel { get; set; }

        [UIHint("_ReadOnly")]
        public string SectionName { get; set; }
        public string SectionNameForLabel { get; set; }

        [UIHint("_ReadOnly")]
        public string SubSectionName { get; set; }
        public string SubSectionNameForLabel { get; set; }

        public string SelectedEmploymentType { get; set; }

        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ButtonText { get; set; }
        public bool DeleteEnable { get; set; }
        public string SelectedClass { get; set; }
        public string ErrorClass { get; set; }
        public int IsError { get; set; }
        public bool isExist { get; set; }
        public bool IsPhotoExist { get; set; }

        public bool IsEmpEditDesignation { get; set; }
        public bool IsSalaryStructureProcess { get; set; }

        public bool IsRefreshmentEligible { get; set; }
        public bool IsGPFEligible { get; set; }

        #endregion

    }
}