using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEPZA_MEDICAL.DAL.FAM.CustomEntities
{
    public class BSPLReportHeadMappingSearch
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }

        public int BSPLReportHeadId { get; set; }
        public string BSPLReportHeadName { get; set; }

        public int ReportHeadSerial { set; get; }
    }
}
