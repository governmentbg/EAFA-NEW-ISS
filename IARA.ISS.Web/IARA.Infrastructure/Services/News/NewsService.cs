using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.NewsPublic;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class NewsService : Service, INewsService
    {
        public NewsService(IARADbContext db)
            : base(db)
        {

        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        public NewsDetailsDTO GetPublishedNews(int id)
        {
            DateTime now = DateTime.Now;

            NewsDetailsDTO wantedNews = (from news in Db.News
                                         where news.Id == id
                                           && news.PublishStart.HasValue
                                           && news.PublishEnd.HasValue
                                           && news.PublishEnd.Value >= now
                                           && news.PublishStart.Value <= now
                                           && news.IsActive
                                         select new NewsDetailsDTO
                                         {
                                             Id = news.Id,
                                             Title = news.Title,
                                             Content = news.Contents,
                                             PublishStart = news.PublishStart,
                                         }).SingleOrDefault();

            if (wantedNews != null)
            {
                int newsMainPhotoTypeId = GetNewsMainPhotoTypeId();
                wantedNews.Image = (from newsFile in Db.NewsFiles
                                    join file in Db.Files on newsFile.FileId equals file.Id
                                    where newsFile.IsActive
                                          && file.IsActive
                                          && newsFile.FileTypeId == newsMainPhotoTypeId
                                          && newsFile.RecordId == id
                                    select $"data:{file.MimeType};base64,{Convert.ToBase64String(file.Content)}").FirstOrDefault();


                wantedNews.Files = GetNewsFiles(wantedNews.Id);
            }

            return wantedNews;
        }

        public List<FileInfoDTO> GetNewsFiles(int id)
        {
            List<FileInfoDTO> files = Db.GetFiles(Db.NewsFiles, id);
            int newsMainPhotoTypeId = GetNewsMainPhotoTypeId();

            return files.Where(file => file.FileTypeId != newsMainPhotoTypeId).ToList();
        }

        public List<NewsImageDTO> GetNewsImages(int[] newsIds)
        {
            int newsMainPhotoTypeId = GetNewsMainPhotoTypeId();
            List<NewsImageDTO> listOfMainImages = (from newsFile in Db.NewsFiles
                                                   join file in Db.Files on newsFile.FileId equals file.Id
                                                   where newsFile.IsActive
                                                         && file.IsActive
                                                         && newsFile.FileTypeId == newsMainPhotoTypeId
                                                         && newsIds.Contains(newsFile.RecordId)
                                                   select new NewsImageDTO
                                                   {
                                                       NewsId = newsFile.RecordId,
                                                       Image = $"data:{file.MimeType};base64,{Convert.ToBase64String(file.Content)}"
                                                   }).ToList();

            return listOfMainImages;
        }

        public IQueryable<NewsDTO> GetAll(NewsFilters filters)
        {
            IQueryable<NewsDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllNews();
            }
            else if (filters.HasAnyFilters())
            {
                result = GetParametersFilteredNews(filters);
            }
            else
            {
                result = GetFreeTextFilteredNews(filters.FreeTextSearch);
            }

            return result;
        }

        public IQueryable<NewsMobileDTO> GetAllMobileNews(NewsFilters filters)
        {
            DateTime now = DateTime.Now;
            IQueryable<NewsMobileDTO> newsListQuery = null;

            if (filters != null && filters.HasFreeTextSearch())
            {
                string text = filters.FreeTextSearch.ToLowerInvariant();
                newsListQuery = (from news in Db.News
                                 where news.PublishStart.HasValue
                                 && news.PublishEnd.HasValue
                                 && news.PublishEnd.Value >= now
                                 && news.PublishStart.Value <= now
                                 && news.IsActive
                                 && (news.Title.ToLower().Contains(text)
                                    || news.Contents.ToLower().Contains(text)
                                    || news.ContentSummary.ToLower().Contains(text))
                                 orderby news.PublishStart descending
                                 select new NewsMobileDTO
                                 {
                                     Id = news.Id,
                                     Title = news.Title,
                                     Summary = news.ContentSummary,
                                     PublishStart = news.PublishStart,
                                     PublishEnd = news.PublishEnd
                                 });
            }
            else
            {
                newsListQuery = (from news in Db.News
                                 where news.PublishStart.HasValue
                                 && news.PublishEnd.HasValue
                                 && news.PublishEnd.Value >= now
                                 && news.PublishStart.Value <= now
                                 && news.IsActive
                                 orderby news.PublishStart descending
                                 select new NewsMobileDTO
                                 {
                                     Id = news.Id,
                                     Title = news.Title,
                                     Summary = news.ContentSummary,
                                     PublishStart = news.PublishStart,
                                     PublishEnd = news.PublishEnd
                                 });
            }


            if (filters != null && filters.HasAnyFilters(false))
            {
                if (filters.DateFrom.HasValue)
                {
                    newsListQuery = newsListQuery.Where(news => news.PublishStart >= filters.DateFrom);
                }

                if (filters.DateTo.HasValue)
                {
                    filters.DateTo = filters.DateTo.Value.AddDays(1).AddSeconds(-1);

                    newsListQuery = newsListQuery.Where(news => news.PublishStart <= filters.DateTo);
                }

                if (filters.DistrictsIds != null && filters.DistrictsIds.Length > 0)
                {
                    HashSet<int> newsIds = (from newsDistrict in Db.NewsDistricts
                                            join news in Db.News on newsDistrict.NewsId equals news.Id
                                            where news.PublishStart.HasValue
                                                  && news.PublishEnd.HasValue
                                                  && news.PublishEnd.Value >= now
                                                  && news.PublishStart.Value <= now
                                                  && news.IsActive
                                                  && filters.DistrictsIds.Contains(newsDistrict.DistrictId)
                                                  && newsDistrict.IsActive
                                            select newsDistrict.NewsId).ToHashSet();

                    newsListQuery = from news in newsListQuery
                                    where newsIds.Contains(news.Id)
                                    select news;
                }
            }

            return newsListQuery;
        }

        public List<NewsMobileDTO> SetNewsHasImageFlag(List<NewsMobileDTO> newsList)
        {
            int[] newsIds = newsList.Select(x => x.Id).ToArray();

            if (newsIds.Length > 0)
            {
                int newsMainPhotoTypeId = GetNewsMainPhotoTypeId();
                List<int> newsWithMainPhoto = (from newsFile in Db.NewsFiles
                                               join file in Db.Files on newsFile.FileId equals file.Id
                                               where newsFile.IsActive
                                                     && file.IsActive
                                                     && newsFile.FileTypeId == newsMainPhotoTypeId
                                                     && newsIds.Contains(newsFile.RecordId)
                                               select newsFile.RecordId).ToList();

                foreach (var news in newsList)
                {
                    if (newsWithMainPhoto.Contains(news.Id))
                        news.HasImage = true;
                }
            }

            return newsList;
        }


        public NewsDetailsMobileDTO GetNewsDetails(int newsId)
        {
            DateTime now = DateTime.Now;
            NewsDetailsMobileDTO newsDetail = (from news in Db.News
                                               where news.Id == newsId
                                               && news.PublishStart.HasValue
                                               && news.PublishEnd.HasValue
                                               && news.PublishEnd.Value >= now
                                               && news.PublishStart.Value <= now
                                               && news.IsActive
                                               orderby news.PublishStart descending
                                               select new NewsDetailsMobileDTO
                                               {
                                                   Id = news.Id,
                                                   Title = news.Title,
                                                   Content = news.Contents,
                                                   PublishStart = news.PublishStart,
                                               }).FirstOrDefault();

            if (newsDetail != null)
            {
                int newsMainPhotoTypeId = GetNewsMainPhotoTypeId();
                newsDetail.HasImage = (from newsFile in Db.NewsFiles
                                       join file in Db.Files on newsFile.FileId equals file.Id
                                       where newsFile.IsActive
                                             && file.IsActive
                                             && newsFile.FileTypeId == newsMainPhotoTypeId
                                             && newsFile.RecordId == newsId
                                       select newsFile.RecordId).Any();
            }

            return newsDetail;
        }

        public DownloadableFileDTO GetNewsMainPhoto(int newsId)
        {
            int newsMainPhotoTypeId = GetNewsMainPhotoTypeId();
            DownloadableFileDTO newsWithMainPhoto = (from newsFile in Db.NewsFiles
                                                     join file in Db.Files on newsFile.FileId equals file.Id
                                                     where newsFile.IsActive
                                                           && file.IsActive
                                                           && newsFile.FileTypeId == newsMainPhotoTypeId
                                                           && newsFile.RecordId == newsId
                                                     select new DownloadableFileDTO
                                                     {
                                                         MimeType = file.MimeType,
                                                         Bytes = file.Content,
                                                         FileName = file.Name
                                                     }).SingleOrDefault();

            return newsWithMainPhoto;
        }

        private IQueryable<NewsDTO> GetAllNews()
        {
            DateTime now = DateTime.Now;
            IQueryable<NewsDTO> arrayOfNews = from news in Db.News
                                              where news.PublishStart.HasValue
                                              && news.PublishEnd.HasValue
                                              && news.PublishEnd.Value >= now
                                              && news.PublishStart.Value <= now
                                              && news.IsActive
                                              orderby news.Id descending
                                              select new NewsDTO
                                              {
                                                  Id = news.Id,
                                                  Title = news.Title,
                                                  Summary = news.ContentSummary,
                                                  CreatedOn = news.CreatedOn,
                                                  PublishStart = news.PublishStart,
                                                  PublishEnd = news.PublishEnd,
                                                  HasNotificationsSent = news.HasNotificationsSent
                                              };

            return arrayOfNews;
        }

        private IQueryable<NewsDTO> GetParametersFilteredNews(NewsFilters filters)
        {
            DateTime now = DateTime.Now;
            IQueryable<NewsDTO> arrayOfNews = from news in Db.News
                                              where news.PublishStart.HasValue
                                                  && news.PublishEnd.HasValue
                                                  && news.PublishEnd.Value >= now
                                                  && news.PublishStart.Value <= now
                                                  && news.IsActive
                                              orderby news.Id descending
                                              select new NewsDTO
                                              {
                                                  Id = news.Id,
                                                  Title = news.Title,
                                                  Summary = news.ContentSummary,
                                                  CreatedOn = news.CreatedOn,
                                                  PublishStart = news.PublishStart,
                                                  PublishEnd = news.PublishEnd,
                                                  HasNotificationsSent = news.HasNotificationsSent
                                              };

            if (filters.DistrictsIds != null && filters.DistrictsIds.Length > 0)
            {
                HashSet<int> newsIds = (from newsDistrict in Db.NewsDistricts
                                        join news in Db.News on newsDistrict.NewsId equals news.Id
                                        where news.PublishStart.HasValue
                                              && news.PublishEnd.HasValue
                                              && news.PublishEnd.Value >= now
                                              && news.PublishStart.Value <= now
                                              && news.IsActive
                                              && filters.DistrictsIds.Contains(newsDistrict.DistrictId)
                                              && newsDistrict.IsActive
                                        select newsDistrict.NewsId).ToHashSet();

                arrayOfNews = from news in arrayOfNews
                              where newsIds.Contains(news.Id)
                              select news;
            }

            if (filters.DateFrom.HasValue)
            {
                arrayOfNews = arrayOfNews.Where(news => news.CreatedOn >= filters.DateFrom);
            }

            if (filters.DateTo.HasValue)
            {
                filters.DateTo = filters.DateTo.Value.AddDays(1).AddSeconds(-1);

                arrayOfNews = arrayOfNews.Where(news => news.CreatedOn <= filters.DateTo);
            }

            return arrayOfNews;
        }

        private IQueryable<NewsDTO> GetFreeTextFilteredNews(string text)
        {
            DateTime now = DateTime.Now;
            DateTime? date = DateTimeUtils.TryParseDate(text);

            IQueryable<NewsDTO> arrayOfNews = from news in Db.News
                                              where news.PublishStart.HasValue
                                                    && news.PublishEnd.HasValue
                                                    && news.PublishEnd.Value >= now
                                                    && news.PublishStart.Value <= now
                                                    && news.IsActive
                                                    && (news.Title.ToLower().Contains(text)
                                                        || news.Contents.ToLower().Contains(text)
                                                        || (date.HasValue && (date.Value.Date == news.CreatedOn.Date))
                                                        || (date.HasValue && (date.Value.Date == news.CreatedOn.Date)))
                                              orderby news.Id descending
                                              select new NewsDTO
                                              {
                                                  Id = news.Id,
                                                  Title = news.Title,
                                                  Summary = news.ContentSummary,
                                                  CreatedOn = news.CreatedOn,
                                                  PublishStart = news.PublishStart,
                                                  PublishEnd = news.PublishEnd,
                                                  HasNotificationsSent = news.HasNotificationsSent
                                              };

            return arrayOfNews;
        }

        private int GetNewsMainPhotoTypeId()
        {
            return Db.NfileTypes.Where(x => x.Code == nameof(FileTypeEnum.NEWS_MAIN_PHOTO)).Select(x=>x.Id).FirstOrDefault();
        }
    }
}
