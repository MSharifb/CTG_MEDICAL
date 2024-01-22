<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptEmployeeTrainingInfo.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptEmployeeTrainingInfo" %>

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
                                Training Type
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlTrainingType" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Training Title
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlTrainingTitle" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">                 
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Employee
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Organized By
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlOrganizedBy" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">                 
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Funded By
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlFundedBy" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Country
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlCountry" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">                 
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Date From
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtFromDate" CssClass="datePicker" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                To
                            </label>
                            <div class="col-sm-8">
                             <asp:TextBox ID="txtToDate" CssClass="datePicker" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Department
                            </label>
                            <div class="col-sm-8">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlDepartment" ClientIDMode="Static">
                            </asp:DropDownList>
                            </div>
                        </div>
                    </div>
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
                <rsweb:ReportViewer ID="rvTestResult" runat="server" Width="100%" Height="100%" Visible="true"
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
