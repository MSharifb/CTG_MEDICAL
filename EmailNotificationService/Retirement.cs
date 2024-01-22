using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
    public class Retirement
    {
        LogFilesWritting _logFilesWritting = new LogFilesWritting();
        public Int16 EmailSendDateInMonth;
        public string ToEmailAddress;
        public string CCEmailAddress;
        public string BCCEmailAddress;
        public string Subject;
        public string Salutation;
        public string BodyHeader;
        public string BodyFooter;
        public string ToEmailPersonName;
        public string EmailBody;
        public string tableHeaderBackgroundColor;
        public string tableBorderColor;
        public string NoRecordFound;
        public bool IsNoRecord=true;

        public Retirement()
        {
            var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
            var doc = new System.Xml.XPath.XPathDocument(xmlpath);
            var navigator = doc.CreateNavigator();

            EmailSendDateInMonth =Convert.ToInt16( navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/EmailSendDateInMonth").Value);
            var selectSingleNode = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/ToEmailAddress");
            if (selectSingleNode != null)
                ToEmailAddress = selectSingleNode.Value;
            CCEmailAddress = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/CCEmailAddress").Value;
            BCCEmailAddress = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/BCCEmailAddress").Value;
            
            Subject = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/Subject").Value + " " + DateTime.Now.AddMonths(1).ToString("MMMM") + "/" + DateTime.Now.AddMonths(1).Year;
            Salutation = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/Salutation").Value;
            BodyHeader = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/BodyHeader").Value;
            BodyFooter = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/BodyFooter").Value;
            NoRecordFound = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/NoRecordFound").Value;
            ToEmailPersonName = navigator.SelectSingleNode("//EmailType/RetirementNotificationEmail/ToEmailPersonName").Value;
           
            tableHeaderBackgroundColor = navigator.SelectSingleNode("//ColorCode/TableHeaderBackgroundColor").Value;
            tableBorderColor = navigator.SelectSingleNode("//ColorCode/TableBorderColor").Value;

            if (EmailSendDateInMonth == System.DateTime.Now.Day)
            {
               string body= CreateBody();
                if (IsNoRecord)
                {
                    EmailBody = "<html><body> <div style='display:block;'>" + Salutation + " " + ToEmailPersonName + "</div></br>";
                    EmailBody += BodyHeader + "</br></br>" + body + "</br></br>";
                    EmailBody += "<div style='display:block;margin:10px 0;'>" + BodyFooter + "</div></body></html>";
                }
                else
                {
                    EmailBody = "<html><body> <div style='display:block;'>" + Salutation + " " + ToEmailPersonName + "</div></br>";
                    EmailBody += NoRecordFound + "</br></br>";
                    EmailBody += "<div style='display:block;margin:10px 0;'>" + BodyFooter + "</div></body></html>";
                }
            }
        }

        private string CreateBody()
        {
            DataSet dsResult = new DataSet();
            SqlConnection connection = null;

            string content = string.Empty;

            try
            {
                connection = Settings.GetConnectionInstance() as SqlConnection;
                connection.Open();

                DateTime rr = Convert.ToDateTime(System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/01");

                DateTime startDate = rr.AddMonths(1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                
                SqlCommand command = new SqlCommand("PRM_RetirementList", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FromDate", startDate);
                command.Parameters.AddWithValue("@ToDate", endDate);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dsResult);

                if (dsResult.Tables[0].Rows.Count > 0)
                {
                    content = "<table id='contentTable' style='border-collapse:collapse;border:" + tableBorderColor + ";margin-left:10px; width:80%;'>";
                    content += "<thead style='width:80%;padding:1px;background:" + tableHeaderBackgroundColor + "; text-align:center; font-family:bold;'>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Initial</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Employee Name</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Designation</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Division</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Date of Joining</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Date of Retirement</th>";
                    content += "</thead><tbody style='width:80%;'>";

                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        var initial = dr["EmployeeInitial"];
                        var name = dr["FullName"];
                        var Designation = dr["Designation"];
                        var division = dr["Division"];
                        var dateOfJoining = Convert.ToDateTime(dr["DateofJoining"]).ToString("dd-MMM-yyyy");
                        var RetirementDate = Convert.ToDateTime(dr["RetirementDate"]).ToString("dd-MMM-yyyy");

                        content += "<tr style='width:80%;'><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += initial + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += name + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += Designation + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += division + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dateOfJoining + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += RetirementDate + "</td></tr>";
                    }
                    content += "</tbody></table>";
                }
                else
                {
                    IsNoRecord = false;
                }
            }
            catch (SqlException sex)
            {
                _logFilesWritting.LogInfo(sex.Message);
            }

            return content;
        }

    }
}
