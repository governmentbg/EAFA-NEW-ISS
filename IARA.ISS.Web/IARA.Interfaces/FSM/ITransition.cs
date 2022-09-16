using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.Interfaces.FSM
{
    public interface ITransition
    {
        ApplicationStatusesEnum NextStatus { get; }

        ApplicationStatusesEnum Action(int id, FileInfoDTO uploadFile, string statusReason = "");
        ApplicationStatusesEnum Action(int id, PaymentDataDTO paymentData, string statusReason = "");
        ApplicationStatusesEnum Action(int id, string statusReason);
        ApplicationStatusesEnum Action(int id, EventisDataDTO eventisData, string statusReason = "");
        ApplicationStatusesEnum Action(int id, string draftContent, List<FileInfoDTO> files, string statusReason = "");

        ApplicationStatusesEnum PostAction(int id, ApplicationStatusesEnum newStatus);
        void PreAction(int id, ApplicationStatusesEnum currentStatus);

        bool CanTransition(int id, bool isTriggeredManually, string statusReason);
        bool CanTransition(int id, FileInfoDTO uploadedFile);
        bool CanTransition(int id, PaymentDataDTO paymentData);
        bool CanTransition(int id, EventisDataDTO eventisData);
    }
}
