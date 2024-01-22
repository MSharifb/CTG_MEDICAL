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
    public partial class MembershipPreviewForm : ReportBase
    {
        bool status = false;
        string initial = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string membershipId = Request.QueryString["id"]; 
                int id=Convert.ToInt32(membershipId);
                if (id > 0)
                {
                    int empId = cpfContext.CPF_MembershipInfo.Where(x => x.Id == id).FirstOrDefault().EmployeeId;
                    initial = context.PRM_EmploymentInfo.Where(y => y.Id == empId).FirstOrDefault().EmployeeInitial;
                    GenerateReport(initial);

                    lblMsg.Text = string.Empty;
                    if (status == true)
                    {
                        lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    }
                    status = false;
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
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
                rvMembershipPreviewForm.Reset();
                rvMembershipPreviewForm.ProcessingMode = ProcessingMode.Local;
                rvMembershipPreviewForm.LocalReport.ReportPath = Server.MapPath("~/Reports/CPF/rdlc/MembershipPreviewForm.rdlc");

                rvMembershipPreviewForm.LocalReport.DataSources.Add(new ReportDataSource("dsGenerateMembershipReport", data));
                //string searchParameters = "";

                //ReportParameter p1 = new ReportParameter("param", searchParameters);
                //rvMembershipPreviewForm.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }
            else
            {
                status = true;
                rvMembershipPreviewForm.Reset();
            }
            rvMembershipPreviewForm.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvMembershipPreviewForm.DataBind();

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

                    //string EmployeeInitial = txtEmployeeInitial.Text.Trim();
                    e.DataSources.Clear();
                    var dataAddition = cpfContext.CPF_RptGenerateMembershipReport(initial, numErrorCode, strErrorMsg).ToList();
                    dsName = "dsSubMembershipForm";
                    e.DataSources.Add(new ReportDataSource(dsName, dataAddition));
                    break;

                case "_SubNomeneeFormReport":

                    //string EmployeeInitialNome = txtEmployeeInitial.Text.Trim();
                    e.DataSources.Clear();
                    var Nomineedata = cpfContext.CPF_RptGenerateMembershipReportSub_NomineeForm(initial, numErrorCode, strErrorMsg).ToList();
                    dsName = "dsSubNomeneeFormReport";
                    e.DataSources.Add(new ReportDataSource(dsName, Nomineedata));
                    break;
            }
        }

        protected void rvMembershipPreviewForm_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Page_Load(null, null);
        }

        protected void btnBack_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/CPF/MembershipApplication/Create");
        }
    }
}