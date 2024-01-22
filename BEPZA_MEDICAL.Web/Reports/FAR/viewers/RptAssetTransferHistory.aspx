<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptAssetTransferHistory.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.FAR.viewers.RptAssetTransferHistory" %>


<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 30px" id="message">
                <asp:Label ID="lblMsg" runat="server">        
                </asp:Label>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Asset Code
                            </label>
                            <div class="col-sm-7">
                                <asp:TextBox ClientIDMode="Static" ID="txtAssetCode" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Zone&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-7">
                                <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Location
                            </label>
                            <div class="col-sm-7">
                                <asp:DropDownList CssClass="form-control" runat="server" ID="ddlToLocation">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Sub Category
                            </label>
                            <div class="col-sm-7">
                                <asp:DropDownList CssClass="form-control" runat="server" ID="ddlSubCategory"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Employee ID
                            </label>
                            <div class="col-sm-7">
                                <asp:DropDownList CssClass="form-control" runat="server" ID="ddlEmployeeList">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Date Period From&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-7">
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-6">
                        <div class="form-group">
                            <label class="col-sm-5 control-label">
                                To&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-7">
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="GroupBox">
                <div class="text-center" style="text-align: center">
                    <div class="">
                        <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                            OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>
            <div>
                <rsweb:ReportViewer ID="rvAssetTransferHistory" runat="server" ShowExportControls="true" Visible="false"
                    Width="100%" Height="100%" OnReportRefresh="rvAssetTransferHistory_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvAssetTransferHistory" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">

        function LoadADCAutocmplete() {

            $(document).ready(function () {

                ///////////////////////// start of Asset ///////////////////////

                $("#<%=txtAssetCode.ClientID %>").autocomplete({
                    source: function (request, response) {
                        var param = { keyword: $("#<%=txtAssetCode.ClientID %>").val() };
                        $.ajax({
                            url: "FARAutocomplete.aspx/GetAssetList",
                            data: JSON.stringify(param),
                            dataType: "json",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            dataFilter: function (data) { return data; },
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        value: item.AssetCode
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
                        }
                    },
                    minLength: 1
                });

                $("#<%=txtAssetCode.ClientID %>").keydown(function (event) {
                    if (event.keyCode == 46 || event.keyCode == 8) {
                    }
                });
                
            }
    </script>
</asp:Content>
