using IARA.Flux.Models;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class ArrivalNotificationService : BaseFishingActivityService
    {
        private readonly ArrivalDeclarationService _arrivalDeclarationService;

        public ArrivalNotificationService(IARADbContext dbContext, IExtendedLogger logger) 
            : base(dbContext, logger, "ArrivalNotificationService.cs")
        {
            _arrivalDeclarationService = new ArrivalDeclarationService(dbContext, logger);
        }

        public List<ShipLogBookPage> MapOriginal(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = _arrivalDeclarationService.MapArrivalData(fishingActivity);
            return relatedPages;
        }

        public List<ShipLogBookPage> MapCancel(IDType referenceId)
        {
            // Clear arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = _arrivalDeclarationService.ClearArrivalData(referenceId);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapDelete(IDType referenceId)
        {
            // Delete arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = _arrivalDeclarationService.ClearArrivalData(referenceId);

            return relatedPages;
        }

    }
}
