using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.FMS.ViewModel
{
    public class FDRTypeViewModel
    {
        #region Ctor
        public FDRTypeViewModel()
        {
            this.FDRTypeList = new List<SelectListItem>();
        }
        #endregion

        #region Other
                public IList<SelectListItem> FDRTypeList { get; set; }
        #endregion

    }
}