<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master"
    AutoEventWireup="true" CodeBehind="RptSimulateFDRRegister.aspx.cs"
    Inherits="BEPZA_MEDICAL.Web.Reports.FMS.viewers.RptSimulateFDRRegister" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server">
                </asp:Label>
            </div>

            <div id="rptContainer" style="width: 98%; height: 100%; display: block; text-align: center; margin-left:10px;">

                <rsweb:ReportViewer ID="rvFDRInfo" runat="server" Width="100%"
                    ShowPrintButton="true" SkinID="" AsyncRendering="true" Visible="true"
                    Height="100%" InteractiveDeviceInfos="(Collection)" SizeToReportContent="true">
                </rsweb:ReportViewer>

            </div>
            <div class="clear">
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvFDRInfo" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
