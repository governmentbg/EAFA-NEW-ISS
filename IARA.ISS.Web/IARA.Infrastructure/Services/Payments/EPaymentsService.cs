using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.Infrastructure.Services.Payments;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class EPaymentsService : IPaymentsService
    {
        private EGovPaymentsService eGovPaymentsService;
        private EPayPaymentsService ePayPaymentsService;

        public EPaymentsService(IARADbContext dbContext,
                                IApplicationStateMachine stateMachine,
                                IExtendedLogger logger)
        {
            this.eGovPaymentsService = new EGovPaymentsService(dbContext, stateMachine, logger);
            this.ePayPaymentsService = new EPayPaymentsService(dbContext, stateMachine, logger);
        }

        public ApplicationHierarchyTypesEnum MovePaymentStatus(string paymentId, bool isPaymentCanceled, bool isFromEPay)
        {
            int applicationId;

            if (isFromEPay)
            {
                applicationId = int.Parse(paymentId);
                this.ePayPaymentsService.MovePaymentStatus(applicationId, isPaymentCanceled);

                if (isPaymentCanceled)
                {
                    this.ePayPaymentsService.AnnulTicket(applicationId);
                }

                return this.ePayPaymentsService.GetApplicationOrigin(applicationId);
            }
            else
            {
                applicationId = eGovPaymentsService.GetApplicationIdByPaymentRef(paymentId);
                this.eGovPaymentsService.MovePaymentStatus(applicationId, isPaymentCanceled);

                if (isPaymentCanceled)
                {
                    this.eGovPaymentsService.AnnulTicket(applicationId);
                }

                return this.eGovPaymentsService.GetApplicationOrigin(applicationId);
            }
        }
    }
}
