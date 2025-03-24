using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces;

namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations
{
    public class Version3 : IVersion
    {
        private readonly INomenclatureDatesClear nomenclatureDates;

        public Version3(INomenclatureDatesClear nomenclatureDates)
        {
            this.nomenclatureDates = nomenclatureDates;
        }
        public void Migrate(AppDbContext context)
        {
            context.DropTable<NFishingGear>();
            context.CreateTable<NFishingGear>();
            nomenclatureDates.Remove(nameof(NomenclatureEnum.FishingGear));
        }
    }
}
