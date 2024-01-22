using AutoMapper;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel;
using BEPZA_MEDICAL.Web.Areas.SMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class AMSMapper
    {
        public AMSMapper()
        {
            //Common ConfigType
            Mapper.CreateMap<CommonConfigTypeViewModel, BEPZA_MEDICAL.DAL.AMS.CommonConfigType>();
            Mapper.CreateMap<BEPZA_MEDICAL.DAL.AMS.CommonConfigType, CommonConfigTypeViewModel>();

            //Common Config
            Mapper.CreateMap<CommonConfigViewModel, CommonConfigGetResult>();
            Mapper.CreateMap<CommonConfigGetResult, CommonConfigViewModel>();

            //AMS_StayRule
            Mapper.CreateMap<StayRuleViewModel, AMS_StayRule>();
            Mapper.CreateMap<AMS_StayRule, StayRuleViewModel>();

            //AMS_ZoneWiseQuota
            Mapper.CreateMap<ZoneWiseQuotaViewModel, AMS_ZoneWiseQuota>();
            Mapper.CreateMap<AMS_ZoneWiseQuota, ZoneWiseQuotaViewModel>();

            //AMS_DesignationInfo
            Mapper.CreateMap<DesignationInfoViewModel, AMS_DesignationInfo>();
            Mapper.CreateMap<AMS_DesignationInfo, DesignationInfoViewModel>();

            //AMS_AutoReminder
            Mapper.CreateMap<AutoReminderViewModel, AMS_AutoReminder>();
            Mapper.CreateMap<AMS_AutoReminder, AutoReminderViewModel>();

            //AMS_AnsarDegree
            Mapper.CreateMap<AccademicQlfnInfoViewModel, AMS_AnsarDegree>();
            Mapper.CreateMap<AMS_AnsarDegree, AccademicQlfnInfoViewModel>();

            //AMS_AnsarEmpInfo
            Mapper.CreateMap<EmploymentInfoViewModel, AMS_AnsarEmpInfo>();
            Mapper.CreateMap<AMS_AnsarEmpInfo, EmploymentInfoViewModel>();

            //AMS_AnsarExperience
            Mapper.CreateMap<JobExperienceInfoViewModel, AMS_AnsarExperience>();
            Mapper.CreateMap<AMS_AnsarExperience, JobExperienceInfoViewModel>();

            //AMS_AnsarPersonalInfo
            Mapper.CreateMap<PersonalInfoViewModel, AMS_AnsarPersonalInfo>();
            Mapper.CreateMap<AMS_AnsarPersonalInfo, PersonalInfoViewModel>();

            //AMS_AnsarServiceHistory
            Mapper.CreateMap<EmpServiceHistoryViewModel, AMS_AnsarServiceHistory>();
            Mapper.CreateMap<AMS_AnsarServiceHistory, EmpServiceHistoryViewModel>();

            //AMS_AnsarPhoto
            Mapper.CreateMap<AnsarPhotoGraphViewModel, AMS_AnsarPhoto>();
            Mapper.CreateMap<AMS_AnsarPhoto, AnsarPhotoGraphViewModel>();

            //AMS_Blacklist
            Mapper.CreateMap<BlacklistViewModel, AMS_Blacklist>();
            Mapper.CreateMap<AMS_Blacklist, BlacklistViewModel>();

            //AMS_BlacklistRemoval
            Mapper.CreateMap<BlacklistRemovalViewModel, AMS_BlacklistRemoval>();
            Mapper.CreateMap<AMS_BlacklistRemoval, BlacklistRemovalViewModel>();

            //SMS_SecurityInfo
            Mapper.CreateMap<SecurityPersonnelEmpInfoViewModel, SMS_SecurityInfo>();
            Mapper.CreateMap<SMS_SecurityInfo, SecurityPersonnelEmpInfoViewModel>();

            //SMS_SecurityServiceHistory
            Mapper.CreateMap<SecurityServiceHistoryViewModel, SMS_SecurityServiceHistory>();
            Mapper.CreateMap<SMS_SecurityServiceHistory, SecurityServiceHistoryViewModel>();
        }
    }
}