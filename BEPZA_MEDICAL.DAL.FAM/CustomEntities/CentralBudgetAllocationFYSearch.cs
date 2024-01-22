using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class CentralBudgetAllocationFYSearch
    {
        public int DivisionUnitId { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountHeadCode { get; set; }
        public string AccountHeadName { get; set; }
        public string AccountHeadType { get; set; }
        public decimal Amount { get; set; }
    }
}
