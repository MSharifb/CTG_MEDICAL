﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptFDRDailyreport.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.FMS.viewers.RptFDRDailyreport" %>

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
                                    Zone
                                </label>
                                <div class="col-sm-8">
                                    <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    FDR No.
                                </label>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtFDRNo" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                     <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Open/Renewal Year
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlYear">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Month
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlMonth">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="display: none">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                   Offer Duration
                                </label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtDuration" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                </label>
                                 <div class="col-sm-6">
                                        <asp:RadioButton ID="RadioButton5"  GroupName="FDR" runat="server" Text="All" Checked="True"/>
                                        <asp:RadioButton ID="RadioButton1"  GroupName="FDR" runat="server" Text="Year"/>
                                        <asp:RadioButton ID="RadioButton2" GroupName="FDR" runat="server" Text="Month"/>
                                </div>
                            </div>
                         </div>
                    </div> 
                     
                    <div class="row">
                         <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Date
                                </label>
                                <div class="col-sm-4">
                                    <asp:TextBox ID="txtFDRDateTo" runat="server" CssClass="datePicker" Width="100px"></asp:TextBox>
                                </div>
                                
                            </div>
                        </div>    
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                </label>
                                <div class="col-sm-6">
                                        <asp:RadioButton ID="RadioButton3"  GroupName="FDRDate" runat="server" Text="Initial Deposite" Checked="True"/>
                                        <asp:RadioButton ID="RadioButton4" GroupName="FDRDate" runat="server" Text="FDR Amount"/>
                                 </div>
                            </div>
                         </div>           
                    </div>  
                                  
                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Bank
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlBank">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Branch
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlBranch">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Bank Type
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlBankType">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Fund Type
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList  CssClass="form-control"  runat="server" ID="ddlFundType" Enabled="False">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                     </div>

                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                               <label class="col-sm-4 control-label">
                                   With Footer
                                </label>
                               <div class="col-sm-8">
                                <asp:CheckBox ID="isFooter" runat="server" />
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
                <div id="rptContainer" style="width: 100%; height: 100%; display: block; text-align: center;">
                    <rsweb:ReportViewer ID="rvFDRSchedule" runat="server" Width="100%" ShowPrintButton="true" SkinID="" AsyncRendering="true" Visible="true"
                        Height="100%" InteractiveDeviceInfos="(Collection)"
                        OnReportRefresh="rvFDRSchedule_ReportRefresh" SizeToReportContent="true">
                    </rsweb:ReportViewer>
                </div>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvFDRSchedule" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">

        //On UpdatePanel Refresh
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm != null) {
            prm.add_endRequest(function (sender, e) {
                if (sender._postBackSettings.panelsToUpdate != null) {
                    $("#ddlZone").multiselect();
                }
            });
        };

    </script>

</asp:Content>
