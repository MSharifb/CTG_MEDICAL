using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PMI.ViewModel.Estimation
{
    public class EstimationListViewModel
    {
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("EstimationDetailId")]
        public int EstimationDetailId { get; set; }

        [DisplayName("ProjectId")]
        public int ProjectId { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [DisplayName("EstimationItemId")]
        public int EstimationItemId { get; set; }

        [DisplayName("Item Name")]
        public string ItemName { get; set; }

        [DisplayName("Zone Code")]
        public string ZoneCode { get; set; }

        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Estimation Date")]
        public DateTime EstimationDate { get; set; }

        public decimal BudgetAmount { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public int? ZoneId { get; set; }

        [DisplayName("To")]
        public DateTime EstimationDateTo { get; set; }
    }
}