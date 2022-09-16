using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using IARA.Logging.Abstractions.Interfaces;

namespace IARA.Common.Utils
{
    public class EmailClient : IDisposable, IEmailClient
    {
        private SmtpClient mailSender;
        private bool disposedValue;
        private IExtendedLogger logger;

        public EmailClient(string host, int? port = null, IExtendedLogger logger = null)
        {
            this.logger = logger;

            mailSender = port.HasValue ? new SmtpClient(host, port.Value) : new SmtpClient(host);

            mailSender.DeliveryFormat = SmtpDeliveryFormat.International;
            mailSender.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailSender.SendCompleted += this.MailSender_SendCompleted;
        }

        public Task SendEmailAsync(string mailTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true)
        {
            return SendEmailsAsync(new List<string> { mailTo }, mailFrom, mailSubject, mailBody, isBodyHtml);
        }

        public Task SendEmailsAsync(IEnumerable<string> mailsTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true)
        {
            MailMessage messageToSend = new MailMessage
            {
                From = new MailAddress(mailFrom)
            };

            foreach (string mailTo in mailsTo.Where(e => !string.IsNullOrEmpty(e)))
            {
                messageToSend.To.Add(mailTo);
            }

            messageToSend.IsBodyHtml = isBodyHtml;
            messageToSend.Subject = mailSubject;
            messageToSend.Body = mailBody;

            return mailSender.SendMailAsync(messageToSend);
        }

        private void MailSender_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                logger?.LogException(e.Error);
            }
            else if (e.Cancelled)
            {
                logger?.LogWarning("Email sending cancelled");
            }
        }

        public void SendEmail(string mailTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true)
        {
            SendEmails(new List<string> { mailTo }, mailFrom, mailSubject, mailBody, isBodyHtml);
        }

        public void SendEmails(IEnumerable<string> mailsTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true)
        {
            MailMessage messageToSend = new MailMessage
            {
                From = new MailAddress(mailFrom)
            };

            foreach (string mailTo in mailsTo.Where(e => !string.IsNullOrEmpty(e)))
            {
                messageToSend.To.Add(mailTo);
            }

            messageToSend.IsBodyHtml = isBodyHtml;
            messageToSend.Subject = mailSubject;
            messageToSend.Body = mailBody;

            mailSender.Send(messageToSend);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mailSender?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
