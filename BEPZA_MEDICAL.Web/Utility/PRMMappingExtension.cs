using System.Collections.Generic;
using AutoMapper;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryHead;
using BEPZA_MEDICAL.Web.Areas.PGM.Models.SalaryStructure;

using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Employee;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.PersonalInfo;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRate;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.HumanResourceRateAssign;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.JobAdvertisementInfo;

using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalGroup;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ApprovalFlow;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Notification;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.RequestedApplication;

namespace BEPZA_MEDICAL.Web.Utility
{
    public static class PRMMappingExtensions
    {

        public static EmployeeContractAttachmentFiles ToModel(this PRM_EmpContactFiles gradeStep)
        {
            return Mapper.Map<PRM_EmpContactFiles, EmployeeContractAttachmentFiles>(gradeStep);
        }
        public static PRM_EmpContactFiles ToEntity(this EmployeeContractAttachmentFiles gradeStepModel)
        {
            return Mapper.Map<EmployeeContractAttachmentFiles, PRM_EmpContactFiles>(gradeStepModel);
        }

        //PRM_HumanResourceRateAssignMaster
        public static HumanResourceRateAssignModel ToModel(this PRM_HumanResourceRateAssignMaster humanResourceRateAssign)
        {
            return Mapper.Map<PRM_HumanResourceRateAssignMaster, HumanResourceRateAssignModel>(humanResourceRateAssign);
        }
        public static PRM_HumanResourceRateAssignMaster ToEntity(this HumanResourceRateAssignModel humanResourceRateAssignModel)
        {
            return Mapper.Map<HumanResourceRateAssignModel, PRM_HumanResourceRateAssignMaster>(humanResourceRateAssignModel);
        }

        //PRM_HumanResourceRateAssignDetail
        public static HumanResourceRateAssignDetailModel ToModel(this PRM_HumanResourceRateAssignDetail humanResourceRateDetailAssign)
        {
            return Mapper.Map<PRM_HumanResourceRateAssignDetail, HumanResourceRateAssignDetailModel>(humanResourceRateDetailAssign);
        }
        public static PRM_HumanResourceRateAssignDetail ToEntity(this HumanResourceRateAssignDetailModel humanResourceRateDetailAssignModel)
        {
            return Mapper.Map<HumanResourceRateAssignDetailModel, PRM_HumanResourceRateAssignDetail>(humanResourceRateDetailAssignModel);
        }

        //Human Resource Rate
        public static HumanResourceRateViewModel ToModel(this PRM_HumanResourceRateMaster humanResourceRate)
        {
            return Mapper.Map<PRM_HumanResourceRateMaster, HumanResourceRateViewModel>(humanResourceRate);
        }
        public static PRM_HumanResourceRateMaster ToEntity(this HumanResourceRateViewModel humanResourceRateModel)
        {
            return Mapper.Map<HumanResourceRateViewModel, PRM_HumanResourceRateMaster>(humanResourceRateModel);
        }

        //Human Resource Rate Detail
        public static HumanResourceRateDetailViewModel ToModel(this PRM_HumanResourceRateDetail humanResourceRateDetail)
        {
            return Mapper.Map<PRM_HumanResourceRateDetail, HumanResourceRateDetailViewModel>(humanResourceRateDetail);
        }
        public static PRM_HumanResourceRateDetail ToEntity(this HumanResourceRateDetailViewModel humanResourceRateDetailModel)
        {
            return Mapper.Map<HumanResourceRateDetailViewModel, PRM_HumanResourceRateDetail>(humanResourceRateDetailModel);
        }

        //GradeStep
        public static GradeStepViewModel ToModel(this PRM_GradeStep gradeStep)
        {
            return Mapper.Map<PRM_GradeStep, GradeStepViewModel>(gradeStep);
        }
        public static PRM_GradeStep ToEntity(this GradeStepViewModel gradeStepModel)
        {
            return Mapper.Map<GradeStepViewModel, PRM_GradeStep>(gradeStepModel);
        }

        //job grade
        public static JobGradeViewModel ToModel(this PRM_JobGrade jobGrade)
        {
            return Mapper.Map<PRM_JobGrade, JobGradeViewModel>(jobGrade);
        }
        public static PRM_JobGrade ToEntity(this JobGradeViewModel jobGrade)
        {
            return Mapper.Map<JobGradeViewModel, PRM_JobGrade>(jobGrade);
        }


        //Country Division
        public static CountryDivisionViewModel ToModel(this PRM_CountryDivision obj)
        {
            return Mapper.Map<PRM_CountryDivision, CountryDivisionViewModel>(obj);
        }
        public static PRM_CountryDivision ToEntity(this CountryDivisionViewModel model)
        {
            return Mapper.Map<CountryDivisionViewModel, PRM_CountryDivision>(model);
        }

        //District
        public static DistrictViewModel ToModel(this PRM_District obj)
        {
            return Mapper.Map<PRM_District, DistrictViewModel>(obj);
        }
        public static PRM_District ToEntity(this DistrictViewModel model)
        {
            return Mapper.Map<DistrictViewModel, PRM_District>(model);
        }

        //Thana
        public static ThanaViewModel ToModel(this PRM_Thana obj)
        {
            return Mapper.Map<PRM_Thana, ThanaViewModel>(obj);
        }
        public static PRM_Thana ToEntity(this ThanaViewModel model)
        {
            return Mapper.Map<ThanaViewModel, PRM_Thana>(model);
        }

        //Resource Category
        public static ResourceCategoryViewModel ToModel(this PRM_ResourceCategory obj)
        {
            return Mapper.Map<PRM_ResourceCategory, ResourceCategoryViewModel>(obj);
        }
        public static PRM_ResourceCategory ToEntity(this ResourceCategoryViewModel model)
        {
            return Mapper.Map<ResourceCategoryViewModel, PRM_ResourceCategory>(model);
        }

        //Resource Category
        public static ResourceInfoViewModel ToModel(this PRM_ResourceInfo obj)
        {
            return Mapper.Map<PRM_ResourceInfo, ResourceInfoViewModel>(obj);
        }
        public static PRM_ResourceInfo ToEntity(this ResourceInfoViewModel model)
        {
            return Mapper.Map<ResourceInfoViewModel, PRM_ResourceInfo>(model);
        }

        //Common Config
        public static CommonConfigViewModel ToModel(this CommonConfigGetResult obj)
        {
            return Mapper.Map<CommonConfigGetResult, CommonConfigViewModel>(obj);
        }
        public static CommonConfigGetResult ToEntity(this CommonConfigViewModel model)
        {
            return Mapper.Map<CommonConfigViewModel, CommonConfigGetResult>(model);
        }

        //Common Config Type
        public static List<CommonConfigTypeViewModel> ToModelList(this List<CommonConfigType> objlist)
        {
            List<CommonConfigTypeViewModel> list = new List<CommonConfigTypeViewModel>();
            foreach (var item in objlist)
            {
                list.Add((Mapper.Map<CommonConfigType, CommonConfigTypeViewModel>(item)));
            }
            return list;
        }
        public static List<CommonConfigType> ToEntityList(this List<CommonConfigTypeViewModel> modellist)
        {
            List<CommonConfigType> list = new List<CommonConfigType>();
            foreach (var item in modellist)
            {
                list.Add((Mapper.Map<CommonConfigTypeViewModel, CommonConfigType>(item)));
            }
            return list;
        }

        //Employment Info
        public static EmploymentInfoViewModel ToModel(this PRM_EmploymentInfo obj)
        {
            return Mapper.Map<PRM_EmploymentInfo, EmploymentInfoViewModel>(obj);
        }
        public static PRM_EmploymentInfo ToEntity(this EmploymentInfoViewModel obj)
        {
            return Mapper.Map<EmploymentInfoViewModel, PRM_EmploymentInfo>(obj);
        }

        //Personal Info

        public static PersonalInfoViewModel ToModel(this PRM_EmpPersonalInfo obj)
        {
            return Mapper.Map<PRM_EmpPersonalInfo, PersonalInfoViewModel>(obj);
        }
        public static PRM_EmpPersonalInfo ToEntity(this PersonalInfoViewModel obj)
        {
            return Mapper.Map<PersonalInfoViewModel, PRM_EmpPersonalInfo>(obj);
        }

        // Accademic info
        public static AccademicQlfnInfoViewModel ToModel(this PRM_EmpDegree entity)
        {
            return Mapper.Map<PRM_EmpDegree, AccademicQlfnInfoViewModel>(entity);
        }
        public static PRM_EmpDegree ToEntity(this AccademicQlfnInfoViewModel model)
        {
            return Mapper.Map<AccademicQlfnInfoViewModel, PRM_EmpDegree>(model);
        }

        // Job Experience info
        public static JobExperienceInfoViewModel ToModel(this PRM_EmpExperience obj)
        {
            return Mapper.Map<PRM_EmpExperience, JobExperienceInfoViewModel>(obj);
        }
        public static PRM_EmpExperience ToEntity(this JobExperienceInfoViewModel obj)
        {
            return Mapper.Map<JobExperienceInfoViewModel, PRM_EmpExperience>(obj);
        }

        // Professional Training info
        public static ProfessionalTrainingInfoViewModel ToModel(this PRM_EmpTrainingInfo obj)
        {
            return Mapper.Map<PRM_EmpTrainingInfo, ProfessionalTrainingInfoViewModel>(obj);
        }
        public static PRM_EmpTrainingInfo ToEntity(this ProfessionalTrainingInfoViewModel obj)
        {
            return Mapper.Map<ProfessionalTrainingInfoViewModel, PRM_EmpTrainingInfo>(obj);
        }

        // Professional Certification info
        public static ProfessionalCertificationInfoViewModel ToModel(this PRM_EmpCertification obj)
        {
            return Mapper.Map<PRM_EmpCertification, ProfessionalCertificationInfoViewModel>(obj);
        }
        public static PRM_EmpCertification ToEntity(this ProfessionalCertificationInfoViewModel obj)
        {
            return Mapper.Map<ProfessionalCertificationInfoViewModel, PRM_EmpCertification>(obj);
        }

        // Professional License info
        public static ProfessionalLicenseInfoViewModel ToModel(this PRM_EmpLicenseInfo obj)
        {
            return Mapper.Map<PRM_EmpLicenseInfo, ProfessionalLicenseInfoViewModel>(obj);
        }
        public static PRM_EmpLicenseInfo ToEntity(this ProfessionalLicenseInfoViewModel obj)
        {
            return Mapper.Map<ProfessionalLicenseInfoViewModel, PRM_EmpLicenseInfo>(obj);
        }

