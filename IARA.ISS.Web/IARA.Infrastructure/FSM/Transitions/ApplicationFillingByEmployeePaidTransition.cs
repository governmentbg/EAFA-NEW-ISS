using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Files;
using System.Collections.Generic;
using System;
using System.Linq;
using IARA.DomainModels.DTOModels.FSM;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;
using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.ApplFillByEmpPaid)]
    internal class ApplicationFillingByEmployeePaidTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, draft content, files and statusReason as parameters.";

        public ApplicationFillingByEmployeePaidTransition(TransitionContext context)
            : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            bool isPaid = this.GetApplicationTypeIsPaid(id);

            if (isPaid == false)
            {
                return false;
            }

            string eventisNumber = (from appl in db.Applications
                                    where appl.Id == id
                                    select appl.EventisNum).Single();

            Application application = this.GetApplication(id);

            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(id);

            return application.AssignedUserId != null
                    && !string.IsNullOrEmpty(eventisNumber)
                    && paymentStatus != PaymentStatusesEnum.PaidOK 
                    && paymentStatus != PaymentStatusesEnum.NotNeeded;
        }

        public override ApplicationStatusesEnum Action(int id, string draftContent, List<FileInfoDTO> files, string statusReason = "")
        {
            this.UpdateDraftContentAndFiles(id, draftContent, files);

            return base.Action(id, statusReason);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            throw new NotSupportedException(ActionErrorMessage);
        }
    }
}
