<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptItemLedger.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.INV.viewers.RptItemLedger" %>

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
                    <span class="label">Date From <label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:TextBox ID="dpDateFrom" runat="server" CssClass="datePicker required" Width="100px" ></asp:TextBox>
                    </span>
                    <span class="label-right">To <label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:TextBox ID="dpDateTo" runat="server" CssClass="datePicker required" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Item Type</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemType" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Category</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlCategory" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Color</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlColor" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Model</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlModel" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Item</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItem" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Asset/Goods</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlAsset" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>
                    <span class="label-right">&nbsp;</span>
                    <span class="field">
                        &nbsp;
                    </span>
                </div>
            </div>

            <div class="GroupBox">
                <div class="form-group">
                    <div class="text-center" style="text-align: center">
                        <div class="">
                            <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                type="submit" OnClick="btnViewReport_Click" />
                        </div>
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>

            <div>
                <rsweb:ReportViewer ID="rvItemLedger" runat="server" Width="100%" Height="100%" Visible="false"
                    OnReportRefresh="rvItemLedger_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvItemLedger" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