        // Professional License info
        public static JobSkillInfoViewModel ToModel(this PRM_EmpJobSkill obj)
        {
            return Mapper.Map<PRM_EmpJobSkill, JobSkillInfoViewModel>(obj);
        }
        public static PRM_EmpJobSkill ToEntity(this JobSkillInfoViewModel obj)
        {
            return Mapper.Map<JobSkillInfoViewModel, PRM_EmpJobSkill>(obj);
        }

        //---------------------------------------

        ////Salary Structure

        //public static SalaryStructureModel ToModel(this PRM_SalaryStructure obj)
        //{
        //    return Mapper.Map<PRM_SalaryStructure, SalaryStructureModel>(obj);
        //}
        //public static PRM_SalaryStructure ToEntity(this SalaryStructureModel obj)
        //{
        //    return Mapper.Map<SalaryStructureModel, PRM_SalaryStructure>(obj);
        //}

        //public static List<PRM_SalaryStructure> ToEntityList(this List<SalaryStructureModel> modellist)
        //{
        //    List<PRM_SalaryStructure> list = new List<PRM_SalaryStructure>();
        //    foreach (var item in modellist)
        //    {
        //        list.Add(Mapper.Map<SalaryStructureModel, PRM_SalaryStructure>(item));
        //    }
        //    return list;
        //}

        ////Salary Structure Details

        //public static SalaryStructureDetailsViewModel ToModel(this PRM_SalaryStructureDetail obj)
        //{
        //    return Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsViewModel>(obj);
        //}
        //public static PRM_SalaryStructureDetail ToEntity(this SalaryStructureDetailsViewModel obj)
        //{
        //    return Mapper.Map<SalaryStructureDetailsViewModel, PRM_SalaryStructureDetail>(obj);
        //}

        //public static List<SalaryStructureDetailsViewModel> ToModelList(this List<PRM_SalaryStructureDetail> objlist)
        //{
        //    List<SalaryStructureDetailsViewModel> list = new List<SalaryStructureDetailsViewModel>();
        //    foreach (var item in objlist)
        //    {
        //        list.Add(Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsViewModel>(item));
        //    }

        //    return list;
        //}
        //public static List<PRM_SalaryStructureDetail> ToEntityList(this List<SalaryStructureDetailsViewModel> modellist)
        //{
        //    List<PRM_SalaryStructureDetail> list = new List<PRM_SalaryStructureDetail>();
        //    foreach (var item in modellist)
        //    {
        //        list.Add(Mapper.Map<SalaryStructureDetailsViewModel, PRM_SalaryStructureDetail>(item));
        //    }

        //    return list;
        //}

        //---------------------------------------

        //public static List<SalaryStructureDetailsViewModel> ToModelList(this List<PRM_SalaryStructureDetail> objlist)
        //{
        //    List<SalaryStructureDetailsViewModel> list = new List<SalaryStructureDetailsViewModel>();
        //    foreach (var item in objlist)
        //    {
        //        list.Add(Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsViewModel>(item));
        //    }

        //    return list;
        //}


        //public static List<SalaryStructureDetailsViewModel> ToModelList(this List<PRM_SalaryHead> objlist)
        //{
        //    List<SalaryStructureDetailsViewModel> list = new List<SalaryStructureDetailsViewModel>();
        //    foreach (var item in objlist)
        //    {
        //        list.Add(Mapper.Map<PRM_SalaryHead, SalaryStructureDetailsViewModel>(item));
        //    }

        //    return list;
        //}

        public static List<PRM_SalaryHead> ToEntityList(this List<SalaryHeadViewModel> modellist)
        {
            List<PRM_SalaryHead> list = new List<PRM_SalaryHead>();
            foreach (var item in modellist)
            {
                list.Add(Mapper.Map<SalaryHeadViewModel, PRM_SalaryHead>(item));
            }

            return list;
        }
        public static DesignationViewModel ToModel(this PRM_Designation entity)
        {
            return Mapper.Map<PRM_Designation, DesignationViewModel>(entity);
        }
        public static PRM_Designation ToEntity(this DesignationViewModel model)
        {
            return Mapper.Map<DesignationViewModel, PRM_Designation>(model);
        }

        //Division Head mapping           
        public static DivisionHeadMapingViewModel ToModel(this PRM_DivisionHeadMaping obj)
        {
            return Mapper.Map<PRM_DivisionHeadMaping, DivisionHeadMapingViewModel>(obj);
        }
        public static PRM_DivisionHeadMaping ToEntity(this DivisionHeadMapingViewModel obj)
        {
            return Mapper.Map<DivisionHeadMapingViewModel, PRM_DivisionHeadMaping>(obj);
        }

        //Employment Contract Info           
        public static EmploymentContractPeriodViewModel ToModel(this PRM_EmpContractInfo obj)
        {
            return Mapper.Map<PRM_EmpContractInfo, EmploymentContractPeriodViewModel>(obj);
        }
        public static PRM_EmpContractInfo ToEntity(this EmploymentContractPeriodViewModel obj)
        {
            return Mapper.Map<EmploymentContractPeriodViewModel, PRM_EmpContractInfo>(obj);
        }

        // Employment PhotoGraph
        public static EmployeePhotoGraphViewModel ToModel(this PRM_EmpPhoto entity)
        {
            return Mapper.Map<PRM_EmpPhoto, EmployeePhotoGraphViewModel>(entity);
        }
        public static PRM_EmpPhoto ToEntity(this EmployeePhotoGraphViewModel model)
        {
            return Mapper.Map<EmployeePhotoGraphViewModel, PRM_EmpPhoto>(model);
        }

        //public static EmployeeTransferInfoViewModel ToModel(this PRM_EmpTransferInfo entity)
        //{
        //    return Mapper.Map<PRM_EmpTransferInfo, EmployeeTransferInfoViewModel>(entity);
        //}
        //public static PRM_EmpTransferInfo ToEntity(this EmployeeTransferInfoViewModel model)
        //{
        //    return Mapper.Map<EmployeeTransferInfoViewModel, PRM_EmpTransferInfo>(model);
        //}

        public static EmployeeSeperationViewModel ToModel(this PRM_EmpSeperation entity)
        {
            return Mapper.Map<PRM_EmpSeperation, EmployeeSeperationViewModel>(entity);
        }
        public static PRM_EmpSeperation ToEntity(this EmployeeSeperationViewModel model)
        {
            return Mapper.Map<EmployeeSeperationViewModel, PRM_EmpSeperation>(model);
        }

        public static EmployeeMappingViewModel ToModel(this PRM_HumanResourceMapping entity)
        {
            return Mapper.Map<PRM_HumanResourceMapping, EmployeeMappingViewModel>(entity);
        }
        public static PRM_HumanResourceMapping ToEntity(this EmployeeMappingViewModel model)
        {
            return Mapper.Map<EmployeeMappingViewModel, PRM_HumanResourceMapping>(model);
        }

        ////Salary Structure Details Deduction

        //public static SalaryStructureDetailsDeductionHeadViewModel ToModelSalaryStructureDetailsDeductionHeadViewModel(this PRM_SalaryStructureDetail obj)
        //{
        //    return Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsDeductionHeadViewModel>(obj);
        //}
        //public static PRM_SalaryStructureDetail ToEntity(this SalaryStructureDetailsDeductionHeadViewModel obj)
        //{
        //    return Mapper.Map<SalaryStructureDetailsDeductionHeadViewModel, PRM_SalaryStructureDetail>(obj);
        //}

        //emp salary details
        public static SalaryStructureDetailsModel ToModel(this PRM_EmpSalaryDetail obj)
        {
            return Mapper.Map<PRM_EmpSalaryDetail, SalaryStructureDetailsModel>(obj);
        }

        //Employee Confirmation Increment Promotion
        public static EmployeeConfirmationIncrementPromotionViewModel ToModel(this PRM_EmpStatusChange obj)
        {
            return Mapper.Map<PRM_EmpStatusChange, EmployeeConfirmationIncrementPromotionViewModel>(obj);
        }
        public static PRM_EmpStatusChange ToEntity(this EmployeeConfirmationIncrementPromotionViewModel model)
        {
            return Mapper.Map<EmployeeConfirmationIncrementPromotionViewModel, PRM_EmpStatusChange>(model);
        }
        //Personal Family Member Information

        public static PersonalFamilyMemberInformationViewModel ToModel(this PRM_EmpFamilyMemberInfo obj)
        {
            return Mapper.Map<PRM_EmpFamilyMemberInfo, PersonalFamilyMemberInformationViewModel>(obj);
        }
        public static PRM_EmpFamilyMemberInfo ToEntity(this PersonalFamilyMemberInformationViewModel model)
        {
            return Mapper.Map<PersonalFamilyMemberInformationViewModel, PRM_EmpFamilyMemberInfo>(model);
        }

        //Personal Emergency Contract Information
        public static PersonalEmergencyContractViewModel ToModel(this PRM_EmpContractPersonInfo obj)
        {
            return Mapper.Map<PRM_EmpContractPersonInfo, PersonalEmergencyContractViewModel>(obj);
        }
        public static PRM_EmpContractPersonInfo ToEntity(this PersonalEmergencyContractViewModel model)
        {
            return Mapper.Map<PersonalEmergencyContractViewModel, PRM_EmpContractPersonInfo>(model);
        }

        //Personal Emergency Contract Information        
        public static ReferenceInfoViewModel ToModel(this PRM_EmpReferanceGuarantor obj)
        {
            return Mapper.Map<PRM_EmpReferanceGuarantor, ReferenceInfoViewModel>(obj);
        }
        public static PRM_EmpReferanceGuarantor ToEntity(this ReferenceInfoViewModel model)
        {
            return Mapper.Map<ReferenceInfoViewModel, PRM_EmpReferanceGuarantor>(model);
        }

        //Personal Nominee        
        public static PersonalNomineeViewModel ToModel(this PRM_EmpNominee entity)
        {
            return Mapper.Map<PRM_EmpNominee, PersonalNomineeViewModel>(entity);
        }
        public static PRM_EmpNominee ToEntity(this PersonalNomineeViewModel model)
        {
            return Mapper.Map<PersonalNomineeViewModel, PRM_EmpNominee>(model);
        }

