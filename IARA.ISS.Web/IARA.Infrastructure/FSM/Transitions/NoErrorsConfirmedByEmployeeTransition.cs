using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.NoErrConfirmByEmp)]
    internal class NoErrorsConfirmedByEmployeeTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, statusReason and isTriggeredManually as parameters.";

        public NoErrorsConfirmedByEmployeeTransition(TransitionContext context)
               : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);
            ApplicationHierarchyTypesEnum applicationHierarchyType = this.GetApplicationHierarchyType(id);

            if (applicationHierarchyType == ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
            {
                return paymentStatus == PaymentStatusesEnum.NotNeeded || paymentStatus == PaymentStatusesEnum.PaidOK;
            }
            else
            {
                string eventisNumber = this.GetApplicationEventisNumber(id);

                return string.IsNullOrEmpty(eventisNumber)
                       && (paymentStatus == PaymentStatusesEnum.NotNeeded
                           || paymentStatus == PaymentStatusesEnum.PaidOK
                           || paymentStatus == PaymentStatusesEnum.Unpaid);
            }
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason)
        {
            ApplicationHierarchyTypesEnum applicationHierarchyType = this.GetApplicationHierarchyType(id);

            if (applicationHierarchyType == ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
            {
                this.UpdateTicketRegisterApplicationStatus(id, TicketStatusEnum.APPROVED);
            }

            return base.Action(id, statusReason);
        }
    }
}
