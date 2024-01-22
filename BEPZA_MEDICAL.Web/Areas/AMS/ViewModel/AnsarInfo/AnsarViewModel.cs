using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BEPZA_MEDICAL.Web.Areas.AMS.ViewModel.AnsarInfo
{
    public class AnsarViewModel
    {
        #region Constructor

        public AnsarViewModel()
        {
            EmploymentInfo = new EmploymentInfoViewModel();
            PersonalInfo = new PersonalInfoViewModel();
            AccademicQlfnInfo = new AccademicQlfnInfoViewModel();
            JobExperienceInfo = new JobExperienceInfoViewModel();
            EmpServiceHistoryViewModel = new EmpServiceHistoryViewModel();
            AnsarPhotograph = new AnsarPhotoGraphViewModel();
        }

        #endregion

        #region Standerd Property

        public int Id { get; set; }
        public string BEPZAId { get; set; } 
        public EmploymentInfoViewModel EmploymentInfo { get; set; }
        public PersonalInfoViewModel PersonalInfo { get; set; }
        public AccademicQlfnInfoViewModel AccademicQlfnInfo { get; set; }
        public JobExperienceInfoViewModel JobExperienceInfo { get; set; }
        public AnsarPhotoGraphViewModel AnsarPhotograph { get; set; }
       
        
        public EmpServiceHistoryViewModel EmpServiceHistoryViewModel { get; set; }

        #endregion

        #region Others
        public string ViewType { get; set; }
        public int EmployeeId { get; set; } 

        #endregion
    }
 

}