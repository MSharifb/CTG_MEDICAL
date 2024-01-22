<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master"
    AutoEventWireup="true" CodeBehind="CpfReportMaster.aspx.cs"
    Inherits="BEPZA_MEDICAL.Web.Reports.CPF.viewers.CpfReportMaster" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <fieldset>
                <div class="GroupBox" id="message">
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red">        
                    </asp:Label>
                </div>

                <div class="GroupBox">

                    <%--  <span class="label">
                            <asp:Label ID="lbl1" Text="Report " runat="server" /><label class="required-field"> *</label>
                        </span>--%>
                    <%--<span class="field">
                            <asp:DropDownList runat="server" AutoPostBack="true" ID="ddlReport" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged">
                            </asp:DropDownList>
                        </span>--%>
                    <div id="cbxIsRunning" runat="server" class="row">
                        <span class="label-right">
                            <asp:CheckBox ID="IsRunning" runat="server" />&nbsp;<b> Is Running</b>
                        </span>
                        <span class="field">&nbsp;</span>
                    </div>
                    <div id="cbxIsInactive" runat="server" class="row">
                        <span class="label-right">
                            <asp:CheckBox ID="IsInactive" runat="server" />&nbsp;<b> Is Inactive</b>
                        </span>
                        <span class="field">&nbsp;</span>
                    </div>
                    <div class="row" id="divMonthYear" runat="server">
                        <span class="label">
                            <asp:Label ID="Label1" Text="Year " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectYear">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label7" Text="Month " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectMonth">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row" id="divEmpInfo" runat="server">
                        <span class="label">Employee ID
                            <label class="required-field">
                                *</label></span>

                        <span class="field">
                            <asp:TextBox ID="txtEmployeeId" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <div id="autoCompleteContainer"></div>
                            <asp:HiddenField runat="server" ID="hdnEmployeeId" />
                        </span>
                        <span class="label-right">Employee Name </span>
                        <span class="field">
                            <asp:TextBox ID="txtEmployeeName" runat="server" Class="read-only" ClientIDMode="Static"></asp:TextBox>
                            <asp:HiddenField runat="server" ID="hdnEmployeeName" />
                        </span>
                    </div>

                    <div class="row" id="divZoneInfo" runat="server">
                        <span class="label">
                            <asp:Label ID="lblZone" Text="Zone " runat="server" /><label class="required-field"> *</label>
                        </span>
                        <span class="field">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>

                    <div class="row">
                        <div class="button-crude button-center" style="width: 400px">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />

                            <asp:Button Text="Send Mail" runat="server" ID="btnSendMail" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnSendMail_Click" />

                            <asp:Button Text="View Nominee Report" runat="server" ID="btnViewNomineeReport" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnViewNomineeReport_Click" />
                        </div>
                        </span>
                         <span class="label-right">&nbsp; </span><span class="field">
                    </div>

                    <div class="clear">
                    </div>
                </div>
                <div>
                    <rsweb:ReportViewer ID="rvPFStatementRpt" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvPFStatementRpt_ReportRefresh" ZoomMode="PageWidth" SizeToReportContent="True">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
                </span>
            </fieldset>

        </ContentTemplate>

        <Triggers>
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>

    </asp:UpdatePanel>


    <style type="text/css">
        #autoCompleteContainer {
            display: block;
            position: relative;
        }

        .ui-autocomplete {
            position: absolute;
            z-index: 3000 !important;
        }
    </style>

    <script type="text/javascript">


          function LoadEmployeeAutocmplete() {

          var txtEmployeeId = $("#<%=txtEmployeeId.ClientID %>");
          var txtEmployeeName = $("#<%=txtEmployeeName.ClientID %>");

            txtEmployeeName.val($("#<%=hdnEmployeeName.ClientID %>").val());

            txtEmployeeId.autocomplete({
                appendTo: "#autoCompleteContainer",
                source: function (request, response) {
                    var param = { keyword: txtEmployeeId.val() };
                    $.ajax({
                        url: "CPFAutocomplete.aspx/GetEmployeeList",
                        data: JSON.stringify(param),
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d,
                                function (item) {
                                    return {
                                        label: item.EmpId + ' - ' + item.EmployeeName,
                                        value: item.EmpId,
                                        empName: item.EmployeeName
                                    }
                                }));
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            console.log(textStatus);
                        }
                    });
                },
                select: function (event, ui) {
                    if (ui.item) {

                        txtEmployeeId.val(ui.item.value);
                        $("#<%=hdnEmployeeId.ClientID %>").val(ui.item.value);

                        txtEmployeeName.val(ui.item.empName);
                        $("#<%=hdnEmployeeName.ClientID %>").val(ui.item.empName);

                    }
                },
                minLength: 1
            });

        }

        $("#btnViewReport").click(function () {
            $("#report").dialog({
                title: "Report Viewer",
                buttons: {
                    Close: function () {
                        $(this).dialog('close');
                    }
                },
                modal: true
            });
        });

        //$(function () {            
        //});
    </script>

</asp:Content>
