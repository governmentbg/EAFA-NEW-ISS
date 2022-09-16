using System.Collections.Generic;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IShipsRegisterNomenclaturesService
    {
        List<ShipEventTypeDTO> GetEventTypes();

        List<FleetTypeNomenclatureDTO> GetFleetTypes();

        List<NomenclatureDTO> GetPublicAidTypes();

        List<SailAreaNomenclatureDTO> GetSailAreas();

        List<NomenclatureDTO> GetSegments();

        List<VesselTypeNomenclatureDTO> GetVesselTypes();

        List<NomenclatureDTO> GetPorts();

        List<NomenclatureDTO> GetHullMaterials();

        List<NomenclatureDTO> GetFuelTypes();
    }
}
