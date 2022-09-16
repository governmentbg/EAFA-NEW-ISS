using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.DataAccess;
using IARA.DI;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.FVMSModels.ExternalModels;
using IARA.FVMSModels.NISS;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces;
using IARA.Interfaces.Flux;
using IARA.Interfaces.FluxIntegrations.Ships;
using IARA.Interfaces.FVMSIntegrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.SysToSysSecCom;
using TL.SysToSysSecCom.Interfaces;
using TLTTS.Common.ConfigModels;

namespace IARA.Flux.Tests
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            DateTime d = DateTime.Now;
            string dateStr = d.ToString("yyyy-MM-ddTHH:mm:sszzz");
            var gae = DateTime.ParseExact(dateStr, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            IServiceProvider serviceProvider = GetServiceProvider();

            //FluxGenerateMethodTests.RunMethods(serviceProvider);
            //serviceProvider.ACDRTest();

            //int[] shipIds = new int[] { 
            //    32597, 40601, 46056, 51987, 52205, 52248, 57938, 59382, 60050, 61894, 58198, 59352,
            //    59370, 59783, 59797, 61420, 6432, 6716, 13038, 13645, 5792, 7316, 10185, 17683, 17705, 
            //    17766, 17830 
            //};

            //foreach (int shipId in shipIds)
            //{
            //    VesselReportTest(serviceProvider, new List<int> { shipId });
            //}

            //FirstSaleReportTest(serviceProvider, 129596, ReportPurposeCodes.Original);
            AdmissionReportTest(serviceProvider, 56862, ReportPurposeCodes.Original);

            //TestNISS(serviceProvider);

            //TestFishingActivitiesFromDB(serviceProvider);

            Console.WriteLine("Finished");

            Console.ReadLine();
        }

        private static void TestNISS(IServiceProvider serviceProvider)
        {
            IPermitsAndLicencesService service = serviceProvider.GetService<IPermitsAndLicencesService>();
            service.GetPermits("BGR002330678");
        }

        private static void TestRequestNISS(IServiceProvider serviceProvider)
        {
            string cfr = "BGR002420795";
            NISSQuery query = new NISSQuery
            {
                Identifier = cfr
            };

            ISecureHttpClient secureHttpClient = serviceProvider.GetService<ISecureHttpClient>();

            string url = "https://iara-internal.egov.bg";
            var result = secureHttpClient.SendAsync<NISSQuery, List<ApiPerm>>($"{url}/api/Integration/FVMS/PermitsByCFRQuery", query).Result;
        }

        private static async Task TestFishingActivitiesFromDB(IServiceProvider serviceProvider)
        {
            //DateTime dateFrom = new DateTime(2022, 6, 2, 19, 0, 0);
            //DateTime dateTo = new DateTime(2022, 6, 2, 19, 59, 59);
            serviceProvider.GetService<ICryptoHelper>();
            IARADbContext db = serviceProvider.GetService<IARADbContext>();
            IFishingActivitiesDomainService service = serviceProvider.GetService<IFishingActivitiesDomainService>();
            IFluxFishingActivitiesReceiverService receiverService = serviceProvider.GetService<IFluxFishingActivitiesReceiverService>();

            string tripIdentifier = "BGR-TRP-5JVlaCq0GrlUXk5JBERg";// "BGR-TRP-hch-CTuUik2zTEUVHmSg";//"BGR-TRP-3BYTQo2EOdI6tRF9DieA";

            List<Fluxfvmsrequest> requests = db.Fluxfvmsrequests.Where(x => x.WebServiceName == "ReceiveFishingActivitiesReport"
                                                                            //&& x.RequestDateTime >= dateFrom
                                                                            //&& x.RequestDateTime < dateTo
                                                                            && x.RequestContent.Contains(tripIdentifier)
                                                                      ).OrderBy(x => x.RequestDateTime).ToList();

            foreach (var request in requests)
            {
                var report = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(request.RequestContent);
                service.ReceiveFishingActivitiesReport(report);
                //bool result = await receiverService.ReportFishingActivities(report);
            }
        }

        private static async Task TestPublishFAReportEndpoint(IServiceProvider serviceProvider)
        {
            ISecureHttpClient secureHttpClient = serviceProvider.GetService<ISecureHttpClient>();

            var request = new FLUXFAReportMessageType
            {
                FLUXReportDocument = new FLUXReportDocumentType
                {
                    CreationDateTime = DateTime.Now,
                    ID = new IDType[] { new IDType { schemeID = "UUID", Value = Guid.NewGuid().ToString() } },
                    PurposeCode = new CodeType { listID = "feagea", Value = "9" },
                    OwnerFLUXParty = new FLUXPartyType
                    {
                        ID = new IDType[] { new IDType { schemeID = "343", Value = "3431" } }
                    }
                },
                FAReportDocument = new FAReportDocumentType[]
                {
                    new FAReportDocumentType
                    {
                        AcceptanceDateTime = DateTime.Now,
                        SpecifiedVesselTransportMeans = new VesselTransportMeansType
                        {
                            ID = new IDType[] { new IDType { schemeID = "", Value = "" } },
                            SpeedMeasure = new MeasureType { unitCode = "gaegea", Value = 343 }
                        }
                    }
                }
            };

            await secureHttpClient.SendAsync<FLUXFAReportMessageType>("http://localhost:5000/api/Integration/FishingActivities/PublishFAReport", request);
        }

        private static Task<bool> ACDRTest(this IServiceProvider serviceProvider)
        {
            IAggregatedCatchReportService fluxAcdrService = serviceProvider.GetRequiredService<IAggregatedCatchReportService>();
            return fluxAcdrService.ReportAggregatedCatches();
        }

        private static Task<bool> VesselReportTest(this IServiceProvider serviceProvider, List<int> shipIds)
        {
            IFluxVesselDomainReceiverService fluxVesselService = serviceProvider.GetRequiredService<IFluxVesselDomainReceiverService>();
            IShipsRegisterService vesselService = serviceProvider.GetRequiredService<IShipsRegisterService>();
            IVesselToFluxVesselReportMapper mapper = serviceProvider.GetRequiredService<IVesselToFluxVesselReportMapper>();

            FLUXReportVesselInformationType vesselInformation;

            if (shipIds.Count == 1)
            {
                ShipRegisterEditDTO ship = vesselService.GetShip(shipIds[0]);
                vesselInformation = mapper.MapVesselToFluxSubVcd(ship, ReportPurposeCodes.Original);
            }
            else
            {
                List<ShipRegisterEditDTO> ships = new List<ShipRegisterEditDTO>();
                foreach (int shipId in shipIds)
                {
                    ships.Add(vesselService.GetShip(shipId));
                }

                ships = ships.OrderBy(x => x.EventDate).ToList();
                vesselInformation = mapper.MapVesselToFluxSubVcd(ships, ReportPurposeCodes.Original);
            }

            return fluxVesselService.ReportVesselChange(vesselInformation);
        }

        private static Task<bool> FirstSaleReportTest(this IServiceProvider serviceProvider, int pageId, ReportPurposeCodes purpose)
        {
            IARADbContext db = serviceProvider.GetService<IARADbContext>();
            IFluxSalesDomainReceiverService fluxSalesDomainReceiverService = serviceProvider.GetRequiredService<IFluxSalesDomainReceiverService>();
            ISalesReportMapper salesReportMapper = serviceProvider.GetRequiredService<ISalesReportMapper>();

            Guid referenceId = (from page in db.FirstSaleLogBookPages
                                where page.Id == pageId
                                select page.FluxIdentifier).First();

            FLUXSalesReportMessageType message = salesReportMapper.MapFirstSalePageToSalesReport(pageId, purpose, referenceId);
            return fluxSalesDomainReceiverService.ReportSalesDocument(message);
        }

        private static Task<bool> AdmissionReportTest(this IServiceProvider serviceProvider, int pageId, ReportPurposeCodes purpose)
        {
            IARADbContext db = serviceProvider.GetService<IARADbContext>();
            IFluxSalesDomainReceiverService fluxSalesDomainReceiverService = serviceProvider.GetRequiredService<IFluxSalesDomainReceiverService>();
            ISalesReportMapper salesReportMapper = serviceProvider.GetRequiredService<ISalesReportMapper>();

            Guid referenceId = (from page in db.AdmissionLogBookPages
                                where page.Id == pageId
                                select page.FluxIdentifier).First();

            FLUXSalesReportMessageType message = salesReportMapper.MapAdmissionPageToSalesReport(pageId, purpose, referenceId);
            return fluxSalesDomainReceiverService.ReportSalesDocument(message);
        }

        private static Task<bool> TransportationReportTest(this IServiceProvider serviceProvider, int pageId, ReportPurposeCodes purpose)
        {
            IARADbContext db = serviceProvider.GetService<IARADbContext>();
            IFluxSalesDomainReceiverService fluxSalesDomainReceiverService = serviceProvider.GetRequiredService<IFluxSalesDomainReceiverService>();
            ISalesReportMapper salesReportMapper = serviceProvider.GetRequiredService<ISalesReportMapper>();

            Guid referenceId = (from page in db.TransportationLogBookPages
                                where page.Id == pageId
                                select page.FluxIdentifier).First();

            FLUXSalesReportMessageType message = salesReportMapper.MapTransportPageToSalesReport(pageId, purpose, referenceId);
            return fluxSalesDomainReceiverService.ReportSalesDocument(message);
        }

        private static IServiceProvider GetServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                  .AddJsonFile("appsettings.json", false)
                  .Build();

            services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(configuration);
            services.AddSingleton<ISecureHttpClient, SecureHttpClient>();
            services.AddSingleton<ICryptoHelper, CryptoHelper>();

            CommonInitializer.AddDomainServices(services);
            CommonInitializer.AddManualDomainServices(services);
            SettingsInitializer.AddSettingsModels(services, configuration);
            DatabaseInitializer.AddDbContext(services, ConnectionStrings.Default);

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
