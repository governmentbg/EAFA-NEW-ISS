using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.Security.Permissions;

namespace IARA.WebHelpers.Helpers
{
    public class PermittedCodePages
    {

        public static readonly Dictionary<string, List<PageCodeEnum>> ALL = new()
        {
            {
                nameof(Permissions.QualifiedFishersApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.AptitudeCourceExam,
                    PageCodeEnum.CommFishLicense
                }
            },
            {
                nameof(Permissions.ScientificFishingApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.SciFi
                }
            },
            {
                nameof(Permissions.LegalEntitiesApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.LE
                }
            },
            {
                nameof(Permissions.BuyersApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.RegFirstSaleCenter,
                    PageCodeEnum.RegFirstSaleBuyer,
                    PageCodeEnum.ChangeFirstSaleBuyer,
                    PageCodeEnum.TermFirstSaleBuyer,
                    PageCodeEnum.ChangeFirstSaleCenter,
                    PageCodeEnum.TermFirstSaleCenter
                }
            },
            {
                nameof(Permissions.ShipsRegisterApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.ShipRegChange,
                    PageCodeEnum.RegVessel,
                    PageCodeEnum.DeregShip
                }
            },
            {
                nameof(Permissions.TicketApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.RecFish,
                    PageCodeEnum.RecFish14And18,
                    PageCodeEnum.RecFishDisbl,
                    PageCodeEnum.RecFishElder,
                    PageCodeEnum.RecFishStd,
                    PageCodeEnum.RecFishUnd14
                }
            },
            {
                nameof(Permissions.AquacultureFacilitiesApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.AquaFarmReg,
                    PageCodeEnum.AquaFarmChange,
                    PageCodeEnum.AquaFarmDereg
                }
            },
            {
                nameof(Permissions.FishingCapacityApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.IncreaseFishCap,
                    PageCodeEnum.ReduceFishCap,
                    PageCodeEnum.TransferFishCap,
                    PageCodeEnum.CapacityCertDup
                }
            },
            {
                nameof(Permissions.StatisticalFormsAquaFarmApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.StatFormAquaFarm
                }
            },
            {
                nameof(Permissions.StatisticalFormsFishVesselsApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.StatFormFishVessel
                }
            },
            {
                nameof(Permissions.StatisticalFormsReworkApplicationsRead),
                new List<PageCodeEnum>
                {
                    PageCodeEnum.StatFormRework
                }
            },
            {
                nameof(Permissions.CommercialFishingPermitApplicationsRead),
                new List<PageCodeEnum>
                {
                     PageCodeEnum.CommFish,
                     PageCodeEnum.RightToFishResource,
                     PageCodeEnum.RightToFishThirdCountry,
                     PageCodeEnum.PoundnetCommFish,
                     PageCodeEnum.PoundnetCommFishLic,
                     PageCodeEnum.CatchQuataSpecies,
                }
            },
            {
                nameof(Permissions.ApplicationsRead),
                new List<PageCodeEnum>()
            }
        };
    }
}
