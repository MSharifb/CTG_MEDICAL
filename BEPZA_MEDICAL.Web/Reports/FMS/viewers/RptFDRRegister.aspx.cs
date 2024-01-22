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

namespace BEPZA_MEDICAL.Web.Reports.FMS.viewers
{
    public partial class RptFDRRegister :ReportBase
    {

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
                var FDRNo = ddlFDRNo.SelectedItem.Text;
                GenerateReport(FDRNo);
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion

        #region Generate Report

        public void GenerateReport(string FDRNo)
        {
            #region Processing Report Data

            rvFDRInfo.Reset();
            rvFDRInfo.ProcessingMode = ProcessingMode.Local;
            rvFDRInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/FDRRegister.rdlc");
            int fundTypeId =Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_FDRRegister(FDRNo, fundTypeId) select s).ToList();

            #region  parameter

            string fundType = fmsContext.FMS_FDRType
                               .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("FundType", fundType);
            ReportParameter p2 = new ReportParameter("ReportTitle", "FDR Register");
            rvFDRInfo.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRRegister", data);
            rvFDRInfo.LocalReport.DataSources.Add(dataSource);
            this.rvFDRInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRInfo.DataBind();

            //ExportToPDF
            String newFileName = "FDRInfo_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvFDRInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //dynamic data = null;
            //var dsName = "DSCompanyInfo";
            //data = (from c in base.context.vwCompanyInformations select c).ToList();
            //e.DataSources.Add(new ReportDataSource(dsName, data));
            dynamic data = null;
            var dsName = string.Empty;
            var fdrId = 0;

            if (e.ReportPath != "_ReportHeader")
            {
                fdrId = Convert.ToInt32(e.Parameters["FDRID"].Values[0].ToString());
            }

            switch (e.ReportPath)
            {
                case "_FDRInfoInstallmentSchedule":
                    data = (from InstSch in base.fmsContext.SP_FMS_FDRInfoInstallmentSchedule(fdrId) select InstSch).ToList();
                    dsName = "DSInstallmentSchedule";
                    break;
                case "_ReportHeader":
                    //data = (from c in base.context.vwCompanyInformations
                    //        select c).ToList();
                    //dsName = "DSCompanyInfo";
                     data = null;
                     dsName = "DSCompanyInfo";
                     data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                     e.DataSources.Add(new ReportDataSource(dsName, data));
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
            HashSet<int> clsFDRId = new HashSet<int>(fmsContext.FMS_FDRClosingInfo.Where(s=>s.IsRenew==false).Select(x => x.FixedDepositInfoId));
            int fundTypeId = Convert.ToInt32(Session["FDRTypeId"].ToString());
            ddlFDRNo.DataSource = fmsContext.FMS_FixedDepositInfo
                                 //.Where(x =>!clsFDRId.Contains(x.Id) && x.FDRTypeId == fundTypeId)
                                 .Where(x => x.FDRTypeId == fundTypeId)
                                 .ToList()
                                 .DistinctBy(x => x.FDRNumber);
            ddlFDRNo.DataValueField = "FDRNumber";
            ddlFDRNo.DataTextField = "FDRNumber";
            ddlFDRNo.DataBind();
            ddlFDRNo.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion

        protected void rvFDRInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}