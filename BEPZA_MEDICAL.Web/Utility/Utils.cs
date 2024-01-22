
using BEPZA_MEDICAL.DAL.CPF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;

namespace BEPZA_MEDICAL.Web.Utility
{
    public class NoCache : ActionFilterAttribute, IActionFilter
    {



        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }

    public class PropertyReflector
    {
        public static T ClearProperties<T>(T obj)
        {
            foreach (PropertyInfo field in typeof(T).GetProperties())
            {
                if (field.Name != "Id" && field.Name != "EmployeeId" && field.Name != "EmpoyeeId" && field.Name != "EmpCode" && field.Name != "EmployeeInitial" && field.Name != "FullName" && field.Name != "Message" && field.Name != "errClass" && field.Name != "ButtonText" && field.Name != "strMode" && field.Name != "DateofInactive" && field.Name != "IsContractual")
                {
                    string type = field.PropertyType.Name;
                    if (type == "Nullable`1")
                    {
                        field.SetValue(obj, null, null);
                    }
                    else if (type == "Int32")
                    {
                        field.SetValue(obj, 0, null);
                    }
                    else if (type == "String")
                    {
                        field.SetValue(obj, "", null);
                    }
                    else if (type == "Decimal")
                    {
                        field.SetValue(obj, Convert.ToDecimal(0), null);
                    }
                    else if (type == "DateTime")
                    {
                        field.SetValue(obj, DateTime.Now, null);
                    }
                    else if (type == "Boolean")
                    {
                        field.SetValue(obj, false, null);
                    }
                }
                else if (field.Name == "strMode")
                {
                    field.SetValue(obj, "add", null);
                }
                else
                {
                    //Need to Implement if any other case occurs
                }
            }
            return obj;
        }
    }

    public class fileUploader : HttpPostedFileBase
    {
        public fileUploader()
        {
        }
    }

    public class EmailProcessor
    {

        private string logFileLocation;
        public void SendEmail(string strFromAddress, string senderName, string toAddresses, string strCC, string subject, string messagebodyContent)
        {
            try
            {
                System.Web.Mail.MailMessage mail = new System.Web.Mail.MailMessage();

                mail.From = senderName + "<" + strFromAddress + ">";
                mail.To = toAddresses;
                //mail.Cc = strCC;
                mail.Subject = subject;
                mail.Body = messagebodyContent;
                mail.BodyFormat = System.Web.Mail.MailFormat.Html;
                //mail.
                mail.Priority = System.Web.Mail.MailPriority.High;

                System.Web.Mail.SmtpMail.SmtpServer = AppConstant.mailServer;

                System.Web.Mail.SmtpMail.Send(mail);
            }
            catch (Exception)
            { }
        }

        public bool SendEmail(string smtp, string FromEmailAddress, string FromEmailpassword, string ToEmailAddress, string CCEmailAddress, string BCCEmailAddress, string subject, string message, string attachmentFilename = "")
        {
            bool isSuccess = false;
            try
            {
                var loginInfo = new NetworkCredential(FromEmailAddress, FromEmailpassword);
                var msg = new MailMessage();

                var smtpClient = new SmtpClient(smtp);

                if (!string.IsNullOrEmpty(attachmentFilename))
                {
                    Attachment attachment = new Attachment(attachmentFilename, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = attachment.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(attachmentFilename);
                    disposition.ModificationDate = File.GetLastWriteTime(attachmentFilename);
                    disposition.ReadDate = File.GetLastAccessTime(attachmentFilename);
                    disposition.FileName = Path.GetFileName(attachmentFilename);
                    disposition.Size = new FileInfo(attachmentFilename).Length;
                    disposition.DispositionType = DispositionTypeNames.Attachment;
                    msg.Attachments.Add(attachment);
                }


                msg.From = new MailAddress(FromEmailAddress);
                msg.To.Add(new MailAddress(ToEmailAddress));
                

                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                smtpClient.Host = WebConfigurationManager.AppSettings["mailServer"];

                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;

                //smtpClient.Credentials = (ICredentialsByHost)CredentialCache.DefaultNetworkCredentials;

                try
                {
                    smtpClient.Send(msg);
                    isSuccess = true;
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    isSuccess = false;
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == SmtpStatusCode.MailboxBusy ||
                            status == SmtpStatusCode.MailboxUnavailable)
                        {
                            LogInfo("Delivery failed - retrying in 5 seconds.");
                            System.Threading.Thread.Sleep(5000);
                            smtpClient.Send(msg);
                        }
                        else
                        {
                            LogInfo("Failed to deliver message to " + ex.InnerExceptions[i].FailedRecipient);
                        }
                    }
                }
                return isSuccess;

            }
            catch (Exception ex)
            {
                LogInfo("Failed to deliver message. Please provide valid information for proper transaction. " + ex.Message);
                isSuccess = false;
                return isSuccess;
            }
        }

        public void LogInfo(string lines)
        {
            string message = string.Format("{0:yyyy-MMM-dd hh:mm:ss:tt} - ", DateTime.Now) + lines;
            logFileLocation = CreateFile();
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
    }

    public static class ResponseHelper
    {
        public static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
        {

            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;

                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);

                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedFileExtensionAttribute : ValidationAttribute
    {
        public string[] AllowedFileExtensions { get; private set; }
        public AllowedFileExtensionAttribute(params string[] allowedFileExtensions)
        {
            AllowedFileExtensions = allowedFileExtensions;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as HttpPostedFileBase;
            if (file != null)
            {
                if (!AllowedFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                {
                    return new ValidationResult(string.Format("{1} Allowed file extensions for : {0} : {2}", string.Join(", ", AllowedFileExtensions), validationContext.DisplayName, this.ErrorMessage));
                }
            }
            return null;
        }
    }

}
