using System.Collections.Generic;
using IARA.Common.GridModels;

namespace IARA.DomainModels.DTOModels.Common
{
    public class NotificationsDTO : BaseGridResultModel<NotificationDTO>
    {
        public NotificationsDTO()
        {
            this.Records = new List<NotificationDTO>();
            this.TotalUnread = 0;
            this.TotalRecordsCount = 0;
        }

        public int TotalUnread { get; set; }

    }
}
