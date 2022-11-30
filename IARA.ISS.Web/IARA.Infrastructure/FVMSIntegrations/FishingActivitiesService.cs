using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.FVMSModels.FA;
using IARA.Interfaces.FVMSIntegrations;

namespace IARA.Infrastructure.FVMSIntegrations
{
    public class FishingActivitiesService : BaseService, IFishingActivitiesService
    {
        public FishingActivitiesService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public List<OpenedLogBookPage> GetOpenedLogBookPages(string tripId)
        {
            HashSet<int> logBookPageIds = (from faLogBookPage in Db.FvmsfishingActivityReportLogBookPages
                                           where faLogBookPage.TripIdentifier == tripId
                                                  && faLogBookPage.IsActive
                                           select faLogBookPage.ShipLogBookPageId).ToHashSet();

            List<OpenedLogBookPage> data = (from logBookPage in Db.ShipLogBookPages
                                            join logBook in Db.LogBooks on logBookPage.LogBookId equals logBook.Id
                                            join logBookPermitLicense in Db.LogBookPermitLicenses on logBookPage.LogBookPermitLicenceId equals logBookPermitLicense.Id
                                            join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                            join fishingGearRegister in Db.FishingGearRegisters on logBookPage.FishingGearRegisterId equals fishingGearRegister.Id
                                            join fishingGear in Db.NfishingGears on fishingGearRegister.FishingGearTypeId equals fishingGear.Id
                                            where logBookPageIds.Contains(logBookPage.Id)
                                            select new OpenedLogBookPage
                                            {
                                                Number = logBookPage.PageNum,
                                                LogBookNumber = logBook.LogNum,
                                                LicenseNumber = permitLicense.RegistrationNum,
                                                FishingGearData = new FishingGearCharacteristics
                                                {
                                                    Code = fishingGear.Code,
                                                    Description = fishingGearRegister.Description,
                                                    Height = fishingGearRegister.Height,
                                                    MeshSize = fishingGearRegister.NetEyeSize,
                                                    QuantityOnBoard = fishingGearRegister.GearCount,
                                                    TrawlModel = fishingGearRegister.TrawlModel,
                                                    NetsNumber = fishingGearRegister.NumberOfNetsInFleet,
                                                    OneNetLength = fishingGearRegister.NetNominalLength,
                                                    Number = fishingGearRegister.HookCount,
                                                    NumberOfLines = fishingGearRegister.LineCount,
                                                    Dimension = fishingGearRegister.Length,
                                                    //BarDistance // TODO ?
                                                    //DevicesAndGearAttachments // TODO ?
                                                }
                                            }).ToList();

            return data;
        }
    }
}
