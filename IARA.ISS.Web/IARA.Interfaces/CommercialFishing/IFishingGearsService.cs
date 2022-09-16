using System.Collections.Generic;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.CommercialFishing
{
    public interface IFishingGearsService : IService
    {
        List<FishingGearDTO> GetCommercialFishingPermitLicenseFishingGears(int permitLicenseId, bool noId = false);
        List<FishingGearDTO> GetShipFishingGears(int shipUId);
        List<NomenclatureDTO> GetShipFishingGearNomenclatures(int shipId, int year);
        /// <summary>
        /// Calls Db.SaveChanges() at the end. Only `permitLicenseId` or only `inspectionId` should be passed
        /// </summary>
        /// <param name="fishingGear">Fishing gear model</param>
        /// <param name="permitLicenseId">Id of Commercial fishing permit license</param>
        /// <param name="inspectionId">Id of Inspection</param>
        /// <returns></returns>
        int AddOrEditFishingGear(FishingGearDTO fishingGear, int? permitLicenseId, int? inspectionId);
        void MapFishingGearMarksAndPingers(List<FishingGearDTO> result, bool noIds = false);
    }
}
