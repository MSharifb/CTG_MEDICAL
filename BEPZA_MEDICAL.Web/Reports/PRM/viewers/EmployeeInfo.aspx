<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.master"
    CodeBehind="EmployeeInfo.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.EmployeeInfo" %>

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
                <div class="form-horizontal">
                    <div class="row">
                        <div class="col-sm-6">
                             <div class="form-group">
                                <label class="col-sm-3 control-label">
                                    Employee
                                </label>
                                <div class="col-sm-9">
                                      <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static" >
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
                <rsweb:ReportViewer ID="rvEmployeeInfo" runat="server" Width="100%" Height="100%" Visible="true"
                    OnReportRefresh="rvEmployeeInfo_ReportRefresh" SizeToReportContent="True">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvEmployeeInfo" />
        </Triggers>
    </asp:UpdatePanel>


<%--    <script type="text/javascript">

        $('#<%=txtEmpId.ClientID %>').keyup(function () {
            GetEmployeeInfo();
        });

        $('#<%=txtEmpId.ClientID %>').keydown(function () {
            GetEmployeeInfo();
        });

        $('#<%=txtEmpId.ClientID %>').change(function () {
            GetEmployeeInfo();
        });



        function GetEmployeeInfo() {
            var param = $("#<%=txtEmpId.ClientID %>").val();
            $.ajax({
                type: "POST",
                url: "EmployeeInfo.aspx/GetEmployeeNameByEmpId",
                data: '{empId: "' + param + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var EmpName = data.d;
                    $('#<%=txtEmpName.ClientID %>').val(EmpName);
                },
                failure: function (response) {
                    // alert(response.d);
                }
            });
        }

    </script>--%>

</asp:Content>
