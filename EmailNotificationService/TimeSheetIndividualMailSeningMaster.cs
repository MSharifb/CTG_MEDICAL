using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
    public class TimeSheetIndividualMailSeningMaster : LogFilesWritting
    {
        public string smpt;
        public string FromemailAddress;
        public string FromemailPassword;

        public int NoOfPreviousMonthsConsider = default(int);
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
        string content = string.Empty;


        public TimeSheetIndividualMailSeningMaster()
        {
            try
            {
                var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
                var doc = new System.Xml.XPath.XPathDocument(xmlpath);
                var navigator = doc.CreateNavigator();

                smpt = navigator.SelectSingleNode("//appsettings/SMTP").Value;
                FromemailAddress = navigator.SelectSingleNode("//appsettings/FromEmail").Value;
                FromemailPassword = navigator.SelectSingleNode("//appsettings/FromEmailPassword").Value;

                NoOfPreviousMonthsConsider = Convert.ToInt16(navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/NoOfPreviousMonthsConsider").Value);
                EmailSendDateInMonthList = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/EmailSendDateInMonth").Value;
                CCEmailAddress = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/CCEmailAddress").Value;
                BCCEmailAddress = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/BCCEmailAddress").Value;

                Subject = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/Subject").Value + " " + DateTime.Now.AddMonths(-NoOfPreviousMonthsConsider).ToString("MMMM") + "/" + DateTime.Now.AddMonths(-NoOfPreviousMonthsConsider).Year;
                Salutation = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/Salutation").Value;
                BodyHeader = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/BodyHeader").Value;
                BodyFooter = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/BodyFooter").Value;
                NoRecordFound = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/NoRecordFound").Value;
                ToEmailPersonName = navigator.SelectSingleNode("//PIMEmailNotification/TimesheetEmailNotification/TimeSheetNotSubmittedLastMonthIndividual/ToEmailPersonName").Value;

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
        public void MailSendingIndividual() 
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

                SqlCommand command = new SqlCommand("SP_PIM_SendEmailIndividualForTimeSheet".Trim(), connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FromDate", startDate);
                command.Parameters.AddWithValue("@ToDate", endDate);
                command.Parameters.AddWithValue("@employeeID", string.Empty);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dsResult);

                DataTable dt = dsResult.Tables[0].DefaultView.ToTable(true, "EmpID", "EmialAddress", "FullName");

                if (dt.Rows.Count > 0)
                {
                    

                    foreach (DataRow drItem in dt.Rows)
                    {
                        string EmpID =Convert.ToString(drItem["EmpID"]);
                        string toEmialAddress = Convert.ToString(drItem["EmialAddress"]);
                        string toEmailPersonName = Convert.ToString(drItem["FullName"]);

                        TimeSheetEmailNotificationIndividual tsBodyInv = new TimeSheetEmailNotificationIndividual();
                        content = tsBodyInv.CreateBody(EmpID, NoOfPreviousMonthsConsider);
                        
                        EmailBody = "<html><body> <div style='display:block;'>" + Salutation + " " + toEmailPersonName + "</div></br>";
                        EmailBody += BodyHeader + "</br></br>" + content + "</br></br>";
                        EmailBody += "<div style='display:block;margin:10px 0;'>" + BodyFooter + "</div></body></html>";

                        EmailSending emailSending = new EmailSending();

                        if (emailSending.SendEmail(smpt, FromemailAddress, FromemailPassword, toEmialAddress, CCEmailAddress, BCCEmailAddress, Subject, EmailBody, ""))
                        {
                            LogInfo("Successful- Time Sheet individual. E-mail sent.");
                        }
                    }
                }
                else
                {
                    EmailBody = "<html><body> <div style='display:block;'>" + Salutation + " " + ToEmailPersonName + "</div></br>";
                    EmailBody += NoRecordFound + "</br></br>";
                    EmailBody += "<div style='display:block;margin:10px 0;'>" + BodyFooter + "</div></body></html>";
                }

            }
            catch (SqlException ex)
            {
                LogInfo(ex.Message);
            }
        }
    }
}
