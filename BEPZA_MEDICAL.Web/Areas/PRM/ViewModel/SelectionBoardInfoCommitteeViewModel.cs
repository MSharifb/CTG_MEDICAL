using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SelectionBoardInfoCommitteeViewModel : BaseViewModel
    {
        #region Standard Property

        //[Required]
        public int SelectionBoardInfoId { get; set; }

        // [Required]
        [DisplayName("Is External")]
        public bool IsExternal { get; set; }

        //[Required]

        public int? MemberEmployeeId { get; set; }

        // [Required]
        [DisplayName("Member Name")]
        public string MemberName { get; set; }

        // [Required]
        [DisplayName("Member Designation")]
        public string MemberDesignation { get; set; }

        //[Required]
        [DisplayName("Member Role")]
        public string MemberRole { get; set; }

        //[Required]
        [DisplayName("Active Status")]
        public bool ActiveStatus { get; set; }

        //[Required]
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        #endregion

        #region Other

        public string MemberEmpId { get; set; }
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }       
        #endregion
    }
}