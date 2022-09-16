using System;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.EnterPaymentData)]
    internal class EnteringPaymentDataTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, paymentType, paymentRefNumber and statusReason as parameters.";

        public EnteringPaymentDataTransition(TransitionContext context)
           : base(context)
        { }

        public override bool CanTransition(int id, PaymentDataDTO paymentData)
        {
            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);

            return paymentStatus == PaymentStatusesEnum.Unpaid && paymentData != null;
        }

        public override ApplicationStatusesEnum Action(int id, PaymentDataDTO paymentData, string statusReason = "")
        {
            this.UpdateApplicationPaymentData(id, paymentData);

            return base.Action(id, statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            throw new NotSupportedException(ActionErrorMessage);
        }
    }
}
