using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.RenewApplFill)]
    internal class RenewApplicationFillingTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public RenewApplicationFillingTransition(TransitionContext context)
             : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            return true;
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            return base.Action(id, statusReason);
        }
    }
}
