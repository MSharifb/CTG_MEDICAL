using System;
using System.Collections.Generic;
using System.Linq;

using BEPZA_MEDICAL.DAL.FAM;
using BEPZA_MEDICAL.DAL.Infrastructure;
using BEPZA_MEDICAL.DAL.FAM.CustomEntities;


namespace BEPZA_MEDICAL.Domain.FAM
{
    public class FAMCommonService
    {
        #region Fields

        FAM_UnitOfWork _famUnit;

        #endregion

        #region Ctor

        public FAMCommonService(FAM_UnitOfWork uow)
        {
            _famUnit = uow;
        }

        #endregion

        #region Properties

        public FAM_UnitOfWork FAMUnit { get { return _famUnit; } }

        #endregion

        #region Workflow method

        public IList<BankSearch> GetBankSearchedList(string bankName = "")
        {
            var query = @"SELECT bm.Id Id, bn.Name BankName, bc.BankAccountNo BankAc, coa.AccountHeadName AccountHead, bm.SWIFTCode SWIFTCode, bm.Address BankAddr 
                            FROM dbo.FAM_BankBranchMap bm
                            LEFT JOIN dbo.FAM_BankBranchAccountNo bc ON bm.Id = bc.BankBranchMapId
                            LEFT JOIN dbo.FAM_ChartOfAccount coa ON bc.AccountHeadId = coa.Id
                            LEFT JOIN dbo.PRM_BankName bn ON bm.BankId = bn.Id";
            if (!string.IsNullOrEmpty(bankName))
                query += " WHERE bn.Name LIKE '%" + bankName + "%'";

            var data = FAMUnit.BankSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<DivisionSearch> GetDivisionSearchedList(int? DivisionUnitId = 0)
        {
            var query = @"SELECT DISTINCT DV.Id Id, DV.Name DivisionUnitName
                            FROM dbo.PRM_Division DV
                            INNER JOIN dbo.FAM_DivisionUnitBudgetHead DB ON DV.Id = DB.DivisionUnitId";

            if ((DivisionUnitId != null && DivisionUnitId != 0))
                query += " WHERE DV.Id = '"+ DivisionUnitId +"'";

            var data = FAMUnit.DivisionSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<SpecialAccountAssignmentSearch> GetSpecialAccountAssignmentSearchedList(int? PurposeId = 0)
        {
            var query = @"SELECT DISTINCT PAM.Id Id, P.Id PurposeId, P.Name PurposeName, CA.AccountHeadName AccountHeadName, PAM.Remarks 
                            FROM dbo.FAM_Purpose P
                            INNER JOIN dbo.FAM_PurposeAccountHeadMapping PAM ON P.Id = PAM.PurposeId
                            INNER JOIN DBO.FAM_ChartOfAccount CA ON CA.Id = PAM.AccountHeadId ";

            if ((PurposeId != null && PurposeId != 0))
                query += " WHERE P.Id = '" + PurposeId + "'";

            var data = FAMUnit.SpecialAccountAssignmentSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<DivisionFinancialYearSearch> GetDivisionFYSearchedList(int? DivisionUnitId = 0, int? FinancialYearId = 0)
        {
            var query = @"SELECT DISTINCT DB.Id, DV.Id DivisionUnitId, DV.Name DivisionUnitName, FY.Id FinancialYearId, FY.FinancialYearName FinancialYearName
                            FROM dbo.PRM_Division DV
                            INNER JOIN dbo.FAM_DivisionUnitBudgetAllocation DB ON DV.Id = DB.DivisionUnitId
                            INNER JOIN dbo.FAM_FinancialYearInformation FY ON FY.Id = DB.FinancialYearId";

            if (((FinancialYearId != null && FinancialYearId != 0)) && ((DivisionUnitId != null && DivisionUnitId != 0)))
            {
                query += " WHERE FY.Id = '" + FinancialYearId + "' AND DV.Id = '" + DivisionUnitId + "'";
            }
            
            else
            {
                if ((FinancialYearId != null && FinancialYearId != 0))
                    query += " WHERE FY.Id = '" + FinancialYearId + "'";

                if ((DivisionUnitId != null && DivisionUnitId != 0))
                    query += " WHERE DV.Id = '" + DivisionUnitId + "'";
            }
            

            var data = FAMUnit.DivisionFinancialYearSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<DivisionUnitFinancialYearRevisionSearch> GetDivisionUnitFYRevisionSearchList(int? DivisionUnitId = 0, int? FinancialYearId = 0)
        {
            var query = @"SELECT BR.Id, DV.Id DivisionUnitId, DV.Name DivisionUnitName, FY.Id FinancialYearId, 
                            FY.FinancialYearName FinancialYearName, BR.RevisionNo RevisionNo
                            FROM dbo.PRM_Division DV
                            INNER JOIN dbo.FAM_DivisionUnitBudgetRevision BR ON DV.Id = BR.DivisionUnitId
                            INNER JOIN dbo.FAM_FinancialYearInformation FY ON FY.Id = BR.FinancialYearId";

            if (((FinancialYearId != null && FinancialYearId != 0)) && ((DivisionUnitId != null && DivisionUnitId != 0)))
            {
                query += " WHERE FY.Id = '" + FinancialYearId + "' AND DV.Id = '" + DivisionUnitId + "'";
            }

            else
            {
                if ((FinancialYearId != null && FinancialYearId != 0))
                    query += " WHERE FY.Id = '" + FinancialYearId + "'";

                if ((DivisionUnitId != null && DivisionUnitId != 0))
                    query += " WHERE DV.Id = '" + DivisionUnitId + "'";
            }

            var data = FAMUnit.DivisionUnitFinancialYearRevisionSearch.GetWithRawSql(query);

            return data.ToList();
        }






        public IList<CentralBudgetFinancialYearSearch> GetCentralBudgetFYSearchedList(int? FinancialYearId = 0, string UserName = "",
            int userEmployeeId = 0)
        {
            var query = @"SELECT DISTINCT CB.Id, FY.FinancialYearName FinancialYearName, CB.ApprovalStatus, CB.Remarks Remarks
                            FROM dbo.FAM_FinancialYearInformation FY
                            INNER JOIN dbo.FAM_CentralBudgetAllocation CB ON FY.Id = CB.FinancialYearId
                            WHERE (CB.IUser = '" + UserName + "' OR CB.CurrentApprovalNodeId = " + userEmployeeId + ")";

            if ((FinancialYearId != null && FinancialYearId != 0))
                query += " AND FY.Id = '" + FinancialYearId + "'";

            var data = FAMUnit.CentralBudgetFinancialYearSearch.GetWithRawSql(query);

            return data.ToList();
        }
        public IList<CentralBudgetFinancialYearSearch> GetCentralBudgetFYRecommendApproveSearchedList(int? FinancialYearId = 0, int? userId = 0)
        {
            var query = @"SELECT DISTINCT CB.Id, FY.FinancialYearName FinancialYearName, CB.Remarks Remarks
                            FROM dbo.FAM_FinancialYearInformation FY
                            INNER JOIN dbo.FAM_CentralBudgetAllocation CB ON FY.Id = CB.FinancialYearId
                            INNER JOIN dbo.FAM_ApprovalPathNodeInfo AP ON AP.NodeEmpId = CB.CurrentApprovalNodeId
                            WHERE CB.CurrentApprovalNodeId = '"+ userId +"' ";

            //if ((FinancialYearId != null && FinancialYearId != 0))
            //    query += " WHERE FY.Id = '" + FinancialYearId + "'";

            if ((FinancialYearId != null && FinancialYearId != 0))
                query += " AND FY.Id = '" + FinancialYearId + "'";




            var data = FAMUnit.CentralBudgetFinancialYearSearch.GetWithRawSql(query);

            return data.ToList();
        }








        public IList<RevisionOfCentralBudgetFYSearch> GetRevisionOfCentralBudgetFYSearchedList(int? FinancialYearId = 0)
        {
            var query = @"SELECT DISTINCT RB.Id, FY.FinancialYearName FinancialYearName, RB.RevisionNo, RB.Remarks Remarks
                            FROM dbo.FAM_FinancialYearInformation FY
                            INNER JOIN dbo.FAM_CentralBudgetRevision RB ON FY.Id = RB.FinancialYearId";

            if ((FinancialYearId != null && FinancialYearId != 0))
                query += " WHERE FY.Id = '" + FinancialYearId + "'";

            var data = FAMUnit.RevisionOfCentralBudgetFYSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<CentralBudgetAllocationFYSearch> GetCentralBudgetAllocationFYSearchList(int? FinancialYearId = 0)
        {
            var query = @"SELECT dtl.AccountHeadId,sum(dtl.RevisedAmount) as Amount,coa.AccountHeadCode,coa.AccountHeadName,coa.AccountHeadType
                            FROM dbo.FAM_DivisionUnitBudgetAllocation d
                            INNER JOIN dbo.FAM_DivisionUnitBudgetAllocationDetails dtl ON d.Id = dtl.DivisionUnitBudgetId
                            INNER JOIN FAM_ChartOfAccount coa on dtl.AccountHeadId = coa.Id
                            WHERE d.FinancialYearId = '" + FinancialYearId + "' group by dtl.AccountHeadId,coa.AccountHeadCode,coa.AccountHeadName,coa.AccountHeadType Order by coa.AccountHeadType desc";

            //if ((FinancialYearId != null && FinancialYearId != 0))
            //    query += " WHERE FY.Id = '" + FinancialYearId + "'";

            var data = FAMUnit.CentralBudgetAllocationFYSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<RevisionCentralBudgetByFYSearch> GetRevisionCentralBudgetAllocationFYSearchList(int? FinancialYearId = 0)
        {
            var query = @"SELECT dtl.AccountHeadId,dtl.RevisedAmount,coa.AccountHeadCode,coa.AccountHeadName,coa.AccountHeadType
                            FROM dbo.FAM_CentralBudgetAllocation d
                            INNER JOIN dbo.FAM_CentralBudgetAllocationDetails dtl ON d.Id = dtl.CentralBudgetId
                            INNER JOIN FAM_ChartOfAccount coa on dtl.AccountHeadId = coa.Id
                            WHERE d.FinancialYearId = '" + FinancialYearId + "' Order by coa.AccountHeadType desc";

            //if ((FinancialYearId != null && FinancialYearId != 0))
            //    query += " WHERE FY.Id = '" + FinancialYearId + "'";

            var data = FAMUnit.RevisionCentralBudgetByFYSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<DivisionUnitBudgetByDUFYSearch> GetRevisionDivisionUnitBudgetByDUFYSearchList(int? DivisionUnitId = 0, int? FinancialYearId = 0)
        {
            var query = @"SELECT dtl.AccountHeadId,dtl.RevisedAmount,coa.AccountHeadCode,coa.AccountHeadName,coa.AccountHeadType
                            FROM dbo.FAM_DivisionUnitBudgetAllocation d
                            INNER JOIN dbo.FAM_DivisionUnitBudgetAllocationDetails dtl ON d.Id = dtl.DivisionUnitBudgetId
                            INNER JOIN FAM_ChartOfAccount coa on dtl.AccountHeadId = coa.Id
                            WHERE d.FinancialYearId = '" + FinancialYearId + "' AND d.DivisionUnitId = '" + DivisionUnitId + "' Order by coa.AccountHeadType desc";

            //if ((FinancialYearId != null && FinancialYearId != 0))
            //    query += " WHERE FY.Id = '" + FinancialYearId + "'";

            var data = FAMUnit.DivisionUnitBudgetByDUFYSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<ChequeSearch> GetChequeSearchedList(string bankName = "", string chequeNo = "")
        {
            var query = @"SELECT DISTINCT cm.Id Id, b.Name BankName, b.Id BankId, bc.BankAccountNo BankAc, cm.ChequeBookNumber ChequeBookNumber
                            FROM dbo.FAM_ChequeBookInfo cm
                            LEFT JOIN dbo.FAM_ChequeInfo cd ON cm.Id = cd.ChequeBookId
                            LEFT JOIN dbo.FAM_BankBranchAccountNo bc ON cm.BankAccountId = bc.Id
                            LEFT JOIN dbo.FAM_BankBranchMap bm ON bc.BankBranchMapId = bm.Id
                            LEFT JOIN dbo.PRM_BankName b ON bm.BankId = b.Id WHERE 1 = 1 ";
            if (!string.IsNullOrEmpty(bankName) && !bankName.Equals("0"))
                query += " AND b.Id = "+ bankName;
            if (!string.IsNullOrEmpty(chequeNo))
                query += " AND cm.ChequeBookNumber LIKE '%"+ chequeNo +"%' ";

            var data = FAMUnit.ChequeSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IQueryable<BSPLReportHeadMappingSearch> GetReportBSPLHeadSearchedList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount,bool isCount, int? ReportId = 0, int? BSPLReportHeadId = 0)
        {
            var query = @"SELECT DISTINCT BHM.Id, RN.Id ReportId, RN.Name ReportName, BN.Id BSPLReportHeadId, BN.Name BSPLReportHeadName, BHM.ReportHeadSerial
                            FROM dbo.FAM_ReportName RN
                            INNER JOIN dbo.FAM_BSPLReportHeadMapping BHM ON RN.Id = BHM.ReportId
                            INNER JOIN dbo.FAM_BSPLReportHeadName BN ON BN.Id = BHM.BSPLReportHeadId";

            if (((BSPLReportHeadId != null && BSPLReportHeadId != 0)) && ((ReportId != null && ReportId != 0)))
            {
                query += " WHERE BN.Id = '" + BSPLReportHeadId + "' AND RN.Id = '" + ReportId + "'";
            }

            else
            {
                if ((BSPLReportHeadId != null && BSPLReportHeadId != 0))
                    query += " WHERE BN.Id = '" + BSPLReportHeadId + "'";

                if ((ReportId != null && ReportId != 0))
                    query += " WHERE RN.Id = '" + ReportId + "'";
            }


            var data = FAMUnit.BSPLReportHeadMappingSearchRepository.GetWithRawSql(query).AsQueryable();

          //  return data.ToList();
            if (isCount)
            {
                return data;
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize); ;
            }
        }

//        public IList<VoucherSearch> GetVoucherSearchedList(
//            int projectId = 0,
//            string voucherNumber = "",
//            DateTime? voucherDateTo = null,
//            DateTime? voucherDateFrom = null,
//            string payee = "",
//            int? divId = 0,
//            string voucherStatus = "",
//            int voucherTypeId = 0,
//            int clientId = 0,
//            string PendingApproval = ""
//              )
//        {
//            var query = @"SELECT DISTINCT
//                                vi.Id,
//	                            pi.Id ProjectId, 
//	                            pi.ProjectTitle, 
//	                            vi.VoucherNumber, 
//	                            vi.VoucherDate,
//	 
//	                            vi.PayeeClientId,
//	                            client.ClientName ClientName, 
//	                            vi.PayeeStaffId,
//	                            staff.FullName StaffName,
//	                            vi.PayeeVendorId,
//	                            vendor.VendorName VendorName,
//	
//	                            vi.ReceivedBy,
//	                            vi.ReceiveFromClientId,
//	                            vi.ReceiveFromStaffId,
//	                            vi.ReceiveFromVendorId,
//	
//	                            div.Id DivisionId,
//	                            div.Name DivisionName,
//	
//	                            vi.VoucherStatus,	
//	                            vi.VoucherTypeId,
//	                            vt.Name VoucherTypeName,
//	
//	                            node.FullName CurrentLocation
//	
//                            FROM dbo.FAM_VoucherInfo vi
//                            LEFT JOIN dbo.FAM_VoucherInfoDetails vid ON vi.Id = vid.VoucherId
//                            LEFT JOIN dbo.PIM_ProjectInfo [pi] ON vid.ProjectId = pi.Id
//                            LEFT JOIN dbo.FAM_VoucherType vt ON vi.VoucherTypeId = vt.Id
//                            LEFT JOIN dbo.PRM_Division div ON pi.DivisionId = div.Id
//                            LEFT JOIN dbo.PRM_EmploymentInfo staff ON vi.PayeeStaffId = staff.Id
//                            LEFT JOIN dbo.PIM_ClientInfo client ON vi.PayeeClientId = client.Id
//                            LEFT JOIN dbo.FAM_VendorInformation vendor ON vi.PayeeVendorId = vendor.Id
//                            LEFT JOIN dbo.PRM_EmploymentInfo node ON vi.CurrentApprovalNodeId = node.Id
//
//                            WHERE 1=1";

//            if (projectId != 0)
//                query += " AND pi.Id = " + projectId;
            
//            if (!string.IsNullOrEmpty(voucherNumber))
//                query += " AND vi.VoucherNumber  LIKE '%" + voucherNumber + "%' ";
            
//            if(voucherDateTo.HasValue && voucherDateFrom.HasValue)
//                query += " AND ( vi.VoucherDate > '" + voucherDateFrom.Value.ToString("yyyy-MM-dd") + "' AND vi.VoucherDate < '" + voucherDateTo.Value.ToString("yyyy-MM-dd") + "' )";

//            if(!string.IsNullOrEmpty(payee))
//                query += " AND ( client.ClientName  LIKE '%" + payee + "%' OR staff.FullName  LIKE '%" + payee + "%' OR vendor.VendorName  LIKE '%" + payee + "%' )";

//            if (divId != null)
//                query += " AND div.Id = " + divId;

//            if (!string.IsNullOrEmpty(voucherStatus))
//                query += " AND vi.VoucherStatus = '" + voucherStatus + "'";

//            if (voucherTypeId != 0)
//                query += " AND vi.VoucherTypeId = " + voucherTypeId;

//            if (clientId != 0)
//                query += " AND vi.PayeeClientId = " + clientId;
            
//            var data = FAMUnit.VoucherSearch.GetWithRawSql(query);

//            return data.ToList();
//        }

        public IList<VoucherSearch> GetVoucherSearchedList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount,
            int projectId = 0,
            string voucherNumber = "",
            DateTime? voucherDateTo = null,
            DateTime? voucherDateFrom = null,
            string payee = "",
            int? divId = 0,
            string voucherStatus = "",
            int voucherTypeId = 0,
            int clientId = 0,
            string PendingApproval = "",
            string UserName="",
            int userEmployeeId=0
              )
        {
            var query = @"SELECT DISTINCT
                                vi.Id,
	                            pi.Id ProjectId, 
	                            pi.ProjectTitle, 
	                            vi.VoucherNumber, 
	                            vi.VoucherDate,
	 
	                            vi.PayeeClientId,
	                            client.ClientName ClientName, 
	                            vi.PayeeStaffId,
	                            staff.FullName StaffName,
	                            vi.PayeeVendorId,
	                            vendor.VendorName VendorName,
	
	                            vi.ReceivedBy,
	                            vi.ReceiveFromClientId,
	                            vi.ReceiveFromStaffId,
	                            vi.ReceiveFromVendorId,
	
	                            div.Id DivisionId,
	                            div.Name DivisionName,
	
	                            vi.VoucherStatus,	
	                            vi.VoucherTypeId,
	                            vt.Name VoucherTypeName,
	
	                            node.FullName CurrentLocation
	
                            FROM dbo.FAM_VoucherInfo vi
                            LEFT JOIN dbo.FAM_VoucherInfoDetails vid ON vi.Id = vid.VoucherId
                            LEFT JOIN dbo.PIM_ProjectInfo [pi] ON vid.ProjectId = pi.Id
                            LEFT JOIN dbo.FAM_VoucherType vt ON vi.VoucherTypeId = vt.Id
                            LEFT JOIN dbo.PRM_Division div ON pi.DivisionId = div.Id
                            LEFT JOIN dbo.PRM_EmploymentInfo staff ON vi.PayeeStaffId = staff.Id
                            LEFT JOIN dbo.PIM_ClientInfo client ON vi.PayeeClientId = client.Id
                            LEFT JOIN dbo.FAM_VendorInformation vendor ON vi.PayeeVendorId = vendor.Id
                            LEFT JOIN dbo.PRM_EmploymentInfo node ON vi.CurrentApprovalNodeId = node.Id
                            LEFT JOIN dbo.FAM_FinancialYearInformation FY ON vi.FinancialYearId = FY.Id
                            
                            WHERE 1=1 AND FY.IsActive = 1";

            if (projectId != 0)
                query += " AND pi.Id = " + projectId;

            if (!string.IsNullOrEmpty(voucherNumber))
                query += " AND vi.VoucherNumber  LIKE '%" + voucherNumber + "%' ";

            if (voucherDateTo.HasValue && voucherDateFrom.HasValue)
                query += " AND ( vi.VoucherDate > '" + voucherDateFrom.Value.ToString("yyyy-MM-dd") + "' AND vi.VoucherDate < '" + voucherDateTo.Value.ToString("yyyy-MM-dd") + "' )";

            if (!string.IsNullOrEmpty(payee))
                query += " AND ( client.ClientName  LIKE '%" + payee + "%' OR staff.FullName  LIKE '%" + payee + "%' OR vendor.VendorName  LIKE '%" + payee + "%' )";

            if (divId != null)
                query += " AND div.Id = " + divId;

            if (!string.IsNullOrEmpty(voucherStatus))
                query += " AND vi.VoucherStatus = '" + voucherStatus + "'";

            if (voucherTypeId != 0)
                query += " AND vi.VoucherTypeId = " + voucherTypeId;

            if (clientId != 0)
                query += " AND vi.PayeeClientId = " + clientId;

            if (UserName != null)
            {
                query += " AND (vi.IUser = '" + UserName + "' OR vi.CurrentApprovalNodeId = " + userEmployeeId +") ";
                //query += " AND (vi.IUser = '" + UserName + "' OR vi.CurrentApprovalNodeId = (SELECT Id FROM PRM_EmploymentInfo Emp WHERE Emp.EmpID = (SELECT EmpId FROM [IWM_MFSSecurity].[dbo].[TblUsers] WHERE LoginId = '" + UserName + "'))) ";

                
            }
            

            var data = FAMUnit.VoucherSearch.GetWithRawSql(query).AsQueryable();

            //return data.ToList();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<FAM_ChartOfAccount> GetCOA(bool isOnlyPostHead)
        {
            IList<FAM_ChartOfAccount> coalist = new List<FAM_ChartOfAccount>();

            if (isOnlyPostHead)
            {
                coalist = _famUnit.ChartOfAccount.Get(d => d.IsPostingAccount == true).OrderBy(d=>d.AccountHeadCode).ToList();
            }
            else
            {
                coalist = _famUnit.ChartOfAccount.GetAll().OrderBy(d=>d.AccountHeadCode).ToList();            
            }

            return coalist;
        }

        public string[] CurrentBalanceOfAccountHead(int companyId, int myAccountHeadId)
        {
           
            string[] query = _famUnit.FunctionRepository.CurrentBalanceOfAccountHead(companyId, myAccountHeadId);

            return query;
        }

//        public string RecommenderOrApprover(int? userEmpId, int? approvalPathId)
//        {
//            string query = @"SELECT DISTINCT ApprovalType
//                            FROM FAM_ApprovalPathNodeInfo WHERE NodeEmpId = " + userEmpId + " AND PathId= " + approvalPathId + "";
//            var data = FAMUnit.VoucherSearch.GetWithRawSql(query).AsQueryable();
//            return data;
//        }
        #endregion
    }
}
