using System;
using System.Collections.Generic;
using System.Linq;
using FirebaseAdmin.Messaging;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.NewsManagment;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.NewsManagment
{
    public class NewsManagementService : Service, INewsManagementService
    {
        private readonly FirebaseMessaging firebaseMessaging;
        private readonly IExtendedLogger logger;

        public NewsManagementService(IARADbContext db,
                                     FirebaseMessaging firebaseMessaging,
                                     IExtendedLogger logger)
            : base(db)
        {
            this.logger = logger;
            this.firebaseMessaging = firebaseMessaging;
        }

        public IQueryable<NewsManagementDTO> GetAll(NewsManagmentFilters filters)
        {
            IQueryable<NewsManagementDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllNews(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredNews(filters)
                    : GetFreeTextFilteredNews(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public NewsManagementEditDTO Get(int id)
        {
            DateTime now = DateTime.Now;

            NewsManagementEditDTO result = (from news in Db.News
                                            where news.Id == id
                                            select new NewsManagementEditDTO
                                            {
                                                Id = news.Id,
                                                Title = news.Title,
                                                Content = news.Contents,
                                                Summary = news.ContentSummary,
                                                PublishEnd = news.PublishEnd,
                                                PublishStart = news.PublishStart,
                                                HasNotificationsSent = news.HasNotificationsSent,
                                                IsDistrictLimited = news.IsDistrictLimited
                                            }).First();

            result.DistrictsIds = GetDistrictsIds(id);

            List<FileInfoDTO> files = Db.GetFiles(Db.NewsFiles, id);
            int newsMainPhotoTypeId = Db.NfileTypes.Where(x => x.Code == nameof(FileTypeEnum.NEWS_MAIN_PHOTO)).Single().Id;

            result.Files = files.Where(x => x.FileTypeId != newsMainPhotoTypeId).ToList();
            result.MainImage = files.Where(x => x.FileTypeId == newsMainPhotoTypeId).SingleOrDefault();

            return result;
        }

        public int Add(NewsManagementEditDTO news)
        {
            News entry = new News
            {
                Title = news.Title,
                Contents = news.Content,
                ContentSummary = news.Summary,
                PublishStart = news.PublishStart,
                PublishEnd = news.PublishEnd,
                HasNotificationsSent = false,
                IsDistrictLimited = news.IsDistrictLimited
            };

            Db.News.Add(entry);

            if (news.IsDistrictLimited && news.DistrictsIds != null)
            {
                AddDistricts(entry, news.DistrictsIds);
            }

            AddOrEditFiles(entry, news.MainImage, news.Files);
            Db.SaveChanges();

            return entry.Id;
        }

        public void Edit(NewsManagementEditDTO news)
        {
            DateTime now = DateTime.Now;

            News dbNews = (from n in Db.News
                                .AsSplitQuery()
                                .Include(x => x.NewsFiles)
                           where n.Id == news.Id.Value
                           select n).First();

            dbNews.Title = news.Title;
            dbNews.Contents = news.Content;
            dbNews.ContentSummary = news.Summary;
            dbNews.PublishStart = news.PublishStart;
            dbNews.PublishEnd = news.PublishEnd;
            dbNews.IsDistrictLimited = news.IsDistrictLimited;

            EditDistricts(dbNews.Id, news.DistrictsIds, dbNews.IsDistrictLimited);
            AddOrEditFiles(dbNews, news.MainImage, news.Files);

            Db.SaveChanges();
        }

        public void DeleteNews(int id)
        {
            DeleteRecordWithId(Db.News, id);
            Db.SaveChanges();
        }

        public void UndoDeletedNews(int id)
        {
            UndoDeleteRecordWithId(Db.News, id);
            Db.SaveChanges();
        }

        public string GetMainImage(int newsId)
        {
            string result = (from newsFile in Db.NewsFiles
                             join file in Db.Files on newsFile.FileId equals file.Id
                             join fType in Db.NfileTypes on newsFile.FileTypeId equals fType.Id
                             where file.IsActive
                                && newsFile.IsActive
                                && newsFile.RecordId == newsId
                                && fType.Code == nameof(FileTypeEnum.NEWS_MAIN_PHOTO)
                             select $"url(data:{file.MimeType};base64,{Convert.ToBase64String(file.Content)}").SingleOrDefault();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.News, id);
        }

        public void SendMobileNotifications()
        {
            DateTime now = DateTime.Now;

            using var transaction = Db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            try
            {
                List<News> unsentNews = (from news in Db.News
                                         where !news.HasNotificationsSent
                                            && news.PublishStart.HasValue && news.PublishStart.Value <= now
                                            && news.PublishEnd.HasValue && news.PublishEnd.Value > now
                                         select news).ToList();

                if (unsentNews.Count == 0)
                {
                    return;
                }

                foreach (News newsItem in unsentNews)
                {
                    newsItem.HasNotificationsSent = true;
                }

                Db.SaveChanges();

                //Потребители абонирани за нотификации за новини притежаващи firebase token
                var devices = (from user in Db.Users
                               join info in Db.UserInfos on user.Id equals info.UserId
                               join mobile in Db.UserMobileDevices on user.Id equals mobile.UserId
                               where user.ValidFrom <= now && user.ValidTo > now
                                   && mobile.IsActive
                                   && info.NewsSubscriptionType != nameof(NewsSubscriptionTypes.None)
                                   && mobile.FirebaseTokenKey != null
                               select new
                               {
                                   UserId = user.Id,
                                   Token = mobile.FirebaseTokenKey,
                                   NewsSubscriptionType = Enum.Parse<NewsSubscriptionTypes>(info.NewsSubscriptionType)
                               }).ToList();

                if (devices.Count == 0)
                {
                    transaction.Commit();
                    return;
                }

                var userIds = devices.Where(x => x.NewsSubscriptionType == NewsSubscriptionTypes.Districts).Select(x => x.UserId);
                var newsIds = unsentNews.Where(x => x.IsDistrictLimited).Select(x => x.Id);

                //Всички subscription-и за области за текущите новини и потребителите абонирани за нотификация за област.
                var districtNotifications = (from newsDistrict in Db.NewsDistricts
                                             join newsDistrictSubscription in Db.NewsDistrictUserSubscriptions on newsDistrict.DistrictId equals newsDistrictSubscription.DistrictId
                                             where newsDistrict.IsActive
                                             && newsDistrictSubscription.IsActive
                                             && newsIds.Contains(newsDistrict.NewsId)
                                             && userIds.Contains(newsDistrictSubscription.UserId)
                                             select new
                                             {
                                                 DistrictId = newsDistrict.Id,
                                                 NewsId = newsDistrict.NewsId,
                                                 UserId = newsDistrictSubscription.UserId
                                             }).ToList();

                List<Message> messages = new List<Message>();
                foreach (var news in unsentNews)
                {
                    foreach (var device in devices)
                    {
                        //Ако новината не се отнася към област се изпраща към всички абонирани за нотификации за новини(Subsciption type - ALL или Districts)
                        if (!news.IsDistrictLimited)
                        {
                            messages.Add(BuildNewsMessage(device.Token, news.Title, news.Id));
                        }
                        else
                        {
                            //Проверка дали потребителя е абониран за някоя от областите на новината
                            if (districtNotifications.Any(x => x.NewsId == news.Id && x.UserId == device.UserId))
                            {
                                messages.Add(BuildNewsMessage(device.Token, news.Title, news.Id));
                            }
                        }
                    }
                }

                firebaseMessaging.SendMobileNotifications(messages, out List<Exception> exceptions, out List<string> invalidTokens);

                if (invalidTokens != null && invalidTokens.Count > 0)
                {
                    foreach (var mobileDevice in Db.UserMobileDevices.Where(x => invalidTokens.Contains(x.FirebaseTokenKey)))
                    {
                        mobileDevice.FirebaseTokenKey = null;
                    }

                    Db.SaveChanges();
                }


                if (exceptions != null && exceptions.Count > 0)
                {
                    foreach (var exception in exceptions)
                    {
                        logger.LogException(exception);
                    }
                }

                Db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {

            }
        }

        private IQueryable<NewsManagementDTO> GetAllNews(bool showInactive)
        {
            DateTime now = DateTime.Now;

            IQueryable<NewsManagementDTO> result = from news in Db.News
                                                   where news.IsActive == !showInactive
                                                   orderby news.Id descending
                                                   select new NewsManagementDTO
                                                   {
                                                       Id = news.Id,
                                                       Title = news.Title,
                                                       PublishStart = news.PublishStart,
                                                       PublishEnd = news.PublishEnd,
                                                       CreatedBy = news.CreatedBy,
                                                       IsPublished = news.PublishStart.HasValue && news.PublishStart <= now,
                                                       HasNotificationsSent = news.HasNotificationsSent,
                                                       IsActive = news.IsActive
                                                   };

            return result;
        }

        private IQueryable<NewsManagementDTO> GetParametersFilteredNews(NewsManagmentFilters filters)
        {
            DateTime now = DateTime.Now;

            IQueryable<NewsManagementDTO> result = from news in Db.News
                                                   where news.IsActive == !filters.ShowInactiveRecords
                                                        && (string.IsNullOrEmpty(filters.Content)
                                                            || news.Contents.ToLower().Contains(filters.Content.ToLower()))
                                                   orderby news.Id descending
                                                   select new NewsManagementDTO
                                                   {
                                                       Id = news.Id,
                                                       Title = news.Title,
                                                       PublishStart = news.PublishStart,
                                                       PublishEnd = news.PublishEnd,
                                                       CreatedBy = news.CreatedBy,
                                                       IsPublished = news.PublishStart.HasValue && news.PublishStart <= now,
                                                       HasNotificationsSent = news.HasNotificationsSent,
                                                       IsActive = news.IsActive
                                                   };


            if (!string.IsNullOrEmpty(filters.Title))
            {
                result = result.Where(news => news.Title.ToLower().Contains(filters.Title.ToLower()));
            }

            if (filters.DateFrom.HasValue && filters.DateTo.HasValue)
            {
                result = result.Where(news => news.PublishStart.HasValue && news.PublishEnd.HasValue
                                                              && news.PublishEnd.Value.Date >= filters.DateFrom.Value.Date
                                                              && news.PublishStart.Value.Date <= filters.DateTo.Value.Date);
            }

            if (filters.IsPublished.HasValue)
            {
                result = result.Where(news => (news.PublishStart.HasValue && news.PublishStart.Value <= now) == filters.IsPublished.Value);
            }

            return result;
        }

        private IQueryable<NewsManagementDTO> GetFreeTextFilteredNews(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();
            DateTime? date = DateTimeUtils.TryParseDate(text);
            DateTime now = DateTime.Now;

            IQueryable<NewsManagementDTO> result = from news in Db.News
                                                   where news.IsActive == !showInactive &&
                                                         (news.Title.ToLower().Contains(text) ||
                                                          news.Contents.ToLower().Contains(text) ||
                                                         (date.HasValue && news.PublishStart.HasValue && (date.Value.Date == news.PublishStart.Value.Date)) ||
                                                         (date.HasValue && news.PublishEnd.HasValue && (date.Value.Date == news.PublishEnd.Value.Date)))
                                                   orderby news.Id descending
                                                   select new NewsManagementDTO
                                                   {
                                                       Id = news.Id,
                                                       Title = news.Title,
                                                       PublishStart = news.PublishStart,
                                                       PublishEnd = news.PublishEnd,
                                                       CreatedBy = news.CreatedBy,
                                                       IsPublished = news.PublishStart.HasValue && news.PublishStart <= now,
                                                       HasNotificationsSent = news.HasNotificationsSent,
                                                       IsActive = news.IsActive
                                                   };

            return result;
        }

        private List<int> GetDistrictsIds(int id)
        {
            List<int> result = (from news in Db.News
                                join district in Db.NewsDistricts on news.Id equals district.NewsId
                                where news.Id == id
                                    && district.IsActive
                                select district.DistrictId).ToList();

            return result;
        }

        private void AddDistricts(News news, List<int> districtIds)
        {
            foreach (int districtId in districtIds)
            {
                NewsDistrict entry = new NewsDistrict
                {
                    News = news,
                    DistrictId = districtId
                };

                Db.NewsDistricts.Add(entry);
            }
        }

        private void EditDistricts(int newsId, List<int> districtIds, bool isDistrictLimited)
        {
            List<NewsDistrict> dbDistricts = (from nd in Db.NewsDistricts
                                              where nd.NewsId == newsId
                                              select nd).ToList();

            if (!isDistrictLimited || districtIds == null)
            {
                foreach (NewsDistrict nd in dbDistricts)
                {
                    nd.IsActive = false;
                }

                return;
            }

            foreach (int districtId in districtIds)
            {
                NewsDistrict dbDistrict = dbDistricts.Where(x => x.DistrictId == districtId).SingleOrDefault();

                if (dbDistrict == null)
                {
                    NewsDistrict entry = new NewsDistrict
                    {
                        NewsId = newsId,
                        DistrictId = districtId
                    };

                    Db.NewsDistricts.Add(entry);
                }
                else
                {
                    dbDistrict.IsActive = true;
                }
            }
        }

        private void AddOrEditFiles(News news, FileInfoDTO image, List<FileInfoDTO> files)
        {
            AddOrEditNewsImage(news, image);

            if (files != null)
            {
                foreach (FileInfoDTO file in files)
                {
                    Db.AddOrEditFile(news, news.NewsFiles, file);
                }
            }
        }

        private void AddOrEditNewsImage(News news, FileInfoDTO image)
        {
            if (image != null)
            {
                // photo exists in this context
                if (image.Id.HasValue)
                {
                    NewsFile newsFile = GetNewsImage(news.NewsFiles, image.Id.Value);
                    if (newsFile != null)
                    {
                        newsFile.IsActive = !image.Deleted;
                        Db.AddOrEditFile(image, increaseReferenceCounter: false);
                    }
                    else
                    {
                        File updatedReferenceFile = Db.AddOrEditFile(image, increaseReferenceCounter: true);
                        newsFile = new NewsFile
                        {
                            Record = news,
                            File = updatedReferenceFile,
                            FileTypeId = GetNewsMainImageFileType()
                        };

                        Db.NewsFiles.Add(newsFile);
                    }
                }
                //file is new in this context
                else if (image.File != null)
                {
                    image.FileTypeId = GetNewsMainImageFileType();

                    File newFile = Db.AddOrEditFile(image, increaseReferenceCounter: true);

                    NewsFile oldImage = GetNewsImage(news.NewsFiles);
                    if (oldImage != null)
                    {
                        oldImage.IsActive = false;
                    }

                    NewsFile newsFile = GetNewsImage(news.NewsFiles, newFile.Id);
                    if (newsFile != null)
                    {
                        newsFile.IsActive = true;
                    }
                    else
                    {
                        newsFile = new NewsFile
                        {
                            Record = news,
                            File = newFile,
                            FileTypeId = image.FileTypeId
                        };

                        Db.NewsFiles.Add(newsFile);
                    }
                }
            }
        }

        private NewsFile GetNewsImage(ICollection<NewsFile> newsFiles, int? fileId = null)
        {
            IEnumerable<NewsFile> query = from newsFile in newsFiles
                                          join file in Db.Files on newsFile.FileId equals file.Id
                                          join fileType in Db.NfileTypes on newsFile.FileTypeId equals fileType.Id
                                          where file.IsActive
                                              && fileType.Code == nameof(FileTypeEnum.NEWS_MAIN_PHOTO)
                                          select newsFile;

            if (fileId.HasValue)
            {
                query = from newsFile in query
                        where newsFile.FileId == fileId.Value
                        select newsFile;
            }
            else
            {
                query = from newsFile in query
                        where newsFile.IsActive
                        select newsFile;
            }

            return query.SingleOrDefault();
        }

        private int GetNewsMainImageFileType()
        {
            int id = (from fType in Db.NfileTypes
                      where fType.Code == nameof(FileTypeEnum.NEWS_MAIN_PHOTO)
                      select fType.Id).Single();

            return id;
        }

        private static Message BuildNewsMessage(string token, string body, int newsId)
        {
            Message msg = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = "IARA",
                    Body = body
                },
                Data = new Dictionary<string, string>() { { "NewsId", newsId.ToString() } },
            };

            return msg;
        }
    }
}

