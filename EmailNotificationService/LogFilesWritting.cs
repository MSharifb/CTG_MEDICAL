using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Data.OleDb;

namespace EmailNotificationService
{
    public class LogFilesWritting
    {
        private readonly string logFileLocation;
        List<String> logString = new List<String>();

        public LogFilesWritting()
        {
            logFileLocation = CreateFile();
        }

        #region Log file maintenance for email notification------------------

        private void LogInfo()
        {
            LogInfo(string.Empty);
        }

        public void LogInfo(string lines)
        {
            string message = string.Format("{0:yyyy-MMM-dd hh:mm:ss:tt} - ", DateTime.Now) + lines;

            StreamWriter file = File.AppendText(logFileLocation);
            file.WriteLine(message);

            file.Close();
        }

        private void LogInfo(string format, params object[] args)
        {
            string message = string.Format("{0:yyyy-MM-dd hh:mm:ss:tt} - ", DateTime.Now) + (args.Length > 0 ? string.Format(format, args) : format);
            //var ddd = dd.LogFileLocation;
            StreamWriter file = File.AppendText(logFileLocation);

            file.WriteLine(message);

            file.Close();

        }

        public string CreateFile()
        {
            string FilePath = System.IO.Directory.GetCurrentDirectory();
            string FileName = "Notification_Log.txt";
            string path = FilePath + "\\" + FileName;

            if (!File.Exists(path))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Notification Log...........");
                    sw.WriteLine("************************");
                }
            }

            return path;
        }

        public void WriteIO()
        {
            string[] lines = logString.ToArray();

            string fileName = logFileLocation + "\\Notification_Log.txt";

            System.IO.File.WriteAllLines(fileName, lines);

        }

        #endregion
    }
}
