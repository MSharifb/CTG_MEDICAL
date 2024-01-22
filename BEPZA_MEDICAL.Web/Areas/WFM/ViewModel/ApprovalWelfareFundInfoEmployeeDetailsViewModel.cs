using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class ApprovalWelfareFundInfoEmployeeDetailsViewModel : BaseViewModel
    {
        #region Standard Property

        public int ApprovalWelfareFundInfoId { get; set; }

        public int EmployeeId { get; set; }

        //[Required]
        [UIHint("_OnlyNumber")]
        public decimal? ApprovedAmount { get; set; }

        public int ApplicationId { get; set; }

        public bool IsOnline { get; set; }


        #endregion

        #region Other
        public bool IsCheckedFinal { get; set; }
        public string EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string AppliedAmount { get; set; }

        #endregion
    }
}