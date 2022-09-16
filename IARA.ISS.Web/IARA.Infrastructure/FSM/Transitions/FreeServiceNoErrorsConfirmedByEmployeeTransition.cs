using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.FreeServiceNoErrConfByEmp)]
    internal class FreeServiceNoErrorsConfirmedByEmployeeTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public FreeServiceNoErrorsConfirmedByEmployeeTransition(TransitionContext context)
             : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            bool isPaid = this.GetApplicationTypeIsPaid(id);

            if (isPaid == false)
            {
                return true;
            }
            else
            {
                PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);

                return paymentStatus == PaymentStatusesEnum.NotNeeded;
            }
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason)
        {
            return base.Action(id, statusReason);
        }
    }
}
