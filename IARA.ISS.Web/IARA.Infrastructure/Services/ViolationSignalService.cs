using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.ConfigModels;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ReportViolations;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using TL.Email.ExtendedClient;

namespace IARA.Infrastructure.Services
{
    public class ViolationSignalService : BaseService, IViolationSignalService
    {
        private const string REPORT_VIOLATION_MAIL = nameof(REPORT_VIOLATION_MAIL);
        private const string GOOGLE_URL = "https://www.google.com/maps/search/?api=1&query={0},{1}";

        private readonly IEmailWorkerQueue emailWorkerQueue;
        private readonly EmailClientSettings emailSettings;

        public ViolationSignalService(IARADbContext dbContext, EmailClientSettings emailSettings, IEmailWorkerQueue emailWorkerQueue)
            : base(dbContext)
        {
            this.emailSettings = emailSettings;
            this.emailWorkerQueue = emailWorkerQueue;
        }

        public Task<bool> EnqueueSignalForViolationEmail(ReportViolationDTO violation, int userId)
        {
            NnotificationTemplate template = GetViolationTemplate();

            var user = GetUserInfo(userId);

            Dictionary<NotificationTemplateCodes, string> templateCodes = new Dictionary<NotificationTemplateCodes, string>
            {
                { NotificationTemplateCodes.USERNAME, user.Username },
                { NotificationTemplateCodes.FIRST_NAME, user.FirstName },
                { NotificationTemplateCodes.LAST_NAME, user.LastName },
                { NotificationTemplateCodes.URL, GetLocationUrl(violation) },
                { NotificationTemplateCodes.TYPE, violation.SignalType.DisplayName },
                { NotificationTemplateCodes.PHONE, violation.Phone },
                { NotificationTemplateCodes.DATETIME, violation.Date.Value.ToString(DateTimeFormats.DISPLAY_DATETIME_FORMAT) },
                { NotificationTemplateCodes.DESCRIPTION, violation.Description }
            };

            string formattedBody = NotificationTemplateUtils.BuildNotification(template.Body, templateCodes);

            return this.emailWorkerQueue.Enqueue(emailSettings.ReportViolationEmail, template.Subject, formattedBody, from: this.emailSettings.SenderMail);
        }

        private string GetLocationUrl(ReportViolationDTO violation)
        {
            if (violation != null && violation.Location != null)
            {
                var location = violation.Location;
                return string.Format(GOOGLE_URL, location.Latitude, location.Longitude);
            }
            else
            {
                return ErrorResources.msgLocationNotSent;
            }
        }

        private NnotificationTemplate GetViolationTemplate()
        {
            NnotificationTemplate template = Db.NnotificationTemplates
                                              .Where(x => x.Code == REPORT_VIOLATION_MAIL
                                                  && x.ValidFrom <= DateTime.Now
                                                  && x.ValidTo >= DateTime.Now)
                                              .First();
            return template;
        }

        private (string Username, string FirstName, string LastName) GetUserInfo(int userId)
        {
            var user = (from usr in Db.Users
                        join person in Db.Persons on usr.PersonId equals person.Id
                        where usr.Id == userId
                        select new
                        {
                            Username = usr.Username,
                            FirstName = person.FirstName,
                            LastName = person.LastName,
                        }).First();

            return (user.Username, user.FirstName, user.LastName);
        }
    }
}
