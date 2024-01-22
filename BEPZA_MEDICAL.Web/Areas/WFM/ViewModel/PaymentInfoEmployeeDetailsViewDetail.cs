using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class PaymentInfoEmployeeDetailsViewDetail : BaseViewModel
    {
        #region Standard Property

        public int PaymentInfoId { get; set; }
        public int EmployeeId { get; set; }

        #endregion

        #region Other
        public bool IsCheckedFinal { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string ApprovedAmount { get; set; }

        #endregion
    }
}