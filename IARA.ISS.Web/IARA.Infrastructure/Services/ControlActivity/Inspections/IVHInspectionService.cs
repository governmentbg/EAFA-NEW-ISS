using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class IVHInspectionService : BaseInspectionService<InspectionTransportVehicleDTO>
    {
        public IVHInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionTransportVehicleDTO Get(InspectionTransportVehicleDTO inspection)
        {
            InspectionTransportVehicleDTO ivhDTO = (
                from insp in Db.TransportVehicleInspections
                where insp.InspectionId == inspection.Id.Value
                select new InspectionTransportVehicleDTO
                {
                    VehicleTypeId = insp.VehicleTypeId,
                    CountryId = insp.CountryId,
                    TractorBrand = insp.TractorBrand,
                    TractorModel = insp.TractorModel,
                    TractorLicensePlateNum = insp.TractorLicensePlateNum,
                    TrailerLicensePlateNum = insp.TrailerLicensePlateNum,
                    IsSealed = insp.IsSealedVehicle,
                    SealInstitutionId = insp.SealInstitutionId,
                    SealCondition = insp.SealCondition,
                    TransporterComment = insp.TransporterComment,
                    InspectionAddress = insp.InspectionLocation,
                    TransportDestination = insp.TransportDestination,
                    InspectionLocation = insp.InpectionLocationCoordinates != null
                       ? new LocationDTO { Longitude = insp.InpectionLocationCoordinates.X, Latitude = insp.InpectionLocationCoordinates.Y }
                       : null,
                }).Single();

            ivhDTO.CatchMeasures = GetDeclarationCatchMeasures(inspection.Id.Value);
            ivhDTO.Personnel = GetPersonnel(inspection.Id.Value);

            return AssignFromBase(ivhDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionTransportVehicleDTO ivhDTO)
        {
            TransportVehicleInspection newIvhDbEntry = new()
            {
                Inspection = inspDbEntry,
                CountryId = ivhDTO.CountryId,
                IsSealedVehicle = ivhDTO.IsSealed,
                SealCondition = ivhDTO.SealCondition,
                SealInstitutionId = ivhDTO.SealInstitutionId,
                TractorLicensePlateNum = ivhDTO.TractorLicensePlateNum,
                TrailerLicensePlateNum = ivhDTO.TrailerLicensePlateNum,
                TransporterComment = ivhDTO.TransporterComment,
                VehicleTypeId = ivhDTO.VehicleTypeId,
                InspectionLocation = ivhDTO.InspectionAddress,
                TractorBrand = ivhDTO.TractorBrand,
                TractorModel = ivhDTO.TractorModel,
                TransportDestination = ivhDTO.TransportDestination,
                InpectionLocationCoordinates = ivhDTO.InspectionLocation != null
                    ? new Point(ivhDTO.InspectionLocation.Longitude.Value, ivhDTO.InspectionLocation.Latitude.Value)
                    : null,
                IsActive = true
            };

            AddDeclarationCatchMeasures(inspDbEntry, ivhDTO.CatchMeasures);

            Db.TransportVehicleInspections.Add(newIvhDbEntry);
        }
    }
}
