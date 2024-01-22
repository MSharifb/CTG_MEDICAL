using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class OrganizationalSetupManpowerInfoViewModel : BaseViewModel
    {
        #region Ctorm
        public OrganizationalSetupManpowerInfoViewModel()
        {          
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
            this.TempManPwrList = new List<OrganizationalSetupManpowerInfoViewModel>();
        }
        #endregion

        #region Standard Properties
        [Required]
        [DisplayName("Organogram Level")]
        public int OrganogramLevelId { get; set; }

        [Required]
        [DisplayName("Status Designation")]
        public virtual int DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

        [Required]
        [DisplayName("Sanctioned Post")]
        [Range(0, 99999, ErrorMessage = "Sanctioned Post must be between 0 and 99999.")]
        public virtual int SanctionedPost { get; set; }

        [Required]
        [DisplayName("Active Status")]
        public virtual bool ActiveStatus { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        #endregion       

        #region Others

    
        [DisplayName("Organogram Level Name")]
        [UIHint("_ReadOnly")]
        public string OrganogramLevelName { get; set; }

        public int? SLNo { get; set; }
        public string DesignationName { get; set; }
        public IList <OrganizationalSetupManpowerInfoViewModel> TempManPwrList { get; set; }
        #endregion
    }
}
