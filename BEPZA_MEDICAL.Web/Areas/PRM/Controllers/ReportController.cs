using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel.Report;
using BEPZA_MEDICAL.Web.Controllers;
using BEPZA_MEDICAL.Web.Utility;
using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ReportController : BaseController
    {
        #region Fields
        private readonly ReportViewerViewModel _model;
        #endregion

        #region Ctor
        public ReportController()
        {
            _model = new ReportViewerViewModel();
        }
        #endregion

        #region Actions
        
        public ActionResult EmployeeInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ZoneWiseEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptZoneWiseEmployeeList.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult  DivisionWiseEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptDivisionWiseEmployeeList.aspx");
            return View("ReportViewer", _model);
        }


        public ActionResult SectionWiseEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptSectionWiseEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult GradeWiseEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptGradeWiseEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult InactiveEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/InactiveEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ConfirmedEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/ConfirmedEmpList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ConfirmationEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/ConfirmationEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmpContractExtension()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmpContractExtension.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListByJoiningConfirmationSeparation()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByJoiningConfirmationSeparation.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult PromotedEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeePromotion.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ListOfResourceInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/ListOfResourceInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult TransferredEmp()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/TransferredEmployee.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult DesignationWiseManpowerSummary()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptDesignationWiseManpowerSummary.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult IncrementedEmployeeInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/IncrementedEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListForAnnualIncrement()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListForAnnualIncrement.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeTrainingInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmpTrainingInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListDOBWise()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByDOB.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListByBloodGroup()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByBloodGroup.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListByMarriageDate()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByMarriageDate.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListByDateOfRetirement()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByDateOfRetirement.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListByGender()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByGender.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ListOfEmployeeByServiceLength()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/ListOfEmployeeByServiceLength.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListByJobSkill()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/EmployeeListByJobSkill.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult JobVacancyReport() 
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/JobVacancy.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult InactiveEmployeeListByLeave()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/InactiveEmployeeListByLeave.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult DesignationList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptDesignation.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ShortListedApplicant()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptShortListedApplicant.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FinallyApprovedApplicant()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptFinallyApprovedApplicant.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult TestResultInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptTestResultInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeClassWiseManpowerInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeClassWiseManpowerInfo.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult FinalTestResult()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptFinalTestResult.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult PromotionSheetforOfficer()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptPromotionSheetForOfficer.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult TerminatedEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptTerminatedEmployeeList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult SeniorityList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptSeniority.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DepartmentWiseManpowerInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptDepartmentWiseManpowerInfo.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult PromotionSheetforStaff()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptPromotionSheetForStaff.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ACRforStaffRpt()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptACRforStaff.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ACRforOfficerRpt()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptACRforOfficer.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ListofSuspensionEmployee()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptSuspensionEmployee.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ListofEligibleforPromotionEmployee()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptListofEligibleforPromotionEmployee.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult PromotionDueSheet()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptPromotionDueSheet.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult RptEmployeeContactList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeContactList.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult AttendanceReportforApplicant()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/AttendanceReportforApplicant.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult ApplicantsSummaryInformation()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptApplicantsSummaryInformation.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult ListofApplicants()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptListofApplicants.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult AdmissionFeesLedger()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptAdmissionFeesLedger.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DistrictWiseApplicantInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptDistrictWiseApplicantInfo.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult UniversityWiseApplicantInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptUniversityWiseApplicantList.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DesignationWiseEmployeeList()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptDesignationWiseEmpList.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult DepartmentProceding()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/DepartmentProceding.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmpClsWiseManpower()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmpClassWiseAllZonesManpower.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult PresentStatusofManpowerAndVehicle()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/PresentStatusofManpowerAndVehicle.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult RequestedApplicationList(int? approvalProcessId, int? approvalStatusId, DateTime? startDate, DateTime? endDate)
        {
            var reportUrl = "~/Reports/PRM/viewers/RequestedApplicationList.aspx";
            reportUrl += "?param1=" + approvalProcessId;
            reportUrl += "&param2=" + approvalStatusId;
            reportUrl += "&param3=" + startDate;
            reportUrl += "&param4=" + endDate;

            _model.ReportPath = Url.Content(reportUrl);
            if (approvalProcessId != null)
            {
                ReportBase.BudgetInformationSession = reportUrl;
            }
            if (ReportBase.BudgetInformationSession != null)
            {
                _model.ReportPath = ReportBase.BudgetInformationSession;
            }

            return Redirect(_model.ReportPath);
        }
        public ActionResult UserLoginHistory()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/UserLoginHistory.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult EmpTrainingInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeTrainingInfo.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult EmpForeignTour()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeForeignTour.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeListWithPhoto()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeListWithPhoto.aspx");
            return View("ReportViewer", _model);
        }
        
        public ActionResult EmployeeServiceHistory()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RptEmployeeServiceHistory.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult HigherGradeSelection()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/HigherGradeSelection.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult RecreationLeave()
        {
            _model.ReportPath = Url.Content("~/Reports/PRM/viewers/RecreationLeave.aspx");
            return View("ReportViewer", _model);
        }   
        #endregion


    }
}
