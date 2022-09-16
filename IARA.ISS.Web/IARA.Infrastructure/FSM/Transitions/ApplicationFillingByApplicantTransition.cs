using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Files;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.ApplFillByApplicant)]
    internal class ApplicationFillingByApplicantTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, draft content, files and statusReason as parameters.";

        public ApplicationFillingByApplicantTransition(TransitionContext context)
            : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            return true;
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
