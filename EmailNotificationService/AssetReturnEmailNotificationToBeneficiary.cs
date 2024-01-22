using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
    public class AssetReturnEmailNotificationToBeneficiary
    {
        LogFilesWritting _logFilesWritting = new LogFilesWritting();
        //public Int16 EmailSendDateInMonth;
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
        public bool IsRecordFound;

        public AssetReturnEmailNotificationToBeneficiary()
        {
            IsRecordFound = true;
            var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
            var doc = new System.Xml.XPath.XPathDocument(xmlpath);
            var navigator = doc.CreateNavigator();

            //EmailSendDateInMonth = Convert.ToInt16(navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/EmailSendDateInMonth").Value);

            var selectSingleNode = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/ToEmailAddress");
            if (selectSingleNode != null)
            {
                ToEmailAddress = selectSingleNode.Value;
            }

            CCEmailAddress = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/CCEmailAddress").Value;
            BCCEmailAddress = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/BCCEmailAddress").Value;

            Subject = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/Subject").Value + " " + DateTime.Now.ToString("MMMM") + "/" + DateTime.Now.Year;
            Salutation = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/Salutation").Value;
            BodyHeader = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/BodyHeader").Value;
            BodyFooter = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/BodyFooter").Value;
            NoRecordFound = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/NoRecordFound").Value;
            ToEmailPersonName = navigator.SelectSingleNode("//ADCEmailNotification/AssetReturnNotificationEmail/ToEmailPersonName").Value;

            tableHeaderBackgroundColor = navigator.SelectSingleNode("//ColorCode/TableHeaderBackgroundColor").Value;
            tableBorderColor = navigator.SelectSingleNode("//ColorCode/TableBorderColor").Value;

            string body = CreateBody();

            if (IsRecordFound)
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

        private string CreateBody()
        {
            DataSet dsResult = new DataSet();
            SqlConnection connection = null;

            string content = string.Empty;

            try
            {
                connection = Settings.GetConnectionInstance() as SqlConnection;
                connection.Open();

                SqlCommand command = new SqlCommand("ADC_EmailNotify_AssetNotReturned", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dsResult);

                if (dsResult.Tables[0].Rows.Count > 0)
                {
                    content = "<table id='contentTable' style='border-collapse:collapse;border:" + tableBorderColor + ";margin-left:10px; width:80%;'>";
                    content += "<thead style='width:80%;padding:1px;background:" + tableHeaderBackgroundColor + "; text-align:center; font-family:bold;'>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>B. Employee Name</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Initial</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Asset Name</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Purchase Date</th>";
                    content += "<th style='border-collapse:collapse;border:" + tableBorderColor + ";'>Expected Date Of Return</th>";
                    content += "</thead><tbody style='width:80%;'>";

                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        content += "<tr style='width:80%;'><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dr["BeneficiaryEmployeeName"] + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dr["BeneficiaryEmployeeInitial"] + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dr["AssetName"] + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dr["PurchaseDate"] + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += dr["ExpectedDateOfReturn"] + "</td><td style='border-collapse:collapse;border:" + tableBorderColor + ";'>";
                        content += "</td></tr>";

                    }
                    content += "</tbody></table>";
                }
                else
                {
                    IsRecordFound = false;
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
