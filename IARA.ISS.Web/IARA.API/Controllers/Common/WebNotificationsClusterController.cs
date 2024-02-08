using IARA.WebHelpers;
using TL.SysToSysSecCom.Abstractions.Interfaces;
using TL.SysToSysSecCom.Abstractions.Models;
using TL.WebNotifications.Abstractions.Enums;
using TL.WebNotifications.Cluster.Controllers;
using TL.WebNotifications.Cluster.Interfaces;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    public class WebNotificationsClusterController : BaseWebNotificationsClusterController<NotificationTypes>
    {
        public WebNotificationsClusterController(ICryptoHelper cryptoHelper,
                                          SysToSysCryptoSettings settings,
                                          IRequestContentSerializer contentSerializer,
                                          IClusteredWebNotificationsSender<NotificationTypes> notificationsSender)
            : base(cryptoHelper, settings, contentSerializer, notificationsSender)
        { }
    }
}
