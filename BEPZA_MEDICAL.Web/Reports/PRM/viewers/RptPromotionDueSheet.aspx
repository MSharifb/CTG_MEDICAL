<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptPromotionDueSheet.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptPromotionDueSheet" %>

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
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Department
                            </label>
                            <div class="col-sm-8">
                                <asp:DropDownList CssClass="form-control select-single" ID="ddlDepartment" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Designation
                            </label>
                            <div class="col-sm-8">
                                <asp:DropDownList CssClass="form-control select-single" ID="ddlDesignation" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Zone
                            </label>
                            <div class="col-sm-8">
                                <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <span class="label">&nbsp;</span>
                    <span class="field">
                        <div class="button-crude button-left">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" OnClientClick="return fnValidate();"
                                OnClick="btnViewReport_Click" />
                        </div>
                    </span>
                </div>
                <div class="clear">
                </div>

                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvTestResult" runat="server" Width="100%" Height="100%"
                        OnReportRefresh="rvTestResult_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvTestResult" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