        //Personal Nominee Details        
        public static PersonalNomineeDetailsViewModel ToModel(this PRM_EmpNomineeDetail entity)
        {
            return Mapper.Map<PRM_EmpNomineeDetail, PersonalNomineeDetailsViewModel>(entity);
        }
        public static PRM_EmpNomineeDetail ToEntity(this PersonalNomineeDetailsViewModel model)
        {
            return Mapper.Map<PersonalNomineeDetailsViewModel, PRM_EmpNomineeDetail>(model);
        }

        //emp attachment        
        public static EmpAttachmentViewModel ToModel(this PRM_EmpAttachmentFile entity)
        {
            return Mapper.Map<PRM_EmpAttachmentFile, EmpAttachmentViewModel>(entity);
        }
        public static PRM_EmpAttachmentFile ToEntity(this EmpAttachmentViewModel model)
        {
            return Mapper.Map<EmpAttachmentViewModel, PRM_EmpAttachmentFile>(model);
        }

        //Personal Publication

        public static PersonalPublicationViewModel ToModel(this PRM_EmpPublicationInfo entity)
        {
            return Mapper.Map<PRM_EmpPublicationInfo, PersonalPublicationViewModel>(entity);
        }
        public static PRM_EmpPublicationInfo ToEntity(this PersonalPublicationViewModel model)
        {
            return Mapper.Map<PersonalPublicationViewModel, PRM_EmpPublicationInfo>(model);
        }

        //Personal Visa Information

        public static VisaInfoViewModel ToModel(this PRM_EmpVisaPassportInfo entity)
        {
            return Mapper.Map<PRM_EmpVisaPassportInfo, VisaInfoViewModel>(entity);
        }
        public static PRM_EmpVisaPassportInfo ToEntity(this VisaInfoViewModel model)
        {
            return Mapper.Map<VisaInfoViewModel, PRM_EmpVisaPassportInfo>(model);
        }

        //Company Information

        public static CompanyInformationViewModel ToModel(this PRM_CompanyInfo entity)
        {
            return Mapper.Map<PRM_CompanyInfo, CompanyInformationViewModel>(entity);
        }
        public static PRM_CompanyInfo ToEntity(this CompanyInformationViewModel model)
        {
            return Mapper.Map<CompanyInformationViewModel, PRM_CompanyInfo>(model);
        }


        //employee seperation
        public static EmployeeActivationViewModel ToModel(this PRM_EmployeeActivationHistory entity)
        {
            return Mapper.Map<PRM_EmployeeActivationHistory, EmployeeActivationViewModel>(entity);
        }
        public static PRM_EmployeeActivationHistory ToEntity(this EmployeeActivationViewModel model)
        {
            return Mapper.Map<EmployeeActivationViewModel, PRM_EmployeeActivationHistory>(model);
        }

        //Organogram Level
        public static OrganogramLevelViewModel ToModel(this PRM_OrganogramLevel entity)
        {
            return Mapper.Map<PRM_OrganogramLevel, OrganogramLevelViewModel>(entity);
        }
        public static PRM_OrganogramLevel ToEntity(this OrganogramLevelViewModel model)
        {
            return Mapper.Map<OrganogramLevelViewModel, PRM_OrganogramLevel>(model);
        }

        //SalaryScale
        public static SalaryScaleViewModel ToModel(this PRM_SalaryScale entity)
        {
            return Mapper.Map<PRM_SalaryScale, SalaryScaleViewModel>(entity);
        }
        public static PRM_SalaryScale ToEntity(this SalaryScaleViewModel model)
        {
            return Mapper.Map<SalaryScaleViewModel, PRM_SalaryScale>(model);
        }

        //public static List<PRM_SalaryStructure> ToEntityList(this List<SalaryStructureViewModel> modellist)
        //{
        //    List<PRM_SalaryStructure> list = new List<PRM_SalaryStructure>();
        //    foreach (var item in modellist)
        //    {
        //        list.Add(Mapper.Map<SalaryStructureViewModel, PRM_SalaryStructure>(item));
        //    }
        //    return list;
        //}

        //BankName
        public static BankNameViewModel ToModel(this PRM_BankName entity)
        {
            return Mapper.Map<PRM_BankName, BankNameViewModel>(entity);
        }
        public static PRM_BankName ToEntity(this BankNameViewModel model)
        {
            return Mapper.Map<BankNameViewModel, PRM_BankName>(model);
        }

        //BankBranch
        public static BankBranchViewModel ToModel(this PRM_BankBranch entity)
        {
            return Mapper.Map<PRM_BankBranch, BankBranchViewModel>(entity);
        }
        public static PRM_BankBranch ToEntity(this BankBranchViewModel model)
        {
            return Mapper.Map<BankBranchViewModel, PRM_BankBranch>(model);
        }

        //OrganizationalSetupManpowerInfo
        public static OrganizationalSetupManpowerInfoViewModel ToModel(this PRM_OrganizationalSetupManpowerInfo entity)
        {
            return Mapper.Map<PRM_OrganizationalSetupManpowerInfo, OrganizationalSetupManpowerInfoViewModel>(entity);
        }
        public static PRM_OrganizationalSetupManpowerInfo ToEntity(this OrganizationalSetupManpowerInfoViewModel model)
        {
            return Mapper.Map<OrganizationalSetupManpowerInfoViewModel, PRM_OrganizationalSetupManpowerInfo>(model);
        }

        //EmpWealthStatementInfo
        public static EmployeeWealthStatementViewModel ToModel(this PRM_EmpWealthStatementInfo entity)
        {
            return Mapper.Map<PRM_EmpWealthStatementInfo, EmployeeWealthStatementViewModel>(entity);
        }
        public static PRM_EmpWealthStatementInfo ToEntity(this EmployeeWealthStatementViewModel model)
        {
            return Mapper.Map<EmployeeWealthStatementViewModel, PRM_EmpWealthStatementInfo>(model);
        }

        //EmpLeverage
        public static EmpLeverageViewModel ToModel(this PRM_EmpLeverage entity)
        {
            return Mapper.Map<PRM_EmpLeverage, EmpLeverageViewModel>(entity);
        }
        public static PRM_EmpLeverage ToEntity(this EmpLeverageViewModel model)
        {
            return Mapper.Map<EmpLeverageViewModel, PRM_EmpLeverage>(model);
        }

        //Clearance Checklist Information
        public static ClearanceChecklistViewModel ToModel(this PRM_ClearanceChecklist entity)
        {
            return Mapper.Map<PRM_ClearanceChecklist, ClearanceChecklistViewModel>(entity);
        }
        public static PRM_ClearanceChecklist ToEntity(this ClearanceChecklistViewModel model)
        {
            return Mapper.Map<ClearanceChecklistViewModel, PRM_ClearanceChecklist>(model);
        }

        //Clearance Checklist Detail
        public static ClearanceChecklistDetailsViewModel ToModel(this PRM_ClearanceChecklistDetail entity)
        {
            return Mapper.Map<PRM_ClearanceChecklistDetail, ClearanceChecklistDetailsViewModel>(entity);
        }
        public static PRM_ClearanceChecklistDetail ToEntity(this ClearanceChecklistDetailsViewModel model)
        {
            return Mapper.Map<ClearanceChecklistDetailsViewModel, PRM_ClearanceChecklistDetail>(model);
        }

        //Recruitment Qualification Information
        public static RecruitmentQualificationInfoViewModel ToModel(this PRM_RecruitmentQualificationInfo entity)
        {
            return Mapper.Map<PRM_RecruitmentQualificationInfo, RecruitmentQualificationInfoViewModel>(entity);
        }
        public static PRM_RecruitmentQualificationInfo ToEntity(this RecruitmentQualificationInfoViewModel model)
        {
            return Mapper.Map<RecruitmentQualificationInfoViewModel, PRM_RecruitmentQualificationInfo>(model);
        }

        //Recruitment Qualification Details
        public static RecruitmentQualificationDetailsViewModel ToModel(this PRM_RecruitmentQualificationDetails entity)
        {
            return Mapper.Map<PRM_RecruitmentQualificationDetails, RecruitmentQualificationDetailsViewModel>(entity);
        }
        public static PRM_RecruitmentQualificationDetails ToEntity(this RecruitmentQualificationDetailsViewModel model)
        {
            return Mapper.Map<RecruitmentQualificationDetailsViewModel, PRM_RecruitmentQualificationDetails>(model);
        }

        //Employee Clearance Information
        public static EmpClearanceInfoViewModel ToModel(this PRM_EmpClearanceInfo entity)
        {
            return Mapper.Map<PRM_EmpClearanceInfo, EmpClearanceInfoViewModel>(entity);
        }
        public static PRM_EmpClearanceInfo ToEntity(this EmpClearanceInfoViewModel model)
        {
            return Mapper.Map<EmpClearanceInfoViewModel, PRM_EmpClearanceInfo>(model);
        }


        //Employee Clearance Form Details
        public static EmpClearanceInfoFormDetailsViewModel ToModel(this PRM_EmpClearanceInfoFormDetail entity)
        {
            return Mapper.Map<PRM_EmpClearanceInfoFormDetail, EmpClearanceInfoFormDetailsViewModel>(entity);
        }
        public static PRM_EmpClearanceInfoFormDetail ToEntity(this EmpClearanceInfoFormDetailsViewModel model)
        {
            return Mapper.Map<EmpClearanceInfoFormDetailsViewModel, PRM_EmpClearanceInfoFormDetail>(model);
        }


        //Employee Clearance Checklist Details
        public static EmpClearanceInfoChecklistDetailsViewModel ToModel(this PRM_EmpClearanceInfoCheklistDetail entity)
        {
            return Mapper.Map<PRM_EmpClearanceInfoCheklistDetail, EmpClearanceInfoChecklistDetailsViewModel>(entity);
        }
        public static PRM_EmpClearanceInfoCheklistDetail ToEntity(this EmpClearanceInfoChecklistDetailsViewModel model)
        {
            return Mapper.Map<EmpClearanceInfoChecklistDetailsViewModel, PRM_EmpClearanceInfoCheklistDetail>(model);
        }

        //Quota Info
        public static QuotaInfoViewModel ToModel(this PRM_QuotaInfo entity)
        {
            return Mapper.Map<PRM_QuotaInfo, QuotaInfoViewModel>(entity);
        }
        public static PRM_QuotaInfo ToEntity(this QuotaInfoViewModel model)
        {
            return Mapper.Map<QuotaInfoViewModel, PRM_QuotaInfo>(model);
        }

