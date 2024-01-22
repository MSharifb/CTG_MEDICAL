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
    public partial class RptACRforStaff : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptACRforStaff()
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
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptACRforStaff.rdlc");

            var data = (from s in base.prmContext.SP_PRM_RptACRforStaffInfo(fromDate, icNo, strZoneId) select s).ToList();

            #region Search parameter

            //var parmList = (from s in base.prmContext.SP_PRM_RptPromotionSheetForOfficerHeadInfo(fromDate, departmentId, DesignationId) select s).DistinctBy(x => x.Name).ToList();

            //string searchParameters1 = string.Empty;
            //string searchParameters2 = string.Empty;
            //string searchParameters3 = string.Empty;
            //string searchParameters4 = string.Empty;
            //string searchParameters5 = string.Empty;
            //string searchParameters6 = string.Empty;
            //string searchParameters7 = string.Empty;

            //foreach (var item in parmList)
            //{
            //    searchParameters1 = item.Experience;
            //    searchParameters3 = item.SanctionedPost.ToString();
            //    searchParameters4 = item.forDirectRec.ToString();
            //    searchParameters5 = item.forPromotion.ToString();
            //    searchParameters6 = item.PayScale;
            //    searchParameters7 = item.GradeName;
            //}
            //searchParameters2 = ddlDesignation.SelectedItem.Text;

            //ReportParameter p1 = new ReportParameter("Qualification", searchParameters1);
            //ReportParameter p2 = new ReportParameter("Possition", searchParameters2);
            //ReportParameter p3 = new ReportParameter("ClrPost", searchParameters3);
            //ReportParameter p4 = new ReportParameter("DisrectRec", searchParameters4);
            //ReportParameter p5 = new ReportParameter("PromotionRec", searchParameters5);
            //ReportParameter p6 = new ReportParameter("SalaryScale", searchParameters6);
            //ReportParameter p7 = new ReportParameter("Grade", searchParameters7);

            //rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7});

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSACRforStaff", data);
            rvTestResult.LocalReport.DataSources.Add(dataSource);
            this.rvTestResult.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTestResult.DataBind();

            //ExportToPDF
            String newFileName = "ACRForStaff_" + Guid.NewGuid() + ".pdf";
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

                case "RptACRforStaff_Assessment":
                    data = (from s in base.prmContext.SP_PRM_RptACRforStaffAssessmentInfo(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACRStaffAssessment";
                    break;

                case "RptACRforStaff_AssessmentRptRView":
                    data = (from s in base.prmContext.SP_PRM_RptACRforStaffAssessmentRptReview(fromDate, icNo, strZoneId) select s).ToList();
                    dsName = "DSACRStaffAssessmentRptRView";
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