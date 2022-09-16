using System;
using System.Collections.Generic;
using IARA.Common;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.EstDataIrregularity)]
    internal class EstablishDataIrregularityTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public EstablishDataIrregularityTransition(TransitionContext context)
           : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            return isTriggeredManually && this.HasNoApplicationPaymentObligations(id) && !string.IsNullOrEmpty(statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            return base.Action(id, statusReason);
        }
    }
}
