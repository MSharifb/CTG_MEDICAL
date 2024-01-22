<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptAssetRegisterWithDepreciationCal.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.FAR.viewers.RptAssetRegisterWithDepreciationCal" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" ClientIDMode="Static">
    <style type="text/css">
        .inline-rb input[type="radio"] {
            width: auto;
        }

        .inline-rb label {
            display: inline;
            float: none !important;
            padding-right: 10px;
        }
    </style>
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
                                Report Type&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-7">
                                <asp:RadioButtonList ID="rdRptType" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" CssClass="inline-rb">
                                    <asp:ListItem Text="Summary" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Detail" Value="0"></asp:ListItem>
                                </asp:RadioButtonList>
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
                                Year&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-7">
                                <asp:DropDownList CssClass="form-control select-single required" runat="server" ID="ddlYear" ></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                  <%--  <div class="col-xs-6" id="divAssetCategory" style="display:none">--%>
                      <div class="col-xs-6">
                       <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Asset Category
                            </label>
                            <div class="col-sm-7">
                                <asp:DropDownList  CssClass="form-control select-single" runat="server" ID="ddlAssetCategory" Width="100%"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>

              <%--  <div class="row">
                    <div class="col-xs-6">
                         <div class="form-group">
                            <label class="col-sm-5 control-label">
                                Month&nbsp;<span class="required-field">*</span>
                            </label>
                            <div class="col-sm-7">
                                <asp:DropDownList CssClass="form-control select-single required" runat="server" ID="ddlMonth"></asp:DropDownList>
                            </div>
                        </div>                        
                    </div>
                </div>--%>

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
            <div">
                <rsweb:ReportViewer  ID="rvAssetRegisterWithDepreciationCal" runat="server" ShowExportControls="true" Visible="false"
                    Width="100%" Height="100%" OnReportRefresh="rvAssetRegisterWithDepreciationCal_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvAssetRegisterWithDepreciationCal" />
        </Triggers>
        <%-- <Triggers>
            <asp:PostBackTrigger ControlID="rdRptType" />
        </Triggers>--%>
    </asp:UpdatePanel>

    <script type="text/javascript">
        $('#<%=rdRptType.ClientID %>').change(function (e) {
            e.preventDefault();
            //debugger;
            var dd = $('#<%=rdRptType.ClientID %> input:checked').val();
            if ($('#<%=rdRptType.ClientID %> input:checked').val() == '1') {
                $('#divAssetCategory').hide();
            } else {
                $('#divAssetCategory').show();
            }

        });

    </script>
</asp:Content>
