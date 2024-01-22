using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
    public class MilestoneDeliveryEmailNotificationChild : LogFilesWritting
    {
        public string tableHeaderBackgroundColor;
        public string tableBorderColor;
        public bool IsNoRecord = true;
        string content = string.Empty;


        public MilestoneDeliveryEmailNotificationChild()
        {
            try
            {
                var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
                var doc = new System.Xml.XPath.XPathDocument(xmlpath);
                var navigator = doc.CreateNavigator();

                tableHeaderBackgroundColor = navigator.SelectSingleNode("//ColorCode/TableHeaderBackgroundColor").Value;
                tableBorderColor = navigator.SelectSingleNode("//ColorCode/TableBorderColor").Value;
            }

            catch (Exception ex)
            {
                LogInfo(ex.Message);
            }
        }

        /// <summary>
        /// Milestone Delivery email notification. Project wise mail body creation by using this method for each project. Using SP is SP_PIM_SendEmailForMilestoneDelivery
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="NoOfAfterMonthsConsider"></param>
        /// <returns></returns>
        public string CreateBody(int ProjectId, int NoOfAfterMonthsConsider)
        {
            DataSet dsResult = new DataSet();
            SqlConnection connection = null;

            try
            {
                connection = Settings.GetConnectionInstance() as SqlConnection;
                connection.Open();

                // Start date is equals to current date and End date is equals to (startDate+NoOfAfterMonthsConsider)
                DateTime startDate = Convert.ToDateTime(System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day);
                DateTime endDate = startDate.AddMonths(NoOfAfterMonthsConsider);

                SqlCommand command = new SqlCommand("SP_PIM_SendEmailForMilestoneDelivery".Trim(), connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FromDate", startDate);
                command.Parameters.AddWithValue("@ToDate", endDate);
                command.Parameters.AddWithValue("@ProjectId", ProjectId);


                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dsResult);

                if (dsResult.Tables[0].Rows.Count > 0)
                {

                    content = "<table id='contentTable' style='border-collapse:collapse;border:" + tableBorderColor + ";margin-left:10px; width:80%;'>";
                    content += "<thead style='width:80%;padding:1px;background:" + tableHeaderBackgroundColor + "; text-align:center; font-family:bold;'>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Project No.</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Project Title</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Division</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>PL</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>PS</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Milestone Name</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Scheduled Delivery Date</th>";
                    content += "</thead><tbody style='width:80%;'>";

                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        var ProjectNo = dr["ProjectNo"];
                        var ProjectTitle = dr["ProjectTitle"];
                        var Division = dr["Division"];
                        var PLName = dr["PLName"];
                        var PSName = dr["PSName"];
                        var ActivityName = dr["ActivityName"];
                        var ScheduleDate =Convert.ToDateTime(dr["ScheduleDate"]).ToString("dd-MMM-yyyy");

                        content += "<tr style='width:80%;'><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += ProjectNo + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += ProjectTitle + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += Division + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += PLName + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += PSName + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += ActivityName + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";text-align:center;'>";
                        content += ScheduleDate + "</td></tr>";

                    }
                    content += "</tbody></table>";
                }
                else
                {
                    IsNoRecord = false;
                }
            }
            catch (SqlException ex)
            {
                LogInfo(ex.Message);
            }

            return content;
        }
    }
}