        //District Quota 
        public static DistrictQuotaViewModel ToModel(this PRM_DistrictQuota entity)
        {
            return Mapper.Map<PRM_DistrictQuota, DistrictQuotaViewModel>(entity);
        }
        public static PRM_DistrictQuota ToEntity(this DistrictQuotaViewModel model)
        {
            return Mapper.Map<DistrictQuotaViewModel, PRM_DistrictQuota>(model);
        }

        // Job Requisition Information
        public static JobRequisitionInfoViewModel ToModel(this PRM_JobRequisitionInfo entity)
        {
            return Mapper.Map<PRM_JobRequisitionInfo, JobRequisitionInfoViewModel>(entity);
        }
        public static PRM_JobRequisitionInfo ToEntity(this JobRequisitionInfoViewModel model)
        {
            return Mapper.Map<JobRequisitionInfoViewModel, PRM_JobRequisitionInfo>(model);
        }

        // Job Requisition Information Details
        public static JobRequisitionInfoDetailsViewModel ToModel(this PRM_JobRequisitionInfoDetail entity)
        {
            return Mapper.Map<PRM_JobRequisitionInfoDetail, JobRequisitionInfoDetailsViewModel>(entity);
        }
        public static PRM_JobRequisitionInfoDetail ToEntity(this JobRequisitionInfoDetailsViewModel model)
        {
            return Mapper.Map<JobRequisitionInfoDetailsViewModel, PRM_JobRequisitionInfoDetail>(model);
        }



        // Applicant Information
        public static ApplicantInfoViewModel ToModel(this PRM_ApplicantInfo entity)
        {
            return Mapper.Map<PRM_ApplicantInfo, ApplicantInfoViewModel>(entity);
        }
        public static PRM_ApplicantInfo ToEntity(this ApplicantInfoViewModel model)
        {
            return Mapper.Map<ApplicantInfoViewModel, PRM_ApplicantInfo>(model);
        }

        // Applicant Information Educatinal Qualification
        public static ApplicantInfoQualificationViewModel ToModel(this PRM_ApplicantInfoQualification entity)
        {
            return Mapper.Map<PRM_ApplicantInfoQualification, ApplicantInfoQualificationViewModel>(entity);
        }
        public static PRM_ApplicantInfoQualification ToEntity(this ApplicantInfoQualificationViewModel model)
        {
            return Mapper.Map<ApplicantInfoQualificationViewModel, PRM_ApplicantInfoQualification>(model);
        }

        // Job Requisition Information Summary
        public static JobRequisitionInfoSummaryViewModel ToModel(this PRM_JobRequisitionInfoSummary entity)
        {
            return Mapper.Map<PRM_JobRequisitionInfoSummary, JobRequisitionInfoSummaryViewModel>(entity);
        }
        public static PRM_JobRequisitionInfoSummary ToEntity(this JobRequisitionInfoSummaryViewModel model)
        {
            return Mapper.Map<JobRequisitionInfoSummaryViewModel, PRM_JobRequisitionInfoSummary>(model);
        }

        // Job Requisition Information Summary Detail
        public static RequisitionInfoSummaryDetail ToModel(this PRM_JobRequisitionInfoSummaryDetail entity)
        {
            return Mapper.Map<PRM_JobRequisitionInfoSummaryDetail, RequisitionInfoSummaryDetail>(entity);
        }
        public static PRM_JobRequisitionInfoSummaryDetail ToEntity(this RequisitionInfoSummaryDetail model)
        {
            return Mapper.Map<RequisitionInfoSummaryDetail, PRM_JobRequisitionInfoSummaryDetail>(model);
        }


        // Job Requisition Information Approval
        public static JobRequisitionInfoApprovalViewModel ToModel(this PRM_JobRequisitionInfoApproval entity)
        {
            return Mapper.Map<PRM_JobRequisitionInfoApproval, JobRequisitionInfoApprovalViewModel>(entity);
        }
        public static PRM_JobRequisitionInfoApproval ToEntity(this JobRequisitionInfoApprovalViewModel model)
        {
            return Mapper.Map<JobRequisitionInfoApprovalViewModel, PRM_JobRequisitionInfoApproval>(model);
        }

        // Job Requisition Information Approval Detail
        public static JobRequisitionInfoApprovalDetailViewModel ToModel(this PRM_JobRequisitionInfoApprovalDetail entity)
        {
            return Mapper.Map<PRM_JobRequisitionInfoApprovalDetail, JobRequisitionInfoApprovalDetailViewModel>(entity);
        }
        public static PRM_JobRequisitionInfoApprovalDetail ToEntity(this JobRequisitionInfoApprovalDetailViewModel model)
        {
            return Mapper.Map<JobRequisitionInfoApprovalDetailViewModel, PRM_JobRequisitionInfoApprovalDetail>(model);
        }

        // Clearance From Ministry 
        public static ClearanceInfoFromMinistryViewModel ToModel(this PRM_ClearanceInfoFromMinistry entity)
        {
            return Mapper.Map<PRM_ClearanceInfoFromMinistry, ClearanceInfoFromMinistryViewModel>(entity);
        }
        public static PRM_ClearanceInfoFromMinistry ToEntity(this ClearanceInfoFromMinistryViewModel model)
        {
            return Mapper.Map<ClearanceInfoFromMinistryViewModel, PRM_ClearanceInfoFromMinistry>(model);
        }

        // Clearance From Ministry Detail
        public static ClearanceInfoFromMinistryDetailViewModel ToModel(this PRM_ClearanceInfoFromMinistryDetail entity)
        {
            return Mapper.Map<PRM_ClearanceInfoFromMinistryDetail, ClearanceInfoFromMinistryDetailViewModel>(entity);
        }
        public static PRM_ClearanceInfoFromMinistryDetail ToEntity(this ClearanceInfoFromMinistryDetailViewModel model)
        {
            return Mapper.Map<ClearanceInfoFromMinistryDetailViewModel, PRM_ClearanceInfoFromMinistryDetail>(model);
        }

        // Applicant ShortList 

        public static ApplicantShortListViewModel ToModel(this PRM_ApplicantShortList entity)
        {
            return Mapper.Map<PRM_ApplicantShortList, ApplicantShortListViewModel>(entity);
        }
        public static PRM_ApplicantShortList ToEntity(this ApplicantShortListViewModel model)
        {
            return Mapper.Map<ApplicantShortListViewModel, PRM_ApplicantShortList>(model);
        }


        // Applicant ShortList Detail

        public static ApplicantShortListViewModel ToModel(this ApplicantShortListDetailViewModel entity)
        {
            return Mapper.Map<ApplicantShortListDetailViewModel, ApplicantShortListViewModel>(entity);
        }
        public static PRM_ApplicantShortListDetail ToEntity(this ApplicantShortListDetailViewModel model)
        {
            return Mapper.Map<ApplicantShortListDetailViewModel, PRM_ApplicantShortListDetail>(model);
        }


        // Applicant ShortList  Approval

        public static ApplicantShortListApprovalViewModel ToModel(this PRM_ApplicantShortListApproval entity)
        {
            return Mapper.Map<PRM_ApplicantShortListApproval, ApplicantShortListApprovalViewModel>(entity);
        }
        public static PRM_ApplicantShortListApproval ToEntity(this ApplicantShortListApprovalViewModel model)
        {
            return Mapper.Map<ApplicantShortListApprovalViewModel, PRM_ApplicantShortListApproval>(model);
        }


        // Applicant ShortList  Approval Detail

        public static ApplicantShortListApprovalDetailViewModel ToModel(this PRM_ApplicantShortListApprovalDetail entity)
        {
            return Mapper.Map<PRM_ApplicantShortListApprovalDetail, ApplicantShortListApprovalDetailViewModel>(entity);
        }
        public static PRM_ApplicantShortListApprovalDetail ToEntity(this ApplicantShortListApprovalDetailViewModel model)
        {
            return Mapper.Map<ApplicantShortListApprovalDetailViewModel, PRM_ApplicantShortListApprovalDetail>(model);
        }


        //Job Advertisement Information
        public static JobAdvertisementInfoViewModel ToModel(this PRM_JobAdvertisementInfo entity)
        {
            return Mapper.Map<PRM_JobAdvertisementInfo, JobAdvertisementInfoViewModel>(entity);
        }
        public static PRM_JobAdvertisementInfo ToEntity(this JobAdvertisementInfoViewModel model)
        {
            return Mapper.Map<JobAdvertisementInfoViewModel, PRM_JobAdvertisementInfo>(model);
        }

        //Job Advertisement Info Detail Media
        public static JobAdvertisementInfoMediaViewModel ToModel(this PRM_JobAdvertisementInfoDetailMedia entity)
        {
            return Mapper.Map<PRM_JobAdvertisementInfoDetailMedia, JobAdvertisementInfoMediaViewModel>(entity);
        }
        public static PRM_JobAdvertisementInfoDetailMedia ToEntity(this JobAdvertisementInfoMediaViewModel model)
        {
            return Mapper.Map<JobAdvertisementInfoMediaViewModel, PRM_JobAdvertisementInfoDetailMedia>(model);
        }

        //Job Advertisement Info Detail Requisition
        public static JobAdvertisementInfoRequisitionViewModel ToModel(this PRM_JobAdvertisementInfoDetailRequisition entity)
        {
            return Mapper.Map<PRM_JobAdvertisementInfoDetailRequisition, JobAdvertisementInfoRequisitionViewModel>(entity);
        }
        public static PRM_JobAdvertisementInfoDetailRequisition ToEntity(this JobAdvertisementInfoRequisitionViewModel model)
        {
            return Mapper.Map<JobAdvertisementInfoRequisitionViewModel, PRM_JobAdvertisementInfoDetailRequisition>(model);
        }

        //Job Advertisement Info Detail Attachment
        public static JobAdvertisementInfoAttachmentViewModel ToModel(this PRM_JobAdvertisementInfoDetailAttachment entity)
        {
            return Mapper.Map<PRM_JobAdvertisementInfoDetailAttachment, JobAdvertisementInfoAttachmentViewModel>(entity);
        }
        public static PRM_JobAdvertisementInfoDetailAttachment ToEntity(this JobAdvertisementInfoAttachmentViewModel model)
        {
            return Mapper.Map<JobAdvertisementInfoAttachmentViewModel, PRM_JobAdvertisementInfoDetailAttachment>(model);
        }

