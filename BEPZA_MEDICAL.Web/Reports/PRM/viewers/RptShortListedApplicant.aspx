<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptShortListedApplicant.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptShortListedApplicant" %>



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
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Reference  No.&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" ID="ddlRefNo" ClientIDMode="Static" CssClass="form-control required">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Position Name&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control required" runat="server" ID="ddlDesignation" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
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
            </div>
            <div class="GroupBox">
                <div class="form-group">
                    <div class="text-center" style="text-align: center">
                        <div class="">
                            <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                OnClick="btnViewReport_Click" />
                        </div>
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>
            <div class="GroupBox">
                <rsweb:ReportViewer ID="rvShortListedApplicant" runat="server" Width="100%"
                    Height="100%" OnReportRefresh="rvShortListedApplicant_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvShortListedApplicant" />
        </Triggers>
    </asp:UpdatePanel>



    <script type="text/javascript">

        //cascading dropdown
        $('#<%=ddlRefNo.ClientID %>').change(function () {
            //alert($(this).val());

            var id = parseInt($(this).val());
            if (id > 0) {
                $.ajax({
                    type: "POST",
                    url: "RptShortListedApplicant.aspx/FetchDesignation",
                    data: "{Id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        var designationDDL = $('#<%=ddlDesignation.ClientID %>');
                        designationDDL.empty();
                        designationDDL.append($('<option/>', {
                            value: '0',
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
