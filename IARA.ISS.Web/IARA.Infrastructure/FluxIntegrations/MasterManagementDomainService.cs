using System;
using System.Threading.Tasks;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class MasterManagementDomainService : BaseService, IMasterManagementDomainService
    {
        public MasterManagementDomainService(IARADbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Изпраща на заявка за данни
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public FLUXMDRReturnMessageType QueryNomenclatureData(FLUXMDRQueryMessageType query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получаване на данни от FLUX модул
        /// </summary>
        /// <param name="response"></param>
        public void ReceiveNomenclatureData(FLUXMDRReturnMessageType response)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReceiveResponse(FLUXResponseMessageType response)
        {
            throw new NotImplementedException();
        }
    }
}
