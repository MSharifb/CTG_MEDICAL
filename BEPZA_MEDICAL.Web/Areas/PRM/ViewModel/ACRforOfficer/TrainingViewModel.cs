using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class TrainingViewModel
    {
        #region Training
        public string TrainingTitle { get; set; }
        public string Institution { get; set; }
        public string TraingType { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string TrainingYear { set; get; }
        #endregion 

    }
}