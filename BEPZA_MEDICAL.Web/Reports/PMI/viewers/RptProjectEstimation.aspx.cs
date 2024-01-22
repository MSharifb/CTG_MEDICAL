using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.DAL.PMI;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Objects;
using System.IO;

namespace BEPZA_MEDICAL.Web.Reports.PMI.viewers
{
    public partial class RptProjectEstimation : ReportBase
    {
        #region Fields

        bool checkStatus;

        #endregion

        #region Ctor
        public RptProjectEstimation()
        {
            //
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

        int nameOfWorksId = 0;
        string strZoneId = string.Empty;


        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                checkStatus = false;

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

                if (ddlNameOfWorks.SelectedIndex > 0)
                {
                    nameOfWorksId = Convert.ToInt32(ddlNameOfWorks.SelectedValue);
                }

                GenerateReport(strZoneId, nameOfWorksId);

                lblMsg.Text = "";

                if (checkStatus == true)
                {
                    lblMsg.Text = Utility.Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvProjectEstimation.Reset();
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
        public void GenerateReport(string strZoneId, int nameOfWorksId)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            #region Processing Report Data

            rvProjectEstimation.Reset();
            rvProjectEstimation.ProcessingMode = ProcessingMode.Local;
            rvProjectEstimation.LocalReport.ReportPath = Server.MapPath("~/Reports/PMI/rdlc/RptProjectEstimation.rdlc");

            var data = pmiContext.sp_PMI_RptProjectEstimation(nameOfWorksId, strZoneId, numErrorCode, strErrorMsg).ToList();

            if (data.Count() > 0)
            {
                #region Search parameter

                string searchParameters = string.Empty;
                searchParameters = ddlNameOfWorks.SelectedItem.Text;
                ReportParameter p1 = new ReportParameter("param", searchParameters);

                rvProjectEstimation.LocalReport.SetParameters(new ReportParameter[] { p1});

                #endregion


                ReportDataSource dataSource = new ReportDataSource("dsProjectEstimation", data);
                rvProjectEstimation.LocalReport.DataSources.Add(dataSource);
                this.rvProjectEstimation.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            }
            else
            {
                checkStatus = true;
                rvProjectEstimation.Reset();
            }
            rvProjectEstimation.DataBind();

            //ExportToPDF
            String newFileName = "ProjectEstimationReport_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvProjectEstimation, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                switch (e.ReportPath)
                {
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
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region User Methods
        private void PopulateDropdownList()
        {
            var projectMaster = pmiContext.PMI_ProjectMaster.ToList();

            ddlNameOfWorks.DataSource = projectMaster.OrderBy(x => x.NameOfWorks).ToList();
            ddlNameOfWorks.DataValueField = "Id";
            ddlNameOfWorks.DataTextField = "NameOfWorks";
            ddlNameOfWorks.DataBind();
            ddlNameOfWorks.Items.Insert(0, new ListItem("[Select One]", "0"));

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

        }
        #endregion

        protected void rvProjectEstimation_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

    }
}