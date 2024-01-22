using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class SalaryStructureDetailsDeductionHeadViewModel
    {
        public int Id { get; set; }
        public int SalaryStructureId { get; set; }

        [Required]
        [DisplayName("Salary Head")]
        public int HeadId { get; set; }

        [Required]
        [DisplayName("Deduction Heads")]
        public int HeadType { get; set; }

        [Required]
        [DisplayName("Amount Type")]
        public int AmountType { get; set; }

        [DisplayName("Amount")]
        [UIHint("_OnlyCurrency")]
        public decimal Amount { set; get; }

        [DisplayName("Taxable")]
        public bool IsTaxable { set; get; }
    }
}