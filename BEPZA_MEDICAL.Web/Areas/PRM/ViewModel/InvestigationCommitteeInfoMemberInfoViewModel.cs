using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class InvestigationCommitteeInfoMemberInfoViewModel : BaseViewModel
    {
        #region Standard Property

        public int InvestigationCommitteeInfoId { get; set; }
        public bool IsExternal { get; set; }
        public bool ActiveStatus { get; set; }
        public int? MemberEmployeeId { get; set; }

        [DisplayName("Member Name")]
        public string MemberName { get; set; }

        [DisplayName("Member Designation")]
        public string MemberDesignation { get; set; }

        public string MemberRole { get; set; }
        public string MemberRemarks { get; set; }

        #endregion

        #region Other
        [DisplayName("Member ID")]
        public string MemberEmpId { get; set; }
        #endregion
    }
}