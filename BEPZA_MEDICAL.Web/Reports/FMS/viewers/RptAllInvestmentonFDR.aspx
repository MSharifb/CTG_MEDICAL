<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptAllInvestmentonFDR.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.FMS.viewers.RptAllInvestmentonFDR" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="form-horizontal">
                  <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Date&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtFDRDateTo" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                               <label class="col-sm-4 control-label">
                                </label>
                                <div class="col-sm-8">
                                        <asp:RadioButton ID="RadioButton1"  GroupName="FDR" runat="server" Text="FDR Date" Checked="True" />
                                        <asp:RadioButton ID="RadioButton2" GroupName="FDR" runat="server" Text="Maturity Date"/>
                                    </div>
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
                <div id="rptContainer" style="width: 100%; height: 100%; display: block; text-align: center;">
                    <rsweb:ReportViewer ID="rvFDRSchedule" runat="server" Width="100%" ShowPrintButton="true" SkinID="" AsyncRendering="true" Visible="true"
                        Height="100%" InteractiveDeviceInfos="(Collection)"
                        OnReportRefresh="rvFDRSchedule_ReportRefresh" SizeToReportContent="true">
                    </rsweb:ReportViewer>
                </div>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvFDRSchedule" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
