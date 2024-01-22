using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP_BEPZA.Web.Areas.AMS.Models.ShiftInformation
{
    public class AMS_ShiftInformationViewDetail
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public System.DateTime PeriodFrom { get; set; }
        public System.DateTime PeriodTo { get; set; }
        public System.TimeSpan InTime { get; set; }
        public System.TimeSpan OutTime { get; set; }
        public Nullable<int> InGraceTime { get; set; }
        public Nullable<int> OutGraceTime { get; set; }
        public Nullable<int> AbsentTime { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public System.DateTime IDate { get; set; }
        public string EUser { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }
}
