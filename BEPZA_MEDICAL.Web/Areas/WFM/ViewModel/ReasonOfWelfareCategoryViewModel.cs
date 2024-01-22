using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BEPZA_MEDICAL.Web.Areas.WFM.ViewModel
{
    public class ReasonOfWelfareCategoryViewModel : BaseViewModel
    {
        #region Ctor
        public ReasonOfWelfareCategoryViewModel()
        {
            this.WelfareFundCategoryList = new List<SelectListItem>();
        }

        #endregion


        #region Standard Properties       

        [Required]
        [DisplayName("Category Name")]
        public int WelfareFundCategoryId { get; set; }
       
        [Required]       
        [MaxLength(250, ErrorMessage = "Maximum length 250.")]
        public string Reason { get; set; }

        #endregion

        #region Others 

        public string WelfareFundCategoryName { get; set; }
        public IList<SelectListItem> WelfareFundCategoryList { get; set; }
        #endregion
    }
}