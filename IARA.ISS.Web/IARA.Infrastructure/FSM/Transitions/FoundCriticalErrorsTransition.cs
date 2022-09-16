using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.FoundCriticalErr)]
    internal class FoundCriticalErrorsTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public FoundCriticalErrorsTransition(TransitionContext context)
              : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            string regixChecksResultErrorLevel = this.GetHighestErrorLevel(id);

            return !string.IsNullOrEmpty(regixChecksResultErrorLevel)
                   && regixChecksResultErrorLevel == nameof(RegixCheckStatusesEnum.ERROR)
                   && !string.IsNullOrEmpty(statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            return base.Action(id, statusReason);
        }
    }
}
