using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPZA_MEDICAL.DAL.CPF;
using System.Data.Entity.Core.Objects;

namespace BEPZA_MEDICAL.DAL.CPF
{
    public class CPF_ExecuteFunctions
    {
        #region Fields
        private readonly ERP_BEPZACPFEntities _context;
        #endregion

        #region Ctor

        public CPF_ExecuteFunctions(ERP_BEPZACPFEntities context)
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

            var query = _context.CommonConfigGet(displayName, id, name, sortBy, sortType, rowIndex, maxRows, numTotalRows, numErrorCode, strErrorMsg);

            totalRows = Convert.ToInt32(numTotalRows.Value);

            return query.ToList();
        }

        public int CommonConfigTypeDML(string displayName, int id, string name, int sortOrder, string remarks, string user, string mode)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CommonConfigTypeDML(displayName, id, name, sortOrder, remarks, user, mode, numErrorCode, strErrorMsg);
            query = Convert.ToInt32(numErrorCode.Value);
            return query;
        }

        public IList<GetDebitOrCredit> GetDebitOrCredit(int bankAccontId, string transactionType)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.CPF_GetDebitCreditBankReconciliation(bankAccontId, transactionType, numErrorCode, strErrorMsg).ToList();

            return query;
        }

        public int DeletePFMembeship(int applicationId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CPF_DeleteMembership(applicationId, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int ProfitDistributionProcess(string PeriodYear, string PeriodMonth, decimal InterestRate, string Remarks, string strUser)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CPF_procProfitDistribution(PeriodYear, PeriodMonth, InterestRate, Remarks, strUser, numErrorCode, strErrorMsg);

            return Convert.ToInt16(numErrorCode.Value);
        }

        public int ProfitDistributionProcessRollback(int PeriodId, string strUser)
        {

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CPF_procProfitDistributionRollback(PeriodId, strUser, numErrorCode, strErrorMsg);

            return Convert.ToInt16(numErrorCode.Value);
        }

        public int ProfitDistributionProcessRollbackIndividual(int PeriodId, int DetailId, string strUser)
        {

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var query = _context.CPF_procProfitDistributionRollbackIndividual(PeriodId, DetailId, strUser, numErrorCode, strErrorMsg);

            return Convert.ToInt16(numErrorCode.Value);
        }

        public IList<GetMyLoanStatusResult> GetMyLoanStatus(int employeeId)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.CPF_GetMyLoanStatus(employeeId, numErrorCode, strErrorMsg).ToList();

            return query;
        }

        public IList<CPF_EmployeeCPF_WF_FundStatus_Result> GetProvidentFundStatusFor(int employeeId)
        {
            var query = _context.CPF_EmployeeCPF_WF_FundStatus(employeeId, DateTime.Now).ToList();

            return query;
        }

        public IList<CPF_GetMyLoanSummary_Result> GetMyLoanSummary(int employeeId)
        {
            var query = _context.CPF_GetMyLoanSummary(employeeId).ToList();

            return query;
        }

        public IList<CPF_GetMyCPFSummary_Result> GetMyCpfSummary(string employeeId)
        {
            var query = _context.CPF_GetMyCPFSummary(employeeId).ToList();

            return query;
        }

        public CPF_RptCpfMemberInformation_Result GetMembershipInformation(int? employeeId)
        {
            return _context.CPF_RptCpfMemberInformation(employeeId).FirstOrDefault();
        }

        #endregion
    }
}
