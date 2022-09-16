using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.Regix.Mockups;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class MockupForeignPersonCheckService : BaseForeignPersonCheckService
    {
        public MockupForeignPersonCheckService(ConnectionStrings connectionString,
                                         IRegiXClientService regixService,
                                         IRegixConclusionsService conclusionsService,
                                         ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<ForeignIdentityInfoResponseType>> HandleDataResponse(IRegiXClientService regixClientService,
                                                                                          RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> data)
        {
            return GetResponse(data, async (request) =>
            {
                //MOCKUP
                await Task.Delay(RegixChecksMockups.MILISECONDS_DELAY);
                return RegixChecksMockups.FOREIGN_PERSON;
            });
        }

        protected override int UpdateRegiXResponse(int checkId,
                                                   object response,
                                                   RegixResponseStatusEnum responseStatus,
                                                   RegixCheckStatusesEnum errorLevel,
                                                   string errorMessage,
                                                   IARADbContext db)
        {
            errorLevel = errorLevel == RegixCheckStatusesEnum.ERROR ? RegixCheckStatusesEnum.WARN : errorLevel;

            return base.UpdateRegiXResponse(checkId, response, responseStatus, errorLevel, errorMessage, db);
        }
    }
}
