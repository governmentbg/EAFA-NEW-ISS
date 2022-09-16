using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.InitEmpReviewAndCorr)]
    internal class InitiateEmpReviewAndCorrectionsTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public InitiateEmpReviewAndCorrectionsTransition(TransitionContext context)
           : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            if (isTriggeredManually)
            {
                return this.HasPassedAtLeastOneNotCriticalRegiXCheck(id);
            }

            string regixChecksResultErrorLevel = this.GetHighestErrorLevel(id);
            ApplicationHierarchyTypesEnum applicationHierarchyType = this.GetApplicationHierarchyType(id);

            return !string.IsNullOrEmpty(regixChecksResultErrorLevel)
                   && regixChecksResultErrorLevel != nameof(RegixCheckStatusesEnum.ERROR)
                   && ((applicationHierarchyType == ApplicationHierarchyTypesEnum.RecreationalFishingTicket
                                && regixChecksResultErrorLevel != nameof(RegixCheckStatusesEnum.NONE)
                        ) || applicationHierarchyType != ApplicationHierarchyTypesEnum.RecreationalFishingTicket
                      );
        }
    }
}
