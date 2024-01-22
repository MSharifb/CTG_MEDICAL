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
    public class EstimationItemSetupViewModel
    {
        public EstimationItemSetupViewModel()
        {
            this.UnitList = new List<SelectListItem>();
            this.EstimationItemList = new List<EstimationItemSetupViewModel>();

            //this.IUser = HttpContext.Current.User.Identity.Name;
            //this.IDate = DateTime.Now;
        }
        public int Id { get; set; }
        public int? EstimationSetupId { get; set; }

        [DisplayName("Unit")]
        public int UnitId { get; set; }

        //[Required]
        [DisplayName("Estimation Item")]
        public string EstimationItem { get; set; }

        //[Required]
        [UIHint("_ReadOnly")]
        [DisplayName("Item SL. No.")]
        public string SLNo { get; set; }

        [DisplayName("Unit Price (Tk)")]
        public decimal? UnitPrice { get; set; }
        public string EstimationHead { get; set; }
        public string Unit { get; set; }

        [DisplayName("Description")]
        public string DescriptionDetails { get; set; }
        public IList<SelectListItem> UnitList { get; set; }
        public IList<EstimationItemSetupViewModel> EstimationItemList { get; set; }

        [DisplayName("Code")]
        public string ItemCode { get; set; }
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(EstimationHead))
                filterExpressionBuilder.Append(String.Format("EstimationHead like {0} AND ", EstimationHead));

            if (!String.IsNullOrWhiteSpace(EstimationItem))
                filterExpressionBuilder.Append(String.Format("EstimationHead like {0} AND ", EstimationItem));

            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }

    }
}