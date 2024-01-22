using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class DivisionUnitFinancialYearRevisionSearch
    {
        public int Id { get; set; }
        public int DivisionUnitId { get; set; }
        public string DivisionUnitName { get; set; }

        public int FinancialYearId { get; set; }
        public string FinancialYearName { get; set; }

        public int RevisionNo { get; set; }
    }
}
