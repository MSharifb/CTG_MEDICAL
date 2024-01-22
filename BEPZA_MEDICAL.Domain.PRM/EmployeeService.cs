using System;
using System.Collections.Generic;
using System.Linq;
using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.PGM;

using BEPZA_MEDICAL.Utility;



namespace BEPZA_MEDICAL.Domain.PRM
{
    public class EmployeeService : PRMCommonSevice
    {
        #region Fields

        #endregion

        #region Constructor

        public EmployeeService(PRM_UnitOfWork uow)
            : base(uow)
        {
            
        }

        #endregion

        #region Workflow methods

        public int GetCount(string filterExpression)
        {
            var query = from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch() select empInfo;


            if (!String.IsNullOrWhiteSpace(filterExpression))
            {
                if (filterExpression.Contains("like"))
                {
                    string[] srcString = filterExpression.Split(' ');

                    string whereExpression = GetWhereExpression(filterExpression);
                    query = query.Where(whereExpression);

                    return query.Count();
                }
                else
                {
                    return query.Where(filterExpression).Count();
                }
            }
            else
                return query.Count();
        }

        public IList<EmployeeSearch> GetPaged(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount)
        {
            return new List<EmployeeSearch>();
        }

        // get all employee
        public IList<EmployeeSearch> GetPaged(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount,

            string EmpId,
            string EmpName,
            int? DesigName,
            int? EmpTypeId,
            int? DivisionName,
            int? JobLocName,
            int? GradeName,
            int? StaffCategoryId,
            //int? ResourceLevelId,
            int? OrganogramLevelId,
            int ZoneInfoId,
            int EmpStatus,
            out int totalRecords
            //string LoginEmpId = ""
            )
        {

            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         where

                         (EmpId == null || EmpId == "" || empInfo.EmpID == EmpId)
                         && (EmpName == null || EmpName == "" || empInfo.FullName.Contains(EmpName))
                         && (DesigName == null || DesigName == 0 || empInfo.DesignationId == DesigName)
                         && (EmpTypeId == null || EmpTypeId == 0 || empInfo.EmploymentTypeId == EmpTypeId)
                         && (DivisionName == null || DivisionName == 0 || empInfo.DivisionId == DivisionName)
                         && (JobLocName == null || JobLocName == 0 || empInfo.JobLocationId == JobLocName)
                         && (GradeName == null || GradeName == 0 || empInfo.JobGradeId == GradeName)
                         && (StaffCategoryId == null || StaffCategoryId == 0 || empInfo.StaffCategoryId == StaffCategoryId)
                             //&& (ResourceLevelId == null || ResourceLevelId == 0 || empInfo.ResourceLevelId == ResourceLevelId)
                         && (OrganogramLevelId == null || OrganogramLevelId == 0 || empInfo.OrganogramLevelId == OrganogramLevelId)

                         && (EmpStatus == 0 || empInfo.EmploymentStatusId == EmpStatus)
                         //&& (LoginEmpId == null || LoginEmpId == "" || empInfo.EmpID == LoginEmpId)
                         //&& (empInfo.ZoneInfoId == ZoneInfoId)

                         orderby empInfo.Id

                         select new EmployeeSearch()
                            {
                                ID = empInfo.Id,
                                EmpId = empInfo.EmpID,
                                GradeId = empInfo.JobGradeId,
                                GradeName = empInfo.PRM_JobGrade.GradeName,
                                DateOfJoining = empInfo.DateofJoining,
                                DateOfConfirmation = empInfo.DateofConfirmation,
                                DesignationId = empInfo.DesignationId,
                                DesigName = empInfo.PRM_Designation.Name,
                                DivisionId = empInfo.DivisionId,
                                DivisionName = empInfo.PRM_Division.Name,
                                EmpName = empInfo.FullName,
                                EmpTypeId = empInfo.EmploymentTypeId,
                                EmpTypeName = empInfo.PRM_EmploymentType.Name,
                                JobLocationId = empInfo.JobLocationId,
                                JobLocName = empInfo.PRM_JobLocation.Name,
                                //ResourceLevelId = empInfo.ResourceLevelId,
                                OrganogramLevelId = empInfo.OrganogramLevelId,
                                StaffCategoryId = empInfo.StaffCategoryId,
                                DateOfInactive = empInfo.DateofInactive,
                                IsContractual = empInfo.IsContractual,
                                ZoneInfoId = empInfo.ZoneInfoId,
                            }).Distinct();

            if (ZoneInfoId > 0)
            {
                query = query.Where(q => q.ZoneInfoId == ZoneInfoId);
            }

            if (filterExpression != "") query = query.Where(GetWhereExpression(filterExpression));
            var result = query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            totalRecords = query.ToList().Count;

            return result;
        }

