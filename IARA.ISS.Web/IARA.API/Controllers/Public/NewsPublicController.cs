using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.NewsPublic;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class NewsPublicController : BaseController
    {
        private readonly INewsService newsService;
        private readonly IFileService fileService;
        private readonly ICommonNomenclaturesService nomenclaturesService;

        public NewsPublicController(IPermissionsService permissionsService,
            INewsService newsService,
            IFileService fileService,
            ICommonNomenclaturesService nomenclaturesService)
            : base(permissionsService)
        {
            this.newsService = newsService;
            this.fileService = fileService;
            this.nomenclaturesService = nomenclaturesService;
        }

        [HttpGet]
        public IActionResult GetPublishedNews([FromQuery] int id)
        {
            NewsDetailsDTO news = this.newsService.GetPublishedNews(id);

            if (news == null)
            {
                return this.NotFound();
            }

            return this.Ok(news);
        }

        [HttpGet]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = this.fileService.GetFileForDownload(id);
            return this.File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        public IActionResult GetAll([FromBody] GridRequestModel<NewsFilters> gridRequestModel)
        {
            IQueryable<NewsDTO> listOfNews = this.newsService.GetAll(gridRequestModel.Filters);
            return this.PageResult(listOfNews, gridRequestModel, false);
        }

        [HttpPost]
        public IActionResult GetNewsImages([FromBody] int[] newsIds)
        {
            List<NewsImageDTO> newsImages = this.newsService.GetNewsImages(newsIds);
            return this.Ok(newsImages);
        }

        // TODO: Може би това не трябва да е тука
        [HttpGet]
        public IActionResult GetDistricts()
        {
            List<DomainModels.Nomenclatures.NomenclatureDTO> result = this.nomenclaturesService.GetDistricts();
            return this.Ok(result);
        }
    }
}
