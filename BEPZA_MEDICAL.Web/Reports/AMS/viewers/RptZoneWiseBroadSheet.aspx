<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptZoneWiseBroadSheet.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.AMS.viewers.RptZoneWiseBroadSheet" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server" Style="text-align: center">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <span class="label">Zone
                        <label style='color: red'>
                            *</label></span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>

                   <span class="label-right">Employee
                     </span>
                    <span class="field">
                        <asp:DropDownList ID="ddlEmployee" CssClass="form-control select-single" runat="server" ></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Designation
                    </span>
                    <span class="field">
                        <asp:DropDownList ID="ddlDesignation" CssClass="form-control select-single" runat="server" ></asp:DropDownList>
                    </span>
                    <span class="label-right">District </span>
                    <span class="field">
                        <asp:DropDownList ID="ddlDistrict" CssClass="form-control select-single" runat="server" ></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Punishment
                                                </span>
                    <span class="field">
                        <asp:DropDownList ID="ddlPunishment" CssClass="form-control select-single" runat="server" ></asp:DropDownList>
                    </span>
                    <span class="label-right">Reward </span>
                    <span class="field">
                        <asp:DropDownList ID="ddlReward" CssClass="form-control select-single" runat="server" ></asp:DropDownList>
                    </span>
                </div>

                <div class="row">
                    <span class="label">Year of service from
                        </span>
                    <span class="field">
                        <asp:TextBox ID="txtFrom" runat="server" TextMode="Number"></asp:TextBox>
                    </span>
                    <span class="label-right">to </span>
                    <span class="field">
                         <asp:TextBox ID="txtTo" runat="server" TextMode="Number"></asp:TextBox>                       
                    </span>
                </div>
                <div class="row">
                    <span class="label">Status
                        <label style='color: red'>
                            *</label></span>
                    <span class="field">
                        <asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList>
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
                <rsweb:ReportViewer ID="rvZoneWiseSecurityPersonnelList" runat="server" Width="100%" Height="100%" Visible="true"
                    OnReportRefresh="rvZoneWiseSecurityPersonnelList_ReportRefresh" HyperlinkTarget="_blank" SizeToReportContent="true" EnableHyperlinks = true >
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvZoneWiseSecurityPersonnelList" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
