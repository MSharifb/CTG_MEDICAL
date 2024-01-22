﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptFixedAssetRegistration.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.FAR.viewers.RptFixedAssetRegistration" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" ClientIDMode="Static">
   
     <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 30px" id="message">
                <asp:Label ID="lblMsg" runat="server">        
                </asp:Label>
            </div>
            <div class="clearfix"></div>
            <div class="GroupBox">
                <div class="">
                    <div class="row">
                        <div class="col-sm-4">
                            <div class="form-group">
                                Zone&nbsp;<span class="required-field">*</span><br />
                                <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                Asset         
                                <br />
                                <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlAssetList" ClientIDMode="Static" Width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                Category<br />
                                <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlCategory" AutoPostBack="True" ClientIDMode="Static" Width="100%"
                                    OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-4">
                            <div class="form-group">
                                Date Period From&nbsp;<span class="required-field">*</span><br />
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>

                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                To&nbsp;<span class="required-field">*</span><br />
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker date" Width="100px"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                Sub Category<br />
                                <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlSubCategory" ClientIDMode="Static" Width="100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
             <div class="clear clearfix">
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
                <rsweb:ReportViewer ID="rvFixedAssetRegistration" runat="server" ShowExportControls="true" Visible="false"
                    Width="100%" Height="100%" OnReportRefresh="rvFixedAssetRegistration_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvFixedAssetRegistration" />
        </Triggers>
    </asp:UpdatePanel>   
</asp:Content>