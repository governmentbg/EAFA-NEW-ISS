using System;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.PaperApplRecordAdd)]
    internal class PaperApplicationRecordAddTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, eventisData and statusReason as parameters.";

        public PaperApplicationRecordAddTransition(TransitionContext context)
              : base(context)
        { }

        public override bool CanTransition(int id, EventisDataDTO eventisData)
        {
            return eventisData != null && !string.IsNullOrEmpty(eventisData.EventisNumber);
        }

        public override ApplicationStatusesEnum Action(int id, EventisDataDTO eventisData, string statusReason = "")
        {
            this.FillApplicationEventisNumber(id, eventisData);
            return base.Action(id, statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason)
        {
            throw new NotSupportedException(ActionErrorMessage);
        }
    }
}
