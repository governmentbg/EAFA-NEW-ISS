using System;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Enums;
using IARA.Infrastructure.FSM.Models;
using IARA.Infrastructure.FSM.Utils;
using IARA.Infrastructure.Services.Internal;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = TransitionCodesEnum.UploadApplSuccVer)]
    internal class UploadApplicationWithSucceedVerificationTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with pageCode and uploadedFile as parameters.";

        public UploadApplicationWithSucceedVerificationTransition(TransitionContext context)
            : base(context)
        { }

        public override bool CanTransition(int id, FileInfoDTO uploadedFile)
        {
            bool isValid = this.FileHasValidIntegrityAndSignature(id, uploadedFile);

            return isValid;
        }

        public override ApplicationStatusesEnum Action(int id, FileInfoDTO uploadedFile, string statusReason = "")
        {
            PageCodeEnum applicationPageCode = this.GetApplicationPageCode(id);
            Type filesEntityType = FSMUtils.GetApplicationFilesEntityType(applicationPageCode);
            int applicationRegisterId = this.GetApplicationRegisterId(applicationId: id);

            File file = db.AddOrEditFile(uploadedFile, true);

            object applFilesDbSet = typeof(IARADbContext).GetMethod("Set").MakeGenericMethod(filesEntityType).Invoke(db, new object[] { });

            IFileEntity applFile = Activator.CreateInstance(filesEntityType, new object[] { }) as IFileEntity;

            applFile.File = file;
            applFile.FileTypeId = uploadedFile.FileTypeId;
            applFile.RecordId = applicationRegisterId;

            applFilesDbSet.GetType().GetMethod("Add").Invoke(applFilesDbSet, new object[] { applFile });

            base.Action(id, statusReason);

            ApplicationStatusesEnum nextState = this.stateMachine.Act(id, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return nextState;
        }


        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            throw new NotSupportedException(ActionErrorMessage);
        }
    }
}
