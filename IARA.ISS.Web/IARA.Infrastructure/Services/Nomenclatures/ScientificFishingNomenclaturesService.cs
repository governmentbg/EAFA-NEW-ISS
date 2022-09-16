using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services
{
    public class ScientificFishingNomenclaturesService : Service, IScientificFishingNomenclaturesService
    {
        public ScientificFishingNomenclaturesService(IARADbContext db)
                : base(db)
        { }

        public List<ScientificFishingReasonNomenclatureDTO> GetPermitReasons()
        {
            DateTime now = DateTime.Now;

            List<ScientificFishingReasonNomenclatureDTO> result = (from reason in Db.NpermitReasons
                                                                   orderby reason.Name
                                                                   select new ScientificFishingReasonNomenclatureDTO
                                                                   {
                                                                       Value = reason.Id,
                                                                       DisplayName = reason.Name,
                                                                       IsLegalReason = reason.IsLegalReason,
                                                                       IsActive = reason.ValidFrom <= now && reason.ValidTo > now
                                                                   }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetPermitStatuses()
        {
            return GetCodeNomenclature(Db.NpermitStatuses);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
