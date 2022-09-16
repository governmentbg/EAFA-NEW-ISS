using IARA.Mobile.Application.DTObjects.Profile.API;
using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Pub.Converters
{
    public class RolesToVisibilityConverter : BaseValueConverter<bool, TLObservableCollection<RoleApiDto>>
    {
        public override bool ConvertTo(TLObservableCollection<RoleApiDto> roles)
        {
            if (roles == null || roles.Count == 0)
                return false;

            if (roles.Count == 1)
                return roles[0].Id != -1; //Публичен потребител id = -1

            return true;
        }
    }
}
