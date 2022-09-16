using System;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.FileInFreeService)]
    internal class FileInFreeServiceTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, statusReason and eventisData as parameters.";

        public FileInFreeServiceTransition(TransitionContext context)
        : base(context)
        { }

        public override bool CanTransition(int id, EventisDataDTO eventisData)
        {
            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);

            return paymentStatus == PaymentStatusesEnum.NotNeeded && eventisData != null && !string.IsNullOrEmpty(eventisData.EventisNumber);
        }

        public override ApplicationStatusesEnum Action(int id, EventisDataDTO eventisData, string statusReason = "")
        {
            this.FillApplicationEventisNumber(id, eventisData);
            return base.Action(id, statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            throw new NotSupportedException(ActionErrorMessage);
        }
    }
}
