using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using IARA.Common.Enums;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class AquacultureFacilitiesNomenclaturesService : Service, IAquacultureFacilitiesNomenclaturesService
    {
        public AquacultureFacilitiesNomenclaturesService(IARADbContext db)
            : base(db)
        {
        }

        public List<NomenclatureDTO> GetAllAquacultureNomenclatures()
        {
            List<NomenclatureDTO> result = (from aqua in Db.AquacultureFacilitiesRegister
                                            where aqua.RecordType == nameof(RecordTypesEnum.Register)
                                            orderby aqua.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = aqua.Id,
                                                DisplayName = aqua.Name,
                                                Description = $"{{UROR}}: {aqua.UrorNum} | {{REGNUM}}: {aqua.RegNum}",
                                                IsActive = aqua.IsActive
                                            }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetAquaculturePowerSupplyTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NaquaculturePowerSupplyTypes);
            return result;
        }

        public List<NomenclatureDTO> GetAquacultureWaterAreaTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from area in Db.NaquacultureWaterAreaTypes
                                            orderby area.Code
                                            select new NomenclatureDTO
                                            {
                                                Value = area.Id,
                                                Code = area.Code,
                                                DisplayName = area.Name,
                                                IsActive = area.ValidFrom <= now && area.ValidTo > now
                                            }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetWaterLawCertificateTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NwaterLawCertificateTypes);
            return result;
        }

        public List<NomenclatureDTO> GetAquacultureInstallationTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NaquacultureInstallationTypes);
            return result;
        }

        public List<NomenclatureDTO> GetInstallationBasinPurposeTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NinstallationBasinPurposeTypes);
            return result;
        }

        public List<NomenclatureDTO> GetInstallationBasinMaterialTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NinstallationBasinMaterialTypes);
            return result;
        }

        public List<NomenclatureDTO> GetHatcheryEquipmentTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NhatcheryEquipmentTypes);
            return result;
        }

        public List<NomenclatureDTO> GetInstallationNetCageTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NinstallationNetCageTypes);
            return result;
        }

        public List<NomenclatureDTO> GetInstallationCollectorTypes()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NinstallationCollectorTypes);
            return result;
        }

        public List<NomenclatureDTO> GetAquacultureStatusTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NaquacultureStatuses);
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
