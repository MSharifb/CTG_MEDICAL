<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptEmployeeClassWiseManpowerInfo.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptEmployeeClassWiseManpowerInfo" %>


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
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Class Name
                            </label>
                            <div class="col-sm-8">
                                <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployeeClass">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                As on Date
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtGenerationDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="GroupBox">
                <div class="text-center" style="text-align: center">
                    <div class="">
                        <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                            OnClick="btnViewReport_Click" />
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>
            <div>
                <rsweb:ReportViewer ID="rvOrgWiseManpower" runat="server" Width="100%" Visible="true"
                    Height="100%" OnReportRefresh="rvOrgWiseManpower_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvOrgWiseManpower" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
