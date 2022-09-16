using IARA.Common.Enums;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.PayExpOfTerm)]
    internal class PaymentExpirationOfCertainTermTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public PaymentExpirationOfCertainTermTransition(TransitionContext context)
             : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            Application application = this.GetApplication(id);

            // TODO

            return isTriggeredManually;
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            return base.Action(id, statusReason);
        }
    }
}
