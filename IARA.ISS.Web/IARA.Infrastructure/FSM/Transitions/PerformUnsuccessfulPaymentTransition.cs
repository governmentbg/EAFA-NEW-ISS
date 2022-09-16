using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.PerformUnsuccPayment)]
    internal class PerformUnsuccessfulPaymentTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public PerformUnsuccessfulPaymentTransition(TransitionContext context)
             : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);

            return paymentStatus == PaymentStatusesEnum.Unpaid && !string.IsNullOrEmpty(statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            this.UpdateApplicationPaymentStatus(id, PaymentStatusesEnum.PaymentFail);
            return base.Action(id, statusReason);
        }
    }
}
