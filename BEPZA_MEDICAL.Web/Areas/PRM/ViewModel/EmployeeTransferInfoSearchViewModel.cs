using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class EmployeeTransferInfoSearchViewModel
    {
        #region Properties
        public int? ID { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeInitial { get; set; }
        public int? DesignationId { get; set; }
        public int? FromDivisionId { get; set; }
        public int? ToDivisionId { get; set; }
        public int? FromLocationId { get; set; }
        public int? ToLocationId { get; set; }       
        public DateTime TransferDateFrom { get; set; }
        public DateTime TransferDateTo { get; set; }


        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();
            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));

            if (!String.IsNullOrWhiteSpace(EmpId))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmpID like {0} AND ", EmpId));

            if (!String.IsNullOrWhiteSpace(EmployeeName))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.FullName like {0} AND ", EmployeeName));

            if (!String.IsNullOrWhiteSpace(EmployeeInitial))
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.EmployeeInitial like {0} AND ", EmployeeInitial));

            if (DesignationId.HasValue && DesignationId > 0)
                filterExpressionBuilder.Append(String.Format("PRM_EmploymentInfo.DesignationId = {0} AND ", DesignationId));

            if (FromDivisionId.HasValue && FromDivisionId > 0)
                filterExpressionBuilder.Append(String.Format("FromDivisionId = {0} AND ", FromDivisionId));

            if (ToDivisionId.HasValue && ToDivisionId > 0)
                filterExpressionBuilder.Append(String.Format("ToDivisionId = {0} AND ", ToDivisionId));

            if (FromLocationId.HasValue && FromLocationId > 0)
                filterExpressionBuilder.Append(String.Format("FromLocationId = {0} AND ", FromLocationId));

            if (ToLocationId.HasValue && ToLocationId > 0)
                filterExpressionBuilder.Append(String.Format("ToLocationId = {0} AND ", ToLocationId));

            //if (TransferDateFrom != null && !TransferDateFrom.Date.ToString().Contains("01/01/0001"))
            //{
            //    string ActualRateFromP = "TransferDate >= " + TransferDateFrom.ToLongDateString() + " AND ";
            //    filterExpressionBuilder.Append(ActualRateFromP);
            //}
            //if (TransferDateTo != null && !TransferDateTo.Date.ToString().Contains("01/01/0001"))
            //{
            //    string ActualRateToP = "TransferDate <= " + TransferDateTo.ToLongDateString() + " AND ";
            //    filterExpressionBuilder.Append(ActualRateToP);
            //}
            
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}

