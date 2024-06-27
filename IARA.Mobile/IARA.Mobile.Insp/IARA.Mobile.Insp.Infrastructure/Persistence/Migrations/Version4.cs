using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces;

namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations
{
    public class Version4 : IVersion
    {
        public void Migrate(AppDbContext context)
        {
            context.CreateTable<NLaws>();
        }
    }
}
