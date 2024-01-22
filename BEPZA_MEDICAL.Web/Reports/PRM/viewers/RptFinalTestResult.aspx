<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptFinalTestResult.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptFinalTestResult" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GroupBox" id="message">
                <asp:Label ID="lblMsg" runat="server" Style="text-align: center">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Advertisement Code&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-8">
                                <asp:DropDownList ID="ddlAdvertisement" runat="server" CssClass="form-control required"></asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                Position Name&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-8">
                                <asp:DropDownList ID="ddlDesignation" runat="server" CssClass="form-control required"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group" style="display:none">
                            <label class="col-sm-4 control-label">
                                Zone
                            </label>
                            <div class="col-sm-8">
                                <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                            </div>
                        </div>
                    </div>
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

            <div class="GroupBox">
                <rsweb:ReportViewer ID="rvTestResult" runat="server" Width="100%" Height="100%"
                    OnReportRefresh="rvTestResult_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvTestResult" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">

        //cascading dropdown
        $('#<%=ddlAdvertisement.ClientID %>').change(function () {
            //alert($(this).val());

            var id = parseInt($(this).val());
            if (id > 0) {
                $.ajax({
                    type: "POST",
                    url: "RptFinallyApprovedApplicant.aspx/FetchDesignation",
                    data: "{Id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        var designationDDL = $('#<%=ddlDesignation.ClientID %>');
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
        })

    </script>
</asp:Content>
