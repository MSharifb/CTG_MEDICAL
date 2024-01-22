using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class RevisionOfCentralBudgetFYSearch
    {
        public int Id { get; set; }
        public int FinancialYearId { get; set; }
        public string FinancialYearName { get; set; }
        public int RevisionNo { get; set; }
        public string Remarks { get; set; }
    }
}
