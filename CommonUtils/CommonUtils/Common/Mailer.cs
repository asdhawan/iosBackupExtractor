using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Mail;
using EMailMessage = System.Net.Mail.MailMessage;
using System.Threading.Tasks;
using System.Configuration;

namespace CommonUtils
{
    public static class Mailer
    {
        private static readonly string noReplyFromAddress = ConfigurationManager.AppSettings["NoReplyFromAddress"];
        public static void SendEmail(string emailTo, List<string> emailCc, string emailSubject, string emailBody, bool bAsync = false)
        { SendEmail(null, noReplyFromAddress, emailTo, emailCc, emailSubject, emailBody, new List<string>(), bAsync); }

        public static void SendEmail(string smtpServerAddress, string emailTo, List<string> emailCc, string emailSubject, string emailBody, bool bAsync = false)
        { SendEmail(smtpServerAddress, noReplyFromAddress, emailTo, emailCc, emailSubject, emailBody, new List<string>(), bAsync); }

        public static void SendEmail(string smtpServerAddress, string emailFrom, string emailTo, List<string> emailCc, string emailSubject, string emailBody, bool bAsync = false)
        { SendEmail(smtpServerAddress, emailFrom, emailTo, emailCc, emailSubject, emailBody, new List<string>(), bAsync); }

        public static void SendEmail(string smtpServerAddress, string emailTo, List<string> emailCc, string emailSubject, string emailBody, List<string> attachmentFilePaths, bool bAsync = false)
        { SendEmail(smtpServerAddress, noReplyFromAddress, emailTo, emailCc, emailSubject, emailBody, attachmentFilePaths, bAsync); }

        public static void SendEmail(string smtpServerAddress, string emailTo, List<string> emailCc, string emailSubject, string emailBody, List<Attachment> attachments, bool bAsync = false)
        { SendEmail(smtpServerAddress, noReplyFromAddress, emailTo, emailCc, emailSubject, emailBody, attachments, bAsync); }

        public static void SendEmail(string smtpServerAddress, string emailFrom, string emailTo, List<string> emailCc, string emailSubject, string emailBody, List<string> attachmentFilePaths, bool bAsync = false)
        {
            List<Attachment> attachments = new List<Attachment>();
            foreach (string attachmentFilePath in attachmentFilePaths)
            {
                if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                    attachments.Add(new Attachment(attachmentFilePath));
            }
            SendEmail(smtpServerAddress, emailFrom, emailTo, emailCc, emailSubject, emailBody, attachments, bAsync);
        }

        public static void SendEmail(string smtpServerAddress, string emailFrom, string emailTo, List<string> emailCc, string emailSubject, string emailBody, List<Attachment> attachments, bool bAsync = false)
        { SendEmail(smtpServerAddress, emailFrom, emailTo, emailCc, new List<string>(), emailSubject, emailBody, attachments, bAsync); }

        public static void SendEmail(string smtpServerAddress, string emailFrom, string emailTo, List<string> emailCc, List<string> emailBcc, string emailSubject, string emailBody, List<Attachment> attachments, bool bAsync = false)
        {
            try
            {
                EMailMessage mm = new EMailMessage(emailFrom, emailTo);
                if (emailCc != null)
                {
                    foreach (string ccEmail in emailCc)
                    {
                        if (!string.IsNullOrEmpty(ccEmail))
                            mm.CC.Add(ccEmail);
                    }
                }
                if (emailBcc != null)
                {
                    foreach (string bccEmail in emailBcc)
                    {
                        if (!string.IsNullOrEmpty(bccEmail))
                            mm.Bcc.Add(bccEmail);
                    }
                }
                mm.Subject = emailSubject;
                mm.Body = emailBody;
                mm.IsBodyHtml = true;

                if (attachments != null)
                {
                    foreach (Attachment a in attachments)
                    {
                        mm.Attachments.Add(a);
                    }
                }

                SmtpClient sm = string.IsNullOrEmpty(smtpServerAddress) ? new SmtpClient() : new SmtpClient(smtpServerAddress);
                sm.Timeout = 300000;
                if (bAsync)
                    Task.Factory.StartNew(() => SendAndLogEmail(sm, mm));
                else
                    SendAndLogEmail(sm, mm);
            }
            catch (Exception ex) { Logger.LogError("Mailer.SendEmail()", "Error preparing E-Mail", ex); }
        }

        private static void SendAndLogEmail(SmtpClient smtpClient, EMailMessage emailMessage)
        {
            try
            {
                smtpClient.Send(emailMessage);
                emailMessage.Dispose();
            }
            catch (Exception ex) { Logger.LogError("Mailer.SendEmail()", "Error sending E-Mail", ex); }
        }

    }
}
