using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace EmailNotificationService
{
    public class Settings
    {
        public static IDbConnection GetConnectionInstance()
        {
            IDbConnection objIdbConnection = null;

            var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
            var doc = new System.Xml.XPath.XPathDocument(xmlpath);
            var navigator = doc.CreateNavigator();

            var serverName = navigator.SelectSingleNode("//appsettings/servername");
            var username = navigator.SelectSingleNode("//appsettings/username");
            var password = navigator.SelectSingleNode("//appsettings/password");
            var database = navigator.SelectSingleNode("//appsettings/database");

            objIdbConnection = new SqlConnection("Data Source=" + serverName.Value + ";Initial Catalog=" + database.InnerXml + ";User ID=" + username.InnerXml + ";Password=" + password.InnerXml + ";Connect Timeout=3600");

            return objIdbConnection;
        }
    }
}
