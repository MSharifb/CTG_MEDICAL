using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Utility
{
    public static class CPFEnum
    {
        public enum ProfitRateType { Yearly = 1, Monthly };

        public enum CPFReports
        {

            Individual_Monthly_PF_Statement = 1,
            Monthly_PF_Statement,

            Monthly_PF_and_Loan_Statement_1,
            Monthly_PF_and_Loan_Statement_2,

            Monthly_Refundable_Loan_Statement,
            Monthly_Non_Refundable_Loan_Statement,

            Individual_Loan_Collection_Statement_1,
            Individual_Loan_Collection_Statement_2,

            PF_Membership_Application_Form,
            PF_Nominee_Form,

            PF_Loan_Application_Form,
            PF_Membership_Active_Inactive,

            My_PF_Statement,

            PF_Statement_History

        }

        public enum CPFLoanStatusFIlterForReport
        {
            Closed = 1,
            Running = 2,
            Both
        }

    }
}
