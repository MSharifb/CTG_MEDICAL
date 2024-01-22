<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptDepartmentWiseManpowerInfo.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptDepartmentWiseManpowerInfo" %>



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
                <div class="row">
                    <span class="label">
                        <asp:Label ID="Label5" Text="From Date" runat="server" />
                        <label class="required-field">*</label>
                    </span>
                    <span class="field">
                        <asp:TextBox ID="txtFromDate" runat="server" ClientIDMode="Static" CssClass="datePicker date" Width="100px"></asp:TextBox>
                    </span>

                    <span class="label">
                        <asp:Label ID="Label1" Text="To Date" runat="server" />
                        <label class="required-field">*</label>
                    </span>
                    <span class="field">
                        <asp:TextBox ID="txtToDate" runat="server" ClientIDMode="Static" CssClass="datePicker date" Width="100px"></asp:TextBox>
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="Label2" Text="Department" runat="server" />
                    </span>
                    <span class="field">
                        <asp:DropDownList runat="server" ID="ddlDivision">
                        </asp:DropDownList>
                    </span>
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
            <div class="GroupBox">
                <rsweb:ReportViewer ID="rvDeptWiseManpower" runat="server" Width="100%"
                    Height="100%" OnReportRefresh="rvDeptWiseManpower_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvDeptWiseManpower" />
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
