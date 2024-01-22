using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;

namespace BEPZA_MEDICAL.Domain.PRM
{
    public class PersonalInfoService : PRMCommonSevice
    {
        #region Constructor

        public PersonalInfoService(PRM_UnitOfWork uow)
            : base(uow) { }

        #endregion

        #region Bisiness Logic Validation

        public string CheckBusinessLogic(PRM_EmpPersonalInfo obj)
        {
            string businessError = string.Empty;

            //DateTime joiningDate = this.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId).DateofJoining;
            var emp = this.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId);

            if (emp.DateofJoining != null && emp.DateofBirth > emp.DateofJoining)
            {
                businessError = "Date of Birth should be lower than Joining Date(" + emp.DateofJoining.ToString("dd-MM-yyyy") + ")";
                return businessError;
            }
            if (obj.MarriageDate != null && emp.DateofBirth > obj.MarriageDate)
            {
                businessError = "Marrige Date should be greater than Date of Birth";
                return businessError;
            }

            return string.Empty;
        }

        public string CheckBusinessLogicACC(PRM_EmpDegree obj)
        {
            string businessError = string.Empty;

            if (CheckGPASelected(obj))
            {
                if (obj.CGPA == 0)
                {
                    businessError = "The field CGPA must be greater than 0.";

                    return businessError;
                }
                if (obj.Scale == 0)
                {
                    businessError = "The field Scale must be greater than 0.";

                    return businessError;
                }
                if (obj.CGPA > obj.Scale)
                {
                    businessError = "The field Scale must be greater than the field CGPA.";

                    return businessError;
                }
            }
            //else
            //{
            //    if (obj.ObtainMarks == 0)
            //    {
            //        businessError = "The field Obtained Marks (%) must be greater than 0.";

            //        return businessError;
            //    }
            //}

            return string.Empty;
        }

