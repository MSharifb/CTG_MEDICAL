<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptCategoryWiseAnsarList.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.AMS.viewers.RptCategoryWiseAnsarList" %>

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
                    <span class="label">Category <label style='color: red'>
                         *</label></span>
                    <span class="field">
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass = "required" ></asp:DropDownList>
                    </span>
                    <span class="label-right">Status <label style='color: red'>
                         *</label></span>
                    <span class="field">
                        <asp:DropDownList ID="ddlStatus" runat="server" ></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Zone <label style='color: red'>
                         *</label></span>
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
                                 OnClick="btnViewReport_Click" />
                        </div>
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>

            <div>
                <rsweb:ReportViewer ID="rvCategoryWiseAnsarList" runat="server" Width="100%" Height="100%" Visible="false"
                    OnReportRefresh="rvCategoryWiseAnsarList_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvCategoryWiseAnsarList" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
