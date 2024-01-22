using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class FixedDepositInfoInstallmentScheduleViewModel : BaseViewModel
    {
        public int FixedDepositInfoId { get; set; }

        public DateTime InsDate { get; set; }

        public decimal InsAmount { get; set; }

        public decimal Tax { get; set; }
        public decimal BankCharge { get; set; }
        public decimal Profit { get; set; }

        #region Other
        public int? SLNo { get; set; }
        #endregion
    }
}