        //Selection Criteria
        public static SelectionCriteriaViewModel ToModel(this PRM_SelectionCriteria entity)
        {
            return Mapper.Map<PRM_SelectionCriteria, SelectionCriteriaViewModel>(entity);
        }
        public static PRM_SelectionCriteria ToEntity(this SelectionCriteriaViewModel model)
        {
            return Mapper.Map<SelectionCriteriaViewModel, PRM_SelectionCriteria>(model);
        }

        //Selection Criteria Detail
        public static SelectionCriteriaDetailViewModel ToModel(this PRM_SelectionCriteriaDetail entity)
        {
            return Mapper.Map<PRM_SelectionCriteriaDetail, SelectionCriteriaDetailViewModel>(entity);
        }
        public static PRM_SelectionCriteriaDetail ToEntity(this SelectionCriteriaDetailViewModel model)
        {
            return Mapper.Map<SelectionCriteriaDetailViewModel, PRM_SelectionCriteriaDetail>(model);
        }


        //Selection Board Information
        public static SelectionBoardInfoViewModel ToModel(this PRM_SelectionBoardInfo entity)
        {
            return Mapper.Map<PRM_SelectionBoardInfo, SelectionBoardInfoViewModel>(entity);
        }
        public static PRM_SelectionBoardInfo ToEntity(this SelectionBoardInfoViewModel model)
        {
            return Mapper.Map<SelectionBoardInfoViewModel, PRM_SelectionBoardInfo>(model);
        }


        //Selection Board Information Committee Member
        public static SelectionBoardInfoCommitteeViewModel ToModel(this PRM_SelectionBoardInfoCommittee entity)
        {
            return Mapper.Map<PRM_SelectionBoardInfoCommittee, SelectionBoardInfoCommitteeViewModel>(entity);
        }
        public static PRM_SelectionBoardInfoCommittee ToEntity(this SelectionBoardInfoCommitteeViewModel model)
        {
            return Mapper.Map<SelectionBoardInfoCommitteeViewModel, PRM_SelectionBoardInfoCommittee>(model);
        }

        //Applicant Interview Card Issue
        public static ApplicantInterviewCardIssueViewModel ToModel(this PRM_ApplicantInterviewCardIssue entity)
        {
            return Mapper.Map<PRM_ApplicantInterviewCardIssue, ApplicantInterviewCardIssueViewModel>(entity);
        }
        public static PRM_ApplicantInterviewCardIssue ToEntity(this ApplicantInterviewCardIssueViewModel model)
        {
            var map = new PRM_ApplicantInterviewCardIssue
            {
                Id = model.Id,
                ReferenceNo = model.ReferenceNo,
                ReferenceDate = model.ReferenceDate,
                InterviewDateAndTime = model.InterviewDate,
                InterviewTime = model.InterviewTime,
                Conditions = model.Conditions,
                Venue = model.Venue,
                Subject = model.Subject,
                Body = model.Body,
                SignatoryICNo = model.SignatoryICNo,
                AdvertisementInfoId = model.AdvertisementInfoId,
                DesignationId = model.DesignationId,
                SelectionCriteriaOrExamTypeId = model.SelectionCriteriaOrExamTypeId,
                IsIssue = model.IsIssue,
                IUser = model.IUser,
                EUser = model.EUser,
                EDate = model.EDate,
                FromRollNo = model.FromRollNo,
                ToRollNo = model.ToRollNo
            };
            return map;
        }

        //Applicant Interview Card Issue Detail
        public static ApplicantInterviewCardIssueDetailViewModel ToModel(this PRM_ApplicantInterviewCardIssueDetail entity)
        {
            return Mapper.Map<PRM_ApplicantInterviewCardIssueDetail, ApplicantInterviewCardIssueDetailViewModel>(entity);
        }
        public static PRM_ApplicantInterviewCardIssueDetail ToEntity(this ApplicantInterviewCardIssueDetailViewModel model)
        {
            return Mapper.Map<ApplicantInterviewCardIssueDetailViewModel, PRM_ApplicantInterviewCardIssueDetail>(model);
        }

        //Test Result for Application Information
        public static TestResultforApplicantInfoViewModel ToModel(this PRM_TestResultforApplicantInfo entity)
        {
            return Mapper.Map<PRM_TestResultforApplicantInfo, TestResultforApplicantInfoViewModel>(entity);
        }
        public static PRM_TestResultforApplicantInfo ToEntity(this TestResultforApplicantInfoViewModel model)
        {
            return Mapper.Map<TestResultforApplicantInfoViewModel, PRM_TestResultforApplicantInfo>(model);
        }

        //Selected Application Information Approval
        public static SelectedApplicantInfoApprovalViewModel ToModel(this PRM_SelectedApplicantInfoApproval entity)
        {
            return Mapper.Map<PRM_SelectedApplicantInfoApproval, SelectedApplicantInfoApprovalViewModel>(entity);
        }
        public static PRM_SelectedApplicantInfoApproval ToEntity(this SelectedApplicantInfoApprovalViewModel model)
        {
            return Mapper.Map<SelectedApplicantInfoApprovalViewModel, PRM_SelectedApplicantInfoApproval>(model);
        }

        //Selected Application Information Approval Detail
        public static SelectedApplicantInfoApprovalDetailViewModel ToModel(this PRM_SelectedApplicantInfoApprovalDetail entity)
        {
            return Mapper.Map<PRM_SelectedApplicantInfoApprovalDetail, SelectedApplicantInfoApprovalDetailViewModel>(entity);
        }
        public static PRM_SelectedApplicantInfoApprovalDetail ToEntity(this SelectedApplicantInfoApprovalDetailViewModel model)
        {
            return Mapper.Map<SelectedApplicantInfoApprovalDetailViewModel, PRM_SelectedApplicantInfoApprovalDetail>(model);
        }
        //Selected Application Information 
        public static SelectedApplicantInfoViewModel ToModel(this PRM_SelectedApplicantInfo entity)
        {
            return Mapper.Map<PRM_SelectedApplicantInfo, SelectedApplicantInfoViewModel>(entity);
        }
        public static PRM_SelectedApplicantInfo ToEntity(this SelectedApplicantInfoViewModel model)
        {
            return Mapper.Map<SelectedApplicantInfoViewModel, PRM_SelectedApplicantInfo>(model);
        }
        //Selected Application Information  Detail
        public static SelectedApplicantInfoDetailViewModel ToModel(this PRM_SelectedApplicantInfoDetail entity)
        {
            return Mapper.Map<PRM_SelectedApplicantInfoDetail, SelectedApplicantInfoDetailViewModel>(entity);
        }
        public static PRM_SelectedApplicantInfoDetail ToEntity(this SelectedApplicantInfoDetailViewModel model)
        {
            return Mapper.Map<SelectedApplicantInfoDetailViewModel, PRM_SelectedApplicantInfoDetail>(model);
        }

        //Manage Appointment Letter Informarion
        public static AppointmentLetterInfoViewModel ToModel(this PRM_AppointmentLetterInfo entity)
        {
            return Mapper.Map<PRM_AppointmentLetterInfo, AppointmentLetterInfoViewModel>(entity);
        }
        public static PRM_AppointmentLetterInfo ToEntity(this AppointmentLetterInfoViewModel model)
        {
            return Mapper.Map<AppointmentLetterInfoViewModel, PRM_AppointmentLetterInfo>(model);
        }

        //Order Type Informarion
        public static OrderTypeInfoViewModel ToModel(this PRM_OrderTypeInfo entity)
        {
            return Mapper.Map<PRM_OrderTypeInfo, OrderTypeInfoViewModel>(entity);
        }
        public static PRM_OrderTypeInfo ToEntity(this OrderTypeInfoViewModel model)
        {
            return Mapper.Map<OrderTypeInfoViewModel, PRM_OrderTypeInfo>(model);
        }

        //ACR Criteria Informarion
        public static ACRCriteriaInformationViewModel ToModel(this PRM_ACRCriteriaInformation entity)
        {
            return Mapper.Map<PRM_ACRCriteriaInformation, ACRCriteriaInformationViewModel>(entity);
        }
        public static PRM_ACRCriteriaInformation ToEntity(this ACRCriteriaInformationViewModel model)
        {
            return Mapper.Map<ACRCriteriaInformationViewModel, PRM_ACRCriteriaInformation>(model);
        }

        //Punishment Type Information
        public static PunishmentTypeInfoViewModel ToModel(this PRM_PunishmentTypeInfo entity)
        {
            return Mapper.Map<PRM_PunishmentTypeInfo, PunishmentTypeInfoViewModel>(entity);
        }
        public static PRM_PunishmentTypeInfo ToEntity(this PunishmentTypeInfoViewModel model)
        {
            return Mapper.Map<PunishmentTypeInfoViewModel, PRM_PunishmentTypeInfo>(model);
        }

        //Punishment Type Information Detail
        public static PunishmentTypeInfoDetailViewModel ToModel(this PRM_PunishmentTypeInfoDetail entity)
        {
            return Mapper.Map<PRM_PunishmentTypeInfoDetail, PunishmentTypeInfoDetailViewModel>(entity);
        }
        public static PRM_PunishmentTypeInfoDetail ToEntity(this PunishmentTypeInfoDetailViewModel model)
        {
            return Mapper.Map<PunishmentTypeInfoDetailViewModel, PRM_PunishmentTypeInfoDetail>(model);
        }

        //ACR Attributes Information
        public static ACRAttributesInformationViewModel ToModel(this PRM_ACRAttributesInformation entity)
        {
            return Mapper.Map<PRM_ACRAttributesInformation, ACRAttributesInformationViewModel>(entity);
        }
        public static PRM_ACRAttributesInformation ToEntity(this ACRAttributesInformationViewModel model)
        {
            return Mapper.Map<ACRAttributesInformationViewModel, PRM_ACRAttributesInformation>(model);
        }

