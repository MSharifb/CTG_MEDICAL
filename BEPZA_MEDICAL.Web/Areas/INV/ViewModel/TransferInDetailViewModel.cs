using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class TransferInDetailViewModel
    {
        public TransferInDetailViewModel()
        {
            //
        }

        [Required]
        public int Id { get; set; }
        public int ItemId { get; set; }
        [Required]
        public int TransferInId { get; set; }
        //[Required]
        public int? PurchaseId { get; set; }

        [DisplayName("Quantity")]
        [Range(0,99999999)]
        public decimal Quantity { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        //Others
        public string Item { get; set; }
        public string Unit { get; set; }
        [DisplayName("MRR #")]
        public string MRR { get; set; }
        //[DisplayName("SL. No.")]
        //public int SlNo { get; set; }
        public string ErrMessage { get; set; }
        public int ReceivedFromId { get; set; }
        public string StrMode { get; set; }
        public decimal SumQty { get; set; }

    }
}