        public bool CheckGPASelected(PRM_EmpDegree obj)
        {
            var resultList = PRMUnit.AcademicGradeRepository.Fetch();
            if (resultList != null)
            {
                foreach (var item in resultList)
                {
                    if (obj.AcademicGradeId == item.Id)
                    {
                        if (item.Name.ToUpper() == "GPA")
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public string CheckBusinessLogicJOBEXP(PRM_EmpExperience obj, string strMode)
        {
            string businessError = string.Empty;

            if (obj.EndDate < obj.FromDate)
            {
                businessError = "The field To Date must be greater than From Date.";

                return businessError;
            }
            if (!obj.isInternalExperience)
            {
                var emp = PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId);

                if (obj.FromDate >= emp.DateofJoining)
                {
                    businessError = "The field From Date must be less than the Date of Joining (" + emp.DateofJoining.ToString("dd-MMM-yyyy") + ").";

                    return businessError;
                }
            }

            if (JobExperienceDateRangeCheck(obj.FromDate, obj.EndDate, obj.Id, obj.EmployeeId, strMode))
            {
                return "Experience period is not valid.";
            }

            return string.Empty;
        }

        public string CheckBusinessLogicPROTRA(PRM_EmpTrainingInfo obj, string strMode)
        {
            string businessError = string.Empty;

            //DateTime dateOfBirth = base.PRMUnit.PersonalInfoRepository.Get(q => q.EmployeeId == obj.EmployeeId).FirstOrDefault().DateofBirth;
            var emp = base.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId);

            //if (emp.DateofBirth != null && emp.DateofBirth > obj.FromDate)
            //{
            //    businessError = "The field From Date must be Greater than Date of Birth(" + Convert.ToDateTime(emp.DateofBirth).ToString("dd-MM-yyyy") + ")";
            //    return businessError;
            //}

            //if (obj.ToDate < obj.FromDate)
            //{
            //    businessError = "The field To Date must be greater than From Date.";

            //    return businessError;
            //}

            ////  if (obj.ToDate == obj.FromDate && obj.Duration < 1)
            //if (obj.Duration < 1)
            //{
            //    businessError = "The field Duration must be greater than 0.";

            //    return businessError;
            //}
            //var hours = (obj.ToDate - obj.FromDate).TotalHours + 24;
            //if (obj.Duration > hours)
            //{
            //    businessError = "The field Duration must be less than or equal to total hours ('To Date'-'From Date').";

            //    return businessError;
            //}

            //if (ProfessionalTrainingDateRangeCheck(obj.FromDate, obj.ToDate, obj.Id, obj.EmployeeId, strMode))
            //{
            //    return "Training period id not valid.";
            //}

            return string.Empty;
        }

        public string CheckBusinessLogicPROCER(PRM_EmpCertification obj)
        {
            string businessError = string.Empty;

            //var emp = PRMUnit.PersonalInfoRepository.GetByID(obj.EmployeeId, "EmployeeId");
            var emp = base.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId);
            if (emp != null)
            {
                if (Convert.ToInt32(obj.CertificationYear) <= Convert.ToDateTime(emp.DateofBirth).Year)
                {
                    businessError = "The field Year must be higher than the year of birth.";

                    return businessError;
                }
            }

            return string.Empty;
        }

        public string CheckBusinessLogicLICEN(PRM_EmpLicenseInfo obj)
        {
            string businessError = string.Empty;

            //if (obj.ExpireDate < obj.PermitDate)
            //{
            //    businessError = "The field To Expire Date must be greater than Permit Date.";

            //    return businessError;
            //}

            return string.Empty;
        }

        public string CheckBusinessLogicJOBSKL(PRM_EmpJobSkill obj)
        {
            string businessError = string.Empty;

            //if (obj.ExpireDate < obj.PermitDate)
            //{
            //    businessError = "The field To Expire Date must be greater than Permit Date.";

            //    return businessError;
            //}

            return string.Empty;
        }

        public string CheckBusinessLogicVisaPassport(PRM_EmpVisaPassportInfo obj)
        {
            string businessError = string.Empty;
            DateTime dateofBirth = DateTime.Now;
            if (obj.VisaPassportFor == "Own")
            {
                //dateofBirth = (from c in PRMUnit.PersonalInfoRepository.Fetch()
                //               where c.EmployeeId == obj.EmployeeId
                //               select c.DateofBirth).FirstOrDefault();

                var emp = PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId);
                if (emp != null)
                {
                    dateofBirth = Convert.ToDateTime(emp.DateofBirth);
                }
            }
            else
            {
                dateofBirth = (from c in PRMUnit.PersonalFamilyInformation.Fetch()
                               where c.EmployeeId == obj.EmployeeId && c.Id == obj.FamilyMemberId
                               select c.DateofBirth).FirstOrDefault();
            }

            //if (dateofBirth != null && (dateofBirth > obj.IssueDate))
            if (dateofBirth > obj.IssueDate)
            {
                businessError = "Issue Date must be greater than Person's Birth Date (" + dateofBirth.ToString("dd-MM-yyyy") + ")";
                return businessError;
            }

            if (obj.ExpireDate < obj.IssueDate)
            {
                businessError = "Expire Date must be greater than Issue Date.";

                return businessError;
            }

            string strMode = obj.Id == 0 ? "add" : "";
            if (VisaInfoDateRangeCheck(obj.Type, obj.IssueDate, obj.ExpireDate, obj.FamilyMemberId, obj.Id, obj.IssueCountryId, obj.EmployeeId, strMode))
            {
                return "Duplicate " + obj.Type + " date period.";
            }
            return string.Empty;
        }

        public string CheckDuplicateVisa(string visaPassNo, int countryId)
        {
            string businessError = string.Empty;
            var query = from e in PRMUnit.EmployeeVisaInfoRepository.Fetch()
                        where e.VisaPassportNo == visaPassNo && e.IssueCountryId == countryId
                        select e;
            var result = query.ToList().Count();

            if (result > 0)
            {
                businessError = "The Visa No. has already issued for selected country.";
                return businessError;
            }
            return string.Empty;
        }

        public string CheckDuplicateVisaUpdate(string visaPassNo, int countryId, int id) //should pass the table id, and check except this id this is exist or not
        {
            string businessError = string.Empty;
            var query = from e in PRMUnit.EmployeeVisaInfoRepository.Fetch()
                        where e.VisaPassportNo == visaPassNo && e.IssueCountryId == countryId && e.Id != id
                        select e;
            var result = query.ToList().Count();

            if (result > 0)
            {
                businessError = "The Visa No. has already issued for selected country.";
                return businessError;
            }
            return string.Empty;
        }

