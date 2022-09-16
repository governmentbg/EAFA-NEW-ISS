using System;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.UploadApplFailedVer)]
    internal class UploadApplicationWithFailedVerificationTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with pageCode and uploadedFile as parameters.";

        public UploadApplicationWithFailedVerificationTransition(TransitionContext context)
           : base(context)
        { }

        public override bool CanTransition(int id, FileInfoDTO uploadedFile)
        {
            bool isValid = this.FileHasValidIntegrityAndSignature(id, uploadedFile);

            return !isValid;
        }

        public override ApplicationStatusesEnum Action(int id, FileInfoDTO uploadedFile, string statusReason = "")
        {
            throw new InvalidOperationException(ErrorResources.msgInvalidPDF);
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            throw new NotSupportedException(ActionErrorMessage);
        }
    }
}
