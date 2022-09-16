using System;
using System.Threading;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;
using System.Linq;
using IARA.Security;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.Infrastructure.Services.Payments
{
    public abstract class BasePaymentService : BaseService
    {
        protected readonly IApplicationStateMachine stateMachine;
        protected readonly IExtendedLogger logger;

        protected BasePaymentService(IARADbContext dbContext, IApplicationStateMachine stateMachine, IExtendedLogger logger)
            : base(dbContext)
        {
            this.stateMachine = stateMachine;
            this.logger = logger;
        }

        public ApplicationHierarchyTypesEnum GetApplicationOrigin(int applicationId)
        {
            string applicationHierarchyType = (from appl in Db.Applications
                                               join applHierType in Db.NapplicationStatusHierarchyTypes
                                                    on appl.ApplicationStatusHierTypeId equals applHierType.Id
                                               where appl.Id == applicationId
                                               select applHierType.Code).Single();

            return Enum.Parse<ApplicationHierarchyTypesEnum>(applicationHierarchyType);
        }

        public bool MovePaymentStatus(int applicationId, bool isPaymentCanceled)
        {
            var statuses = GetApplicationStatus(applicationId);

            if (!isPaymentCanceled && statuses.PaymentStatus == PaymentStatusesEnum.Unpaid && statuses.ApplicationStatus != ApplicationStatusesEnum.PAYMENT_PROCESSING)
            {
                MarkPaymentAsProcessing(applicationId);

                return true;
            }
            else if (isPaymentCanceled && statuses.PaymentStatus != PaymentStatusesEnum.PaidOK && statuses.ApplicationStatus != ApplicationStatusesEnum.PAYMENT_ANNUL)
            {
                MarkPaymentAsAnnuled(applicationId, ApplicationStatusesEnum.PAYMENT_ANNUL.ToString());

                return true;
            }

            return false;
        }

        protected void MarkPaymentAsProcessing(int applicationId)
        {
            this.stateMachine.Act(applicationId, ApplicationStatusesEnum.PAYMENT_PROCESSING);
        }

        protected void MarkPaymentAsPaid(int applicationId, string paymentRefNumber, string status, DateTime changeTime)
        {
            this.stateMachine.Act(
                 applicationId,
                 new PaymentDataDTO
                 {
                     PaymentRefNumber = paymentRefNumber,
                     PaymentDateTime = changeTime
                 },
                 null,
                 string.Format(AppResources.statusInPayEGovBG, status)
             );
        }

        protected void MarkPaymentAsAnnuled(int applicationId, string status)
        {
            string statusReason = string.Format(AppResources.statusInPayEGovBG, status);
            this.stateMachine.Act(applicationId, ApplicationStatusesEnum.PAYMENT_ANNUL, statusReason);
        }

        public void AnnulTicket(int applicationId)
        {
            var ticket = (from a in Db.Applications
                          join t in Db.FishingTickets on a.Id equals t.ApplicationId
                          where a.Id == applicationId
                          select t).FirstOrDefault();

            DateTime now = DateTime.Now;

            ticket.TicketStatusId = Db.NticketStatuses
                .Where(x => x.Code == TicketStatusEnum.CANCELED.ToString()
                         && x.ValidFrom <= now
                         && x.ValidTo >= now)
                .Select(x => x.Id)
                .Single();

            Db.SaveChanges();
        }

        protected (string Name, string EgnLnch, IdentifierTypeEnum IdentifierType) GetCurrentUserInfo()
        {
            DateTime now = DateTime.Now;
            int currentUserId = Thread.CurrentPrincipal.GetUserId() ?? DefaultConstants.SYSTEM_USER_ID;

            var userInfo = (from user in Db.Users
                            join userPerson in Db.Persons on user.PersonId equals userPerson.Id
                            join person in Db.Persons on new { userPerson.EgnLnc, userPerson.IdentifierType }
                            equals new { person.EgnLnc, person.IdentifierType }
                            where user.Id == currentUserId && person.ValidFrom <= now && person.ValidTo > now
                            select new
                            {
                                Name = $"{person.FirstName} {person.LastName}",
                                EgnLnch = person.EgnLnc,
                                IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                            }).Single();

            return (userInfo.Name, userInfo.EgnLnch, userInfo.IdentifierType);
        }

        protected (PaymentStatusesEnum PaymentStatus, ApplicationStatusesEnum ApplicationStatus) GetApplicationStatus(int applicationId)
        {
            var result = (from application in Db.Applications
                          join paymentStatus in Db.NPaymentStatuses on application.PaymentStatusId equals paymentStatus.Id
                          join applicationStatus in Db.NapplicationStatuses on application.ApplicationStatusId equals applicationStatus.Id
                          where application.Id == applicationId
                          select new
                          {
                              Paymentstatus = Enum.Parse<PaymentStatusesEnum>(paymentStatus.Code),
                              ApplicationStatus = Enum.Parse<ApplicationStatusesEnum>(applicationStatus.Code)
                          }).First();

            return (result.Paymentstatus, result.ApplicationStatus);
        }

        protected (decimal Amount, string Description) GetPaymentDetails(int applicationId)
        {
            var result = (from appl in Db.Applications
                          join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                          join applPayment in Db.ApplicationPayments on appl.Id equals applPayment.ApplicationId
                          where appl.Id == applicationId && applPayment.IsActive
                          select new
                          {
                              applPayment.TotalAmountBgn,
                              ApplicationType = applType.Name
                          }).First();

            return (result.TotalAmountBgn, string.Format(AppResources.payForApplicationType, result.ApplicationType));
        }

    }
}
