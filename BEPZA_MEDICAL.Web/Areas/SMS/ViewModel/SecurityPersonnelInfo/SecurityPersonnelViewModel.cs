using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;

namespace BEPZA_MEDICAL.Web.Areas.SMS.ViewModel.SecurityPersonnelInfo
{
    public class SecurityPersonnelViewModel
    {
        #region Constructor

        public SecurityPersonnelViewModel()
        {
            EmploymentInfo = new SecurityPersonnelEmpInfoViewModel();
            AccademicQlfnInfo = new AccademicQlfnInfoViewModel();
            JobExperienceInfo = new JobExperienceInfoViewModel();
            EmpServiceHistoryViewModel = new SecurityServiceHistoryViewModel();
        }

        #endregion

        #region Standerd Property

        public int Id { get; set; }
        public string BEPZAId { get; set; }
        public SecurityPersonnelEmpInfoViewModel EmploymentInfo { get; set; }
        public AccademicQlfnInfoViewModel AccademicQlfnInfo { get; set; }
        public JobExperienceInfoViewModel JobExperienceInfo { get; set; }

        public SecurityServiceHistoryViewModel EmpServiceHistoryViewModel { get; set; }

        #endregion

        #region Others
        public string ViewType { get; set; }
        public int EmployeeId { get; set; }

        #endregion
    }


}