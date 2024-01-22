using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_BEPZA.Web.Areas.AMS.Models.AttendanceProcess
{
    public class AttendanceProcessViewModel : BaseViewModel
    {
        [Required]
        [UIHint("_Date")]
        [DisplayName("Process Date")]
        public DateTime ProcessDate { get; set; } 

    }
}