using BEPZA_MEDICAL.DAL.CPF;
using BEPZA_MEDICAL.Utility;
using BEPZA_MEDICAL.Web.Reports.CPF.CpfCommon;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace BEPZA_MEDICAL.Web.Reports.CPF.viewers
{
    public partial class CpfReportMaster : ReportBase
    {
        #region Properties

        public string FromEmailAddress;
        public string ToEmailAddress;
        public string CCEmailAddress;
        public string BCCEmailAddress;
        public string Subject;
        public string Salutation;
        public string BodyHeader;
        public string BodyFooter;
        public string FromEmailPersonName;
        public string EmailBody;
        public string tableHeaderBackgroundColor;
        public string tableBorderColor;
        public string NoRecordFound;
        public bool IsNoRecord = true;
        public string Smpt;
        public string FromemailPassword;

        #endregion

        #region Event

        private int selectedReportId = 0;
        private bool IsNomineeBtnClicked = false;
        bool status = false;
        ERP_BEPZACPFEntities _context = new ERP_BEPZACPFEntities();
        CPF_ReportsCommon common = new CPF_ReportsCommon();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "LoadEmployeeAutocmplete", "LoadEmployeeAutocmplete();", true);

            //TODO: Load employee by selected zone wise

            if (Request.QueryString.Count > 0 && Request.QueryString["Id"] != null)
            {
                selectedReportId = int.Parse(Request.QueryString["Id"].ToString());
            }

            //search field modification
            ddlSelectYear.Attributes.Add("class", "required");
            ddlSelectMonth.Attributes.Add("class", "required");
            divEmpInfo.Visible = false;
            btnSendMail.Visible = false;
            btnViewNomineeReport.Visible = false;
            txtEmployeeId.Attributes.Remove("required");
            divMonthYear.Visible = true;
            cbxIsRunning.Visible = false;
            cbxIsInactive.Visible = false;
            LoadSearchFields(selectedReportId);
            //end

            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                int emplyeeId = default(int);
                bool loanStatus = IsRunning.Checked;
                bool isInactive = IsInactive.Checked;

                if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.My_PF_Statement))
                {
                    txtEmployeeId.Text = MyAppSession.EmpId;
                }

                if (txtEmployeeId.Text != string.Empty)
                {
                    var empId = txtEmployeeId.Text.Trim().ToString();
                    emplyeeId = (from p in context.PRM_EmploymentInfo
                                 where p.EmpID.Trim().ToUpper() == empId.Trim().ToUpper()
                                 select p.Id).FirstOrDefault();
                }

                var list = new List<BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo>();
                BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo obj = null;

                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        obj = new BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo();
                        obj.Id = Convert.ToInt32(item.Value);
                        obj.ZoneName = item.Text;
                        list.Add(obj);
                    }
                }

                GenerateReport(selectedYear, selectedMonth, emplyeeId, list, loanStatus, isInactive);

                lblMsg.Text = default(string);
                if (status == true)
                {
                    lblMsg.Text = BEPZA_MEDICAL.Web.Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void GenerateReport(string selectedYear, string selectedMonth, int emplyeeId, IEnumerable<BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo> zoneList, bool IsRunningLoan, bool IsInactive)
        {

            try
            {
                bool IsRefundable = false;
                string toMonth = string.Empty;
                string toYear = string.Empty;
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                if (IsNomineeBtnClicked)
                {
                    selectedReportId = Convert.ToByte(CPFEnum.CPFReports.PF_Nominee_Form);
                }

                BEPZA_MEDICAL.Utility.CPFEnum.CPFReports reportType = (BEPZA_MEDICAL.Utility.CPFEnum.CPFReports)Enum.Parse(typeof(BEPZA_MEDICAL.Utility.CPFEnum.CPFReports), selectedReportId.ToString());

                if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement))
                {
                    IsRefundable = true;
                }

                var data = common.GetReportData(reportType, selectedMonth, toMonth, selectedYear, toYear, emplyeeId, IsRefundable, IsRunningLoan, IsInactive, zoneList, numErrorCode, strErrorMsg);

                String newFileName = string.Empty;
                switch (reportType)
                {
                    case CPFEnum.CPFReports.Individual_Monthly_PF_Statement:
                        data = data as IList<CPF_SP_RptIndividualMonthlyPFStatement_Result>;
                        newFileName = "IndividualMonthlyPFStatement_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.My_PF_Statement:
                        data = data as IList<CPF_SP_RptIndividualMonthlyPFStatement_Result>;
                        newFileName = "MyPFStatement_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Monthly_PF_Statement:
                        data = data as IList<CPF_SP_RptMonthlyPFStatementMaster_Result>;
                        newFileName = "MonthlyPFStatement_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_1:
                        data = data as IList<CPF_SP_RptMonthlyPFStatement_Result>;
                        newFileName = "MonthlyPFAndLoanStatement1_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Monthly_PF_and_Loan_Statement_2:
                        data = data as IList<CPF_SP_RptMonthlyPFStatement_Result>;
                        newFileName = "MonthlyPFAndLoanStatement2_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement:
                        data = data as IList<CPF_SP_PFListOfLoan_Result>;
                        newFileName = "MonthlyRefundableLoan_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement:
                        data = data as IList<CPF_SP_PFListOfLoan_Result>;
                        newFileName = "MonthlyNonRefundableLoan_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1:
                        data = data as IList<CPF_SP_RptMonthlyPFStatement_Result>;
                        newFileName = "IndividualLoanCollectionStatement1_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2:
                        data = data as IList<CPF_SP_PFListOfLoan_Result>;
                        newFileName = "IndividualLoanCollectionStatement2_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.PF_Membership_Application_Form:
                        data = data as IList<CPF_RptCpfMemberInformation_Result>;
                        newFileName = "MembershipApplicationForm_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.PF_Nominee_Form:
                        data = data as IList<CPF_SP_RptNomineeFormInformation_Result>;
                        newFileName = "NomineeForm_" + Guid.NewGuid() + ".pdf";
                        break;
                    case CPFEnum.CPFReports.PF_Loan_Application_Form:
                        data = data as IList<CPF_GetMyCPFLoanApplication_Result>;
                        newFileName = "LOANForm_" + Guid.NewGuid() + ".pdf";
                        break;

                    case CPFEnum.CPFReports.PF_Membership_Active_Inactive:
                        data = data as IList<CPF_SP_RptNewJoinInMembership_Result>;
                        newFileName = "PF_Membership_" + Guid.NewGuid() + ".pdf";
                        break;

                    case CPFEnum.CPFReports.PF_Statement_History:
                        data = data as IList<CPF_SP_RptPFStatementHistory_Result>;
                        newFileName = "IndividualPFStatementHistory_" + Guid.NewGuid() + ".pdf";
                        break;

                    default:
                        data = null;
                        break;

                }
                var reportHeader = base.GetZoneInfoForReportHeader();

                if (data.Count > 0 && !string.IsNullOrEmpty(selectedYear) && !string.IsNullOrEmpty(selectedMonth))
                {
                    lblMsg.Text = "";
                    rvPFStatementRpt.Reset();
                    rvPFStatementRpt.ProcessingMode = ProcessingMode.Local;
                    rvPFStatementRpt.LocalReport.ReportPath = Server.MapPath(common.GetReportPath(reportType));

                    rvPFStatementRpt.LocalReport.DataSources.Add(new ReportDataSource(common.GetReportDataSetName(reportType).ToString(), data));
                    rvPFStatementRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));
                    string parameterLine1 = string.Empty;

                    #region Loan Filtering

                    if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement))
                    {
                        parameterLine1 = "LIST OF REFUNDABLE LOAN";
                        if (IsRunningLoan == true)
                        {
                            rvPFStatementRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/CPF/rdlc/Monthly_Running_Loan_Statement.rdlc");
                        }
                    }
                    if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement))
                    {
                        parameterLine1 = "LIST OF NON-REFUNDABLE LOAN";
                        if (IsRunningLoan == true)
                        {
                            rvPFStatementRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/CPF/rdlc/Monthly_Running_Loan_Statement.rdlc");
                        }
                    }
                    #endregion

                    string parameterLine2 = "Statement for the month of " + selectedMonth.Substring(0, 3) + "-" + selectedYear;


                    if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Active_Inactive))
                    {
                        if (selectedMonth == "[Select One]")
                            parameterLine2 = "Statement for the year of " + selectedYear;
                        if (IsInactive)
                            parameterLine2 = string.Concat(parameterLine2, " (Inactive)");
                        else
                            parameterLine2 = string.Concat(parameterLine2, " (Active)");
                    }

                    if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Application_Form) || selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Nominee_Form) || selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Loan_Application_Form))
                    {
                        //nothing for parameters
                    }
                    else
                    {
                        ReportParameter p1 = new ReportParameter("titleParam", parameterLine1);
                        ReportParameter p2 = new ReportParameter("param", parameterLine2);
                        rvPFStatementRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                    }
                }
                else
                {
                    status = true;
                    rvPFStatementRpt.Reset();
                }
                if (selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Application_Form) || selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Nominee_Form) || selectedReportId == Convert.ToByte(CPFEnum.CPFReports.PF_Loan_Application_Form))
                {
                    //nothing for subreport
                }
                else
                {
                    this.rvPFStatementRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                }
                rvPFStatementRpt.DataBind();

                //ExportToPDF
                String newFilePath = "~/Content/TempFiles/" + newFileName;
                FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
                BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvPFStatementRpt, newFileName, fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //try
            //{
            //    var dsName = string.Empty;
            //    var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            //    var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            //    var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
            //    var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
            //    dynamic data = null;
            //    switch (e.ReportPath)
            //    {
            //        case "_CPFReportHeader":

            //            dsName = "DSCompanyInfo";
            //            data = (from c in base.context.PRM_CompanyInfo
            //                    select c).ToList();
            //            e.DataSources.Add(new ReportDataSource(dsName, data));
            //            break;

            //        case "_ReportHeaderA4Portrait":

            //            dsName = "DSCompanyInfo";
            //            data = (from c in base.context.PRM_CompanyInfo
            //                    select c).ToList();
            //            e.DataSources.Add(new ReportDataSource(dsName, data));
            //            break;

            //        case "_ReportHeaderA4LandS":

            //            dsName = "DSCompanyInfo";
            //            data = (from c in base.context.PRM_CompanyInfo
            //                    select c).ToList();
            //            e.DataSources.Add(new ReportDataSource(dsName, data));
            //            break;
            //    }
            //    // _ReportHeaderA4LandS

            //}
            //catch (Exception ex)
            //{
            //    lblMsg.Text = ex.Message;
            //}
        }

        protected void rvPFStatementRpt_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        protected void ddlReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSearchFields(selectedReportId);
        }

        protected void btnViewNomineeReport_Click(object sender, EventArgs e)
        {
            IsNomineeBtnClicked = true;
            btnViewReport_Click(sender, e);
        }
        #endregion

        #region Private Methods

        private void PopulateDropdownList()
        {
            // Populate report list
            //var list = Common.GetEnumAsDictionary<CPFEnum.CPFReports>(true);
            //foreach (KeyValuePair<int, string> kvItem in list)
            //{
            //    ddlReport.Items.Add(new ListItem(kvItem.Value, kvItem.Key.ToString()));
            //}
            //ddlReport.DataBind();
            //if (selectedReportId > 0)
            //{
            //    ddlReport.SelectedValue = selectedReportId.ToString();
            //}
            //--

            int j = 0;
            foreach (var year in BEPZA_MEDICAL.Web.Utility.Common.PopulateYearList().ToList())
            {
                ddlSelectYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                j++;
            }
            ddlSelectYear.Items.Insert(0, new ListItem("[Select One]", "0"));
            ddlSelectYear.Items.FindByValue(DateTime.Now.Year.ToString()).Selected = true;
            int k = 0;
            foreach (var month in BEPZA_MEDICAL.Web.Utility.Common.PopulateMonthListForReport().ToList())
            {
                ddlSelectMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                k++;
            }
            ddlSelectMonth.Items.Insert(0, new ListItem("[Select One]", "0"));
            ddlSelectMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;

            ddlZone.DataSource = context.PRM_ZoneInfo.ToList();
            ddlZone.DataValueField = "Id";
            ddlZone.DataTextField = "ZoneName";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

        }

        protected void LoadSearchFields(int reportId)
        {
            if(reportId == Convert.ToByte(CPFEnum.CPFReports.My_PF_Statement))
            {
                divMonthYear.Visible = false;
                divZoneInfo.Visible = false;
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.Individual_Monthly_PF_Statement) || reportId == Convert.ToByte(CPFEnum.CPFReports.Individual_Loan_Collection_Statement_1) || reportId == Convert.ToByte(CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2))
            {
                cbxIsRunning.Visible = false;
                divZoneInfo.Visible = false;
                divEmpInfo.Visible = true;
                txtEmployeeId.Attributes.Add("class", "required");
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.Monthly_Refundable_Loan_Statement) || reportId == Convert.ToByte(CPFEnum.CPFReports.Monthly_Non_Refundable_Loan_Statement))
            {
                cbxIsRunning.Visible = true;
                divEmpInfo.Visible = false;
                txtEmployeeId.Attributes.Remove("required");
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Application_Form) || reportId == Convert.ToByte(CPFEnum.CPFReports.PF_Nominee_Form))
            {
                divMonthYear.Visible = false;
                cbxIsRunning.Visible = false;
                divEmpInfo.Visible = true;
                txtEmployeeId.Attributes.Add("class", "required");
                ddlSelectYear.Attributes.Remove("required");
                ddlSelectMonth.Attributes.Remove("required");
                btnViewNomineeReport.Visible = true;
                btnViewReport.Text = "View Membership Report";
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.Individual_Loan_Collection_Statement_2))
            {
                cbxIsRunning.Visible = true;
                divEmpInfo.Visible = true;
                txtEmployeeId.Attributes.Add("class", "required");
                ddlSelectYear.Attributes.Remove("required");
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.Monthly_PF_Statement))
            {
                btnSendMail.Visible = false;
                divZoneInfo.Visible = false;
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.PF_Loan_Application_Form))
            {
                divMonthYear.Visible = false;
                cbxIsRunning.Visible = false;
                divZoneInfo.Visible = false;

                divEmpInfo.Visible = true;
                txtEmployeeId.Attributes.Add("class", "required");
                ddlSelectYear.Attributes.Remove("required");
                ddlSelectMonth.Attributes.Remove("required");
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.PF_Membership_Active_Inactive))
            {
                btnSendMail.Visible = false;
                divZoneInfo.Visible = false;
                cbxIsInactive.Visible = true;

                //ddlSelectMonth.cl = ddlSelectMonth.CssClass.Replace("required valid", "");
                ddlSelectMonth.Attributes["class"] = " ";
            }
            if (reportId == Convert.ToByte(CPFEnum.CPFReports.PF_Statement_History))
            {
                cbxIsRunning.Visible = false;
                divZoneInfo.Visible = false;
                divEmpInfo.Visible = true;
                txtEmployeeId.Attributes.Add("class", "required");
            }
        }

        #endregion

        #region Send mail
        [NoCache]
        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = default(string);

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                int emplyeeId = default(int);
                bool loanStatus = IsRunning.Checked;
                bool isInactive = IsInactive.Checked;

                if (txtEmployeeId.Text != string.Empty)
                {
                    var empId = txtEmployeeId.Text.Trim().ToString();
                    emplyeeId = (from p in context.PRM_EmploymentInfo
                                 where p.EmpID.Trim().ToUpper() == empId.Trim().ToUpper()
                                 select p.Id).FirstOrDefault();
                }

                var list = new List<BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo>();
                BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo obj = null;
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        obj = new BEPZA_MEDICAL.DAL.PRM.PRM_ZoneInfo();
                        obj.Id = Convert.ToInt32(item.Value);
                        obj.ZoneName = item.Text;
                        list.Add(obj);
                    }
                }

                string toMonth = string.Empty;
                string toYear = string.Empty;

                bool IsRefundable = false;

                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                #region Get Data

                BEPZA_MEDICAL.Utility.CPFEnum.CPFReports reportType = (BEPZA_MEDICAL.Utility.CPFEnum.CPFReports)Enum.Parse(typeof(BEPZA_MEDICAL.Utility.CPFEnum.CPFReports), selectedReportId.ToString());

                var data = common.GetReportData(reportType, selectedMonth, toMonth, selectedYear, toYear, emplyeeId, IsRefundable, loanStatus, isInactive, list, numErrorCode, strErrorMsg);

                #endregion

                #region Get Sender mail Info

                #endregion

                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        int EmpID = Convert.ToInt32(item.EmployeeId);
                        ToEmailAddress = prmContext.PRM_EmploymentInfo.Where(q => q.Id == EmpID).SingleOrDefault().EmialAddress == null ? "" : prmContext.PRM_EmploymentInfo.Where(q => q.Id == EmpID).SingleOrDefault().EmialAddress;
                        CCEmailAddress = "";
                        BCCEmailAddress = "";
                        Subject = "Monthly Provident Fund Statement";


                        #region Check Velidation

                        //if (ToEmailAddress == string.Empty && txtEmployeeID.Text != string.Empty)
                        //{
                        //    lblMsg.Text = "Receiver mail address is empty.";
                        //}
                        //else if (FromEmailAddress != string.Empty && ToEmailAddress != string.Empty && txtEmployeeID.Text != string.Empty)
                        //{
                        //    lblMsg.Text = "Email has been sent successfully!";
                        //}

                        #endregion

                        /********************/

                        var smtp = WebConfigurationManager.AppSettings["smtp"];
                        string Fromemail = WebConfigurationManager.AppSettings["FromEmail"];
                        string password = WebConfigurationManager.AppSettings["FromEmailPassword"];

                        FromEmailAddress = Fromemail;
                        FromemailPassword = password;
                        /********************/

                        if (ToEmailAddress != string.Empty && FromEmailAddress != string.Empty)
                        {
                            string body = CreateBody(item);

                            EmailProcessor email = new EmailProcessor();
                            email.SendEmail(Smpt, FromEmailAddress, FromemailPassword, ToEmailAddress, CCEmailAddress, BCCEmailAddress, Subject, body, "");
                        }
                    }

                }
                else
                {
                    status = true;
                }

                if (status == true)
                {
                    lblMsg.Text = BEPZA_MEDICAL.Web.Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        [NoCache]
        private string CreateBody(dynamic item)
        {
            var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
            var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
            //var jobGradeId = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
            //var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
            //var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
            //var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            string content = string.Empty;

            try
            {
                DateTime rr = Convert.ToDateTime(System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/01");

                DateTime startDate = rr.AddMonths(1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                if (item != null)
                {
                    var EmployeeId = item.EmployeeId;
                    var EmpId = item.EmpID;

                    var name = item.FullName;
                    var Designation = item.Designation;
                    var zoneName = item.ZoneName;

                    string monthYear = item.SalaryMonth + ", " + item.SalaryYear;

                    decimal currentOwnContrib = item.PFOwn
                        , currentComContrib = item.PFCompany
                        , currentOwnInt = item.ProfitOwn
                        , currentComInt = item.ProfitCompany
                        , prevWonContrib = item.PFTotalOwn - item.PFOwn
                        , prevComContrib = item.PFTotalCompany - item.PFCompany
                        , prevOwnInt = item.ProfitTotalOwn - item.ProfitOwn
                        , prevComInt = item.ProfitTotalCompany - item.ProfitCompany
                        , totalContrib = 0
                        , totalInt = 0
                        , grandTotal = 0;

                    totalContrib = currentOwnContrib + currentComContrib + prevWonContrib + prevComContrib;
                    totalInt = currentOwnInt + currentComInt + prevOwnInt + prevComInt;
                    grandTotal = totalContrib + totalInt;

                    content = @"<div style='text-align:center; width:80%; font-weight:bold; font-size:16px; margin:0 auto;padding:5px 0;'>Monthly Provident Fund Statement</div>";
                    content += @"<div style='text-align:center; font-size:15px;'>For the month of " + monthYear + "</div>";
                    content += @"<div style='text-align:center; font-size:15px; margin:3px 0;'>" + zoneName + "</div>";

                    content += "<div style='margin-top:50px; margin-left:50px; font-size:14px; width:50%; color:#000000;'>";
                    content += "<table style='width:80%; font-size:12px; font-family:verdana;'>";
                    content += "<tr><td style='width:40%;'>Employee ID</td> <td style='width:1%'>:</td><td style='padding:3px 5px;'>" + EmpId + "</td></tr>";
                    content += "<tr><td style='width:40%;'>Name</td><td style='width:1%'>:</td><td style='padding:3px 5px;'>" + name + "</td></tr>";
                    content += "<tr><td style='width:40%;'>Designation</td><td style='width:1%'>:</td><td style='padding:3px 5px;'>" + Designation + "</td></tr>";
                    content += "</table>";
                    content += "</div>";


                    content += "<div style='margin-top:20px; margin-left:50px; width:60%; color:#000000;'>";
                    content += "<table style='border-width:1px; border-color:#2c3e50;border-style:solid; border-collapse: separate; font-family:verdana; font-size:12px;'>";
                    content += "<thead>";
                    content += "<tr style='background:#2c3e50; color:#FFFFFF; font-weight:bold;'><th style='padding:5px 10px; width:300px; text-align:center;'>Description</th><th style='padding:5px 10px; width:100px; text-align:right;'>Contribution</th><th style='padding:5px 10px; width:100px; text-align:right;'>Interest</th></tr>";
                    content += "</thead>";
                    content += "<tbody>";
                    content += "<tr><td colspan='3' style='font-weight:bold; color:#2c3e50; padding:3px'>Contribution in " + monthYear + "</td></tr>";
                    content += "<tr><td style='padding-left:50px;'>Own Contribution</td><td style='text-align:right;'>" + currentOwnContrib.ToString("N2") + "</td><td style='text-align:right;'>" + currentOwnInt.ToString("N2") + "</td></tr>";
                    content += "<tr><td style='padding-left:50px;'>BEPZA Contribution</td><td style='text-align:right;'>" + currentComContrib.ToString("N2") + "</td><td style='text-align:right;'>" + currentComInt.ToString("N2") + "</td></tr>";
                    content += "<tr><td colspan='3' style='font-weight:bold; color:#2c3e50; padding:3px'>Previous Contribution</td></tr>";
                    content += "<tr><td style='padding-left:50px;'>Own Contribution</td><td style='text-align:right;'>" + prevWonContrib.ToString("N2") + "</td><td style='text-align:right;'>" + prevOwnInt.ToString("N2") + "</td></tr>";
                    content += "<tr><td style='padding-left:50px;'>BEPZA Contribution</td><td style='text-align:right;'>" + prevComContrib.ToString("N2") + "</td><td style='text-align:right;'>" + prevComInt.ToString("N2") + "</td></tr>";
                    content += "</tbody>";
                    content += "<tfoot>";
                    content += "<tr><td style='font-weight:bold; color:#2c3e50; padding:3px'>Total</td><td style='text-align:right; font-weight:bold; color:#2c3e50; border-top:1px solid #2c3e50'>" + totalContrib.ToString("N2") + "</td><td style='text-align:right; font-weight:bold; color:#2c3e50; border-top:1px solid #2c3e50'>" + totalInt.ToString("N2") + "</td></tr>";
                    content += "<tr><td colspan='3' style='font-weight:bold; color:#2c3e50; padding:20px 5px 5px 5px'>Total (Contribution + Interest) = " + grandTotal.ToString("N2") + "</td></tr>";
                    content += "</tfoot>";
                    content += "</table>";
                    content += "</div>";
                }
                else
                {
                    IsNoRecord = false;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
            }

            return content;
        }

        #endregion

    }
}