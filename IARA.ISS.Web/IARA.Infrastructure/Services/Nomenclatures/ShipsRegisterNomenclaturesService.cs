using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class ShipsRegisterNomenclaturesService : Service, IShipsRegisterNomenclaturesService
    {
        public ShipsRegisterNomenclaturesService(IARADbContext db)
            : base(db)
        {
        }

        public List<ShipEventTypeDTO> GetEventTypes()
        {
            DateTime now = DateTime.Now;

            List<ShipEventTypeDTO> events = (from type in Db.NeventTypes
                                             join eventGroup in Db.NeventTypeGroups on type.GroupId equals eventGroup.Id
                                             orderby eventGroup.OrderNum, type.Code, type.Name
                                             select new ShipEventTypeDTO
                                             {
                                                 Value = type.Id,
                                                 Code = type.Code,
                                                 DisplayName = $"{type.Code} – {type.Name}",
                                                 GroupName = eventGroup.Name,
                                                 IsActive = type.ValidFrom <= now && type.ValidTo > now
                                             }).ToList();

            return events;
        }

        public List<FleetTypeNomenclatureDTO> GetFleetTypes()
        {
            DateTime now = DateTime.Now;

            List<FleetTypeNomenclatureDTO> result = (from fleet in Db.NfleetTypes
                                                     orderby fleet.Name
                                                     select new FleetTypeNomenclatureDTO
                                                     {
                                                         Value = fleet.Id,
                                                         DisplayName = fleet.Name,
                                                         HasFishingCapacity = fleet.HasFishingCapacity,
                                                         HasControlCard = fleet.HasControlCard,
                                                         HasFitnessCertificate = fleet.HasFitnessCertificate,
                                                         IsActive = fleet.ValidFrom <= now && fleet.ValidTo > now
                                                     }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetPublicAidTypes()
        {
            return GetCodeNomenclature(Db.NpublicAidTypes);
        }

        public List<SailAreaNomenclatureDTO> GetSailAreas()
        {
            DateTime now = DateTime.Now;

            List<SailAreaNomenclatureDTO> sailAreas = (from sa in Db.NsailAreas
                                                       orderby sa.Name
                                                       select new SailAreaNomenclatureDTO
                                                       {
                                                           Value = sa.Id,
                                                           DisplayName = sa.Name,
                                                           MaxShoreDistance = sa.MaxShoreDistance,
                                                           MaxSeaState = sa.MaxSeaState,
                                                           IsActive = sa.ValidFrom <= now && sa.ValidTo > now
                                                       }).ToList();

            return sailAreas;
        }

        public List<NomenclatureDTO> GetSegments()
        {
            return GetCodeNomenclature(Db.Nsegments);
        }

        public List<VesselTypeNomenclatureDTO> GetVesselTypes()
        {
            DateTime now = DateTime.Now;

            List<VesselTypeNomenclatureDTO> vesselTypes = (from vt in Db.NvesselTypes
                                                           orderby vt.Name
                                                           select new VesselTypeNomenclatureDTO
                                                           {
                                                               Value = vt.Id,
                                                               Code = vt.Code,
                                                               DisplayName = vt.Name,
                                                               ParentVesselTypeId = vt.ParentVesselTypeId,
                                                               IsActive = vt.ValidFrom <= now && vt.ValidTo > now
                                                           }).ToList();

            return vesselTypes;
        }

        public List<NomenclatureDTO> GetPorts()
        {
            return GetCodeNomenclature(Db.Nports);
        }

        public List<NomenclatureDTO> GetHullMaterials()
        {
            return GetNomenclature(Db.NhullMaterials);
        }

        public List<NomenclatureDTO> GetFuelTypes()
        {
            return GetCodeNomenclature(Db.NfuelTypes);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
