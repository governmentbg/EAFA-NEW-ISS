using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess.Abstractions;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Security.Enums;
using TL.BatchWorkers;
using TL.BatchWorkers.EventArguments;
using TL.BatchWorkers.Interfaces;
using TL.BatchWorkers.Models.Parameters.AsyncWorker;

namespace IARA.Security
{
    public class SecurityEmailSender : ISecurityEmailSender
    {
        private readonly IAsyncWorkerTaskQueue<EmailModel, bool> taskQueue;
        private readonly ScopedServiceProviderFactory scopedServiceProviderFactory;
        private readonly EmailClientSettings emailSettings;

        private readonly Func<string, string, string> formatEmailErrorMessage = (toEmail, subject) => $"Потребител с мейл: {toEmail} не можа да получи мейл. Тема: {subject}";


        public SecurityEmailSender(ScopedServiceProviderFactory scopedServiceProviderFactory, EmailClientSettings emailSettings)
        {
            this.emailSettings = emailSettings;
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;

            this.taskQueue = AsyncWorkerQueueBuilder.CreateInMemoryWorker(new LocalAsyncWorkerSettings<EmailModel, bool>(this.SendEmail));
            this.taskQueue.TaskErrorOccured += this.TaskQueue_TaskErrorOccured;
        }

        private void TaskQueue_TaskErrorOccured(object sender, TaskErrorEventArgs<EmailModel> e)
        {
            if (e.Exception != null)
            {
                using IScopedServiceProvider scopedServiceProvider = this.scopedServiceProviderFactory.GetServiceProvider();
                IExtendedLogger logger = scopedServiceProvider.GetRequiredService<IExtendedLogger>();
                logger.LogException(this.formatEmailErrorMessage(e.Task.InvocationData.ToEmail, e.Task.InvocationData.Subject), e.Exception);
            }
        }

        public string EnqueuePasswordEmail(string email, SecurityEmailTypes emailType, string baseAddress = null)
        {
            if (string.IsNullOrEmpty(baseAddress))
            {
                baseAddress = this.emailSettings.EmailConfirmationUrl;
            }

            string unencodedToken = CommonUtils.GenerateUserToken();
            string urlEncodedToken = HttpUtility.UrlEncode(unencodedToken);

            using IScopedServiceProvider scopedServiceProvider = this.scopedServiceProviderFactory.GetServiceProvider();
            using IUsersDbContext dbContext = scopedServiceProvider.GetService<IUsersDbContext>();
            User user = dbContext.Users.Where(x => x.Email == email).Single();
            Person person = dbContext.Persons.Where(x => x.Id == user.PersonId).FirstOrDefault();
            UserInfo userInfo = dbContext.UserInfos.Where(x => x.UserId == user.Id).Single();
            userInfo.ConfirmEmailKey = unencodedToken;
            userInfo.EmailKeyValidTo = DateTime.Now.AddDays(this.emailSettings.EmailTokenDaysValid);
            userInfo.IsEmailConfirmed = false;

            NnotificationTemplate template = dbContext.NnotificationTemplates.Where(x => x.Code == emailType.ToString()
                                                                    && x.ValidFrom <= DateTime.Now
                                                                    && x.ValidTo >= DateTime.Now).First();

            Dictionary<NotificationTemplateCodes, string> templateCodes = new Dictionary<NotificationTemplateCodes, string>
            {
                { NotificationTemplateCodes.URL, this.BuildUrl(baseAddress, urlEncodedToken, emailType) },
                { NotificationTemplateCodes.USERNAME, user.Username },
                { NotificationTemplateCodes.FIRST_NAME, person?.FirstName },
                { NotificationTemplateCodes.LAST_NAME, person?.LastName }
            };

            string formattedBody = NotificationTemplateUtils.BuildNotification(template.Body, templateCodes);

            EmailModel emailTaskParams = new EmailModel
            {
                Body = formattedBody,
                Subject = template.Subject,
                FromEmail = this.emailSettings.SenderMail,
                ToEmail = email
            };

            this.taskQueue.Enqueue(emailTaskParams);

            dbContext.SaveChanges();

            return unencodedToken;
        }

        private string BuildUrl(string baseAddress, string token, SecurityEmailTypes emailType)
        {
            baseAddress = baseAddress.TrimEnd('/');
            switch (emailType)
            {
                case SecurityEmailTypes.NEW_PUBLIC_USER_MAIL:
                    baseAddress += "/successful-email-confirmation";
                    break;
                case SecurityEmailTypes.ADMIN_RESET_PASS_MAIL:
                case SecurityEmailTypes.NEW_INTERNAL_USER_MAIL:
                case SecurityEmailTypes.FORGOTTEN_PASSWORD_MAIL:
                    baseAddress += "/change-password";
                    break;
                default:
                    break;
            }

            return $"{baseAddress}?token={token}";
        }

        private async Task<bool> SendEmail(EmailModel email, CancellationToken token)
        {
            using IScopedServiceProvider scopedServiceProvider = this.scopedServiceProviderFactory.GetServiceProvider();
            using IEmailClient emailClient = scopedServiceProvider.GetService<IEmailClient>();

            Task task = emailClient.SendEmailAsync(email.ToEmail, email.FromEmail, email.Subject, email.Body);
            await task;

            if (!task.IsFaulted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
