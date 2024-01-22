using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class ItemPurchaseDetailViewModel : BaseViewModel
    {
        public ItemPurchaseDetailViewModel()
        {
            //
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int PurchaseId { get; set; }

        [DisplayName("SL")]
        public int SL { get; set; }

        [DisplayName("Quantity")]
        [Required]
        [Range(0,99999999)]
        public decimal Quantity { get; set; }

        [DisplayName("Rate")]
        [Required]
        [Range(0, 9999999999999999)]
        public decimal Rate { get; set; }

        [DisplayName("Total")]
        [Required]
        [Range(0, 9999999999999999)]
        public decimal TotalCost { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        //Others
        public string Item { get; set; }
        public string Unit { get; set; }

    }
}