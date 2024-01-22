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
    public class EmailNotification : EmailSending
    {
        #region Properties

        #region Object Properties

        private readonly LogFilesWritting _logFilesWritting = new LogFilesWritting();
        private readonly CotractExpire _cotractExpire = new CotractExpire();
        private readonly Retirement _retirement = new Retirement();

        #endregion

        public string SqlConnectionString;
        public string LogFileLocation;
        public string Smpt;
        public string FromemailAddress;
        public string FromemailPassword;
        public string ToEmailAddress;

        #endregion

        #region EmailNotification Constructor

        public EmailNotification()
        {
            try
            {
                //LogFileLocation = _logFilesWritting.CreateFile();
                _logFilesWritting.CreateFile();

                var xmlpath = System.IO.Directory.GetCurrentDirectory() + "\\Configuration.xml";
                var doc = new System.Xml.XPath.XPathDocument(xmlpath);
                var navigator = doc.CreateNavigator();

                var serverName = navigator.SelectSingleNode("//appsettings/servername");
                var username = navigator.SelectSingleNode("//appsettings/username");
                var password = navigator.SelectSingleNode("//appsettings/password");
                var database = navigator.SelectSingleNode("//appsettings/database");

                SqlConnectionString = "Data Source=" + serverName.Value + ";Initial Catalog=" + database.InnerXml + ";User ID=" + username.InnerXml + ";Password=" + password.InnerXml + ";Connect Timeout=3600";

                var selectSingleNode = navigator.SelectSingleNode("//appsettings/SMTP");
                if (selectSingleNode != null)
                    Smpt = selectSingleNode.Value;

                var xPathNavigator = navigator.SelectSingleNode("//appsettings/FromEmail");
                if (xPathNavigator != null)
                    FromemailAddress = xPathNavigator.Value;

                var pathNavigator = navigator.SelectSingleNode("//appsettings/FromEmailPassword");
                if (pathNavigator != null)
                    FromemailPassword = pathNavigator.Value;
            }
            catch (Exception ex)
            {
                _logFilesWritting.LogInfo(ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// E-mail Notification Services
        /// </summary>
        public void NotifyEmailServices()
        {
            #region E-mail Notification for PRM
            
            try
            {
                if (_cotractExpire.EmailSendDateInMonth == System.DateTime.Now.Day)
                {
                    if (SendEmail(Smpt, FromemailAddress, FromemailPassword, _cotractExpire.ToEmailAddress, _cotractExpire.CCEmailAddress, _cotractExpire.BCCEmailAddress, _cotractExpire.Subject, _cotractExpire.EmailBody, ""))
                    {
                        _logFilesWritting.LogInfo("Successful- Contract Expire E-mail sent.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logFilesWritting.LogInfo(ex.Message);
            }

            #region Retirement
            try
            {
                if (_retirement.EmailSendDateInMonth == System.DateTime.Now.Day)
                {
                    if (SendEmail(Smpt, FromemailAddress, FromemailPassword, _retirement.ToEmailAddress, _retirement.CCEmailAddress, _retirement.BCCEmailAddress, _retirement.Subject, _retirement.EmailBody, ""))
                    {
                        _logFilesWritting.LogInfo("Successful- Retirement E-mail sent.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logFilesWritting.LogInfo(ex.Message);
            }

            #endregion
            try
            {
                var annualIncrement = new AnnualIncrementEmailNotification();
                if (annualIncrement.EmailSendDateInMonth == System.DateTime.Now.Day)
                {
                    if (SendEmail(Smpt, FromemailAddress, FromemailPassword, annualIncrement.ToEmailAddress, annualIncrement.CCEmailAddress, annualIncrement.BCCEmailAddress, annualIncrement.Subject, annualIncrement.EmailBody, ""))
                    {
                        _logFilesWritting.LogInfo("Successful- Annual Increment E-mail sent.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logFilesWritting.LogInfo(ex.Message);
            }

            try
            {
                var jobConfirmation = new JobConfirmationEmailNotification();
                if (jobConfirmation.EmailSendDateInMonth == System.DateTime.Now.Day)
                {
                    if (SendEmail(Smpt, FromemailAddress, FromemailPassword, jobConfirmation.ToEmailAddress, jobConfirmation.CCEmailAddress, jobConfirmation.BCCEmailAddress, jobConfirmation.Subject, jobConfirmation.EmailBody, ""))
                    {
                        _logFilesWritting.LogInfo("Successful- Job Confirmation E-mail sent.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logFilesWritting.LogInfo(ex.Message);
            }
            #endregion

            #region E-mail Notification for PIM

            var obj = new TimeSheetEmailNotificationList();
            if (obj.EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                if (SendEmail(Smpt, FromemailAddress, FromemailPassword, obj.ToEmailAddress, obj.CCEmailAddress, obj.BCCEmailAddress, obj.Subject, obj.EmailBody, ""))
                {
                    _logFilesWritting.LogInfo("Successful- Time Sheet E-mail sent.");
                }
            }

            var objTimeSheetIndividualMailSeningMaster = new TimeSheetIndividualMailSeningMaster();
            if (objTimeSheetIndividualMailSeningMaster.EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                objTimeSheetIndividualMailSeningMaster.MailSendingIndividual();
            }

            var objMilestoneDeliveryMaster = new MilestoneDeliveryEmailNotificationMaster();
            if (objMilestoneDeliveryMaster.EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                objMilestoneDeliveryMaster.MilestoneMailSending();
            }

            var objEquipmentDeliveryMaster = new EquipmentDeliveryMailNotificationMaster();
            if (objEquipmentDeliveryMaster.EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                objEquipmentDeliveryMaster.EquipmentDeliveryScheduleMailSending();
            }

            var objInvoiceMadeMailMaster = new InvoiceMadeMailNotificationMaster();
            if (objInvoiceMadeMailMaster.EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                objInvoiceMadeMailMaster.InvoiceMadeScheduleMailSending();
            }

            var objInvoiceRealizationMailMaster = new InvoiceRealizationMailNotificationMaster();
            if (objInvoiceRealizationMailMaster.EmailSendDateInMonth == System.DateTime.Now.Day)
            {
                objInvoiceRealizationMailMaster.InvoiceRealizationScheduleMailSending();
            }


            #endregion

            #region E-mail Notification for ADC

            try
            {
                var _objAssetReturn = new AssetReturnEmailNotificationToBeneficiary();

                if (_objAssetReturn.IsRecordFound)
                {
                    if (SendEmail(Smpt, FromemailAddress, FromemailPassword, _objAssetReturn.ToEmailAddress, _objAssetReturn.CCEmailAddress, _objAssetReturn.BCCEmailAddress, _objAssetReturn.Subject, _objAssetReturn.EmailBody, ""))
                    {
                        _logFilesWritting.LogInfo("Successful- Asset not return notify E-mail sent.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logFilesWritting.LogInfo(ex.Message);
            }

            #endregion
        }
    }
}
