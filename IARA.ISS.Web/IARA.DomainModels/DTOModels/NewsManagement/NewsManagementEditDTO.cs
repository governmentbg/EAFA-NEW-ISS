using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.NewsManagment
{
    public class NewsManagementEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Content { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(1000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Summary { get; set; }

        public DateTime? PublishStart { get; set; }

        public DateTime? PublishEnd { get; set; }

        public bool HasNotificationsSent { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool IsDistrictLimited { get; set; }

        public List<int> DistrictsIds { get; set; }

        public List<FileInfoDTO> Files { get; set; }

        public FileInfoDTO MainImage { get; set; }
    }
}
