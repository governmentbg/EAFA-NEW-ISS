using IARA.Flux.Models;

namespace IARA.Interfaces.FluxIntegrations
{
    public interface IMasterManagementDomainService : IBaseFluxService
    {
        /// <summary>
        /// Изпраща на заявка за данни
        /// </summary>
        /// <returns></returns>
        FLUXMDRReturnMessageType QueryNomenclatureData(FLUXMDRQueryMessageType query);

        /// <summary>
        /// Получаване на данни от FLUX модул
        /// </summary>
        /// <param name="response"></param>
        void ReceiveNomenclatureData(FLUXMDRReturnMessageType response);
    }
}
