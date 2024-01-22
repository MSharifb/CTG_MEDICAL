using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptACRforOfficer: ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptACRforOfficer()
        {
            _empRepository = new PRM_GenericRepository<PRM_EmploymentInfo>(new ERP_BEPZAPRMEntities());
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

        #region Button Event
        string fromDate = string.Empty;
        string icNo = string.Empty;
        string strZoneId = string.Empty;

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> zoneList = new List<int>();
                int[] arrZoneList = new int[] { };
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                arrZoneList = zoneList.ToArray();

                strZoneId = ConvertZoneArrayListToString(arrZoneList);

                fromDate = txtFromDate.Text;
                icNo = txtIcNo.Text;
                GenerateReport(fromDate, icNo, strZoneId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(string fromDate, string icNo, string strZoneId)
        {
            #region Processing Report Data

            rvTestResult.Reset();
            rvTestResult.ProcessingMode = ProcessingMode.Local;
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptACRforOfficer.rdlc");

            var data = (from s in base.prmContext.SP_PRM_RptACRforOfficerInfo(fromDate, icNo, strZoneId) select s).ToList();

            #region Search parameter
            var ChaMark = (from s in base.prmContext.PRM_EmpACRPersonalCharacteristics
                           join emp in base.prmContext.PRM_EmploymentInfo on s.EmployeeId equals emp.Id
                           join y in base.prmContext.PRM_EmpACRPersonalCharacteristicsDetail on s.OfficerInfoId equals y.OfficerInfoId
                           where (emp.EmpID == icNo)
                           select y
                          ).ToList();
            var PerMark = (from s in base.prmContext.PRM_EmpACRPerformanceOfWork
                           join emp in base.prmContext.PRM_EmploymentInfo on s.EmployeeId equals emp.Id
                           join y in base.prmContext.PRM_EmpACRPerformanceOfWorkDetail on s.OfficerInfoId equals y.OfficerInfoId
                           where (emp.EmpID == icNo)
                           select y
                          ).ToList();

            string searchParameters1 = string.Empty;

            searchParameters1 = (ChaMark.Sum(x => x.Mark) + PerMark.Sum(x => x.Mark)).ToString();

            ReportParameter p1 = new ReportParameter("TotalMark", searchParameters1);

            rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1});

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSACRforOfficer", data);
            rvTestResult.LocalReport.DataSources.Add(dataSource);
            this.rvTestResult.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTestResult.DataBind();

            //ExportToPDF
            String newFileName = "ACRForOfficer_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvTestResult, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            var empId = string.Empty;

            //if (e.ReportPath != "_ReportACRHeader")
            //{
            //    empId = e.Parameters["EmpID"].Values[0].ToString();
            //}
            switch (e.ReportPath)
            {
                case "RptACRforOfficerCoverP":
                    data = (from s in base.prmContext.SP_PRM_RptACRforOfficerInfo(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACRforOfficerCover";
                    break;
                    
                case "RptACR_Education":
                    data = (from s in base.prmContext.SP_PRM_RptACREducationQuaInfo(icNo) select s).ToList();
                    dsName = "DSACREducation";
                    break;

                case "RptACR_TrainingInfo":
                    data = (from s in base.prmContext.SP_PRM_RptACRTrainingInfo(icNo) select s).ToList();
                    dsName = "DSACRTraining";
                    break;

                case "RptACR_LanguageEffi":
                    data = (from s in base.prmContext.SP_PRM_RptACRLanguageEfficiency(icNo) select s).ToList();
                    dsName = "DSACRLanguage";
                    break;

                case "RptACRforOfficerPersonalCharacteristics":
                    data = (from s in base.prmContext.SP_PRM_RptACRforOfficerPersonalCharacteristics(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACROfficerPersonalCharacteristics";
                    break;

                case "RptACRforOfficerPerformanceOfWork":
                    data = (from s in base.prmContext.SP_PRM_RptACRforOfficerPerformanceOfWork(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACROfficerPerformanceOfWork";
                    break;

                case "RptACRforOfficer56Part":
                    data = (from s in base.prmContext.SP_PRM_RptACRforOfficerInfo5678part(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACROfficer56Part";
                    break;

                case "RptACRforOfficer78Part":
                    data = (from s in base.prmContext.SP_PRM_RptACRforOfficerInfo5678part(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACROfficer78Part";
                    break;

                case "_ReportACRHeader":
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    dsName = "DSCompanyInfo";
                    break;

                default:
                    break;
            }
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }
        #endregion

        protected void rvTestResult_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }
        #endregion

    }
}