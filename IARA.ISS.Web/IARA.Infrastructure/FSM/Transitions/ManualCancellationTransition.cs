using System;
using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.ManualCancel)]
    internal class ManualCancellationTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, statusReason and isTriggeredManually as parameters.";

        public ManualCancellationTransition(TransitionContext context)
            : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            return isTriggeredManually && !String.IsNullOrEmpty(statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason)
        {
            return base.Action(id, statusReason);
        }
    }
}
