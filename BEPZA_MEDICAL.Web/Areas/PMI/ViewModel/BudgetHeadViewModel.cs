using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel
{
    public class BudgetHeadViewModel : BaseViewModel
    {
        #region Ctor
        public BudgetHeadViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #endregion

        #region Standard Property

        [DisplayName("Budget Head")]
        public int? ParentId { get; set; }


        [DisplayName("Budget Head")]
        [Required]
        public string BudgetHeadName { get; set; }



        [DisplayName("Active")]
        [Required]
        public bool? IsActive { get; set; }

        [DisplayName("Created By")]
        [UIHint("_readOnly")]
        public string CreatedBy { get; set; }

        [DisplayName("Created Date")]
        [UIHint("_readOnly")]
        public DateTime? CreatedDate { get; set; }

        #endregion

        #region Other Property

        public List<BudgetHeadViewModel> BudgetHeadViewModelList { get; set; }

        public IList<SelectListItem> BudgetHeadList { get; set; }

        [DisplayName("Budget Sub Head")]
        public string BudgetSubHeadName { get; set; }

        public int? SortOrder { get; set; }

        #endregion
    }
}