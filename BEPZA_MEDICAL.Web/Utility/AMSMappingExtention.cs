using AutoMapper;
using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Areas.AMS.ViewModel;
using BEPZA_MEDICAL.Web.Areas.SMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class AMSMappingExtention
    {
        #region Common Config Type
        public static List<CommonConfigTypeViewModel> ToModelList(this List<BEPZA_MEDICAL.DAL.AMS.CommonConfigType> objlist)
        {
            List<CommonConfigTypeViewModel> list = new List<CommonConfigTypeViewModel>();
            foreach (var item in objlist)
            {
                list.Add((Mapper.Map<BEPZA_MEDICAL.DAL.AMS.CommonConfigType, CommonConfigTypeViewModel>(item)));
            }
            return list;
        }
        public static List<BEPZA_MEDICAL.DAL.AMS.CommonConfigType> ToEntityList(this List<CommonConfigTypeViewModel> modellist)
        {
            List<BEPZA_MEDICAL.DAL.AMS.CommonConfigType> list = new List<BEPZA_MEDICAL.DAL.AMS.CommonConfigType>();
            foreach (var item in modellist)
            {
                list.Add((Mapper.Map<CommonConfigTypeViewModel, BEPZA_MEDICAL.DAL.AMS.CommonConfigType>(item)));
            }
            return list;
        }

        #endregion

        #region Common Config
        public static CommonConfigViewModel ToModel(this CommonConfigGetResult obj)
        {
            return Mapper.Map<CommonConfigGetResult, CommonConfigViewModel>(obj);
        }
        public static CommonConfigGetResult ToEntity(this CommonConfigViewModel model)
        {
            return Mapper.Map<CommonConfigViewModel, CommonConfigGetResult>(model);
        }

        #endregion

        #region AMS_StayRule mapping extension
        public static StayRuleViewModel ToModel(this AMS_StayRule entity)
        {
            return Mapper.Map<AMS_StayRule, StayRuleViewModel>(entity);
        }
        public static AMS_StayRule ToEntity(this StayRuleViewModel model)
        {
            return Mapper.Map<StayRuleViewModel, AMS_StayRule>(model);
        }

        #endregion

        #region AMS_ZoneWiseQuota mapping extension
        public static ZoneWiseQuotaViewModel ToModel(this AMS_ZoneWiseQuota entity)
        {
            return Mapper.Map<AMS_ZoneWiseQuota, ZoneWiseQuotaViewModel>(entity);
        }
        public static AMS_ZoneWiseQuota ToEntity(this ZoneWiseQuotaViewModel model)
        {
            return Mapper.Map<ZoneWiseQuotaViewModel, AMS_ZoneWiseQuota>(model);
        }

        #endregion

        #region AMS_DesignationInfo mapping extension
        public static DesignationInfoViewModel ToModel(this AMS_DesignationInfo entity)
        {
            return Mapper.Map<AMS_DesignationInfo, DesignationInfoViewModel>(entity);
        }
        public static AMS_DesignationInfo ToEntity(this DesignationInfoViewModel model)
        {
            return Mapper.Map<DesignationInfoViewModel, AMS_DesignationInfo>(model);
        }

        #endregion

        #region AMS_AutoReminder mapping extension
        public static AutoReminderViewModel ToModel(this AMS_AutoReminder entity)
        {
            return Mapper.Map<AMS_AutoReminder, AutoReminderViewModel>(entity);
        }
        public static AMS_AutoReminder ToEntity(this AutoReminderViewModel model)
        {
            return Mapper.Map<AutoReminderViewModel, AMS_AutoReminder>(model);
        }

        #endregion

        #region AMS_AnsarDegree mapping extension
        public static AccademicQlfnInfoViewModel ToModel(this AMS_AnsarDegree entity)
        {
            return Mapper.Map<AMS_AnsarDegree, AccademicQlfnInfoViewModel>(entity);
        }
        public static AMS_AnsarDegree ToEntity(this AccademicQlfnInfoViewModel model)
        {
            return Mapper.Map<AccademicQlfnInfoViewModel, AMS_AnsarDegree>(model);
        }

        #endregion

        #region AMS_AnsarEmpInfo mapping extension
        public static EmploymentInfoViewModel ToModel(this AMS_AnsarEmpInfo entity)
        {
            return Mapper.Map<AMS_AnsarEmpInfo, EmploymentInfoViewModel>(entity);
        }
        public static AMS_AnsarEmpInfo ToEntity(this EmploymentInfoViewModel model)
        {
            return Mapper.Map<EmploymentInfoViewModel, AMS_AnsarEmpInfo>(model);
        }

        #endregion

        #region AMS_AnsarExperience mapping extension
        public static JobExperienceInfoViewModel ToModel(this AMS_AnsarExperience entity)
        {
            return Mapper.Map<AMS_AnsarExperience, JobExperienceInfoViewModel>(entity);
        }
        public static AMS_AnsarExperience ToEntity(this JobExperienceInfoViewModel model)
        {
            return Mapper.Map<JobExperienceInfoViewModel, AMS_AnsarExperience>(model);
        }

        #endregion

        #region AMS_AnsarPersonalInfo mapping extension
        public static PersonalInfoViewModel ToModel(this AMS_AnsarPersonalInfo entity)
        {
            return Mapper.Map<AMS_AnsarPersonalInfo, PersonalInfoViewModel>(entity);
        }
        public static AMS_AnsarPersonalInfo ToEntity(this PersonalInfoViewModel model)
        {
            return Mapper.Map<PersonalInfoViewModel, AMS_AnsarPersonalInfo>(model);
        }

        #endregion

        #region AMS_AnsarServiceHistory mapping extension
        public static EmpServiceHistoryViewModel ToModel(this AMS_AnsarServiceHistory entity)
        {
            return Mapper.Map<AMS_AnsarServiceHistory, EmpServiceHistoryViewModel>(entity);
        }
        public static AMS_AnsarServiceHistory ToEntity(this EmpServiceHistoryViewModel model)
        {
            return Mapper.Map<EmpServiceHistoryViewModel, AMS_AnsarServiceHistory>(model);
        }

        #endregion

        #region AMS_AnsarPhoto mapping extension
        public static AnsarPhotoGraphViewModel ToModel(this AMS_AnsarPhoto entity)
        {
            return Mapper.Map<AMS_AnsarPhoto, AnsarPhotoGraphViewModel>(entity);
        }
        public static AMS_AnsarPhoto ToEntity(this AnsarPhotoGraphViewModel model)
        {
            return Mapper.Map<AnsarPhotoGraphViewModel, AMS_AnsarPhoto>(model);
        }

        #endregion

        #region AMS_Blacklist mapping extension
        public static BlacklistViewModel ToModel(this AMS_Blacklist entity)
        {
            return Mapper.Map<AMS_Blacklist, BlacklistViewModel>(entity);
        }
        public static AMS_Blacklist ToEntity(this BlacklistViewModel model)
        {
            return Mapper.Map<BlacklistViewModel, AMS_Blacklist>(model);
        }

        #endregion

        #region AMS_BlacklistRemoval mapping extension
        public static BlacklistRemovalViewModel ToModel(this AMS_BlacklistRemoval entity)
        {
            return Mapper.Map<AMS_BlacklistRemoval, BlacklistRemovalViewModel>(entity);
        }
        public static AMS_BlacklistRemoval ToEntity(this BlacklistRemovalViewModel model)
        {
            return Mapper.Map<BlacklistRemovalViewModel, AMS_BlacklistRemoval>(model);
        }

        #endregion

        #region SMS_SecurityInfo mapping extension
        public static SecurityPersonnelEmpInfoViewModel ToModel(this SMS_SecurityInfo entity)
        {
            return Mapper.Map<SMS_SecurityInfo, SecurityPersonnelEmpInfoViewModel>(entity);
        }
        public static SMS_SecurityInfo ToEntity(this SecurityPersonnelEmpInfoViewModel model)
        {
            return Mapper.Map<SecurityPersonnelEmpInfoViewModel, SMS_SecurityInfo>(model);
        }

        #endregion

        #region SMS_SecurityServiceHistory mapping extension
        public static SecurityServiceHistoryViewModel ToModel(this SMS_SecurityServiceHistory entity)
        {
            return Mapper.Map<SMS_SecurityServiceHistory, SecurityServiceHistoryViewModel>(entity);
        }
        public static SMS_SecurityServiceHistory ToEntity(this SecurityServiceHistoryViewModel model)
        {
            return Mapper.Map<SecurityServiceHistoryViewModel, SMS_SecurityServiceHistory>(model);
        }

        #endregion
    }
}