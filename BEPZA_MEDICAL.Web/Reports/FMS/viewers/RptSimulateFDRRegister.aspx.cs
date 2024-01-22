using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.FMS.viewers
{
    public partial class RptSimulateFDRRegister : ReportBase
    {
        #region Page Event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                var FDRId = Convert.ToInt32(Request.QueryString["Id"]);
                var interestRate = Convert.ToDecimal(Request.QueryString["interestRate"]);
                var interestRateDuration = Convert.ToDecimal(Request.QueryString["interestRateDuration"]);
                var taxRate = Convert.ToDecimal(Request.QueryString["taxRate"]);
                var totalBankCharge = Convert.ToDecimal(Request.QueryString["totalBankCharge"]);
                var isCommitChange = Convert.ToInt32(Request.QueryString["isCommitChange"]);

                GenerateReport(FDRId, interestRate, interestRateDuration, taxRate, totalBankCharge, isCommitChange);
            }
        }
        #endregion

        #region Generate Report

        public void GenerateReport(int FDRId, decimal interestRate, decimal interestRateDuration, decimal taxRate, decimal totalBankCharge, int isCommitChange)
        {
            #region Processing Report Data

            rvFDRInfo.Reset();
            rvFDRInfo.ProcessingMode = ProcessingMode.Local;
            rvFDRInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/FMS/rdlc/FDRRegister.rdlc");
            int fundTypeId = Convert.ToInt32((Session["FDRTypeId"]));

            var data = (from s in base.fmsContext.SP_FMS_SimulateFDRRegister(FDRId, interestRate, interestRateDuration, taxRate, totalBankCharge, isCommitChange) select s).ToList();

            #region  parameter

            string fundType = fmsContext.FMS_FDRType
                               .Where(x => x.Id == fundTypeId).FirstOrDefault().Name;

            ReportParameter p1 = new ReportParameter("FundType", fundType);
            ReportParameter p2 = new ReportParameter("ReportTitle", "FDR Register (Simulated)");
            rvFDRInfo.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            #endregion

            ReportDataSource dataSource = new ReportDataSource("DSFDRRegister", data);
            rvFDRInfo.LocalReport.DataSources.Add(dataSource);
            this.rvFDRInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvFDRInfo.DataBind();

            //ExportToPDF
            //String newFileName = "FDRInfo_" + Guid.NewGuid() + ".pdf";
            //String newFilePath = "~/Content/TempFiles/" + newFileName;
            //FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            //Common.ExportPDF(rvFDRInfo, newFileName, fs);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
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
    }
}