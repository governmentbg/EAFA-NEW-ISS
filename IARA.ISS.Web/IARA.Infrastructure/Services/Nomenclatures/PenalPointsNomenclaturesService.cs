using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using IARA.Common.Enums;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class PenalPointsNomenclaturesService : BaseService, IPenalPointsNomenclaturesService
    {
        public PenalPointsNomenclaturesService(IARADbContext db)
            : base(db)
        { }

        public List<NomenclatureDTO> GetAllPenalDecrees()
        {
            List<NomenclatureDTO> decrees = (from decree in Db.PenalDecreesRegisters
                                             join type in Db.NpenalDecreeTypes on decree.PenalDecreeTypeId equals type.Id
                                             join status in Db.PenalDecreeStatuses on decree.Id equals status.PenalDecreeId
                                             join statusType in Db.NpenalDecreeStatusTypes on status.StatusTypeId equals statusType.Id
                                             where type.Code == nameof(PenalDecreeTypeEnum.PenalDecree)
                                                && statusType.Code == nameof(PenalDecreeStatusTypesEnum.Valid)
                                                && decree.IsActive
                                                && status.IsActive
                                             orderby decree.DecreeNum
                                             select new NomenclatureDTO
                                             {
                                                 Value = decree.Id,
                                                 DisplayName = decree.DecreeNum,
                                                 IsActive = decree.IsActive
                                             }).ToList();

            return decrees;
        }

        public List<NomenclatureDTO> GetAllPenalPoinsStatuses()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NpenalPointStatuses);
            return result;
        }
    }
}
