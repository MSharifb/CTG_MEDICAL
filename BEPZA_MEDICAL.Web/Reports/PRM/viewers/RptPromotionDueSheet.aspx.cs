using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptPromotionDueSheet : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptPromotionDueSheet()
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

                string  strZoneId = ConvertZoneArrayListToString(arrZoneList);
                int departmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
                int designationId = Convert.ToInt32(ddlDesignation.SelectedValue);

                GenerateReport(strZoneId, designationId, departmentId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report
        public void GenerateReport( string ZoneId, int Designation, int DepartmentId)
        {
            #region Processing Report Data

            rvTestResult.Reset();
            rvTestResult.ProcessingMode = ProcessingMode.Local;
            rvTestResult.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptPromotionDueSheetQualificationPart.rdlc");

            var data = (from s in base.prmContext.SP_PRM_PromotionDueSheetQualification(ZoneId, Designation,DepartmentId) select s).ToList();

            #region Search parameter

            //string searchParameters = string.Empty;
            //searchParameters = ddlExamType.SelectedItem.Text;
            //ReportParameter p1 = new ReportParameter("ExamType", searchParameters);
            //rvTestResult.LocalReport.SetParameters(new ReportParameter[] { p1 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSPromotionDueSheet", data);
            rvTestResult.LocalReport.DataSources.Add(dataSource);
            this.rvTestResult.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvTestResult.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;
            var ZoneId = 0;
            var DesignationId = 0;
            var yearOfExp = 0;
            var degreeLevelId = 0;
            var jobGradeId = 0;
            if (e.ReportPath != "_ReportHeader")
            {
                DesignationId = Convert.ToInt32(e.Parameters["DesignationId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["DesignationId"].Values[0]));
                yearOfExp = Convert.ToInt32(e.Parameters["YearOfExp"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["YearOfExp"].Values[0]));
                //totalYearOfExp = Convert.ToInt32(e.Parameters["TotalYearOfExp"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["TotalYearOfExp"].Values[0]));
                jobGradeId = Convert.ToInt32(e.Parameters["JobGradeId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["JobGradeId"].Values[0]));
                //degreeLevelId = Convert.ToInt32(e.Parameters["DegreeLevelId"].Values[0] == null ? 0 : Convert.ToInt32(e.Parameters["DegreeLevelId"].Values[0]));

            }
            switch (e.ReportPath)
            {
                case "RptPromotionDueSheet":
                    data = (from s in base.prmContext.SP_PRM_PromotionDueSheet(ZoneId, DesignationId, yearOfExp, 0, jobGradeId) select s).ToList();
                    dsName = "DSPromotionDueSheetEmp";
                    break;
                case "RptPromotionDueSheetACRPart":
                    data = (from s in base.prmContext.SP_PRM_PromotionDueSheetACRPart(ZoneId, DesignationId, yearOfExp, 0, jobGradeId, degreeLevelId, 0) select s).ToList();
                    dsName = "DSPromotionDueSheetACR";
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
            ddlDepartment.DataSource = context.PRM_Division.OrderBy(x => x.SortOrder).ToList();
            ddlDepartment.DataValueField = "Id";
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("All", "0"));

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

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