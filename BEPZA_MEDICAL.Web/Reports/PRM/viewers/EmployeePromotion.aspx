<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.master"
    CodeBehind="EmployeePromotion.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.EmployeePromotion" %>

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
                <div class="form-horizontal">
                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Promotion Date(From)&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="dtPromotionFromDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Promotion Date(To)&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="dtPromotionToDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
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
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlDivision">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Salary Scale Name
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlSalaryScaleName" Enabled="False">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Job Title(From)
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlJobTitleFrom">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    <asp:Label ID="Label2" Text="Job Title(To)" runat="server" />
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlJobTitleTo">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Job Grade(From)
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlJobGradeFrom">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Job Grade(To)
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlJobGradeTo">
                                    </asp:DropDownList>
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
                </div>
            </div>

            <div class="GroupBox">
                <div class="text-center" style="text-align: center">
                    <div class="">
                        <asp:Button Text="View Report" CssClass="btn btn-primary" runat="server" ID="btnViewReport" ValidationGroup="view"
                            OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>

            <div>
                <rsweb:ReportViewer ID="rvEmployeePromotion" runat="server" Width="100%" Visible="true"
                    Height="100%" OnReportRefresh="rvEmployeePromotion_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvEmployeePromotion" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
