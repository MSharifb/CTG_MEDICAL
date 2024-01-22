using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel
{
    public class ScrapDetailViewModel
    {
        public ScrapDetailViewModel()
        {
            //
        }

        [Required]
        public int Id { get; set; }
        public int ItemId { get; set; }
        [Required]
        public int ScrapId { get; set; }
        [DisplayName("Quantity")]
        [Range(0, 99999999)]
        public decimal Quantity { get; set; }
        [DisplayName("Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        [Required]
        public int ItemStatusId { get; set; }

        //Others
        public string Item { get; set; }
        public string Unit { get; set; }

        public int EmployeeId { get; set; }
        public decimal SumQty { get; set; }

    }
}