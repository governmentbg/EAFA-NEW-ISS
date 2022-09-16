using System;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebHelpers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AreaRouteAttribute : RouteAttribute
    {
        public AreaRouteAttribute(AreaType area = AreaType.None, string template = "[controller]/[action]")
            : base(GenerateTemplate(area, template))
        {
            Order = 0;
        }

        private static string GenerateTemplate(AreaType area, string template)
        {
            switch (area)
            {
                case AreaType.None:
                    return template;

                case AreaType.Administrative:
                    return $"{nameof(AreaType.Administrative)}/{template}";

                case AreaType.Public:
                    return $"{nameof(AreaType.Public)}/{template}";

                case AreaType.Nomenclatures:
                    return $"{nameof(AreaType.Nomenclatures)}/{template}";

                case AreaType.Integration:
                    return $"{nameof(AreaType.Integration)}/{template}";

                case AreaType.MobileAdministrative:
                    return $"Mobile/Administrative/{template}";

                case AreaType.MobilePublic:
                    return $"Mobile/Public/{template}";

                case AreaType.Common:
                    return $"{nameof(AreaType.Common)}/{template}";

                default:
                    throw new ArgumentException("Invalid area type");
            }
        }
    }

    public enum AreaType
    {
        None,
        Administrative,
        Public,
        Nomenclatures,
        MobileAdministrative,
        MobilePublic,
        Integration,
        Common
    }
}
