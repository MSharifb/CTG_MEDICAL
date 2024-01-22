using BEPZA_MEDICAL.DAL.PRM;
using BEPZA_MEDICAL.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace BEPZA_MEDICAL.Web.Reports.PRM.viewers
{
    public partial class RptSeniority : ReportBase
    {
        #region Page Event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }
        #endregion

        #region User Methods

        private void PopulateDropdownList()
        {
            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            var divisionList = (from orgTypeMap in base.context.PRM_OrganogramTypeMapping
                                join orgLevel in base.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                                where orgTypeMap.TableIdName == "DivisionId"
                                select new
                                {
                                    Id = orgLevel.Id, //Organogramlevel Id used as a Division Id
                                    Name = orgLevel.LevelName //Organogramlevel Name used as a Division Name
                                }).ToList();

            ddlDivision.DataSource = context.PRM_Division.Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            ddlDivision.DataValueField = "Id";
            ddlDivision.DataTextField = "Name";
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0, new ListItem("All", "0"));


            var officeList = (from orgTypeMap in base.context.PRM_OrganogramTypeMapping
                              join orgLevel in base.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                              where orgTypeMap.TableIdName == "DisciplineId"
                              select new
                              {
                                  Id = orgLevel.Id, //Organogramlevel Id used as a Discipline Id
                                  Name = orgLevel.LevelName //Organogramlevel Name used as a Discipline Name
                              }).ToList();



            var sectionList = (from orgTypeMap in base.context.PRM_OrganogramTypeMapping
                               join orgLevel in base.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                               where orgTypeMap.TableIdName == "SectionId"
                               select new
                               {
                                   Id = orgLevel.Id, //Organogramlevel Id used as a Section Id
                                   Name = orgLevel.LevelName //Organogramlevel Name used as a Section Name
                               }).ToList();

            ddlSection.DataSource = context.PRM_Section.Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();
            ddlSection.DataValueField = "Id";
            ddlSection.DataTextField = "Name";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("All", "0"));



            var designationList = (from desig in base.context.PRM_Designation
                                   select new
                                   {
                                       Id = desig.Id,
                                       Name = desig.Name
                                   }).ToList();

            ddlDesignation.DataSource = context.PRM_Designation.OrderBy(x => x.SortingOrder).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

            ddlEmployee.DataSource = context.PRM_EmploymentInfo.Select(q => new { ZoneInfoId = q.ZoneInfoId, EmpID = q.EmpID, DisplayText = q.FullName + " [" + q.EmpID +" ]"}).Where(x => x.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.DisplayText).ToList();
            ddlEmployee.DataValueField = "EmpID";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", ""));

        }

        #endregion

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            List<int> zoneList = new List<int>();

            foreach (ListItem item in ddlZone.Items)
            {
                if (item.Selected)
                {
                    zoneList.Add(Convert.ToInt32(item.Value));
                }
            }

            string strZoneId = string.Join(",", zoneList.ToArray());
            var divisionId = Convert.ToInt32(ddlDivision.SelectedValue);
            var sectionId = Convert.ToInt32(ddlSection.SelectedValue);
            var designationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            var empID = ddlEmployee.SelectedValue;
            //var empID = string.Empty;
            //empID = txtEmpId.Text;
            GenerateReport(strZoneId, divisionId, sectionId, designationId, empID);

        }

        #endregion

        #region Generate Report

        public void GenerateReport(string strZoneId, int divisionId, int sectionId, int designationId, string empID)
        {
            List<PRM_ZoneInfo> list = new List<PRM_ZoneInfo>();
            rvEmployeeInfo.Reset();
            rvEmployeeInfo.ProcessingMode = ProcessingMode.Local;
            rvEmployeeInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PRM/rdlc/RptSeniority.rdlc");

            #region Processing Report Data

            var data = (from e in base.context.SP_PRM_RptSeniority(strZoneId, divisionId, sectionId, designationId, empID) select e).ToList();

            #endregion

            if (data.Count > 0)
            {
                rvEmployeeInfo.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                //string searchParameters = "As on : " + effectiveDate.ToString("dd MMM yyyy");
                //ReportParameter p1 = new ReportParameter("param", searchParameters);
                //rvEmployeeInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }

            this.rvEmployeeInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeInfo.DataBind();

            //ExportToPDF
            String newFileName = "SeniorityEmployeeList_" + Guid.NewGuid() + ".pdf";
            String newFilePath = "~/Content/TempFiles/" + newFileName;
            FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Create);
            BEPZA_MEDICAL.Web.Utility.Common.ExportPDF(rvEmployeeInfo, newFileName, fs);
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
          
            dynamic data = null;
            var dsName = string.Empty;
            var empId = 0;

            if (e.ReportPath != "_ReportHeader")
            {
                empId = Convert.ToInt32(e.Parameters["EmployeeId"].Values[0].ToString());
            }

            switch (e.ReportPath)
            {

                case "_PromotedDesignation":
                    data = base.context.SP_PRM_RptSeniorityPromotedDesignation(empId).ToList();
                    dsName = "DataSet1";
                    break;


                case "_ReportHeader":
                    data = (from c in base.context.SP_PRM_GetReportHeaderByZoneID(LoggedUserZoneInfoId)
                            select c).ToList();
                    dsName = "DSCompanyInfo";
                    break;

                default:
                    break;
            }

            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        #endregion

        protected void rvEmployeeInfo_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportBase dbContext = new ReportBase();

            var officeList = (from orgTypeMap in dbContext.context.PRM_OrganogramTypeMapping
                              join orgLevel in dbContext.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                              where (orgTypeMap.TableIdName == "DisciplineId" && orgLevel.ParentId == Convert.ToInt32(ddlDivision.SelectedValue))
                              select new
                              {
                                  Id = orgLevel.Id, //Organogramlevel Id used as a Discipline Id
                                  Name = orgLevel.LevelName //Organogramlevel Name used as a Discipline Name
                              }).ToList();



            var sectionList = (from orgTypeMap in dbContext.context.PRM_OrganogramTypeMapping
                               join orgLevel in dbContext.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                               where (orgTypeMap.TableIdName == "SectionId" && orgLevel.ParentId == Convert.ToInt16(ddlDivision.SelectedValue))
                               select new
                               {
                                   Id = orgLevel.Id, //Organogramlevel Id used as a Section Id
                                   Name = orgLevel.LevelName //Organogramlevel Name used as a Section Name
                               }).ToList();

            ddlSection.DataSource = sectionList;
            ddlSection.DataValueField = "Id";
            ddlSection.DataTextField = "Name";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("All", "0"));
        }


        [WebMethod]
        public static ArrayList GetOfficeByDeptId(int Id)
        {
            ReportBase dbContext = new ReportBase();
            ArrayList list = new ArrayList();

            var items = (from orgTypeMap in dbContext.context.PRM_OrganogramTypeMapping
                         join orgLevel in dbContext.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                         where orgTypeMap.TableIdName == "DisciplineId" && orgLevel.ParentId == Id

                         select new
                         {
                             Id = orgLevel.Id,
                             Name = orgLevel.LevelName
                         }).ToList();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }


        [WebMethod]
        public static ArrayList GetSectionByDeptId(int Id)
        {
            ReportBase dbContext = new ReportBase();
            ArrayList list = new ArrayList();

            var items = (from orgTypeMap in dbContext.context.PRM_OrganogramTypeMapping
                         join orgLevel in dbContext.context.PRM_OrganogramLevel on orgTypeMap.OrganogramTypeId equals orgLevel.OrganogramTypeId
                         where orgTypeMap.TableIdName == "SectionId" && orgLevel.ParentId == Id

                         select new
                         {
                             Id = orgLevel.Id,
                             Name = orgLevel.LevelName
                         }).ToList();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }


        [WebMethod]
        public static ArrayList GetDesignation(int Id)
        {
            ReportBase dbContext = new ReportBase();
            ArrayList list = new ArrayList();

            var items = (from orgManpwr in dbContext.context.PRM_OrganizationalSetupManpowerInfo
                         join designation in dbContext.context.PRM_Designation on orgManpwr.DesignationId equals designation.Id
                         where orgManpwr.OrganogramLevelId == Id
                         select new
                         {
                             Id = designation.Id,
                             Name = designation.Name
                         }).ToList();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }

        [WebMethod]
        public static string GetTreeData()
        {
            ReportBase dbContext = new ReportBase();
            var nodes = dbContext.context.PRM_OrganogramLevel.ToList();

            var parentNode = nodes.Where(x => x.ParentId == 0).FirstOrDefault();

            JsTreeNode rootNode = new JsTreeNode();

            if (parentNode != null)
            {
                rootNode.attr = new Attributes();
                rootNode.attr.id = Convert.ToString(parentNode.Id);
                rootNode.attr.rel = "root" + Convert.ToString(parentNode.Id);
                rootNode.data = new Data();

                StringBuilder lvlName = GenerateNodeText(parentNode);
                rootNode.data.title = Convert.ToString(lvlName);
                rootNode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";
                //  PopulateTree(parentNode, rootNode, nodes);
            }


            return new JavaScriptSerializer().Serialize(nodes);
        }

        private static StringBuilder GenerateNodeText(PRM_OrganogramLevel parentNode)
        {
            StringBuilder lvlName = new StringBuilder();
            lvlName.Append(parentNode.LevelName);

            if (parentNode.PRM_OrganogramType != null)
            {
                lvlName.Append(" [");
                lvlName.Append(parentNode.PRM_OrganogramType.Name);
                lvlName.Append("]");
            }
            return lvlName;
        }
        public void PopulateTree(PRM_OrganogramLevel parentNode, JsTreeNode jsTNode, List<PRM_OrganogramLevel> nodes)
        {
            StringBuilder nodeText = new StringBuilder();
            jsTNode.children = new List<JsTreeNode>();
            foreach (var dr in nodes)
            {
                if (dr != null)
                {
                    if (dr.ParentId == parentNode.Id)
                    {
                        JsTreeNode cnode = new JsTreeNode();
                        cnode.attr = new Attributes();
                        cnode.attr.id = Convert.ToString(dr.Id);
                        cnode.attr.rel = "folder" + dr.Id;
                        cnode.data = new Data();
                        nodeText = GenerateNodeText(dr);
                        cnode.data.title = Convert.ToString(nodeText);

                        cnode.attr.mdata = "{ draggable : true, max_children : 100, max_depth : 100 }";

                        jsTNode.children.Add(cnode);
                        PopulateTree(dr, cnode, nodes);
                    }
                }
            }
        }

    }
}


