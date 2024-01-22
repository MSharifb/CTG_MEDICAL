<%@ Page Language="C#" Title="Membership Application Preview" AutoEventWireup="true" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master"
CodeBehind="MembershipPreviewForm.aspx.cs" Inherits="BOM_MPA.Web.Reports.CPF.viewers.MembershipPreviewForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="width: 98%; margin: 10px auto; min-height: 15px" id="message">
                <div class="row">
                    
                   <span class="field">
                    <div class="button-crude button-left">
                        <asp:Button Text="Go Back" runat="server" ID="btnBack" ValidationGroup="view" type="submit"
                            OnClick="btnBack_Click" />
                    </div>
                    </span>
                    <span class="label-right">
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red">        
                    </asp:Label>
                    </span>
                </div>
            </div>
            <div class="GroupBox" style="width: 98%; margin: 10px auto;">
                <rsweb:ReportViewer ID="rvMembershipPreviewForm" runat="server" Width="100%" Height="100%"
                    OnReportRefresh="rvMembershipPreviewForm_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvMembershipPreviewForm" />
        </Triggers>
    </asp:UpdatePanel>

    <%--<script type="text/javascript">

        $(document).ready(function () {
            $("#<%=txtEmpInitial.ClientID %>").autocomplete({
                source: function (request, response) {
                    var param = { keyword: $("#<%=txtEmpInitial.ClientID %>").val() };
                    $.ajax({
                        url: "IncrementedEmployeeList.aspx/GetEmployeeInitialList",
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
                url: "IncrementedEmployeeList.aspx/GetEmployeeNameByInitial",
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
        
    
    </script>--%>

</asp:Content>