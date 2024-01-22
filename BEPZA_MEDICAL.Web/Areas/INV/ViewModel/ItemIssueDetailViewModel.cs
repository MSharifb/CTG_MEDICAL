using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class ItemIssueDetailViewModel : BaseViewModel
    {
        public ItemIssueDetailViewModel()
        {
            //
        }

        //[Required]
        //public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int IssueId { get; set; }

        public int? PurchaseId { get; set; }
        
        [DisplayName("Quantity")]
        [Required]
        [Range(0, 99999999)]
        public decimal IssueQuantity { get; set; }

        [Range(0, 99999999)]
        [Required]
        public decimal DemandQuantity { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        //Others
        public string Item { get; set; }
        public string Unit { get; set; }

        //For validation purpose
        public decimal SumQty { get; set; }
        public int RequisitionId { get; set; }

    }
}