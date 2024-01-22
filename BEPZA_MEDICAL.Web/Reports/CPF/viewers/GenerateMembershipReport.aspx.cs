using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BOM_MPA.Web.Utility;
using Microsoft.Reporting.WebForms;
using System.Data.Objects;

namespace BOM_MPA.Web.Reports.CPF.viewers
{
    public partial class GenerateMembershipReport : ReportBase
    {
        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CPFLoadAutocomplete", "CPFLoadAutocomplete();", true); 
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = string.Empty;
                string EmployeeInitial = txtEmpInitial.Text.Trim();
                txtEmpName.Text = GetEmployeeNameByInitial(EmployeeInitial);

                GenerateReport(EmployeeInitial);

               
                if (status == true)
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    rvGenerateMembership.Reset();
                }

                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
                rvGenerateMembership.Reset();
            }
        }

        private void GenerateReport(string EmployeeInitial)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = cpfContext.CPF_RptGenerateMembershipReport(EmployeeInitial, numErrorCode, strErrorMsg).ToList();
                        
            if (data.Count() > 0)
            {
                lblMsg.Text = string.Empty;
                rvGenerateMembership.Reset();
                rvGenerateMembership.ProcessingMode = ProcessingMode.Local;
                rvGenerateMembership.LocalReport.ReportPath = Server.MapPath("~/Reports/CPF/rdlc/GenerateMembershipReport.rdlc");

                rvGenerateMembership.LocalReport.DataSources.Add(new ReportDataSource("dsGenerateMembershipReport", data));           
                string searchParameters = "";

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvGenerateMembership.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }
            else
            {
                status = true;
                rvGenerateMembership.Reset();
            }
            rvGenerateMembership.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvGenerateMembership.DataBind();
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var dsName = string.Empty;
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            switch (e.ReportPath)
            {
                case "_ReportHeader":
                    dsName = "DSCompanyInfo";
                    var data = (from c in base.context.vwCompanyInformations
                                select c).ToList();
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                case "_MembershipForm":

                    string EmployeeInitial = txtEmpInitial.Text.Trim();
                    e.DataSources.Clear();
                    var dataAddition = cpfContext.CPF_RptGenerateMembershipReport(EmployeeInitial, numErrorCode, strErrorMsg).ToList();
                    dsName = "dsSubMembershipForm";
                    e.DataSources.Add(new ReportDataSource(dsName, dataAddition));
                    break;

                case "_SubNomeneeFormReport":

                    string EmployeeInitialNome = txtEmpInitial.Text.Trim();
                    e.DataSources.Clear();
                    var Nomineedata = cpfContext.CPF_RptGenerateMembershipReportSub_NomineeForm(EmployeeInitialNome, numErrorCode, strErrorMsg).ToList();
                    dsName = "dsSubNomeneeFormReport";
                    e.DataSources.Add(new ReportDataSource(dsName, Nomineedata));
                    break;
            }
        }

        protected void rvGenerateMembership_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
    }
}