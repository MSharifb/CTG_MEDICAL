using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation
{
    public class EstimationSetupViewModel : BaseViewModel
    {
        public EstimationSetupViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            this.UnitList = new List<SelectListItem>();
            this.EstimationHeadList = new List<SelectListItem>();
            this.StatusList = new List<SelectListItem>();

            this.EstimationSetupViewModelList = new List<EstimationSetupViewModel>();
        }
        #region Standard Property

        [DisplayName("Estimation Head")]
        public int EstimationHeadId { get; set; }

        [DisplayName("Estimation Item")]
        public string ItemName { get; set; }

        [DisplayName("Unit")]
        public int UnitId { get; set; }

        [DisplayName("Unit Price")]
        [Range(0d, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal UnitPrice { get; set; }

        [DisplayName("Description")]
        public string ItemDescription { get; set; }

        [DisplayName("Code")]
        public string ItemCode { get; set; }


        #endregion

        #region Others

        public IList<SelectListItem> EstimationHeadList { get; set; }
        public IList<SelectListItem> UnitList { get; set; }
        public IList<SelectListItem> StatusList { get; set; }

        [DisplayName("Detailed Description")]
        public string Description { get; set; }

        public List<EstimationSetupViewModel> EstimationSetupViewModelList { get; set; }

        #endregion
    }
}