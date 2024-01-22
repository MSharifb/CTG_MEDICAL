using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MFS_IWM.Web.Utility;

namespace MFS_IWM.Web.Areas.PRM.ViewModel
{
    public class JobExperience
    {
        public int Id { get; set; }
        public string Organization { set; get; }
        public string Designation { set; get; }
        public string EmploymentType { set; get; }
        public string OrganizationType { set; get; }
        public DateTime? FromDate { set; get; }
        public DateTime? EndDate { set; get; } 
        public TimeSpan? Duration{ set; get; } 
    }
}