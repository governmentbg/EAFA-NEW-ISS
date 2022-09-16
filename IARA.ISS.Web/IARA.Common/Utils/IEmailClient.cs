using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IARA.Common.Utils
{
    public interface IEmailClient : IDisposable
    {
        void SendEmail(string mailTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true);
        Task SendEmailAsync(string mailTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true);
        void SendEmails(IEnumerable<string> mailsTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true);
        Task SendEmailsAsync(IEnumerable<string> mailsTo, string mailFrom, string mailSubject, string mailBody, bool isBodyHtml = true);
    }
}
