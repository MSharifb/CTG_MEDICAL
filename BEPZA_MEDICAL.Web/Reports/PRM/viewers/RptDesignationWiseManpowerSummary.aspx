<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptDesignationWiseManpowerSummary.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptDesignationWiseManpowerSummary" %>


<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <%-- <link type="text/css" href="../../../assets/css/bootstrap.min.css" rel="stylesheet" />
    <link type="text/css" href="../../../assets/css/bootstrap-multiselect.css" rel="stylesheet" />
    <script type="text/javascript" src="../../../assets/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="../../../assets/js/bootstrap-multiselect.js"></script>

    <script type="text/javascript">
        $(function () {
            $('[id*=ddlZone]').multiselect({
                includeSelectAllOption: true
            });
        });
    </script>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 30px" id="message">
                <asp:Label ID="lblMsg" runat="server" ClientIDMode="Static">
                </asp:Label>
            </div>
            <div class="clearfix"></div>
            <div class="GroupBox">
                <div class="form-horizontal">
                    <%--      <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    From Date&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtFromDate" runat="server" ClientIDMode="Static" CssClass="datePicker date"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    To Date&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtToDate" runat="server" ClientIDMode="Static" CssClass="datePicker date"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>--%>
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
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Designation
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlDesignation">
                                    </asp:DropDownList>
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
                <rsweb:ReportViewer ID="rvDesignationWiseManpower" runat="server" Width="100%" Visible="true"
                    Height="100%" OnReportRefresh="rvDesignationWiseManpower_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvDesignationWiseManpower" />
        </Triggers>
    </asp:UpdatePanel>

    <%--    <script type="text/javascript">
        $('#<%=txtToDate.ClientID %>').live('change', function () {
            //debugger;
            $("#message").empty();
            var frmDate = $('#<%=txtFromDate.ClientID %>').val();
            var tDate = $('#<%=txtToDate.ClientID %>').val();
            var fromDate = new Date(frmDate);
            var toDate = new Date();

            if ($('#<%=txtFromDate.ClientID %>').val() == null || $('#<%=txtFromDate.ClientID %>').val() == "") {
                $('#<%=txtToDate.ClientID %>').val('');
                $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> Please select 'To Date' before 'From Date' </span>  </div> ");
                return;
            }

            if (fromDate > toDate) {
                // Do something
                $("#message").html("<div class=\"validation-summary-errors\" data-valmsg-summary=\"true\"> <span> 'Effective To Date' is greater than or equal to 'Effective From Date'  </span>  </div> ");
                $('#<%=txtToDate.ClientID %>').val('');
            }
        })
    </script>--%>
</asp:Content>

