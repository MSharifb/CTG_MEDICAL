using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforStaff
{
    public class ACRforStaffViewModel : BaseViewModel
    {
        #region Ctor
        public ACRforStaffViewModel()
        {
            StaffInfo = new StaffInfoViewModel();
            BioData = new StaffBioDataViewModel();
            AssessmentInfo = new StaffAssessmentInfoViewModel();
        }
        #endregion

        #region Search Property
        public string ActionName { get; set; }

        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        #endregion

        #region Standerd Property
        public string EmpId { get; set; }

        public StaffInfoViewModel StaffInfo { get; set; }
        public StaffBioDataViewModel BioData { get; set; }
        public StaffAssessmentInfoViewModel AssessmentInfo { get; set; }
        #endregion

        #region Others
        public string ViewType
        {
            get;
            set;
        }

        

        #endregion

    }
}