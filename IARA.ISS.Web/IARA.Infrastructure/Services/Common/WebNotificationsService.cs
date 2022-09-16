using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels;
using IARA.DomainModels.DTOModels.Common;
using IARA.EntityModels.Entities;
using IARA.Interfaces.Common;
using IARA.Notifications.Enums;
using IARA.Notifications.Interfaces;
using IARA.Notifications.Models;

namespace IARA.Infrastructure.Services.Common
{
    public class WebNotificationsService : BaseService, IWebNotificationsService
    {
        private INotificationsNotifier notifier;
        public WebNotificationsService(INotificationsNotifier notifier, IARADbContext dbContext)
            : base(dbContext)
        {
            this.notifier = notifier;
        }

        public Task<bool> NotifyUser<T>(int userId, string templateCode, T model, string notificationUrl = null)
            where T : class
        {
            var notif = BuildNotification(templateCode, model);

            var notificationLog = Db.NotificationsLogs.Add(new NotificationsLog
            {
                UserId = userId,
                SentDateTime = DateTime.Now,
                NotificationId = notif.TemplateId,
            }).Entity;

            Db.SaveChanges();

            var notification = new Notification<NotificationDTO>(NotificationTypes.User)
            {
                Message = new NotificationDTO
                {
                    Id = notificationLog.Id,
                    RecievedDate = DateTime.Now,
                    Text = notif.Body,
                    Subtitle = notif.Header,
                    IsRead = false,
                    Url = notificationUrl
                },
            };

            string jsonMessage = CommonUtils.Serialize(notification.Message);
            notificationLog.Message = jsonMessage;

            Db.SaveChanges();

            return notifier.AddClientNotification(userId.ToString(), notification);
        }

        public NotificationsDTO GetUserNotifications(int userId, int page, int pageSize)
        {
            int totalCount = Db.NotificationsLogs
                               .Where(x => x.UserId == userId)
                               .Count();

            int totalUnread = Db.NotificationsLogs
                                .Where(x => x.UserId == userId
                                    && x.ReceveDateTime == null)
                                .Count();

            var notifications = (from notif in Db.NotificationsLogs
                                 where notif.UserId == userId
                                 orderby notif.SentDateTime descending
                                 select new
                                 {
                                     notif.Message,
                                     notif.ReceveDateTime
                                 })
                                 .Skip(page * pageSize)
                                 .Take(pageSize);


            var notificationsModel = new NotificationsDTO
            {
                Records = new List<NotificationDTO>(pageSize),
                TotalUnread = totalUnread,
                TotalRecordsCount = totalCount
            };

            foreach (var notif in notifications)
            {
                var notification = CommonUtils.Deserialize<NotificationDTO>(notif.Message);
                notification.IsRead = notif.ReceveDateTime != null && notif.ReceveDateTime.HasValue;
                notificationsModel.Records.Add(notification);
            }

            return notificationsModel;
        }

        public bool MarkNotificationAsRead(int userId, int notificationId)
        {
            var notification = Db.NotificationsLogs.Where(x => x.Id == notificationId && x.UserId == userId).FirstOrDefault();
            if (notification != null)
            {
                notification.ReceveDateTime = DateTime.Now;
                Db.SaveChanges();
                return true;
            }

            return false;
        }

        public void MarkNotificationsAsRead(int userId, IEnumerable<int> notificationIDs)
        {
            var ids = notificationIDs.ToHashSet();

            var notifications = Db.NotificationsLogs.Where(x => x.UserId == userId && notificationIDs.Contains(x.Id));

            var now = DateTime.Now;

            foreach (var notification in notifications)
            {
                notification.ReceveDateTime = now;
            }

            Db.SaveChanges();
        }

        public Task<bool> NotifyAllConnectedUsers<T>(string body, string header = null, string notificationUrl = null)
        {
            var notification = new Notification<NotificationDTO>(NotificationTypes.User)
            {
                Message = new NotificationDTO
                {
                    Id = 0,
                    RecievedDate = DateTime.Now,
                    Text = body,
                    Subtitle = header,
                    IsRead = false,
                    Url = notificationUrl
                },
            };

            return notifier.AddBroadcastNotification(notification);
        }

        public Task<bool> NotifyAllConnectedUsers<T>(string templateCode, T model, string notificationUrl = null)
        {
            var notif = BuildNotification(templateCode, model);

            var notification = new Notification<NotificationDTO>(NotificationTypes.User)
            {
                Message = new NotificationDTO
                {
                    Id = 0,
                    RecievedDate = DateTime.Now,
                    Text = notif.Body,
                    Subtitle = notif.Header,
                    IsRead = false,
                    Url = notificationUrl
                },
            };

            return notifier.AddBroadcastNotification(notification);
        }

        private (int TemplateId, string Header, string Body) BuildNotification<T>(string templateCode, T model)
        {
            var dbTemplate = Db.NnotificationTemplates
                .Where(x => x.Code == templateCode
                         && x.ValidFrom <= DateTime.Now
                         && x.ValidTo >= DateTime.Now)
                .Select(x => new
                {
                    x.Id,
                    x.Body,
                    x.Subject
                })
                .Single();

            string message = NotificationTemplateUtils.BuildNotification<T>(dbTemplate.Body, model);
            string header = NotificationTemplateUtils.BuildNotification<T>(dbTemplate.Subject, model);

            return (dbTemplate.Id, header, message);
        }
    }
}
