using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces;

namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations
{
    public class Version5 : IVersion
    {
        private readonly INomenclatureDatesClear nomenclatureDates;

        public Version5(INomenclatureDatesClear nomenclatureDates)
        {
            this.nomenclatureDates = nomenclatureDates;
        }
        public void Migrate(AppDbContext context)
        {
            context.DropTable<ShipOwner>();
            context.CreateTable<ShipOwner>();

            nomenclatureDates.Remove(NomenclatureEnum.ShipOwner);
        }
    }
}
