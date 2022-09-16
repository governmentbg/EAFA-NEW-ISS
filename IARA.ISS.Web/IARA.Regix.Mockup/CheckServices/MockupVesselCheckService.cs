using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DataAccess;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended.Models.IAMA;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class MockupVesselCheckService : BaseVesselCheckService
    {
        public MockupVesselCheckService(ConnectionStrings connectionString,
                                  IRegiXClientService regixService,
                                  IRegixConclusionsService regixConclusionsService,
                                  ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override RegixCheckStatus CompareApplicationData(VesselContext response, VesselContext compare, BaseContextData context)
        {
            return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, ErrorResources.msgVesselNotFoundInRegister);
        }

        protected override Task<ResponseType<ShipsResponse>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<VesselRequest, VesselContext> request)
        {
            var response = new ShipsResponse()
            {
                Ships = new GovReportShip[]
                 {
                    new GovReportShip
                    {
                        Characteristics = new ShipCharacteristics
                        {
                            BodyNumber = "4134312",
                            BT = 34,
                            BTSpecified = true,
                            Displacement = 45,
                            DisplacementSpecified = true,
                            DW = 50,
                            DWSpecified = true,
                            Engines = new Engine[]
                            {
                                new Engine
                                {
                                    EngineNumber = "4234234",
                                    EnginteType = "main",
                                    Power = 52,
                                    PowerSpecified = true,
                                    SystemModification = "none"
                                }
                            },
                            MaxLength = 50,
                            MaxLengthSpecified = true,
                            MaxWidth = 20,
                            MaxWidthSpecified = true,
                            NT = 20,
                            NTSpecified = true,
                            NumberOfEngines = 1,
                            NumberOfEnginesSpecified = true,
                            SumEnginePower = 52,
                            ShipboardHeight = 15,
                            Waterplane = 5,
                            WaterplaneSpecified = true,
                            ShipboardHeightSpecified = true,
                            SumEnginePowerSpecified = true,
                            EnginesFuel = "diesel"
                        }
                    }
                 }
            };

            return Task.FromResult(new ResponseType<ShipsResponse>
            {
                Response = response,
                Type = RegixResponseStatusEnum.Cache
            });
        }

        protected override VesselContext MapToLocalData(ShipsResponse response, RegixContextData<VesselRequest, VesselContext> requestContext)
        {
            return RegixDataMappers.MapRegixVessel(response.Ships[0]);
        }

        protected override int UpdateRegiXResponse(int checkId, object response, RegixResponseStatusEnum responseStatus, RegixCheckStatusesEnum errorLevel, string errorMessage, IARADbContext db)
        {
            errorLevel = errorLevel == RegixCheckStatusesEnum.ERROR ? RegixCheckStatusesEnum.WARN : errorLevel;

            return base.UpdateRegiXResponse(checkId, response, responseStatus, errorLevel, errorMessage, db);
        }
    }
}
