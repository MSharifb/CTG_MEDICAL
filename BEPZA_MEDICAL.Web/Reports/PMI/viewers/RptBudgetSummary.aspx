<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/Common/MasterPage/ReportMaster.Master" AutoEventWireup="true" CodeBehind="RptBudgetSummary.aspx.cs" Inherits="BEPZA_MEDICAL.Web.Reports.PMI.viewers.RptBudgetSummary" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div runat="server" id="dvControls">
                <div class="GroupBox" id="message">
                    <asp:Label ID="lblMsg" runat="server" Style="text-align: center">
                    </asp:Label>
                </div>
                <div id="dvParam" style="width: 600px; margin-left: auto; margin-right: auto;">
                    <asp:GridView ID="Gridview1" runat="server" ShowFooter="true" AutoGenerateColumns="false" OnRowCreated="Gridview1_RowCreated">
                        <Columns>
                            <asp:TemplateField HeaderText="Financial Year">
                                <ItemTemplate>
                                    <asp:DropDownList ID="DropDownList1" runat="server"
                                        AppendDataBoundItems="true">
                                        <asp:ListItem Value="-1">Select</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:DropDownList ID="DropDownList2" runat="server"
                                        AppendDataBoundItems="true">
                                        <asp:ListItem Value="-1">Select</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="Right" />
                                <FooterTemplate>
                                    <asp:Button ID="ButtonAdd" runat="server"
                                        Text="Add New Row"
                                        OnClick="ButtonAdd_Click" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Remove</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <span class="label">Zone</span>
                    <span class="field">
                        <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                    </span>
                    <span class="label-right">Budget Head</span>
                    <span class="field">
                        <asp:DropDownList ID="ddlBudgetHead" runat="server"></asp:DropDownList>
                    </span>
                </div>
            </div>
            <div class="GroupBox">
                <div class="form-group">
                    <div class="text-center" style="text-align: center">
                        <div class="">
                            <asp:Button CssClass="btn btn-sm btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                type="submit" OnClick="btnViewReport_Click" />
                        </div>
                    </div>
                </div>
                <div class="clear clearfix">
                </div>
            </div>
            <rsweb:ReportViewer ID="rvBudgetSummary" runat="server" Width="100%" Height="100%" AsyncRendering="true" Visible="true" InteractiveDeviceInfos="(Collection)" SizeToReportContent="True"
                OnReportRefresh="rvBudgetSummary_ReportRefresh">
            </rsweb:ReportViewer>
            <div class="clear">
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvBudgetSummary" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function RemoveRow(obj) {
            //debugger;
            $(obj).closest('tr').remove();
            return false;
        }
    </script>
    <style type="text/css">
        #ContentPlaceHolder1_Gridview1 {
            border: 1px solid #E9E9E9 !important;
        }

            #ContentPlaceHolder1_Gridview1 tbody th {
                background: #E9E9E9;
                min-height: 3px;
                height: 30px;
                vertical-align: middle;
                text-align: center;
                font-weight: bold;
                border: 1px solid #E9E9E9 !important;
                min-width: 70px;
                font-size: 13px;
            }

            #ContentPlaceHolder1_Gridview1 tbody td {
                background: #FFFFFF;
                padding: 5px;
                border: 1px solid #E9E9E9 !important;
            }

                #ContentPlaceHolder1_Gridview1 tbody td select {
                    width: 200px;
                    height: 30px;
                }
    </style>
</asp:Content>
