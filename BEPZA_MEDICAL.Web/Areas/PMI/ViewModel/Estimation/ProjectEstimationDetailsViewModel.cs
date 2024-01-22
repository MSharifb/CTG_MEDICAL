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
    public class ProjectEstimationDetailsViewModel : BaseViewModel
    {
        #region ctor

        public ProjectEstimationDetailsViewModel()
        {

            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;

            EstimationItemList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();

        }

        #endregion

        #region Standard Property

        public int MasterId { get; set; }

        [Required]
        [DisplayName("Estimation Head")]
        public int EstimationHeadId { get; set; }

        [DisplayName("Estimation Item")]
        [Required]
        public int EstimationItemId { get; set; }

        [DisplayName("Unit")]
        [Required]
        public int UnitId { get; set; }

        [DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }


        [DisplayName("Total Amount")]
        [Required]
        public decimal TotalAmount { get; set; }

        [DisplayName("Description")]
        public string ItemDescription { get; set; }
        public string SerialNo { get; set; }

        #endregion

        #region Others

        [DisplayName("Estimation Head")]
        public string EstimationHead { get; set; }

        [DisplayName("Description")]
        public string EstimationHeadDescription { get; set; }

        public string UnitName { get; set; }

        public IList<SelectListItem> EstimationItemList { get; set; }

        public IList<SelectListItem> UnitList { get; set; }

        public string EstimationItemName { get; set; }

        #endregion
    }

    public class EstimationItemInfo
    {
        public int EstimationHeadId { get; set; }

        public int EstimationItemId { get; set; }

        public string ItemName { get; set; }

        public int UnitId { get; set; }

        public string UnitName { get; set; }

        public decimal UnitPrice { get; set; }
    }

}
