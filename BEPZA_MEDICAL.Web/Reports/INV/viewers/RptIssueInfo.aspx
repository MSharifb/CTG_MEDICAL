<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptIssueInfo.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.INV.viewers.RptIssueInfo" %>

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
                    <span class="label">Issue Date From <label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:TextBox ID="dpIssueDateFrom" runat="server" CssClass="datePicker required" Width="100px" ></asp:TextBox>
                    </span>
                    <span class="label-right">To <label style='color: red'>
                        *</label></span>
                    <span class="field">
                        <asp:TextBox ID="dpIssueDateTo" runat="server" CssClass="datePicker required" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Indent Date From </span>
                    <span class="field">
                        <asp:TextBox ID="dpIndentDateFrom" runat="server" CssClass="datePicker" Width="100px" ></asp:TextBox>
                    </span>
                    <span class="label-right">To</span>
                    <span class="field">
                        <asp:TextBox ID="dpIndentDateTo" runat="server" CssClass="datePicker" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Issue No</span>
                    <span class="field">
                        <asp:TextBox ID="tbIssueNo" runat="server"></asp:TextBox>
                    </span>
                    <span class="label-right">Indent</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlIndent" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Issued To </span>
                    <span class="field">
                        <asp:TextBox ID="txtEmployeeId" runat="server" ></asp:TextBox>
                        <asp:HiddenField runat="server" ID="hdnEmployeeId" />
                    </span>
                    <span class="label-right">Employee Name </span>
                    <span class="field">
                        <asp:TextBox ID="txtEmployeeName" runat="server" Class ="read-only"></asp:TextBox>
                        <asp:HiddenField runat="server" ID="hdnEmployeeName" />
                    </span>
                </div>
                <div class="row">
                    <span class="label">Item Type</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItemType" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Category</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlCategory" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Color</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlColor" runat="server"></asp:DropDownList>
                    </span>
                    <span class="label-right">Model</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlModel" runat="server"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">Item</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlItem" runat="server"></asp:DropDownList>
                    </span>
                    <%--<span class="label-right">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>--%>
                </div>
                <div class="row">
                    <span class="label">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
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
                <rsweb:ReportViewer ID="rvIssueInfo" runat="server" Width="100%" Height="100%" Visible="false"
                    OnReportRefresh="rvIssueInfo_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvIssueInfo" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        var selectedValue;
        var selectedText;
        function LoadEmployeeAutocmplete() {

            $(document).ready(function () {
                (function ($) {
                    if (!$.curCSS) {
                        $.curCSS = $.css;
                    }
                })(jQuery);

                $("#<%=txtEmployeeName.ClientID %>").val($("#<%=hdnEmployeeName.ClientID %>").val());

                $("#<%=txtEmployeeId.ClientID %>").autocomplete({
                    source: function (request, response) {
                        var param = { keyword: $("#<%=txtEmployeeId.ClientID %>").val() };
                        $.ajax({
                            url: "INVAutocomplete.aspx/GetEmployeeList",
                            data: JSON.stringify(param),
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            dataFilter: function (data) { return data; },
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        value: item.EmpId + ' - ' + item.EmployeeName
                                    }
                                }))
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                alert(textStatus);
                            }
                        });
                    },
                    position: {
                        //my: "right bottom",
                        at: "left top",
                        //of: $("#EmpPopup"), 
                        offset: "1 25"
                    },
                    select: function (event, ui) {
                        if (ui.item) {
                            var EmpId = ui.item.value.split(' - ');

                            $("#<%=txtEmployeeId.ClientID %>").val(EmpId[0]);
                            $("#<%=hdnEmployeeId.ClientID %>").val(EmpId[0]);

                            $("#<%=txtEmployeeName.ClientID %>").val(EmpId[1]);
                            $("#<%=hdnEmployeeName.ClientID %>").val(EmpId[1]);

                            selectedValue = EmpId[0]; //console.log(selectedValue);
                            selectedText = EmpId[1];
                        }
                    },
                    minLength: 1
                });

                $("#<%=txtEmployeeId.ClientID %>").keydown(function (event) {
                    if (event.keyCode == 46 || event.keyCode == 8) {
                        selectedValue = ''; selectedText = '';
                        $("#<%=txtEmployeeId.ClientID %>").val('');
                        $("#<%=hdnEmployeeId.ClientID %>").val('');

                        $("#<%=txtEmployeeName.ClientID %>").val();
                        $("#<%=hdnEmployeeName.ClientID %>").val();
                    }
                });

                $("#<%=txtEmployeeId.ClientID %>").focusout(function () {
                    $("#<%=txtEmployeeId.ClientID %>").val(selectedValue);
                    $("#<%=hdnEmployeeId.ClientID %>").val(selectedValue);

                    $("#<%=txtEmployeeName.ClientID %>").val(selectedText);
                    $("#<%=hdnEmployeeName.ClientID %>").val(selectedText);
                });
            });
        }
    </script>
</asp:Content>
