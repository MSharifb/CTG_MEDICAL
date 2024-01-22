<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptProgressReport.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PMI.viewers.RptProgressReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server" Style="text-align: center">        
                </asp:Label>
            </div>

            <div class="GroupBox">
                <div class="form-horizontal">
                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    From Date&nbsp;<span class="required-field">*</span>
                                </label>
                                </label>
                    <div class="col-sm-8">
                        <asp:TextBox ID="dtFromDate" runat="server" CssClass="datePicker" Width="100px"></asp:TextBox>
                    </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    To Date&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="dtToDate" runat="server" CssClass="datePicker" Width="100px"></asp:TextBox>
                                </div>
                                <span class="field" style="display: none">
                                    <asp:TextBox ID="tbProjectforId" runat="server">                   
                                    </asp:TextBox>
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="row" style="display: none">
                        <span class="label">Zone</span>
                        <span class="field">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>

                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Report Date&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList ID="ddlProgressReport" runat="server" CssClass="form-control required"></asp:DropDownList>
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
                <rsweb:ReportViewer ID="rvFinancialBudget" runat="server" Width="100%" Height="100%" AsyncRendering="true" Visible="true" SizeToReportContent="True"
                    OnReportRefresh="rvFinancialBudget_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvFinancialBudget" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">

        $('#<%=dtFromDate.ClientID %>').change(function () {
            var pfromDate = $(this).val();
            var ptoDate = $('#<%=dtToDate.ClientID %>').val();
            var pprojectForId = $('#<%=tbProjectforId.ClientID %>').val();
            if (pfromDate != '' && ptoDate != '') {
                callProgressReport(pprojectForId, pfromDate, ptoDate);
            }
        })
        $('#<%=dtToDate.ClientID %>').change(function () {
            var pfromDate = $('#<%=dtFromDate.ClientID %>').val();
            var ptoDate = $('#<%=dtToDate.ClientID %>').val();
            var pprojectForId = $('#<%=tbProjectforId.ClientID %>').val();
            if (pfromDate != '' && ptoDate != '') {
                callProgressReport(pprojectForId, pfromDate, ptoDate);
            }
        })

        function callProgressReport(pprojectForId, pfromDate, ptoDate) {
            $.ajax({
                type: "POST",
                url: "RptProgressReport.aspx/FetchProgressReport",
                data: "{projectForId: " + pprojectForId + ", fromDate: '" + pfromDate + "', toDate: '" + ptoDate + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    var designationDDL = $('#<%=ddlProgressReport.ClientID %>');
                    designationDDL.empty();
                    designationDDL.append($('<option/>', {
                        value: '',
                        text: '[Select One]'
                    }));
                    $.each(response.d, function (key, value) {
                        designationDDL.append($("<option></option>").val(value.Id).html(value.Name));
                    });
                }
            });
            }

            function dateFormat(jsonDate) {
                var shortDate = null;
                if (jsonDate) {
                    var regex = /-?\d+/;
                    var matches = regex.exec(jsonDate);
                    var dt = new Date(parseInt(matches[0]));
                    var month = dt.getMonth() + 1;
                    var monthString = month > 9 ? month : '0' + month;
                    var day = dt.getDate();
                    var dayString = day > 9 ? day : '0' + day;
                    var year = dt.getFullYear();
                    shortDate = monthString + '/' + dayString + '/' + year;
                }
                return shortDate;
            }
    </script>
</asp:Content>
