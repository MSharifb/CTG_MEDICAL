using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_BEPZA.Web.Areas.AMS.Models.DailyAttendanceStatus
{
    public class DailyAttendanceStatusModel
    {
        #region Ctor
        public DailyAttendanceStatusModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = "Create";
            //this.StatusList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        public int Id { get; set; }

        [Required]
        [DisplayName("Present")]
        public string Present { get; set; }

        [Required]
        [DisplayName("Early Arrival")]
        public string EarlyArrival { get; set; }

        [Required]
        [DisplayName("Early Departure")]
        public string EarlyDeparture { get; set; }

        [Required]
        [DisplayName("Late Arrival")]
        public string LateArrival { get; set; }

        [Required]
        [DisplayName("Late Departure")]
        public string LateDeparture { get; set; }

        [Required]
        [DisplayName("Absent")]
        public string Absent { get; set; }

        [Required]
        [DisplayName("Leave")]
        public string Leave { get; set; }

        [Required]
        [DisplayName ("Out Station Duty")]
        public string OutStationDuty { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }

        #endregion

        #region Other

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        //public IList<SelectListItem> StatusList { get; set; }
        #endregion
    }
}