        //ACR Attributes Information Detail
        public static ACRAttributesInformationDetailViewModel ToModel(this PRM_ACRAttributesInformationDetail entity)
        {
            return Mapper.Map<PRM_ACRAttributesInformationDetail, ACRAttributesInformationDetailViewModel>(entity);
        }
        public static PRM_ACRAttributesInformationDetail ToEntity(this ACRAttributesInformationDetailViewModel model)
        {
            return Mapper.Map<ACRAttributesInformationDetailViewModel, PRM_ACRAttributesInformationDetail>(model);
        }
        //ACR Rank Informarion
        public static ACRRankInformationViewModel ToModel(this PRM_ACRRankInformation entity)
        {
            return Mapper.Map<PRM_ACRRankInformation, ACRRankInformationViewModel>(entity);
        }
        public static PRM_ACRRankInformation ToEntity(this ACRRankInformationViewModel model)
        {
            return Mapper.Map<ACRRankInformationViewModel, PRM_ACRRankInformation>(model);
        }

        //Complaint Note Sheet
        public static ComplaintNoteSheetViewModel ToModel(this PRM_ComplaintNoteSheet entity)
        {
            return Mapper.Map<PRM_ComplaintNoteSheet, ComplaintNoteSheetViewModel>(entity);
        }
        public static PRM_ComplaintNoteSheet ToEntity(this ComplaintNoteSheetViewModel model)
        {
            return Mapper.Map<ComplaintNoteSheetViewModel, PRM_ComplaintNoteSheet>(model);
        }

        //FIR Information

        public static FIRInfoViewModel ToModel(this PRM_FIRInfo entity)
        {
            return Mapper.Map<PRM_FIRInfo, FIRInfoViewModel>(entity);
        }
        public static PRM_FIRInfo ToEntity(this FIRInfoViewModel model)
        {
            return Mapper.Map<FIRInfoViewModel, PRM_FIRInfo>(model);
        }

        //Charge SheetInformation

        public static ChargeSheetInfoViewModel ToModel(this PRM_ChargeSheetInfo entity)
        {
            return Mapper.Map<PRM_ChargeSheetInfo, ChargeSheetInfoViewModel>(entity);
        }
        public static PRM_ChargeSheetInfo ToEntity(this ChargeSheetInfoViewModel model)
        {
            return Mapper.Map<ChargeSheetInfoViewModel, PRM_ChargeSheetInfo>(model);
        }
        //Emp ACR Officer Information
        public static OfficerInfoViewModel ToModel(this PRM_EmpACROfficerInfo entity)
        {
            return Mapper.Map<PRM_EmpACROfficerInfo, OfficerInfoViewModel>(entity);
        }
        public static PRM_EmpACROfficerInfo ToEntity(this OfficerInfoViewModel model)
        {
            return Mapper.Map<OfficerInfoViewModel, PRM_EmpACROfficerInfo>(model);
        }

        //Emp ACR Officer Health Test Report
        public static OfficerHealthTestReportViewModel ToModel(this PRM_EmpACROfficerHealthTestReport entity)
        {
            return Mapper.Map<PRM_EmpACROfficerHealthTestReport, OfficerHealthTestReportViewModel>(entity);
        }
        public static PRM_EmpACROfficerHealthTestReport ToEntity(this OfficerHealthTestReportViewModel model)
        {
            return Mapper.Map<OfficerHealthTestReportViewModel, PRM_EmpACROfficerHealthTestReport>(model);
        }

        //Note and Order Information
        public static NoteOrderInfoViewModel ToModel(this PRM_NoteOrderInfo entity)
        {
            return Mapper.Map<PRM_NoteOrderInfo, NoteOrderInfoViewModel>(entity);
        }
        public static PRM_NoteOrderInfo ToEntity(this NoteOrderInfoViewModel model)
        {
            return Mapper.Map<NoteOrderInfoViewModel, PRM_NoteOrderInfo>(model);
        }

        //Notice Information
        public static NoticeInfoViewModel ToModel(this PRM_NoticeInfo entity)
        {
            return Mapper.Map<PRM_NoticeInfo, NoticeInfoViewModel>(entity);
        }
        public static PRM_NoticeInfo ToEntity(this NoticeInfoViewModel model)
        {
            return Mapper.Map<NoticeInfoViewModel, PRM_NoticeInfo>(model);
        }

        //Emp ACR Officer Bio data
        public static OfficerBioDataViewModel ToModel(this PRM_EmpACROfficerBioData entity)
        {
            return Mapper.Map<PRM_EmpACROfficerBioData, OfficerBioDataViewModel>(entity);
        }
        public static PRM_EmpACROfficerBioData ToEntity(this OfficerBioDataViewModel model)
        {
            return Mapper.Map<OfficerBioDataViewModel, PRM_EmpACROfficerBioData>(model);
        }

        //Emp ACR Graph & Recommendation
        public static GraphAndRecommendation ToModel(this PRM_EmpACRGraphAndRecommendation entity)
        {
            return Mapper.Map<PRM_EmpACRGraphAndRecommendation, GraphAndRecommendation>(entity);
        }
        public static PRM_EmpACRGraphAndRecommendation ToEntity(this GraphAndRecommendation model)
        {
            return Mapper.Map<GraphAndRecommendation, PRM_EmpACRGraphAndRecommendation>(model);
        }

        //Emp ACR Reviewing Officer Comments
        public static ReviewingOfficerComments ToModel(this PRM_EmpACRReviewingOfficerComments entity)
        {
            return Mapper.Map<PRM_EmpACRReviewingOfficerComments, ReviewingOfficerComments>(entity);
        }
        public static PRM_EmpACRReviewingOfficerComments ToEntity(this ReviewingOfficerComments model)
        {
            return Mapper.Map<ReviewingOfficerComments, PRM_EmpACRReviewingOfficerComments>(model);
        }

        //Emp ACR Information for authority
        public static InformationForAuthority ToModel(this PRM_EmpACRInformationForAuthority entity)
        {
            return Mapper.Map<PRM_EmpACRInformationForAuthority, InformationForAuthority>(entity);
        }
        public static PRM_EmpACRInformationForAuthority ToEntity(this InformationForAuthority model)
        {
            return Mapper.Map<InformationForAuthority, PRM_EmpACRInformationForAuthority>(model);
        }

        //Explanation Received Information
        public static ExplanationReceivedInfoViewModel ToModel(this PRM_ExplanationReceivedInfo entity)
        {
            return Mapper.Map<PRM_ExplanationReceivedInfo, ExplanationReceivedInfoViewModel>(entity);
        }
        public static PRM_ExplanationReceivedInfo ToEntity(this ExplanationReceivedInfoViewModel model)
        {
            return Mapper.Map<ExplanationReceivedInfoViewModel, PRM_ExplanationReceivedInfo>(model);
        }

        //Emp ACR Personal Characteristics
        public static OfficerPersonalCharacteristicsViewModel ToModel(this PRM_EmpACRPersonalCharacteristics entity)
        {
            return Mapper.Map<PRM_EmpACRPersonalCharacteristics, OfficerPersonalCharacteristicsViewModel>(entity);
        }
        public static PRM_EmpACRPersonalCharacteristics ToEntity(this OfficerPersonalCharacteristicsViewModel model)
        {
            return Mapper.Map<OfficerPersonalCharacteristicsViewModel, PRM_EmpACRPersonalCharacteristics>(model);
        }

        //Emp ACR Personal Characteristics Detail

        public static OfficerPersonalCharacteristicsDetailViewModel ToModel(this PRM_EmpACRPersonalCharacteristicsDetail entity)
        {
            return Mapper.Map<PRM_EmpACRPersonalCharacteristicsDetail, OfficerPersonalCharacteristicsDetailViewModel>(entity);
        }
        public static PRM_EmpACRPersonalCharacteristicsDetail ToEntity(this OfficerPersonalCharacteristicsDetailViewModel model)
        {
            return Mapper.Map<OfficerPersonalCharacteristicsDetailViewModel, PRM_EmpACRPersonalCharacteristicsDetail>(model);
        }

        //Emp ACR Performance of work
        public static OfficerPerformanceofWorkViewModel ToModel(this PRM_EmpACRPerformanceOfWork entity)
        {
            return Mapper.Map<PRM_EmpACRPerformanceOfWork, OfficerPerformanceofWorkViewModel>(entity);
        }
        public static PRM_EmpACRPerformanceOfWork ToEntity(this OfficerPerformanceofWorkViewModel model)
        {
            return Mapper.Map<OfficerPerformanceofWorkViewModel, PRM_EmpACRPerformanceOfWork>(model);
        }

        //Emp ACR Performance of work detail
        public static OfficerPerformanceofWorkDetailViewModel ToModel(this PRM_EmpACRPerformanceOfWorkDetail entity)
        {
            return Mapper.Map<PRM_EmpACRPerformanceOfWorkDetail, OfficerPerformanceofWorkDetailViewModel>(entity);
        }
        public static PRM_EmpACRPerformanceOfWorkDetail ToEntity(this OfficerPerformanceofWorkDetailViewModel model)
        {
            return Mapper.Map<OfficerPerformanceofWorkDetailViewModel, PRM_EmpACRPerformanceOfWorkDetail>(model);
        }

        //Hearing Fixation Information
        public static HearingFixationInfoViewModel ToModel(this PRM_HearingFixationInfo entity)
        {
            return Mapper.Map<PRM_HearingFixationInfo, HearingFixationInfoViewModel>(entity);
        }
        public static PRM_HearingFixationInfo ToEntity(this HearingFixationInfoViewModel model)
        {
            return Mapper.Map<HearingFixationInfoViewModel, PRM_HearingFixationInfo>(model);
        }

        //Hearing Fixation Information Detail
        public static HearingFixationInfoDetailViewModel ToModel(this PRM_HearingFixationInfoDetail entity)
        {
            return Mapper.Map<PRM_HearingFixationInfoDetail, HearingFixationInfoDetailViewModel>(entity);
        }
        public static PRM_HearingFixationInfoDetail ToEntity(this HearingFixationInfoDetailViewModel model)
        {
            return Mapper.Map<HearingFixationInfoDetailViewModel, PRM_HearingFixationInfoDetail>(model);
        }

        // Emp ACR Staff Info
        public static StaffInfoViewModel ToModel(this PRM_EmpACRStaffInfo entity)
        {
            return Mapper.Map<PRM_EmpACRStaffInfo, StaffInfoViewModel>(entity);
        }
        public static PRM_EmpACRStaffInfo ToEntity(this StaffInfoViewModel model)
        {
            return Mapper.Map<StaffInfoViewModel, PRM_EmpACRStaffInfo>(model);
        }


