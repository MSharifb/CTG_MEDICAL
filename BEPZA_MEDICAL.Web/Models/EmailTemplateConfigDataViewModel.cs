using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BEPZA_MEDICAL.Web
{
    public class EmailTemplateConfigDataViewModel : BaseViewModel
    {
        #region Primitive Properties

        [Required(), DisplayName("Indivisual Employee Timesheet Day Limit")]
        public int TimesheetDayLimit1 { get; set; }

        [Required(), DisplayName("General Employee Timesheet Day Limit")]
        public int TimesheetDayLimit2 { get; set; }

        [Required(), DisplayName("Equipment/Milestone Delivery Mail Send in Every")]
        public int EmailSendDayInterval { get; set; }

        [Required(), DisplayName("Equipment/Milestone Delivery Mail Send Befor")]
        public int EmailSendBeforeDayLimit { get; set; }

        [Required(), DisplayName("Timesheet Mail Sending Date limit for 15 Days")]
        public DateTime TimesheetEmailSendUpToDateLimit { get; set; }

        [Required(), DisplayName("From Email Address"), MaxLength(100)]
        public string FromEmailAddress { get; set; }

        [Required(), DisplayName("From Name"), MaxLength(100)]
        public string FromName { get; set; }

        [Required(), DisplayName("SMTP Server")]
        public string SMTPServer { get; set; }

        [Required(), DisplayName("SMTP User Namer")]
        public string SMTPUserName { get; set; }

        [Required(), DisplayName("SMTP User Password")]
        public string SMTPUserPassword { get; set; }

        [Required(), DisplayName("SMTP Port")]
        public int SMTPPort { get; set; }

        #endregion
    }
}