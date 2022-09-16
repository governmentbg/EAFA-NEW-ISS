using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.NoErrFromChecks)]
    internal class NoErrorsExternalRegistriesChecksTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public NoErrorsExternalRegistriesChecksTransition(TransitionContext context)
               : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            string regixChecksResultErrorLevel = this.GetHighestErrorLevel(id);

            return !string.IsNullOrEmpty(regixChecksResultErrorLevel) && regixChecksResultErrorLevel == nameof(RegixCheckStatusesEnum.NONE);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
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
