using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel.IssueReturn
{
    public class IssueReturnDetailViewModel : BaseViewModel
    {
        public IssueReturnDetailViewModel()
        {
            //
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int IssueReturnId { get; set; }

        [Required]
        public int ItemStatusId { get; set; }

        [DisplayName("Quantity")]
        [Required]
        [Range(0, 99999999)]
        public decimal Quantity { get; set; }

        [DisplayName("Comment")]
        [MaxLength(100)]
        public string Comment { get; set; }

        //Others
        public string Item { get; set; }
        public string Unit { get; set; }

        //For item validation
        public decimal SumQty { get; set; }
        public int EmployeeId { get; set; }

        public int ScrapId { get; set; }

    }
}