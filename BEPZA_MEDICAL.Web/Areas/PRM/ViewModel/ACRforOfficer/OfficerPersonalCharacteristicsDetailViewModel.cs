using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class OfficerPersonalCharacteristicsDetailViewModel:BaseViewModel
    {
        #region Standard
        public int OfficerInfoId { get; set; }
        public int AttributeDetailId { get; set; }
        public decimal? Mark { get; set; }
        #endregion

        public string AttributeName { get; set; }
        public decimal? SerialNumber { get; set; }
        public bool ChkFour { get; set; }
        public bool ChkThree { get; set; }
        public bool CkhTwo { get; set; }
        public bool ChkOne { get; set; }

    }
}