        //Hearing Information
        public static HearingInfoViwModel ToModel(this PRM_HearingInfo entity)
        {
            return Mapper.Map<PRM_HearingInfo, HearingInfoViwModel>(entity);
        }
        public static PRM_HearingInfo ToEntity(this HearingInfoViwModel model)
        {
            return Mapper.Map<HearingInfoViwModel, PRM_HearingInfo>(model);
        }
        //Emp ACR Staff Bio Data
        public static StaffBioDataViewModel ToModel(this PRM_EmpACRStaffBioData entity)
        {
            return Mapper.Map<PRM_EmpACRStaffBioData, StaffBioDataViewModel>(entity);
        }
        public static PRM_EmpACRStaffBioData ToEntity(this StaffBioDataViewModel model)
        {
            return Mapper.Map<StaffBioDataViewModel, PRM_EmpACRStaffBioData>(model);
        }

        //Investigation Committee Information
        public static InvestigationCommitteeInfoViewModel ToModel(this PRM_InvestigationCommitteeInfo entity)
        {
            return Mapper.Map<PRM_InvestigationCommitteeInfo, InvestigationCommitteeInfoViewModel>(entity);
        }
        public static PRM_InvestigationCommitteeInfo ToEntity(this InvestigationCommitteeInfoViewModel model)
        {
            return Mapper.Map<InvestigationCommitteeInfoViewModel, PRM_InvestigationCommitteeInfo>(model);
        }

        //Investigation Committee Information Member Info
        public static InvestigationCommitteeInfoMemberInfoViewModel ToModel(this PRM_InvestigationCommitteeInfoMemberInfo entity)
        {
            return Mapper.Map<PRM_InvestigationCommitteeInfoMemberInfo, InvestigationCommitteeInfoMemberInfoViewModel>(entity);
        }
        public static PRM_InvestigationCommitteeInfoMemberInfo ToEntity(this InvestigationCommitteeInfoMemberInfoViewModel model)
        {
            return Mapper.Map<InvestigationCommitteeInfoMemberInfoViewModel, PRM_InvestigationCommitteeInfoMemberInfo>(model);
        }

        //Investigation Committee Information Formed For
        public static InvestigationCommitteeInfoFormedForViewModel ToModel(this PRM_InvestigationCommitteeInfoFormedFor entity)
        {
            return Mapper.Map<PRM_InvestigationCommitteeInfoFormedFor, InvestigationCommitteeInfoFormedForViewModel>(entity);
        }
        public static PRM_InvestigationCommitteeInfoFormedFor ToEntity(this InvestigationCommitteeInfoFormedForViewModel model)
        {
            return Mapper.Map<InvestigationCommitteeInfoFormedForViewModel, PRM_InvestigationCommitteeInfoFormedFor>(model);
        }

        //Emp ACR Assessment Info
        public static StaffAssessmentInfoViewModel ToModel(this PRM_EmpACRAssessmentInfo entity)
        {
            return Mapper.Map<PRM_EmpACRAssessmentInfo, StaffAssessmentInfoViewModel>(entity);
        }
        public static PRM_EmpACRAssessmentInfo ToEntity(this StaffAssessmentInfoViewModel model)
        {
            return Mapper.Map<StaffAssessmentInfoViewModel, PRM_EmpACRAssessmentInfo>(model);
        }
        //Emp ACR Assessment Info Detail
        public static StaffAssessmentInfoDetailViewModel ToModel(this PRM_EmpACRAssessmentInfoDetail entity)
        {
            return Mapper.Map<PRM_EmpACRAssessmentInfoDetail, StaffAssessmentInfoDetailViewModel>(entity);
        }
        public static PRM_EmpACRAssessmentInfoDetail ToEntity(this StaffAssessmentInfoDetailViewModel model)
        {
            return Mapper.Map<StaffAssessmentInfoDetailViewModel, PRM_EmpACRAssessmentInfoDetail>(model);
        }
        //Office Equipment, Furniture info
        public static OfficeEquipmentFurnitureInfoViewModel ToModel(this PRM_OfficeEquipmentFurnitureInfo entity)
        {
            return Mapper.Map<PRM_OfficeEquipmentFurnitureInfo, OfficeEquipmentFurnitureInfoViewModel>(entity);
        }
        public static PRM_OfficeEquipmentFurnitureInfo ToEntity(this OfficeEquipmentFurnitureInfoViewModel model)
        {
            return Mapper.Map<OfficeEquipmentFurnitureInfoViewModel, PRM_OfficeEquipmentFurnitureInfo>(model);
        }

        //Investigation Report
        public static InvestigationReportViewModel ToModel(this PRM_InvestigationReport entity)
        {
            return Mapper.Map<PRM_InvestigationReport, InvestigationReportViewModel>(entity);
        }
        public static PRM_InvestigationReport ToEntity(this InvestigationReportViewModel model)
        {
            return Mapper.Map<InvestigationReportViewModel, PRM_InvestigationReport>(model);
        }


        //Appeal/Review Information

        public static AppealReviewInfoViewModel ToModel(this PRM_AppealReviewInfo entity)
        {
            return Mapper.Map<PRM_AppealReviewInfo, AppealReviewInfoViewModel>(entity);
        }
        public static PRM_AppealReviewInfo ToEntity(this AppealReviewInfoViewModel model)
        {
            return Mapper.Map<AppealReviewInfoViewModel, PRM_AppealReviewInfo>(model);
        }
        //Office Equipment, Furniture info Usages
        public static OfficeEquipmentFurnitureUsagesInfoViewModel ToModel(this PRM_OfficeEquipmentFurnitureUsagesInfo entity)
        {
            return Mapper.Map<PRM_OfficeEquipmentFurnitureUsagesInfo, OfficeEquipmentFurnitureUsagesInfoViewModel>(entity);
        }
        public static PRM_OfficeEquipmentFurnitureUsagesInfo ToEntity(this OfficeEquipmentFurnitureUsagesInfoViewModel model)
        {
            return Mapper.Map<OfficeEquipmentFurnitureUsagesInfoViewModel, PRM_OfficeEquipmentFurnitureUsagesInfo>(model);
        }
        //Office Equipment, Furniture info Usages Detail
        public static OfficeEquipmentFurnitureUsagesInfoDetailViewModel ToModel(this PRM_OfficeEquipmentFurnitureUsagesInfoDetail entity)
        {
            return Mapper.Map<PRM_OfficeEquipmentFurnitureUsagesInfoDetail, OfficeEquipmentFurnitureUsagesInfoDetailViewModel>(entity);
        }
        public static PRM_OfficeEquipmentFurnitureUsagesInfoDetail ToEntity(this OfficeEquipmentFurnitureUsagesInfoDetailViewModel model)
        {
            return Mapper.Map<OfficeEquipmentFurnitureUsagesInfoDetailViewModel, PRM_OfficeEquipmentFurnitureUsagesInfoDetail>(model);
        }


        //Manage Notes and Document Information 
        public static NotesAndDocumentInfoViewModel ToModel(this PRM_NotesAndDocumentInfo entity)
        {
            return Mapper.Map<PRM_NotesAndDocumentInfo, NotesAndDocumentInfoViewModel>(entity);
        }
        public static PRM_NotesAndDocumentInfo ToEntity(this NotesAndDocumentInfoViewModel model)
        {
            return Mapper.Map<NotesAndDocumentInfoViewModel, PRM_NotesAndDocumentInfo>(model);
        }
        //Manage Notes and Document Information Attachment Detail
        public static NotesAndDocumentInfoAttachmentDetailViewModel ToModel(this PRM_NotesAndDocumentInfoAttachmentDetail entity)
        {
            return Mapper.Map<PRM_NotesAndDocumentInfoAttachmentDetail, NotesAndDocumentInfoAttachmentDetailViewModel>(entity);
        }
        public static PRM_NotesAndDocumentInfoAttachmentDetail ToEntity(this NotesAndDocumentInfoAttachmentDetailViewModel model)
        {
            return Mapper.Map<NotesAndDocumentInfoAttachmentDetailViewModel, PRM_NotesAndDocumentInfoAttachmentDetail>(model);
        }
        //Manage Notes and Document Information Comments Detail
        public static NotesAndDocumentInfoCommentsDetailViewModel ToModel(this PRM_NotesAndDocumentInfoCommentsDetail entity)
        {
            return Mapper.Map<PRM_NotesAndDocumentInfoCommentsDetail, NotesAndDocumentInfoCommentsDetailViewModel>(entity);
        }
        public static PRM_NotesAndDocumentInfoCommentsDetail ToEntity(this NotesAndDocumentInfoCommentsDetailViewModel model)
        {
            return Mapper.Map<NotesAndDocumentInfoCommentsDetailViewModel, PRM_NotesAndDocumentInfoCommentsDetail>(model);
        }


        //Suspension of Employee
        public static SuspensionOfEmployeeViewModel ToModel(this PRM_SuspensionOfEmployee entity)
        {
            return Mapper.Map<PRM_SuspensionOfEmployee, SuspensionOfEmployeeViewModel>(entity);
        }
        public static PRM_SuspensionOfEmployee ToEntity(this SuspensionOfEmployeeViewModel model)
        {
            return Mapper.Map<SuspensionOfEmployeeViewModel, PRM_SuspensionOfEmployee>(model);
        }
        //Suspension of Employee Detail
        public static SuspensionOfEmployeeDetailViewModel ToModel(this PRM_SuspensionOfEmployeeDetail entity)
        {
            return Mapper.Map<PRM_SuspensionOfEmployeeDetail, SuspensionOfEmployeeDetailViewModel>(entity);
        }
        public static PRM_SuspensionOfEmployeeDetail ToEntity(this SuspensionOfEmployeeDetailViewModel model)
        {
            return Mapper.Map<SuspensionOfEmployeeDetailViewModel, PRM_SuspensionOfEmployeeDetail>(model);
        }
        //Acceptance Letter Information 
        public static AcceptanceLetterInfoViewModel ToModel(this PRM_AcceptanceLetterInfo entity)
        {
            return Mapper.Map<PRM_AcceptanceLetterInfo, AcceptanceLetterInfoViewModel>(entity);
        }
        public static PRM_AcceptanceLetterInfo ToEntity(this AcceptanceLetterInfoViewModel model)
        {
            return Mapper.Map<AcceptanceLetterInfoViewModel, PRM_AcceptanceLetterInfo>(model);
        }