        public string CheckDuplicatePassport(string visaPassNo, int countryId)
        {
            string businessError = string.Empty;
            var query = from e in PRMUnit.EmployeeVisaInfoRepository.Fetch()
                        where e.VisaPassportNo == visaPassNo && e.IssueCountryId == countryId
                        select e;
            var result = query.ToList().Count();

            if (result > 0)
            {
                businessError = "The Passport No. has already issued for selected country.";
                return businessError;
            }
            return string.Empty;
        }

        public string CheckDuplicatePassportUpdate(string visaPassNo, int countryId, int id)
        {
            string businessError = string.Empty;
            var query = from e in PRMUnit.EmployeeVisaInfoRepository.Fetch()
                        where e.VisaPassportNo == visaPassNo && e.IssueCountryId == countryId && e.Id != id
                        select e;
            var result = query.ToList().Count();

            if (result > 0)
            {
                businessError = "The Passport No. has already issued for selected country.";
                return businessError;
            }
            return string.Empty;
        }

        public bool CheckDuplicateVisaPassport(string visaPassNo, int countryId)
        {


            var query = from e in PRMUnit.EmployeeVisaInfoRepository.Fetch()
                        where e.VisaPassportNo == visaPassNo && e.IssueCountryId == countryId
                        select e;

            var result = query.ToList().Count();

            return result > 0;

        }
        #endregion

        #region Workflow methods

        public IQueryable<vwAccademicQlfy> GetAllAccademicQlfnInfoByEmployeeID(int EmployeeId)
        {
            var list = from ac in PRMUnit.vwAccademicQlfyRepository.Fetch()
                       where ac.EmployeeId == EmployeeId
                       select ac;

            return list;
        }

        public IQueryable<JobExperience> GetAllJobExperienceInfoByEmployeeID(int EmployeeId)
        {
            var list = from ac in PRMUnit.JobExperienceInfoRepository.Fetch()
                       join ot in PRMUnit.OrganizationTypeRepository.Fetch() on ac.OrganizationTypeId equals ot.Id
                       join et in PRMUnit.EmploymentTypeRepository.Fetch() on ac.EmploymentTypeId equals et.Id
                       where ac.EmployeeId == EmployeeId
                       select new JobExperience
                       {
                           Id = ac.Id,
                           Organization1 = ac.Organization1,
                           Designation = ac.Designation,
                           EmploymentType = et.Name,
                           OrganizationType = ot.Name,
                           FromDate = ac.FromDate,
                           EndDate = ac.EndDate
                       };

            return list;
        }

        public bool JobExperienceDateRangeCheck(DateTime sDate, DateTime eDate, int experienceID, int employeeID, string strMode)
        {
            bool rv = false;

            if (strMode == "add")
            {
                rv = PRMUnit.JobExperienceInfoRepository.Fetch().Where(
                          x => (x.EmployeeId == employeeID) &&
                              (
                                  (x.FromDate <= sDate && sDate <= x.EndDate) ||
                                  (x.FromDate <= eDate && eDate <= x.EndDate) ||
                                  (sDate < x.FromDate && x.EndDate < eDate))
                              ).Any();
            }
            else
            {
                rv = PRMUnit.JobExperienceInfoRepository.Fetch().Where(
                          x => (x.EmployeeId == employeeID && experienceID != x.Id) &&
                              (
                                  (x.FromDate <= sDate && sDate <= x.EndDate)
                                  || (x.FromDate <= eDate && eDate <= x.EndDate)
                                  || (sDate < x.FromDate && x.EndDate < eDate))
                              ).Any();
            }

            return rv;
        }

        public IQueryable<ProfessionalTraining> GetAllProfessionalTrainingInfoByEmployeeID(int EmployeeId)
        {
            var list = from ac in PRMUnit.ProfessionalTrainingRepository.Fetch()
                       join c in PRMUnit.CountryRepository.Fetch() on ac.CountryId equals c.Id
                       where ac.EmployeeId == EmployeeId
                       select new ProfessionalTraining
                       {
                           Id = ac.Id,
                           TrainingTitle = ac.TrainingTitle,
                           OrganizedBy = ac.OrganizedBy,
                           TrainingType = ac.TrainingTypeId == null ? string.Empty : ac.PRM_TrainingType.Name,
                           FromDate = ac.FromDate,
                           ToDate = ac.ToDate,
                           Country = c.Name,
                           Duration = ac.Duration,
                           TrainingYear = ac.TrainingYear
                       };

            return list;
        }

