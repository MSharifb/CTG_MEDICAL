using System;
using System.Text;

namespace BEPZA_MEDICAL.DAL.PGM.CustomEntities
{
    public class OverTimeSearch
    {
        public long Id { get; set; }
        public string SalaryYear { get; set; }
        public string SalaryMonth { get; set; }
        public string DepartmentName{ get; set; }
        public DateTime ? PaymentDate { get; set; }
        public string IcNo { get; set; }
        public int DepartmentId { get; set; }
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(SalaryYear))
                filterExpressionBuilder.Append(String.Format("SalaryYear like {0} AND ", SalaryYear));

            if (!String.IsNullOrWhiteSpace(SalaryMonth))
                filterExpressionBuilder.Append(String.Format("SalaryMonth like {0} AND ", SalaryMonth));

            if (PaymentDate != null)
                filterExpressionBuilder.Append(String.Format("PaymentDate like {0} AND ", PaymentDate));

            filterExpressionBuilder.Append(String.Format("IsPayment =0 AND"));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }
    }
}
