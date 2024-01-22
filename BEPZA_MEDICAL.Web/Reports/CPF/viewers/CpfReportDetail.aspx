<%@ Page Title="" Language="C#" 
    MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" 
    AutoEventWireup="true" CodeBehind="CpfReportDetail.aspx.cs" 
    Inherits="BEPZA_MEDICAL.Web.Reports.CPF.viewers.CpfReportDetail" %>

<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms"
    Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="width: 98%; margin: 10px auto; min-height: 15px" id="message">
                <div class="row">

                    <span class="field">
                        <div class="button-crude button-left">
                            <asp:Button Text="Close Window" runat="server" ID="btnClose" OnClientClick="window.close(); return false;" />
                        </div>
                    </span>
                    <span class="label-right">
                        <asp:Label ID="lblMsg" runat="server" ForeColor="Red">        
                        </asp:Label>
                    </span>
                </div>
            </div>
            <div class="GroupBox" style="width: 98%; margin: 10px auto;">
                <rsweb:ReportViewer ID="rvCpfRpt" ShowZoomControl="True" ShowPrintButton="True" 
                    ZoomMode="PageWidth" SizeToReportContent="False"
                    OnReportRefresh="rvCpfRpt_ReportRefresh" 
                    runat="server" Height="100%" Width="100%" >
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvMonthlySalaryRpt" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
