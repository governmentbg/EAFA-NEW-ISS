using IARA.Mobile.Application.Interfaces.Database;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Entities.Nomenclatures;
using IARA.Mobile.Shared.Extensions;
using System;
using System.Linq;

namespace IARA.Mobile.Shared.Utilities
{
    public class PageVersionUtility : IPageVersion
    {
        private readonly IDbContextBuilder _builder;

        public PageVersionUtility(IDbContextBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public bool IsPageAllowed(string pageName, string platform)
        {
            if (!_builder.DatabaseExists)
            {
                return true;
            }

            int buildNumber = VersionExtensions.GetBuilderNumber();

            using (IDbContext context = _builder.CreateContext())
            {
                return !context.TLTable<NVersion>()
                    .Where(f => f.Name == pageName && f.OSType == platform && f.Version > buildNumber)
                    .Any();
            }
        }
    }
}
