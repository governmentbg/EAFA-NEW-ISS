using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces;

namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations
{
    public class Version6 : IVersion
    {
        public void Migrate(AppDbContext context)
        {
            context.CreateTable<InspectionFiles>();
        }
    }
}
