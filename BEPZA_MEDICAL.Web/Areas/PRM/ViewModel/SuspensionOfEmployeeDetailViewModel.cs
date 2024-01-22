using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SuspensionOfEmployeeDetailViewModel:BaseViewModel
    {
        #region Salary Payments
        public int HeadId { get; set; }
        public string SalaryHead { get; set; }
        public decimal Amount { get; set; }
        public decimal ActualAmount { get; set; }
        public string AmountType { get; set; }
        public string HeadType { get; set; }
        public bool IsTaxable { get; set; }
        #endregion

    }
}