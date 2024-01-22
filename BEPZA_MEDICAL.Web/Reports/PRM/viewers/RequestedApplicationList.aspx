<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RequestedApplicationList.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RequestedApplicationList" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <rsweb:reportviewer id="rvApplicationList" runat="server" width="100%" height="100%"
                onreportrefresh="rvApplicationList_ReportRefresh">
            </rsweb:reportviewer>
            <div class="clear">
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvApplicationList" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
