using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BEPZA_MEDICAL.DAL.AMS
{
    public class AMS_UnitOfWork
    {
        #region Fields

        AMS_ExecuteFunctions _functionRepository;
        AMS_GenericRepository<CommonConfigType> _configTypeRepository;
        AMS_GenericRepository<PRM_OrganizationType> _organizationTypeRepository;
        AMS_GenericRepository<AMS_AutoReminder> _autoReminderRepository;
        AMS_GenericRepository<AMS_Category> _ansarCategoryRepository;
        AMS_GenericRepository<AMS_DesignationInfo> _ansarDesignationRepository;
        AMS_GenericRepository<AMS_ReminderType> _reminderTypeRepository;
        AMS_GenericRepository<AMS_StayRule> _stayRuleRepository;
        AMS_GenericRepository<AMS_ZoneWiseQuota> _zoneWiseQuotaRepository;
        AMS_GenericRepository<AMS_EmpStatus> _empStatusRepository;
        AMS_GenericRepository<AMS_AnsarDegree> _ansarDegreeRepository;
        AMS_GenericRepository<AMS_AnsarEmpInfo> _ansarEmpInfoRepository;
        AMS_GenericRepository<AMS_AnsarExperience> _ansarExperienceRepository;
        AMS_GenericRepository<AMS_AnsarPersonalInfo> _ansarPersonalInfoRepository;
        AMS_GenericRepository<AMS_AnsarServiceHistory> _ansarServiceHistoryRepository;
        AMS_GenericRepository<AMS_AnsarPhoto> _ansarPhotographRepository;
        AMS_GenericRepository<vwAMSAnsarEducationalQlfy> _vwAnsarEducationalQlfyRepository;
        AMS_GenericRepository<AMS_Blacklist> _blacklistRepository;
        AMS_GenericRepository<AMS_BlacklistRemoval> _blacklistRemovalRepository;
        AMS_GenericRepository<SMS_SecurityInfo> _securityInfoRepository;
        AMS_GenericRepository<SMS_SecurityServiceHistory> _securityServiceHistoryRepository;
        AMS_GenericRepository<SMS_Organization> _securityOrganizationRepository;

        #endregion

        #region Constactor

        public AMS_UnitOfWork(
            AMS_ExecuteFunctions functionRepository,
            AMS_GenericRepository<CommonConfigType> configTypeRepository,
            AMS_GenericRepository<PRM_OrganizationType> organizationTypeRepository,
            AMS_GenericRepository<AMS_AutoReminder> autoReminderRepository,
            AMS_GenericRepository<AMS_Category> ansarCategoryRepository,
            AMS_GenericRepository<AMS_DesignationInfo> ansarDesignationRepository,
            AMS_GenericRepository<AMS_ReminderType> reminderTypeRepository,
            AMS_GenericRepository<AMS_StayRule> stayRuleRepository,
            AMS_GenericRepository<AMS_ZoneWiseQuota> zoneWiseQuotaRepository,
            AMS_GenericRepository<AMS_EmpStatus> empStatusRepository,
            AMS_GenericRepository<AMS_AnsarDegree> ansarDegreeRepository,
            AMS_GenericRepository<AMS_AnsarEmpInfo> ansarEmpInfoRepository,
            AMS_GenericRepository<AMS_AnsarExperience> ansarExperienceRepository,
            AMS_GenericRepository<AMS_AnsarPersonalInfo> ansarPersonalInfoRepository,
            AMS_GenericRepository<AMS_AnsarServiceHistory> ansarServiceHistoryRepository,
            AMS_GenericRepository<AMS_AnsarPhoto> ansarPhotographRepository,
            AMS_GenericRepository<vwAMSAnsarEducationalQlfy> vwAnsarEducationalQlfyRepository,
            AMS_GenericRepository<AMS_Blacklist> blacklistRepository,
            AMS_GenericRepository<AMS_BlacklistRemoval> blacklistRemovalRepository,
            AMS_GenericRepository<SMS_SecurityInfo> securityInfoRepository,
            AMS_GenericRepository<SMS_SecurityServiceHistory> securityServiceHistoryRepository,
            AMS_GenericRepository<SMS_Organization> securityOrganizationRepository
            
            )
        {
            this._functionRepository = functionRepository;
            this._configTypeRepository = configTypeRepository;
            this._organizationTypeRepository = organizationTypeRepository;
            this._autoReminderRepository = autoReminderRepository;
            this._ansarCategoryRepository = ansarCategoryRepository;
            this._ansarDesignationRepository = ansarDesignationRepository;
            this._reminderTypeRepository = reminderTypeRepository;
            this._stayRuleRepository = stayRuleRepository;
            this._zoneWiseQuotaRepository = zoneWiseQuotaRepository;
            this._empStatusRepository = empStatusRepository;
            this._ansarDegreeRepository = ansarDegreeRepository;
            this._ansarEmpInfoRepository = ansarEmpInfoRepository;
            this._ansarExperienceRepository = ansarExperienceRepository;
            this._ansarPersonalInfoRepository = ansarPersonalInfoRepository;
            this._ansarServiceHistoryRepository = ansarServiceHistoryRepository;
            this._ansarPhotographRepository = ansarPhotographRepository;
            this._vwAnsarEducationalQlfyRepository = vwAnsarEducationalQlfyRepository;
            this._blacklistRepository = blacklistRepository;
            this._blacklistRemovalRepository = blacklistRemovalRepository;
            this._securityInfoRepository = securityInfoRepository;
            this._securityServiceHistoryRepository = securityServiceHistoryRepository;
            this._securityOrganizationRepository = securityOrganizationRepository;
        }

        #endregion

        #region Properties

        public AMS_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        public AMS_GenericRepository<CommonConfigType> ConfigTypeRepository
        {
            get
            {
                return _configTypeRepository;
            }
        }

        public AMS_GenericRepository<PRM_OrganizationType> OrganizationTypeRepository
        {
            get
            {
                return _organizationTypeRepository;
            }
        }

        public AMS_GenericRepository<AMS_AutoReminder> AutoReminderRepository
        {
            get
            {
                return _autoReminderRepository;
            }
        }

        public AMS_GenericRepository<AMS_Category> AnsarCategoryRepository
        {
            get
            {
                return _ansarCategoryRepository;
            }
        }

        public AMS_GenericRepository<AMS_DesignationInfo> AnsarDesignationRepository
        {
            get
            {
                return _ansarDesignationRepository;
            }
        }

        public AMS_GenericRepository<AMS_ReminderType> ReminderTypeRepository
        {
            get
            {
                return _reminderTypeRepository;
            }
        }

        public AMS_GenericRepository<AMS_StayRule> StayRuleRepository
        {
            get
            {
                return _stayRuleRepository;
            }
        }

        public AMS_GenericRepository<AMS_ZoneWiseQuota> ZoneWiseQuotaRepository
        {
            get
            {
                return _zoneWiseQuotaRepository;
            }
        }

        public AMS_GenericRepository<AMS_EmpStatus> EmpStatusRepository
        {
            get
            {
                return _empStatusRepository;
            }
        }

        public AMS_GenericRepository<AMS_AnsarDegree> AnsarDegreeRepository
        {
            get
            {
                return _ansarDegreeRepository;
            }
        }

        public AMS_GenericRepository<AMS_AnsarEmpInfo> AnsarEmpInfoRepository
        {
            get
            {
                return _ansarEmpInfoRepository;
            }
        }

        public AMS_GenericRepository<AMS_AnsarExperience> AnsarExperienceRepository
        {
            get
            {
                return _ansarExperienceRepository;
            }
        }

        public AMS_GenericRepository<AMS_AnsarPersonalInfo> AnsarPersonalInfoRepository
        {
            get
            {
                return _ansarPersonalInfoRepository;
            }
        }

        public AMS_GenericRepository<AMS_AnsarServiceHistory> AnsarServiceHistoryRepository
        {
            get
            {
                return _ansarServiceHistoryRepository;
            }
        }

        public AMS_GenericRepository<AMS_AnsarPhoto> AnsarPhotographRepository
        {
            get
            {
                return _ansarPhotographRepository;
            }
        }

        public AMS_GenericRepository<vwAMSAnsarEducationalQlfy> vwAnsarEducationalQlfyRepository
        {
            get
            {
                return _vwAnsarEducationalQlfyRepository;
            }
        }

        public AMS_GenericRepository<AMS_Blacklist> BlacklistRepository
        {
            get
            {
                return _blacklistRepository;
            }
        }

        public AMS_GenericRepository<AMS_BlacklistRemoval> BlacklistRemovalRepository
        {
            get
            {
                return _blacklistRemovalRepository;
            }
        }

        public AMS_GenericRepository<SMS_SecurityInfo> SecurityInfoRepository
        {
            get
            {
                return _securityInfoRepository;
            }
        }

        public AMS_GenericRepository<SMS_SecurityServiceHistory> SecurityServiceHistoryRepository
        {
            get
            {
                return _securityServiceHistoryRepository;
            }
        }

        public AMS_GenericRepository<SMS_Organization> SecurityOrganizationRepository
        {
            get
            {
                return _securityOrganizationRepository;
            }
        }
        
        #endregion
    }


}