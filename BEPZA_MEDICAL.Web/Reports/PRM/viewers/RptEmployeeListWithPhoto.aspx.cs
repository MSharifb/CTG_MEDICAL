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
    public partial class RptEmployeeListWithPhoto : ReportBase
    {
        #region Fields
        private readonly PRM_GenericRepository<PRM_EmploymentInfo> _empRepository;
        #endregion

        #region Ctor
        public RptEmployeeListWithPhoto()
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
                int zoneId = LoggedUserZoneInfoId;
            }
        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
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

            string strZoneId = ConvertZoneArrayListToString(arrZoneList);

            var divisionId = Convert.ToInt32(ddlDivision.SelectedValue);
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            var empTypeId = Convert.ToInt32(ddlEmploymentType.SelectedValue);
            var jobGrade = Convert.ToInt32(ddlJobGrade.SelectedValue);
            var employeeId = Convert.ToInt32(ddlEmployee.SelectedValue);

            GenerateReport(strZoneId, divisionId, designationId, empTypeId, jobGrade, employeeId);
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneId, int divisionId, int designationId, int empTypeId, int jobGradeId, int EmployeeId)
        {
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptEmployeeListwithPhoto.rdlc");

            #region Processing Report Data

            var data = (from s in base.prmContext.SP_PRM_RptEmployeeListwithPhoto(strZoneId, divisionId, designationId, empTypeId, jobGradeId, EmployeeId) select s).ToList();

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSEmpInfo", data);
            rvEmployeeInfo.LocalReport.DataSources.Add(dataSource);
            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "EmployeeInfo_withPhoto" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                    select c).ToList();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        #endregion

        #region User Methods

        private void PopulateDropdownList()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlDivision.DataSource = context.PRM_Division.Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.SortOrder).ToList();
            ddlDivision.DataValueField = "Id";
            ddlDivision.DataTextField = "Name";
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0, new ListItem("All", "0"));

            ddlEmploymentType.DataSource = context.PRM_EmploymentType.OrderBy(x => x.SortOrder).ToList();
            ddlEmploymentType.DataValueField = "Id";
            ddlEmploymentType.DataTextField = "Name";
            ddlEmploymentType.DataBind();
            ddlEmploymentType.Items.Insert(0, new ListItem("All", "0"));

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.SortingOrder).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

            var prm_SalaryScaleEntity = context.PRM_SalaryScale.Where(t => t.DateOfEffective <= DateTime.Now).OrderByDescending(t => t.DateOfEffective).First();
            ddlJobGrade.DataSource = context.PRM_JobGrade.Where(q => q.SalaryScaleId == prm_SalaryScaleEntity.Id).OrderBy(x => x.Id).ToList();
            ddlJobGrade.DataValueField = "Id";
            ddlJobGrade.DataTextField = "GradeName";
            ddlJobGrade.DataBind();
            ddlJobGrade.Items.Insert(0, new ListItem("All", "0"));

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
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}