<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptDesignation.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptDesignation" %>



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
                        <asp:Label ID="Label2" Text="Designation" runat="server" />
                    </span>
                    <span class="field">
                        <asp:DropDownList runat="server" ID="ddlDesignation" ClientIDMode="Static">
                        </asp:DropDownList>
                    </span>

<%--                    <span class="label-right">
                        <asp:Label ID="Label3" Text="Job Grade" runat="server" />
                    </span>
                    <span class="field">
                        <asp:DropDownList runat="server" ID="ddlJobGrade" ClientIDMode="Static">
                        </asp:DropDownList>
                    </span>--%>

                </div>

                <div class="row">
                    <span class="label">&nbsp; </span>
                    <span class="field">
                        <div class="button-crude button-left">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                type="submit" OnClick="btnViewReport_Click" />
                        </div>
                    </span>
                </div>
                <div class="clear">
                </div>
            </div>
            <div>
                <rsweb:ReportViewer ClientIDMode="Static" ID="rvDesignation" runat="server" ShowPrintButton="true" Width="100%" Visible="true"
                    Height="100%" OnReportRefresh="rvDesignation_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvDesignation" />
        </Triggers>
    </asp:UpdatePanel>

<%--    <script type="text/javascript">

        //cascading dropdown
        $('#<%=ddlDesignation.ClientID %>').change(function () {
           // alert($('#<%=ddlDesignation.ClientID %> option:selected').val());
            FillJobGrade($(this).val());
        })

        // JobGrade by Designaiton
        function FillJobGrade(designationId) {

            $.ajax({
                type: "POST",
                url: "RptDesignation.aspx/GetJobGradeByDesignaitonId",
                data: '{id: "' + designationId + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
               success: function (response) {
                    //debugger;
                    var heads = $.parseJSON(response);
                    var JobGrades = $('#<%=ddlJobGrade.ClientID %>');
                    // clear all previous options
                   // $("#DesignationId>option").remove();
                    // populate the products
                    for (i = 0; i < heads.length; i++) {
                        JobGrades.append($("<option />").val(heads[i].Value).text(heads[i].Text));
                    }
                },
                success: function (r) {
                    //debugger;
                    var rr = r.d;

                    var ddlJobGrades = $('#<%=ddlJobGrade.ClientID %>');
                   // ddlJobGrades.empty().append('<option selected="selected" value="0">Please select</option>');
                    $.each(r.d, function () {
                        ddlJobGrades.append($("<option></option>").val(this['GradeId']).html(this['GradeName']));
                    });
                },
                failure: function (response) {
                    // alert(response.d);
                }
             });
        }
    </script>--%>
</asp:Content>
