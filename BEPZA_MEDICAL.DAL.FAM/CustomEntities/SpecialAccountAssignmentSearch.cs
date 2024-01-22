using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class SpecialAccountAssignmentSearch
    {
        public int Id { get; set; }
        public int PurposeId { get; set; }
        public string PurposeName { get; set; }

        public int AccountHeadId { get; set; }
        public string AccountHeadName { get; set; }

        public string Remarks { get; set; }
    }
}
