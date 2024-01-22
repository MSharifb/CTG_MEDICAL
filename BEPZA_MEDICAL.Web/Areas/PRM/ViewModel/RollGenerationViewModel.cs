using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class RollGenerationViewModel : BaseViewModel
    {
        public RollGenerationViewModel()
        {            
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #region Standard Properties
        [Display(Name="Advertisement Code")]
        [Required]
        public string AdCode { get; set; }
        [Display(Name = "Name of Post")]
        public Int32 DesignationId { get; set; }
        public Int32 RollNoFrom { get; set; }
        #endregion 
        public IList<SelectListItem> JobAdvertisementInfoList { set; get; }

        public IList<SelectListItem> JobPostList { set; get; }

    }
}