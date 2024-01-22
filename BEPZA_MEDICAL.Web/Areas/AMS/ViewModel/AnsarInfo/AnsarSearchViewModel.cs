using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel
{
    public class AnsarSearchViewModel
    {
        public AnsarSearchViewModel()
        {
                
        }

        #region Properties
        public int? ID { get; set; }
        public string BEPZAId { get; set; }
        public string FullName { get; set; }
        public int? DesignationId { get; set; }
        public int? StatusId { get; set; }
        public string AnsarId { get; set; }

        public int SelectedEmployeeStatus { get; set; }
        public string ActionName { get; set; }
        public string Message { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ZoneInfoId { get; set; }
        public string NationalID { get; set; }
        
        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("Id = {0} AND ", ID));

            if (!String.IsNullOrWhiteSpace(BEPZAId))
                filterExpressionBuilder.Append(String.Format("BEPZAId like {0} AND ", BEPZAId));

            if (!String.IsNullOrWhiteSpace(FullName))
                filterExpressionBuilder.Append(String.Format("FullName like {0} AND ", FullName));

            if (DesignationId.HasValue && DesignationId > 0)
                filterExpressionBuilder.Append(String.Format("DesignationId = {0} AND ", DesignationId));

            if (StatusId.HasValue && StatusId > 0)
                filterExpressionBuilder.Append(String.Format("StatusId = {0} AND ", StatusId));

            if (!String.IsNullOrWhiteSpace(AnsarId))
                filterExpressionBuilder.Append(String.Format("AnsarId like {0} AND ", AnsarId));

            if (ZoneInfoId > 0)
                filterExpressionBuilder.Append(String.Format("ZoneInfoId = {0} AND ", ZoneInfoId));

            
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion




    }
}