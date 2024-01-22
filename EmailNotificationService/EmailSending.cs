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
    public class EmailSending : LogFilesWritting
    {
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

                string[] ccEmailAddresses = CCEmailAddress.Split(',');
                if (ccEmailAddresses.Length > 0)
                {
                    foreach (string itemCC in ccEmailAddresses)
                    {
                        msg.CC.Add(new MailAddress(itemCC.Trim()));
                    }
                }

                string[] bccEmailAddresses = BCCEmailAddress.Split(',');
                if (bccEmailAddresses.Length > 0)
                {
                    foreach (string itemBCC in bccEmailAddresses)
                    {
                        msg.Bcc.Add(new MailAddress(itemBCC.Trim()));
                    }
                }

                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;

                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;

                smtpClient.Credentials = (ICredentialsByHost)CredentialCache.DefaultNetworkCredentials;

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
    }
}
