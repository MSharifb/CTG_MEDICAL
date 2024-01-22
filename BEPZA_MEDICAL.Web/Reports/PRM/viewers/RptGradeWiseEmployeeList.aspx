<%@ Page Title="" Language="C#" EnableEventValidation="false" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptGradeWiseEmployeeList.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptGradeWiseEmployeeList" %>

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
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Zone
                                </label>
                                <div class="col-sm-8">
                                    <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Salary Scale&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlSalaryScale" AppendDataBoundItems="true" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group">
                            <div class="col-sm-6">
                                <label class="col-sm-4 control-label">
                                    Grade From
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlGrade" AppendDataBoundItems="true" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <label class="col-sm-4 control-label">
                                    Grade To
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlGradeTo" AppendDataBoundItems="true" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group">                 
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">
                                        As on Date&nbsp;<span class="required-field">*</span>
                                    </label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txtGenerationDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div
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
                <div id="rpt-container" style="width: 100%; height: 100%; display: block; text-align: center;">
                    <rsweb:ReportViewer ID="rvEmployeeInfo" runat="server" Width="100%" ShowPrintButton="true" SkinID="" AsyncRendering="true" Visible="true"
                        Height="100%" InteractiveDeviceInfos="(Collection)"
                        OnReportRefresh="rvEmployeeInfo_ReportRefresh">
                    </rsweb:ReportViewer>
                </div>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvEmployeeInfo" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        //cascading dropdown
        $('#<%=ddlSalaryScale.ClientID %>').change(function () {
            var id = parseInt($(this).val());
            if (id > 0) {
                $.ajax({
                    type: "POST",
                    url: "RptGradeWiseEmployeeList.aspx/GetGradeListBySalaryScaleId",
                    data: "{Id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var jobGradeDDL = $('#<%=ddlGrade.ClientID %>');
                        var jobGradeDDLTo = $('#<%=ddlGradeTo.ClientID %>');

                        jobGradeDDL.empty();
                        jobGradeDDLTo.empty();

                        jobGradeDDL.append($('<option/>', {
                            value: '0',
                            text: 'Select'
                        }));
                        jobGradeDDLTo.append($('<option/>', {
                            value: '0',
                            text: 'Select'
                        }));

                        $.each(response.d, function (key, value) {
                            jobGradeDDL.append($("<option></option>").val(value.Id).html(value.Name));
                        });
                        $.each(response.d, function (key, value) {
                            jobGradeDDLTo.append($("<option></option>").val(value.Id).html(value.Name));
                        });
                    }
                });
                }
        });


    </script>
</asp:Content>


