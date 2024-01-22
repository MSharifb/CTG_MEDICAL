<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptTerminatedEmployeeList.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptTerminatedEmployeeList" %>



<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <span class="label">
                        <asp:Label ID="Label5" Text="Termination from Date " runat="server" />
                        <label class="required-field">
                            *</label>
                    </span><span class="field">
                        <asp:TextBox ID="txtSeperationDateFrom" runat="server" Width="100px" CssClass="datePicker date"></asp:TextBox>
                    </span><span class="label-right">
                        <asp:Label ID="Label6" Text="Termination to Date " runat="server" />
                        <label class="required-field">
                            *</label>
                    </span><span class="field">
                        <asp:TextBox ID="txtSeperationDateTo" runat="server" Width="100px" CssClass="datePicker date"></asp:TextBox>
                    </span>
                </div>
                <div class="button-crude">
                    <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                        OnClientClick="return fnValidate();" OnClick="btnViewReport_Click" />
                </div>
                <div class="clear">
                </div>
            </div>
            <div>
                <rsweb:ReportViewer ID="rvTerminatedEmployeeList" runat="server" Width="100%" Visible="true"
                    Height="100%" OnReportRefresh="rvTerminatedEmployeeList_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvTerminatedEmployeeList" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

