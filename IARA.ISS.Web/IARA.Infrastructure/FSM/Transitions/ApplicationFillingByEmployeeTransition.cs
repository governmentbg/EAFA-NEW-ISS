using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.ApplFillByEmp)]
    internal class ApplicationFillingByEmployeeTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, draft content, files and statusReason as parameters.";

        public ApplicationFillingByEmployeeTransition(TransitionContext context)
            : base(context)
        { }


        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            string eventisNumber = (from appl in db.Applications
                                    where appl.Id == id
                                    select appl.EventisNum).Single();

            Application application = this.GetApplication(id);

            return application.AssignedUserId != null && !string.IsNullOrEmpty(eventisNumber);
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