        public bool ProfessionalTrainingDateRangeCheck(DateTime sDate, DateTime eDate, int trainingID, int employeeID, string strMode)
        {
            bool rv = false;

            if (strMode == "add")
            {
                rv = PRMUnit.ProfessionalTrainingRepository.Fetch().Where(
                          x => (x.EmployeeId == employeeID) &&
                              ((x.FromDate <= sDate && sDate <= x.ToDate)
                              || (x.FromDate <= eDate && eDate <= x.ToDate)
                               ||
                              (sDate < x.FromDate && x.ToDate < eDate))).Any();
            }
            else
            {
                rv = PRMUnit.ProfessionalTrainingRepository.Fetch().Where(
                          x => (x.EmployeeId == employeeID && trainingID != x.Id) &&
                              ((x.FromDate <= sDate && sDate <= x.ToDate)
                              || (x.FromDate <= eDate && eDate <= x.ToDate)
                               ||
                              (sDate < x.FromDate && x.ToDate < eDate))).Any();
            }

            return rv;
        }

        public bool VisaInfoDateRangeCheck(string type, DateTime sDate, DateTime eDate, int? memberID, int Id, int CountryId, int employeeID, string strMode)
        {
            bool rv = false;

            if (strMode == "add")
            {
                rv = PRMUnit.EmployeeVisaInfoRepository.Fetch().Where(
                          x => (x.Type == type && x.EmployeeId == employeeID && x.FamilyMemberId == memberID && x.IssueCountryId == CountryId) &&
                              ((x.IssueDate <= sDate && sDate <= x.ExpireDate)
                              || (x.IssueDate <= eDate && eDate <= x.ExpireDate)
                               ||
                              (sDate < x.IssueDate && x.ExpireDate < eDate))).Any();
            }
            else
            {
                rv = PRMUnit.EmployeeVisaInfoRepository.Fetch().Where(
                          x => (x.Type == type && x.EmployeeId == employeeID && x.FamilyMemberId == memberID && x.IssueCountryId == CountryId && x.Id != Id) &&
                              ((x.IssueDate <= sDate && sDate <= x.ExpireDate)
                              || (x.IssueDate <= eDate && eDate <= x.ExpireDate)
                               ||
                              (sDate < x.IssueDate && x.ExpireDate < eDate))).Any();
            }

            return rv;
        }

        public IQueryable<ProfessionalCertification> GetAllCertificationInfoByEmployeeID(int EmployeeId)
        {
            var list = from ac in PRMUnit.CertificationRepository.Fetch()
                       join cc in PRMUnit.CertificationCategoryRepository.Fetch() on ac.CertificationCatagoryId equals cc.Id
                       join c in PRMUnit.CountryRepository.Fetch() on ac.CountryId equals c.Id
                       join ci in PRMUnit.CertificationInstituteRepository.Fetch() on ac.CertificationInstituteId equals ci.Id
                       where ac.EmployeeId == EmployeeId
                       select new ProfessionalCertification
                       {
                           Id = ac.Id,
                           CertificationCatagory = cc.Name,
                           CertificationTitle = ac.CertificationTitle,
                           CertificationInstitute = ci.Name,
                           Country = c.Name,
                           CertificationYear = ac.CertificationYear,
                           Result = ac.Result
                       };

            return list;
        }

        public IQueryable<ProfessionalLicense> GetAllLicenseInfoByEmployeeID(int EmployeeId)
        {
            var list = from ac in PRMUnit.LicenseRepository.Fetch()
                       join cc in PRMUnit.LicenseTypeRepository.Fetch() on ac.LicensTypeId equals cc.Id
                       join c in PRMUnit.CountryRepository.Fetch() on ac.IssuingCountryId equals c.Id
                       where ac.EmployeeId == EmployeeId
                       select new ProfessionalLicense
                       {
                           Id = ac.Id,
                           LicenseType = cc.Name,
                           LicenseNo = ac.LicenseNo,
                           Institute = ac.IssuingInstitute,
                           Country = c.Name,
                           PermitDate = ac.PermitDate,
                           ExpireDate = ac.ExpireDate
                       };

            return list;
        }

