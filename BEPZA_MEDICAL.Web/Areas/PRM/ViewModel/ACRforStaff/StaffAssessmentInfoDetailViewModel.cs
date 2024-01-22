using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff
{
    public class StaffAssessmentInfoDetailViewModel:BaseViewModel
    {
        #region Standard
        public int StaffInfoId { get; set; }
        public int AttributeDetailId { get; set; }
        public decimal? Mark { get; set; }
        public string Remarks { get; set; }
        #endregion

        #region Other
        public string AttributeName { get; set; }
        public decimal? SerialNumber { get; set; }

        public bool ChkFive { get; set; }
        public bool ChkFour { get; set; }
        public bool ChkThree { get; set; }
        public bool CkhTwo { get; set; }
        public bool ChkOne { get; set; }

        #endregion
    }
}