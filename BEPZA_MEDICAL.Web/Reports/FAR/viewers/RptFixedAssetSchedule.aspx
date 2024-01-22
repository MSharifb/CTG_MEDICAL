<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptFixedAssetSchedule.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.FAR.viewers.RptFixedAssetSchedule" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .inline-rb {
        }

            .inline-rb input {
                float: left;
            }

            .inline-rb label {
                text-align: right;
                padding: 2px 3px;
            }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 30px" id="message">
                <asp:Label ID="lblMsg" runat="server">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Zone</label>&nbsp;<span class="required-field">*</span><br />
                            <asp:ListBox CssClass="form-control" SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Category</label>
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlCategory">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Item Type </label>
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlItemType">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="row">
                      <div class="col-sm-4">
                        <div class="form-group">
                            <label>Financial Year</label>&nbsp;<span class="required-field">*</span><br />
                            <asp:DropDownList ID="ddlFinancialYear" runat="server" ClientIDMode="Static" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Refurbishment Type</label>&nbsp;<span class="required-field">*</span>
                            <asp:RadioButtonList ID="rdbRefurbishment" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" CssClass="inline-rb">
                                <asp:ListItem Text="Both" Value="B" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Without" Value="WO"></asp:ListItem>
                                <asp:ListItem Text="Refurbishment" Value="R"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Schedule Type</label>&nbsp;<span class="required-field">*</span>
                            <asp:RadioButtonList ID="rdRptType" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" CssClass="inline-rb">
                                <asp:ListItem Text="Itemwise" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Groupwise" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Zonewise" Value="2"></asp:ListItem>
                                <%--  <asp:ListItem Text="Categorywise" Value="3"></asp:ListItem>--%>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                  
                </div>
                <%--<div class="row">                  
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>Date Period From</label>&nbsp;<span class="required-field">*</span><br />
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>

                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="form-group">
                            <label>To</label>&nbsp;<span class="required-field">*</span><br />
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                        </div>
                    </div>
                </div>--%>
            </div>
            <div class="GroupBox" id="rpt-container">
                <div class="text-center" style="text-align: center">
                    <div class="">
                        <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                            OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>

            <div class="clearfix">
                <rsweb:ReportViewer ID="rvFixedAssetSchedule" runat="server" ShowExportControls="true" ShowPrintButton="true" Visible="false"
                    Width="100%" Height="100%" OnReportRefresh="rvFixedAssetSchedule_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvFixedAssetSchedule" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        // container is either the ReportViewer control itself, or a div containing it. 

        function fixReportingServices(container) {
            if ($.browser.safari) { // toolbars appeared on separate lines. 
                $('#' + container + ' table').each(function (i, item) {
                    if ($(item).attr('id') && $(item).attr('id').match(/fixedTable$/) != null)
                        $(item).css('display', 'table');
                    else
                        $(item).css('display', 'inline-block');
                });
            }
        }

        // needed when AsyncEnabled=true. 

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () { fixReportingServices('rpt-container'); });

        /*$(document).ready(function () { fixReportingServices('rpt-container');});*/

    </script>
</asp:Content>
