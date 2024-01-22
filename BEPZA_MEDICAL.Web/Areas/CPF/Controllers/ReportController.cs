using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Areas.CPF.Models.Report;
using System;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.CPF.Controllers
{
    public class ReportController : Controller
    {
        #region Fields

        private readonly ReportViewModel model;

        #endregion

        #region Ctor

        public ReportController()
        {
            model = new ReportViewModel();
        }

        #endregion

        public ActionResult CPFReportMaster(int Id)
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Id);
            model.Id = Id;
            return View("ReportViewer", model);
        }

        public ActionResult IndividualMonthlyPFStatement()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Individual_Monthly_PF_Statement));
            return View("ReportViewer", model);
        }

        public ActionResult MonthlyPFStatement()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Monthly_PF_Statement));
            return View("ReportViewer", model);
        }

        public ActionResult MonthlyPFandLoanStatement1()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_1));
            return View("ReportViewer", model);
        }
        public ActionResult MonthlyPFandLoanStatement2()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_2));
            return View("ReportViewer", model);
        }

        public ActionResult MonthlyRefundableLoanStatement()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement));
            return View("ReportViewer", model);
        }
        public ActionResult MonthlyNonRefundableLoanStatement()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement));
            return View("ReportViewer", model);
        }

        public ActionResult IndividualLoanCollectionStatement1()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1));
            return View("ReportViewer", model);
        }
        public ActionResult IndividualLoanCollectionStatement2()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2));
            return View("ReportViewer", model);
        }

        public ActionResult PFMembershipApplicationForm()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Application_Form));
            return View("ReportViewer", model);
        }

        public ActionResult PFNomineeForm()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.PF_Nominee_Form));
            return View("ReportViewer", model);
        }

        public ActionResult PFLoanApplicationForm()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.PF_Loan_Application_Form));
            return View("ReportViewer", model);
        }

        public ActionResult MyPFStatement()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.My_PF_Statement));
            return View("ReportViewer", model);
        }

        public ActionResult PFMembershipActiveInactive()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Active_Inactive));
            return View("ReportViewer", model);
        }
        public ActionResult PFStatementHistory()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFReportMaster.aspx?Id=" + Convert.ToByte(CPFEnum.CPFReports.PF_Statement_History));
            return View("ReportViewer", model);
        }

        //-------------------
        public ActionResult CPFYearlyStatement()
        {
            model.ReportPath = Url.Content("~/Reports/CPF/viewers/CPFYearlyStatement.aspx");
            return View("ReportViewer", model);
        }

    }
}
