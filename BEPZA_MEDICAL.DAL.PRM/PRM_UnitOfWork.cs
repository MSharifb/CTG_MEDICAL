using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.DAL.PRM
{
    public class PRM_UnitOfWork
    {
        #region Fields

        PRM_GenericRepository<PRM_Designation> _designationRepository;
        PRM_GenericRepository<PRM_JobGrade> _jobGradeRepository;
        PRM_GenericRepository<PRM_GradeStep> _jobGradeStepRepository;
        PRM_GenericRepository<PRM_StaffCategory> _staffCategoryRepository;
        PRM_GenericRepository<PRM_GradeStep> _gradeStepRepository;
        PRM_GenericRepository<PRM_Country> _countryRepository;
        PRM_GenericRepository<PRM_VisaType> _visaTypeRepository;
        PRM_GenericRepository<PRM_LicenseType> _licenseType;
        PRM_GenericRepository<PRM_EmpTrainingInfo> _professionalTraining;
        PRM_GenericRepository<PRM_EmpCertification> _certification;
        PRM_GenericRepository<PRM_EmpLicenseInfo> _license;
        PRM_GenericRepository<PRM_EmpJobSkill> _jobSkill;
        PRM_GenericRepository<PRM_JobSkill> _jobSkillName;
        PRM_GenericRepository<PRM_JobSkillLevel> _jobSkillLevel;
        PRM_GenericRepository<PRM_CertificationCategory> _certificationCategory;
        PRM_GenericRepository<PRM_CountryDivision> _countryDivisionRepository;
        PRM_GenericRepository<PRM_DivisionHeadMaping> _divisionHeadaping;
        PRM_GenericRepository<PRM_District> _districtRepository;
        PRM_GenericRepository<PRM_Thana> _thanaRepository;
        PRM_GenericRepository<PRM_EmploymentInfo> _employmentInfo;
        PRM_GenericRepository<PRM_EmpSalary> _empSalary;
        PRM_GenericRepository<PRM_EmpSalaryDetail> _empSalaryDetails;
        PRM_GenericRepository<PRM_EmpPersonalInfo> _personalInfo;
        PRM_GenericRepository<PRM_EmpDegree> _accademicQlfn;
        PRM_GenericRepository<PRM_EmpExperience> _jobExperienceInfo;
        PRM_GenericRepository<PRM_SubjectGroup> _subjectGroup;
        PRM_GenericRepository<CommonConfigType> _commonConfigType;
        PRM_GenericRepository<PRM_SalaryHeadGroup> _salaryHeadGroupRepository;
        PRM_GenericRepository<PRM_Division> _divisionRepository;
        PRM_GenericRepository<PRM_MaritalStatus> _maritalStatus;
        PRM_GenericRepository<PRM_BloodGroup> _bloodGroup;
        PRM_GenericRepository<PRM_JobLocation> _jobLocationRepository;
        PRM_GenericRepository<PRM_EmploymentType> _employmentTypeRepository;
        PRM_GenericRepository<PRM_ResourceLevel> _resourceLevelRepository;
        PRM_GenericRepository<PRM_SalaryHead> _salaryHeadRepository;
        PRM_GenericRepository<PRM_NameTitle> _nameTitle;
        PRM_GenericRepository<PRM_Nationality> _nationality;
        PRM_GenericRepository<PRM_Discipline> _empDiscipline;
        PRM_GenericRepository<PRM_ShiftName> _shiftName;
        PRM_GenericRepository<PRM_BankName> _bankName;
        PRM_GenericRepository<PRM_OrganizationType> _organizationType;
        PRM_GenericRepository<PRM_EmploymentStatus> _employmentStatus;
        PRM_GenericRepository<PRM_ResourceCategory> _resourceCategory;
        PRM_GenericRepository<PRM_ResourceType> _ResourceType;
        PRM_GenericRepository<PRM_ResourceInfo> _ResourceInfo;
        PRM_GenericRepository<PRM_MeasurementUnit> _MeasurementUnit;
        PRM_GenericRepository<PRM_BankBranch> _bankBranch;
        
        PRM_GenericRepository<PRM_EmpPhoto> _employeePhotographRepository;
        PRM_GenericRepository<PRM_DegreeLevel> _examDegreeLavel;
        PRM_GenericRepository<PRM_EmpContractInfo> _employmentContractInfo;
        PRM_GenericRepository<PRM_AcademicGrade> _academicGrade;
        PRM_GenericRepository<PRM_UniversityAndBoard> _universityAndBoard;
        PRM_GenericRepository<PRM_CertificationInstitute> _certificationInstitute;
        PRM_GenericRepository<vwAccademicQlfy> _vwAccademicQlfy;
        //PRM_GenericRepository<PRM_EmpTransferInfo> _EmployeeTransferInfo;
        PRM_GenericRepository<PRM_EmpLanguageEfficiency> _employeeLanguageEfficiency;
        PRM_GenericRepository<PRM_Language> _language;
        PRM_GenericRepository<PRM_ProefficiencyLevel> _efficencyLevel;
        PRM_GenericRepository<PRM_EmpStatusChange> _EmpStatusChange;
        PRM_GenericRepository<PRM_Relation> _relation;
        PRM_GenericRepository<PRM_Profession> _profession;
        PRM_GenericRepository<PRM_EmpFamilyMemberInfo> _familyMemberInformation;
        PRM_GenericRepository<PRM_EmpContractPersonInfo> _emergencyContractPerson;
        PRM_GenericRepository<PRM_EmpSeperation> _EmpSeperation;
        PRM_GenericRepository<PRM_HumanResourceMapping> _HumanResourceMapping;
        PRM_GenericRepository<PRM_NomineeFor> _employeeNomineeFor;
        PRM_GenericRepository<PRM_EmpNominee> _employeeNominee;
        PRM_GenericRepository<PRM_EmpNomineeDetail> _employeeNomineeDetails;
        PRM_GenericRepository<PRM_EmpReferanceGuarantor> _empReferenceGuarantor;
        PRM_GenericRepository<PRM_EmpAttachmentFile> _empAttachment;
        PRM_GenericRepository<PRM_AttachmentType> _empAttachmentType;
        PRM_GenericRepository<PRM_PublicationArea> _publicationArea;
        PRM_GenericRepository<PRM_EmpPublicationInfo> _employePublication;
        PRM_GenericRepository<PRM_EmpVisaPassportInfo> _employeeVisaInformation;
        PRM_GenericRepository<PRM_CompanyInfo> _companyInformation;
        PRM_GenericRepository<PRM_Religion> _religion;
        PRM_GenericRepository<PRM_EmpContactFiles> _attachmentContactFiles;
        PRM_GenericRepository<LMS_tblLeaveType> _leaveType;
        PRM_GenericRepository<PRM_HumanResourceRateMaster> _humanResourceRateMaster;
        PRM_GenericRepository<PRM_HumanResourceRateDetail> _humanResourceRateDetail;
        PRM_GenericRepository<PRM_HumanResourceRateAssignMaster> _humanResourceRateAssignMaster;
        PRM_GenericRepository<PRM_HumanResourceRateAssignDetail> _humanResourceRateAssignDetail;

        PRM_ExecuteFunctions _functionRepository;
       
        PRM_GenericRepository<PRM_EmployeeActivationHistory> _empActivationHistory;
        PRM_GenericRepository<PRM_OrganogramLevel> _organogramLevel;
        PRM_GenericRepository<PRM_OrganogramType> _organogramType;
        PRM_GenericRepository<PRM_SalaryScale> _salaryScale;
        PRM_GenericRepository<PRM_OrganizationalSetupManpowerInfo> _organizationalSetupManpowerInfo;
        PRM_GenericRepository<PRM_QuotaName> _quotaName;
        PRM_GenericRepository<PRM_EmployeeClass> _employeeClass;
        PRM_GenericRepository<PRM_EmploymentProcess> _employmentProcess;
        PRM_GenericRepository<PRM_EmpLeverage> _empLeverage;
        PRM_GenericRepository<PRM_EmpWealthStatementInfo> _empWealthStatement;
        PRM_GenericRepository<PRM_AssetType> _assetType;
        PRM_GenericRepository<PRM_TrainingType> _trainingType;
        PRM_GenericRepository<PRM_Location> _location;
        PRM_GenericRepository<PRM_ClearanceForm> _clearanceFormRepository;
        PRM_GenericRepository<PRM_ClearanceChecklist> _clearanceChecklistRepository;
        PRM_GenericRepository<PRM_ClearanceChecklistDetail> _clearanceChecklistDetailRepository;
        PRM_GenericRepository<PRM_RecruitmentQualificationInfo> _recruitmentQualificationInfoRepository;
        PRM_GenericRepository<PRM_RecruitmentQualificationDetails> _recruitmentQualificationDetailRepository;
        PRM_GenericRepository<PRM_EmpClearanceInfo> _empClearanceInfoRepository;
        PRM_GenericRepository<PRM_EmpClearanceInfoFormDetail> _empClearanceInfoFormDetailRepository;
        PRM_GenericRepository<PRM_EmpClearanceInfoCheklistDetail> _empClearanceInfoChecklistDetailRepository;
        PRM_GenericRepository<PRM_QuotaInfo> _quotaInfoRepository;
        PRM_GenericRepository<PRM_DistrictQuota> _districtQuotaRepository;
        PRM_GenericRepository<PRM_Section> _sectionRepository;
        PRM_GenericRepository<PRM_JobRequisitionInfo> _jobRequisitionInfoRepository;
        PRM_GenericRepository<PRM_JobRequisitionInfoDetail> _jobRequisitionInfoDetailRepository;
        PRM_GenericRepository<PRM_OrganogramTypeMapping> _organogramTypeMappingRepository;
        PRM_GenericRepository<PRM_ApplicantInfo> _applicantInfoRepository;
        PRM_GenericRepository<PRM_ApplicantInfoQualification> _applicantInfoQualificationRepository;
        PRM_GenericRepository<PRM_JobAdvertisementInfo> _jobAdvertisementInfoRepository;
        PRM_GenericRepository<PRM_JobRequisitionInfoSummary> _jobRequisitionInfoSummaryRepository;
        PRM_GenericRepository<PRM_JobRequisitionInfoSummaryDetail> _jobRequisitionInfoSummaryDetailRepository;
        PRM_GenericRepository<PRM_JobRequisitionInfoApproval> _jobRequisitionInfoApprovalRepository;
        PRM_GenericRepository<PRM_JobRequisitionInfoApprovalDetail> _jobRequisitionInfoApprovalDetailRepository;
        PRM_GenericRepository<PRM_ClearanceInfoFromMinistry> _clearanceFromMinistryRepository;
        PRM_GenericRepository<PRM_ClearanceInfoFromMinistryDetail> _clearanceFromMinistryDetailRepository;
        PRM_GenericRepository<PRM_ApplicantShortList> _applicantShortListRepository;
        PRM_GenericRepository<PRM_ApplicantShortListDetail> _applicantShortListDetailRepository;
        PRM_GenericRepository<PRM_ApplicantShortListApproval> _applicantShortListApprovalRepository;
        PRM_GenericRepository<PRM_ApplicantShortListApprovalDetail> _applicantShortListApprovalDetailRepository;
        PRM_GenericRepository<PRM_Media> _mediaRepository;
        PRM_GenericRepository<PRM_JobAdvertisementInfoDetailMedia> _jobAdvertisementInfoDetailMediaRepository;
        PRM_GenericRepository<PRM_JobAdvertisementInfoDetailRequisition> _jobAdvertisementInfoDetailRequisitionRepository;
        PRM_GenericRepository<PRM_JobAdvertisementInfoDetailAttachment> _jobAdvertisementInfoDetailAttachmentRepository;
        PRM_GenericRepository<PRM_SelectionCritariaOrExamType> _selectionCritariaOrExamTypeRepository;
        PRM_GenericRepository<PRM_SelectionCriteria> _selectionCriteriaRepository;
        PRM_GenericRepository<PRM_SelectionCriteriaDetail> _selectionCriteriaDetailRepository;
        PRM_GenericRepository<PRM_SelectionBoardInfo> _selectionBoardInfoRepository;
        PRM_GenericRepository<PRM_SelectionBoardInfoCommittee> _selectionBoardInfoCommitteeRepository;
        PRM_GenericRepository<PRM_ApplicantInterviewCardIssue> _applicantInterviewCardIssueRepository;
        PRM_GenericRepository<PRM_ApplicantInterviewCardIssueDetail> _applicantInterviewCardIssueDetailRepository;
        PRM_GenericRepository<PRM_Region> _regionRepository;
        PRM_GenericRepository<PRM_TestResultforApplicantInfo> _testResultforApplicantInfoRepository;
        PRM_GenericRepository<PRM_TestResultforApplicantInfoDetail> _testResultforApplicantInfoDetailRepository;
        PRM_GenericRepository<PRM_SelectedApplicantInfoApproval> _selectedApplicantInfoApprovalRepository;
        PRM_GenericRepository<PRM_SelectedApplicantInfoApprovalDetail> _selectedApplicantInfoApprovalDetailRepository;
        PRM_GenericRepository<PRM_SelectedApplicantInfo> _selectedApplicantInfoRepository;
        PRM_GenericRepository<PRM_SelectedApplicantInfoDetail> _selectedApplicantInfoDetailRepository;
        PRM_GenericRepository<PRM_AppointmentLetterInfo> _appointmentLetterInfoRepository;
        PRM_GenericRepository<PRM_OrderTypeInfo> _orderTypeInfoRepository;
        PRM_GenericRepository<PRM_DisciplinaryActionType> _disciplinaryActionTypeRepository;
        PRM_GenericRepository<PRM_ACRCriteriaInformation> _acrCriteriaInformationRepository;
        PRM_GenericRepository<PRM_PunishmentTypeInfo> _punishmentTypeInfoRepository;
        PRM_GenericRepository<PRM_PunishmentTypeInfoDetail> _punishmentTypeInfoDetailRepository;
        PRM_GenericRepository<PRM_ACRAttributesInformation> _acrAttributesInfoRepository;
        PRM_GenericRepository<PRM_ACRAttributesInformationDetail> _acrAttributesInfoDetailRepository;
        PRM_GenericRepository<PRM_ACRRankInformation> _acrRankInformationRepository;
        PRM_GenericRepository<PRM_ComplaintNoteSheet> _complaintNoteSheetRepository;
        PRM_GenericRepository<PRM_FIRInfo> _firInfoRepository;
        PRM_GenericRepository<PRM_ChargeSheetInfo> _chargeSheetInfoRepository;
        PRM_GenericRepository<PRM_EmpACROfficerInfo> _acrOfficerInfoRepository;
        PRM_GenericRepository<PRM_EmpACROfficerHealthTestReport> _acrOfficerHealthTestReportRepository;
        PRM_GenericRepository<PRM_NoteOrderInfo> _noteOrderInfoReportRepository;
        PRM_GenericRepository<PRM_NoticeInfo> _noticeInfoReportRepository;
        PRM_GenericRepository<PRM_EmpACROfficerBioData> _acrOfficerBioDataRepository;
        PRM_GenericRepository<PRM_EmpACRGraphAndRecommendation> _acrGraphAndRecommendationRepository;
        PRM_GenericRepository<PRM_EmpACRReviewingOfficerComments> _acrReviewingOfficerCommentsRepository;
        PRM_GenericRepository<PRM_EmpACRInformationForAuthority> _acrInformationForAuthorityRepository;
        PRM_GenericRepository<PRM_NoticeType> _noticeTypeRepository;
        PRM_GenericRepository<PRM_ExplanationReceivedInfo> _explanationReceivedInfoRepository;
        PRM_GenericRepository<PRM_HearingFixationInfo> _hearingFixationInfoRepository;
        PRM_GenericRepository<PRM_HearingFixationInfoDetail> _hearingFixationInfoDetailRepository;
        PRM_GenericRepository<PRM_EmpACRPersonalCharacteristics> _acrPersonalCharacteristicsRepository;
        PRM_GenericRepository<PRM_EmpACRPersonalCharacteristicsDetail> _acrPersonalCharacteristicsDetailRepository;
        PRM_GenericRepository<PRM_EmpACRPerformanceOfWork> _acrPerformanceOfWorkRepository;
        PRM_GenericRepository<PRM_EmpACRPerformanceOfWorkDetail> _acrPerformanceOfWorkDetailRepository;
        PRM_GenericRepository<PRM_EmpACRStaffInfo> _acrStaffInfoRepository;
        PRM_GenericRepository<PRM_HearingInfo> _hearingInfoRepository;
        PRM_GenericRepository<PRM_EmpACRStaffBioData> _acrStaffBioDataRepository;
        PRM_GenericRepository<PRM_InvestigationCommitteeInfo> _investigationCommitteeInfoRepository;
        PRM_GenericRepository<PRM_InvestigationCommitteeInfoMemberInfo> _investigationCommitteeInfoMemberInfoRepository;
        PRM_GenericRepository<PRM_InvestigationCommitteeInfoFormedFor> _investigationCommitteeInfoFormedForRepository;
        PRM_GenericRepository<PRM_EmpACRAssessmentInfo> _acrAssessmentInfoRepository;
        PRM_GenericRepository<PRM_EmpACRAssessmentInfoDetail> _acrAssessmentInfoDetailRepository;
        PRM_GenericRepository<PRM_OfficeEquipmentFurnitureInfo> _officeEquipmentFurnitureInfoRepository;
        PRM_GenericRepository<PRM_InvestigationReport> _investigationReportRepository;
        PRM_GenericRepository<PRM_AppealReviewInfo> _appealReviewInfoRepository;
        PRM_GenericRepository<PRM_OfficeEquipmentFurnitureUsagesInfo> _officeEquipmentFurnitureUsagesInfoRepository;
        PRM_GenericRepository<PRM_OfficeEquipmentFurnitureUsagesInfoDetail> _officeEquipmentFurnitureUsagesInfoDetailRepository;
        PRM_GenericRepository<PRM_NotesAndDocumentInfo> _notesAndDocumentInfoRepository;
        PRM_GenericRepository<PRM_NotesAndDocumentInfoAttachmentDetail> _notesAndDocumentInfoAttachmentDetailRepository;
        PRM_GenericRepository<PRM_NotesAndDocumentInfoCommentsDetail> _notesAndDocumentInfoCommentsDetailRepository;
        PRM_GenericRepository<PRM_SuspensionOfEmployee> _suspensionOfEmployeeRepository;
        PRM_GenericRepository<PRM_SuspensionOfEmployeeDetail> _suspensionOfEmployeeDetailRepository;
        PRM_GenericRepository<PRM_AcceptanceLetterInfo> _acceptanceLetterInfoRepository;
        PRM_GenericRepository<PRM_DesignationHistory> _designationHistoryRepository;
        PRM_GenericRepository<PRM_DesignationHistoryDetail> _designationHistoryDetailRepository;
        PRM_GenericRepository<PRM_PunishmentRestriction> _punishmentRestrictionRepository;
        PRM_GenericRepository<PRM_StatusDesignationInfo> _statusDesignationInfoRepository;
        PRM_GenericRepository<PRM_EmpForeignTourInfo> _foreignTourInfoRepository;
        PRM_GenericRepository<PRM_ZoneInfo> _zoneInfoRepository;
        PRM_GenericRepository<PRM_LicenseCategory> _licenseCategory;
        PRM_GenericRepository<PRM_RetirementAgeInfo> _retirementAgeInfoRepository;
        PRM_GenericRepository<PRM_EmpLifeInsurance> _lifeInsuranceRepository;
        PRM_GenericRepository<PRM_OrganogramCategoryType> _organogramCategoryTypeRepository;
        PRM_GenericRepository<PRM_DegreeType> _degreeTypeRepository;
        PRM_GenericRepository<PRM_DegreeLevelMapping> _degreeLevelMappingRepository;
        PRM_GenericRepository<PRM_DegreeLevelMappingDetail> _degreeLevelMappingDetailRepository;
        PRM_GenericRepository<PRM_QueryAnalyzerTable> _queryAnalyzerTableRepository;
        PRM_GenericRepository<PRM_QueryAnalyzerTableItems> _queryAnalyzerTableItemsRepository;
        PRM_GenericRepository<PRM_PassportType> _passPortTypeRepository;
        PRM_GenericRepository<PRM_ProfessionalCertificate> _professionalCertificateRepository;
        PRM_GenericRepository<PRM_RecruitmentEducationalQualification> _recruitmentEducationalQualificationRepository;
        PRM_GenericRepository<PRM_RecruitmentJobExperience> _recruitmentJobExperienceRepository;
        PRM_GenericRepository<PRM_RecruitmentSubjectOrGroup> _recruitmentSubjectOrGroupRepository;
        PRM_GenericRepository<PRM_JobAdvertisementPostDetail> _jobAdvertisementPostDetailRepository;
        private readonly PRM_GenericRepository<PGM_Configuration> _pgmConfiguration;

        private readonly PRM_GenericRepository<vwPRMOrganogramZoneDetail> _vwOrganogramZoneDetail;

        /* Notification */
        private readonly PRM_GenericRepository<NTF_Notification> _notification;
        private readonly PRM_GenericRepository<NTF_NotificationFlow> _notificationFlow;
        private readonly PRM_GenericRepository<NTF_NotificationFlowSetup> _notificationFlowSetup;
        private readonly PRM_GenericRepository<NTF_NotificationReadBy> _notificationReadBy;

        private readonly PRM_GenericRepository<vwNotificationFlowSetup> _vwNotificationFlowSetup;

        /* Approval System */
        PRM_GenericRepository<APV_ApprovalGroup> _ApprovalGroupRepository;
        PRM_GenericRepository<APV_ApprovalProcess> _ApprovalProcessRepository;
        PRM_GenericRepository<APV_ApprovalStep> _ApprovalStepRepository;
        PRM_GenericRepository<APV_ApprovalFlowMaster> _ApprovalFlowMasterRepository;
        PRM_GenericRepository<APV_ApprovalFlowDetail> _ApprovalFlowDetailRepository;
        PRM_GenericRepository<APV_ApproverType> _ApproverTypeRepository;
        PRM_GenericRepository<APV_ApproverAuthorType> _ApproverAuthorTypeRepository;
        PRM_GenericRepository<APV_ApproverInfo> _ApproverInfoRepository;
        PRM_GenericRepository<APV_EmployeeWiseApproverInfo> _EmployeeWiseApproverInfoRepository;
        PRM_GenericRepository<vwAPVAssignedApprovalFlow> _AssignedApprovalFlowViewRepository;
        PRM_GenericRepository<vwAPVApproverInformation> _ApproverInformationViewRepository;
        PRM_GenericRepository<APV_ApprovalStatus> _ApprovalStatusRepository;
        PRM_GenericRepository<APV_GetApproverInfoByApplicant_Result> _GetApproverRepository;
        PRM_GenericRepository<APV_ApplicationInformation> _RequestedApplicationInformationRepository;
        PRM_GenericRepository<vwApvApplicationWiseApprovalStatu> _ApprovalHisotryRepository;
        PRM_GenericRepository<APV_MessageVariableInformation> _MessageVariableRepository;
        PRM_GenericRepository<APV_ApprovalFlowConfiguration> _ApprovalFlowConfigurationRepository;


        /* e-Recruitment */
        PRM_GenericRepository<EREC_tblgeneralinfo> _eRectblgeneralinfoRepository;
        PRM_GenericRepository<EREC_tbleducationalinfo> _eRectbleducationalinfoRepository;
        PRM_GenericRepository<EREC_tblmaritalinfo> _eRectblmaritalinfoRepository;
        PRM_GenericRepository<EREC_tbladdress> _eRectbladdressRepository;

        PRM_GenericRepository<EREC_tblCountry> _eRectblCountryRepository;
        PRM_GenericRepository<EREC_tblDistrict> _eRectblDistrictRepository;
        PRM_GenericRepository<EREC_tblExamination> _eRectblExaminationRepository;
        PRM_GenericRepository<EREC_tblUpazila> _eRectblUpazilaRepository;
        PRM_GenericRepository<PRM_JobAdvertisementInfoDistrict> _JobAdvertisementDestrict;

        //SMS sending
        PRM_GenericRepository<PRM_EmpSmsHistory> _empSmsHistory;
        PRM_GenericRepository<PRM_UserLoginHistory> _userLoginHistory;

        //Employee Service History 
        PRM_GenericRepository<PRM_EmpServiceHistory> _empServiceHistory;

        PRM_GenericRepository<PRM_Practice> _practice;

        #endregion

        #region Constactor

        public PRM_UnitOfWork(
            PRM_GenericRepository<CommonConfigType> configTypeRepository,
            PRM_GenericRepository<PRM_HumanResourceRateMaster> humanResourceRateMaster,
            PRM_GenericRepository<PRM_HumanResourceRateDetail> humanResourceRateDetail,
            PRM_GenericRepository<PRM_HumanResourceRateAssignMaster> humanResourceRateAssignMaster,
            PRM_GenericRepository<PRM_HumanResourceRateAssignDetail> humanResourceRateAssignDetail,
            PRM_GenericRepository<PRM_JobGrade> jobGradeRepository,
            PRM_GenericRepository<PRM_GradeStep> jobGradeStepRepository,
            PRM_GenericRepository<PRM_Designation> designationRepository,
            PRM_GenericRepository<PRM_StaffCategory> staffCategoryRepository,
            PRM_GenericRepository<PRM_GradeStep> gradeStepRepository,
            PRM_GenericRepository<PRM_Country> countryRepository,
            PRM_GenericRepository<PRM_VisaType> visaTypeRepository,
            PRM_GenericRepository<PRM_LicenseType> licenseType,
            PRM_GenericRepository<PRM_EmpTrainingInfo> professionalTraining,
            PRM_GenericRepository<PRM_EmpCertification> certification,
            PRM_GenericRepository<PRM_EmpLicenseInfo> license,
            PRM_GenericRepository<PRM_EmpJobSkill> jobSkill,
            PRM_GenericRepository<PRM_JobSkill> jobSkillName,
            PRM_GenericRepository<PRM_JobSkillLevel> jobSkillLevel,
            PRM_GenericRepository<PRM_CertificationCategory> certificationCategory,
            PRM_GenericRepository<PRM_CountryDivision> countryDivisionRepository,
            PRM_GenericRepository<PRM_DivisionHeadMaping> divisionHeadMaping,
            PRM_GenericRepository<PRM_District> districtRepository,
            PRM_GenericRepository<PRM_Thana> thanaRepository,
            PRM_GenericRepository<PRM_EmploymentInfo> employmentInfo,
            PRM_GenericRepository<PRM_EmpSalary> empSalary,
            PRM_GenericRepository<PRM_EmpPersonalInfo> personalInfo,
            PRM_GenericRepository<PRM_EmpDegree> accademicQlfn,
            PRM_GenericRepository<PRM_EmpExperience> jobExperienceInfo,
            PRM_GenericRepository<PRM_SubjectGroup> subjectGroup,
            PRM_GenericRepository<PRM_SalaryHeadGroup> salaryHeadGroupRepository,
            PRM_GenericRepository<PRM_Division> divisionRepository,
            PRM_GenericRepository<PRM_MaritalStatus> maritalStatus,
            PRM_GenericRepository<PRM_BloodGroup> bloodGroup,
            PRM_GenericRepository<PRM_JobLocation> jobLocationRepository,
            PRM_GenericRepository<PRM_EmploymentType> employmentTypeRepository,
            PRM_GenericRepository<PRM_ResourceLevel> resourceLevelRepository,
            PRM_GenericRepository<PRM_SalaryHead> salaryHeadRepository,
            PRM_GenericRepository<PRM_NameTitle> nameTitle,
            PRM_GenericRepository<PRM_Nationality> nationality,
            PRM_GenericRepository<PRM_Discipline> empDiscipline,
            PRM_GenericRepository<PRM_ShiftName> shiftName,
            PRM_GenericRepository<PRM_BankName> bankName,
            PRM_GenericRepository<PRM_OrganizationType> organizationType,
            PRM_GenericRepository<PRM_EmploymentStatus> employmentStatus,
            PRM_GenericRepository<PRM_ResourceCategory> resourceCategory,
            PRM_GenericRepository<PRM_ResourceType> ResourceType,
            PRM_GenericRepository<PRM_ResourceInfo> ResourceInfo,
            PRM_GenericRepository<PRM_MeasurementUnit> MeasurementUnit,
            PRM_GenericRepository<PRM_BankBranch> bankBranch,

            PRM_GenericRepository<PRM_EmpPhoto> employeePhotographRepository,
            PRM_GenericRepository<PRM_EmpContractInfo> employmentContractInfo,
            PRM_GenericRepository<PRM_DegreeLevel> examDegreeLavel,
            PRM_GenericRepository<PRM_AcademicGrade> result,
            PRM_GenericRepository<PRM_UniversityAndBoard> universityAndBoard,
            PRM_GenericRepository<PRM_CertificationInstitute> certificationInstitute,
            PRM_GenericRepository<vwAccademicQlfy> vwaccademicQlfy,
            //PRM_GenericRepository<PRM_EmpTransferInfo> EmployeeTransferInfo,
            PRM_GenericRepository<PRM_EmpSalaryDetail> empSalaryDetails,
            PRM_GenericRepository<PRM_EmpLanguageEfficiency> employeeLanguageEfficiency,
            PRM_GenericRepository<PRM_Language> language,
            PRM_GenericRepository<PRM_ProefficiencyLevel> efficencyLevel,
            PRM_GenericRepository<PRM_EmpStatusChange> EmpStatusChange,
            PRM_GenericRepository<PRM_Relation> relation,
            PRM_GenericRepository<PRM_Profession> profession,
            PRM_GenericRepository<PRM_EmpFamilyMemberInfo> familyMemberInformation,
            PRM_GenericRepository<PRM_EmpContractPersonInfo> emergencyContractPerson,
            PRM_GenericRepository<PRM_EmpSeperation> EmpSeperation,
            PRM_GenericRepository<PRM_HumanResourceMapping> HumanResourceMapping,
            PRM_GenericRepository<PRM_NomineeFor> employeeNomineeFor,
            PRM_GenericRepository<PRM_EmpNominee> employeeNominee,
            PRM_GenericRepository<PRM_EmpNomineeDetail> employeeNomineeDetails,
            PRM_GenericRepository<PRM_EmpReferanceGuarantor> empReferenceGuarantor,
            PRM_GenericRepository<PRM_EmpAttachmentFile> empAttachment,
            PRM_GenericRepository<PRM_AttachmentType> empAttachmentType,
            PRM_GenericRepository<PRM_PublicationArea> publicationArea,
            PRM_GenericRepository<PRM_EmpPublicationInfo> employePublication,
            PRM_GenericRepository<PRM_EmpVisaPassportInfo> employeeVisaInformation,
            PRM_GenericRepository<PRM_CompanyInfo> companyInformation,
            PRM_GenericRepository<PRM_Religion> religion,
            PRM_GenericRepository<PRM_EmpContactFiles> attachmentContactFiles,
            PRM_ExecuteFunctions functionRepository,
            PRM_GenericRepository<LMS_tblLeaveType> leaveType,           
            PRM_GenericRepository<PRM_EmployeeActivationHistory> empActivationRepository,
            PRM_GenericRepository<PRM_OrganogramLevel> organogramLevel,
            PRM_GenericRepository<PRM_OrganogramType> organogramType,
            PRM_GenericRepository<PRM_SalaryScale> salaryScale,
            PRM_GenericRepository<PRM_OrganizationalSetupManpowerInfo> organizationalSetupManpowerInfo,
            PRM_GenericRepository<PRM_QuotaName> quotaName,
            PRM_GenericRepository<PRM_EmployeeClass> employeeClass,
            PRM_GenericRepository<PRM_EmploymentProcess> employmentProcess,
            PRM_GenericRepository<PRM_EmpLeverage> empLeverage,
            PRM_GenericRepository<PRM_EmpWealthStatementInfo> empWealthStatement,
            PRM_GenericRepository<PRM_AssetType> assetType,
            PRM_GenericRepository<PRM_TrainingType> trainingType,
            PRM_GenericRepository<PRM_Location> location,
            PRM_GenericRepository<PRM_ClearanceForm> clearanceFormRepository,
            PRM_GenericRepository<PRM_ClearanceChecklist> clearanceChecklistRepository,
            PRM_GenericRepository<PRM_ClearanceChecklistDetail> clearanceChecklistDetailRepository,
            PRM_GenericRepository<PRM_RecruitmentQualificationInfo> recruitmentQualificationInfoRepository,
            PRM_GenericRepository<PRM_RecruitmentQualificationDetails> recruitmentQualificationDetailRepository,
            PRM_GenericRepository<PRM_EmpClearanceInfo> empClearanceInfoRepository,
            PRM_GenericRepository<PRM_EmpClearanceInfoFormDetail> empClearanceInfoFormDetailRepository,
            PRM_GenericRepository<PRM_EmpClearanceInfoCheklistDetail> empClearanceInfoChecklistDetailRepository,
            PRM_GenericRepository<PRM_QuotaInfo> quotaInfoRepository,
            PRM_GenericRepository<PRM_DistrictQuota> districtQuotaRepository,
            PRM_GenericRepository<PRM_Section> sectionRepository,
            PRM_GenericRepository<PRM_JobRequisitionInfo> jobRequisitionInfoRepository,
            PRM_GenericRepository<PRM_JobRequisitionInfoDetail> jobRequisitionInfoDetailRepository,
            PRM_GenericRepository<PRM_OrganogramTypeMapping> OrganogramTypeMappingRepository,
            PRM_GenericRepository<PRM_ApplicantInfo> ApplicantInfoRepository,
            PRM_GenericRepository<PRM_ApplicantInfoQualification> ApplicantInfoQualificationRepository,
            PRM_GenericRepository<PRM_JobAdvertisementInfo> JobAdvertisementInfoRepository,
            PRM_GenericRepository<PRM_JobRequisitionInfoSummary> jobRequisitionInfoSummaryRepository,
            PRM_GenericRepository<PRM_JobRequisitionInfoSummaryDetail> jobRequisitionInfoSummaryDetailRepository,
            PRM_GenericRepository<PRM_JobRequisitionInfoApproval> jobRequisitionInfoApprovalRepository,
            PRM_GenericRepository<PRM_JobRequisitionInfoApprovalDetail> jobRequisitionInfoApprovalDetailRepository,
            PRM_GenericRepository<PRM_ClearanceInfoFromMinistry> clearanceFromMinistryRepository,
            PRM_GenericRepository<PRM_ClearanceInfoFromMinistryDetail> clearanceFromMinistryDetailRepository,
            PRM_GenericRepository<PRM_ApplicantShortList> applicantShortListRepository,
            PRM_GenericRepository<PRM_ApplicantShortListDetail> applicantShortListDetailRepository,
            PRM_GenericRepository<PRM_ApplicantShortListApproval> applicantShortListApprovalRepository,
            PRM_GenericRepository<PRM_ApplicantShortListApprovalDetail> applicantShortListApprovalDetailRepository,
            PRM_GenericRepository<PRM_Media> mediaRepository,
            PRM_GenericRepository<PRM_JobAdvertisementInfoDetailMedia> jobAdvertisementInfoDetailMediaRepository,
            PRM_GenericRepository<PRM_JobAdvertisementInfoDetailRequisition> jobAdvertisementInfoDetailRequisitionRepository,
            PRM_GenericRepository<PRM_JobAdvertisementInfoDetailAttachment> jobAdvertisementInfoDetailAttachmentRepository,
            PRM_GenericRepository<PRM_SelectionCritariaOrExamType> selectionCritariaOrExamTypeRepository,
            PRM_GenericRepository<PRM_SelectionCriteria> selectionCriteriaRepository,
            PRM_GenericRepository<PRM_SelectionCriteriaDetail> selectionCriteriaDetailRepository,
            PRM_GenericRepository<PRM_SelectionBoardInfo> selectionBoardInfoRepository,
            PRM_GenericRepository<PRM_SelectionBoardInfoCommittee> selectionBoardInfoCommitteeRepository,
            PRM_GenericRepository<PRM_ApplicantInterviewCardIssue> applicantInterviewCardIssueRepository,
            PRM_GenericRepository<PRM_ApplicantInterviewCardIssueDetail> applicantInterviewCardIssueDetailRepository,
            PRM_GenericRepository<PRM_Region> regionRepository,
            PRM_GenericRepository<PRM_TestResultforApplicantInfo> testResultforApplicantInfoRepository,
            PRM_GenericRepository<PRM_TestResultforApplicantInfoDetail> testResultforApplicantInfoDetailRepository,
            PRM_GenericRepository<PRM_SelectedApplicantInfoApproval> selectedApplicantInfoApprovalRepository,
            PRM_GenericRepository<PRM_SelectedApplicantInfoApprovalDetail> selectedApplicantInfoApprovalDetailRepository,
            PRM_GenericRepository<PRM_SelectedApplicantInfo> selectedApplicantInfoRepository,
            PRM_GenericRepository<PRM_SelectedApplicantInfoDetail> selectedApplicantInfoDetailRepository,
            PRM_GenericRepository<PRM_AppointmentLetterInfo> appointmentLetterInfoRepository,
            PRM_GenericRepository<PRM_OrderTypeInfo> orderTypeInfoRepository,
            PRM_GenericRepository<PRM_DisciplinaryActionType> disciplinaryActionTypeRepository,
            PRM_GenericRepository<PRM_ACRCriteriaInformation> acrCriteriaInformationRepository,
            PRM_GenericRepository<PRM_PunishmentTypeInfo> punishmentTypeInfoRepository,
            PRM_GenericRepository<PRM_PunishmentTypeInfoDetail> punishmentTypeInfoDetailRepository,
            PRM_GenericRepository<PRM_ACRAttributesInformation> acrAttributesInfoRepository,
            PRM_GenericRepository<PRM_ACRAttributesInformationDetail> acrAttributesInfoDetailRepository,
            PRM_GenericRepository<PRM_ACRRankInformation> acrRankInformationRepository,
            PRM_GenericRepository<PRM_ComplaintNoteSheet> complaintNoteSheetRepository,
            PRM_GenericRepository<PRM_FIRInfo> firInfoRepository,
            PRM_GenericRepository<PRM_ChargeSheetInfo> chargeSheetInfoRepository,
            PRM_GenericRepository<PRM_EmpACROfficerInfo> acrOfficerInfoRepository,
            PRM_GenericRepository<PRM_EmpACROfficerHealthTestReport> acrOfficerHealthTestReportRepository,
            PRM_GenericRepository<PRM_NoteOrderInfo> noteOrderInfoReportRepository,
            PRM_GenericRepository<PRM_NoticeInfo> noticeInfoReportRepository,
            PRM_GenericRepository<PRM_EmpACROfficerBioData> acrOfficerBioDataRepository,
            PRM_GenericRepository<PRM_EmpACRGraphAndRecommendation> acrGraphAndRecommendationRepository,
            PRM_GenericRepository<PRM_EmpACRReviewingOfficerComments> acrReviewingOfficerCommentsRepository,
            PRM_GenericRepository<PRM_EmpACRInformationForAuthority> acrInformationForAuthorityRepository,
            PRM_GenericRepository<PRM_NoticeType> noticeTypeRepository,
            PRM_GenericRepository<PRM_ExplanationReceivedInfo> explanationReceivedInfoRepository,
            PRM_GenericRepository<PRM_HearingFixationInfo> hearingFixationInfoRepository,
            PRM_GenericRepository<PRM_HearingFixationInfoDetail> hearingFixationInfoDetailRepository,
            PRM_GenericRepository<PRM_EmpACRPersonalCharacteristics> acrPersonalCharacteristicsRepository,
            PRM_GenericRepository<PRM_EmpACRPersonalCharacteristicsDetail> acrPersonalCharacteristicsDetailRepository,
            PRM_GenericRepository<PRM_EmpACRPerformanceOfWork> acrPerformanceOfWorkRepository,
            PRM_GenericRepository<PRM_EmpACRPerformanceOfWorkDetail> acrPerformanceOfWorkDetailRepository,
            PRM_GenericRepository<PRM_EmpACRStaffInfo> acrStaffInfoRepository,
            PRM_GenericRepository<PRM_HearingInfo> hearingInfoRepository,
            PRM_GenericRepository<PRM_EmpACRStaffBioData> acrStaffBioDataRepository,
            PRM_GenericRepository<PRM_InvestigationCommitteeInfo> investigationCommitteeInfoRepository,
            PRM_GenericRepository<PRM_InvestigationCommitteeInfoMemberInfo> investigationCommitteeInfoMemberInfoRepository,
            PRM_GenericRepository<PRM_InvestigationCommitteeInfoFormedFor> investigationCommitteeInfoFormedForRepository,
            PRM_GenericRepository<PRM_EmpACRAssessmentInfo> acrAssessmentInfoRepository,
            PRM_GenericRepository<PRM_EmpACRAssessmentInfoDetail> acrAssessmentInfoDetailRepository,
            PRM_GenericRepository<PRM_OfficeEquipmentFurnitureInfo> officeEquipmentFurnitureInfoRepository,
            PRM_GenericRepository<PRM_InvestigationReport> investigationReportRepository,
            PRM_GenericRepository<PRM_AppealReviewInfo> appealReviewInfoRepository,
            PRM_GenericRepository<PRM_OfficeEquipmentFurnitureUsagesInfo> officeEquipmentFurnitureUsagesInfoRepository,
            PRM_GenericRepository<PRM_OfficeEquipmentFurnitureUsagesInfoDetail> officeEquipmentFurnitureUsagesInfoDetailRepository,
            PRM_GenericRepository<PRM_NotesAndDocumentInfo> notesAndDocumentInfoRepository,
            PRM_GenericRepository<PRM_NotesAndDocumentInfoAttachmentDetail> notesAndDocumentInfoAttachmentDetailRepository,
            PRM_GenericRepository<PRM_NotesAndDocumentInfoCommentsDetail> notesAndDocumentInfoCommentsDetailRepository,
            PRM_GenericRepository<PRM_SuspensionOfEmployee> suspensionOfEmployeeRepository,
            PRM_GenericRepository<PRM_SuspensionOfEmployeeDetail> suspensionOfEmployeeDetailRepository,
            PRM_GenericRepository<PRM_AcceptanceLetterInfo> acceptanceLetterInfoRepository,
            PRM_GenericRepository<PRM_DesignationHistory> designationHistoryRepository,
            PRM_GenericRepository<PRM_DesignationHistoryDetail> designationHistoryDetailRepository,
            PRM_GenericRepository<PRM_PunishmentRestriction> punishmentRestrictionRepository,
            PRM_GenericRepository<PRM_StatusDesignationInfo> statusDesignationInfoRepository,
            PRM_GenericRepository<PRM_EmpForeignTourInfo> foreignTourInfoRepository,
            PRM_GenericRepository<PRM_ZoneInfo> zoneRepository,
            PRM_GenericRepository<PRM_LicenseCategory> licenseCategory,
            PRM_GenericRepository<PRM_RetirementAgeInfo> retirementAgeInfoRepository,
            PRM_GenericRepository<PRM_EmpLifeInsurance> lifeInsuranceRepository,
            PRM_GenericRepository<PRM_OrganogramCategoryType> organogramCategoryTypeRepository,
            PRM_GenericRepository<PRM_DegreeType> degreeTypeRepository,
            PRM_GenericRepository<PRM_DegreeLevelMapping> degreeLevelMappingRepository,
            PRM_GenericRepository<PRM_DegreeLevelMappingDetail> degreeLevelMappingDetailRepository,
            PRM_GenericRepository<PRM_QueryAnalyzerTable> queryAnalyzerTableRepository,
            PRM_GenericRepository<PRM_QueryAnalyzerTableItems> queryAnalyzerTableItemsRepository,
            PRM_GenericRepository<PRM_PassportType> passPortTypeRepository,
            PRM_GenericRepository<PRM_ProfessionalCertificate> professionalCertificateRepository,
            PRM_GenericRepository<PRM_RecruitmentEducationalQualification> recruitmentEducationalQualificationRepository,
            PRM_GenericRepository<PRM_RecruitmentJobExperience> recruitmentJobExperienceRepository,
            PRM_GenericRepository<PRM_RecruitmentSubjectOrGroup> recruitmentSubjectOrGroupRepository,
            PRM_GenericRepository<PRM_JobAdvertisementPostDetail> jobAdvertisementPostDetailRepository,

            PRM_GenericRepository<APV_ApprovalGroup> ApprovalGroupRepository,
            PRM_GenericRepository<APV_ApprovalProcess> ApprovalProcessRepository,
            PRM_GenericRepository<APV_ApprovalStep> ApprovalStepRepository,
            PRM_GenericRepository<APV_ApprovalFlowMaster> ApprovalFlowMasterRepository,
            PRM_GenericRepository<APV_ApprovalFlowDetail> ApprovalFlowDetailRepository,
            PRM_GenericRepository<APV_ApproverType> ApproverTypeRepository,
            PRM_GenericRepository<APV_ApproverAuthorType> ApproverAuthorTypeRepository,
            PRM_GenericRepository<APV_ApproverInfo> ApproverInfoRepository,
            PRM_GenericRepository<APV_EmployeeWiseApproverInfo> EmployeeWiseApproverInfoRepository,
            PRM_GenericRepository<vwAPVAssignedApprovalFlow> AssignedApprovalFlowViewRepository,
            PRM_GenericRepository<vwAPVApproverInformation> ApproverInformationViewRepository,
            PRM_GenericRepository<APV_ApprovalStatus> ApprovalStatusRepository,
            PRM_GenericRepository<APV_GetApproverInfoByApplicant_Result> GetApproverRepository,
            PRM_GenericRepository<APV_ApplicationInformation> RequestedApplicationInformationRepository,
            PRM_GenericRepository<vwApvApplicationWiseApprovalStatu> ApprovalHisotryRepository,
            PRM_GenericRepository<APV_MessageVariableInformation> MessageVariableRepository,

            PRM_GenericRepository<EREC_tblgeneralinfo> eRectblgeneralinfoRepository,
            PRM_GenericRepository<EREC_tbleducationalinfo> eRectbleducationalinfoRepository,
            PRM_GenericRepository<EREC_tblmaritalinfo> eRectblmaritalinfoRepository,
            PRM_GenericRepository<EREC_tbladdress> EREC_tbladdress,
            PRM_GenericRepository<EREC_tblCountry> eRectblCountryRepository,
            PRM_GenericRepository<EREC_tblDistrict> eRectblDistrictRepository,
            PRM_GenericRepository<EREC_tblExamination> eRectblExaminationRepository,
            PRM_GenericRepository<EREC_tblUpazila> eRectblUpazilaRepository,
            PRM_GenericRepository<PRM_JobAdvertisementInfoDistrict> JobAdvertisementDestrict,
            PRM_GenericRepository<PGM_Configuration> pgmconfiguration,

            PRM_GenericRepository<vwPRMOrganogramZoneDetail> VWPRMOrganogramZoneDetail,

            PRM_GenericRepository<NTF_Notification> notification,
            PRM_GenericRepository<NTF_NotificationFlow> notificationFlow,
            PRM_GenericRepository<NTF_NotificationFlowSetup> notificationFlowSetup,
            PRM_GenericRepository<NTF_NotificationReadBy> notificationReadBy,
            PRM_GenericRepository<vwNotificationFlowSetup> vwNotificationFlowSetup,

            PRM_GenericRepository<PRM_EmpSmsHistory> empSmsHistory,
            PRM_GenericRepository<PRM_UserLoginHistory> userLoginHistory,

            PRM_GenericRepository<APV_ApprovalFlowConfiguration> ApprovalFlowConfigurationRepository,

            PRM_GenericRepository<PRM_EmpServiceHistory> empServiceHistory,

            PRM_GenericRepository<PRM_Practice> practice
            )
        {


            this._humanResourceRateMaster = humanResourceRateMaster;
            this._humanResourceRateDetail = humanResourceRateDetail;

            this._humanResourceRateAssignMaster = humanResourceRateAssignMaster;
            this._humanResourceRateAssignDetail = humanResourceRateAssignDetail;

            this._commonConfigType = configTypeRepository;
            this._jobGradeRepository = jobGradeRepository;
            this._jobGradeStepRepository = jobGradeStepRepository;
            this._designationRepository = designationRepository;
            this._staffCategoryRepository = staffCategoryRepository;
            this._gradeStepRepository = gradeStepRepository;
            this._countryRepository = countryRepository;
            this._visaTypeRepository = visaTypeRepository;
            this._licenseType = licenseType;
            this._professionalTraining = professionalTraining;
            this._certification = certification;
            this._license = license;
            this._jobSkill = jobSkill;
            this._jobSkillName = jobSkillName;
            this._jobSkillLevel = jobSkillLevel;
            this._certificationCategory = certificationCategory;
            this._countryDivisionRepository = countryDivisionRepository;
            this._divisionHeadaping = divisionHeadMaping;
            this._districtRepository = districtRepository;
            this._thanaRepository = thanaRepository;
            this._employmentInfo = employmentInfo;
            this._empSalary = empSalary;
            this._personalInfo = personalInfo;
            this._accademicQlfn = accademicQlfn;
            this._jobExperienceInfo = jobExperienceInfo;
            this._subjectGroup = subjectGroup;
            this._salaryHeadGroupRepository = salaryHeadGroupRepository;
            this._divisionRepository = divisionRepository;
            this._maritalStatus = maritalStatus;
            this._bloodGroup = bloodGroup;
            this._jobLocationRepository = jobLocationRepository;
            this._employmentTypeRepository = employmentTypeRepository;
            this._resourceLevelRepository = resourceLevelRepository;
            this._salaryHeadRepository = salaryHeadRepository;
            this._nameTitle = nameTitle;
            this._nationality = nationality;
            this._empDiscipline = empDiscipline;
            this._shiftName = shiftName;
            this._bankName = bankName;
            this._organizationType = organizationType;
            this._employmentStatus = employmentStatus;
            this._resourceCategory = resourceCategory;
            this._ResourceType = ResourceType;
            this._ResourceInfo = ResourceInfo;
            this._MeasurementUnit = MeasurementUnit;
            this._bankBranch = bankBranch;

            this._employeePhotographRepository = employeePhotographRepository;
            this._employmentContractInfo = employmentContractInfo;
            this._examDegreeLavel = examDegreeLavel;
            this._functionRepository = functionRepository;
            this._universityAndBoard = universityAndBoard;
            this._certificationInstitute = certificationInstitute;
            this._academicGrade = result;
            this._accademicQlfn = accademicQlfn;
            this._vwAccademicQlfy = vwaccademicQlfy;
            //this._EmployeeTransferInfo = EmployeeTransferInfo;
            this._empSalaryDetails = empSalaryDetails;
            this._language = language;
            this._efficencyLevel = efficencyLevel;
            this._employeeLanguageEfficiency = employeeLanguageEfficiency;
            this._EmpStatusChange = EmpStatusChange;
            this._relation = relation;
            this._profession = profession;
            this._familyMemberInformation = familyMemberInformation;
            this._emergencyContractPerson = emergencyContractPerson;
            this._EmpSeperation = EmpSeperation;
            this._HumanResourceMapping = HumanResourceMapping;
            this._employeeNomineeFor = employeeNomineeFor;
            this._employeeNominee = employeeNominee;
            this._employeeNomineeDetails = employeeNomineeDetails;
            this._empReferenceGuarantor = empReferenceGuarantor;
            this._empAttachment = empAttachment;
            this._empAttachmentType = empAttachmentType;
            this._publicationArea = publicationArea;
            this._employePublication = employePublication;
            this._employeeVisaInformation = employeeVisaInformation;
            this._companyInformation = companyInformation;
            this._religion = religion;
            this._attachmentContactFiles = attachmentContactFiles;
            this._leaveType = leaveType;          
            this._empActivationHistory = empActivationRepository;
            this._organogramType = organogramType;
            this._organogramLevel = organogramLevel;
            this._salaryScale = salaryScale;
            this._organizationalSetupManpowerInfo = organizationalSetupManpowerInfo;
            this._quotaName = quotaName;
            this._employeeClass = employeeClass;
            this._employmentProcess = employmentProcess;
            this._empLeverage = empLeverage;
            this._empWealthStatement = empWealthStatement;
            this._assetType = assetType;
            this._trainingType = trainingType;
            this._location = location;
            this._clearanceFormRepository = clearanceFormRepository;
            this._clearanceChecklistRepository = clearanceChecklistRepository;
            this._clearanceChecklistDetailRepository = clearanceChecklistDetailRepository;
            this._recruitmentQualificationInfoRepository = recruitmentQualificationInfoRepository;
            this._recruitmentQualificationDetailRepository = recruitmentQualificationDetailRepository;
            this._empClearanceInfoRepository = empClearanceInfoRepository;
            this._empClearanceInfoFormDetailRepository = empClearanceInfoFormDetailRepository;
            this._empClearanceInfoChecklistDetailRepository = empClearanceInfoChecklistDetailRepository;
            this._quotaInfoRepository = quotaInfoRepository;
            this._districtQuotaRepository = districtQuotaRepository;
            this._sectionRepository = sectionRepository;
            this._jobRequisitionInfoRepository = jobRequisitionInfoRepository;
            this._jobRequisitionInfoDetailRepository = jobRequisitionInfoDetailRepository;
            this._organogramTypeMappingRepository = OrganogramTypeMappingRepository;
            this._applicantInfoRepository = ApplicantInfoRepository;
            this._applicantInfoQualificationRepository = ApplicantInfoQualificationRepository;
            this._jobAdvertisementInfoRepository = JobAdvertisementInfoRepository;
            this._jobRequisitionInfoSummaryRepository = jobRequisitionInfoSummaryRepository;
            this._jobRequisitionInfoSummaryDetailRepository = jobRequisitionInfoSummaryDetailRepository;
            this._jobRequisitionInfoApprovalRepository = jobRequisitionInfoApprovalRepository;
            this._jobRequisitionInfoApprovalDetailRepository = jobRequisitionInfoApprovalDetailRepository;
            this._clearanceFromMinistryRepository = clearanceFromMinistryRepository;
            this._clearanceFromMinistryDetailRepository = clearanceFromMinistryDetailRepository;
            this._applicantShortListRepository = applicantShortListRepository;
            this._applicantShortListDetailRepository = applicantShortListDetailRepository;
            this._applicantShortListApprovalRepository = applicantShortListApprovalRepository;
            this._applicantShortListApprovalDetailRepository = applicantShortListApprovalDetailRepository;
            this._mediaRepository = mediaRepository;
            this._jobAdvertisementInfoDetailMediaRepository = jobAdvertisementInfoDetailMediaRepository;
            this._jobAdvertisementInfoDetailRequisitionRepository = jobAdvertisementInfoDetailRequisitionRepository;
            this._jobAdvertisementInfoDetailAttachmentRepository = jobAdvertisementInfoDetailAttachmentRepository;
            this._selectionCritariaOrExamTypeRepository = selectionCritariaOrExamTypeRepository;
            this._selectionCriteriaRepository = selectionCriteriaRepository;
            this._selectionCriteriaDetailRepository = selectionCriteriaDetailRepository;
            this._selectionBoardInfoRepository = selectionBoardInfoRepository;
            this._selectionBoardInfoCommitteeRepository = selectionBoardInfoCommitteeRepository;
            this._applicantInterviewCardIssueRepository = applicantInterviewCardIssueRepository;
            this._applicantInterviewCardIssueDetailRepository = applicantInterviewCardIssueDetailRepository;
            this._regionRepository = regionRepository;
            this._testResultforApplicantInfoRepository = testResultforApplicantInfoRepository;
            this._testResultforApplicantInfoDetailRepository = testResultforApplicantInfoDetailRepository;
            this._selectedApplicantInfoApprovalRepository = selectedApplicantInfoApprovalRepository;
            this._selectedApplicantInfoApprovalDetailRepository = selectedApplicantInfoApprovalDetailRepository;
            this._selectedApplicantInfoRepository = selectedApplicantInfoRepository;
            this._selectedApplicantInfoDetailRepository = selectedApplicantInfoDetailRepository;
            this._appointmentLetterInfoRepository = appointmentLetterInfoRepository;
            this._orderTypeInfoRepository = orderTypeInfoRepository;
            this._disciplinaryActionTypeRepository = disciplinaryActionTypeRepository;
            this._acrCriteriaInformationRepository = acrCriteriaInformationRepository;
            this._punishmentTypeInfoRepository = punishmentTypeInfoRepository;
            this._punishmentTypeInfoDetailRepository = punishmentTypeInfoDetailRepository;
            this._acrAttributesInfoRepository = acrAttributesInfoRepository;
            this._acrAttributesInfoDetailRepository = acrAttributesInfoDetailRepository;
            this._acrRankInformationRepository = acrRankInformationRepository;
            this._complaintNoteSheetRepository = complaintNoteSheetRepository;
            this._firInfoRepository = firInfoRepository;
            this._chargeSheetInfoRepository = chargeSheetInfoRepository;
            this._acrOfficerInfoRepository = acrOfficerInfoRepository;
            this._acrOfficerHealthTestReportRepository = acrOfficerHealthTestReportRepository;
            this._noteOrderInfoReportRepository = noteOrderInfoReportRepository;
            this._noticeInfoReportRepository = noticeInfoReportRepository;
            this._acrOfficerBioDataRepository = acrOfficerBioDataRepository;
            this._acrGraphAndRecommendationRepository = acrGraphAndRecommendationRepository;
            this._acrReviewingOfficerCommentsRepository = acrReviewingOfficerCommentsRepository;
            this._acrInformationForAuthorityRepository = acrInformationForAuthorityRepository;
            this._noticeTypeRepository = noticeTypeRepository;
            this._explanationReceivedInfoRepository = explanationReceivedInfoRepository;
            this._hearingFixationInfoRepository = hearingFixationInfoRepository;
            this._hearingFixationInfoDetailRepository = hearingFixationInfoDetailRepository;
            this._acrPersonalCharacteristicsRepository = acrPersonalCharacteristicsRepository;
            this._acrPersonalCharacteristicsDetailRepository = acrPersonalCharacteristicsDetailRepository;
            this._acrPerformanceOfWorkRepository = acrPerformanceOfWorkRepository;
            this._acrPerformanceOfWorkDetailRepository = acrPerformanceOfWorkDetailRepository;
            this._acrStaffInfoRepository = acrStaffInfoRepository;
            this._hearingInfoRepository = hearingInfoRepository;
            this._acrStaffBioDataRepository = acrStaffBioDataRepository;
            this._investigationCommitteeInfoRepository = investigationCommitteeInfoRepository;
            this._investigationCommitteeInfoMemberInfoRepository = investigationCommitteeInfoMemberInfoRepository;
            this._investigationCommitteeInfoFormedForRepository = investigationCommitteeInfoFormedForRepository;
            this._acrAssessmentInfoRepository = acrAssessmentInfoRepository;
            this._acrAssessmentInfoDetailRepository = acrAssessmentInfoDetailRepository;
            this._officeEquipmentFurnitureInfoRepository = officeEquipmentFurnitureInfoRepository;
            this._investigationReportRepository = investigationReportRepository;
            this._appealReviewInfoRepository = appealReviewInfoRepository;
            this._officeEquipmentFurnitureUsagesInfoRepository = officeEquipmentFurnitureUsagesInfoRepository;
            this._officeEquipmentFurnitureUsagesInfoDetailRepository = officeEquipmentFurnitureUsagesInfoDetailRepository;
            this._notesAndDocumentInfoRepository = notesAndDocumentInfoRepository;
            this._notesAndDocumentInfoAttachmentDetailRepository = notesAndDocumentInfoAttachmentDetailRepository;
            this._notesAndDocumentInfoCommentsDetailRepository = notesAndDocumentInfoCommentsDetailRepository;
            this._suspensionOfEmployeeRepository = suspensionOfEmployeeRepository;
            this._suspensionOfEmployeeDetailRepository = suspensionOfEmployeeDetailRepository;
            this._acceptanceLetterInfoRepository = acceptanceLetterInfoRepository;
            this._designationHistoryRepository = designationHistoryRepository;
            this._designationHistoryDetailRepository = designationHistoryDetailRepository;
            this._punishmentRestrictionRepository = punishmentRestrictionRepository;
            this._statusDesignationInfoRepository = statusDesignationInfoRepository;
            this._foreignTourInfoRepository = foreignTourInfoRepository;
            this._zoneInfoRepository = zoneRepository;
            this._licenseCategory = licenseCategory;
            this._retirementAgeInfoRepository = retirementAgeInfoRepository;
            this._lifeInsuranceRepository = lifeInsuranceRepository;
            this._organogramCategoryTypeRepository = organogramCategoryTypeRepository;
            this._degreeTypeRepository = degreeTypeRepository;
            this._degreeLevelMappingRepository = degreeLevelMappingRepository;
            this._degreeLevelMappingDetailRepository = degreeLevelMappingDetailRepository;
            this._queryAnalyzerTableRepository = queryAnalyzerTableRepository;
            this._queryAnalyzerTableItemsRepository = queryAnalyzerTableItemsRepository;
            this._passPortTypeRepository = passPortTypeRepository;
            this._professionalCertificateRepository = professionalCertificateRepository;
            this._recruitmentEducationalQualificationRepository = recruitmentEducationalQualificationRepository;
            this._recruitmentJobExperienceRepository = recruitmentJobExperienceRepository;
            this._recruitmentSubjectOrGroupRepository = recruitmentSubjectOrGroupRepository;
            this._jobAdvertisementPostDetailRepository = jobAdvertisementPostDetailRepository;


            this._pgmConfiguration = pgmconfiguration;

            this._vwOrganogramZoneDetail = VWPRMOrganogramZoneDetail;

            /********* Notification ****************/

            this._notification = notification;
            this._notificationFlow = notificationFlow;
            this._notificationFlowSetup = notificationFlowSetup;
            this._notificationReadBy = notificationReadBy;

            this._vwNotificationFlowSetup = vwNotificationFlowSetup;


            /********* Approval ****************/

            this._ApprovalGroupRepository = ApprovalGroupRepository;
            this._ApprovalProcessRepository = ApprovalProcessRepository;
            this._ApprovalStepRepository = ApprovalStepRepository;
            this._ApprovalFlowMasterRepository = ApprovalFlowMasterRepository;
            this._ApprovalFlowDetailRepository = ApprovalFlowDetailRepository;
            this._ApproverTypeRepository = ApproverTypeRepository;
            this._ApproverAuthorTypeRepository = ApproverAuthorTypeRepository;
            this._ApproverInfoRepository = ApproverInfoRepository;
            this._EmployeeWiseApproverInfoRepository = EmployeeWiseApproverInfoRepository;
            this._AssignedApprovalFlowViewRepository = AssignedApprovalFlowViewRepository;
            this._ApproverInformationViewRepository = ApproverInformationViewRepository;
            this._ApprovalStatusRepository = ApprovalStatusRepository;
            this._GetApproverRepository = GetApproverRepository;
            this._RequestedApplicationInformationRepository = RequestedApplicationInformationRepository;
            this._ApprovalHisotryRepository = ApprovalHisotryRepository;
            this._MessageVariableRepository = MessageVariableRepository;
            this._ApprovalFlowConfigurationRepository = ApprovalFlowConfigurationRepository;
            /********* End of Approval ****************/


            this._eRectblgeneralinfoRepository = eRectblgeneralinfoRepository;
            this._eRectbleducationalinfoRepository = eRectbleducationalinfoRepository;
            this._eRectblmaritalinfoRepository = eRectblmaritalinfoRepository;

            this._eRectbladdressRepository = EREC_tbladdress;
            this._eRectblCountryRepository = eRectblCountryRepository;
            this._eRectblDistrictRepository = eRectblDistrictRepository;
            this._eRectblExaminationRepository = eRectblExaminationRepository;
            this._eRectblUpazilaRepository = eRectblUpazilaRepository;
            this._JobAdvertisementDestrict = JobAdvertisementDestrict;

            this._empSmsHistory = empSmsHistory;
            this._userLoginHistory = userLoginHistory;

            this._empServiceHistory = empServiceHistory;

            this._practice = practice;
        }

        #endregion

        #region Properties       

        public PRM_GenericRepository<PRM_HumanResourceRateMaster> HumanResourceRateMasterRepository
        {
            get
            {
                return _humanResourceRateMaster;
            }
        }

        public PRM_GenericRepository<PRM_HumanResourceRateDetail> HumanResourceRateDetailRepository
        {
            get
            {
                return _humanResourceRateDetail;
            }
        }

        public PRM_GenericRepository<PRM_HumanResourceRateAssignMaster> HumanResourceRateAssignMasterRepository
        {
            get
            {
                return _humanResourceRateAssignMaster;
            }
        }

        public PRM_GenericRepository<PRM_HumanResourceRateAssignDetail> HumanResourceRateAssignDetailRepository
        {
            get
            {
                return _humanResourceRateAssignDetail;
            }
        }

        public PRM_GenericRepository<CommonConfigType> ConfigTypeRepository
        {
            get
            {
                return _commonConfigType;
            }
        }

        public PRM_GenericRepository<PRM_EmpContactFiles> AttachmentContactFilesRepository
        {
            get
            {
                return _attachmentContactFiles;
            }
        }

        public PRM_GenericRepository<PRM_Designation> DesignationRepository
        {
            get
            {
                return _designationRepository;
            }
        }

        public PRM_GenericRepository<PRM_JobGrade> JobGradeRepository
        {
            get
            {
                return _jobGradeRepository;
            }
        }

        public PRM_GenericRepository<PRM_GradeStep> JobGradeStepRepository
        {
            get
            {
                return _jobGradeStepRepository;
            }
        }

        public PRM_GenericRepository<PRM_StaffCategory> PRM_StaffCategoryRepository
        {
            get
            {
                return _staffCategoryRepository;
            }
        }

        public PRM_GenericRepository<PRM_GradeStep> PRM_GradeStepRepository
        {
            get
            {
                return _gradeStepRepository;
            }
        }

        public PRM_GenericRepository<PRM_Division> DivisionRepository
        {
            get
            {
                return _divisionRepository;
            }
        }

        public PRM_GenericRepository<PRM_MaritalStatus> MaritalStatusRepository
        {
            get
            {
                return _maritalStatus;
            }
        }

        public PRM_GenericRepository<PRM_DegreeLevel> ExamDegreeLavelRepository
        {
            get
            {
                return _examDegreeLavel;
            }
        }

        public PRM_GenericRepository<PRM_AcademicGrade> AcademicGradeRepository
        {
            get
            {
                return _academicGrade;
            }
        }

        public PRM_GenericRepository<PRM_UniversityAndBoard> UniversityAndBoardRepository
        {
            get
            {
                return _universityAndBoard;
            }
        }

        public PRM_GenericRepository<PRM_CertificationInstitute> CertificationInstituteRepository
        {
            get
            {
                return _certificationInstitute;
            }
        }

        public PRM_GenericRepository<PRM_BloodGroup> BloodGroupRepository
        {
            get
            {
                return _bloodGroup;
            }
        }

        public PRM_GenericRepository<PRM_Country> CountryRepository
        {
            get
            {
                return _countryRepository;
            }
        }

        public PRM_GenericRepository<PRM_VisaType> VisaTypeRepository
        {
            get
            {
                return _visaTypeRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmpTrainingInfo> ProfessionalTrainingRepository
        {
            get
            {
                return _professionalTraining;
            }
        }
        public PRM_GenericRepository<PRM_EmpCertification> CertificationRepository
        {
            get
            {
                return _certification;
            }
        }
        public PRM_GenericRepository<PRM_EmpLicenseInfo> LicenseRepository
        {
            get
            {
                return _license;
            }
        }
        public PRM_GenericRepository<PRM_EmpJobSkill> JobSkillRepository
        {
            get
            {
                return _jobSkill;
            }
        }
        public PRM_GenericRepository<PRM_JobSkill> JobSkillNameRepository
        {
            get
            {
                return _jobSkillName;
            }
        }
        public PRM_GenericRepository<PRM_JobSkillLevel> JobSkillLevelRepository
        {
            get
            {
                return _jobSkillLevel;
            }
        }
        public PRM_GenericRepository<PRM_CertificationCategory> CertificationCategoryRepository
        {
            get
            {
                return _certificationCategory;
            }
        }


        public PRM_GenericRepository<PRM_LicenseType> LicenseTypeRepository
        {
            get
            {
                return _licenseType;
            }
        }

        public PRM_GenericRepository<PRM_District> DistrictRepository
        {
            get
            {
                return _districtRepository;
            }
        }

        public PRM_GenericRepository<PRM_Thana> ThanaRepository
        {
            get
            {
                return _thanaRepository;
            }
        }

        public PRM_GenericRepository<PRM_CountryDivision> CountryDivisionRepository
        {
            get
            {
                return _countryDivisionRepository;
            }
        }

        public PRM_GenericRepository<PRM_DivisionHeadMaping> DivisionHeadMapingRepository
        {
            get
            {
                return _divisionHeadaping;
            }
        }

        public PRM_GenericRepository<PRM_EmploymentInfo> EmploymentInfoRepository
        {
            get
            {
                return _employmentInfo;
            }
        }

        public PRM_GenericRepository<PRM_EmpContractInfo> EmploymentContractInfoRepository
        {
            get
            {
                return _employmentContractInfo;
            }
        }

        public PRM_GenericRepository<PRM_EmpSalary> EmpSalaryRepository
        {
            get
            {
                return _empSalary;
            }
        }

        public PRM_GenericRepository<PRM_EmpSalaryDetail> EmpSalaryDetailRepository
        {
            get
            {
                return _empSalaryDetails;
            }
        }

        public PRM_GenericRepository<PRM_EmpPersonalInfo> PersonalInfoRepository
        {
            get
            {
                return _personalInfo;
            }
        }

        public PRM_GenericRepository<PRM_EmpDegree> AccademicQualificationRepository
        {
            get
            {
                return _accademicQlfn;
            }
        }

        public PRM_GenericRepository<PRM_EmpExperience> JobExperienceInfoRepository
        {
            get
            {
                return _jobExperienceInfo;
            }
        }

        public PRM_GenericRepository<PRM_SubjectGroup> SubjectGroupRepository
        {
            get
            {
                return _subjectGroup;
            }
        }

        public PRM_ExecuteFunctions FunctionRepository
        {
            get
            {
                return _functionRepository;
            }
        }

        public PRM_GenericRepository<PRM_SalaryHeadGroup> PRM_SalaryHeadGroupRepository
        {
            get
            {
                return _salaryHeadGroupRepository;
            }
        }

        public PRM_GenericRepository<PRM_JobLocation> JobLocationRepository
        {
            get
            {
                return _jobLocationRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmploymentType> EmploymentTypeRepository
        {
            get
            {
                return _employmentTypeRepository;
            }
        }

        public PRM_GenericRepository<PRM_ResourceLevel> ResourceLevelRepository
        {
            get
            {
                return _resourceLevelRepository;
            }
        }

        public PRM_GenericRepository<PRM_SalaryHead> SalaryHeadRepository
        {
            get
            {
                return _salaryHeadRepository;
            }
        }

        public PRM_GenericRepository<PRM_NameTitle> NameTitleRepository
        {
            get { return _nameTitle; }
        }

        public PRM_GenericRepository<PRM_Nationality> NationalityRepository
        {
            get { return _nationality; }
        }

        public PRM_GenericRepository<PRM_Discipline> DisciplineRepository
        {
            get { return _empDiscipline; }
        }

        public PRM_GenericRepository<PRM_ShiftName> ShiftNameRepository
        {
            get { return _shiftName; }
        }

        public PRM_GenericRepository<PRM_BankName> BankNameRepository
        {
            get { return _bankName; }
        }

        public PRM_GenericRepository<PRM_OrganizationType> OrganizationTypeRepository
        {
            get { return _organizationType; }
        }

        public PRM_GenericRepository<PRM_EmploymentStatus> EmploymentStatusRepository
        {
            get { return _employmentStatus; }
        }

        public PRM_GenericRepository<PRM_ResourceCategory> ResourceCategoryRepository
        {
            get
            {
                return _resourceCategory;
            }
        }
        public PRM_GenericRepository<PRM_ResourceType> ResourceTypeRepository
        {
            get
            {
                return _ResourceType;
            }
        }
        public PRM_GenericRepository<PRM_ResourceInfo> ResourceInfoRepository
        {
            get
            {
                return _ResourceInfo;
            }
        }
        public PRM_GenericRepository<PRM_MeasurementUnit> MeasurementUnitRepository
        {
            get
            {
                return _MeasurementUnit;
            }
        }

        public PRM_GenericRepository<PRM_BankBranch> BankBranchRepository
        {
            get { return _bankBranch; }
        }

        public PRM_GenericRepository<vwAccademicQlfy> vwAccademicQlfyRepository
        {
            get
            {
                return _vwAccademicQlfy;
            }
        }
        
        public PRM_GenericRepository<PRM_EmpPhoto> EmployeePhotoGraphRepository
        {
            get
            {
                return _employeePhotographRepository;
            }
        }
        //public PRM_GenericRepository<PRM_EmpTransferInfo> EmployeeTransferInfoRepository
        //{
        //    get
        //    {
        //        return _EmployeeTransferInfo;
        //    }
        //}
        public PRM_GenericRepository<PRM_Language> Language
        {
            get
            {
                return _language;
            }
        }
        public PRM_GenericRepository<PRM_EmpLanguageEfficiency> EmployeeLanguageEfficiency
        {
            get
            {
                return _employeeLanguageEfficiency;
            }
        }
        public PRM_GenericRepository<PRM_ProefficiencyLevel> ProfficiencyLevel
        {
            get
            {
                return _efficencyLevel;
            }
        }


        public PRM_GenericRepository<PRM_EmpStatusChange> EmpStatusChangeRepository
        {
            get
            {
                return _EmpStatusChange;
            }
        }

        public PRM_GenericRepository<PRM_Relation> Relation
        {
            get
            {
                return _relation;
            }
        }
        public PRM_GenericRepository<PRM_Profession> ProfessionRepository
        {
            get
            {
                return _profession;
            }
        }
        public PRM_GenericRepository<PRM_EmpFamilyMemberInfo> PersonalFamilyInformation
        {
            get
            {
                return _familyMemberInformation;
            }
        }
        public PRM_GenericRepository<PRM_EmpContractPersonInfo> EmergencyContractPerson
        {
            get
            {
                return _emergencyContractPerson;
            }
        }
        public PRM_GenericRepository<PRM_EmpSeperation> EmpSeperationRepository
        {
            get
            {
                return _EmpSeperation;
            }
        }
        public PRM_GenericRepository<PRM_NomineeFor> NomineeFor
        {
            get
            {
                return _employeeNomineeFor;
            }
        }
        public PRM_GenericRepository<PRM_EmpNominee> Nominee
        {
            get
            {
                return _employeeNominee;
            }
        }
        public PRM_GenericRepository<PRM_EmpNomineeDetail> NomineeDetails
        {
            get
            {
                return _employeeNomineeDetails;
            }
        }

        public PRM_GenericRepository<PRM_EmpReferanceGuarantor> EmpReferenceGuarantorRepository
        {
            get
            {
                return _empReferenceGuarantor;
            }
        }

        public PRM_GenericRepository<PRM_EmpAttachmentFile> EmpAttachementRepository
        {
            get
            {
                return _empAttachment;
            }
        }

        public PRM_GenericRepository<PRM_AttachmentType> EmpAttachementTypeRepository
        {
            get
            {
                return _empAttachmentType;
            }
        }

        public PRM_GenericRepository<PRM_HumanResourceMapping> EmployeeMappingRepository
        {
            get
            {
                return _HumanResourceMapping;
            }
        }
        public PRM_GenericRepository<PRM_PublicationArea> PublicationArea
        {
            get
            {
                return _publicationArea;
            }
        }
        public PRM_GenericRepository<PRM_EmpPublicationInfo> EmployeePublication
        {
            get
            {
                return _employePublication;
            }
        }

        public PRM_GenericRepository<PRM_EmpVisaPassportInfo> EmployeeVisaInfoRepository
        {
            get
            {
                return _employeeVisaInformation;
            }
        }


        public PRM_GenericRepository<PRM_CompanyInfo> CompanyInformation
        {
            get
            {
                return _companyInformation;
            }
        }

        public PRM_GenericRepository<PRM_Religion> Religion
        {
            get { return _religion; }
        }

        public PRM_GenericRepository<LMS_tblLeaveType> LeaveType
        {
            get { return _leaveType; }
        }


        public PRM_GenericRepository<PRM_EmployeeActivationHistory> EmpActivationRepository
        {
            get
            {
                return _empActivationHistory;
            }
        }

        public PRM_GenericRepository<PRM_OrganogramLevel> OrganogramLevelRepository
        {
            get { return _organogramLevel; }
        }

        public PRM_GenericRepository<PRM_OrganogramType> OrganogramTypeRepository
        {
            get { return _organogramType; }
        }

        public PRM_GenericRepository<PRM_SalaryScale> SalaryScaleRepository
        {
            get { return _salaryScale; }
        }

        public PRM_GenericRepository<PRM_OrganizationalSetupManpowerInfo> OrganizationalSetupManpowerInfoRepository
        {
            get { return _organizationalSetupManpowerInfo; }
        }


        public PRM_GenericRepository<PRM_QuotaName> QuotaNameRepository
        {
            get { return _quotaName; }
        }
        public PRM_GenericRepository<PRM_EmployeeClass> EmployeeClassRepository
        {
            get { return _employeeClass; }
        }
        public PRM_GenericRepository<PRM_EmploymentProcess> EmploymentProcessRepository
        {
            get { return _employmentProcess; }
        }

        public PRM_GenericRepository<PRM_EmpLeverage> EmpLeverageRepository
        {
            get { return _empLeverage; }
        }

        public PRM_GenericRepository<PRM_EmpWealthStatementInfo> EmpWealthStatementRepository
        {
            get { return _empWealthStatement; }
        }

        public PRM_GenericRepository<PRM_AssetType> AssetTypeRepository
        {
            get { return _assetType; }
        }


        public PRM_GenericRepository<PRM_TrainingType> TrainingTypeRepository
        {
            get { return _trainingType; }
        }

        public PRM_GenericRepository<PRM_Location> LocationRepository
        {
            get { return _location; }
        }
        public PRM_GenericRepository<PRM_ClearanceForm> ClearanceFormRepository
        {
            get
            {
                return _clearanceFormRepository;
            }
        }


        public PRM_GenericRepository<PRM_ClearanceChecklist> ClearanceChecklistRepository
        {
            get
            {
                return _clearanceChecklistRepository;
            }
        }
        public PRM_GenericRepository<PRM_ClearanceChecklistDetail> ClearanceChecklistDetailRepository
        {
            get
            {
                return _clearanceChecklistDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_RecruitmentQualificationInfo> RecruitmentQualificationRepository
        {
            get
            {
                return _recruitmentQualificationInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_RecruitmentQualificationDetails> RecruitmentQualificationDetailRepository
        {
            get
            {
                return _recruitmentQualificationDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmpClearanceInfo> EmpClearanceInfoRepository
        {
            get
            {
                return _empClearanceInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmpClearanceInfoFormDetail> EmpClearanceInfoFormDetailRepository
        {
            get
            {
                return _empClearanceInfoFormDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmpClearanceInfoCheklistDetail> EmpClearanceInfoChecklistDetailRepository
        {
            get
            {
                return _empClearanceInfoChecklistDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_QuotaInfo> QuotaInfoRepository
        {
            get
            {
                return _quotaInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_DistrictQuota> DistrictQuotaRepository
        {
            get
            {
                return _districtQuotaRepository;
            }
        }
        public PRM_GenericRepository<PRM_Section> SectionRepository
        {
            get
            {
                return _sectionRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobRequisitionInfo> JobRequisitionInfoRepository
        {
            get
            {
                return _jobRequisitionInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobRequisitionInfoDetail> JobRequisitionInfoDetailRepository
        {
            get
            {
                return _jobRequisitionInfoDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_OrganogramTypeMapping> OrganogramTypeMappingRepository
        {
            get
            {
                return _organogramTypeMappingRepository;
            }
        }


        public PRM_GenericRepository<PRM_ApplicantInfo> ApplicantInfoRepository
        {
            get
            {
                return _applicantInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_ApplicantInfoQualification> ApplicantInfoQualificationRepository
        {
            get
            {
                return _applicantInfoQualificationRepository;
            }
        }



        public PRM_GenericRepository<PRM_JobAdvertisementInfo> JobAdvertisementInfoRepository
        {
            get
            {
                return _jobAdvertisementInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobRequisitionInfoSummary> JobRequisitionInfoSummaryRepository
        {
            get
            {
                return _jobRequisitionInfoSummaryRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobRequisitionInfoSummaryDetail> JobRequisitionInfoSummaryDetailRepository
        {
            get
            {
                return _jobRequisitionInfoSummaryDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_JobRequisitionInfoApproval> JobRequisitionInfoApprovalRepository
        {
            get
            {
                return _jobRequisitionInfoApprovalRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobRequisitionInfoApprovalDetail> JobRequisitionInfoApprovalDetailRepository
        {
            get
            {
                return _jobRequisitionInfoApprovalDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_ClearanceInfoFromMinistry> ClearanceInfoFromMinistryRepository
        {
            get
            {
                return _clearanceFromMinistryRepository;
            }
        }
        public PRM_GenericRepository<PRM_ClearanceInfoFromMinistryDetail> ClearanceInfoFromMinistryDetailRepository
        {
            get
            {
                return _clearanceFromMinistryDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_ApplicantShortList> ApplicantShortListRepository
        {
            get
            {
                return _applicantShortListRepository;
            }
        }
        public PRM_GenericRepository<PRM_ApplicantShortListDetail> ApplicantShortListDetailRepository
        {
            get
            {
                return _applicantShortListDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_ApplicantShortListApproval> ApplicantShortListApprovalRepository
        {
            get
            {
                return _applicantShortListApprovalRepository;
            }
        }
        public PRM_GenericRepository<PRM_ApplicantShortListApprovalDetail> ApplicantShortListApprovalDetailRepository
        {
            get
            {
                return _applicantShortListApprovalDetailRepository;
            }
        }


        public PRM_GenericRepository<PRM_Media> MediaRepository
        {
            get
            {
                return _mediaRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobAdvertisementInfoDetailMedia> JobAdvertisementInfoDetailMediaRepository
        {
            get
            {
                return _jobAdvertisementInfoDetailMediaRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobAdvertisementInfoDetailRequisition> JobAdvertisementInfoDetailRequisitionRepository
        {
            get
            {
                return _jobAdvertisementInfoDetailRequisitionRepository;
            }
        }

        public PRM_GenericRepository<PRM_JobAdvertisementInfoDetailAttachment> JobAdvertisementInfoDetailAttachmentRepository
        {
            get
            {
                return _jobAdvertisementInfoDetailAttachmentRepository;
            }
        }
        public PRM_GenericRepository<PRM_SelectionCritariaOrExamType> SelectionCritariaOrExamTypeRepository
        {
            get
            {
                return _selectionCritariaOrExamTypeRepository;
            }
        }
        public PRM_GenericRepository<PRM_SelectionCriteria> SelectionCriteriaRepository
        {
            get
            {
                return _selectionCriteriaRepository;
            }
        }
        public PRM_GenericRepository<PRM_SelectionCriteriaDetail> SelectionCriteriaDetailRepository
        {
            get
            {
                return _selectionCriteriaDetailRepository;
            }
        }


        public PRM_GenericRepository<PRM_SelectionBoardInfo> SelectionBoardInfoRepository
        {
            get
            {
                return _selectionBoardInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_SelectionBoardInfoCommittee> SelectionBoardInfoCommitteeRepository
        {
            get
            {
                return _selectionBoardInfoCommitteeRepository;
            }
        }
        public PRM_GenericRepository<PRM_ApplicantInterviewCardIssue> ApplicantInterviewCardIssueRepository
        {
            get
            {
                return _applicantInterviewCardIssueRepository;
            }
        }
        public PRM_GenericRepository<PRM_ApplicantInterviewCardIssueDetail> ApplicantInterviewCardIssueDetailRepository
        {
            get
            {
                return _applicantInterviewCardIssueDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_Region> RegionRepository
        {
            get
            {
                return _regionRepository;
            }
        }

        public PRM_GenericRepository<PRM_TestResultforApplicantInfo> TestResultforApplicantInfoRepository
        {
            get
            {
                return _testResultforApplicantInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_TestResultforApplicantInfoDetail> TestResultforApplicantInfoDetailRepository
        {
            get
            {
                return _testResultforApplicantInfoDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_SelectedApplicantInfoApproval> SelectedApplicantInfoApprovalRepository
        {
            get
            {
                return _selectedApplicantInfoApprovalRepository;
            }
        }

        public PRM_GenericRepository<PRM_SelectedApplicantInfoApprovalDetail> SelectedApplicantInfoApprovalDetailRepository
        {
            get
            {
                return _selectedApplicantInfoApprovalDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_SelectedApplicantInfo> SelectedApplicantInfoRepository
        {
            get
            {
                return _selectedApplicantInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_SelectedApplicantInfoDetail> SelectedApplicantInfoDetailRepository
        {
            get
            {
                return _selectedApplicantInfoDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_AppointmentLetterInfo> AppointmentLetterInfoRepository
        {
            get
            {
                return _appointmentLetterInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_OrderTypeInfo> OrderTypeInfoRepository
        {
            get
            {
                return _orderTypeInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_DisciplinaryActionType> DisciplinaryActionTypeRepository
        {
            get
            {
                return _disciplinaryActionTypeRepository;
            }
        }
        public PRM_GenericRepository<PRM_ACRCriteriaInformation> ACRCriteriaInformationRepository
        {
            get
            {
                return _acrCriteriaInformationRepository;
            }
        }



        public PRM_GenericRepository<PRM_PunishmentTypeInfo> PunishmentTypeInfoRepository
        {
            get
            {
                return _punishmentTypeInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_PunishmentTypeInfoDetail> PunishmentTypeInfoDetailRepository
        {
            get
            {
                return _punishmentTypeInfoDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_ACRAttributesInformation> ACRAttributesInformationRepository
        {
            get
            {
                return _acrAttributesInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_ACRAttributesInformationDetail> ACRAttributesInformationDetailRepository
        {
            get
            {
                return _acrAttributesInfoDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_ACRRankInformation> ACRRankInformationRepository
        {
            get
            {
                return _acrRankInformationRepository;
            }
        }

        public PRM_GenericRepository<PRM_ComplaintNoteSheet> ComplaintNoteSheetRepository
        {
            get
            {
                return _complaintNoteSheetRepository;
            }
        }
        public PRM_GenericRepository<PRM_FIRInfo> FIRInfoRepository
        {
            get
            {
                return _firInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_ChargeSheetInfo> ChargeSheetInfoRepository
        {
            get
            {
                return _chargeSheetInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACROfficerInfo> EmpACROfficerInfoRepository
        {
            get
            {
                return _acrOfficerInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACROfficerHealthTestReport> OfficerHealthTestReportRepository
        {
            get
            {
                return _acrOfficerHealthTestReportRepository;
            }
        }
        public PRM_GenericRepository<PRM_NoteOrderInfo> NoteOrderInfoReportRepository
        {
            get
            {
                return _noteOrderInfoReportRepository;
            }
        }

        public PRM_GenericRepository<PRM_NoticeInfo> NoticeInfoReportRepository
        {
            get
            {
                return _noticeInfoReportRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACROfficerBioData> EmpACROfficerBioDataRepository
        {
            get
            {
                return _acrOfficerBioDataRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRGraphAndRecommendation> ACRGraphAndRecommendationRepository
        {
            get
            {
                return _acrGraphAndRecommendationRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRReviewingOfficerComments> ACRReviewingOfficerCommentsRepository
        {
            get
            {
                return _acrReviewingOfficerCommentsRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRInformationForAuthority> ACRInformationForAuthorityRepository
        {
            get
            {
                return _acrInformationForAuthorityRepository;
            }
        }

        public PRM_GenericRepository<PRM_NoticeType> NoticeTypeRepository
        {
            get
            {
                return _noticeTypeRepository;
            }
        }

        public PRM_GenericRepository<PRM_ExplanationReceivedInfo> ExplanationReceivedInfoRepository
        {
            get
            {
                return _explanationReceivedInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_HearingFixationInfo> HearingFixationInfoRepository
        {
            get
            {
                return _hearingFixationInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_HearingFixationInfoDetail> HearingFixationInfoDetailRepository
        {
            get
            {
                return _hearingFixationInfoDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRPersonalCharacteristics> ACRPersonalCharacteristicsRepository
        {
            get
            {
                return _acrPersonalCharacteristicsRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRPersonalCharacteristicsDetail> ACRPersonalCharacteristicsDetailRepository
        {
            get
            {
                return _acrPersonalCharacteristicsDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRPerformanceOfWork> ACRPerformanceOfWorkRepository
        {
            get
            {
                return _acrPerformanceOfWorkRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRPerformanceOfWorkDetail> ACRPerformanceOfWorkDetailRepository
        {
            get
            {
                return _acrPerformanceOfWorkDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRStaffInfo> ACRStaffInfoRepository
        {
            get
            {
                return _acrStaffInfoRepository;
            }
        }


        public PRM_GenericRepository<PRM_HearingInfo> HearingInfoRepository
        {
            get
            {
                return _hearingInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRStaffBioData> ACRStaffBioDataRepository
        {
            get
            {
                return _acrStaffBioDataRepository;
            }
        }
        public PRM_GenericRepository<PRM_InvestigationCommitteeInfo> InvestigationCommitteeInfoRepository
        {
            get
            {
                return _investigationCommitteeInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_InvestigationCommitteeInfoMemberInfo> InvestigationCommitteeInfoMemberInfoRepository
        {
            get
            {
                return _investigationCommitteeInfoMemberInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_InvestigationCommitteeInfoFormedFor> InvestigationCommitteeInfoFormedForRepository
        {
            get
            {
                return _investigationCommitteeInfoFormedForRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRAssessmentInfo> ACRAssessmentInfoRepository
        {
            get
            {
                return _acrAssessmentInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_EmpACRAssessmentInfoDetail> ACRAssessmentInfoDetailRepository
        {
            get
            {
                return _acrAssessmentInfoDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_OfficeEquipmentFurnitureInfo> OfficeEquipmentFurnitureInfoRepository
        {
            get
            {
                return _officeEquipmentFurnitureInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_InvestigationReport> InvestigationReportRepository
        {
            get
            {
                return _investigationReportRepository;
            }
        }

        public PRM_GenericRepository<PRM_AppealReviewInfo> AppealReviewInfoRepository
        {
            get
            {
                return _appealReviewInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_OfficeEquipmentFurnitureUsagesInfo> OfficeEquipmentFurnitureUsagesInfoRepository
        {
            get
            {
                return _officeEquipmentFurnitureUsagesInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_OfficeEquipmentFurnitureUsagesInfoDetail> OfficeEquipmentFurnitureUsagesInfoDetailRepository
        {
            get
            {
                return _officeEquipmentFurnitureUsagesInfoDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_NotesAndDocumentInfo> NotesAndDocumentInfoRepository
        {
            get
            {
                return _notesAndDocumentInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_NotesAndDocumentInfoAttachmentDetail> NotesAndDocumentInfoAttachmentDetailRepository
        {
            get
            {
                return _notesAndDocumentInfoAttachmentDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_NotesAndDocumentInfoCommentsDetail> NotesAndDocumentInfoCommentsDetailRepository
        {
            get
            {
                return _notesAndDocumentInfoCommentsDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_SuspensionOfEmployee> SuspensionOfEmployeeRepository
        {
            get
            {
                return _suspensionOfEmployeeRepository;
            }
        }
        public PRM_GenericRepository<PRM_SuspensionOfEmployeeDetail> SuspensionOfEmployeeDetailRepository
        {
            get
            {
                return _suspensionOfEmployeeDetailRepository;
            }
        }
        public PRM_GenericRepository<PRM_AcceptanceLetterInfo> AcceptanceLetterInfoRepository
        {
            get
            {
                return _acceptanceLetterInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_DesignationHistory> DesignationHistoryRepository
        {
            get
            {
                return _designationHistoryRepository;
            }
        }

        public PRM_GenericRepository<PRM_DesignationHistoryDetail> DesignationHistoryDetailRepository
        {
            get
            {
                return _designationHistoryDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_PunishmentRestriction> PunishmentRestrictionRepository
        {
            get
            {
                return _punishmentRestrictionRepository;
            }
        }

        public PRM_GenericRepository<PRM_StatusDesignationInfo> StatusDesignationInfoRepository
        {
            get
            {
                return _statusDesignationInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmpForeignTourInfo> ForeignTourInfoRepository
        {
            get
            {
                return _foreignTourInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_ZoneInfo> ZoneInfoRepository
        {
            get
            {
                return _zoneInfoRepository;
            }
        }
        public PRM_GenericRepository<PRM_LicenseCategory> LicenseCategoryRepository
        {
            get
            {
                return _licenseCategory;
            }
        }

        public PRM_GenericRepository<PRM_RetirementAgeInfo> RetirementAgeInfoRepository
        {
            get
            {
                return _retirementAgeInfoRepository;
            }
        }

        public PRM_GenericRepository<PRM_EmpLifeInsurance> LifeInsuranceRepository
        {
            get
            {
                return _lifeInsuranceRepository;
            }
        }

        public PRM_GenericRepository<PRM_OrganogramCategoryType> OrganogramCategoryTypeRepository
        {
            get
            {
                return _organogramCategoryTypeRepository;
            }
        }

        public PRM_GenericRepository<PRM_DegreeType> DegreeTypeRepository
        {
            get
            {
                return _degreeTypeRepository;
            }
        }
        public PRM_GenericRepository<PRM_DegreeLevelMapping> DegreeLevelMappingRepository
        {
            get
            {
                return _degreeLevelMappingRepository;
            }
        }
        public PRM_GenericRepository<PRM_DegreeLevelMappingDetail> DegreeLevelMappingDetailRepository
        {
            get
            {
                return _degreeLevelMappingDetailRepository;
            }
        }

        public PRM_GenericRepository<PRM_QueryAnalyzerTable> QueryAnalyzerTableRepository
        {
            get
            {
                return _queryAnalyzerTableRepository;
            }
        }
        public PRM_GenericRepository<PRM_QueryAnalyzerTableItems> QueryAnalyzerTableItemsRepository
        {
            get
            {
                return _queryAnalyzerTableItemsRepository;
            }
        }

        public PRM_GenericRepository<PRM_PassportType> PassportTypeRepository
        {
            get
            {
                return _passPortTypeRepository;
            }
        }
        public PRM_GenericRepository<PRM_ProfessionalCertificate> ProfessionalCertificateRepository
        {
            get
            {
                return _professionalCertificateRepository;
            }
        }
        public PRM_GenericRepository<PRM_RecruitmentEducationalQualification> RecruitmentEducationalQualificationRepository
        {
            get
            {
                return _recruitmentEducationalQualificationRepository;
            }
        }
        public PRM_GenericRepository<PRM_RecruitmentJobExperience> RecruitmentJobExperienceRepository
        {
            get
            {
                return _recruitmentJobExperienceRepository;
            }
        }
        public PRM_GenericRepository<PRM_RecruitmentSubjectOrGroup> RecruitmentSubjectOrGroupRepository
        {
            get
            {
                return _recruitmentSubjectOrGroupRepository;
            }
        }
        public PRM_GenericRepository<PRM_JobAdvertisementPostDetail> JobAdvertisementPostDetailRepository
        {
            get
            {
                return _jobAdvertisementPostDetailRepository;
            }
        }




        public PRM_GenericRepository<APV_ApprovalGroup> ApprovalGroupRepository
        {
            get
            {
                return _ApprovalGroupRepository;
            }
        }

        public PRM_GenericRepository<APV_ApprovalProcess> ApprovalProcessRepository
        {
            get
            {
                return _ApprovalProcessRepository;
            }
        }

        public PRM_GenericRepository<APV_ApprovalStep> ApprovalStepRepository
        {
            get
            {
                return _ApprovalStepRepository;
            }
        }

        public PRM_GenericRepository<APV_ApprovalFlowMaster> ApprovalFlowMasterRepository
        {
            get
            {
                return _ApprovalFlowMasterRepository;
            }
        }

        public PRM_GenericRepository<APV_ApprovalFlowDetail> ApprovalFlowDetailRepository
        {
            get
            {
                return _ApprovalFlowDetailRepository;
            }
        }

        public PRM_GenericRepository<APV_ApproverType> ApproverTypeRepository
        {
            get
            {
                return _ApproverTypeRepository;
            }
        }

        public PRM_GenericRepository<APV_ApproverAuthorType> ApproverAuthorTypeRepository
        {
            get
            {
                return _ApproverAuthorTypeRepository;
            }
        }

        public PRM_GenericRepository<APV_ApproverInfo> ApproverInfoRepository
        {
            get
            {
                return _ApproverInfoRepository;
            }
        }

        public PRM_GenericRepository<APV_EmployeeWiseApproverInfo> EmployeeWiseApproverInfoRepository
        {
            get
            {
                return _EmployeeWiseApproverInfoRepository;
            }
        }

        public PRM_GenericRepository<vwAPVAssignedApprovalFlow> AssignedApprovalFlowViewRepository
        {
            get
            {
                return _AssignedApprovalFlowViewRepository;
            }
        }

        public PRM_GenericRepository<vwAPVApproverInformation> ApproverInformationViewRepository
        {
            get
            {
                return _ApproverInformationViewRepository;
            }
        }

        public PRM_GenericRepository<APV_ApprovalStatus> ApprovalStatusRepository
        {
            get
            {
                return _ApprovalStatusRepository;
            }
        }

        public PRM_GenericRepository<APV_GetApproverInfoByApplicant_Result> GetApproverRepository
        {
            get
            {
                return _GetApproverRepository;
            }
        }


        public PRM_GenericRepository<APV_ApplicationInformation> RequestedApplicationInformationRepository
        {
            get
            {
                return _RequestedApplicationInformationRepository;
            }
        }

        public PRM_GenericRepository<APV_MessageVariableInformation> MessageVariableRepository
        {
            get
            {
                return _MessageVariableRepository;
            }
        }

        public PRM_GenericRepository<APV_ApprovalFlowConfiguration> ApprovalFlowConfigurationRepository
        {
            get
            {
                return _ApprovalFlowConfigurationRepository;
            }
        }

        public PRM_GenericRepository<EREC_tblgeneralinfo> ERECgeneralinfoRepository
        {
            get
            {
                return _eRectblgeneralinfoRepository;
            }
        }
        public PRM_GenericRepository<EREC_tbleducationalinfo> ERECeducationalinfoRepository
        {
            get
            {
                return _eRectbleducationalinfoRepository;
            }
        }
        public PRM_GenericRepository<EREC_tblmaritalinfo> ERECmaritalinfoRepository
        {
            get
            {
                return _eRectblmaritalinfoRepository;
            }
        }

        public PRM_GenericRepository<EREC_tbladdress> ERECtblAddressRepository
        {
            get
            {
                return this._eRectbladdressRepository;
            }
        }

        public PRM_GenericRepository<EREC_tblCountry> ERECtblCountryRepository
        {
            get
            {
                return this._eRectblCountryRepository;
            }
        }
        public PRM_GenericRepository<EREC_tblDistrict> ERECtblDistrictRepository
        {
            get
            {
                return this._eRectblDistrictRepository;
            }
        }

        public PRM_GenericRepository<EREC_tblExamination> ERECtblExaminationRepository
        {
            get
            {
                return this._eRectblExaminationRepository;
            }
        }
        public PRM_GenericRepository<EREC_tblUpazila> ERECtblUpazilaRepository
        {
            get
            {
                return this._eRectblUpazilaRepository;
            }
        }

        public PRM_GenericRepository<PRM_JobAdvertisementInfoDistrict> JobAdvertisementDestrictRepository
        {
            get
            {
                return _JobAdvertisementDestrict;
            }
        }

        public PRM_GenericRepository<vwApvApplicationWiseApprovalStatu> ApprovalHistoryRepository
        {
            get
            {
                return _ApprovalHisotryRepository;
            }
        }

        public PRM_GenericRepository<PGM_Configuration> PgmConfigurationRepository { get { return _pgmConfiguration; } }

        public PRM_GenericRepository<vwPRMOrganogramZoneDetail> VwOrganogramZoneDetailRepository { get { return _vwOrganogramZoneDetail; } }

        public PRM_GenericRepository<NTF_Notification> NotificationRepository { get { return _notification; } }
        public PRM_GenericRepository<NTF_NotificationFlow> NotificationFlowRepository { get { return _notificationFlow; } }
        public PRM_GenericRepository<NTF_NotificationFlowSetup> NotificationFlowSetupRepository { get { return _notificationFlowSetup; } }
        public PRM_GenericRepository<NTF_NotificationReadBy> NotificationReadByRepository { get { return _notificationReadBy; } }

        public PRM_GenericRepository<vwNotificationFlowSetup> VwNotificationFlowSetupRepository { get { return _vwNotificationFlowSetup; } }

        public PRM_GenericRepository<PRM_EmpSmsHistory> EmpSmsHistoryRepository
        {
            get
            {
                return _empSmsHistory;
            }
        }

        public PRM_GenericRepository<PRM_UserLoginHistory> UserLoginHistoryRepository
        {
            get
            {
                return _userLoginHistory;
            }
        }

        public PRM_GenericRepository<PRM_EmpServiceHistory> EmployeeServiceHistoryRepository
        {
            get
            {
                return _empServiceHistory;
            }
        }

        public PRM_GenericRepository<PRM_Practice> PracticeRepository
        {
            get
            {
                return _practice;
            }
        }

        #endregion
    }
}