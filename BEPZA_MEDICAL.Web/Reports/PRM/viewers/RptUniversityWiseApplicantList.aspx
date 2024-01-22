<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptUniversityWiseApplicantList.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptUniversityWiseApplicantList" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                                    Advertisement Code&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList runat="server" ID="ddladvertisementCode" ClientIDMode="Static" CssClass="form-control required">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Position Name
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control" runat="server" ID="ddlDesignation" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Degree Type&nbsp;<span class="required-field">*</span>
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control required" runat="server" ID="ddlDegreeType" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    University/Institute
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control" runat="server" ID="ddlUniversity" ClientIDMode="Static">
                                    </asp:DropDownList>
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
                <rsweb:ReportViewer ID="rvApplicant" runat="server" Width="100%"
                    Height="100%" OnReportRefresh="rvApplicant_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvApplicant" />
        </Triggers>
    </asp:UpdatePanel>

            <script type="text/javascript">

        //cascading dropdown
        $('#<%=ddladvertisementCode.ClientID %>').change(function () {
            //alert($(this).val());

            var id = parseInt($(this).val());
            if (id > 0) {
                $.ajax({
                    type: "POST",
                    url: "RptApplicantsSummaryInformation.aspx/FetchDesignation",
                    data: "{Id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        var designationDDL = $('#<%=ddlDesignation.ClientID %>');
                        designationDDL.empty();
                        designationDDL.append($('<option/>', {
                            value: '0',
                            text: 'All'
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
