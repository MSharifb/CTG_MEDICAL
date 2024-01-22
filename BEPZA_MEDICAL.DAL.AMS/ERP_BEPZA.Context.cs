﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ERP_BEPZA_AMSEntities : DbContext
    {
        public ERP_BEPZA_AMSEntities()
            : base("name=ERP_BEPZA_AMSEntities")
        {
            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 3600;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AMS_AutoReminder> AMS_AutoReminder { get; set; }
        public virtual DbSet<AMS_Category> AMS_Category { get; set; }
        public virtual DbSet<AMS_DesignationInfo> AMS_DesignationInfo { get; set; }
        public virtual DbSet<AMS_ReminderType> AMS_ReminderType { get; set; }
        public virtual DbSet<AMS_StayRule> AMS_StayRule { get; set; }
        public virtual DbSet<AMS_ZoneWiseQuota> AMS_ZoneWiseQuota { get; set; }
        public virtual DbSet<PRM_ZoneInfo> PRM_ZoneInfo { get; set; }
        public virtual DbSet<CommonConfigType> CommonConfigType { get; set; }
        public virtual DbSet<AMS_AnsarDegree> AMS_AnsarDegree { get; set; }
        public virtual DbSet<AMS_EmpStatus> AMS_EmpStatus { get; set; }
        public virtual DbSet<AMS_AnsarPhoto> AMS_AnsarPhoto { get; set; }
        public virtual DbSet<AMS_AnsarEmpInfo> AMS_AnsarEmpInfo { get; set; }
        public virtual DbSet<vwAMSAnsarEducationalQlfy> vwAMSAnsarEducationalQlfy { get; set; }
        public virtual DbSet<PRM_OrganizationType> PRM_OrganizationType { get; set; }
        public virtual DbSet<AMS_AnsarExperience> AMS_AnsarExperience { get; set; }
        public virtual DbSet<AMS_BlacklistRemoval> AMS_BlacklistRemoval { get; set; }
        public virtual DbSet<PRM_EmploymentInfo> PRM_EmploymentInfo { get; set; }
        public virtual DbSet<SMS_SecurityInfo> SMS_SecurityInfo { get; set; }
        public virtual DbSet<SMS_Organization> SMS_Organization { get; set; }
        public virtual DbSet<AMS_Blacklist> AMS_Blacklist { get; set; }
        public virtual DbSet<AMS_AnsarPersonalInfo> AMS_AnsarPersonalInfo { get; set; }
        public virtual DbSet<AMS_AnsarServiceHistory> AMS_AnsarServiceHistory { get; set; }
        public virtual DbSet<SMS_SecurityServiceHistory> SMS_SecurityServiceHistory { get; set; }
    
        public virtual ObjectResult<sp_AMS_RptCategoryWiseAnsarList_Result> sp_AMS_RptCategoryWiseAnsarList(string zoneList, Nullable<int> categoryId, Nullable<int> statusId, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("CategoryId", categoryId) :
                new ObjectParameter("CategoryId", typeof(int));
    
            var statusIdParameter = statusId.HasValue ?
                new ObjectParameter("StatusId", statusId) :
                new ObjectParameter("StatusId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AMS_RptCategoryWiseAnsarList_Result>("sp_AMS_RptCategoryWiseAnsarList", zoneListParameter, categoryIdParameter, statusIdParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_AMS_RptZoneWiseAnsarList_Result> sp_AMS_RptZoneWiseAnsarList(string zoneList, Nullable<int> statusId, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var statusIdParameter = statusId.HasValue ?
                new ObjectParameter("StatusId", statusId) :
                new ObjectParameter("StatusId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AMS_RptZoneWiseAnsarList_Result>("sp_AMS_RptZoneWiseAnsarList", zoneListParameter, statusIdParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_AMS_RptBlacklistedAnsarList_Result> sp_AMS_RptBlacklistedAnsarList(string zoneList, Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var dateFromParameter = dateFrom.HasValue ?
                new ObjectParameter("DateFrom", dateFrom) :
                new ObjectParameter("DateFrom", typeof(System.DateTime));
    
            var dateToParameter = dateTo.HasValue ?
                new ObjectParameter("DateTo", dateTo) :
                new ObjectParameter("DateTo", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AMS_RptBlacklistedAnsarList_Result>("sp_AMS_RptBlacklistedAnsarList", zoneListParameter, dateFromParameter, dateToParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_AMS_RptReminderLetter_Result> sp_AMS_RptReminderLetter(string zoneList, Nullable<int> reminderTypeId, Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var reminderTypeIdParameter = reminderTypeId.HasValue ?
                new ObjectParameter("ReminderTypeId", reminderTypeId) :
                new ObjectParameter("ReminderTypeId", typeof(int));
    
            var dateFromParameter = dateFrom.HasValue ?
                new ObjectParameter("DateFrom", dateFrom) :
                new ObjectParameter("DateFrom", typeof(System.DateTime));
    
            var dateToParameter = dateTo.HasValue ?
                new ObjectParameter("DateTo", dateTo) :
                new ObjectParameter("DateTo", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AMS_RptReminderLetter_Result>("sp_AMS_RptReminderLetter", zoneListParameter, reminderTypeIdParameter, dateFromParameter, dateToParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_AMS_GetServiceDuration_Result> sp_AMS_GetServiceDuration(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AMS_GetServiceDuration_Result>("sp_AMS_GetServiceDuration", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<sp_SMS_RptOrganizationWiseSecurityPersonnelList_Result> sp_SMS_RptOrganizationWiseSecurityPersonnelList(string zoneList, Nullable<bool> isActive, Nullable<int> orgId, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("IsActive", isActive) :
                new ObjectParameter("IsActive", typeof(bool));
    
            var orgIdParameter = orgId.HasValue ?
                new ObjectParameter("OrgId", orgId) :
                new ObjectParameter("OrgId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SMS_RptOrganizationWiseSecurityPersonnelList_Result>("sp_SMS_RptOrganizationWiseSecurityPersonnelList", zoneListParameter, isActiveParameter, orgIdParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_SMS_RptSecurityPersonnelListWorkingInIntelligence_Result> sp_SMS_RptSecurityPersonnelListWorkingInIntelligence(string zoneList, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SMS_RptSecurityPersonnelListWorkingInIntelligence_Result>("sp_SMS_RptSecurityPersonnelListWorkingInIntelligence", zoneListParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_SMS_RptAwardedOrPunishedSecurityPersonnelList_Result> sp_SMS_RptAwardedOrPunishedSecurityPersonnelList(string zoneList, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SMS_RptAwardedOrPunishedSecurityPersonnelList_Result>("sp_SMS_RptAwardedOrPunishedSecurityPersonnelList", zoneListParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_AMS_RptEmployeeWiseAnsarDetails_Result> sp_AMS_RptEmployeeWiseAnsarDetails(Nullable<int> employeeId, string zoneList, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var employeeIdParameter = employeeId.HasValue ?
                new ObjectParameter("EmployeeId", employeeId) :
                new ObjectParameter("EmployeeId", typeof(int));
    
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_AMS_RptEmployeeWiseAnsarDetails_Result>("sp_AMS_RptEmployeeWiseAnsarDetails", employeeIdParameter, zoneListParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_SMS_RptSecurityPersonnelDetails_Result> sp_SMS_RptSecurityPersonnelDetails(Nullable<int> employeeId, string zoneList, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var employeeIdParameter = employeeId.HasValue ?
                new ObjectParameter("EmployeeId", employeeId) :
                new ObjectParameter("EmployeeId", typeof(int));
    
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SMS_RptSecurityPersonnelDetails_Result>("sp_SMS_RptSecurityPersonnelDetails", employeeIdParameter, zoneListParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_SMS_RptZoneWiseSecurityPersonnelList_Result> sp_SMS_RptZoneWiseSecurityPersonnelList(string zoneList, Nullable<bool> isActive, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("IsActive", isActive) :
                new ObjectParameter("IsActive", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SMS_RptZoneWiseSecurityPersonnelList_Result>("sp_SMS_RptZoneWiseSecurityPersonnelList", zoneListParameter, isActiveParameter, numErrorCode, strErrorMsg);
        }
    
        public virtual ObjectResult<sp_SMS_RptZoneWiseBroadSheet_Result> sp_SMS_RptZoneWiseBroadSheet(string zoneList, Nullable<bool> isActive, Nullable<int> designationId, Nullable<int> districtId, Nullable<int> punishmentId, Nullable<int> rewardId, Nullable<decimal> from, Nullable<decimal> to, Nullable<int> employeeId, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            var zoneListParameter = zoneList != null ?
                new ObjectParameter("zoneList", zoneList) :
                new ObjectParameter("zoneList", typeof(string));
    
            var isActiveParameter = isActive.HasValue ?
                new ObjectParameter("IsActive", isActive) :
                new ObjectParameter("IsActive", typeof(bool));
    
            var designationIdParameter = designationId.HasValue ?
                new ObjectParameter("DesignationId", designationId) :
                new ObjectParameter("DesignationId", typeof(int));
    
            var districtIdParameter = districtId.HasValue ?
                new ObjectParameter("DistrictId", districtId) :
                new ObjectParameter("DistrictId", typeof(int));
    
            var punishmentIdParameter = punishmentId.HasValue ?
                new ObjectParameter("PunishmentId", punishmentId) :
                new ObjectParameter("PunishmentId", typeof(int));
    
            var rewardIdParameter = rewardId.HasValue ?
                new ObjectParameter("RewardId", rewardId) :
                new ObjectParameter("RewardId", typeof(int));
    
            var fromParameter = from.HasValue ?
                new ObjectParameter("From", from) :
                new ObjectParameter("From", typeof(decimal));
    
            var toParameter = to.HasValue ?
                new ObjectParameter("To", to) :
                new ObjectParameter("To", typeof(decimal));
    
            var employeeIdParameter = employeeId.HasValue ?
                new ObjectParameter("EmployeeId", employeeId) :
                new ObjectParameter("EmployeeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SMS_RptZoneWiseBroadSheet_Result>("sp_SMS_RptZoneWiseBroadSheet", zoneListParameter, isActiveParameter, designationIdParameter, districtIdParameter, punishmentIdParameter, rewardIdParameter, fromParameter, toParameter, employeeIdParameter, numErrorCode, strErrorMsg);
        }
    }
}