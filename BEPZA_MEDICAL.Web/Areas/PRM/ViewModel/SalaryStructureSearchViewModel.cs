using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace ERP_BEPZA.Web.Areas.PRM.ViewModel
{
    public class SalaryStructureSearchViewModel
    {
        #region Properties
        
        public int? Id { get; set; }

        public int? SalaryScaleId { get; set; }

        public int? GradeId { get; set; }
        public int? StepId { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (Id.HasValue)
                filterExpressionBuilder.Append(String.Format("Id = {0} AND ", Id));

            if (SalaryScaleId.HasValue && SalaryScaleId > 0)
                filterExpressionBuilder.Append(String.Format("SalaryScaleId = {0} AND ", SalaryScaleId));

            if (GradeId.HasValue && GradeId > 0)
                filterExpressionBuilder.Append(String.Format("GradeId = {0} AND ", GradeId));

            if (StepId.HasValue && StepId > 0)
                filterExpressionBuilder.Append(String.Format("StepId = {0} AND ", StepId));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}