        public IQueryable<EmpJobSkill> GetAllGetJobSkillInfoByEmployeeID(int EmployeeId)
        {
            var list = from ac in PRMUnit.JobSkillRepository.Fetch()
                       join sn in PRMUnit.JobSkillNameRepository.Fetch() on ac.JobSkillId equals sn.Id
                       join sl in PRMUnit.JobSkillLevelRepository.Fetch() on ac.JobSkillLevelId equals sl.Id
                       where ac.EmployeeId == EmployeeId
                       select new EmpJobSkill
                       {
                           Id = ac.Id,
                           SkillName = sn.Name,
                           EfficiencyLevel = sl.Name,
                           YearofExperience = ac.YearofExperience
                       };

            return list;
        }

        public IQueryable<EmpLeverage> GetAllLeverageByEmployeeID(int EmployeeId)
        {
            var list = from lev in PRMUnit.EmpLeverageRepository.Fetch()
                       where lev.EmployeeId == EmployeeId
                       select new EmpLeverage
                       {
                           Id = lev.Id,
                           ItemName = lev.ItemName,
                           ItemDescription = lev.ItemDescription,
                           IssueDate = lev.IssueDate,
                           ItemQnty = lev.ItemQnty
                       };

            return list;
        }

        public IQueryable<WealthStatement> GetAllWealthStatementByEmployeeID(int EmployeeId)
        {
            var list = from wealthstatment in PRMUnit.EmpWealthStatementRepository.Fetch()
                       join ast in PRMUnit.AssetTypeRepository.Fetch() on wealthstatment.AssetTypeId equals ast.Id
                       where wealthstatment.EmployeeId == EmployeeId
                       select new WealthStatement
                       {
                           Id = wealthstatment.Id,
                           AssetType = ast.Name,
                           AssetName = wealthstatment.AssetName,
                           AssetQuantity = wealthstatment.AssetQuantity,
                           AssetGainer = wealthstatment.AssetGainer,
                           AssetGainDate = wealthstatment.AssetGainDate


                       };

            return list;
        }
        public IQueryable<ServiceHistory> GetAllServiceHistoryByEmployeeID(int EmployeeId)
        {
            //get service history form employee Confirmation, Increment, Promotion and Demotion (tabelName PRM_EmpStatusChange)
            var list = (from empStatusChange in PRMUnit.EmpStatusChangeRepository.Fetch()

                            //from sH in PRMUnit.EmpServiceHistoryRepository.Fetch()
                            //join de in PRMUnit.DesignationRepository.Fetch() on sH.DesignationId equals de.Id

                            //join empType in PRMUnit.EmploymentTypeRepository.Fetch() on sH.EmploymentTypeId equals empType.Id into gEmpType
                            //from subEmpType in gEmpType.DefaultIfEmpty()

                            //join empProcess in PRMUnit.EmploymentProcessRepository.Fetch() on sH.EmploymentProcessId equals empProcess.Id into gEmpProcess
                            //from subEmpProcess in gEmpProcess.DefaultIfEmpty()

                            //join organogramLevel in PRMUnit.OrganogramLevelRepository.Fetch() on sH.OrganogramLevelId equals organogramLevel.Id into gOrganogramLevel
                            //from subOrganogramLevel in gOrganogramLevel.DefaultIfEmpty()

                            //join Office in PRMUnit.DisciplineRepository.Fetch() on sH.OfficeId equals Office.Id into gOffice
                            //from subOffice in gOffice.DefaultIfEmpty()

                            //join department in PRMUnit.DivisionRepository.Fetch() on sH.DepartmentId equals department.Id into gDepartment
                            //from subDepartment in gDepartment.DefaultIfEmpty()

                            //join section in PRMUnit.SectionRepository.Fetch() on sH.SectionId equals section.Id into gSection
                            //from subSection in gSection.DefaultIfEmpty()

                        where empStatusChange.EmployeeId == EmployeeId && empStatusChange.EffectiveDate <= DateTime.Now
                        select new ServiceHistory
                        {
                            Id = empStatusChange.Id,
                            Office = empStatusChange.PRM_Discipline1.Name,
                            Department = empStatusChange.PRM_Division1.Name,
                            Designation = empStatusChange.PRM_Designation1.Name,
                            EffectiveDate = empStatusChange.EffectiveDate,
                            Type = empStatusChange.Type
                        }).Union
                       (
                        from esh in PRMUnit.EmployeeServiceHistoryRepository.Fetch()
                        join emp in PRMUnit.EmploymentInfoRepository.Fetch() on esh.EmployeeId equals emp.Id
                        where esh.EmployeeId == EmployeeId
                        select new ServiceHistory
                        {
                            Id = esh.Id,
                            Office = emp.PRM_Discipline.Name,
                            Department = emp.PRM_Division.Name,
                            Designation = emp.PRM_Designation.Name,
                            EffectiveDate = esh.EffectiveDate == null ? DateTime.Now : esh.EffectiveDate,
                            Type = esh.Type
                        });

            return list;
        }
        #endregion

