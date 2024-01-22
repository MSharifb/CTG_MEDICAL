using System;
using System.Collections.Generic;
using System.Linq;
using BEPZA_MEDICAL.DAL.AMS;
using BEPZA_MEDICAL.DAL.Infrastructure;
using System.Net.Mail;
using System.Net;


namespace BEPZA_MEDICAL.Domain.AMS
{
    public class AMSCommonService
    {
        #region Fields

        AMS_UnitOfWork _amsUnit;

        #endregion

        #region Ctor

        public AMSCommonService(AMS_UnitOfWork unitOfWork)
        {
            _amsUnit = unitOfWork;
        }

        #endregion

        #region Properties

        public AMS_UnitOfWork AMSUnit { get { return _amsUnit; } }

        #endregion

        #region Workflow method

        public string GetNewAnsarBEPZAID()
        {
            string BEPZAID = "0001";
            try
            {
                var lastBEPZAID = AMSUnit.AnsarEmpInfoRepository.GetAll().OrderByDescending(q => Convert.ToInt32(q.BEPZAId)).FirstOrDefault().BEPZAId;
                BEPZAID = (Convert.ToInt32(lastBEPZAID) + 1).ToString("0000");
            }
            catch { }
            return BEPZAID;
        }

        public AMS_AnsarPhoto GetAnsarPhoto(int employeeID, bool isPhoto)
        {
            AMS_AnsarPhoto entity = (from c in AMSUnit.AnsarPhotographRepository.Fetch()
                                   where c.EmployeeId == employeeID && c.IsPhoto == isPhoto
                                   select c).FirstOrDefault();
            return entity;
        }

        public bool IsPhotoExist(int id, bool isPhoto)
        {
            AMS_AnsarPhoto toDisplay = null;
            try
            {
                toDisplay = GetAnsarPhoto(id, isPhoto);
            }
            catch { }

            if (toDisplay != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IQueryable<vwAMSAnsarEducationalQlfy> GetAllAccademicQlfnInfoByAnsarEmpID(int EmployeeId)
        {
            var list = from ac in AMSUnit.vwAnsarEducationalQlfyRepository.Fetch()
                       where ac.EmployeeId == EmployeeId
                       select ac;

            return list;
        }

        public IQueryable<JobExperience> GetAllJobExperienceInfoByAnsarEmpID(int EmployeeId)
        {
            var list = from ac in AMSUnit.AnsarExperienceRepository.Fetch()
                       join ot in AMSUnit.OrganizationTypeRepository.Fetch() on ac.OrganizationTypeId equals ot.Id
                       where ac.EmployeeId == EmployeeId
                       select new JobExperience
                       {
                           Id = ac.Id,
                           Organization1 = ac.Organization1,
                           OrganizationType = ot.Name,
                           FromDate = ac.FromDate,
                           EndDate = ac.EndDate
                       };

            return list;
        }

        #endregion

        #region inner class

        public class JobExperience
        {
            public int Id { get; set; }
            public string Organization1 { set; get; }
            public string Designation { set; get; }
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

        #endregion

        #region Business Logic Validation

        public string CheckBusinessLogic(AMS_AnsarPersonalInfo obj)
        {
            string businessError = string.Empty;

            //DateTime joiningDate = this.PRMUnit.EmploymentInfoRepository.GetByID(obj.EmployeeId).DateofJoining;
            var emp = this.AMSUnit.AnsarEmpInfoRepository.GetByID(obj.EmployeeId);

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

        public string CheckBusinessLogicJOBEXP(AMS_AnsarExperience obj, string strMode)
        {
            string businessError = string.Empty;

            if (obj.EndDate < obj.FromDate)
            {
                businessError = "The field To Date must be greater than From Date.";

                return businessError;
            }

            if (JobExperienceDateRangeCheck(obj.FromDate, obj.EndDate, obj.Id, obj.EmployeeId, strMode))
            {
                return "Experience period is not valid.";
            }

            return string.Empty;
        }

        public bool JobExperienceDateRangeCheck(DateTime sDate, DateTime eDate, int experienceID, int employeeID, string strMode)
        {
            bool rv = false;

            if (strMode == "add")
            {
                rv = AMSUnit.AnsarExperienceRepository.Fetch().Where(
                          x => (x.EmployeeId == employeeID) &&
                              (
                                  (x.FromDate <= sDate && sDate <= x.EndDate) ||
                                  (x.FromDate <= eDate && eDate <= x.EndDate) ||
                                  (sDate < x.FromDate && x.EndDate < eDate))
                              ).Any();
            }
            else
            {
                rv = AMSUnit.AnsarExperienceRepository.Fetch().Where(
                          x => (x.EmployeeId == employeeID && experienceID != x.Id) &&
                              (
                                  (x.FromDate <= sDate && sDate <= x.EndDate)
                                  || (x.FromDate <= eDate && eDate <= x.EndDate)
                                  || (sDate < x.FromDate && x.EndDate < eDate))
                              ).Any();
            }

            return rv;
        }

        #endregion

    }
}
