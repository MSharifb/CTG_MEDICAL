<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptAwardedOrPunishedSecurityPersonnelList.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.AMS.viewers.RptAwardedOrPunishedSecurityPersonnelList" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server" Style="text-align: center">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <span class="label">Zone <label style='color: red'>
                         *</label></span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>
                </div>
                <%--<div class="row">
                    <span class="label">Status <label style='color: red'>
                         *</label></span>
                    <span class="field">
                        <asp:DropDownList ID="ddlStatus" runat="server" ></asp:DropDownList>
                    </span>
                </div>--%>
            </div>
            
            <div class="GroupBox">
                <div class="form-group">
                    <div class="text-center" style="text-align: center">
                        <div class="">
                            <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                 OnClick="btnViewReport_Click" />
                        </div>
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>

            <div>
                <rsweb:ReportViewer ID="rvAwardedOrPunishedSecurityPersonnelList" runat="server" Width="100%" Height="100%" Visible="false"
                    OnReportRefresh="rvAwardedOrPunishedSecurityPersonnelList_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvAwardedOrPunishedSecurityPersonnelList" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
