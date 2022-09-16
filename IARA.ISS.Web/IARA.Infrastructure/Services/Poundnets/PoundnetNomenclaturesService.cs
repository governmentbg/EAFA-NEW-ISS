using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services
{
    public class PoundnetNomenclaturesService : Service, IPoundnetNomenclaturesService
    {
        public PoundnetNomenclaturesService(IARADbContext db)
                             : base(db)
        {
        }

        public List<NomenclatureDTO> GetPoundnetCategories()
        {
            return GetNomenclature(Db.NpoundNetCategoryTypes);
        }

        public List<NomenclatureDTO> GetSeasonalTypes()
        {
            return GetNomenclature(Db.NpoundNetSeasonTypes);
        }

        public List<NomenclatureDTO> GetPoundnetStatuses()
        {
            return GetCodeNomenclature(Db.NpoundNetStatuses);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
