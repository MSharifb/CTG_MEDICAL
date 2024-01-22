//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BEPZA_MEDICAL.DAL.AMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRM_EmploymentInfo
    {
        public PRM_EmploymentInfo()
        {
            this.AMS_BlacklistRemoval = new HashSet<AMS_BlacklistRemoval>();
            this.AMS_BlacklistRemoval1 = new HashSet<AMS_BlacklistRemoval>();
            this.SMS_SecurityInfo = new HashSet<SMS_SecurityInfo>();
            this.AMS_Blacklist = new HashSet<AMS_Blacklist>();
            this.SMS_SecurityServiceHistory = new HashSet<SMS_SecurityServiceHistory>();
        }
    
        public int Id { get; set; }
        public string EmpID { get; set; }
        public string EmployeeInitial { get; set; }
        public Nullable<int> TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string FullNameBangla { get; set; }
        public System.DateTime DateofJoining { get; set; }
        public int ProvisionMonth { get; set; }
        public Nullable<System.DateTime> DateofConfirmation { get; set; }
        public System.DateTime DateofPosition { get; set; }
        public int DesignationId { get; set; }
        public Nullable<int> StatusDesignationId { get; set; }
        public Nullable<int> DisciplineId { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> SubSectionId { get; set; }
        public int JobLocationId { get; set; }
        public Nullable<int> ResourceLevelId { get; set; }
        public int StaffCategoryId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int ReligionId { get; set; }
        public bool IsContractual { get; set; }
        public bool IsConsultant { get; set; }
        public bool IsOvertimeEligible { get; set; }
        public Nullable<decimal> OvertimeRate { get; set; }
        public string MobileNo { get; set; }
        public string EmialAddress { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BankBranchId { get; set; }
        public string BankAccountNo { get; set; }
        public int EmploymentStatusId { get; set; }
        public Nullable<System.DateTime> DateofInactive { get; set; }
        public bool IsBonusEligible { get; set; }
        public int SalaryScaleId { get; set; }
        public int JobGradeId { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> ContractExpireDate { get; set; }
        public System.DateTime DateofBirth { get; set; }
        public Nullable<decimal> ContractDuration { get; set; }
        public Nullable<int> ContractType { get; set; }
        public int OrganogramLevelId { get; set; }
        public System.DateTime DateofAppointment { get; set; }
        public string OrderNo { get; set; }
        public Nullable<int> QuotaId { get; set; }
        public Nullable<int> EmployeeClassId { get; set; }
        public Nullable<int> EmploymentProcessId { get; set; }
        public string SeniorityPosition { get; set; }
        public Nullable<System.DateTime> DateofSeniority { get; set; }
        public Nullable<System.DateTime> PRLDate { get; set; }
        public bool IsPensionEligible { get; set; }
        public bool IsLeverageEligible { get; set; }
        public string CardNo { get; set; }
        public string FingerPrintIdentiyNo { get; set; }
        public Nullable<System.DateTime> AttendanceEffectiveDate { get; set; }
        public bool AttendanceStatus { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public bool IsGeneralShifted { get; set; }
        public Nullable<int> RegionId { get; set; }
        public int ZoneInfoId { get; set; }
        public string TelephoneOffice { get; set; }
        public string Intercom { get; set; }
        public string HonoraryDegree { get; set; }
        public Nullable<bool> IsEligibleForCpf { get; set; }
        public Nullable<int> TaxRegionId { get; set; }
        public Nullable<byte> TaxAssesseeType { get; set; }
        public Nullable<bool> HavingChildWithDisability { get; set; }
        public Nullable<System.DateTime> DateofRetirement { get; set; }
        public Nullable<int> SalaryWithdrawFromZoneId { get; set; }
        public string ETIN { get; set; }
        public Nullable<bool> IsRefreshmentEligible { get; set; }
        public Nullable<bool> IsGPFEligible { get; set; }
    
        public virtual ICollection<AMS_BlacklistRemoval> AMS_BlacklistRemoval { get; set; }
        public virtual ICollection<AMS_BlacklistRemoval> AMS_BlacklistRemoval1 { get; set; }
        public virtual PRM_ZoneInfo PRM_ZoneInfo { get; set; }
        public virtual ICollection<SMS_SecurityInfo> SMS_SecurityInfo { get; set; }
        public virtual ICollection<AMS_Blacklist> AMS_Blacklist { get; set; }
        public virtual ICollection<SMS_SecurityServiceHistory> SMS_SecurityServiceHistory { get; set; }
    }
}
