using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.NoErrConfirmByEmpFiledIn)]
    internal class NoErrorsConfirmedByEmployeeFiledInApplTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, statusReason and isTriggeredManually as parameters.";

        public NoErrorsConfirmedByEmployeeFiledInApplTransition(TransitionContext context)
               : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);
            string eventisNumber = this.GetApplicationEventisNumber(id);

            return !string.IsNullOrEmpty(eventisNumber)
                   && (paymentStatus == PaymentStatusesEnum.NotNeeded || paymentStatus == PaymentStatusesEnum.PaidOK);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason)
        {
            return base.Action(id, statusReason);
        }
    }
}
