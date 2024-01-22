using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Approval
{
    public class INVRequisitionDtlViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int RequisitionId { get; set; }
        [DisplayName("Quantity")]
        [Range(0, 99999999)]
        public decimal Quantity { get; set; }
        public string ItemName { get; set; }
        public string Comment { get; set; }
        public string Unit { get; set; }
        public decimal RecommendQuantity { get; set; }
        public decimal BalanceQuantity { get; set; }

    }
}