<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptWorkWiseBudget.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PMI.viewers.RptWorkWiseBudget" %>


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
                    <span class="label">Zone<label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:DropDownList ID="ddlBudgetZoneInformation" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Financial Year<label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlFinancialYear" ClientIDMode="Static"></asp:ListBox>
                        <%--<asp:DropDownList Multiple="Multiple" runat="server" ID="ddlFinancialYear" ClientIDMode="Static" CssClass="select-single"></asp:DropDownList>
                        <asp:HiddenField ID="hdnFinancialYear" runat="server" />--%>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Budget Head<label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlBudgetHead" OnSelectedIndexChanged="ddlBudgetHead_SelectedIndexChanged" AutoPostBack="true" ClientIDMode="Static"></asp:ListBox>
                        <%--<asp:DropDownList Multiple="Multiple" runat="server" ID="ddlBudgetHead" ClientIDMode="Static" CssClass="select-single" onchange="PopulateSubHeads();"></asp:DropDownList>
                        <asp:HiddenField ID="hdnBudgetHead" runat="server" />--%>
                    </span>
                </div>

                <div class="row">
                    <span class="label">Budget Sub Head<label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlBudgetSubHead" ClientIDMode="Static"></asp:ListBox>
                        <%--<asp:DropDownList Multiple="Multiple" runat="server" ID="ddlBudgetSubHead" ClientIDMode="Static" CssClass="select-single"></asp:DropDownList>
                        <asp:HiddenField ID="hdnBudgetSubHead" runat="server" />--%>
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
                <rsweb:ReportViewer ID="rvWorkWiseBudget" runat="server" Width="100%" Height="100%" AsyncRendering="true" Visible="true" SizeToReportContent="True"
                    OnReportRefresh="rvWorkWiseBudget_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvWorkWiseBudget" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        $(function () {
            $('[id*=ddlBudgetHead], [id*=ddlBudgetSubHead], [id*=ddlFinancialYear]').multiselect({
                includeSelectAllOption: true
            });
        });

    </script>

</asp:Content>
