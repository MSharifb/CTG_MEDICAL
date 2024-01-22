using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
    public class TimeSheetEmailNotificationList : LogFilesWritting
    {
        public int NoOfPreviousMonthsConsider=default(int);
        public string EmailSendDateInMonthList;
        public int EmailSendDateInMonth;
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

        public TimeSheetEmailNotificationList()
        {
            try
            {
                var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
                var doc = new System.Xml.XPath.XPathDocument(xmlpath);
                var navigator = doc.CreateNavigator();

                NoOfPreviousMonthsConsider = Convert.ToInt16(navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/NoOfPreviousMonthsConsider").Value);
                EmailSendDateInMonthList = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/EmailSendDateInMonth").Value;
                ToEmailAddress = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/ToEmailAddress").Value;
                CCEmailAddress = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/CCEmailAddress").Value;
                BCCEmailAddress = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/BCCEmailAddress").Value;

                Subject = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/Subject").Value + " " + DateTime.Now.AddMonths(-NoOfPreviousMonthsConsider).ToString("MMMM") + "/" + DateTime.Now.AddMonths(-NoOfPreviousMonthsConsider).Year;
                Salutation = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/Salutation").Value;
                BodyHeader = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/BodyHeader").Value;
                BodyFooter = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/BodyFooter").Value;
                NoRecordFound = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/NoRecordFound").Value;
                ToEmailPersonName = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthList/ToEmailPersonName").Value;

                tableHeaderBackgroundColor = navigator.SelectSingleNode("//ColorCode/TableHeaderBackgroundColor").Value;
                tableBorderColor = navigator.SelectSingleNode("//ColorCode/TableBorderColor").Value;

                List<int> EmailSendDateInMonthLists = new List<int>();
                if (EmailSendDateInMonthList.Length > 0)
                {
                    string[] list = EmailSendDateInMonthList.Split(',');
                    if (list.Count() > 0)
                    {
                        foreach (var item in list.Distinct())
                        {
                            if (Convert.ToInt32(item) == System.DateTime.Now.Day)
                            {
                                EmailSendDateInMonth = Convert.ToInt32(item);
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

                                break;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                LogInfo(ex.Message);
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

                DateTime startDate = rr.AddMonths(-NoOfPreviousMonthsConsider);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                SqlCommand command = new SqlCommand("SP_PIM_SendEmailForTimeSheet".Trim(), connection);
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
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Division</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Year</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Month</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Fortnight</th>";
                    content += "</thead><tbody style='width:80%;'>";

                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        var initial = dr["EmployeeInitial"];
                        var employeeName = dr["FullName"];
                        var Division = dr["Division"];
                        var TSYear = dr["TSYear"];
                        var TSMonth = dr["TSMonth"];
                        var FNight = dr["FNight"];

                        content += "<tr style='width:80%;'><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += initial + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += employeeName + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += Division + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";text-align:center;'>";
                        content += TSYear + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += TSMonth + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";text-align:center;'>";
                        content += FNight + "</td></tr>";

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
