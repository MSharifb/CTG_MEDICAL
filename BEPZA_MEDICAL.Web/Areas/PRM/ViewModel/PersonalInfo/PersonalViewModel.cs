
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo;
namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Personal
{
    public class PersonalViewModel
    {
        #region Constructor

        public PersonalViewModel()
        {
            PersonalInfo = new PersonalInfoViewModel();
            AccademicQlfnInfo = new AccademicQlfnInfoViewModel();
            JobExperienceInfo = new JobExperienceInfoViewModel();
            ProfessionalTrainingInfo = new ProfessionalTrainingInfoViewModel();
            ProfessionalCertificationInfo = new ProfessionalCertificationInfoViewModel();
            ProfessionalLicenseInfo = new ProfessionalLicenseInfoViewModel();
            JobSkillInfo = new  JobSkillInfoViewModel();
            PersonalPublication = new PersonalPublicationViewModel();
            PersonalLanguageEfficiency = new PersonalLanguageEfficiencyViewModel();
            PersonalFamilyMemberInformation = new PersonalFamilyMemberInformationViewModel();
            PersonalNominee = new PersonalNomineeViewModel();
            PersonalEmergencyContract = new PersonalEmergencyContractViewModel();
            VisaInformation = new VisaInfoViewModel();
            ReferenceInformation = new ReferenceInfoViewModel();
            EmployeeAttachment = new EmpAttachmentViewModel();
            EmpLeverageViewModel = new EmpLeverageViewModel();
            EmployeeWealthStatementViewModel = new EmployeeWealthStatementViewModel();
            EmpServiceHistoryViewModel = new EmpServiceHistoryViewModel();
            ForeignTourInfoViewModel = new ForeignTourInfoViewModel();
            LifeInsuranceViewModel = new LifeInsuranceViewModel();
        }

        #endregion

        #region Standerd Property

        public PersonalInfoViewModel PersonalInfo { get; set; }
        public AccademicQlfnInfoViewModel AccademicQlfnInfo { get; set; }
        public JobExperienceInfoViewModel JobExperienceInfo { get; set; }
        public ProfessionalTrainingInfoViewModel ProfessionalTrainingInfo { get; set; }
        public ProfessionalCertificationInfoViewModel ProfessionalCertificationInfo { get; set; }
        public ProfessionalLicenseInfoViewModel ProfessionalLicenseInfo { get; set; }
        public JobSkillInfoViewModel JobSkillInfo { get; set; }
        public PersonalEmergencyContractViewModel PersonalEmergencyContract { get; set; }
        public PersonalFamilyMemberInformationViewModel PersonalFamilyMemberInformation { get; set; }
        public PersonalLanguageEfficiencyViewModel PersonalLanguageEfficiency { get; set; }
        public PersonalNomineeViewModel PersonalNominee { get; set; }
        public PersonalPublicationViewModel PersonalPublication { get; set; }
        public VisaInfoViewModel VisaInformation { get; set; }
        public EmpAttachmentViewModel EmployeeAttachment { get; set; }
        public ReferenceInfoViewModel ReferenceInformation { get; set; }
        public EmpLeverageViewModel EmpLeverageViewModel { get; set; }
        public EmployeeWealthStatementViewModel EmployeeWealthStatementViewModel { get; set; }
        public EmpServiceHistoryViewModel EmpServiceHistoryViewModel { get; set; }
        public ForeignTourInfoViewModel ForeignTourInfoViewModel { get; set; }
        public LifeInsuranceViewModel LifeInsuranceViewModel { get; set; }

        public string ViewType { get; set; }
        public int EmployeeId { get; set; }

        #endregion

    }
}