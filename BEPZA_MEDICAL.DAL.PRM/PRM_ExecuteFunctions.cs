using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace BEPZA_MEDICAL.DAL.PRM
{
    public class PRM_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZAPRMEntities _context;
        #endregion

        #region Ctor

        public PRM_ExecuteFunctions(ERP_BEPZAPRMEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Funtions
        public IList<CommonConfigGetResult> GetAllCommonConfig(string displayName,
                                                                int id,
                                                                string name,
                                                                string sortBy,
                                                                string sortType,
                                                                int rowIndex,
                                                                int maxRows,
                                                                out int totalRows
            )
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CommonCongifGet(displayName, id, name, sortBy, sortType, rowIndex, maxRows, numTotalRows, numErrorCode, strErrorMsg);

            totalRows = Convert.ToInt32(numTotalRows.Value);

            return query.ToList();
        }

        public int CommonConfigTypeDML(string displayName, int id, string name, int sortOrder, string remarks, string user, string mode)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CommonConfigTypeDML(displayName, id, name, sortOrder, remarks, user, mode, numErrorCode, strErrorMsg);

            return query;
        }

        public int UpdateEmployeeStatus(string type, int employeeId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.UpdateEmployeeStatus(type, employeeId, numErrorCode, strErrorMsg);

            return query;
        }

        public IList<PRM_FN_GetFinalComplaintEmployee_Result> PRM_FN_GetFinalComplaintEmployee(int employeeId, string restrictionName)
        {
            return _context.PRM_FN_GetFinalComplaintEmployee(employeeId, restrictionName).ToList();
        }
        public IList<SP_PRM_GetAllChildByOrganogramLevelId_Result> SP_PRM_GetAllChildByOrganogramLevelId(int organogramLevelId)
        {
            return _context.SP_PRM_GetAllChildByOrganogramLevelId(organogramLevelId).ToList();
        }

        public IList<APV_GetApproverInfoByApplicant_Result> GetApproverByApplicant(string applicantId, string processNameEnum)
        {
            return _context.APV_GetApproverInfoByApplicant(applicantId, processNameEnum).ToList();
        }

        public IList<APV_GetWelfareFundApplication_Result> GetWelfareFundApplications(string approverId, int approvalProcessId, DateTime? startDate, DateTime? endDate, int approvalStatusId)
        {
            return _context.APV_GetWelfareFundApplication(approverId, approvalProcessId, startDate, endDate, approvalStatusId).ToList();
        }

        public IList<APV_GetWelfareFundApplicationAlternateProcess_Result> GetWelfareFundApplicationsForAlternateProcess(string approverId, int approvalProcessId, DateTime? startDate, DateTime? endDate, int approvalStatusId)
        {
            return _context.APV_GetWelfareFundApplicationAlternateProcess(approverId, approvalProcessId, startDate, endDate, approvalStatusId).ToList();
        }

        public IList<fn_Apv_GetNextApprover_Result> GetNextApprover(int approvalProcessId, int applicationId)
        {
            return _context.fn_Apv_GetNextApprover(approvalProcessId, applicationId).ToList();
        }

        public IList<fn_Apv_GetApproverList_Result> GetNextApprover(int approvalProcessId, int applicationId, int approvalStepId)
        {
            return _context.fn_Apv_GetApproverList(approvalProcessId, applicationId, approvalStepId).ToList();
        }

        public APV_ProceedToNextStep_Result ProceedToNextStep(int? applicationId, string actionName, string comments, int? nextApproverId, string loggedOnUser)
        {
            return _context.APV_ProceedToNextStep(applicationId, actionName, comments, nextApproverId, loggedOnUser).FirstOrDefault();
        }

        public APV_ProceedToNextStepForAlternateProcess_Result ProceedToNextStepAlternateProcess(int? applicationId, string actionName, string comments, int? nextStepId, int? nextApproverId, string loggedOnUser)
        {
            return _context.APV_ProceedToNextStepForAlternateProcess(applicationId, actionName, comments, nextStepId, nextApproverId, loggedOnUser).FirstOrDefault();
        }

        public Apv_sp_InitializeApprovalProcess_Result InitializeApprovalProcess(string approvalProcess, string applicantId, int applicationId, int isOnlineApplication, int approverId, string iUser)
        {
            return _context.Apv_sp_InitializeApprovalProcess(approvalProcess, applicantId, applicationId, isOnlineApplication, approverId, iUser).FirstOrDefault();
        }

        public List<Apv_GetAcrApplication_Result> GetAcrApplications(int approvalProcessId, string approverId, int statusId, DateTime startDate, DateTime enDate)
        {
            return _context.Apv_GetAcrApplication(approvalProcessId, approverId, statusId, startDate, enDate).DefaultIfEmpty().OfType<Apv_GetAcrApplication_Result>().ToList();
        }

        public List<Apv_GetCpfMembershipApplication_Result> GetCpfMembershipApplications(int approvalProcessId, string approverId, int statusId, DateTime startDate, DateTime enDate)
        {
            return _context.Apv_GetCpfMembershipApplication(approvalProcessId, approverId, startDate, enDate, statusId).DefaultIfEmpty().OfType<Apv_GetCpfMembershipApplication_Result>().ToList();
        }

        public List<APV_GetINVRequisitionApplication_Result> GetINVRequisitionApplications(int approvalProcessId, string approverId, int statusId, DateTime startDate, DateTime enDate)
        {
            return _context.APV_GetINVRequisitionApplication(approverId, approvalProcessId,startDate, enDate, statusId).DefaultIfEmpty().OfType<APV_GetINVRequisitionApplication_Result>().ToList();
        }

        public List<Apv_GetInvScrapItemApplication_Result> GetInvScrapItemApplications(int approvalProcessId, string approverId, int statusId, DateTime startDate, DateTime enDate)
        {
            return _context.Apv_GetInvScrapItemApplication(approvalProcessId, approverId, startDate, enDate, statusId).DefaultIfEmpty().OfType<Apv_GetInvScrapItemApplication_Result>().ToList();
        }

        public void APV_INV_UpdateRequisitionDtl(int id, int requisitionId, int itemId, decimal ApproveQuantity)
        {
            var query = _context.APV_UpdateINVRequisitionDtl(id, requisitionId, itemId, ApproveQuantity);
        }

        public void APV_WFM_UpdateApplication(int id, decimal? suggestAmount)
        {
            var query = _context.APV_UpdateWFMApplication(id, suggestAmount);
        }
        public void UpdateUserInfoForTransfer(string EmpId, int ZoneInfoId)
        {
            var query = _context.PRM_UserInfoUpdateForTransfer(EmpId,ZoneInfoId);
        }

        /// <summary>
        /// Get Employees from 
        /// PRM_EmpStatusChange table (employment history - Confirmation, Promotion, Increment, Transfer)
        /// with specified filtering criteria.
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="AsOnDate"></param>
        /// <param name="DesignationId"></param>
        /// <param name="DisciplineId"></param>
        /// <param name="DivisionId"></param>
        /// <param name="JobLocationId"></param>
        /// <param name="EmploymentTypeId"></param>
        /// <param name="SalaryScaleId"></param>
        /// <param name="JobGradeId"></param>
        /// <param name="StepId"></param>
        /// <param name="ZoneInfoId"></param>
        /// <param name="OrganogramLevelId"></param>
        /// <returns>Table/List</returns>
        public IList<ufnGetEmployeeFromHistory_Result> GetEmployeeFromHistory(
            string Year
            , string Month
            , DateTime? AsOnDate
            , int? ZoneInfoId
            , int? OrganogramLevelId
            , int? DesignationId
            , int? DisciplineId
            , int? DivisionId
            , int? JobLocationId
            , int? EmploymentTypeId
            , int? SalaryScaleId
            , int? JobGradeId
            , int? StepId
            , bool IsForPGM = false
            )
        {
            if (!IsForPGM)
            {
                var query = _context.ufnGetEmployeeFromHistory(Year
                    , Month
                    , AsOnDate
                    , ZoneInfoId
                    , OrganogramLevelId
                    , DesignationId
                    , DisciplineId
                    , DivisionId
                    , JobLocationId
                    , EmploymentTypeId
                    , SalaryScaleId
                    , JobGradeId
                    , StepId
                );
                return query.ToList();
            }
            else
            {
                var query = _context.ufnGetEmployeeFromHistoryForPGM(Year
                    , Month
                    , AsOnDate
                    , ZoneInfoId
                    , OrganogramLevelId
                    , DesignationId
                    , DisciplineId
                    , DivisionId
                    , JobLocationId
                    , EmploymentTypeId
                    , SalaryScaleId
                    , JobGradeId
                    , StepId
                );
                return query.ToList();
            }
        }

        #endregion

        public int RollNoGeneration(string AdCode, int DesignationId, int RollNoFrom, string userId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.EREC_uspRollGeneratioin(AdCode, DesignationId, RollNoFrom, userId, numErrorCode, strErrorMsg);

            return query;
        }

    }
}
