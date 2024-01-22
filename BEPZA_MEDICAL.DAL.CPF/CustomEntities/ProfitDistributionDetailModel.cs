using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.CPF.CustomEntities
{
    public class ProfitDistributionDetailModel
    {
        public Int64 Id { get; set; }
        public string Period { get; set; }
        public int EmployeeId { get; set; }
        public int MembershipId { get; set; }

        public string EmployeeInitial { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }

        public string strMembershipID { get; set; }
        public int DesigSortingOrder { get; set; }
        
        public decimal EmpOpening { get; set; }
        public decimal EmpContributionInPeriod { get; set; }
        public decimal EmpProfitInPeriod { get; set; }
        public decimal EmpWithdrawnInPeriod { get; set; }
        public decimal EmpFinalPayment { get; set; }
        public decimal EmpClosingBalance { get; set; }

        public decimal ComOpening { get; set; }
        public decimal ComContributionInPeriod { get; set; }
        public decimal ComProfitInPeriod { get; set; }
        public decimal ComWithdrawnInPeriod { get; set; }
        public decimal ComFinalPayment { get; set; }
        public decimal ComForfeitedAmount { get; set; }
        public decimal ComClosingBalance { get; set; }

        public decimal TotalBalance { get; set; }
    }
}
