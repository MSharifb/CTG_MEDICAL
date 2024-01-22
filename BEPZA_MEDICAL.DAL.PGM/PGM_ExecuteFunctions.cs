using BEPZA_MEDICAL.DAL.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace BEPZA_MEDICAL.DAL.PGM
{
    public class PGM_ExecuteFunctions
    {

        #region Fields
        private readonly ERP_BEPZAPGMEntities _context;
        #endregion

        #region Ctor

        public PGM_ExecuteFunctions(ERP_BEPZAPGMEntities context)
        {
            this._context = context;
        }

        #endregion

        #region Methods

        public int MonthlySalaryProcess(string salaryYear, string salaryMonth, int employeeId, int zoneInfoId, bool processTax, string user, out int totalProcessed)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var numTotalProcessed = new ObjectParameter("numTotalProcessed", typeof(int));

            _context.procMonthlySalary(salaryYear, salaryMonth, employeeId, zoneInfoId, processTax, user, numErrorCode, strErrorMsg, numTotalProcessed);

            totalProcessed = Convert.ToInt32(numTotalProcessed.Value);

            var errorCode = Convert.ToInt32(numErrorCode.Value);
            return errorCode;
        }

        public int MonthlySalaryDistributionProcess(string SalaryYear, string SalaryMonth, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procSalaryDistributionProcess(SalaryYear, SalaryMonth, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int MonthlySalaryRollbackProcess(string SalaryYear, string SalaryMonth, int? ZoneInfoId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procMonthlySalaryRollback(SalaryYear, SalaryMonth, ZoneInfoId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int MonthlySalaryDistributionRollbackProcess(string SalaryYear, string SalaryMonth, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procSalaryDistributionProcessRollback(SalaryYear, SalaryMonth, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int MonthlySalaryRollbackIndividual(Int64 SalaryId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.MonthlySalaryRollbackIndividual(SalaryId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int BonusProcess(string BonusYear, string BonusMonth, DateTime dtEffectiveDate, int bonusTypeId, string amountType, decimal amount, decimal rS, int? religionId, int? DepartmentId, int? SectionId, int? StuffCategoryId, int? JobGradeId, bool IsAll, int? EmployeeId, int? ZoneInfoId, byte? basicCalculationMonth, string remarks, string IUser, out string errorMsg)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procBonusProcess(BonusYear, BonusMonth, dtEffectiveDate, bonusTypeId, amountType, amount, rS, religionId, DepartmentId, SectionId, StuffCategoryId, JobGradeId, IsAll, EmployeeId, ZoneInfoId, basicCalculationMonth, remarks, IUser, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);
            errorMsg = Convert.ToString(strErrorMsg.Value);
            return query;
        }

        public int IncentiveBonusProcess(string FinancialYear, DateTime IncentiveBonusDate, int BonusTypeId, DateTime? OrderDate, string OrderRefNo, string Remark, string FormulaSelect, string FinancialYearForFormula, DateTime? BasicSalaryCalFromFinancialYear, int IncentiveBonusDay, int? DayOfMonth, int? TotalNumOfMonth, int? DayOfYear, int? CalenderYearForFormula, DateTime? BasicSalaryCalFromCalenderYear, int? LEELDOY, int? TDoYWELD, Decimal FormulaFactor, int? ZoneInfoId, int? DepartmentId, int? StaffCategoryId, int? EmploymentTypeId, bool IsAll, int EmployeeId, Decimal RevenueStamp, string IUser, DateTime IDate, string EUser, DateTime? EDate)
        {
            try
            {
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                var query = _context.PGM_uspProcIncentiveBonusProcess(
                    FinancialYear,
                    IncentiveBonusDate,
                    BonusTypeId,
                    OrderDate,
                    OrderRefNo,
                    Remark,
                    FormulaSelect,
                    FinancialYearForFormula,
                    BasicSalaryCalFromFinancialYear,
                    IncentiveBonusDay,
                    DayOfMonth,
                    TotalNumOfMonth,
                    DayOfYear,
                    CalenderYearForFormula,
                    BasicSalaryCalFromCalenderYear,
                    LEELDOY,
                    TDoYWELD,
                    FormulaFactor,

                    ZoneInfoId,
                    DepartmentId,
                    StaffCategoryId,
                    EmploymentTypeId,

                    IsAll,
                    EmployeeId,
                    RevenueStamp,
                    IUser,
                    IDate,
                    EUser,
                    EDate,
                    numErrorCode,
                    strErrorMsg);

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public int IncentiveBonusProcessRollback(int IncentiveBonusId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.PGM_uspProcIncentiveBonusProcessRollback(IncentiveBonusId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int IncentiveBonusProcessRollbackIndividaul(int BonusId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.PGM_uspProcIncentiveBonusProcessRollbackIndividual(BonusId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int LMS_LedgerUpdate(int intLeaveYearID, int intLeaveTypeID, string strEmpID, string strCompanyID)
        {
            int numErrorCode = 0;
            //   var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            try
            {
                var query = _context.LeaveLedgerUpdate(intLeaveYearID, intLeaveTypeID, strEmpID, strCompanyID);
                numErrorCode = 0;
            }
            catch (Exception)
            {
                numErrorCode = -1;
            }

            //query = Convert.ToInt32(numErrorCode.Value);

            return numErrorCode;
        }

        public int BonusProcessRollback(int BonusId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procBonusRollback(BonusId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int BonusProcessRollbackIndividaul(int BonusId, int EmployeeId, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procBonusRollbackIndividual(BonusId, EmployeeId, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int IncomeTaxComputationProcess(string IncomeYear, string SalaryYear, string SalaryMonth, int employeeId, int? zoneInfoId, string IUser)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procIncomeTaxComputation(
                IncomeYear,
                SalaryYear,
                SalaryMonth,
                employeeId,
                1,
                zoneInfoId,
                IUser,
                numErrorCode,
                strErrorMsg);

            // var query = 0;
            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int IncomeTaxRollback(int TaxRuleId, string user, int? zoneId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procIncomeTaxRollback(TaxRuleId, user, zoneId, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int DeleteBankLetter(int Id, string user)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.DeleteBankAdviceLetter(Id, user, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public int DeleteBankLetterBody(Int64 BankLetterId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.DeleteBankLetterBody(BankLetterId, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        public IList<BankLetterDetailAddPayee> BankAdviceLetterAddPayee(string salaryYear, string salaryMonth, string lettetType)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.BankLetterDetailAddPayee(salaryYear, salaryMonth, lettetType, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }

        public IList<GetFinalSettlement> GetFinalSettlementIndividual(int id)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.GetFinalSettlement(id, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }

        public IList<PGM_SP_GetFinalSettlement_Result> GetFinalSettlementIndividual2(int id)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.PGM_SP_GetFinalSettlement(id, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }
        
        public IList<GetLeaveEncashmentEmployee> GetLeaveEncashmentIndividual(int id)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.GetLeaveEncashmentEmployee(id, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }

        public IList<GetEmployeeForOverTime> GetOverTimedEmployee(int id)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.GetEmployeeForOverTime(id, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }

        public IList<sp_PGM_GetEmployeeForBonusIndividual_Result> EmployeeForBonusIndividual(int EmployeeId, string EmpID)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.sp_PGM_GetEmployeeForBonusIndividual(EmployeeId, EmpID, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }

        public IList<GetEmployeeByInitial> GetEmployeeByEmployeeInitial(string initial)
        {
            var numTotalRows = new ObjectParameter("numTotalRows", typeof(int));
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.GetEmployeeByInitial(initial, numErrorCode, strErrorMsg).ToList();

            return query.ToList();
        }

        public int DeleteWithheldSalaryPaymentBySalaryID(Int64 SalaryId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.DeleteWithheldSalaryBySalaryId(SalaryId, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        /// <summary>
        ///  Description:	Get salary structure considering promotion and increment
        ///        i.e. Considering upto month promotion and increment, 
        ///        If salary process is July-2015 then consider promotion and increment upto July-2015 
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="dtSalaryLastDate"></param>
        /// <returns></returns>
        public IList<fnGetSalaryInfoConsiderIncPro_Result> GetSalaryInfoConsiderIncPro(int EmployeeId, DateTime dtSalaryLastDate)
        {
            var query = _context.fnGetSalaryInfoConsiderIncPro(EmployeeId, dtSalaryLastDate);
            return query.ToList();
        }

        //public IList<sp_PGM_GetMonthlySalaryStatement_Result> GetMonthlySalaryStatement(string SalaryYear, string SalaryMonth)
        //{

        //    var query = _context.sp_PGM_GetMonthlySalaryStatement(SalaryYear, SalaryMonth).ToList();

        //    return query;
        //}

        //public IList<sp_PGM_Payslip_Result> GetMonthlySalaryPayslip(string SalaryYear, string SalaryMonth)
        //{

        //    var query = _context.sp_PGM_Payslip(SalaryYear, SalaryMonth).ToList();

        //    return query;
        //}

        public int IncomeTaxRollbackIndividual(int taxRuleId, int employeeId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.procIncomeTaxRollbackIndividual(taxRuleId, employeeId, numErrorCode, strErrorMsg);

            query = Convert.ToInt32(numErrorCode.Value);

            return query;
        }

        /// <summary>
        /// Get basic salary from employee salary structure
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public decimal GetBasicSalary(int employeeId)
        {
            var query = _context.ExecuteStoreQuery<decimal>("SELECT  dbo.fn_getBasicSalary(" + employeeId + ")").FirstOrDefault();

            return query;
        }

        /// <summary>
        /// Get basic salary from join of employee monthly salary,
        /// salary structure.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetBasicSalary(int employeeId, string year, string month)
        {
            var query = _context.ExecuteStoreQuery<decimal>("SELECT  dbo.fn_getBasicSalaryForBonus(" + employeeId + ", '" + year + "', '" + month + "')").FirstOrDefault();

            return query;
        }

        public List<sp_PGM_GetSalaryStructureForArrearAdjustment_Result> GetSalaryStructureForArrearAdjustment(string arrearIds)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var query = _context.sp_PGM_GetSalaryStructureForArrearAdjustment(arrearIds, numErrorCode, strErrorMsg).ToList();
            return query;
        }


        #region Get Zone Info
        public List<vwPGMZoneInfo> GetZoneInfoList()
        {
            return _context.vwPGMZoneInfoes.OrderBy(z=> z.SortOrder).ToList();
        }

        public vwPGMZoneInfo GetZoneInfoById(int zoneInfoId)
        {
            return GetZoneInfoList().FirstOrDefault(e => e.Id == zoneInfoId);
        }
        #endregion

        #region Get Employee Info
        public List<vwPGMEmploymentInfo> GetEmployeeList()
        {
            return _context.vwPGMEmploymentInfoes.OrderBy(e=> e.SortingOrder).ToList();
        }

        public List<vwPGMEmploymentInfo> GetEmployeeList(int salaryWithdrawFromZoneId)
        {
            return GetEmployeeList().Where(e=> e.SalaryWithdrawFromZoneId == salaryWithdrawFromZoneId).ToList();
        }

        public List<vwPGMEmploymentInfo> GetEmployeeList(int salaryWithdrawFromZoneId, DateTime asOnDate)
        {
            return GetEmployeeList(salaryWithdrawFromZoneId).Where(e=> e.DateofJoining <= asOnDate).ToList();
        }

        public List<vwPGMEmploymentInfo> GetEmployeeList(int salaryWithdrawFromZoneId, Boolean onlyActive)
        {
            return GetEmployeeList(salaryWithdrawFromZoneId).Where(e => e.DateofInactive == null).ToList();
        }

        public vwPGMEmploymentInfo GetEmployeeById(int employeeId)
        {
            return GetEmployeeList().FirstOrDefault(e => e.Id == employeeId);
        }

        public vwPGMEmploymentInfo GetEmployeeByEmpId(string empId)
        {
            return GetEmployeeList().FirstOrDefault(e => e.EmpID == empId);
        }

        //public IEnumerable<EmployeeInfoDDLModel> GetEmployeeListForDDL(int salaryWithdrawFromZoneId, bool onlyActive = true)
        //{
        //    var emps = GetEmployeeList().ToList()
        //        .Where(e => Convert.ToInt32(e.SalaryWithdrawFromZoneId) == salaryWithdrawFromZoneId);

        //    if (onlyActive)
        //    {
        //        emps = emps.Where(e => e.DateofInactive == null);
        //    }

        //    var returnemps = emps.Select(q => new EmployeeInfoDDLModel
        //    {
        //        Id = q.Id,
        //        FullName = q.FullName + " [" + q.EmpID + "]"
        //    });

        //    return returnemps.OrderBy(r=> r.FullName).ToList();
        //}
        #endregion

        #region Update Employment Info

        public bool UpdateEmploymentInfo(int employeeId, int jobGradeId, int? bankId, int? bankBranchId, string accountNo, int salaryWithdrawFromZoneId, int? taxRegionId, int? assesseeTypeId, bool havingChildWithDisability, bool isBonusEligible, bool isOvertimeEligible, bool isGPFEligible, bool isPensionEligible, bool isLeverageEligible, bool isRefreshmentEligible, String userName)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            //var query = _context.sp_PGM_UpdateEmploymentInfo(employeeId, jobGradeId, bankId, bankBranchId, accountNo, salaryWithdrawFromZoneId, taxRegionId, assesseeTypeId, havingChildWithDisability
            //    , isBonusEligible, isOvertimeEligible, isGPFEligible, isPensionEligible, isLeverageEligible, isRefreshmentEligible, userName, numErrorCode, strErrorMsg);

            var errorCode = Convert.ToInt32(numErrorCode.Value);

            return errorCode == 0;
        }

        #endregion
        
        #region ELMAH

        public IEnumerable<ufnGetELMAHErrorList_Result> GetElmahErrorList(DateTime date)
        {
            IEnumerable<ufnGetELMAHErrorList_Result> result = null;

            //if (!String.IsNullOrEmpty(orderBy))
            //    result = _context.ufnGetELMAHErrorList(date).OrderBy(orderBy).ToList();
            //else
            result = _context.ufnGetELMAHErrorList(date).ToList();

            return result;
        }

        public IEnumerable<ufnGetELMAHSearchData_Result> GetElmahSearchData(String filterText)
        {
            IEnumerable<ufnGetELMAHSearchData_Result> result = null;

            result = _context.ufnGetELMAHSearchData(filterText).ToList();

            return result;
        }
        #endregion

        #endregion
        
    }
}
