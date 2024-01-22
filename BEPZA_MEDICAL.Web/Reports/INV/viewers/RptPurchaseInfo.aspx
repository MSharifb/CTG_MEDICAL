<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptPurchaseInfo.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.INV.viewers.RptPurchaseInfo" %>

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
                    <span class="label">Purchase Date From <label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:TextBox ID="dpPurchaseDateFrom" runat="server" CssClass="datePicker required" Width="100px" ></asp:TextBox>
                    </span>
                    <span class="label-right">To <label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:TextBox ID="dpPurchaseDateTo" runat="server" CssClass="datePicker required" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">P.O. Date From </span>
                    <span class="field">
                        <asp:TextBox ID="dpPODateFrom" runat="server" CssClass="datePicker" Width="100px" ></asp:TextBox>
                    </span>
                    <span class="label-right">To</span>
                    <span class="field">
                        <asp:TextBox ID="dpPODateTo" runat="server" CssClass="datePicker" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Challan Date From </span>
                    <span class="field">
                        <asp:TextBox ID="dpChallanDateFrom" runat="server" CssClass="datePicker" Width="100px" ></asp:TextBox>
                    </span>
                    <span class="label-right">To</span>
                    <span class="field">
                        <asp:TextBox ID="dpChallanDateTo" runat="server" CssClass="datePicker" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Purchase Type</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlPurchaseType" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Supplier</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlSupplier" runat="server"></asp:DropDownList>
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
                    <%--<span class="label-right">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>--%>
                </div>
                <div class="row">
                    <span class="label">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
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
                <rsweb:ReportViewer ID="rvPurchaseInfo" runat="server" Width="100%" Height="100%" Visible="false"
                    OnReportRefresh="rvPurchaseInfo_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvPurchaseInfo" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
