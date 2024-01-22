<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="PresentStatusofManpowerAndVehicle.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.PresentStatusofManpowerAndVehicle" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox">
                <rsweb:ReportViewer ID="rvEmployeeInfo" runat="server" Width="100%" Height="100%"
                    OnReportRefresh="rvEmployeeInfo_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
