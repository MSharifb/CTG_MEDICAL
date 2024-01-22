using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;

namespace EmailNotificationService
{
    public class TimeSheetEmailNotificationIndividual : LogFilesWritting
    {
       
        public string tableHeaderBackgroundColor;
        public string tableBorderColor;
        public bool IsNoRecord = true;
        string content = string.Empty;


        public TimeSheetEmailNotificationIndividual()
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

        public string CreateBody(string empID, int NoOfPreviousMonthsConsider)
        {
            DataSet dsResult = new DataSet();
            SqlConnection connection = null;

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
                command.Parameters.AddWithValue("@employeeID", empID);


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

        //public DataTable SelectDistinct(DataTable SourceTable, string FieldName)
        //{
        //    // Create a Datatable â€“ datatype same as FieldName
        //    DataTable dt = new DataTable(SourceTable.TableName);
        //    dt.Columns.Add(FieldName, SourceTable.Columns[FieldName].DataType);
        //    // Loop each row & compare each value with one another
        //    // Add it to datatable if the values are mismatch
        //    object LastValue = null;
        //    foreach (DataRow dr in SourceTable.Select("", FieldName))
        //    {
        //        if (LastValue == null || !(ColumnEqual(LastValue, dr[FieldName])))
        //        {
        //            LastValue = dr[FieldName];
        //            dt.Rows.Add(new object[] { LastValue });
        //        }
        //    }
        //    return dt;
        //}
        //private bool ColumnEqual(object A, object B)
        //{
        //    // Compares two values to see if they are equal. Also compares DBNULL.Value.           
        //    if (A == DBNull.Value && B == DBNull.Value) //  both are DBNull.Value
        //        return true;
        //    if (A == DBNull.Value || B == DBNull.Value) //  only one is BNull.Value
        //        return false;
        //    return (A.Equals(B));  // value type standard comparison
        //}
    }
}
