using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DegreeLevelMappingDetailViewModel: BaseViewModel
    {
        #region Ctor
        public DegreeLevelMappingDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        #endregion

        #region Standard Property
        public int DegreeLevelMappingId { get; set; }
        public int? AvailableDegreeLevelId { get; set; }
        public int? DegreeLevelId { get; set; }
        #endregion

        #region Other
        public string DegreeLevelName { get; set; }
        #endregion

    }
}