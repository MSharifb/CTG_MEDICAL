using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel
{
    public class ProjectSectionViewModel
    {
        #region Ctor
        public ProjectSectionViewModel()
        {
            this.ProjectSectionList = new List<SelectListItem>();
        }
        #endregion

        #region Other
        public IList<SelectListItem> ProjectSectionList { get; set; }
        #endregion

    }
}