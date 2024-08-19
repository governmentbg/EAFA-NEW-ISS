using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;

namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations
{
    public class ModelSnapshot
    {
        private readonly AppDbContext context;

        public ModelSnapshot(AppDbContext context)
        {
            this.context = context;
        }

        public void CreateDatabase()
        {
            context.CreateTable<InspectionFiles>();
            context.CreateTable<NLaws>();
            context.CreateTable<Catch>();
            context.CreateTable<Ship>();
            context.CreateTable<NFish>();
            context.CreateTable<NPort>();
            context.CreateTable<Legal>();
            context.CreateTable<Buyer>();
            context.CreateTable<Person>();
            context.CreateTable<Permit>();
            context.CreateTable<NGender>();
            context.CreateTable<LogBook>();
            context.CreateTable<NFishSex>();
            context.CreateTable<PoundNet>();
            context.CreateTable<ErrorLog>();
            context.CreateTable<NCountry>();
            context.CreateTable<Inspector>();
            context.CreateTable<NDistrict>();
            context.CreateTable<NFileType>();
            context.CreateTable<ShipOwner>();
            context.CreateTable<NFleetType>();
            context.CreateTable<NCatchZone>();
            context.CreateTable<Inspection>();
            context.CreateTable<NPermitType>();
            context.CreateTable<FishingGear>();
            context.CreateTable<NPermission>();
            context.CreateTable<NVesselType>();
            context.CreateTable<Aquaculture>();
            context.CreateTable<NFishingGear>();
            context.CreateTable<NInstitution>();
            context.CreateTable<PatrolVehicle>();
            context.CreateTable<NDocumentType>();
            context.CreateTable<PermitLicense>();
            context.CreateTable<NMunicipality>();
            context.CreateTable<NPopulatedArea>();
            context.CreateTable<NWaterBodyType>();
            context.CreateTable<NVesselActivity>();
            context.CreateTable<NInspectionType>();
            context.CreateTable<FishingGearMark>();
            context.CreateTable<RecentInspector>();
            context.CreateTable<NInspectionState>();
            context.CreateTable<NObservationTool>();
            context.CreateTable<NShipAssociation>();
            context.CreateTable<NTurbotSizeGroup>();
            context.CreateTable<InspectorsHistory>();
            context.CreateTable<NFishPresentation>();
            context.CreateTable<NTranslationGroup>();
            context.CreateTable<NRequiredFileType>();
            context.CreateTable<FishingGearPinger>();
            context.CreateTable<NPatrolVehicleType>();
            context.CreateTable<NPermitLicenseType>();
            context.CreateTable<PoundNetFishingGear>();
            context.CreateTable<NInspectionCheckType>();
            context.CreateTable<NTranslationResource>();
            context.CreateTable<NCatchInspectionType>();
            context.CreateTable<NInspectedPersonType>();
            context.CreateTable<NInspectionVesselType>();
            context.CreateTable<NTransportVehicleType>();
            context.CreateTable<PoundNetPermitLicense>();
            context.CreateTable<NFishingGearMarkStatus>();
            context.CreateTable<PoundNetFishingGearMark>();
            context.CreateTable<NFishingGearCheckReason>();
            context.CreateTable<NFishingGearPingerStatus>();
            context.CreateTable<PoundNetFishingGearPinger>();
            context.CreateTable<NFishingGearRecheckReason>();
        }
    }
}
