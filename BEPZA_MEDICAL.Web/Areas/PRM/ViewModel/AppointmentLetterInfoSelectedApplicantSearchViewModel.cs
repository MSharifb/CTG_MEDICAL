using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel
{
    public class AppointmentLetterInfoSelectedApplicantSearchViewModel
    {
        #region Standard Property

        public int Id { get; set; }
       
        public int ApplicantInfoId { get; set; }

      
        [DisplayName("Selected Id")]
        public int SelectedId { get; set; }
        
        #endregion

        #region Other
               
        [Display(Name = "Applicant Name")]
        public string ApplicantName { get; set; }

        public int JobAdvertisementInfoId { get; set; }
        public string JobAdvertisementCode { get; set; }

        public int DesignationId { get; set; }
        public string DesignationName { get; set; }

        #endregion
    }
}