using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web.Areas.INV.ViewModel.Adjustment
{
    public class AdjustmentDetailViewModel : BaseViewModel
    {
        public AdjustmentDetailViewModel()
        {
            //
        }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int AdjustmentId { get; set; }

        [Required]
        public int ItemStatusId { get; set; }

        [DisplayName("Quantity")]
        [Required]
        [Range(0, 99999999)]
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

        //For validation
        public decimal SumQty { get; set; }

    }
}