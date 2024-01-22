using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
   public class AnnualIncrementEmailNotification
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
        public bool IsNoRecord = true;

        public AnnualIncrementEmailNotification()
        {
            var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
            var doc = new System.Xml.XPath.XPathDocument(xmlpath);
            var navigator = doc.CreateNavigator();

            EmailSendDateInMonth = Convert.ToInt16(navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/EmailSendDateInMonth").Value);
            ToEmailAddress = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/ToEmailAddress").Value;
            CCEmailAddress = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/CCEmailAddress").Value;
            BCCEmailAddress = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/BCCEmailAddress").Value;

            Subject = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/Subject").Value + " " + DateTime.Now.AddMonths(1).ToString("MMMM") + "/" + DateTime.Now.AddMonths(1).Year;
            Salutation = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/Salutation").Value;
            BodyHeader = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/BodyHeader").Value;
            BodyFooter = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/BodyFooter").Value;
            ToEmailPersonName = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/ToEmailPersonName").Value;
            NoRecordFound = navigator.SelectSingleNode("//EmailType/AnnualIncrementNotificationEmail/NoRecordFound").Value;
           
            tableHeaderBackgroundColor = navigator.SelectSingleNode("//ColorCode/TableHeaderBackgroundColor").Value;
            tableBorderColor = navigator.SelectSingleNode("//ColorCode/TableBorderColor").Value;

            if (EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                string body = CreateBody();
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

                SqlCommand command = new SqlCommand("PRM_GetEmpForAnnualIncrement".Trim(), connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FromDate", startDate);
                command.Parameters.AddWithValue("@ToDate", endDate);

                command.Parameters.AddWithValue("@EmploymentTypeId", 0);
                command.Parameters.AddWithValue("@DivisionId", 0);
                command.Parameters.AddWithValue("@GradeId", 0);
                command.Parameters.AddWithValue("@DesignationId", 0);

                command.Parameters.AddWithValue("@numErrorCode", 0);
                command.Parameters.AddWithValue("@strErrorMsg", "");

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dsResult);

                if (dsResult.Tables[0].Rows.Count > 0)
                {

                    content = "<table id='contentTable' style='border-collapse:collapse;border:" + tableBorderColor + ";margin-left:10px; width:80%;'>";
                    content += "<thead style='width:80%;padding:1px;background:" + tableHeaderBackgroundColor + "; text-align:center; font-family:bold;'>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Initial</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Employee Name</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Job Title</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Date of Joining</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Grade</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Step</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Current Basic</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Last Increment Date</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Next Increment Date</th>";
                    content += "</thead><tbody style='width:80%;'>";

                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        var initial = dr["EmployeeInitial"];
                        var name = dr["FullName"];
                        var Designation = dr["Designation"];
                        var dateOfJoining = Convert.ToDateTime(dr["DateofJoining"]).ToString("dd-MMM-yyyy");
                        var GradeName = dr["GradeName"];
                        var StepName = dr["StepName"];
                        var currentBasic = Math.Round(Convert.ToDecimal(dr["BasicSalary"]), 2);
                        var LastIncrementDate = dr["LastIncrementDate"] != System.DBNull.Value ? Convert.ToDateTime(dr["LastIncrementDate"]).ToString("dd-MMM-yyyy") : string.Empty;
                        var NextIncrementDate = dr["NextIncrementDate"] != System.DBNull.Value ? Convert.ToDateTime(dr["NextIncrementDate"]).ToString("dd-MMM-yyyy") : string.Empty;

                        content += "<tr style='width:80%;'><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += initial + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += name + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += Designation + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dateOfJoining + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";text-align:center;'>";
                        content += GradeName + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += StepName + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += currentBasic + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";text-align:right;'>";
                        content += LastIncrementDate + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";text-align:center;'>";
                        content += NextIncrementDate + "</td></tr>";

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
                Console.WriteLine(sex.Message);
            }

            return content;
        }
    }
}
