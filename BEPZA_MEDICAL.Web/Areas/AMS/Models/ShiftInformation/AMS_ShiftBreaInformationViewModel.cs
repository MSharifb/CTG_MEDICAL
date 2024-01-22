using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP_BEPZA.Web.Areas.AMS.Models.ShiftInformation
{
    public class AMS_ShiftBreaInformationViewModel
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public string BreakName { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
       
    }
}
