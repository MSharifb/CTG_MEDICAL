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
    public partial class RptListofEligibleforPromotionEmployee : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptListofEligibleforPromotionEmployee()
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
                int departmentId = Convert.ToInt32(ddlDivision.SelectedValue);
                GenerateReport(departmentId, strZoneId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport(int DepartmentId, string strZoneId)
        {
            #region Processing Report Data

            rvTestResult.Reset();
            rvTestResult.ProcessingMode = ProcessingMode.Local;
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptListofEligibleforPromotionEmployee.rdlc");

            var data = (from s in base.prmContext.SP_PRM_EligibleforPromotion(strZoneId, DepartmentId) select s).ToList();

            #region Search parameter

            //string searchParameters = string.Empty;
            //searchParameters = ddlExamType.SelectedItem.Text;
            //ReportParameter p1 = new ReportParameter("ExamType", searchParameters);
            //rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSListofEligibleforPromotionEmployee", data);
            rvTestResult.LocalReport.DataSources.Add(dataSource);
            this.rvTestResult.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTestResult.DataBind();

            //ExportToPDF
            String newFileName = "EligibleEmployeeList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvTestResult, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            var DesignationId = 0;
            var yearOfExp = 0;
            var totalYearOfExp = 0;
            var jobGradeId = 0;
            if (e.ReportPath != "_ReportHeader")
            {
                DesignationId = Convert.ToInt32(e.Parameters["DesignationId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["DesignationId"].Values[0]));
                yearOfExp = Convert.ToInt32(e.Parameters["yearOfExp"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["yearOfExp"].Values[0]));
                totalYearOfExp = Convert.ToInt32(e.Parameters["TotalYearOfExp"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["TotalYearOfExp"].Values[0]));
                jobGradeId = Convert.ToInt32(e.Parameters["JobGradeId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["JobGradeId"].Values[0]));
            }
            switch (e.ReportPath)
            {
                case "RptEligibleforPromotionEmployee":
                    data = (from s in base.prmContext.SP_PRM_EligibleforPromotionEmployee(strZoneId, DesignationId, yearOfExp, totalYearOfExp, jobGradeId, 0, 0) select s).ToList();
                    dsName = "DSListofEligibleforPromotionEmployee";
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
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlDivision.DataSource = context.PRM_Division.OrderBy(x => x.SortOrder).ToList();
            ddlDivision.DataValueField = "Id";
            ddlDivision.DataTextField = "Name";
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

        }
        #endregion

        protected void rvTestResult_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}