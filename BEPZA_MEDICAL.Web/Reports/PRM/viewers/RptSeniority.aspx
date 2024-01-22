<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptSeniority.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptSeniority" %>


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
                                <label class="col-sm-3 control-label">
                                    Zone
                                </label>
                                <div class="col-sm-9">
                                    <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-3 control-label">
                                    Department
                                </label>
                                <div class="col-sm-9">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlDivision" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-3 control-label">
                                    Section
                                </label>
                                <div class="col-sm-9">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlSection" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-3 control-label">
                                    Designation
                                </label>
                                <div class="col-sm-9">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlDesignation" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-3 control-label">
                                    Employee
                                </label>
                                <div class="col-sm-9">
                                      <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static" >
                                    </asp:DropDownList> 
                                   <%-- <asp:TextBox runat="server" ID="txtEmpId" ClientIDMode="Static" onpaste="GetEmployeeInfo();" />--%>
                                </div>
                            </div>
                        </div>
                      <%--  <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Employee Name
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" ID="txtEmpName" ReadOnly="true" ClientIDMode="Static" Style="cursor: default; background-color: #f2f5f8;" />
                                </div>
                            </div>
                        </div>--%>
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

        $('#txtEmpId').keyup(function () {
            GetEmployeeInfo();
        });

        $('#txtEmpId').keydown(function () {
            GetEmployeeInfo();
        });

        $('#txtEmpId').change(function () {
            GetEmployeeInfo();
        });

        function GetEmployeeInfo() {
            var param = $('#txtEmpId').val();
            $('#txtEmpName').val('');
            $.ajax({
                type: "POST",
                url: "EmployeeInfo.aspx/GetEmployeeNameByEmpId",
                data: '{empId: "' + param + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var EmpName = data.d;
                    $('#txtEmpName').val(EmpName);
                },
                failure: function (response) {
                    // alert(response.d);
                }
            });
        }

    </script>

</asp:Content>

