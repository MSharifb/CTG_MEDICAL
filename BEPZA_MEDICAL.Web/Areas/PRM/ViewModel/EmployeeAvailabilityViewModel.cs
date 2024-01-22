using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeAvailabilityViewModel
    {
        #region Properties
        public int? ID { get; set; }
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ActionName { get; set; }
        public string Message { get; set; }
        #endregion

        #region Methods

        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

          
            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }

        #endregion
    }
}