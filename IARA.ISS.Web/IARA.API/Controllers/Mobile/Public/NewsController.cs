using System.Linq;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.NewsPublic;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class NewsController : BaseController
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.newsService = newsService;
        }

        [HttpPost]
        public IActionResult GetAll([FromBody] GridRequestModel<NewsFilters> gridRequestModel)
        {
            IQueryable<NewsMobileDTO> listOfNews = this.newsService.GetAllMobileNews(gridRequestModel.Filters);
            GridResultModel<NewsMobileDTO> result = new(listOfNews, gridRequestModel, false);

            return this.Ok(this.newsService.SetNewsHasImageFlag(result.Records));
        }

        [HttpGet]
        public IActionResult GetNewsMainPhoto([FromQuery] int id)
        {
            DownloadableFileDTO photo = this.newsService.GetNewsMainPhoto(id);

            if (photo == null)
            {
                return this.Ok();
            }

            return this.File(photo.Bytes, photo.MimeType, photo.FileName);
        }

        [HttpGet]
        public IActionResult GetNewsDetails([FromQuery] int newsId)
        {
            NewsDetailsMobileDTO news = this.newsService.GetNewsDetails(newsId);

            if (news == null)
            {
                return this.NotFound();
            }

            return this.Ok(news);
        }
    }
}
