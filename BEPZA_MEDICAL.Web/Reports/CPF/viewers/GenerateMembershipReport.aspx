<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master"
    CodeBehind="GenerateMembershipReport.aspx.cs" Inherits="BOM_MPA.Web.Reports.CPF.viewers.GenerateMembershipReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 15px" id="message">
                <asp:Label ID="lblMsg" runat="server">        
                </asp:Label>
            </div>
            <div class="GroupBox">
               
                <div class="row">
                    <span class="label">
                        <asp:Label ID="Label1" Text="Employee Initial" runat="server" /><label class="required-field">
                            *</label>
                    </span><span class="field">
                        <asp:TextBox runat="server" ID="txtEmpInitial" CssClass="required" />
                    </span><span class="label-right">
                        <asp:Label ID="Label2" Text="Employee Name" runat="server" /></span> <span class="field">
                            <asp:TextBox runat="server" ID="txtEmpName" ReadOnly="true" style = "cursor:default;background-color: #f2f5f8;"/>
                        </span>
                    
                </div>
                <div class="row">
                    <div class="button-crude">
                        <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                            OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                    </div>
                </div>
                <div class="clear">
                </div>
            </div>
            <div class="GroupBox">
                <rsweb:ReportViewer ID="rvGenerateMembership" runat="server" Width="100%" Height="100%"
                    OnReportRefresh="rvGenerateMembership_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvGenerateMembership" />
        </Triggers>
    
    </asp:UpdatePanel>

    <script type="text/javascript">

    function CPFLoadAutocomplete(){

        $(document).ready(function () {
            $("#<%=txtEmpInitial.ClientID %>").autocomplete({
                source: function (request, response) {
                    var param = { keyword: $("#<%=txtEmpInitial.ClientID %>").val() };
                    $.ajax({
                        url: "CPFAutocomplete.aspx/GetEmployeeInitialList",
                        data: JSON.stringify(param),
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    value: item.EmployeeInitial
                                }
                            }))
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert(textStatus);
                        }
                    });
                },
                select: function (event, ui) {
                    if (ui.item) {
                        GetCustomerDetails(ui.item.value);
                    }
                },
                minLength: 1
            });

            $("#<%=txtEmpInitial.ClientID %>").keydown(function (event) {
                if (event.keyCode == 46 || event.keyCode == 8) {
                    $("#<%=txtEmpName.ClientID %>").val('');
                }
            });
        });

        function GetCustomerDetails(initial) {

            $.ajax({
                type: "POST",
                url: "CPFAutocomplete.aspx/GetEmployeeNameByInitial",
                data: '{initial: "' + initial + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#<%=txtEmpName.ClientID %>").val(data.d);
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
        }

    }
    
    </script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#<%=txtEmpInitial.ClientID %>").autocomplete({
                source: function (request, response) {
                    var param = { keyword: $("#<%=txtEmpInitial.ClientID %>").val() };
                    $.ajax({
                        url: "CPFAutocomplete.aspx/GetEmployeeInitialList",
                        data: JSON.stringify(param),
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataFilter: function (data) { return data; },
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    value: item.EmployeeInitial
                                }
                            }))
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert(textStatus);
                        }
                    });
                },
                select: function (event, ui) {
                    if (ui.item) {
                        GetCustomerDetails(ui.item.value);
                    }
                },
                minLength: 1
            });

            $("#<%=txtEmpInitial.ClientID %>").keydown(function (event) {
                if (event.keyCode == 46 || event.keyCode == 8) {
                    $("#<%=txtEmpName.ClientID %>").val('');
                }
            });
        });

        function GetCustomerDetails(initial) {

            $.ajax({
                type: "POST",
                url: "CPFAutocomplete.aspx/GetEmployeeNameByInitial",
                data: '{initial: "' + initial + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#<%=txtEmpName.ClientID %>").val(data.d);
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
        }
        
    
    </script>
</asp:Content>
