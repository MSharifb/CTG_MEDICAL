using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.ACRforOfficer
{
    public class ACRforOfficerViewModel : BaseViewModel
    {
        #region Ctor
        public ACRforOfficerViewModel()
        {
            OfficerInfo = new OfficerInfoViewModel();
            HealthTestReport = new OfficerHealthTestReportViewModel();
            BioData = new OfficerBioDataViewModel();
            PersonalCharacteristics = new OfficerPersonalCharacteristicsViewModel();
            PerformanceOfWork = new OfficerPerformanceofWorkViewModel();
            GraphAndRecommendation = new GraphAndRecommendation();
            ReviewingOfficerComments = new ReviewingOfficerComments();
            InformationForAuthority = new InformationForAuthority();
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

        public OfficerInfoViewModel OfficerInfo { get; set; }
        public OfficerHealthTestReportViewModel HealthTestReport { get; set; }
        public OfficerBioDataViewModel BioData { get; set; }
        public OfficerPersonalCharacteristicsViewModel PersonalCharacteristics { get; set; }
        public OfficerPerformanceofWorkViewModel PerformanceOfWork { get; set; }
        public GraphAndRecommendation GraphAndRecommendation { get; set; }
        public ReviewingOfficerComments ReviewingOfficerComments { get; set; }
        public InformationForAuthority InformationForAuthority { get; set; }
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