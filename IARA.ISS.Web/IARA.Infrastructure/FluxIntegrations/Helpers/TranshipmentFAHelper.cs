using System;
using System.Linq;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationTranshipment(FishingActivityType fishingActivity,
                                                                             FaVesselRoleCodes reportingVesselRole,
                                                                             DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> shipLogBookPages = new List<ShipLogBookPage>();
            DateTime now = DateTime.Now;
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            string relatedVesselRoleCode = fishingActivity.RelatedVesselTransportMeans[0].RoleCode.Value;
            FaVesselRoleCodes relatedVesselRole;
            bool successfulCastRelatedVesseslRole = Enum.TryParse<FaVesselRoleCodes>(relatedVesselRoleCode, out relatedVesselRole);

            if (successfulCastRelatedVesseslRole)
            {
                if (reportingVesselRole == FaVesselRoleCodes.DONOR && relatedVesselRole == FaVesselRoleCodes.RECEIVER)
                {
                    List<FACatchType> unloadedFaCatches = fishingActivity.SpecifiedFACatch.Where(x => x.TypeCode.Value == nameof(FaCatchTypes.UNLOADED)).ToList();
                    MapFaCatchToOriginDeclaration(fishingActivity, unloadedFaCatches, UnloadingTypesEnum.TRN, documentOccurrenceDateTime);
                }
                else if (reportingVesselRole == FaVesselRoleCodes.RECEIVER && relatedVesselRole == FaVesselRoleCodes.DONOR)
                {
                    DateTime transhipmentStartDateTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].StartDateTime;
                    DateTime transhipmentEndDateTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].EndDateTime;

                    List<int> transhipmentFromShipIds = GetShipIds(fishingActivity.RelatedVesselTransportMeans[0]);
                    int? transhipmentFromShipId = transhipmentFromShipIds.FirstOrDefault();
                    if (!transhipmentFromShipId.HasValue)
                    {
                        string msg = $"{LOGGER_MSG_TYPE} No transhipment from ship id of vessel is found for transhipment msg from receiver to donor in trip: {fishingTrip}";
                        Logger.LogWarning(msg);
                    }

                    HashSet<int> logBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);

                    if (logBookPageIds.Count == 0)
                    {
                        Logger.LogWarning($"{LOGGER_MSG_TYPE} There are no pages added for trip {fishingTrip} and vessel {transhipmentFromShipId}");
                    }

                    int logBookPageId = logBookPageIds.Count > 0 ? logBookPageIds.First() : -1;

                    CatchRecord catchRecord = new CatchRecord
                    {
                        CatchOperCount = 1,
                        GearEntryTime = transhipmentStartDateTime,
                        GearExitTime = transhipmentEndDateTime,
                        TransboardFromShipId = transhipmentFromShipId,
                        LogBookPageId = logBookPageId
                    };

                    ShipLogBookPage shipLogBookPage = GetShipLogBookPageById(logBookPageId);

                    shipLogBookPages.Add(shipLogBookPage);

                    Db.CatchRecords.Add(catchRecord);

                    List<FACatchType> loadedCatches = fishingActivity.SpecifiedFACatch.Where(x => x.TypeCode.Value == nameof(FaCatchTypes.LOADED)).ToList();

                    foreach (FACatchType loadedCatch in loadedCatches)
                    {
                        int fishId = GetAquaticOrganismId(loadedCatch.SpeciesCode.Value);
                        int? catchSizeId = GetCatchSizeId(loadedCatch.SpecifiedSizeDistribution);
                        int? catchTypeId = GetCatchTypeId(loadedCatch.TypeCode.Value);
                        int? catchZoneId = GetCatchLocationZoneId(loadedCatch.SpecifiedFLUXLocation);
                        decimal quantity = loadedCatch.WeightMeasure.Value;
                        int? units = loadedCatch.UnitQuantity != null ? (int)loadedCatch.UnitQuantity.Value : default;

                        CatchRecordFish catchRecordFish = new CatchRecordFish
                        {
                            CatchRecord = catchRecord,
                            CatchSizeId = catchSizeId,
                            CatchTypeId = catchTypeId,
                            CatchZoneId = catchZoneId ?? -1,
                            FishId = fishId,
                            IsContinentalCatch = false, // TODO
                            Quantity = quantity, // TODO
                            SturgeonGender = null, // TODO
                            SturgeonSize = null, // TODO
                            SturgeonWeightKg = null, // TODO
                            ThirdCountryCatchZone = null, // TODO
                            TurbotSizeGroupId = null, // TODO
                            TurbotCount = units
                        };

                        Db.CatchRecordFish.Add(catchRecordFish);
                    }
                }
            }
            else
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} non-existing role code ({relatedVesselRoleCode}) of vessel");
            }

            return shipLogBookPages;
        }

    }
}
