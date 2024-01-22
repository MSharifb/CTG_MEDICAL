using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class RequisitionDetailViewModel
    {
        public RequisitionDetailViewModel()
        {
            //
        }

        [Required]
        public int Id { get; set; }
        public int ItemId { get; set; }
        [Required]
        public int RequisitionId { get; set; }
        [DisplayName("Quantity")]
        [Range(0,99999999)]
        public decimal Quantity { get; set; }
        [DisplayName("Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        //Others
        public string Item { get; set; }
        public string Unit { get; set; }

    }
}