        // get all employee
        public IList<EmployeeSearch> GetPagedForPGM(string filterExpression, string sortExpression, string sortDirection
            , int pageIndex, int pageSize, int pagesCount, string EmpId, string EmpName, int? DesigName, int? EmpTypeId
            , int? DivisionName, int? JobLocName, int? GradeName, int? StaffCategoryId, int? OrganogramLevelId
            , int ZoneInfoId, int EmpStatus, out int totalRecords, int? salaryWithdrawZoneId
            )
        {

            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         where

                         (EmpId == null || EmpId == "" || empInfo.EmpID == EmpId)
                         && (EmpName == null || EmpName == "" || empInfo.FullName.Contains(EmpName))
                         && (DesigName == null || DesigName == 0 || empInfo.DesignationId == DesigName)
                         && (EmpTypeId == null || EmpTypeId == 0 || empInfo.EmploymentTypeId == EmpTypeId)
                         && (DivisionName == null || DivisionName == 0 || empInfo.DivisionId == DivisionName)
                         && (JobLocName == null || JobLocName == 0 || empInfo.JobLocationId == JobLocName)
                         && (GradeName == null || GradeName == 0 || empInfo.JobGradeId == GradeName)
                         && (StaffCategoryId == null || StaffCategoryId == 0 || empInfo.StaffCategoryId == StaffCategoryId)
                         && (OrganogramLevelId == null || OrganogramLevelId == 0 || empInfo.OrganogramLevelId == OrganogramLevelId)
                         && (EmpStatus == 0 || empInfo.EmploymentStatusId == EmpStatus)
                         && (salaryWithdrawZoneId == null || salaryWithdrawZoneId == 0 || empInfo.SalaryWithdrawFromZoneId == salaryWithdrawZoneId)

                         select new EmployeeSearch()
                         {
                             ID = empInfo.Id,
                             EmpId = empInfo.EmpID,
                             GradeId = empInfo.JobGradeId,
                             GradeName = empInfo.PRM_JobGrade.GradeName,
                             DateOfJoining = empInfo.DateofJoining,
                             DateOfConfirmation = empInfo.DateofConfirmation,
                             DesignationId = empInfo.DesignationId,
                             DesigName = empInfo.PRM_Designation.Name,
                             DivisionId = empInfo.DivisionId,
                             DivisionName = empInfo.PRM_Division.Name,
                             EmpName = empInfo.FullName,
                             EmpTypeId = empInfo.EmploymentTypeId,
                             EmpTypeName = empInfo.PRM_EmploymentType.Name,
                             JobLocationId = empInfo.JobLocationId,
                             JobLocName = empInfo.PRM_JobLocation.Name,
                             OrganogramLevelId = empInfo.OrganogramLevelId,
                             StaffCategoryId = empInfo.StaffCategoryId,
                             StaffCatName = empInfo.PRM_StaffCategory == null ? String.Empty : empInfo.PRM_StaffCategory.Name,
                             DateOfInactive = empInfo.DateofInactive,
                             IsContractual = empInfo.IsContractual,
                             ZoneInfoId = empInfo.ZoneInfoId,
                             EmpStatusName = empInfo.PRM_EmploymentStatus == null ? String.Empty : empInfo.PRM_EmploymentStatus.Name,
                             IsBonusEligible = empInfo.IsBonusEligible,
                             IsOvertimeEligible = empInfo.IsOvertimeEligible,
                             IsRefreshmentEligible = empInfo.IsRefreshmentEligible,
                             SortingOrder = empInfo.PRM_Designation.SortingOrder
                         }).Distinct();


            if (filterExpression != "") query = query.Where(GetWhereExpression(filterExpression));

            totalRecords = query.ToList().Count;

            if (!String.IsNullOrEmpty(sortExpression))
            {
                return query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return query.OrderBy(q=> q.SortingOrder).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }


        // get all employee for project budget
        public IList<EmployeeSearch> GetPagedPB(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount,

            string EmpId,
            string EmpName,
            int? DesigName,
            int? EmpTypeId,
            int? DivisionName,
            int? JobLocName,
            int? GradeName,
            int? StaffCategoryId,
            //int? ResourceLevelId,
            int EmpStatus,
            out int totalRecords
            )
        {

            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         //join ri in PRMUnit.vwHumanResourceInfoRepository.Fetch()
                         //on empInfo.Id equals ri.EmployeeId
                         where empInfo.DateofInactive == null &&
                         (EmpId == null || EmpId == "" || empInfo.EmpID == EmpId)
                         && (EmpName == null || EmpName == "" || empInfo.FullName.Contains(EmpName))
                         && (DesigName == null || DesigName == 0 || empInfo.DesignationId == DesigName)
                         && (EmpTypeId == null || EmpTypeId == 0 || empInfo.EmploymentTypeId == EmpTypeId)
                         && (DivisionName == null || DivisionName == 0 || empInfo.DivisionId == DivisionName)
                         && (JobLocName == null || JobLocName == 0 || empInfo.JobLocationId == JobLocName)
                         && (GradeName == null || GradeName == 0 || empInfo.JobGradeId == GradeName)
                         && (StaffCategoryId == null || StaffCategoryId == 0 || empInfo.StaffCategoryId == StaffCategoryId)
                             //&& (ResourceLevelId == null || ResourceLevelId == 0 || empInfo.ResourceLevelId == ResourceLevelId)
                         && (EmpStatus == 0 || empInfo.EmploymentStatusId == EmpStatus)

                         orderby empInfo.Id
                         select new EmployeeSearch()
                         {
                             ID = empInfo.Id,
                             EmpId = empInfo.EmpID,
                             GradeId = empInfo.JobGradeId,
                             GradeName = empInfo.PRM_JobGrade.GradeName,
                             DateOfJoining = empInfo.DateofJoining,
                             DateOfConfirmation = empInfo.DateofConfirmation,
                             DesignationId = empInfo.DesignationId,
                             DesigName = empInfo.PRM_Designation.Name,
                             DivisionId = empInfo.DivisionId,
                             DivisionName = empInfo.PRM_Division.Name,
                             EmpName = empInfo.FullName,
                             EmpTypeId = empInfo.EmploymentTypeId,
                             EmpTypeName = empInfo.PRM_EmploymentType.Name,
                             JobLocationId = empInfo.JobLocationId,
                             JobLocName = empInfo.PRM_JobLocation.Name,
                             //ResourceLevelId = empInfo.ResourceLevelId,
                             StaffCategoryId = empInfo.StaffCategoryId,
                             DateOfInactive = empInfo.DateofInactive,
                             IsContractual = empInfo.IsContractual,
                             ResourceRate = 0
                         }).Distinct();