        #region Dropdownlist

        public IQueryable<PRM_District> PopulateDistrictByCountryID(int countryID)
        {
            var districtList = from d in PRMUnit.DistrictRepository.Fetch()
                               join
                                   cd in PRMUnit.CountryDivisionRepository.Fetch() on d.DivisionId equals cd.Id
                               where cd.CountryId == countryID
                               select d;

            return districtList;
        }

        #endregion

        #region Inner class

        public class JobExperience
        {
            public int Id { get; set; }
            public string Organization1 { set; get; }
            public string Designation { set; get; }
            public string EmploymentType { set; get; }
            public string OrganizationType { set; get; }
            public DateTime FromDate { set; get; }
            public DateTime EndDate { set; get; }
            public decimal Duration
            {
                get
                {
                    var d = (EndDate - FromDate).TotalDays / 365;

                    return Convert.ToDecimal(Math.Round(d, 2));
                }
            }
        }

        public class ProfessionalTraining
        {
            public int Id { get; set; }
            public string TrainingTitle { set; get; }
            public string OrganizedBy { set; get; }
            public string TrainingType { set; get; }
            public DateTime? FromDate { set; get; }
            public DateTime? ToDate { set; get; }
            public string TrainingYear { set; get; }
            public string Country { set; get; }
            public decimal? Duration { set; get; }
        }

        public class ProfessionalCertification
        {
            public int Id { get; set; }
            public string CertificationCatagory { set; get; }
            public string CertificationTitle { set; get; }
            public string CertificationInstitute { set; get; }
            public string Country { set; get; }
            public string CertificationYear { set; get; }
            public string Result { set; get; }
        }

        public class ProfessionalLicense
        {
            public int Id { get; set; }
            public string LicenseType { set; get; }
            public string LicenseNo { set; get; }
            public string Institute { set; get; }
            public string Country { set; get; }
            public DateTime? PermitDate { set; get; }
            public DateTime? ExpireDate { set; get; }
        }

        public class EmpJobSkill
        {
            public int Id { get; set; }
            public string SkillName { set; get; }
            public string EfficiencyLevel { set; get; }
            public decimal? YearofExperience { set; get; }
        }

        public class EmpLeverage
        {
            public int Id { get; set; }
            public string ItemName { set; get; }
            public string ItemDescription { set; get; }
            public DateTime? IssueDate { set; get; }
            public int? ItemQnty { set; get; }
        }


        public class WealthStatement
        {
            public int Id { get; set; }
            public string AssetType { get; set; }
            public string AssetName { set; get; }
            public int? AssetQuantity { set; get; }
            public DateTime? AssetGainDate { set; get; }
            public string AssetGainer { set; get; }
        }

        public class ServiceHistory
        {
            public int Id { get; set; }
            public string Designation { get; set; }
            public string EmployeeCategory { set; get; }
            public string EmploymentProcess { set; get; }
            public DateTime? EffectiveDate { set; get; }
            public string SalaryScale { set; get; }
            public string OnganogramLevelName { get; set; }
            public string Office { get; set; }
            public string Department { get; set; }
            public string Section { get; set; }
            public string Type { get; set; }
        }


        public class EmpDateofBirth
        {
            public DateTime DateofBirth { set; get; }
        }

        #endregion
    }
}
