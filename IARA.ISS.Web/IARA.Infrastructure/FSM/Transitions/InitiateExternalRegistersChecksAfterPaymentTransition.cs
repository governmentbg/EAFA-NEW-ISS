using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.InitExtRegsCheckAfterPaid)]
    internal class InitiateExternalRegistersChecksAfterPaymentTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public InitiateExternalRegistersChecksAfterPaymentTransition(TransitionContext context)
         : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);

            ApplicationHierarchyTypesEnum applicationHierarchyType = this.GetApplicationHierarchyType(id);

            bool hasRegixChecks = HasApplicationRegiXChecks(id);
            bool isFishingTickets = applicationHierarchyType == ApplicationHierarchyTypesEnum.RecreationalFishingTicket;

            return (isTriggeredManually || (isFishingTickets && !hasRegixChecks))
                    && (paymentStatus == PaymentStatusesEnum.PaidOK || paymentStatus == PaymentStatusesEnum.NotNeeded)
                    && (isFishingTickets || this.HasRegisterEntry(id, RecordTypesEnum.Application));
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            ApplicationHierarchyTypesEnum applicationHierarchyType = this.GetApplicationHierarchyType(id);

            if (applicationHierarchyType == ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
            {
                UpdateTicketRegisterApplicationStatus(id, TicketStatusEnum.ISSUED);
            }

            return base.Action(id, statusReason);
        }
    }
}
