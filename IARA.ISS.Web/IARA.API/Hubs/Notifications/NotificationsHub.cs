using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.GridModels;
using IARA.DomainModels.DTOModels.Common;
using IARA.Interfaces.Common;
using IARA.Notifications;
using IARA.Notifications.Enums;
using IARA.Notifications.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IdentityModel;

namespace IARA.WebAPI.Hubs
{
    public class NotificationsHub : BaseNotificationsHub
    {
        private ScopedServiceProviderFactory serviceProviderFactory;
        public NotificationsHub(INotificationsSubscriber subscriber, ScopedServiceProviderFactory serviceProviderFactory)
            : base(subscriber)
        {
            this.serviceProviderFactory = serviceProviderFactory;
        }

        public override Task OnConnectedAsync()
        {
            Task task = base.OnConnectedAsync();
            subscriber.SubscribeForEvent(this.Context.ConnectionId, NotificationTypes.User);
            return task;
        }

        [CustomAuthorize]
        public override bool UpdateUserData(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadToken(token);
            var jwtToken = securityToken as JwtSecurityToken;

            var claims = jwtToken.Payload.Where(x => x.Value != null).Select(x => new Claim(x.Key, x.Value.ToString()));
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "JWT", JwtClaimTypes.Name, JwtClaimTypes.Role));

            return UpdateUserData(user);
        }

        protected override string GetUserIdentifier(ClaimsPrincipal user)
        {
            int? userId = default;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var claimsPrincipal = user;
                userId = claimsPrincipal.GetUserId();
            }

            return userId.HasValue ? userId.Value.ToString() : null;
        }

        public bool MarkNotificationAsRead(int notificationId)
        {
            using (IScopedServiceProvider serviceProvider = serviceProviderFactory.GetServiceProvider())
            {
                IWebNotificationsService notificationsService = serviceProvider.GetRequiredService<IWebNotificationsService>();

                string strUserId = subscriber.GetUserId(this.Context.ConnectionId);

                if (!string.IsNullOrEmpty(strUserId))
                {
                    return notificationsService.MarkNotificationAsRead(int.Parse(strUserId), notificationId);
                }

                return false;
            }
        }

        public NotificationsDTO GetUserNotifications(BaseGridRequestModel request)
        {
            using (IScopedServiceProvider serviceProvider = serviceProviderFactory.GetServiceProvider())
            {
                IWebNotificationsService notificationsService = serviceProvider.GetRequiredService<IWebNotificationsService>();
                string strUserId = this.subscriber.GetUserId(this.Context.ConnectionId);

                if (!string.IsNullOrEmpty(strUserId))
                {
                    return notificationsService.GetUserNotifications(int.Parse(strUserId), request.PageNumber, request.PageSize);
                }
                else
                {
                    return new NotificationsDTO();
                }
            }
        }
    }
}
