using BEPZA_MEDICAL.Web.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class DistrictWiseApplicantInfoViewModel
    {
        #region Ctor
        public DistrictWiseApplicantInfoViewModel()
        {
            this.DesignationList = new List<SelectListItem>();
            this.JobAdvertisementList = new List<SelectListItem>();
            this.reportTemp = new List<ReportTemp>();
        }
        #endregion

        [DisplayName("Job Advertisement Code")]
        public int? JobAdvertisementId { get; set; }
        public IList<SelectListItem> JobAdvertisementList { get; set; }

        [DisplayName("Post Name")]
        public int DesignationId { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }
        public List<ReportTemp> reportTemp { get; set; }
    }
    public class ReportTemp 
    {
        [DataMember, DataColumn(true)]
        public int ID { get; set; }
        [DataMember, DataColumn(true)]
        public System.String Name { get; set; }
        [DataMember, DataColumn(true)]
        public int TotalNo { get; set; }
        [DataMember, DataColumn(true)]
        public string Degree { get; set; }

        [DataMember, DataColumn(true)]
        public string JovAd { get; set; }

        [DataMember, DataColumn(true)]
        public string Designation { get; set; }

    }

}