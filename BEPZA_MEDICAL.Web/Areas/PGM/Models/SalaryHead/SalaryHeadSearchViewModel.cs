using System;
using System.Text;

namespace ERP_BEPZA.Web.Areas.PGM.Models.SalaryHead
{
    public class SalaryHeadSearchViewModel
    {
        #region Properties
        public int? ID { get; set; }
        public string HeadName { get; set; }
        public string HeadType { get; set; }
        public string AmountType { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsGrossPayHead { get; set; }
        public bool IsOtherAddition { get; set; }
        public bool IsOtherDeduction { get; set; }
        #endregion

        #region Methods
        public string GetFilterExpression(string taxable, string grossPay, string otherAddition, string otherDeduction)
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));
            if (!String.IsNullOrWhiteSpace(HeadName))
                filterExpressionBuilder.Append(String.Format("HeadName like {0} AND ", HeadName));
            
            if (!String.IsNullOrWhiteSpace(HeadType) && HeadType != "0")
                filterExpressionBuilder.Append(String.Format("HeadType like {0} AND ", HeadType));
            if (!String.IsNullOrWhiteSpace(AmountType) && AmountType!="0")
                filterExpressionBuilder.Append(String.Format("AmountType like {0} AND ", AmountType));

            if (!string.IsNullOrEmpty(taxable) && taxable != "0" && (IsTaxable == true || IsTaxable == false))
                filterExpressionBuilder.Append(String.Format("IsTaxable={0} AND ", String.Format("{0}", IsTaxable)));
            if (!string.IsNullOrEmpty(grossPay) && grossPay != "0" && (IsGrossPayHead == true || IsGrossPayHead == false))
                filterExpressionBuilder.Append(String.Format("IsGrossPayHead ={0} AND ", String.Format("{0}", IsGrossPayHead)));

            if (!string.IsNullOrEmpty(otherAddition) && otherAddition != "0" && (IsOtherAddition == true || IsOtherAddition == false))
                filterExpressionBuilder.Append(String.Format("IsOtherAddition={0} AND ", String.Format("{0}", IsOtherAddition)));
            if (!string.IsNullOrEmpty(otherDeduction) && otherDeduction != "0" && (IsOtherDeduction == true || IsOtherDeduction == false))
                filterExpressionBuilder.Append(String.Format("IsOtherDeduction ={0} AND ", String.Format("{0}", IsOtherDeduction)));
            
            // Remove ' AND ' from last
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            // return
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}