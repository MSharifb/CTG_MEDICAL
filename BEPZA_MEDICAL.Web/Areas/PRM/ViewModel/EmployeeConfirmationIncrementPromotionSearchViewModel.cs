using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeConfirmationIncrementPromotionSearchViewModel
    {
        #region Properties
        public int? ID { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public int? FromDivisionId { get; set; }
        public string Type { get; set; }
        public DateTime EffectiveDate { get; set; }
        
        public int? FromDesignationId { get; set; }
        public int? ToDesignationId { get; set; }

        public int? FromGradeId { get; set; }
        public int? ToGradeId { get; set; }

        public int? FromSalaryScaleId { get; set; }
        public int? ToSalaryScaleId { get; set; }

        public int? FromZoneInfoId { get; set; }
        public int? ToZoneInfoId { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));

            if (!String.IsNullOrWhiteSpace(Type) && Type != "0")
                filterExpressionBuilder.Append(String.Format("Type like {0} AND ", Type));

            if (!String.IsNullOrWhiteSpace(EmpId))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", EmpId));

            if (!String.IsNullOrWhiteSpace(EmployeeName))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.FullName like {0} AND ", EmployeeName));


            if (FromDivisionId.HasValue && FromDivisionId > 0)
                filterExpressionBuilder.Append(String.Format("FromDivisionId = {0} AND ", FromDivisionId));

            if (EffectiveDate != null && !EffectiveDate.Date.ToString().Contains("01/01/0001"))
            {
                string ActualRateFromP = "EffectiveDate >=" + String.Format("{0:dd/MM/yyyy}", EffectiveDate) + " AND ";
                filterExpressionBuilder.Append(ActualRateFromP);
            }

            if (FromDesignationId.HasValue && FromDesignationId > 0)
                filterExpressionBuilder.Append(String.Format("FromDesignationId = {0} AND ", FromDesignationId));

            if (ToDesignationId.HasValue && ToDesignationId > 0)
                filterExpressionBuilder.Append(String.Format("ToDesignationId = {0} AND ", ToDesignationId));

            if (FromGradeId.HasValue && FromGradeId > 0)
                filterExpressionBuilder.Append(String.Format("FromGradeId = {0} AND ", FromGradeId));

            if (ToGradeId.HasValue && ToGradeId > 0)
                filterExpressionBuilder.Append(String.Format("ToGradeId = {0} AND ", ToGradeId));

            if (FromSalaryScaleId.HasValue && FromSalaryScaleId > 0)
                filterExpressionBuilder.Append(String.Format("FromSalaryScaleId = {0} AND ", FromSalaryScaleId));

            if (ToSalaryScaleId.HasValue && ToSalaryScaleId > 0)
                filterExpressionBuilder.Append(String.Format("ToSalaryScaleId = {0} AND ", ToSalaryScaleId));

            if (FromZoneInfoId.HasValue && FromZoneInfoId > 0)
                filterExpressionBuilder.Append(String.Format("FromZoneInfoId = {0} AND ", FromZoneInfoId));

            if (ToZoneInfoId.HasValue && ToZoneInfoId > 0)
                filterExpressionBuilder.Append(String.Format("ToZoneInfoId = {0} AND ", ToZoneInfoId));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }

        #endregion
    }
}