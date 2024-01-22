using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.PGM;
using BEPZA_MEDICAL.DAL.PGM.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BEPZA_MEDICAL.Domain.PGM
{
    public class PGMCommonService
    {
        #region Fields

        PGM_UnitOfWork _pgmUnit { get; set; }

        #endregion

        #region Ctor

        public PGMCommonService(PGM_UnitOfWork uow)
        {
            _pgmUnit = uow;
        }

        #endregion

        #region Properties

        public PGM_UnitOfWork PGMUnit { get { return _pgmUnit; } }

        #endregion

        #region Workflow method

        public IList<LeaveEncashmentSearchModel> GetLeaveEncashmentSearchedList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount, out int totalRecord,
            string year = "", string month = "", string projectNo = "", string employeeId = "", string empInitial = "", string empName = "", int zoneId = 0)
        {
            var query = @"SELECT lev.Id
                            , lev.SalaryYear
                            , lev.SalaryMonth
                            , lev.EmployeeId
                            , emp.EmpID
                            , emp.EmployeeInitial
                            , emp.FullName
                            , lev.EncashmentDays
                            , lev.EncashmentRate
                            , lev.EncashmentAmount 
                        FROM PGM_LeaveEncashment lev 
                            INNER JOIN PRM_EmploymentInfo emp on lev.EmployeeId=emp.Id 
                            INNER JOIN ufnGetEmployeeFromHistoryForPGM('" + year + @"', '" + month + @"', NULL, " + zoneId + @", NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)  EH ON lev.EmployeeId = EH.EmployeeId
";
            if (!string.IsNullOrEmpty(employeeId))
                query += " AND emp.EmpID = '" + employeeId + "'";

            if (!string.IsNullOrEmpty(year))
                query += " AND lev.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(month))
                query += " AND lev.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(empInitial))
                query += " AND emp.EmployeeInitial LIKE '%" + empInitial + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND emp.FullName LIKE '%" + empName + "%'";

            var data = PGMUnit.LeaveEncashmentSearch.GetWithRawSql(query).AsQueryable();

            totalRecord = data.Count();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public SalaryRateCalculation GetSalaryRateByEmployeeID(int? employeeId = 0)
        {
            var query = @"select empd.Id
                                ,empd.EmployeeId
                                ,E.EmpID
                                ,empd.HeadId
                                ,empd.HeadType
                                ,empd.AmountType
                                ,empd.IsTaxable
                                ,dbo.fn_getBasicSalary(empd.EmployeeId) as Basic
                                , emps.GrossSalary
                         from PRM_EmpSalaryDetail empd 
                            inner join PRM_EmpSalary emps on empd.EmployeeId=emps.EmployeeId 
                            INNER JOIN PRM_EmploymentInfo E ON E.Id=empd.EmployeeId                         
                         where empd.HeadId=1";

            if (employeeId != 0)
                query += " AND empd.EmployeeId = " + employeeId;

            var data = PGMUnit.SalaryRateCalculation.GetWithRawSql(query).FirstOrDefault();
            SalaryRateCalculation salary = new SalaryRateCalculation();

            salary.Basic = data != null ? data.Basic : 0;
            salary.GrossSalary = data != null ? data.GrossSalary : 0;
            salary.Rate = Math.Round(Convert.ToDecimal((data != null ? data.Basic : 1) / 18), 2);

            return salary;
        }

        public IList<ClosingBalanceSearchModel> GetClosingBalanceByYear(int? leaveYear = 0, int? leaveTypeId = 0, string EmployeeId = "")
        {
            var query = @"Select lyear.intLeaveYearID,lyear.strYearTitle,
                          levled.intLeaveTypeID,levled.strEmpID EmployeeId,isnull(dbo.FN_ConvertHourToDay(lyear.intLeaveYearID,lyear.strCompanyID,levled.fltCB),0) fltCB 
                          From LMS_tblLeaveYear lyear inner join LMS_tblLeaveLedger levled 
                          on lyear.intLeaveYearID=levled.intLeaveYearID WHERE lyear.intLeaveYearID=(SELECT LY.intLeaveYearID
                            FROM LMS_tblLeaveType LT
                            INNER JOIN LMS_tblLeaveYearType LYT ON LT.intLeaveYearTypeId=LYT.intLeaveYearTypeId
                            INNER JOIN LMS_tblLeaveYear LY ON LYT.intLeaveYearTypeId=LY.intLeaveYearTypeId
                            WHERE LT.intLeaveTypeID=" + leaveTypeId + " AND LY.bitIsActiveYear=1 )";

            //if (leaveYear != 0)
            //    query += " WHERE lyear.strYearTitle='" + leaveYear + "'";

            //if (leaveTypeId != 0)
            //    query += " AND levled.intLeaveTypeID = " + leaveTypeId;

            if (EmployeeId != "")
                query += " AND levled.strEmpID = '" + EmployeeId + "'";

            var data = PGMUnit.ClosingBalance.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<WithheldSalaryPaymentSearchModel> GetWithheldSalaryPaymentSearchedList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount,
            string year = "",
            string month = "",
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string paymentStatus = "",
            string empInitial = "",
            string empName = ""
            )
        {
            var query = @"SELECT SM.Id
                            , SM.SalaryYear
                            , SM.SalaryMonth
                            , E.EmpID
                            , E.EmployeeInitial
                            , E.FullName
                            , E.BankAccountNo AS AccountNo
                            , SD.HeadAmount
                            , SM.IsWithheld
                            , SM.IsPaid
                            , W.PaymentDate
                          FROM PGM_Salary SM INNER JOIN 
                                (SELECT SalaryId,HeadAmount  FROM PGM_SalaryDetail WHERE HeadName='Net Pay' or HeadName='Net Payable') SD
                          ON SM.Id=SD.SalaryId
                          INNER JOIN PRM_EmploymentInfo E on E.Id=SM.EmployeeId 
                          LEFT OUTER JOIN PGM_WithheldSalaryPayment W ON W.SalaryId=SM.Id
                          WHERE SM.IsWithheld=1";


            if (!string.IsNullOrEmpty(year))
                query += " AND SM.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(month))
                query += " AND SM.SalaryMonth LIKE '%" + month + "%'";

            if (fromDate.HasValue && toDate.HasValue)
            {
                query += " AND ( W.PaymentDate >= '" + fromDate.Value.ToString("dd/MMM/yyyy") + "' AND W.PaymentDate <= '" + toDate.Value.ToString("dd/MMM/yyyy") + "' )";
                fromDate = null;
                toDate = null;
            }
            if (fromDate.HasValue)
                query += " AND W.PaymentDate >= '" + fromDate.Value.ToString("dd/MMM/yyyy") + "'";

            if (toDate.HasValue)
                query += " AND W.PaymentDate <= '" + toDate.Value.ToString("dd/MMM/yyyy") + "'";

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                if (paymentStatus == "Paid")
                {
                    query += " AND SM.IsPaid =1 ";
                }
                else if (paymentStatus == "Unpaid")
                {
                    query += " AND SM.IsPaid =0 ";
                }
            }

            if (!string.IsNullOrEmpty(empInitial))
                query += " AND E.EmployeeInitial LIKE '%" + empInitial + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND E.FullName LIKE '%" + empName + "%'";

            var data = PGMUnit.WithheldSalaryPaymentSearch.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<WithheldSalaryPaymentSearchModel> GetPaymentID(int? Id = 0)
        {

            var query = @"SELECT SM.Id, SM.SalaryYear, SM.SalaryMonth,E.Id as EmployeeId,E.EmpID,
                       E.EmployeeInitial, E.FullName,SM.AccountNo,SD.HeadAmount,
                       SM.IsWithheld,E.BankId,E.BankBranchId,SM.IsPaid
                       FROM PGM_Salary SM INNER JOIN 
	                    (SELECT SalaryId,HeadAmount  FROM PGM_SalaryDetail WHERE HeadName='Net Pay' or HeadName='Net Payable') SD
                        ON SM.Id=SD.SalaryId
                         INNER JOIN PRM_EmploymentInfo E on E.Id=SM.EmployeeId 
                         WHERE SM.IsWithheld=1";


            if (Id != 0)
                query += " AND SM.Id = '" + Id + "'";

            var data = PGMUnit.WithheldSalaryPaymentSearch.GetWithRawSql(query);
            return data.ToList();
        }

        public IList<TaxOpeningDetail> GetTaxOpeningDetail()
        {
            var query = @"SELECT 0 as Id,0 as TaxOpeningId,Id as IncomeHeadId,HeadName as IncomeHead,'Salary' as HeadSource,0.0 as IncomeAmount
                        FROM PRM_SalaryHead
                        WHERE IsTaxable=1 and HeadType='Addition'
                        UNION
                        SELECT 0 as Id,0 as TaxOpeningId,Id as IncomeHeadId,BonusType as IncomeHead,'Bonus' as HeadSource,0.0 as IncomeAmount
                        FROM PGM_BonusType
                        WHERE IsTaxable=1";


            var data = PGMUnit.TaxopeningDatailCustom.GetWithRawSql(query);
            return data.ToList();
        }

        public EmployeeBasicSalary GetBasicSalaryByEmployeeId(int? employeeId = 0)
        {
            var query = @"select Id, EmpID, EmployeeInitial, FullName, dbo.fn_getBasicSalary(Id) as Basic
                        FROM PRM_EmploymentInfo";

            if (employeeId != 0)
                query += " where Id = " + employeeId;

            var data = PGMUnit.EmployeeBasicSalarySearch.GetWithRawSql(query);

            EmployeeBasicSalary salary = new EmployeeBasicSalary();

            foreach (EmployeeBasicSalary eSalary in data)
            {
                salary.Basic = eSalary.Basic;
                salary.EmployeeId = eSalary.Id;
                salary.EmpID = eSalary.EmpID;
                salary.EmployeeId = eSalary.EmployeeId;
            }

            return salary;
        }
        
        public IList<GratuitySettlementSearchModel> GratuitySettlementSearchList(DateTime? fromDate = null, DateTime? toDate = null, string paymentStatus = "", string empInitial = "", string empName = "")
        {
            var query = @"SELECT G.EmployeeId, E.EmpID,DATENAME(YEAR, G.DateofPayment) AS SalaryYear,
                        DATENAME(MONTH,G.DateofPayment) AS SalaryMonth,E.EmployeeInitial,
                        E.FullName,G.BasicSalary,E.DateofJoining,E.DateofInactive, E.DateofConfirmation,
                        G.GratuityAmount,G.DateofPayment,G.isPaid
                        FROM PGM_GratuityPayment G  
                        INNER JOIN PRM_EmploymentInfo E ON E.Id=G.EmployeeId 
                        INNER JOIN PRM_EmpSeperation ES ON E.Id=ES.EmployeeId
                        WHERE 0=0 AND E.DateofConfirmation IS NOT NULL AND ES.EffectiveDate IS NOT NULL";

            if (fromDate.HasValue && toDate.HasValue)
            {
                query += " AND ( G.DateofPayment >= '" + fromDate.Value.ToString("dd/MMM/yyyy") + "' AND G.DateofPayment <= '" + toDate.Value.ToString("dd/MMM/yyyy") + "' )";
                fromDate = null;
                toDate = null;
            }
            if (fromDate.HasValue)
                query += " AND G.DateofPayment >= '" + fromDate.Value.ToString("dd/MMM/yyyy") + "'";

            if (toDate.HasValue)
                query += " AND G.DateofPayment <= '" + toDate.Value.ToString("dd/MMM/yyyy") + "'";

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                if (paymentStatus == "Paid")
                {
                    query += " AND G.isPaid=1 ";
                }
                else if (paymentStatus == "Unpaid")
                {
                    query += " AND G.isPaid=0 ";
                }
            }
            if (!string.IsNullOrEmpty(empInitial))
                query += " AND E.EmployeeInitial LIKE '%" + empInitial + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND E.FullName LIKE '%" + empName + "%'";

            var data = PGMUnit.GratuitySettlementSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<FinalSettlementSearchModel> FinalSettlementSearchList(DateTime? fromDate = null, DateTime? toDate = null, string division = "", string empInitial = "", string empName = "")
        {

            var query = @"SELECT FS.EmployeeId, E.EmpID,E.FullName,FS.SalaryPayable,
                            FS.BasicSalary,FS.GrossSalary,FS.DateofSettlement,FS.LeaveEncasement,FS.GratuityPayable,
                            FS.OtherAdjustment,FS.ShortageofNoticePeriod,FS.AdvanceDeduction,FS.NetPFBalance,
                            FS.OtherDeduction, E.DateofJoining,E.DateofInactive,FS.NetPayable

                            FROM PRM_EmploymentInfo E INNER JOIN PGM_FinalSettlement FS
                            ON E.Id=FS.EmployeeId where 0=0 ";

            if (fromDate.HasValue && toDate.HasValue)
            {
                query += " AND ( FS.DateofSettlement >= '" + fromDate.Value.ToString("dd/MMM/yyyy") + "' AND FS.DateofSettlement <= '" + toDate.Value.ToString("dd/MMM/yyyy") + "' )";
                fromDate = null;
                toDate = null;
            }
            if (fromDate.HasValue)
                query += " AND FS.DateofSettlement >= '" + fromDate.Value.ToString("dd/MMM/yyyy") + "'";

            if (toDate.HasValue)
                query += " AND FS.DateofSettlement <= '" + toDate.Value.ToString("dd/MMM/yyyy") + "'";

            if (!string.IsNullOrEmpty(empInitial))
                query += " AND E.EmpID LIKE '%" + empInitial + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND E.FullName LIKE '%" + empName + "%'";

            var data = PGMUnit.FinalSettlementSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<OtherAdjustDeductSearchModel> OtherAdjustmentSearchList(string month, string year, string empId, string empName, string desigId, int zoneId)
        {

            var query = @"SELECT tr.Id
	                            , tr.EmployeeId
	                            , tr.SalaryMonth
	                            , tr.SalaryYear
	                            , tr.Type
	                            , tr.SalaryHeadId
	                            , SalaryHead = Sal.HeadName
	                            , E.EmpID
	                            , EmployeeName = E.FullName
	                            , EmployeeDesignation = D.Name
                                , tr.IsOverrideStructureAmount
	                            , tr.Amount
                             FROM PGM_OtherAdjustDeduct tr
	                            INNER JOIN PRM_EmploymentInfo E on tr.EmployeeId = E.Id
	                            INNER JOIN PRM_Designation D ON E.DesignationId = D.Id
	                            INNER JOIN PRM_SalaryHead Sal ON tr.SalaryHeadId = Sal.Id
                            WHERE E.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(month))
                query += " AND tr.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND tr.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(empId))
                query += " AND E.EmpID LIKE '%" + empId + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND E.FullName LIKE '%" + empName + "%'";

            if (!string.IsNullOrEmpty(desigId))
                query += " AND D.Id = " + Convert.ToInt32(desigId);
            
            query += " ORDER BY D.SortingOrder ASC ";

            var data = PGMUnit.OtehrAdjustDeductSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<OtherAdjustDeductSearchModel> OtherAdjustmentListMaster(string month, string year, int zoneId)
        {

            var query = @"SELECT distinct
	                             tr.SalaryMonth
	                            , tr.SalaryYear
                             FROM PGM_OtherAdjustDeduct tr
                                INNER JOIN PRM_EmploymentInfo EH ON tr.EmployeeId = EH.Id
                            WHERE EH.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(month))
                query += " AND tr.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND tr.SalaryYear LIKE '%" + year + "%'";
            
            var data = PGMUnit.OtehrAdjustDeductSearch.GetWithRawSql(query).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).AsQueryable();;

            return data.ToList();
        }
        #endregion
        
        public IList<BankAdviceLetterSearchModel> GetBankAdviceLetterSearchList(
            string filterExpression, string sortExpression, string sortDirection
            , int pageIndex, int pageSize, int pagesCount, bool isCount,
            string letterType, string year, string month, string referenceNo, string bankName
            )
        {
            var query = @"SELECT BL.Id
                            , BL.LetterType
                            , BL.SalaryYear
                            , BL.SalaryMonth
                            , BL.ReferenceNo
                            , BL.AccountNo
                            , BL.BankId
                            , BL.TotalAmount
                            , BN.bankName AS BankName
                            , BL.ZoneInfoId
                         FROM PGM_BankLetter AS BL INNER JOIN acc_BankInformation AS BN 
                         ON BL.BankId=BN.id WHERE 0=0";

            if (!string.IsNullOrEmpty(letterType))
                query += " AND BL.LetterType LIKE '%" + letterType + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND BL.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(month))
                query += " AND BL.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(referenceNo))
                query += " AND BL.ReferenceNo LIKE '%" + referenceNo + "%'";

            if (!string.IsNullOrEmpty(bankName))
                query += " AND BN.bankName LIKE '%" + bankName + "%'";

            var data = PGMUnit.BankAdviceLetterSearch.GetWithRawSql(query).AsQueryable();
            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<VehicleDeductionBillList> GetVehicleDeductionBillList()
        {
            var query = @"SELECT V.Id,V.SalaryYear,V.SalaryMonth,E.Id AS EmployeeId,E.EmpID,
		                E.FullName,E.EmployeeInitial,V.OfficalAmount,V.PersonalAmount
		                FROM PRM_EmploymentInfo AS E INNER JOIN PGM_VehicleDeduction AS V
		                ON E.Id=V.EmployeeId";

            var data = PGMUnit.VehicleDeductionBillList.GetWithRawSql(query);
            return data.ToList();
        }

        public IList<PGM_NightBillPaymentDetail> GetNightBillEmployeeList(int? departmentId, int? month, int? year)
        {
            var query = @"select 0 as Id, emp.EmpID as ICNo ,at.EmployeeId, emp.FullName as EmployeeName, deg.Name as Designation, at.TotalDays + 0.00 as TotalDays, 0.0 as PerDayRate, 
                        0.0 as RevenueStamp, 0.0 as TotalAmount, 0.0 as NetAmount, 0 as BillId, 'aaa' as IUser, 'aaa' as EUser, getdate() as IDate, getdate() as EDate
                         from 
                        PRM_EmploymentInfo emp join (
                        select EmployeeId, Count(AttendanceDate) as TotalDays
                        from [dbo].[AMS_AttendanceOverTimeProcess]
                        where month(AttendanceDate) = " + month + @" and year(AttendanceDate) = " + year + @" and (IsNightShift = 1 OR (ShiftInTime >= '00:00' and ShiftOutTime <= '08:00' ) )
                        Group by EmployeeId
                        ) at on emp.Id = at.EmployeeId
                        join PRM_Designation deg on deg.Id = emp.DesignationId  where emp.DivisionId = " + departmentId;

            var data = PGMUnit.NightBillPaymentDetailsRepository.GetWithRawSql(query);
            return data.ToList();
        }

        public IList<BankAdviceLetterSearchModel> CheckingBonus()
        {
            var query = @"select BM.*, BD.EmployeeId from PGM_BonusDetail as BD inner join PGM_Bonus as BM on BM.Id=BD.BonusId
                                        where BD.EmployeeId not in (
                                        select BLD.EmployeeId from PGM_BankLetter as BLM inner join PGM_BankLetterDetail as BLD
                                        on BLM.Id=BLD.BankLetterId
                                        where BLM.LetterType='Bonus')";

            var data = PGMUnit.BankAdviceLetterSearch.GetWithRawSql(query);
            return data.ToList();
        }


        public List<SalaryStructureCls> GetSalaryStructureList(
            string filterExpression,
            string sortExpression,
            string sortDirection,
            int pageIndex,
            int pageSize,
            int pagesCount,
            int? SalaryScaleId,
            int? GradeId,
            int? StepId,
            out int totalRecords
            )
        {
            var query = from c in PGMUnit.SalaryStructureRepository.Fetch()
                        join sd in PGMUnit.SalaryStructureDetailRepository.Fetch() on c.Id equals sd.SalaryStructureId
                        join sc in PGMUnit.SalaryScaleRepository.Fetch() on c.SalaryScaleId equals sc.Id
                        join jg in PGMUnit.JobGradeRepository.Fetch() on new { grade = c.GradeId, scale = sc.Id } equals new { grade = jg.Id, scale = jg.SalaryScaleId }
                        join js in PGMUnit.JobGradeStepRepository.Fetch() on new { step = c.StepId, grade = jg.Id } equals new { step = js.Id, grade = js.JobGradeId }
                        join sh in PGMUnit.SalaryHeadRepository.Fetch() on sd.HeadId equals sh.Id
                        where sh.IsBasicHead == true
                         && (SalaryScaleId == null || SalaryScaleId == 0 || c.SalaryScaleId == SalaryScaleId)
                         && (GradeId == null || GradeId == 0 || c.GradeId == GradeId)
                         && (StepId == null || StepId == 0 || c.StepId == StepId)

                        select new SalaryStructureCls()
                        {
                            ID = c.Id,
                            SalaryScaleId = sc.Id,
                            SalaryScaleName = sc.SalaryScaleName,
                            GradeId = c.GradeId,
                            GradeName = jg.GradeName,
                            StepId = c.StepId,
                            StepName = js.StepName,
                            Amount = sd.Amount
                        };
            var result = query.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            totalRecords = result.Count;

            return result;
        }


        public IList<SalaryMonthInfo> GetSalaryInfoList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount, string year, string month, int zoneId)
        {
            var query = @"SELECT DISTINCT SalaryYear, SalaryMonth, IsConfirmed 
                    FROM PGM_Salary PS
                        INNER JOIN PRM_EmploymentInfo E on PS.EmployeeId = E.Id
                    WHERE E.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(year))
                query += " AND SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(month))
                query += " AND SalaryMonth LIKE '%" + month + "%'";

            var data = PGMUnit.SalaryMonthList.GetWithRawSql(query).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<MonthlySalaryDetail> GetSalaryDetailInfoList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount
            ,String empId = ""
            , String year = ""
            , String month = ""
            , int DivisionId = 0
            , int EmploymentTypeId = 0
            , String EmployeeInitial = ""
            , bool? IsWithheld = null
            , int zoneId = 0)
        {
            var query = @"SELECT * FROM (SELECT Id
                                        , SalaryYear 
                                        , SalaryMonth 
                                        , EmployeeId
                                        , EmployeeInitial
                                        , DivisionId
                                        , EmploymentTypeId
                                        , EmpID
                                        , FullName
                                        , AccountNo
                                        , IsWithheld
                                        , IsPaid
                                        , SUM(GrossSal) AS GrossSal
                                        , SUM(TotalDeduction) AS TotalDeduction
                                        , SUM(NetPay) AS NetPay
                                        , SortingOrder
                                    FROM (
                                        SELECT Id,SalaryYear ,SalaryMonth , EmployeeId, EmployeeInitial, DivisionId, EmploymentTypeId, EmpID, FullName, AccountNo, IsWithheld, IsPaid, [Gross Salary] as GrossSal, [Total Deduction] as TotalDeduction, [Net Payable] as NetPay, SortingOrder
                                        FROM 
                                        (SELECT S.Id, S.SalaryYear, S.SalaryMonth, S.EmployeeId, E.EmployeeInitial, E.DivisionId, E.EmploymentTypeId, E.EmpID, E.FullName, E.BankAccountNo AS AccountNo, S.IsWithheld, S.IsPaid, SD.HeadName, SD.HeadAmount, S.IsConfirmed, D.SortingOrder
                                            FROM PGM_Salary S 
                                            INNER JOIN PRM_EmploymentInfo E ON S.EmployeeId = E.Id
                                            INNER JOIN PRM_Designation D ON E.DesignationId = D.Id
                                            INNER JOIN PGM_SalaryDetail SD ON S.Id=SD.SalaryId 
                                        WHERE E.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(empId))
                query += " AND EmpID = '" + empId + "'";

            if (!string.IsNullOrEmpty(year))
                query += " AND S.SalaryYear = '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += " AND S.SalaryMonth= '" + month + "'";

            query += @"AND SD.HeadName IN ('Gross Salary','Total Deduction','Net Payable')) p
                        PIVOT( SUM(HeadAmount) FOR HeadName IN ([Gross Salary],[Total Deduction],[Net Payable])) as Pvt
                    ) tbl
                    GROUP BY Id,SalaryYear ,SalaryMonth , EmployeeId, EmployeeInitial, DivisionId, EmploymentTypeId, EmpID, FullName, AccountNo, IsWithheld, IsPaid, SortingOrder
                    ) tbl WHERE 0=0 ";

            if (DivisionId != 0)
                query += " AND DivisionId=" + DivisionId;

            if (EmploymentTypeId != 0)
                query += " AND EmploymentTypeId=" + EmploymentTypeId;

            if (!string.IsNullOrEmpty(EmployeeInitial))
                query += " AND EmployeeInitial LIKE '%" + EmployeeInitial + "%'";

            if (IsWithheld == true)
                query += " AND IsWithheld=" + 1;

            if (IsWithheld == false)
                query += " AND IsWithheld=" + 0;

            query += " ORDER BY SortingOrder ASC ";

            var data = PGMUnit.MonthlySalaryDetailInfo.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(sortExpression))
                {
                    return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
                }
                else
                {
                    return data.Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();    
                }
            }
        }

        public IList<BankAdviceLetterSearchModel> GetEmployeeExistList(
            string year = "", string month = "", int empId = 0)
        {
            var query = @" SELECT 'A' AS EXIST FROM PGM_BankLetter 
                        B INNER JOIN PGM_BankLetterDetail BD ON B.Id=BD.BankLetterId
                        WHERE B.LetterType ='Salary' ";

            if (!string.IsNullOrEmpty(year))
                query += " AND  B.SalaryYear= '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += "  AND B.SalaryMonth= '" + month + "'";
            if (empId != 0)
            {
                query += " AND BD.EmployeeId= " + empId;

            }
            var data = PGMUnit.BankAdviceLetterSearch.GetWithRawSql(query);
            return data.ToList();
        }

        public decimal? GetYearMonthWiseEmpBasicSalary(
            string empId, string year, string month, int? employeeId = null)
        {
            string strSql = string.Empty;

            if (string.IsNullOrEmpty(empId) && employeeId == null)
                return null;

            if (employeeId == null || Convert.ToInt32(employeeId) == 0)
            {
                strSql = @"SELECT Id, EmpID, EmployeeInitial, FullName, 
                        dbo.fn_getBasicSalaryForBonus(id, '" + year + @"', '" + month + @"') as Basic
                        FROM PRM_EmploymentInfo
                        WHERE EmpId = '" + empId + "'";
            }
            else
            {
                strSql = @"SELECT Id, EmpID, EmployeeInitial, FullName, 
                        dbo.fn_getBasicSalaryForBonus(id, '" + year + @"', '" + month + @"') as Basic
                        FROM PRM_EmploymentInfo
                        WHERE Id = " + employeeId;
            }

            var data = PGMUnit.EmployeeBasicSalarySearch.GetWithRawSql(strSql);
            if (data != null && data.Count() > 0)
                return data.FirstOrDefault().Basic;

            return null;
        }


        public IList<SalaryMonthInfo> GetSalaryDistributionInfoList(
            string filterExpression, string sortExpression, string sortDirection
            , int pageIndex, int pageSize, int pagesCount, bool isCount,
            string year, string month)
        {
            var query = @"select distinct EmployeeId, SalaryYear, SalaryMonth from PGM_SalaryDistribution WHERE 0=0";

            if (!string.IsNullOrEmpty(year))
                query += " AND SalaryYear='" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += " AND SalaryMonth='" + month + "'";

            var data = PGMUnit.SalaryMonthList.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<MonthlySalaryDistributionDetaiCustomModell> GetSalaryDistributionDetailInfoList(
            string year = "", string month = "", string initial = "", int divisionId = 0, int projectId = 0)
        {
            var query = @"  SELECT DM.SalaryId
                                , DM.EmployeeId                         
                                , E.EmpID
                                , E.EmployeeInitial
                                , E.FullName
                                , E.DivisionId
                                , DD.ProjectId
                                , DM.GrossSalary
                                , DM.TotalHours
                                , DM.RatePerHour                            
                                ,P.ProjectNo
                                ,DD.ProjectHour
                                ,DD.ProjectAmount
                            FROM PGM_SalaryDistribution DM 
                            INNER JOIN PGM_SalaryDistributionDetail DD ON DD.SalaryId=DM.SalaryId
                            INNER JOIN PRM_EmploymentInfo E ON E.Id =DM.EmployeeId
                            INNER JOIN PIM_ProjectInfo P ON P.Id =DD.ProjectId WHERE 0=0";

            if (!string.IsNullOrEmpty(year))
                query += " AND DM.SalaryYear= '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += " AND DM.SalaryMonth = '" + month + "'";

            if (!string.IsNullOrEmpty(initial))
                query += " AND E.EmployeeInitial LIKE '%" + initial + "%'";

            if (divisionId != 0)
                query += " AND E.DivisionId=" + divisionId;

            if (projectId != 0)
                query += " AND DD.ProjectId= " + projectId;


            var data = PGMUnit.SalaryDistributionSearch.GetWithRawSql(query);
            return data.ToList();
        }

        public IList<IncomeTaxComputationCustomModel> GetIncomeTaxInfoList(
            string filterExpression, string sortExpression, string sortDirection
            , int pageIndex, int pageSize, int pagesCount, bool isCount, string Incomeyear, int zoneId)
        {
            var query = @"SELECT 
                            IT.Id
                            , IT.TaxRuleId
                            , TR.IncomeYear
                            , TR.AssessmentYear
                        FROM PGM_EmpTax IT INNER JOIN PGM_TaxRule TR ON IT.TaxRuleId = TR.Id
                            INNER JOIN ufnGetEmployeeFromHistoryForPGM(NULL
                                    , NULL, getdate(), " + zoneId + @", NULL, NULL
                                    , NULL, NULL, NULL, NULL, NULL, NULL, NULL)  EH ON IT.EmployeeId = EH.EmployeeId
                        WHERE 0 = 0";

            if (!string.IsNullOrEmpty(Incomeyear))
                query += " AND TR.IncomeYear='" + Incomeyear + "'";

            var data = PGMUnit.IncomeTaxComputationSearch.GetWithRawSql(query).AsQueryable();
            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<IncomeTaxComputationCustomModel> GetIncomeTaxDetailInfoList(
            string Incomeyear = "", string AssessmentYear = "", string initial = ""
            , int divisionId = 0, int empCategoryId = 0, int DesignationId = 0, int? zoneInfoId = 0)
        {
            var query = @"SELECT  EIT.Id
                                , EIT.TaxRuleId
                                , EIT.EmployeeId
                                , EM.EmpID
                                , TR.IncomeYear
                                , TR.AssessmentYear
                                , EM.EmployeeInitial
                                , EM.EmploymentTypeId
                                , EM.DesignationId
                                , EM.DivisionId
                                , EM.FullName
                                , EIT.TaxableIncome
                                , EIT.TaxLiability
                                , EIT.InvestmentRebate
                                , EIT.TaxPayable
                                , EIT.TaxPerMonth
                                , EIT.TaxDeducted
                                , EIT.TaxDue 
                        FROM PGM_EmpTax EIT INNER JOIN 
                            PRM_EmploymentInfo EM ON EIT.EmployeeId=EM.Id INNER JOIN 
                            PGM_TaxRule TR ON EIT.TaxRuleId=TR.Id 
                        WHERE 0=0";

            if (!string.IsNullOrEmpty(Incomeyear))
                query += " AND TR.IncomeYear= '" + Incomeyear + "'";

            if (!string.IsNullOrEmpty(AssessmentYear))
                query += " AND TR.AssessmentYear = '" + AssessmentYear + "'";

            if (!string.IsNullOrEmpty(initial))
                query += " AND EM.EmployeeInitial LIKE '%" + initial + "%'";

            if (divisionId != 0)
                query += " AND EM.DivisionId=" + divisionId;

            if (empCategoryId != 0)
                query += " AND EM.EmploymentTypeId= " + empCategoryId;

            if (DesignationId != 0)
                query += " AND EM.DesignationId= " + DesignationId;

            query += " AND EM.ZoneInfoId= " + zoneInfoId;

            var data = PGMUnit.IncomeTaxComputationSearch.GetWithRawSql(query);
            return data.ToList();
        }

        public IList<BonusTypeCustomModel> GetBonusTypeList(
            string filterExpression, string sortExpression, string sortDirection
            , int pageIndex, int pageSize, int pagesCount, bool isCount)
        {
            var query = @"SELECT BT.Id, BT.BonusType,ISNULL(R.Name,'All') AS Religion,BT.IsTaxable
                        FROM PGM_BonusType BT 
                        LEFT JOIN PRM_Religion R ON BT.ReligionId=R.Id WHERE 0=0";

            var data = PGMUnit.BonusTypeCustopmSearch.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<IncomeTaxComputationCustomModel> GetTaxProcessedEmployeeList(int empId = 0)
        {
            var query = @"select * from PGM_EmpTax ";

            if (empId != 0)
            {
                query += " where EmployeeId=" + empId;
            }

            var data = PGMUnit.IncomeTaxComputationSearch.GetWithRawSql(query);
            return data.ToList();
        }

        public bool IsSalaryProcessExist(int pEmployeeId, string pSalaryYear, string pSalaryMonth)
        {
            if (String.IsNullOrEmpty(pSalaryYear)) pSalaryYear = String.Empty;
            if (String.IsNullOrEmpty(pSalaryMonth)) pSalaryMonth = String.Empty;

            var queryList = (from sal in PGMUnit.SalaryMasterRepository.Get(t => t.SalaryYear == pSalaryYear && t.SalaryMonth == pSalaryMonth)
                             select sal).ToList();

            if (queryList != null && queryList.Count != 0 && pEmployeeId != 0)
            {
                queryList = queryList.Where(t => t.EmployeeId == pEmployeeId).ToList();
            }

            if (queryList.Count() > 0) return true;

            return false;
        }

        public string GetFiscalYearInBengali(string month, string year)
        {
            string fiscalYear = string.Empty;


            if ((month.ToLower() == "january") || (month.ToLower() == "february") || (month.ToLower() == "march") || (month.ToLower() == "april") || (month.ToLower() == "may") || (month.ToLower() == "june"))
            {
                int fYear = Convert.ToInt32(year);
                string lYear = year.Substring(year.Length - 2);
                lYear = ConvertEnglishDigitToBangla(Convert.ToInt32(lYear));
                fiscalYear = ConvertEnglishDigitToBangla((fYear - 1)) + "-" + lYear;
            }
            else
            {
                string lYear = year.Substring(year.Length - 2);
                lYear = ConvertEnglishDigitToBangla(Convert.ToInt32(lYear));
                int lastYear = Convert.ToInt32(lYear);
                fiscalYear = year + "-" + ConvertEnglishDigitToBangla((lastYear + 1));
            }
            return fiscalYear;
        }

        public string ConvertEnglishDigitToBangla(Int64 number)
        {
            string bengaliNumber = string.Concat(number.ToString().Select(c => (char)('\u09E6' + c - '0')));
            return bengaliNumber;
        }

        public string GetAmountInBengaliWords(decimal Amount)
        {
            var query = @"SELECT InWord = [dbo].[FN_BengaliNumberName](" + Amount + ")";

            var data = PGMUnit.AmountInBengaliWords.GetWithRawSql(query);
            return data.FirstOrDefault().InWord;
        }

        public IList<PRM_JobGrade> GetLatestJobGrade()
        {
            var maxSalaryScale = PGMUnit.SalaryScaleRepository.Get(t => t.DateOfEffective <= DateTime.Now).OrderByDescending(t => t.DateOfEffective).FirstOrDefault();

            if (maxSalaryScale == null) return null;

            var q = PGMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == maxSalaryScale.Id);
            return q.ToList();
        }
    }


    public class SalaryStructureCls
    {
        public Int32 ID { get; set; }
        public int SalaryScaleId { get; set; }
        public string SalaryScaleName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public int StepId { get; set; }
        public int? StepName { get; set; }
        public decimal Amount { get; set; }
    }
}


