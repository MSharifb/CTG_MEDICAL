﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptSuspensionEmployee.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptSuspensionEmployee__" %>

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
                                Suspension From Date:&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtFromDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Suspension To Date:&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtToDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
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
    <script type="text/javascript">
        //$(window).load(function () {
        //    $(".loader").fadeOut("slow");
        //})
        $(document).ready(function () {

            $('#<%=txtFromDate.ClientID %>').live('change', function () {
                var from = $('#<%=txtFromDate.ClientID %>').val();
                var to = $('#<%=txtToDate.ClientID %>').val();

                if (to != "") {
                    getDate(toDate(from), toDate(to));
                }
            })

            $('#<%=txtToDate.ClientID %>').live('change', function () {
                var from = $('#<%=txtFromDate.ClientID %>').val();
                var to = $('#<%=txtToDate.ClientID %>').val();
                from=toDate(from);
                to=toDate(to)
                getDate(from, to);
            })

            function toDate(selector) {
                var from = selector.split("-");
                return new Date(from[2], from[1] - 1, from[0]);
            }

            function getDate(from, to) {
                $('#message').empty();
                if (from > to) {
                    $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span>'To Date' must be greater than or equal to 'From Date'.</span> </div> ");
                    $('#<%=txtToDate.ClientID %>').val('');
                    return;
                }
            }
        });

    </script>
</asp:Content>
