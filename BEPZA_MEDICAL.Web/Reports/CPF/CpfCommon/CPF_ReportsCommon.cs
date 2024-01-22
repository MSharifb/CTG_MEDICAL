using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace BEPZA_MEDICAL.Web.Reports.CPF.CpfCommon
{


    public class CPF_ReportsCommon : ReportBase
    {

        /*

         1. Individual Monthly PF Statement
         2. Monthly PF Statement

         3. Monthly PF and Loan Statement 1
         4. Monthly PF and Loan Statement 2

         5. Monthly Refundable Loan Statement
         6. Monthly Non-Refundable Loan Statement

         7. Individual Loan Collection Statement 1
         8. Individual Loan Collection Statement 2

         9. PF Membership Application Form
         10. PF Nominee Form

         11. My PF Statement

         12. PFStatementHistory
         */

        public string GetReportName(CPFEnum.CPFReports report)
        {
            switch (report)
            {
                case CPFEnum.CPFReports.Individual_Monthly_PF_Statement:
                    return "Individual Monthly PF Statement";
                case CPFEnum.CPFReports.My_PF_Statement:
                    return "My PF Statement";
                case CPFEnum.CPFReports.Monthly_PF_Statement:
                    return "Monthly PF Statement";
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_1:
                    return "Monthly PF and Loan Statement 1";
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_2:
                    return "Monthly PF and Loan Statement 2";
                case CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement:
                    return "Monthly Refundable Loan Statement";
                case CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement:
                    return "Monthly Non-Refundable Loan Statement";
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1:
                    return "Individual Loan Collection Statement 1";
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2:
                    return "Individual Loan Collection Statement 2";
                case CPFEnum.CPFReports.PF_Membership_Application_Form:
                    return "PF Membership Application Form";
                case CPFEnum.CPFReports.PF_Nominee_Form:
                    return "PF Nominee Form";
                case CPFEnum.CPFReports.PF_Membership_Active_Inactive:
                    return "Membership Information";
                case CPFEnum.CPFReports.PF_Statement_History:
                    return "PF Statement History";

                default:
                    return string.Empty;
            }
        }

        public string GetReportPath(CPFEnum.CPFReports report)
        {
            switch (report)
            {
                case CPFEnum.CPFReports.Individual_Monthly_PF_Statement:
                    return "~/Reports/CPF/rdlc/Individual_Monthly_PF_Statement.rdlc";
                case CPFEnum.CPFReports.My_PF_Statement:
                    return "~/Reports/CPF/rdlc/Individual_Monthly_PF_Statement.rdlc";
                case CPFEnum.CPFReports.Monthly_PF_Statement:
                    return "~/Reports/CPF/rdlc/Monthly_PF_Statement_01.rdlc";
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_1:
                    return "~/Reports/CPF/rdlc/Monthly_PF_and_Loan_Statement_1.rdlc";
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_2:
                    return "~/Reports/CPF/rdlc/Monthly_PF_and_Loan_Statement_2.rdlc";
                case CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement:
                    return "~/Reports/CPF/rdlc/Monthly_Loan_Statement.rdlc";
                case CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement:
                    return "~/Reports/CPF/rdlc/Monthly_Loan_Statement.rdlc";
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1:
                    return "~/Reports/CPF/rdlc/Individual_Loan_Collection_Statement_1.rdlc";
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2:
                    return "~/Reports/CPF/rdlc/Individual_Loan_Collection_Statement_2.rdlc";
                case CPFEnum.CPFReports.PF_Membership_Application_Form:
                    return "~/Reports/CPF/rdlc/PF_Membership_Application_Form.rdlc";
                case CPFEnum.CPFReports.PF_Nominee_Form:
                    return "~/Reports/CPF/rdlc/PF_Nominee_Form.rdlc";
                case CPFEnum.CPFReports.PF_Loan_Application_Form:
                    return "~/Reports/CPF/rdlc/PF_Loan_Application_Form.rdlc";
                case CPFEnum.CPFReports.PF_Membership_Active_Inactive:
                    return "~/Reports/CPF/rdlc/MembershipActiveInactive.rdlc";
                case CPFEnum.CPFReports.PF_Statement_History:
                    return "~/Reports/CPF/rdlc/Individual_Monthly_PF_Statement.rdlc";

                default:
                    return string.Empty;
            }
        }

        public string GetReportDataSetName(CPFEnum.CPFReports report)
        {
            switch (report)
            {
                case CPFEnum.CPFReports.Individual_Monthly_PF_Statement:
                    return "DS_MonthlyPFStatement";
                case CPFEnum.CPFReports.My_PF_Statement:
                    return "DS_MonthlyPFStatement";
                case CPFEnum.CPFReports.Monthly_PF_Statement:
                    return "DS_CpfRpt_MonthlyPFStatement";
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_1:
                    return "DS_CpfRpt_MonthlyPFandLoanStatement1";
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_2:
                    return "DS_CpfRpt_MonthlyPFandLoanStatement2";
                case CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement:
                    return "DS_CpfRpt_MonthlyLoanStatement";
                case CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement:
                    return "DS_CpfRpt_MonthlyLoanStatement";
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1:
                    return "DS_CpfRpt_IndividualLoanCollectionStatement1";
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2:
                    return "DS_CpfRpt_IndividualLoanCollectionStatement2";
                case CPFEnum.CPFReports.PF_Membership_Application_Form:
                    return "DS_RptCpfMemberInformation";
                case CPFEnum.CPFReports.PF_Nominee_Form:
                    return "DS_CpfRpt_PFNomineeForm";
                case CPFEnum.CPFReports.PF_Loan_Application_Form:
                    return "MyCPFLoanApplication";
                case CPFEnum.CPFReports.PF_Membership_Active_Inactive:
                    return "DS_MembershipActiveInactive";
                case CPFEnum.CPFReports.PF_Statement_History:
                    return "DS_MonthlyPFStatement";

                default:
                    return string.Empty;
            }
        }

        public dynamic GetReportData(CPFEnum.CPFReports report, string fromMonth, string toMonth, string fromYear, string toYear, int employeeId, bool IsRefundable, bool loanStatus, bool IsInactive, IEnumerable<BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo> zoneList, ObjectParameter numErrorCode, ObjectParameter strErrorMsg)
        {
            dynamic data;

            switch (report)
            {
                case CPFEnum.CPFReports.Individual_Monthly_PF_Statement:
                    data = cpfContext.CPF_SP_RptIndividualMonthlyPFStatement(fromYear, fromMonth, employeeId, numErrorCode, strErrorMsg).ToList();

                    return data;
                case CPFEnum.CPFReports.My_PF_Statement:
                    data = cpfContext.CPF_SP_RptIndividualMonthlyPFStatement(fromYear, fromMonth, employeeId, numErrorCode, strErrorMsg).ToList();

                    return data;
                case CPFEnum.CPFReports.Monthly_PF_Statement:
                    data = cpfContext.CPF_SP_RptMonthlyPFStatementMaster(fromYear, fromMonth, 0, numErrorCode, strErrorMsg).ToList();
                    //data = result.Where(r => r.SalaryMonth == fromMonth && r.SalaryYear == fromYear && zoneList.Any(z => z.Id.Equals(r.ZoneInfoId))).ToList();
                    return data;
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_1:
                    var result1 = cpfContext.CPF_SP_RptMonthlyPFStatement(fromYear, fromMonth, 0, numErrorCode, strErrorMsg).ToList();
                    data = result1.Where(r => r.SalaryMonth == fromMonth && r.SalaryYear == fromYear && zoneList.Any(z => z.Id.Equals(r.ZoneInfoId))).ToList();

                    return data;
                case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_2:
                    var result2 = cpfContext.CPF_SP_RptMonthlyPFStatement(fromYear, fromMonth, 0, numErrorCode, strErrorMsg).ToList();
                    data = result2.Where(r => r.SalaryMonth == fromMonth && r.SalaryYear == fromYear && zoneList.Any(z => z.Id.Equals(r.ZoneInfoId))).ToList();

                    return data;
                case CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement:
                    data = cpfContext.CPF_SP_PFListOfLoan(fromYear, fromMonth, null, IsRefundable, loanStatus, numErrorCode, strErrorMsg).ToList();
                    //TODO: Add filtering for Running/Closed loan and Refundable/Non-Refundable in ListOfLoan_SP and ADD designation column

                    return data;
                case CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement:
                    data = cpfContext.CPF_SP_PFListOfLoan(fromYear, fromMonth, null, IsRefundable, loanStatus, numErrorCode, strErrorMsg).ToList();

                    return data;
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1:
                    data = cpfContext.CPF_SP_RptMonthlyPFStatement(fromYear, fromMonth, employeeId, numErrorCode, strErrorMsg).ToList();

                    return data;
                case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2:
                    data = cpfContext.CPF_SP_PFListOfLoan(fromYear, fromMonth, employeeId, null, loanStatus, numErrorCode, strErrorMsg).ToList();

                    return data;
                case CPFEnum.CPFReports.PF_Membership_Application_Form:
                    data = cpfContext.CPF_RptCpfMemberInformation(employeeId).ToList();

                    return data;
                case CPFEnum.CPFReports.PF_Nominee_Form:
                    data = cpfContext.CPF_SP_RptNomineeFormInformation(employeeId, numErrorCode, strErrorMsg).ToList();

                    return data;

                case CPFEnum.CPFReports.PF_Loan_Application_Form:
                    data = cpfContext.CPF_GetMyCPFLoanApplication(employeeId).ToList();

                    return data;
                case CPFEnum.CPFReports.PF_Membership_Active_Inactive:
                    data = cpfContext.CPF_SP_RptNewJoinInMembership(fromYear, fromMonth, IsInactive).ToList();

                    return data;

                case CPFEnum.CPFReports.PF_Statement_History:
                    data = cpfContext.CPF_SP_RptPFStatementHistory(fromYear, fromMonth, employeeId, numErrorCode, strErrorMsg).ToList();
                    return data;

                default:
                    return null;
            }
        }


    } // End of class
}