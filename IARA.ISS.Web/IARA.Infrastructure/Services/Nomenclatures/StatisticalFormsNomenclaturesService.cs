using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class StatisticalFormsNomenclaturesService : BaseService, IStatisticalFormsNomenclaturesService
    {
        public StatisticalFormsNomenclaturesService(IARADbContext dbContext)
            : base(dbContext)
        {
        }

        public List<NomenclatureDTO> GetGrossTonnageIntervals()
        {
            return GetCodeNomenclature(Db.NgrossTonageStatIntervals);
        }

        public List<NomenclatureDTO> GetVesselLengthIntervals()
        {
            return GetCodeNomenclature(Db.NvesselLengthStatIntervals);
        }

        public List<NomenclatureDTO> GetFuelTypes()
        {
            return GetCodeNomenclature(Db.NfuelTypes);
        }

        public List<NomenclatureDTO> GetReworkProductTypes()
        {
            return GetNomenclature(Db.NreworkProductTypes);
        }

        public List<StatisticalFormAquacultureNomenclatureDTO> GetAllAquacultureNomenclatures()
        {
            var result = (from aqua in Db.AquacultureFacilitiesRegister
                          join legal in Db.Legals on aqua.SubmittedForLegalId equals legal.Id
                          where aqua.RecordType == nameof(RecordTypesEnum.Register)
                          orderby aqua.Name
                          select new StatisticalFormAquacultureNomenclatureDTO
                          {
                              Value = aqua.Id,
                              DisplayName = aqua.Name,
                              EIK = legal.Eik,
                              LegalName = legal.Name,
                              UrorNum = aqua.UrorNum,
                              RegNum = aqua.RegNum.HasValue ? aqua.RegNum.ToString() : null,
                              IsActive = aqua.IsActive
                          }).ToList();

            return result;
        }
    }
}
