using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class InvestigationCommitteeInfoFormedForViewModel:BaseViewModel
    {
        #region Standard Property
        public int InvestigationCommitteeInfoId { get; set; }
        public int ComplaintNoteSheetId { get; set; }
        public string Remarks { get; set; }
        #endregion


        #region Others
        public string ComplaintNoteSheetName { get; set; }
        #endregion
    }
}