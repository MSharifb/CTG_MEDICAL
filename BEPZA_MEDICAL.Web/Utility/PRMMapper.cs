using AutoMapper;
using BEPZA_MEDICAL.DAL.PRM;

using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalGroup;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRate;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRateAssign;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.RequestedApplication;


namespace BEPZA_MEDICAL.Web.Utility
{
    public class PRMMapper
    {
        public PRMMapper()
        {
            //PRM_HumanResourceRateAssignMaster
            Mapper.CreateMap<HumanResourceRateAssignModel, PRM_HumanResourceRateAssignMaster>();
            Mapper.CreateMap<PRM_HumanResourceRateAssignMaster, HumanResourceRateAssignModel>();

            //PRM_HumanResourceRateAssignDetail
            Mapper.CreateMap<HumanResourceRateAssignDetailModel, PRM_HumanResourceRateAssignDetail>();
            Mapper.CreateMap<PRM_HumanResourceRateAssignDetail, HumanResourceRateAssignDetailModel>();

            //Attachment Files
            Mapper.CreateMap<EmployeeContractAttachmentFiles, PRM_EmpContactFiles>();
            Mapper.CreateMap<PRM_EmpContactFiles, EmployeeContractAttachmentFiles>();

            //Grade step
            Mapper.CreateMap<GradeStepViewModel, PRM_GradeStep>();
            Mapper.CreateMap<PRM_GradeStep, GradeStepViewModel>();

            //job grade
            Mapper.CreateMap<PRM_JobGrade, JobGradeViewModel>();
            Mapper.CreateMap<JobGradeViewModel, PRM_JobGrade>();

            //Human Resource Rate
            Mapper.CreateMap<PRM_HumanResourceRateMaster, HumanResourceRateViewModel>();
            Mapper.CreateMap<HumanResourceRateViewModel, PRM_HumanResourceRateMaster>();

            //Human Resource Rate Detail
            Mapper.CreateMap<PRM_HumanResourceRateDetail, HumanResourceRateDetailViewModel>();
            Mapper.CreateMap<HumanResourceRateDetailViewModel, PRM_HumanResourceRateDetail>();

            //CountryDivision
            Mapper.CreateMap<CountryDivisionViewModel, PRM_CountryDivision>();
            Mapper.CreateMap<PRM_CountryDivision, CountryDivisionViewModel>();
            //District
            Mapper.CreateMap<DistrictViewModel, PRM_District>();
            Mapper.CreateMap<PRM_District, DistrictViewModel>();
            //Thana
            Mapper.CreateMap<ThanaViewModel, PRM_Thana>();
            Mapper.CreateMap<PRM_Thana, ThanaViewModel>();

            //ResourceCategory
            Mapper.CreateMap<ResourceCategoryViewModel, PRM_ResourceCategory>();
            Mapper.CreateMap<PRM_ResourceCategory, ResourceCategoryViewModel>();

            //ResourceInfo
            Mapper.CreateMap<ResourceInfoViewModel, PRM_ResourceInfo>();
            Mapper.CreateMap<PRM_ResourceInfo, ResourceInfoViewModel>();

            //Common config
            Mapper.CreateMap<CommonConfigViewModel, CommonConfigGetResult>();
            Mapper.CreateMap<CommonConfigGetResult, CommonConfigViewModel>();

            //Common configType
            Mapper.CreateMap<CommonConfigTypeViewModel, CommonConfigType>();
            Mapper.CreateMap<CommonConfigType, CommonConfigTypeViewModel>();

            //Employment Info
            Mapper.CreateMap<EmploymentInfoViewModel, PRM_EmploymentInfo>();
            Mapper.CreateMap<PRM_EmploymentInfo, EmploymentInfoViewModel>();

            //Personal Info
            Mapper.CreateMap<PersonalInfoViewModel, PRM_EmpPersonalInfo>();
            Mapper.CreateMap<PRM_EmpPersonalInfo, PersonalInfoViewModel>();

            //Accademic Info
            Mapper.CreateMap<PRM_EmpDegree, AccademicQlfnInfoViewModel>();
            Mapper.CreateMap<AccademicQlfnInfoViewModel, PRM_EmpDegree>();

            //Job Experience Info
            Mapper.CreateMap<PRM_EmpExperience, JobExperienceInfoViewModel>();
            Mapper.CreateMap<JobExperienceInfoViewModel, PRM_EmpExperience>();

            //Professional Training Info
            Mapper.CreateMap<PRM_EmpTrainingInfo, ProfessionalTrainingInfoViewModel>();
            Mapper.CreateMap<ProfessionalTrainingInfoViewModel, PRM_EmpTrainingInfo>();

            //Professional Certification Info
            Mapper.CreateMap<PRM_EmpCertification, ProfessionalCertificationInfoViewModel>();
            Mapper.CreateMap<ProfessionalCertificationInfoViewModel, PRM_EmpCertification>();

            //Professional License Info
            Mapper.CreateMap<PRM_EmpLicenseInfo, ProfessionalLicenseInfoViewModel>();
            Mapper.CreateMap<ProfessionalLicenseInfoViewModel, PRM_EmpLicenseInfo>();

            //Job Skill Info
            Mapper.CreateMap<PRM_EmpJobSkill, JobSkillInfoViewModel>();
            Mapper.CreateMap<JobSkillInfoViewModel, PRM_EmpJobSkill>();

            //Designation
            Mapper.CreateMap<PRM_Designation, DesignationViewModel>();
            Mapper.CreateMap<DesignationViewModel, PRM_Designation>();

            //Division Head Mapping
            Mapper.CreateMap<PRM_DivisionHeadMaping, DivisionHeadMapingViewModel>();
            Mapper.CreateMap<DivisionHeadMapingViewModel, PRM_DivisionHeadMaping>();

            //Employment Contract Info
            Mapper.CreateMap<PRM_EmpContractInfo, EmploymentContractPeriodViewModel>();
            Mapper.CreateMap<EmploymentContractPeriodViewModel, PRM_EmpContractInfo>();

            //EmployeePhotograph
            Mapper.CreateMap<PRM_EmpPhoto, EmployeePhotoGraphViewModel>();
            Mapper.CreateMap<EmployeePhotoGraphViewModel, PRM_EmpPhoto>();

            //Mapper.CreateMap<PRM_EmpTransferInfo, EmployeeTransferInfoViewModel>();
            //Mapper.CreateMap<EmployeeTransferInfoViewModel, PRM_EmpTransferInfo>();

            Mapper.CreateMap<EmployeeSeperationViewModel, PRM_EmpSeperation>();
            Mapper.CreateMap<PRM_EmpSeperation, EmployeeSeperationViewModel>();

            Mapper.CreateMap<EmployeeMappingViewModel, PRM_HumanResourceMapping>();
            Mapper.CreateMap<PRM_HumanResourceMapping, EmployeeMappingViewModel>();

            //emp salary structure
            Mapper.CreateMap<SalaryStructureDetailsModel, PRM_EmpSalaryDetail>();
            Mapper.CreateMap<PRM_EmpSalaryDetail, SalaryStructureDetailsModel>();

            //Employee Confirmation Increment Promotion
            Mapper.CreateMap<EmployeeConfirmationIncrementPromotionViewModel, PRM_EmpStatusChange>();
            Mapper.CreateMap<PRM_EmpStatusChange, EmployeeConfirmationIncrementPromotionViewModel>();

            //Personal Family Member Information
            Mapper.CreateMap<PersonalFamilyMemberInformationViewModel, PRM_EmpFamilyMemberInfo>();
            Mapper.CreateMap<PRM_EmpFamilyMemberInfo, PersonalFamilyMemberInformationViewModel>();

            //Personal Emergency Contract Information
            Mapper.CreateMap<PersonalEmergencyContractViewModel, PRM_EmpContractPersonInfo>();
            Mapper.CreateMap<PRM_EmpContractPersonInfo, PersonalEmergencyContractViewModel>();

            //Reference/Guarantor Info
            Mapper.CreateMap<ReferenceInfoViewModel, PRM_EmpReferanceGuarantor>();
            Mapper.CreateMap<PRM_EmpReferanceGuarantor, ReferenceInfoViewModel>();

            //Personal Nominee
            Mapper.CreateMap<PersonalNomineeViewModel, PRM_EmpNominee>();
            Mapper.CreateMap<PRM_EmpNominee, PersonalNomineeViewModel>();

            //Personal Nominee Details
            Mapper.CreateMap<PersonalNomineeDetailsViewModel, PRM_EmpNomineeDetail>();
            Mapper.CreateMap<PRM_EmpNomineeDetail, PersonalNomineeDetailsViewModel>();

            //emp attachment
            Mapper.CreateMap<EmpAttachmentViewModel, PRM_EmpAttachmentFile>();
            Mapper.CreateMap<PRM_EmpAttachmentFile, EmpAttachmentViewModel>();

            //Personal Publication
            Mapper.CreateMap<PersonalPublicationViewModel, PRM_EmpPublicationInfo>();
            Mapper.CreateMap<PRM_EmpPublicationInfo, PersonalPublicationViewModel>();

            //Personal Visa Information
            Mapper.CreateMap<VisaInfoViewModel, PRM_EmpVisaPassportInfo>();
            Mapper.CreateMap<PRM_EmpVisaPassportInfo, VisaInfoViewModel>();

            //Company Information
            Mapper.CreateMap<CompanyInformationViewModel, PRM_CompanyInfo>();
            Mapper.CreateMap<PRM_CompanyInfo, CompanyInformationViewModel>();           

            //EmployeeActivation
            Mapper.CreateMap<EmployeeActivationViewModel, PRM_EmployeeActivationHistory>();
            Mapper.CreateMap<PRM_EmployeeActivationHistory, EmployeeActivationViewModel>();

            //OrganogramLevel
            Mapper.CreateMap<OrganogramLevelViewModel, PRM_OrganogramLevel>();
            Mapper.CreateMap<PRM_OrganogramLevel, OrganogramLevelViewModel>();

            //SalaryScale
            Mapper.CreateMap<SalaryScaleViewModel, PRM_SalaryScale>();
            Mapper.CreateMap<PRM_SalaryScale, SalaryScaleViewModel>();

            //BankName
            Mapper.CreateMap<BankNameViewModel, PRM_BankName>();
            Mapper.CreateMap<PRM_BankName, BankNameViewModel>();

            //BankBranch
            Mapper.CreateMap<BankBranchViewModel, PRM_BankBranch>();
            Mapper.CreateMap<PRM_BankBranch, BankBranchViewModel>();

            //OrganizationalSetupManpowerInfo
            Mapper.CreateMap<OrganizationalSetupManpowerInfoViewModel, PRM_OrganizationalSetupManpowerInfo>();
            Mapper.CreateMap<PRM_OrganizationalSetupManpowerInfo, OrganizationalSetupManpowerInfoViewModel>();
                      
            //EmpWealthStatement
            Mapper.CreateMap<EmployeeWealthStatementViewModel, PRM_EmpWealthStatementInfo>();
            Mapper.CreateMap<PRM_EmpWealthStatementInfo, EmployeeWealthStatementViewModel>();

            //EmpLeverage
            Mapper.CreateMap<EmpLeverageViewModel, PRM_EmpLeverage>();
            Mapper.CreateMap<PRM_EmpLeverage, EmpLeverageViewModel>();

            //Clearance Checklist Information
            Mapper.CreateMap<ClearanceChecklistViewModel, PRM_ClearanceChecklist>();
            Mapper.CreateMap<PRM_ClearanceChecklist, ClearanceChecklistViewModel>();

            Mapper.CreateMap<ClearanceChecklistDetailsViewModel, PRM_ClearanceChecklistDetail>();
            Mapper.CreateMap<PRM_ClearanceChecklistDetail, ClearanceChecklistDetailsViewModel>();

            //Recruitment Qualification Information
            Mapper.CreateMap<RecruitmentQualificationInfoViewModel, PRM_RecruitmentQualificationInfo>();
            Mapper.CreateMap<PRM_RecruitmentQualificationInfo, RecruitmentQualificationInfoViewModel>();

            Mapper.CreateMap<RecruitmentQualificationDetailsViewModel, PRM_RecruitmentQualificationDetails>();
            Mapper.CreateMap<PRM_RecruitmentQualificationDetails, RecruitmentQualificationDetailsViewModel>();

            //Employee Clearance Information
            Mapper.CreateMap<PRM_EmpClearanceInfo, EmpClearanceInfoViewModel>();
            Mapper.CreateMap<EmpClearanceInfoViewModel, PRM_EmpClearanceInfo>();

            Mapper.CreateMap<EmpClearanceInfoFormDetailsViewModel, PRM_EmpClearanceInfoFormDetail>();
            Mapper.CreateMap<PRM_EmpClearanceInfoFormDetail, EmpClearanceInfoFormDetailsViewModel>();

            Mapper.CreateMap<EmpClearanceInfoChecklistDetailsViewModel, PRM_EmpClearanceInfoCheklistDetail>();
            Mapper.CreateMap<PRM_EmpClearanceInfoCheklistDetail, EmpClearanceInfoChecklistDetailsViewModel>();

            // Quota Info
            Mapper.CreateMap<QuotaInfoViewModel, PRM_QuotaInfo>();
            Mapper.CreateMap<PRM_QuotaInfo, QuotaInfoViewModel>();

            //District Quota 
            Mapper.CreateMap<DistrictQuotaViewModel, PRM_DistrictQuota>();
            Mapper.CreateMap<PRM_DistrictQuota, DistrictQuotaViewModel>();

            // Job Requisition Information
            Mapper.CreateMap<JobRequisitionInfoViewModel, PRM_JobRequisitionInfo>();
            Mapper.CreateMap<PRM_JobRequisitionInfo, JobRequisitionInfoViewModel>();

            Mapper.CreateMap<JobRequisitionInfoDetailsViewModel, PRM_JobRequisitionInfoDetail>();
            Mapper.CreateMap<PRM_JobRequisitionInfoDetail, JobRequisitionInfoDetailsViewModel>();

            // Applicant Information
            Mapper.CreateMap<ApplicantInfoViewModel, PRM_ApplicantInfo>();
            Mapper.CreateMap<PRM_ApplicantInfo, ApplicantInfoViewModel>();

            Mapper.CreateMap<ApplicantInfoQualificationViewModel, PRM_ApplicantInfoQualification>();
            Mapper.CreateMap<PRM_ApplicantInfoQualification, ApplicantInfoQualificationViewModel>();

            // Job Requisition Information Summary
            Mapper.CreateMap<JobRequisitionInfoSummaryViewModel, PRM_JobRequisitionInfoSummary>();
            Mapper.CreateMap<PRM_JobRequisitionInfoSummary, JobRequisitionInfoSummaryViewModel>();

            Mapper.CreateMap<RequisitionInfoSummaryDetail, PRM_JobRequisitionInfoSummaryDetail>();
            Mapper.CreateMap<PRM_JobRequisitionInfoSummaryDetail, RequisitionInfoSummaryDetail>();

            
            // Job Requisition Information Approval
            Mapper.CreateMap<JobRequisitionInfoApprovalViewModel, PRM_JobRequisitionInfoApproval>();
            Mapper.CreateMap<PRM_JobRequisitionInfoApproval, JobRequisitionInfoApprovalViewModel>();

            Mapper.CreateMap<JobRequisitionInfoApprovalDetailViewModel, PRM_JobRequisitionInfoApprovalDetail>();
            Mapper.CreateMap<PRM_JobRequisitionInfoApprovalDetail, JobRequisitionInfoApprovalDetailViewModel>();

            // Clearance From Ministry 
            Mapper.CreateMap<ClearanceInfoFromMinistryViewModel, PRM_ClearanceInfoFromMinistry>();
            Mapper.CreateMap<PRM_ClearanceInfoFromMinistry, ClearanceInfoFromMinistryViewModel>();

            Mapper.CreateMap<ClearanceInfoFromMinistryDetailViewModel, PRM_ClearanceInfoFromMinistryDetail>();
            Mapper.CreateMap<PRM_ClearanceInfoFromMinistryDetail, ClearanceInfoFromMinistryDetailViewModel>();
            
            // Applicant ShortList 
            Mapper.CreateMap<ApplicantShortListViewModel, PRM_ApplicantShortList>();
            Mapper.CreateMap<PRM_ApplicantShortList, ApplicantShortListViewModel>();

            Mapper.CreateMap<ApplicantShortListDetailViewModel, PRM_ApplicantShortListDetail>();
            Mapper.CreateMap<PRM_ApplicantShortListDetail, ApplicantShortListDetailViewModel>();


            // Applicant ShortList  Approval
            Mapper.CreateMap<ApplicantShortListApprovalViewModel, PRM_ApplicantShortListApproval>();
            Mapper.CreateMap<PRM_ApplicantShortListApproval, ApplicantShortListApprovalViewModel>();

            Mapper.CreateMap<ApplicantShortListApprovalDetailViewModel, PRM_ApplicantShortListApprovalDetail>();
            Mapper.CreateMap<PRM_ApplicantShortListApprovalDetail, ApplicantShortListApprovalDetailViewModel>();

            //Job Advertisement Information
            Mapper.CreateMap<JobAdvertisementInfoViewModel, PRM_JobAdvertisementInfo>();
            Mapper.CreateMap<PRM_JobAdvertisementInfo, JobAdvertisementInfoViewModel>();

            Mapper.CreateMap<JobAdvertisementInfoMediaViewModel, PRM_JobAdvertisementInfoDetailMedia>();
            Mapper.CreateMap<PRM_JobAdvertisementInfoDetailMedia, JobAdvertisementInfoMediaViewModel>();

            Mapper.CreateMap<JobAdvertisementInfoRequisitionViewModel, PRM_JobAdvertisementInfoDetailRequisition>();
            Mapper.CreateMap<PRM_JobAdvertisementInfoDetailRequisition, JobAdvertisementInfoRequisitionViewModel>();

            Mapper.CreateMap<JobAdvertisementInfoAttachmentViewModel, PRM_JobAdvertisementInfoDetailAttachment>();
            Mapper.CreateMap<PRM_JobAdvertisementInfoDetailAttachment, JobAdvertisementInfoAttachmentViewModel>();

            //Selection Criteria
            Mapper.CreateMap<SelectionCriteriaViewModel, PRM_SelectionCriteria>();
            Mapper.CreateMap<PRM_SelectionCriteria, SelectionCriteriaViewModel>();

            Mapper.CreateMap<SelectionCriteriaDetailViewModel, PRM_SelectionCriteriaDetail>();
            Mapper.CreateMap<PRM_SelectionCriteriaDetail, SelectionCriteriaDetailViewModel>();

            //Selection Board Information
            Mapper.CreateMap<SelectionBoardInfoViewModel, PRM_SelectionBoardInfo>();
            Mapper.CreateMap<PRM_SelectionBoardInfo, SelectionBoardInfoViewModel>();

            Mapper.CreateMap<SelectionBoardInfoCommitteeViewModel, PRM_SelectionBoardInfoCommittee>();
            Mapper.CreateMap<PRM_SelectionBoardInfoCommittee, SelectionBoardInfoCommitteeViewModel>();

            //Applicant Interview Card Issue
            Mapper.CreateMap<ApplicantInterviewCardIssueViewModel, PRM_ApplicantInterviewCardIssue>();
            Mapper.CreateMap<PRM_ApplicantInterviewCardIssue, ApplicantInterviewCardIssueViewModel>();

            Mapper.CreateMap<ApplicantInterviewCardIssueDetailViewModel, PRM_ApplicantInterviewCardIssueDetail>();
            Mapper.CreateMap<PRM_ApplicantInterviewCardIssueDetail, ApplicantInterviewCardIssueDetailViewModel>();

            //Test Result for Application Information
            Mapper.CreateMap<TestResultforApplicantInfoViewModel, PRM_TestResultforApplicantInfo>();
            Mapper.CreateMap<PRM_TestResultforApplicantInfo, TestResultforApplicantInfoViewModel>();

            Mapper.CreateMap<TestResultforApplicantInfoDetailViewModel, PRM_TestResultforApplicantInfoDetail>();
            Mapper.CreateMap<PRM_TestResultforApplicantInfoDetail, TestResultforApplicantInfoDetailViewModel>();

            //Selected Application Information Approval
            Mapper.CreateMap<SelectedApplicantInfoApprovalViewModel, PRM_SelectedApplicantInfoApproval>();
            Mapper.CreateMap<PRM_SelectedApplicantInfoApproval, SelectedApplicantInfoApprovalViewModel>();

            Mapper.CreateMap<SelectedApplicantInfoApprovalDetailViewModel, PRM_SelectedApplicantInfoApprovalDetail>();
            Mapper.CreateMap<PRM_SelectedApplicantInfoApprovalDetail, SelectedApplicantInfoApprovalDetailViewModel>();

            //Selected Application Information
            Mapper.CreateMap<SelectedApplicantInfoViewModel, PRM_SelectedApplicantInfo>();
            Mapper.CreateMap<PRM_SelectedApplicantInfo, SelectedApplicantInfoViewModel>();

            Mapper.CreateMap<SelectedApplicantInfoDetailViewModel, PRM_SelectedApplicantInfoDetail>();
            Mapper.CreateMap<PRM_SelectedApplicantInfoDetail, SelectedApplicantInfoDetailViewModel>();

            //Manage Appointment Letter Informarion
            Mapper.CreateMap<AppointmentLetterInfoViewModel, PRM_AppointmentLetterInfo>();
            Mapper.CreateMap<PRM_AppointmentLetterInfo, AppointmentLetterInfoViewModel>();

            //Manage Appointment Letter Informarion
            Mapper.CreateMap<OrderTypeInfoViewModel, PRM_OrderTypeInfo>();
            Mapper.CreateMap<PRM_OrderTypeInfo, OrderTypeInfoViewModel>();

            //ACR Criteria Informarion
            Mapper.CreateMap<ACRCriteriaInformationViewModel, PRM_ACRCriteriaInformation>();
            Mapper.CreateMap<PRM_ACRCriteriaInformation, ACRCriteriaInformationViewModel>();

            //Punishment Type Information
            Mapper.CreateMap<PunishmentTypeInfoViewModel, PRM_PunishmentTypeInfo>();
            Mapper.CreateMap<PRM_PunishmentTypeInfo, PunishmentTypeInfoViewModel>();

            Mapper.CreateMap<PunishmentTypeInfoDetailViewModel, PRM_PunishmentTypeInfoDetail>();
            Mapper.CreateMap<PRM_PunishmentTypeInfoDetail, PunishmentTypeInfoDetailViewModel>();

            //ACR Attributes Information
            Mapper.CreateMap<ACRAttributesInformationViewModel, PRM_ACRAttributesInformation>();
            Mapper.CreateMap<PRM_ACRAttributesInformation, ACRAttributesInformationViewModel>();

            Mapper.CreateMap<ACRAttributesInformationDetailViewModel, PRM_ACRAttributesInformationDetail>();
            Mapper.CreateMap<PRM_ACRAttributesInformationDetail, ACRAttributesInformationDetailViewModel>();

            //ACR Rank Informarion
            Mapper.CreateMap<ACRRankInformationViewModel, PRM_ACRRankInformation>();
            Mapper.CreateMap<PRM_ACRRankInformation, ACRRankInformationViewModel>();

            //Complaint Note Sheet
            Mapper.CreateMap<ComplaintNoteSheetViewModel, PRM_ComplaintNoteSheet>();
            Mapper.CreateMap<PRM_ComplaintNoteSheet, ComplaintNoteSheetViewModel>();

            //FIR Information
            Mapper.CreateMap<FIRInfoViewModel, PRM_FIRInfo>();
            Mapper.CreateMap<PRM_FIRInfo, FIRInfoViewModel>();

            //Charge Sheet Information
            Mapper.CreateMap<ChargeSheetInfoViewModel, PRM_ChargeSheetInfo>();
            Mapper.CreateMap<PRM_ChargeSheetInfo, ChargeSheetInfoViewModel>();

            //Emp ACR Officer Information
            Mapper.CreateMap<OfficerInfoViewModel, PRM_EmpACROfficerInfo>();
            Mapper.CreateMap<PRM_EmpACROfficerInfo, OfficerInfoViewModel>();

            //Emp ACR Officer Health Test Report
            Mapper.CreateMap<OfficerHealthTestReportViewModel, PRM_EmpACROfficerHealthTestReport>();
            Mapper.CreateMap<PRM_EmpACROfficerHealthTestReport, OfficerHealthTestReportViewModel>();

            //Note and Order Information
            Mapper.CreateMap<NoteOrderInfoViewModel, PRM_NoteOrderInfo>();
            Mapper.CreateMap<PRM_NoteOrderInfo, NoteOrderInfoViewModel>();

            //Notice Information
            Mapper.CreateMap<NoticeInfoViewModel, PRM_NoticeInfo>();
            Mapper.CreateMap<PRM_NoticeInfo, NoticeInfoViewModel>();

            //Emp ACR Officer Bio data
            Mapper.CreateMap<OfficerBioDataViewModel, PRM_EmpACROfficerBioData>();
            Mapper.CreateMap<PRM_EmpACROfficerBioData, OfficerBioDataViewModel>();

            //Emp ACR Graph & Recommendation
            Mapper.CreateMap<GraphAndRecommendation, PRM_EmpACRGraphAndRecommendation>();
            Mapper.CreateMap<PRM_EmpACRGraphAndRecommendation, GraphAndRecommendation>();

            //Emp ACR Reviewing Officer Comments
            Mapper.CreateMap<ReviewingOfficerComments, PRM_EmpACRReviewingOfficerComments>();
            Mapper.CreateMap<PRM_EmpACRReviewingOfficerComments, ReviewingOfficerComments>();

            //Emp ACR Information for authority
            Mapper.CreateMap<InformationForAuthority, PRM_EmpACRInformationForAuthority>();
            Mapper.CreateMap<PRM_EmpACRInformationForAuthority, InformationForAuthority>();

            //Explanation Received Information
            Mapper.CreateMap<ExplanationReceivedInfoViewModel, PRM_ExplanationReceivedInfo>();
            Mapper.CreateMap<PRM_ExplanationReceivedInfo, ExplanationReceivedInfoViewModel>();

            //Emp ACR Personal Characteristics
            Mapper.CreateMap<OfficerPersonalCharacteristicsViewModel, PRM_EmpACRPersonalCharacteristics>();
            Mapper.CreateMap<PRM_EmpACRPersonalCharacteristics, OfficerPersonalCharacteristicsViewModel>();

            Mapper.CreateMap<OfficerPersonalCharacteristicsDetailViewModel, PRM_EmpACRPersonalCharacteristicsDetail>();
            Mapper.CreateMap<PRM_EmpACRPersonalCharacteristicsDetail, OfficerPersonalCharacteristicsDetailViewModel>();

            //Emp ACR Performance of work
            Mapper.CreateMap<OfficerPerformanceofWorkViewModel, PRM_EmpACRPerformanceOfWork>();
            Mapper.CreateMap<PRM_EmpACRPerformanceOfWork, OfficerPerformanceofWorkViewModel>();

            Mapper.CreateMap<OfficerPerformanceofWorkDetailViewModel, PRM_EmpACRPerformanceOfWorkDetail>();
            Mapper.CreateMap<PRM_EmpACRPerformanceOfWorkDetail, OfficerPerformanceofWorkDetailViewModel>();


            //Hearing Fixation Information
            Mapper.CreateMap<HearingFixationInfoViewModel, PRM_HearingFixationInfo>();
            Mapper.CreateMap<PRM_HearingFixationInfo, HearingFixationInfoViewModel>();

            Mapper.CreateMap<HearingFixationInfoDetailViewModel, PRM_HearingFixationInfoDetail>();
            Mapper.CreateMap<PRM_HearingFixationInfoDetail, HearingFixationInfoDetailViewModel>();

            // Emp ACR Staff Info
            Mapper.CreateMap<StaffInfoViewModel, PRM_EmpACRStaffInfo>();
            Mapper.CreateMap<PRM_EmpACRStaffInfo, StaffInfoViewModel>();

            // Hearing Information
            Mapper.CreateMap<HearingInfoViwModel, PRM_HearingInfo>();
            Mapper.CreateMap<PRM_HearingInfo, HearingInfoViwModel>();

            //Emp ACR Staff Bio Data
            Mapper.CreateMap<StaffBioDataViewModel, PRM_EmpACRStaffBioData>();
            Mapper.CreateMap<PRM_EmpACRStaffBioData, StaffBioDataViewModel>();


            //Investigation Committee Information
            Mapper.CreateMap<InvestigationCommitteeInfoViewModel, PRM_InvestigationCommitteeInfo>();
            Mapper.CreateMap<PRM_InvestigationCommitteeInfo, InvestigationCommitteeInfoViewModel>();

            Mapper.CreateMap<InvestigationCommitteeInfoMemberInfoViewModel, PRM_InvestigationCommitteeInfoMemberInfo>();
            Mapper.CreateMap<PRM_InvestigationCommitteeInfoMemberInfo, InvestigationCommitteeInfoMemberInfoViewModel>();

            Mapper.CreateMap<InvestigationCommitteeInfoFormedForViewModel, PRM_InvestigationCommitteeInfoFormedFor>();
            Mapper.CreateMap<PRM_InvestigationCommitteeInfoFormedFor, InvestigationCommitteeInfoFormedForViewModel>();

            //Emp ACR Assessment Info
            Mapper.CreateMap<StaffAssessmentInfoViewModel, PRM_EmpACRAssessmentInfo>();
            Mapper.CreateMap<PRM_EmpACRAssessmentInfo, StaffAssessmentInfoViewModel>();

            Mapper.CreateMap<StaffAssessmentInfoDetailViewModel, PRM_EmpACRAssessmentInfoDetail>();
            Mapper.CreateMap<PRM_EmpACRAssessmentInfoDetail, StaffAssessmentInfoDetailViewModel>();

            //Office Equipment, Furniture info
            Mapper.CreateMap<OfficeEquipmentFurnitureInfoViewModel, PRM_OfficeEquipmentFurnitureInfo>();
            Mapper.CreateMap<PRM_OfficeEquipmentFurnitureInfo, OfficeEquipmentFurnitureInfoViewModel>();

            //Investigation Report
            Mapper.CreateMap<InvestigationReportViewModel, PRM_InvestigationReport>();
            Mapper.CreateMap<PRM_InvestigationReport, InvestigationReportViewModel>();

            //Appeal/Review Information
            Mapper.CreateMap<AppealReviewInfoViewModel, PRM_AppealReviewInfo>();
            Mapper.CreateMap<PRM_AppealReviewInfo, AppealReviewInfoViewModel>();

            //Office Equipment, Furniture info Usages
            Mapper.CreateMap<OfficeEquipmentFurnitureUsagesInfoViewModel, PRM_OfficeEquipmentFurnitureUsagesInfo>();
            Mapper.CreateMap<PRM_OfficeEquipmentFurnitureUsagesInfo, OfficeEquipmentFurnitureUsagesInfoViewModel>();

            Mapper.CreateMap<OfficeEquipmentFurnitureUsagesInfoDetailViewModel, PRM_OfficeEquipmentFurnitureUsagesInfoDetail>();
            Mapper.CreateMap<PRM_OfficeEquipmentFurnitureUsagesInfoDetail, OfficeEquipmentFurnitureUsagesInfoDetailViewModel>();


            //Manage Notes and Document Information 
            Mapper.CreateMap<NotesAndDocumentInfoViewModel, PRM_NotesAndDocumentInfo>();
            Mapper.CreateMap<PRM_NotesAndDocumentInfo, NotesAndDocumentInfoViewModel>();

            Mapper.CreateMap<NotesAndDocumentInfoAttachmentDetailViewModel, PRM_NotesAndDocumentInfoAttachmentDetail>();
            Mapper.CreateMap<PRM_NotesAndDocumentInfoAttachmentDetail, NotesAndDocumentInfoAttachmentDetailViewModel>();

            Mapper.CreateMap<NotesAndDocumentInfoCommentsDetailViewModel, PRM_NotesAndDocumentInfoCommentsDetail>();
            Mapper.CreateMap<PRM_NotesAndDocumentInfoCommentsDetail, NotesAndDocumentInfoCommentsDetailViewModel>();
            

            //Suspension of Employee Information 
            Mapper.CreateMap<SuspensionOfEmployeeViewModel, PRM_SuspensionOfEmployee>();
            Mapper.CreateMap<PRM_SuspensionOfEmployee, SuspensionOfEmployeeViewModel>();

            Mapper.CreateMap<SuspensionOfEmployeeDetailViewModel, PRM_SuspensionOfEmployeeDetail>();
            Mapper.CreateMap<PRM_SuspensionOfEmployeeDetail, SuspensionOfEmployeeDetailViewModel>();

            //Acceptance Letter Information 
            Mapper.CreateMap<AcceptanceLetterInfoViewModel, PRM_AcceptanceLetterInfo>();
            Mapper.CreateMap<PRM_AcceptanceLetterInfo, AcceptanceLetterInfoViewModel>();

            //Designation History 
            Mapper.CreateMap<DesignationHistoryViewModel, PRM_DesignationHistory>();
            Mapper.CreateMap<PRM_DesignationHistory, DesignationHistoryViewModel>();

            Mapper.CreateMap<DesignationHistoryDetailViewModel, PRM_DesignationHistoryDetail>();
            Mapper.CreateMap<PRM_DesignationHistoryDetail, DesignationHistoryDetailViewModel>();

            //Status Designation Info
            Mapper.CreateMap<StatusDesignationInfoViewModel, PRM_StatusDesignationInfo>();
            Mapper.CreateMap<PRM_StatusDesignationInfo, StatusDesignationInfoViewModel>();

            //Foreign Tour Info
            Mapper.CreateMap<ForeignTourInfoViewModel, PRM_EmpForeignTourInfo>();
            Mapper.CreateMap<PRM_EmpForeignTourInfo, ForeignTourInfoViewModel>();

            //Zone Info
            Mapper.CreateMap<ZoneInfoViewModel, PRM_ZoneInfo>();
            Mapper.CreateMap<PRM_ZoneInfo, ZoneInfoViewModel>();

            //Retirement Age Info
            Mapper.CreateMap<RetirementAgeInfoViwModel, PRM_RetirementAgeInfo>();
            Mapper.CreateMap<PRM_RetirementAgeInfo, RetirementAgeInfoViwModel>();

            //Life Insurance
            Mapper.CreateMap<LifeInsuranceViewModel, PRM_EmpLifeInsurance>();
            Mapper.CreateMap<PRM_EmpLifeInsurance, LifeInsuranceViewModel>();

            //Degree level Mapping
            Mapper.CreateMap<DegreeLevelMappingViewModel, PRM_DegreeLevelMapping>();
            Mapper.CreateMap<PRM_DegreeLevelMapping, DegreeLevelMappingViewModel>();

            Mapper.CreateMap<DegreeLevelMappingDetailViewModel, PRM_DegreeLevelMappingDetail>();
            Mapper.CreateMap<PRM_DegreeLevelMappingDetail, DegreeLevelMappingDetailViewModel>();


            //Approval Group Mapping
            Mapper.CreateMap<ApprovalGroupViewModel, APV_ApprovalGroup>();
            Mapper.CreateMap<APV_ApprovalGroup, ApprovalGroupViewModel>();

            //Approval Flow Master
            Mapper.CreateMap<ApprovalFlowViewModel, APV_ApprovalFlowMaster>();
            Mapper.CreateMap<APV_ApprovalFlowMaster, ApprovalFlowViewModel>();

            //Approval Flow Details
            Mapper.CreateMap<ApprovalFlowDetailViewModel, APV_ApprovalFlowDetail>();
            Mapper.CreateMap<APV_ApprovalFlowDetail, ApprovalFlowDetailViewModel>();

            //Assing Approval Flow
            Mapper.CreateMap<AssignApprovalFlowViewModel, APV_EmployeeWiseApproverInfo>();
            Mapper.CreateMap<APV_EmployeeWiseApproverInfo, AssignApprovalFlowViewModel>();

            //Set Approver
            Mapper.CreateMap<ApproverInformationDetailsViewModel, APV_ApproverInfo>();
            Mapper.CreateMap<APV_ApproverInfo, ApproverInformationDetailsViewModel>();

            //Set Approver
            Mapper.CreateMap<RequestedApplicationViewModel, APV_ApplicationInformation>();
            Mapper.CreateMap<APV_ApplicationInformation, RequestedApplicationViewModel>();

            //Approval Flow Configuration
            Mapper.CreateMap<ApprovalFlowConfigurationViewModel, APV_ApprovalFlowConfiguration>();
            Mapper.CreateMap<APV_ApprovalFlowConfiguration, ApprovalFlowConfigurationViewModel>();


            //Set Notification Flow Setup Mapping
            Mapper.CreateMap<NotificationFlowSetupViewModel, NTF_NotificationFlowSetup>();
            Mapper.CreateMap<NTF_NotificationFlowSetup, NotificationFlowSetupViewModel>();

            //Set Notification Flow Mapping
            Mapper.CreateMap<NotificationFlowViewModel, NTF_NotificationFlow>();
            Mapper.CreateMap<NTF_NotificationFlow, NotificationFlowViewModel>();

            //Set Notification Mapping
            Mapper.CreateMap<NotificationViewModel, NTF_Notification>();
            Mapper.CreateMap<NTF_Notification, NotificationViewModel>();

            //Set Notification Read By Mapping
            Mapper.CreateMap<NotificationReadByViewModel, NTF_NotificationReadBy>();
            Mapper.CreateMap<NTF_NotificationReadBy, NotificationReadByViewModel>();


            //SMS Sending
            Mapper.CreateMap<SendSMSViewModel, PRM_EmpSmsHistory>();
            Mapper.CreateMap<PRM_EmpSmsHistory, SendSMSViewModel>();

            //Employee Service History
            Mapper.CreateMap<EmployeeServiceHistoryViewModel, PRM_EmpServiceHistory>();
            Mapper.CreateMap<PRM_EmpServiceHistory, EmployeeServiceHistoryViewModel>();

            //Practice
            Mapper.CreateMap<PracticeViewModel, PRM_Practice>();
            Mapper.CreateMap<PRM_Practice, PracticeViewModel>();
        }
    }
}