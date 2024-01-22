<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptPrintProcurementPlan.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PMI.viewers.RptPrintProcurementPlan" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GroupBox">
                <rsweb:ReportViewer ID="rvProcurementPlan" runat="server" Width="100%" Height="100%" AsyncRendering="true" SizeToReportContent="True"
                    OnReportRefresh="rvProcurementPlan_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvProcurementPlan" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