            if (filterExpression != "") query = query.Where(GetWhereExpression(filterExpression));
            var result = query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();

            totalRecords = query.ToList().Count;

            return result;
        }

        // Get Active/Inactive/All Employee
        public IList<EmployeeSearch> GetActivePaged(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount,
            string EmpId,
            string EmpName,
            int? DesigName,
            int? EmpTypeId,
            int? DivisionName,
            int? JobLocName,
            int? GradeName,
            int? StaffCategoryId,
            //int? ResourceLevelId,
            int EmpStatus,
            int IsActive,
            out int totalRecords
            )
        {

            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         where

                         (EmpId == null || EmpId == "" || empInfo.EmpID == EmpId)
                         && (EmpName == null || EmpName == "" || empInfo.FullName.Contains(EmpName))
                         && (DesigName == null || DesigName == 0 || empInfo.DesignationId == DesigName)
                         && (EmpTypeId == null || EmpTypeId == 0 || empInfo.EmploymentTypeId == EmpTypeId)
                         && (DivisionName == null || DivisionName == 0 || empInfo.DivisionId == DivisionName)
                         && (JobLocName == null || JobLocName == 0 || empInfo.JobLocationId == JobLocName)
                         && (GradeName == null || GradeName == 0 || empInfo.PRM_EmpSalary.PRM_JobGrade.Id == GradeName)
                         && (StaffCategoryId == null || StaffCategoryId == 0 || empInfo.StaffCategoryId == StaffCategoryId)
                             // && (ResourceLevelId == null || ResourceLevelId == 0 || empInfo.ResourceLevelId == ResourceLevelId)
                         && (EmpStatus == 0 || empInfo.EmploymentStatusId == EmpStatus)

                         orderby empInfo.Id

                         select new EmployeeSearch()
                         {
                             ID = empInfo.Id,
                             EmpId = empInfo.EmpID,
                             GradeId = empInfo.PRM_EmpSalary.GradeId,
                             GradeName = empInfo.PRM_EmpSalary.PRM_JobGrade.GradeName,
                             DateOfJoining = empInfo.DateofJoining,
                             DateOfConfirmation = empInfo.DateofConfirmation,
                             DesignationId = empInfo.DesignationId,
                             DesigName = empInfo.PRM_Designation.Name,
                             DivisionId = empInfo.DivisionId,
                             DivisionName = empInfo.PRM_Division.Name,
                             EmpName = empInfo.FullName,
                             EmpTypeId = empInfo.EmploymentTypeId,
                             EmpTypeName = empInfo.PRM_EmploymentType.Name,
                             JobLocationId = empInfo.JobLocationId,
                             JobLocName = empInfo.PRM_JobLocation.Name,
                             //ResourceLevelId = empInfo.ResourceLevelId,
                             StaffCategoryId = empInfo.StaffCategoryId,
                             DateOfInactive = empInfo.DateofInactive,
                             IsContractual = empInfo.IsContractual
                         }).Distinct();

            if (IsActive == 1) query = query.Where(q => q.DateOfInactive == null);
            else if (IsActive == 2) query = query.Where(q => q.DateOfInactive != null);

            if (filterExpression != "") query = query.Where(GetWhereExpression(filterExpression));

            var result = query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            totalRecords = query.ToList().Count;

            return result;
        }


        // Get Active/Inactive/All Employee. Specially for Final Settlement
        public IList<EmployeeSearch> GetInactivePaged(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount,
            string EmpId,
            string EmpName,
            int? DesigName,
            int? EmpTypeId,
            int? DivisionName,
            int? JobLocName,
            int? GradeName,
            int? StaffCategoryId,
            //int? ResourceLevelId,
            int EmpStatus,
            int IsActive,
            out int totalRecords
            )
        {

            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         where

                         (EmpId == null || EmpId == "" || empInfo.EmpID == EmpId)
                         && (EmpName == null || EmpName == "" || empInfo.FullName.Contains(EmpName))
                         && (DesigName == null || DesigName == 0 || empInfo.DesignationId == DesigName)
                         && (EmpTypeId == null || EmpTypeId == 0 || empInfo.EmploymentTypeId == EmpTypeId)
                         && (DivisionName == null || DivisionName == 0 || empInfo.DivisionId == DivisionName)
                         && (JobLocName == null || JobLocName == 0 || empInfo.JobLocationId == JobLocName)
                         && (GradeName == null || GradeName == 0 || empInfo.PRM_EmpSalary.PRM_JobGrade.Id == GradeName)
                         && (StaffCategoryId == null || StaffCategoryId == 0 || empInfo.StaffCategoryId == StaffCategoryId)
                             //&& (ResourceLevelId == null || ResourceLevelId == 0 || empInfo.ResourceLevelId == ResourceLevelId)
                         && (EmpStatus == 0 || empInfo.EmploymentStatusId == EmpStatus)

                         orderby empInfo.Id

                         select new EmployeeSearch()
                         {
                             ID = empInfo.Id,
                             EmpId = empInfo.EmpID,
                             GradeId = empInfo.PRM_EmpSalary.GradeId,
                             GradeName = empInfo.PRM_EmpSalary.PRM_JobGrade.GradeName,
                             DateOfJoining = empInfo.DateofJoining,
                             DateOfConfirmation = empInfo.DateofConfirmation,
                             DesignationId = empInfo.DesignationId,
                             DesigName = empInfo.PRM_Designation.Name,
                             DivisionId = empInfo.DivisionId,
                             DivisionName = empInfo.PRM_Division.Name,
                             EmpName = empInfo.FullName,
                             EmpTypeId = empInfo.EmploymentTypeId,
                             EmpTypeName = empInfo.PRM_EmploymentType.Name,
                             JobLocationId = empInfo.JobLocationId,
                             JobLocName = empInfo.PRM_JobLocation.Name,
                             //ResourceLevelId = empInfo.ResourceLevelId,
                             StaffCategoryId = empInfo.StaffCategoryId,
                             DateOfInactive = empInfo.DateofInactive,
                             IsContractual = empInfo.IsContractual
                         }).Distinct();


            if (IsActive == 1)
            {
                query = query.Where(q => q.DateOfInactive == null);
            }
            else if (IsActive == 2)
            {
                query = query.Where(q => q.DateOfInactive != null);
            }

            //else if (IsActive == 2) query = query.Where(q => q.DateOfInactive != null);
            //var sl = (from emp in query where !_puow.FinalSettlement.GetAll().Any(fsl => fsl.EmployeeId == emp.ID && emp.DateOfInactive!=null) select emp).ToList();

            //var orphans = (from c in context.Instances
            //               orderby c.Title
            //               where !(from o in context.CategoryInstances
            //                       select o.InstanceID)
            //               .Contains(c.InstanceID)
            //               select c).Skip(PageIndex * PageSize).Take(PageSize);

            //var finalS = _puow.FinalSettlement.GetAll().ToList(); 

            if (filterExpression != "") query = query.Where(GetWhereExpression(filterExpression));

            var result = query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            totalRecords = query.ToList().Count;

            return result;
        }

        // get employee by active/inactive status
        public IList<EmployeeSearch> GetPaged(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount,
            string EmpId,
            string EmpName,
            int? DesigName,
            int? EmpTypeId,
            int? DivisionName,
            int? JobLocName,
            int? GradeName,
            int? StaffCategoryId,
            int EmpStatus,
            int IsActive,
            int ZoneInfoId,
            out int totalRecords
            )
        {
            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         where
                         (EmpId == null || EmpId == "" || empInfo.EmpID == EmpId)
                         && (EmpName == null || EmpName == "" || empInfo.FullName.Contains(EmpName))
                         && (DesigName == null || DesigName == 0 || empInfo.DesignationId == DesigName)
                         && (EmpTypeId == null || EmpTypeId == 0 || empInfo.EmploymentTypeId == EmpTypeId)
                         && (DivisionName == null || DivisionName == 0 || empInfo.DivisionId == DivisionName)
                         && (JobLocName == null || JobLocName == 0 || empInfo.JobLocationId == JobLocName)
                         && (GradeName == null || GradeName == 0 || empInfo.JobGradeId == GradeName)
                         && (StaffCategoryId == null || StaffCategoryId == 0 || empInfo.StaffCategoryId == StaffCategoryId)
                         && (EmpStatus == 0 || empInfo.EmploymentStatusId == EmpStatus)
                         && (ZoneInfoId == 0 || empInfo.ZoneInfoId == ZoneInfoId)
                         orderby empInfo.Id

                         select new EmployeeSearch()
                         {
                             ZoneInfoId = empInfo.ZoneInfoId,
                             ZoneName = empInfo.PRM_ZoneInfo.ZoneName,
                             ID = empInfo.Id,
                             EmpId = empInfo.EmpID,
                             GradeId = empInfo.JobGradeId,
                             GradeName = empInfo.PRM_JobGrade.GradeName,
                             DateOfJoining = empInfo.DateofJoining,
                             DateOfConfirmation = empInfo.DateofConfirmation,
                             DesignationId = empInfo.DesignationId,
                             DesigName = empInfo.PRM_Designation.Name,
                             DivisionId = empInfo.DivisionId,
                             DivisionName = empInfo.PRM_Division.Name,
                             EmpName = empInfo.FullName,
                             EmpTypeId = empInfo.EmploymentTypeId,
                             EmpTypeName = empInfo.PRM_EmploymentType.Name,
                             JobLocationId = empInfo.JobLocationId,
                             JobLocName = empInfo.PRM_JobLocation.Name,
                             StaffCategoryId = empInfo.StaffCategoryId,
                             DateOfInactive = empInfo.DateofInactive,
                             IsContractual = empInfo.IsContractual
                         }).Distinct();

            //query = query.Where(q => q.ZoneInfoId == LoggedUserZoneInfoId);
            if (IsActive == 1) query = query.Where(q => q.DateOfInactive == null);
            else if (IsActive == 0) query = query.Where(q => q.DateOfInactive != null);

            if (filterExpression != "") query = query.Where(GetWhereExpression(filterExpression));

            var result = query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            totalRecords = query.ToList().Count;

            return result;
        }


        public PRM_EmploymentInfo GetEmployeeInfoByEmployeeId(string empId)
        {
            var query = from emp in base.PRMUnit.EmploymentInfoRepository.Fetch()
                        where emp.EmpID == empId
                        select emp;

            return query.FirstOrDefault();
        }

        public IList<PRM_EmpContractInfo> GetEmpContractByEmpId(int empId)
        {
            var query = from emp in base.PRMUnit.EmploymentContractInfoRepository.Fetch()
                        where emp.EmpoyeeId == empId
                        select emp;

            return query.ToList();
        }

        public bool EmpInitialDuplicateCheck(string initial, string empId)
        {
            var query = from e in PRMUnit.EmploymentInfoRepository.Fetch()
                        where e.EmployeeInitial.Trim().ToLower() == initial.Trim().ToLower()
                        && e.EmpID != empId
                        select e;

            var result = query.ToList().Count();

            return result > 0;
        }

        public PRM_EmpPhoto GetEmployeePhoto(int employeeID, bool isPhoto)
        {
            PRM_EmpPhoto entity = (from c in base.PRMUnit.EmployeePhotoGraphRepository.Fetch()
                                   where c.EmployeeId == employeeID && c.IsPhoto == isPhoto
                                   select c).FirstOrDefault();
            return entity;
        }

        #endregion

        #region Utils

        private string GetWhereExpression(string filterExpression)
        {
            string[] srcString = filterExpression.Split(' ');
            List<string> al = srcString.ToList<string>();
            if (al.Count > 3)
                al.RemoveRange(0, 4);
            string s = string.Join(" ", al.ToArray()); ;
            return s;
        }

        #endregion

        public string GetNewEmployeeID()
        {
            string EmployeeID = "0001";
            try
            {
                var lastEmpID = base.PRMUnit.EmploymentInfoRepository.GetAll().OrderByDescending(q => Convert.ToInt32(q.EmpID)).FirstOrDefault().EmpID;
                EmployeeID = (Convert.ToInt32(lastEmpID) + 1).ToString("0000");
            }
            catch { }
            return EmployeeID;
        }

        public string EmployeeSanctionedPostBusinessLogicValidation(int organogramLevelId, int designationId)
        {

            var empPost = (from empInfo in base.PRMUnit.EmploymentInfoRepository.GetAll()
                           where empInfo.OrganogramLevelId == organogramLevelId && empInfo.DesignationId == designationId
                           select empInfo).ToList();
            var totalEmpPost = empPost == null ? 0 : empPost.Count;

            var OrgSanctionedPost = (from org in base.PRMUnit.OrganizationalSetupManpowerInfoRepository.GetAll()
                                     where (org.OrganogramLevelId == organogramLevelId && org.DesignationId == designationId && org.ActiveStatus == true)
                                     select org).FirstOrDefault();
            var OrgTotalSanctionedPost = OrgSanctionedPost == null ? 0 : OrgSanctionedPost.SanctionedPost;

            if (totalEmpPost >= OrgTotalSanctionedPost)
            {
                return "Already filled up this position.";
            }
            return string.Empty;
        }

        public OrganogramSearch GetEmployeeDeptOfficeSecInfoByOrgogramId(int organogramLevelId)
        {
            var query = (from m in base.PRMUnit.OrganogramLevelRepository.GetAll()
                         where m.Id == organogramLevelId
                         select m).ToList();

            do
            {
                var item = (from OrgL in base.PRMUnit.OrganogramLevelRepository.GetAll()
                            where OrgL.Id == query.ToList().Last().ParentId
                            select OrgL).FirstOrDefault();
                if (item != null)
                {
                    query.Add(item);
                }

            } while (query.ToList().Last().ParentId != 0);

            var obj = new OrganogramSearch();
            var result = (from ogLParent in query
                          join ogLType in base.PRMUnit.OrganogramTypeRepository.GetAll() on ogLParent.OrganogramTypeId equals ogLType.Id
                          join ogLTypeMapp in base.PRMUnit.OrganogramTypeMappingRepository.GetAll() on ogLType.Id equals ogLTypeMapp.OrganogramTypeId
                          select new
                          {
                              Id = ogLParent.Id,
                              LevelName = ogLParent.LevelName,
                              ParentId = ogLParent.ParentId,
                              OrganogramTypeId = ogLParent.OrganogramTypeId,
                              OrganogramTypeName = ogLTypeMapp.OrganogramTypeName
                          }).ToList();

            foreach (var item in result)
            {
                if (item.OrganogramTypeName.Equals(PRMEnum.EmployeeOrganogram.Department.ToString()))// Department
                {
                    obj.DepartmentName = item.LevelName;
                }
                else if (item.OrganogramTypeName.Equals(PRMEnum.EmployeeOrganogram.Office.ToString()))//office
                {
                    obj.OfficeName = item.LevelName;
                }
                else if (item.OrganogramTypeName.Equals(PRMEnum.EmployeeOrganogram.Section.ToString())) // section 
                {
                    obj.SectionName = item.LevelName;
                }
                else if (item.OrganogramTypeName.Equals(PRMEnum.EmployeeOrganogram.SubSection.ToString())) // Sub-Section
                {
                    obj.SubSectionName = item.LevelName;
                }
            }

            obj.OrganogramLevelId = organogramLevelId;
            return obj;
        }

        public IList<OrganogramSearch2> GetEmployeeDeptOfficeSecInfoByOrgogramId2(int organogramLevelId)
        {
            var query = (from m in base.PRMUnit.OrganogramLevelRepository.GetAll()
                         where m.Id == organogramLevelId
                         select m).ToList();
            do
            {
                var item = (from OrgL in base.PRMUnit.OrganogramLevelRepository.GetAll()
                            where OrgL.Id == query.ToList().Last().ParentId
                            select OrgL).FirstOrDefault();

                if (item != null)
                {
                    query.Add(item);
                }
            } while (query.ToList().Last().ParentId != 0);

            var result = (from ogLParent in query
                          join ogLType in base.PRMUnit.OrganogramTypeRepository.GetAll() on ogLParent.OrganogramTypeId equals ogLType.Id
                          join orgTypeMap in base.PRMUnit.OrganogramTypeMappingRepository.GetAll() on ogLType.Id equals orgTypeMap.OrganogramTypeId
                          select new OrganogramSearch2
                          {
                              OrganogramLevelId = ogLParent.Id,
                              OrganogramLevelName = ogLParent.LevelName,
                              OrganogramTypeId = ogLType.Id,
                              OrganogramTypeName = ogLType.Name,
                              TableIdName = orgTypeMap.TableIdName,
                              SortOrder = ogLType.SortOrder
                          }).OrderBy(t => t.SortOrder);

            return result.ToList();
        }

        //public string GetOrganogramHierarchyLabel(int organogramLevelId)
        //{
        //    var OrgSearchList = GetEmployeeDeptOfficeSecInfoByOrgogramId2(organogramLevelId);
        //    var labelName = string.Empty;
        //    var found = false;

        //    IList<OrganogramSearch2> listlbl = new List<OrganogramSearch2>();

        //    foreach (var item in OrgSearchList)
        //    {
        //        found = false;

        //        foreach (var item2 in listlbl)
        //        {
        //            if (item.OrganogramTypeName == item2.OrganogramTypeName)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }

        //        if (!found)
        //        {
        //            var lbl = new OrganogramSearch2 { OrganogramTypeName = item.OrganogramTypeName, OrganogramLevelName = item.OrganogramLevelName };
        //            listlbl.Add(lbl);
        //            labelName += " | " + item.OrganogramTypeName + ": ";
        //            labelName += item.OrganogramLevelName;
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(labelName)) labelName += " |";

        //    return labelName;
        //}

        public OrganogramSearch GetEmpDepartmentOfficeSectionSubSection(int organogramLevelId)
        {
            var OrgSearchList = GetEmployeeDeptOfficeSecInfoByOrgogramId2(organogramLevelId);
            var labelName = string.Empty;
            var found = false;
            var obj = new OrganogramSearch();

            IList<OrganogramSearch> list = new List<OrganogramSearch>();
            foreach (var item in OrgSearchList)
            {
                found = false;
                foreach (var item2 in list)
                {
                    if (item.OrganogramTypeName == item2.OrganogramTypeName)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (item.TableIdName == PRMEnum.EmployeeOrganogramDynamicLabel.DivisionId.ToString())
                    {
                        obj.DepartmentId = item.OrganogramLevelId;
                        obj.DepartmentName = item.OrganogramLevelName;
                        obj.OrganogramTypeName = "Department";
                        list.Add(obj);
                    }
                    else if (item.TableIdName == PRMEnum.EmployeeOrganogramDynamicLabel.OfficeId.ToString())
                    {
                        obj.OfficeId = item.OrganogramLevelId;
                        obj.OfficeName = item.OrganogramLevelName;
                        obj.OrganogramTypeName = "Office";
                        list.Add(obj);
                    }
                    else if (item.TableIdName == PRMEnum.EmployeeOrganogramDynamicLabel.SectionId.ToString())
                    {
                        obj.SectionId = item.OrganogramLevelId;
                        obj.SectionName = item.OrganogramLevelName;
                        obj.OrganogramTypeName = "Section";
                        list.Add(obj);
                    }
                    else if (item.TableIdName == PRMEnum.EmployeeOrganogramDynamicLabel.SubSectionId.ToString())
                    {
                        obj.SubSectionId = item.OrganogramLevelId;
                        obj.SubSectionName = item.OrganogramLevelName;
                        obj.OrganogramTypeName = "SubSection";
                        list.Add(obj);
                    }

                }
            }

            return obj;
        }

        //#region getZoneIdbyOrgId
        //public IList<ZoneOrganogramInfo> GetZoneInfo(int organogramLevelId)
        //{
        //    var query = (from m in base.PRMUnit.OrganogramLevelRepository.GetAll()
        //                 where m.Id == organogramLevelId
        //                 select m).ToList();
        //    do
        //    {
        //        var item = (from OrgL in base.PRMUnit.OrganogramLevelRepository.GetAll()
        //                    where OrgL.Id == query.ToList().Last().ParentId
        //                    select OrgL).FirstOrDefault();

        //        if (item != null)
        //        {
        //            query.Add(item);
        //        }
        //    } while (query.ToList().Last().ParentId != 0);

        //    var result = (from ogLParent in query
        //                  join ogLType in base.PRMUnit.OrganogramTypeRepository.GetAll() on ogLParent.OrganogramTypeId equals ogLType.Id
        //                  join orgTypeMap in base.PRMUnit.OrganogramTypeMappingRepository.GetAll() on ogLType.Id equals orgTypeMap.OrganogramTypeId
        //                  select new ZoneOrganogramInfo
        //                  {
        //                      ZoneInfoId = Convert.ToInt32(ogLParent.ZoneInfoId),
        //                      OrganogramLevelId = ogLParent.Id,
        //                      OrganogramLevelName = ogLParent.LevelName,
        //                      OrganogramTypeId = ogLType.Id,
        //                      OrganogramTypeName = ogLType.Name,
        //                      TableIdName = orgTypeMap.TableIdName,
        //                      SortOrder = ogLType.SortOrder
        //                  }).OrderBy(t => t.SortOrder);

        //    return result.ToList();
        //}

        //public class ZoneOrganogramInfo
        //{
        //    public int ZoneInfoId { get; set; }
        //    public int OrganogramLevelId { get; set; }
        //    public string OrganogramLevelName { get; set; }
        //    public int? OrganogramTypeId { get; set; }
        //    public string OrganogramTypeName { get; set; }
        //    public string TableIdName { get; set; }
        //    public int SortOrder { get; set; }
        //}

        //public int GetZoneIdByOrganogramId(int organogramLevelId)
        //{
        //    var OrgSearchList = GetZoneInfo(organogramLevelId);
        //    var labelName = string.Empty;
        //    var found = false;
        //    //var obj = new ZoneOrganogramInfo();
        //    int zoneInfoId = 0;
        //    IList<ZoneOrganogramInfo> list = new List<ZoneOrganogramInfo>();
        //    foreach (var item in OrgSearchList)
        //    {
        //        found = false;
        //        foreach (var item2 in list)
        //        {
        //            if (item.OrganogramTypeName == item2.OrganogramTypeName)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //        if (!found)
        //        {
        //            if (item.TableIdName == PRMEnum.EmployeeOrganogramDynamicLabel.ExecutiveOfficeOrZoneId.ToString())
        //            {
        //                zoneInfoId = item.ZoneInfoId;
        //            }
        //        }
        //    }
        //    return zoneInfoId;
        //}

        //#endregion


        public EmpLoginInfo GetEmpLoginInfo(string user)
        {
            var query = (from empInfo in base.PRMUnit.EmploymentInfoRepository.Fetch()
                         join des in base.PRMUnit.DesignationRepository.Fetch() on empInfo.DesignationId equals des.Id
                         join dep in base.PRMUnit.DivisionRepository.Fetch() on empInfo.DivisionId equals dep.Id into dpt
                         from lDpt in dpt.DefaultIfEmpty()
                         where (empInfo.EmpID == user)
                         select new EmpLoginInfo()
                         {
                             ID = empInfo.Id,
                             EmpId = empInfo.EmpID,
                             EmpName = empInfo.FullName,
                             DesignationId = des.Id,
                             DesignationName = des.Name,
                             Department = lDpt != null ? lDpt.Name : string.Empty,
                             JoiningDate = empInfo.DateofJoining,
                             ConfirmationDate = empInfo.DateofConfirmation
                         }).FirstOrDefault();

            return query;
        }

       
        public IList<BEPZA_MEDICAL.DAL.PRM.PRM_EmpSalaryDetail> GetEmpSalaryDetailsByGradeAndStepId(int gradeId, int stepId, int empId, out int salaryStructureId)
        {
            var query = from s in base.PRMUnit.EmpSalaryDetailRepository.Fetch()
                        where s.PRM_EmpSalary.GradeId == gradeId && s.PRM_EmpSalary.StepId == stepId && s.EmployeeId == empId
                        select s;

            var queryId = from s in base.PRMUnit.EmpSalaryRepository.Fetch()
                          where s.EmployeeId == empId
                          select s.SalaryStructureId;


            var result = query.ToList();
            salaryStructureId = queryId.FirstOrDefault();

            return result;
        }

        //public decimal GetSumOfGrossPayHeadByEmpId(int salaryStructureId)
        //{
        //    var query = (from sh in base.PRMUnit.SalaryHeadRepository.Fetch()
        //                 join ssd in base.PRMUnit.SalaryStructureDetailRepository.Fetch() on sh.Id equals ssd.HeadId
        //                 where ssd.SalaryStructureId == salaryStructureId && sh.IsGrossPayHead
        //                 select ssd).Sum(x => x.Amount);

        //    return query;

        //}

        public bool GetEmployeeContactByDareRange(DateTime sDate, DateTime eDate, int contactID, int employeeID, string strMode)
        {
            bool rv = false;

            if (strMode == "add")
            {
                rv = PRMUnit.EmploymentContractInfoRepository.Fetch().Where(
                          cp => (cp.EmpoyeeId == employeeID) &&
                              ((cp.ContractStartDate <= sDate && sDate <= cp.ContractEndDate) ||
                              (cp.ContractStartDate <= eDate && eDate <= cp.ContractEndDate)
                              ||
                              (sDate < cp.ContractStartDate && cp.ContractEndDate < eDate))).Any();
            }
            else
            {
                rv = PRMUnit.EmploymentContractInfoRepository.Fetch().Where(
                          cp => (cp.EmpoyeeId == employeeID && contactID != cp.Id) &&
                              ((cp.ContractStartDate <= sDate && sDate <= cp.ContractEndDate)
                              || (cp.ContractStartDate <= eDate && eDate <= cp.ContractEndDate)
                               ||
                              (sDate < cp.ContractStartDate && cp.ContractEndDate < eDate))).Any();
            }

            return rv;
        }

        public IList<EmployeeSalaryStructure> GetEmployeeSalaryStructure(int pEmployeeId)
        {
            var result = (from salDtl in base.PRMUnit.EmpSalaryDetailRepository.Get(t => t.EmployeeId == pEmployeeId)
                          join sal in base.PRMUnit.EmpSalaryRepository.GetAll() on salDtl.EmployeeId equals sal.EmployeeId
                          orderby salDtl.HeadId ascending
                          select new EmployeeSalaryStructure()
                          {
                              HeadId = salDtl.HeadId,
                              HeadType = salDtl.HeadType,
                              AmountType = salDtl.AmountType,
                              IsTaxable = salDtl.IsTaxable,
                              Amount = salDtl.Amount,
                              SalaryStructureId = sal.SalaryStructureId,
                              SalaryScaleId = sal.SalaryScaleId,
                              GradeId = sal.GradeId,
                              StepId = sal.StepId,
                              GrossSalary = sal.GrossSalary,
                              isConsolidated = sal.isConsolidated
                          }).ToList();

            return result;
        }

        public Decimal GetEmployeeBasicSalary(int pEmployeeId)
        {
            Decimal basicSalary = Decimal.Zero;

            var list = GetEmployeeSalaryStructure(pEmployeeId);

            foreach (var item in list)
            {
                var IsBasicHead = base.PRMUnit.SalaryHeadRepository.GetAll().Where(x => x.Id == item.HeadId).Select(x => x.IsBasicHead).FirstOrDefault();

                if (IsBasicHead == true)
                {
                    basicSalary = PRMEnum.FixedPercent.Fixed.ToString().Equals(item.AmountType)
                        ? item.Amount
                        : item.Amount * item.GrossSalary / 100;

                    break;
                }
            }
            return basicSalary;
        }

        /// <summary>
        /// Return hiarichy wise label name, i.e. 0 index is top order label then other
        /// </summary>
        public IList<OrganogramMappingLabel> OrganogramDynamicLabel()
        {
            var orgTypeLableResult = (from orgType in base.PRMUnit.OrganogramTypeRepository.GetAll()
                                      join orgTypeMap in base.PRMUnit.OrganogramTypeMappingRepository.GetAll() on orgType.Id equals orgTypeMap.OrganogramTypeId
                                      select new OrganogramMappingLabel
                                      {
                                          Id = orgType.Id,
                                          OrganogramTypeName = orgType.Name,
                                          OrganogramTypeMappingId = orgTypeMap.Id,
                                          OrganogramTypeMappingName = orgTypeMap.OrganogramTypeName,
                                          OrganogramTypeId = orgTypeMap.OrganogramTypeId,
                                          TableIdName = orgTypeMap.TableIdName,
                                          SortOrder = orgType.SortOrder
                                      }).OrderBy(t => t.SortOrder).ToList();

            return orgTypeLableResult;
        }


        #region Inner class

        public class EmployeeSearch
        {
            public Int32 ID { get; set; }
            public string EmpId { get; set; }
            public string EmpInitial { get; set; }
            public string EmpName { get; set; }
            public int? DesignationId { get; set; }
            public string DesigName { get; set; }
            public int EmpTypeId { get; set; }
            public string EmpTypeName { get; set; }
            public int? DivisionId { get; set; }
            public string DivisionName { get; set; }
            public int? JobLocationId { get; set; }
            public string JobLocName { get; set; }
            public int? GradeId { get; set; }
            public string GradeName { get; set; }
            public int StaffCategoryId { get; set; }
            public string StaffCatName { get; set; }
            //public int? ResourceLevelId { get; set; }
            public int? OrganogramLevelId { get; set; }
            public int ZoneInfoId { get; set; }
            public string ZoneName { get; set; }
            public string ResLevelName { get; set; }
            public DateTime DateOfJoining { get; set; }
            public DateTime? DateOfConfirmation { get; set; }
            public DateTime? DateOfInactive { get; set; }
            public bool IsContractual { get; set; }
            public decimal? ResourceRate { get; set; }
            public string EmpStatusName { get; set; }
            public Boolean IsBonusEligible { get; set; }
            public Boolean IsOvertimeEligible { get; set; }
            public Boolean? IsRefreshmentEligible { get; set; }
            public int? SortingOrder { get; set; }
        }
        public class EmpLoginInfo
        {
            public Int32 ID { get; set; }
            public string EmpId { get; set; }
            public string EmpName { get; set; }
            public int? DesignationId { get; set; }
            public string DesignationName { get; set; }
            public string Department { get; set; }
            public DateTime JoiningDate { get; set; }
            public DateTime? ConfirmationDate { get; set; }
        }


        public class OrganogramSearch
        {
            public int OrganogramLevelId { get; set; }
            public int? DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public int? OfficeId { get; set; }
            public string OfficeName { get; set; }
            public int? SectionId { get; set; }
            public string SectionName { get; set; }
            public int? SubSectionId { get; set; }
            public string SubSectionName { get; set; }
            public string OrganogramTypeName { get; set; }
            public int? SortOrder { get; set; }
        }

        public class OrganogramSearch2
        {
            public int OrganogramLevelId { get; set; }
            public int? OrganogramTypeId { get; set; }
            public string OrganogramTypeName { get; set; }
            public string OrganogramLevelName { get; set; }
            public string TableIdName { get; set; }
            public int SortOrder { get; set; }
        }

        public class EmployeeSalaryStructure
        {
            public int HeadId { get; set; }
            public string HeadType { get; set; }
            public string AmountType { get; set; }
            public bool IsTaxable { get; set; }
            public decimal Amount { get; set; }

            public int SalaryStructureId { get; set; }
            public int? SalaryScaleId { get; set; }
            public int GradeId { get; set; }
            public int StepId { get; set; }
            public decimal GrossSalary { get; set; }
            public bool isConsolidated { get; set; }
        }

        public class OrganogramMappingLabel
        {
            public int Id { get; set; }
            public string OrganogramTypeName { get; set; }
            public int OrganogramTypeMappingId { get; set; }
            public string OrganogramTypeMappingName { get; set; }
            public int OrganogramTypeId { get; set; }
            public string TableIdName { get; set; }
            public int SortOrder { get; set; }
        }
        #endregion
    }
}
