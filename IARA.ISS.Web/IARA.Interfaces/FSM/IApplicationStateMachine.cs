using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.Interfaces.FSM
{
    public interface IApplicationStateMachine : IDisposable
    {
        ApplicationStatusesEnum Act(int id, ApplicationStatusesEnum? toState = null, string statusReason = null);
        ApplicationStatusesEnum Act(int id, FileInfoDTO uploadedFile, ApplicationStatusesEnum? toState = null, string statusReason = null);
        ApplicationStatusesEnum Act(int id, PaymentDataDTO paymentData, ApplicationStatusesEnum? toState = null, string statusReason = null);
        ApplicationStatusesEnum Act(int id, EventisDataDTO eventisData, ApplicationStatusesEnum? toState = null, string statusReason = null);
        ApplicationStatusesEnum Act(int id, string draftContent, List<FileInfoDTO> files, ApplicationStatusesEnum? toState = null, string statusReason = null);
    }
}
