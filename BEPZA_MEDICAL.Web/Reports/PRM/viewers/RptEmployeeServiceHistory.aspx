<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptEmployeeServiceHistory.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PRM.viewers.RptEmployeeServiceHistory" %>

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
                <div class="form-horizontal">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">                              
                                </label>
                                <div class="col-sm-8">
                                <asp:RadioButtonList ID="rbReportType" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
                                    <asp:ListItem class="radio-inline" Text="Format 01" Value="F1" Selected="True"/>
                                    <asp:ListItem class="radio-inline" Text="Format 02" Value="F2" />
                                </asp:RadioButtonList> 
                                    </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6">
                             <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Employee
                                </label>
                                <div class="col-sm-8">
                                      <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static" >
                                    </asp:DropDownList>                                   
                                </div>
                            </div>
                        </div>   
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="col-sm-4 control-label">
                                    Designation
                                </label>
                                <div class="col-sm-8">
                                    <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlDesignation">
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
                    <div class="row">
                        
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
</asp:Content>
