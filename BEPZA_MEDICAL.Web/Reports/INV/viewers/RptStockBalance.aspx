<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptStockBalance.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PMI.viewers.RptStockBalance" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server" Style="text-align: center">        
                </asp:Label>
            </div>
            <div class="GroupBox" style="height: 319px">
                <div class="row">
                    <span class="label">Date From
                        <label class="required-field">*</label></span>
                    <span class="field">
                        <asp:TextBox ID="fromDate" runat="server" CssClass="datePicker required" Width="100px"></asp:TextBox>
                    </span>
                    <span class="label-right">To
                        <label class="required-field">*</label></span>
                    <span class="field">
                        <asp:TextBox ID="toDate" runat="server" CssClass="datePicker required" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>
                    <span class="label">&nbsp;</span>
                    <span class="label-right">
                        <asp:CheckBox ID="chkZeroItem" runat="server" />&nbsp; Show Item(s) with zero balance
                    </span>
                </div>
                <hr />
                <legend style="font-size: 18px; color: black; font-weight: bold">Item Info</legend>
                <div class="row">
                     <span class="label">
                        <asp:CheckBox ID="chkPeriodic" runat="server" />&nbsp; Perodic
                    </span>
                    <span class="label">
                        <asp:CheckBox ID="chkCondemnable" runat="server" />&nbsp; Condemnable
                    </span>
                    <span class="label">
                        <asp:CheckBox ID="chkHasQuota" runat="server" />&nbsp; Has Quota
                    </span>
                </div>
                <div class="row">
                    <span class="label">Item</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItem" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Category</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemCategory" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Type</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemType" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Color</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemColor" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Model</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemModel" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Fixed Asset/Consumable</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemClass" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <%--</fieldset>--%>
            </div>
            <div class="clear clearfix">
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
            </div>

            <div>
                <rsweb:ReportViewer ID="rvStockBalance" runat="server" Width="100%" Height="100%" Visible="false"
                    OnReportRefresh="rvStockBalance_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvStockBalance" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
