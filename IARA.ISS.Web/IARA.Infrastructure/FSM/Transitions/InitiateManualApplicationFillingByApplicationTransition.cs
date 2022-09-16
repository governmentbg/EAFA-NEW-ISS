using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.GoBackToFillApplByApplicant)]
    internal class InitiateManualApplicationFillingByApplicationTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public InitiateManualApplicationFillingByApplicationTransition(TransitionContext context)
        : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            return isTriggeredManually;
        }
    }
}
