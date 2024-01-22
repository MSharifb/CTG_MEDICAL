using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.ComponentModel;

using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using System.Collections.Generic;
using System.Web.Services;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class EmployeeInfo : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        private readonly PRM_GenericRepository<PRM_EmpFamilyMemberInfo> _empFamRepository;
        //private readonly IWM_MFSPRMEntities _context;
        #endregion

        #region Ctor
        public EmployeeInfo(/*EmployeeService empService*/)
        {
            _empRepository = new PRM_GenericRepository<PRM_EmploymentInfo>(new ERP_BEPZAPRMEntities());
            _empFamRepository = new PRM_GenericRepository<PRM_EmpFamilyMemberInfo>(new ERP_BEPZAPRMEntities());

            //_context = new IWM_MFSPRMEntities();
        }
        #endregion

        #region Page Event

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlEmployee.DataSource = context
                .PRM_EmploymentInfo
                .Select(q => new
                {
                    ZoneInfoId = q.ZoneInfoId,
                    EmpID = q.EmpID,
                    DisplayText = q.FullName + " [" + q.EmpID + "]"
                }).ToList()
                .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId)
                .OrderBy(x => x.DisplayText);
            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", ""));

        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var empID = ddlEmployee.SelectedValue;
                var data = (from c in base.context.vwEmployeeInfoRpts
                            where c.EmpID == empID
                            select c).ToList();
                if (data.Count() > 0)
                {
                    GenerateEmployeeInfoReport(empID, data);
                    this.lblMsg.Text = string.Empty;
                }
                else
                {
                    rvEmployeeInfo.Reset();
                    lblMsg.Text = string.Empty;
                    lblMsg.Text = BEPZA_MEDICAL.Web.Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateEmployeeInfoReport(string empId, List<vwEmployeeInfoRpt> data)
        {
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/EmployeeInfo.rdlc");


            #region Processing Report Data

            #endregion
            ReportDataSource dataSource = new ReportDataSource("DSEmployeeInformationRpt", data);
            rvEmployeeInfo.LocalReport.DataSources.Add(dataSource);
            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "EmployeeInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);

        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            var empId = 0;

            if (e.ReportPath != "_ReportHeader")
            {
                empId = Convert.ToInt32(e.Parameters["EmpId"].Values[0].ToString());
            }

            switch (e.ReportPath)
            {
                case "EmployeeInfo_AcademicInfo":
                    data = (from ef in base.context.vwEmpDegreeRpts
                            where ef.EmployeeId == empId
                            select ef).OrderByDescending(x => x.YearOfPassing).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_ServiceHistoryInfo":
                    data = (from ef in base.context.vwEmpServiceHistoryRpts
                            where ef.EmployeeId == empId
                            select ef).OrderBy(s=>s.OrderDate).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_TrainingInfo":
                    data = (from ef in base.context.vwEmpTrainingRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_ForeignTourInfo":
                    data = (from ef in base.context.vwEmpForeignTourRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_ExperienceInfo":
                    data = (from ef in base.context.vwEmpExperienceRpts
                            where ef.EmployeeId == empId
                            select ef).OrderByDescending(x => x.EndDate).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_LeverageInfo":
                    data = (from ef in base.context.vwEmpLeverageInfoRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_WealthInfo":
                    data = (from ef in base.context.vwEmpWealthInfoRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_LanguageProfience":
                    data = (from ef in base.context.vwLanguageProficiencyRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_CertificateInfo":
                    data = (from ef in base.context.vwEmpCertificationRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;
                case "EmployeeInfo_SkillInfo":
                    data = (from ef in base.context.vwEmpJobSkillRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_FamilyInfo":
                    data = (from ef in base.context.vwEmpFamilyInfoRpts
                            where ef.EmployeeId == empId
                            select ef).ToList();
                    dsName = "DataSet1";
                    break;

                case "EmployeeInfo_NomineeInfo":
                    data =  base.context.SP_PRM_EmpNomineeInfo(empId);
                    dsName = "DataSet1";
                    break;

                case "_ReportHeader":
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    dsName = "DSCompanyInfo";
                    break;

                default:
                    break;
            }

            e.DataSources.Add(new ReportDataSource(dsName, data));

        }

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #endregion

        [WebMethod]
        public static string GetEmployeeNameByEmpId(string empId)
        {
            ReportBase dbContext = new ReportBase();
            string employeeName = string.Empty;
            var EmpName = (from empInfo in dbContext.context.PRM_EmploymentInfo
                           where empInfo.EmpID.Trim() == empId.Trim()
                           //&& empInfo.ZoneInfoId == LoggedUserZoneInfoId
                           select empInfo.FullName).FirstOrDefault();
            if (EmpName != null)
            {
                employeeName = EmpName.ToString();
            }
            //return new JavaScriptSerializer().Serialize(employeeName);
            return employeeName;
        }


    }
}