        //Designation History 
        public static DesignationHistoryViewModel ToModel(this PRM_DesignationHistory entity)
        {
            return Mapper.Map<PRM_DesignationHistory, DesignationHistoryViewModel>(entity);
        }
        public static PRM_DesignationHistory ToEntity(this DesignationHistoryViewModel model)
        {
            return Mapper.Map<DesignationHistoryViewModel, PRM_DesignationHistory>(model);
        }


        //Designation History  Detail

        public static DesignationHistoryDetailViewModel ToModel(this PRM_DesignationHistoryDetail entity)
        {
            return Mapper.Map<PRM_DesignationHistoryDetail, DesignationHistoryDetailViewModel>(entity);
        }
        public static PRM_DesignationHistoryDetail ToEntity(this DesignationHistoryDetailViewModel model)
        {
            return Mapper.Map<DesignationHistoryDetailViewModel, PRM_DesignationHistoryDetail>(model);
        }

        //Status Designation Info
        public static StatusDesignationInfoViewModel ToModel(this PRM_StatusDesignationInfo entity)
        {
            return Mapper.Map<PRM_StatusDesignationInfo, StatusDesignationInfoViewModel>(entity);
        }
        public static PRM_StatusDesignationInfo ToEntity(this StatusDesignationInfoViewModel model)
        {
            return Mapper.Map<StatusDesignationInfoViewModel, PRM_StatusDesignationInfo>(model);
        }

        //Foreign Tour Info
        public static ForeignTourInfoViewModel ToModel(this PRM_EmpForeignTourInfo entity)
        {
            return Mapper.Map<PRM_EmpForeignTourInfo, ForeignTourInfoViewModel>(entity);
        }
        public static PRM_EmpForeignTourInfo ToEntity(this ForeignTourInfoViewModel model)
        {
            return Mapper.Map<ForeignTourInfoViewModel, PRM_EmpForeignTourInfo>(model);
        }

        //Zone Info

        public static ZoneInfoViewModel ToModel(this PRM_ZoneInfo entity)
        {
            return Mapper.Map<PRM_ZoneInfo, ZoneInfoViewModel>(entity);
        }
        public static PRM_ZoneInfo ToEntity(this ZoneInfoViewModel model)
        {
            return Mapper.Map<ZoneInfoViewModel, PRM_ZoneInfo>(model);
        }

        //Retirement Age Info


        public static RetirementAgeInfoViwModel ToModel(this PRM_RetirementAgeInfo entity)
        {
            return Mapper.Map<PRM_RetirementAgeInfo, RetirementAgeInfoViwModel>(entity);
        }
        public static PRM_RetirementAgeInfo ToEntity(this RetirementAgeInfoViwModel model)
        {
            return Mapper.Map<RetirementAgeInfoViwModel, PRM_RetirementAgeInfo>(model);
        }

        //Life Insurance
        public static LifeInsuranceViewModel ToModel(this PRM_EmpLifeInsurance entity)
        {
            return Mapper.Map<PRM_EmpLifeInsurance, LifeInsuranceViewModel>(entity);
        }
        public static PRM_EmpLifeInsurance ToEntity(this LifeInsuranceViewModel model)
        {
            return Mapper.Map<LifeInsuranceViewModel, PRM_EmpLifeInsurance>(model);
        }

        //Degree level Mapping
        public static DegreeLevelMappingViewModel ToModel(this PRM_DegreeLevelMapping entity)
        {
            return Mapper.Map<PRM_DegreeLevelMapping, DegreeLevelMappingViewModel>(entity);
        }
        public static PRM_DegreeLevelMapping ToEntity(this DegreeLevelMappingViewModel model)
        {
            return Mapper.Map<DegreeLevelMappingViewModel, PRM_DegreeLevelMapping>(model);
        }

        //Degree level Mapping Detail
        public static DegreeLevelMappingDetailViewModel ToModel(this PRM_DegreeLevelMappingDetail entity)
        {
            return Mapper.Map<PRM_DegreeLevelMappingDetail, DegreeLevelMappingDetailViewModel>(entity);
        }
        public static PRM_DegreeLevelMappingDetail ToEntity(this DegreeLevelMappingDetailViewModel model)
        {
            return Mapper.Map<DegreeLevelMappingDetailViewModel, PRM_DegreeLevelMappingDetail>(model);
        }

        //Approval Group
        public static ApprovalGroupViewModel ToModel(this APV_ApprovalGroup entity)
        {
            return Mapper.Map<APV_ApprovalGroup, ApprovalGroupViewModel>(entity);
        }
        public static APV_ApprovalGroup ToEntity(this ApprovalGroupViewModel model)
        {
            return Mapper.Map<ApprovalGroupViewModel, APV_ApprovalGroup>(model);
        }

        //Approval Flow Master
        public static ApprovalFlowViewModel ToModel(this APV_ApprovalFlowMaster entity)
        {
            return Mapper.Map<APV_ApprovalFlowMaster, ApprovalFlowViewModel>(entity);
        }
        public static APV_ApprovalFlowMaster ToEntity(this ApprovalFlowViewModel model)
        {
            return Mapper.Map<ApprovalFlowViewModel, APV_ApprovalFlowMaster>(model);
        }

        //Approval Flow Details
        public static ApprovalFlowDetailViewModel ToModel(this APV_ApprovalFlowDetail entity)
        {
            return Mapper.Map<APV_ApprovalFlowDetail, ApprovalFlowDetailViewModel>(entity);
        }
        public static APV_ApprovalFlowDetail ToEntity(this ApprovalFlowDetailViewModel model)
        {
            return Mapper.Map<ApprovalFlowDetailViewModel, APV_ApprovalFlowDetail>(model);
        }

        //Assign Approval Flow
        public static AssignApprovalFlowViewModel ToModel(this APV_EmployeeWiseApproverInfo entity)
        {
            return Mapper.Map<APV_EmployeeWiseApproverInfo, AssignApprovalFlowViewModel>(entity);
        }
        public static APV_EmployeeWiseApproverInfo ToEntity(this AssignApprovalFlowViewModel model)
        {
            return Mapper.Map<AssignApprovalFlowViewModel, APV_EmployeeWiseApproverInfo>(model);
        }

        //Set Approver
        public static ApproverInformationDetailsViewModel ToModel(this APV_ApproverInfo entity)
        {
            return Mapper.Map<APV_ApproverInfo, ApproverInformationDetailsViewModel>(entity);
        }
        public static APV_ApproverInfo ToEntity(this ApproverInformationDetailsViewModel model)
        {
            return Mapper.Map<ApproverInformationDetailsViewModel, APV_ApproverInfo>(model);
        }

        public static RequestedApplicationViewModel ToModel(this APV_ApplicationInformation entity)
        {
            return Mapper.Map<APV_ApplicationInformation, RequestedApplicationViewModel>(entity);
        }
        public static APV_ApplicationInformation ToEntity(this RequestedApplicationViewModel model)
        {
            return Mapper.Map<RequestedApplicationViewModel, APV_ApplicationInformation>(model);
        }


        //Approval Flow Configuration
        public static ApprovalFlowConfigurationViewModel ToModel(this APV_ApprovalFlowConfiguration entity)
        {
            return Mapper.Map<APV_ApprovalFlowConfiguration, ApprovalFlowConfigurationViewModel>(entity);
        }
        public static APV_ApprovalFlowConfiguration ToEntity(this ApprovalFlowConfigurationViewModel model)
        {
            return Mapper.Map<ApprovalFlowConfigurationViewModel, APV_ApprovalFlowConfiguration>(model);
        }


        //Notification Flow Setup
        public static NotificationFlowSetupViewModel ToModel(this NTF_NotificationFlowSetup entity)
        {
            return Mapper.Map<NTF_NotificationFlowSetup, NotificationFlowSetupViewModel>(entity);
        }
        public static NTF_NotificationFlowSetup ToEntity(this NotificationFlowSetupViewModel model)
        {
            return Mapper.Map<NotificationFlowSetupViewModel, NTF_NotificationFlowSetup>(model);
        }

        //Notification Flow
        public static NotificationFlowViewModel ToModel(this NTF_NotificationFlow entity)
        {
            return Mapper.Map<NTF_NotificationFlow, NotificationFlowViewModel>(entity);
        }
        public static NTF_NotificationFlow ToEntity(this NotificationFlowViewModel model)
        {
            return Mapper.Map<NotificationFlowViewModel, NTF_NotificationFlow>(model);
        }

        //Notification
        public static NotificationViewModel ToModel(this NTF_Notification entity)
        {
            return Mapper.Map<NTF_Notification, NotificationViewModel>(entity);
        }
        public static NTF_Notification ToEntity(this NotificationViewModel model)
        {
            return Mapper.Map<NotificationViewModel, NTF_Notification>(model);
        }

        //Notification Read By
        public static NotificationReadByViewModel ToModel(this NTF_NotificationReadBy entity)
        {
            return Mapper.Map<NTF_NotificationReadBy, NotificationReadByViewModel>(entity);
        }
        public static NTF_NotificationReadBy ToEntity(this NotificationReadByViewModel model)
        {
            return Mapper.Map<NotificationReadByViewModel, NTF_NotificationReadBy>(model);
        }

        //SMS Sending
        public static SendSMSViewModel ToModel(this PRM_EmpSmsHistory entity)
        {
            return Mapper.Map<PRM_EmpSmsHistory, SendSMSViewModel>(entity);
        }
        public static PRM_EmpSmsHistory ToEntity(this SendSMSViewModel model)
        {
            return Mapper.Map<SendSMSViewModel, PRM_EmpSmsHistory>(model);
        }

        //Employee Service History
        public static EmployeeServiceHistoryViewModel ToModel(this PRM_EmpServiceHistory entity)
        {
            return Mapper.Map<PRM_EmpServiceHistory, EmployeeServiceHistoryViewModel>(entity);
        }
        public static PRM_EmpServiceHistory ToEntity(this EmployeeServiceHistoryViewModel model)
        {
            return Mapper.Map<EmployeeServiceHistoryViewModel, PRM_EmpServiceHistory>(model);
        }



        //Practice
        public static PracticeViewModel ToModel(this PRM_Practice entity)
        {
            return Mapper.Map<PRM_Practice, PracticeViewModel>(entity);
        }
        public static PRM_Practice ToEntity(this PracticeViewModel model)
        {
            return Mapper.Map<PracticeViewModel, PRM_Practice>(model);
        }
        
    }
}