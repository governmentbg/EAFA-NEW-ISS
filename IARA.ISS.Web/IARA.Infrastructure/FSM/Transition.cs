using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Infrastructure.FSM.Models;
using IARA.Infrastructure.FSM.Utils;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;
using IARA.RegixIntegration.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TL.Signer;

namespace IARA.Infrastructure.FSM
{
    internal abstract class Transition : ITransition
    {
        protected IARADbContext db;
        protected IServiceProvider serviceProvider;
        protected IApplicationStateMachine stateMachine;
        private static readonly PageCodeEnum[] ChangeOfCircumstancesPageCodes = new PageCodeEnum[]
        {
            PageCodeEnum.ShipRegChange,
            PageCodeEnum.DeregShip,
            PageCodeEnum.AquaFarmChange,
            PageCodeEnum.AquaFarmDereg,
            PageCodeEnum.ChangeFirstSaleBuyer,
            PageCodeEnum.ChangeFirstSaleCenter,
            PageCodeEnum.TermFirstSaleBuyer,
            PageCodeEnum.TermFirstSaleCenter
        };

        protected Transition(TransitionContext context)
        {
            this.db = context.Db;
            this.stateMachine = context.StateMachine;
            this.NextStatus = Enum.Parse<ApplicationStatusesEnum>(context.ToState);
            this.serviceProvider = context.ServiceProvider;
        }

        public ApplicationStatusesEnum NextStatus { get; protected set; }
        protected abstract string ActionErrorMessage { get; }

        public virtual ApplicationStatusesEnum Action(int id, string statusReason)
        {
            NapplicationStatus newStatus = this.ChangeStatus(id, statusReason);
            return Enum.Parse<ApplicationStatusesEnum>(newStatus.Code);
        }

        public virtual ApplicationStatusesEnum Action(int id, FileInfoDTO uploadFile, string statusReason = "")
        {
            throw new NotImplementedException(ActionErrorMessage);
        }

        public virtual ApplicationStatusesEnum Action(int id, EventisDataDTO eventisData, string statusReason = "")
        {
            throw new NotImplementedException(ActionErrorMessage);
        }

        public virtual ApplicationStatusesEnum Action(int id, PaymentDataDTO paymentData, string statusReason = "")
        {
            throw new NotImplementedException(ActionErrorMessage);
        }

        public virtual ApplicationStatusesEnum Action(int id, string draftContent, List<FileInfoDTO> files, string statusReason = "")
        {
            throw new NotImplementedException(ActionErrorMessage);
        }

        public virtual bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            return false;
        }

        public virtual bool CanTransition(int id, FileInfoDTO uploadedFile)
        {
            return false;
        }

        public virtual bool CanTransition(int id, EventisDataDTO eventisData)
        {
            return false;
        }

        public virtual bool CanTransition(int id, PaymentDataDTO paymentData)
        {
            return false;
        }

        public ApplicationStatusesEnum PostAction(int id, ApplicationStatusesEnum newStatus)
        {
            if (newStatus == ApplicationStatusesEnum.EXT_CHK_STARTED)
            {
                PageCodeEnum applicationPageCode = this.GetApplicationPageCode(id);
                RegixCheckUtils.EnqueueApplicationChecks(this.serviceProvider, id, applicationPageCode).Wait();
            }

            return newStatus;
        }

        public virtual void PreAction(int id, ApplicationStatusesEnum currentStatus)
        {
            //Base method does nothing
        }

        protected bool FileHasValidIntegrityAndSignature(int applicationId, FileInfoDTO uploadedFile)
        {
            try
            {
                var now = DateTime.Now;

                var applicationData = (from appl in db.Applications
                                       join applHistory in db.ApplicationChangeHistories on appl.Id equals applHistory.ApplicationId
                                       join person in db.Persons on appl.SubmittedByPersonId equals person.Id
                                       where appl.Id == applicationId && applHistory.ValidFrom <= now && applHistory.ValidTo >= now
                                       select new
                                       {
                                           PersonIdentifier = person.EgnLnc,
                                           ApplicationHistoryId = applHistory.Id
                                       }).First();

                IPdfSignatureValidator pdfValidator = this.serviceProvider.GetRequiredService<IPdfSignatureValidator>();

                MemoryStream memStream = new MemoryStream();
                uploadedFile.File.CopyTo(memStream);
                PdfDocumentMetadata documentMetadata = new PdfDocumentMetadata(applicationId, applicationData.ApplicationHistoryId, applicationData.PersonIdentifier);

                bool isValid = pdfValidator.ValidateServerSignature(memStream, "IARA", out List<Error> errors, documentMetadata);
                isValid &= pdfValidator.ValidateClientSignature(memStream, applicationData.PersonIdentifier);

                if (!isValid)
                {
                    var metavaluesMissmatch = errors.Where(x => x.Type == ErrorTypes.MetadataValues);

                    if (metavaluesMissmatch.Any())
                    {
                        List<string> messages = new List<string>();

                        foreach (var propertyName in metavaluesMissmatch.Select(x => x.KeyName))
                        {
                            switch (propertyName)
                            {
                                case nameof(PdfDocumentMetadata.ApplicationId):
                                    messages.Add(ErrorResources.msgInvalidApplicationID);
                                    break;
                                case nameof(PdfDocumentMetadata.PersonIdentifier):
                                    messages.Add(ErrorResources.msgInvalidSubmitter);
                                    break;
                                case nameof(PdfDocumentMetadata.ApplicationHistoryId):
                                    messages.Add(ErrorResources.msgInvalidApplicationVersion);
                                    break;
                                default:
                                    break;
                            }
                        }

                        throw new ApplicationFileInvalidException(messages);
                    }
                    else
                    {
                        if (errors.Any())
                        {
                            throw new ApplicationFileInvalidException(errors.Select(x => x.ErrorMessage));
                        }
                        else
                        {
                            throw new ApplicationFileInvalidException(new List<string> { ErrorResources.msgUnsignedFile });
                        }
                    }
                }

                return isValid;
            }
            catch (ApplicationFileInvalidException)
            {
                throw;
            }
            catch (Exception ex)
            {
                IExtendedLogger logger = this.serviceProvider.GetRequiredService<IExtendedLogger>();
                logger.LogException(ex);
                return false;
            }
        }

        protected void FillApplicationEventisNumber(int applicationId, EventisDataDTO eventisData)
        {
            Application application = this.GetApplication(applicationId);
            application.EventisNum = eventisData.EventisNumber;
        }

        protected Application GetApplication(int applicationId)
        {
            Application application = (from appl in db.Applications
                                                      .AsSplitQuery()
                                                      .Include(x => x.ApplicationChangeHistories)
                                                      .Include(x => x.ApplicationFiles)
                                       where appl.Id == applicationId
                                       select appl).Single();

            return application;
        }

        protected string GetApplicationEventisNumber(int applicationId)
        {
            string eventisNum = (from appl in db.Applications
                                 where appl.Id == applicationId
                                 select appl.EventisNum).Single();

            return eventisNum;
        }

        protected ApplicationHierarchyTypesEnum GetApplicationHierarchyType(int applicationId)
        {
            ApplicationHierarchyTypesEnum applicationHierarchyType = (from appl in db.Applications
                                                                      join applHierType in db.NapplicationStatusHierarchyTypes
                                                                           on appl.ApplicationStatusHierTypeId equals applHierType.Id
                                                                      where appl.Id == applicationId
                                                                      select Enum.Parse<ApplicationHierarchyTypesEnum>(applHierType.Code)).Single();

            return applicationHierarchyType;
        }

        protected PageCodeEnum GetApplicationPageCode(int applicationId)
        {
            string applicationPageCode = (from appl in db.Applications
                                          join applType in db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                          where appl.Id == applicationId
                                          select applType.PageCode).Single();

            return Enum.Parse<PageCodeEnum>(applicationPageCode);
        }

        protected PaymentStatusesEnum GetApplicationPaymentStatus(int applicationId)
        {
            string paymentStatus = (from appl in db.Applications
                                    join payStatus in db.NPaymentStatuses on appl.PaymentStatusId equals payStatus.Id
                                    where appl.Id == applicationId
                                    select payStatus.Code).Single();

            return Enum.Parse<PaymentStatusesEnum>(paymentStatus);
        }

        protected IIdentityRecord GetApplicationRegisterEntry(int applicationId, RecordTypesEnum recordType)
        {
            DateTime now = DateTime.Now;

            PageCodeEnum pageCode = GetApplicationPageCode(applicationId);
            Type entityType = FSMUtils.GetApplicationEntityType(pageCode);

            Type[] interfaces = entityType.GetInterfaces();

            IIdentityRecord result;

            if (interfaces.Any(x => x == typeof(IApplicationRegisterEntity)))
            {
                var dbSet = GetApplicationRegisterDbSet<IApplicationRegisterEntity>(entityType);

                result = dbSet.Where(x => x.ApplicationId == applicationId
                                    && x.RecordType == recordType.ToString()
                                    && x.IsActive).SingleOrDefault();
            }
            else if (interfaces.Any(x => x == typeof(IApplicationRegisterValidityEntity)))
            {
                var dbSet = GetApplicationRegisterDbSet<IApplicationRegisterValidityEntity>(entityType);

                result = dbSet.Where(x => x.ApplicationId == applicationId
                                    && x.RecordType == recordType.ToString()
                                    && x.ValidFrom <= now
                                    && x.ValidTo > now).SingleOrDefault();
            }
            else if (interfaces.Any(x => x == typeof(INullableApplicationRegisterEntity)))
            {
                var dbSet = GetApplicationRegisterDbSet<INullableApplicationRegisterEntity>(entityType);

                result = dbSet.Where(x => x.ApplicationId.Value == applicationId
                                    && x.RecordType == recordType.ToString()
                                    && x.IsActive).SingleOrDefault();
            }
            else if (interfaces.Any(x => x == typeof(INullableApplicationRegisterValidityEntity)))
            {
                var dbSet = GetApplicationRegisterDbSet<INullableApplicationRegisterValidityEntity>(entityType);

                result = dbSet.Where(x => x.ApplicationId.Value == applicationId
                                    && x.RecordType == recordType.ToString()
                                    && x.ValidFrom <= now
                                    && x.ValidTo > now).SingleOrDefault();
            }
            else if (interfaces.Any(x => x == typeof(IChangeOfCircumstancesEntity)))
            {
                var dbSet = GetApplicationRegisterDbSet<IChangeOfCircumstancesEntity>(entityType);

                result = dbSet.Where(x => x.ApplicationId == applicationId && x.IsActive).SingleOrDefault();
            }
            else
            {
                throw new ArgumentException("Error getting application register entity type");
            }

            return result;
        }

        protected int GetApplicationRegisterId(int applicationId)
        {
            return GetApplicationRegisterEntry(applicationId, RecordTypesEnum.Application).Id;
        }

        protected NapplicationStatus GetApplicationStatus(int applicationStatusId)
        {
            NapplicationStatus currentStatus = (from applStatus in db.NapplicationStatuses
                                                where applStatus.Id == applicationStatusId
                                                select applStatus).Single();

            return currentStatus;
        }

        protected bool GetApplicationTypeIsPaid(int applicationId)
        {
            bool isPaid = (from appl in db.Applications
                           join applType in db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                           where appl.Id == applicationId
                           select applType.IsPaid).Single();

            return isPaid;
        }

        protected string GetHighestErrorLevel(int applicationId)
        {
            //Взима последното history ID, когато заявлението е било в статус: Стартирани Regix проверки
            RegixCheckStatusesEnum errorLevel = (from check in db.ApplicationRegiXchecks
                                                 where check.ApplicationId == applicationId
                                                 && check.ApplicationChangeHistoryId == (from application in db.Applications
                                                                                         join history in db.ApplicationChangeHistories on application.Id equals history.ApplicationId
                                                                                         join status in db.NapplicationStatuses on history.ApplicationStatusId equals status.Id
                                                                                         where application.Id == applicationId
                                                                                            && status.Code == nameof(ApplicationStatusesEnum.EXT_CHK_STARTED)
                                                                                         orderby history.ValidFrom descending
                                                                                         select history.Id).First()
                                                 select check.ErrorLevel)
                                                 .ToList()
                                                 .Select(x => Enum.Parse<RegixCheckStatusesEnum>(x))
                                 .OrderByDescending(x => x)
                                 .FirstOrDefault();

            return errorLevel == RegixCheckStatusesEnum.EMPTY ? null : errorLevel.ToString();
        }

        protected bool HasApplicationRegiXChecks(int applicationId)
        {
            bool result = (from applRegiXCheck in db.ApplicationRegiXchecks
                           where applRegiXCheck.ApplicationId == applicationId
                           select applRegiXCheck).Any();

            return result;
        }

        protected bool HasNoApplicationPaymentObligations(int applicationId)
        {
            bool noPaymentObligations = false;

            PaymentStatusesEnum paymentStatus = this.GetApplicationPaymentStatus(applicationId);

            if (paymentStatus == PaymentStatusesEnum.NotNeeded || paymentStatus == PaymentStatusesEnum.PaidOK)
            {
                noPaymentObligations = true;
            }

            return noPaymentObligations;
        }

        protected bool HasPassedAtLeastOneNotCriticalRegiXCheck(int applicationId)
        {
            DateTime now = DateTime.Now;
            // TODO check must be similar to GetHighestErrorLevel
            bool result = (from applRegiXCheck in db.ApplicationRegiXchecks
                           where applRegiXCheck.ApplicationId == applicationId && applRegiXCheck.ErrorLevel != nameof(RegixCheckStatusesEnum.ERROR)
                           select applRegiXCheck.Id).Any();

            return result;
        }

        protected bool HasRegisterEntry(int applicationId, RecordTypesEnum recordType)
        {
            PageCodeEnum pageCode = GetApplicationPageCode(applicationId);
            if (ChangeOfCircumstancesPageCodes.Contains(pageCode))
            {
                bool result = (from change in db.ApplicationChangeOfCircumstances
                               where change.ApplicationId == applicationId
                                    && change.IsActive
                               select change).Any();

                return result;
            }

            // TODO for change of circumstances cover recordType.Register case

            bool hasRegisterEntry = GetApplicationRegisterEntry(applicationId, recordType) != null;
            return hasRegisterEntry;
        }
        protected ApplicationPayment UpdateApplicationPaymentData(int applicationId, PaymentDataDTO paymentData)
        {
            int oldPaymentId = (from applPayment in db.ApplicationPayments
                                where applPayment.ApplicationId == applicationId && applPayment.IsActive
                                select applPayment.Id).First();


            Application application = this.GetApplication(applicationId);

            ApplicationPayment applicationPayment = db.AddOrEditApplicationPayment(application,
                                                                                   new ApplicationPaymentDTO
                                                                                   {
                                                                                       Id = oldPaymentId,
                                                                                       PaymentDateTime = paymentData.PaymentDateTime,
                                                                                       PaymentRefNumber = paymentData.PaymentRefNumber,
                                                                                       PaymentStatus = PaymentStatusesEnum.PaidOK,
                                                                                       PaymentType = paymentData.PaymentType
                                                                                   },
                                                                                   oldPaymentId: oldPaymentId);

            int paymentStatusId = this.GetPaymentStatusIdByCode(nameof(PaymentStatusesEnum.PaidOK));
            application.PaymentStatusId = paymentStatusId;

            return applicationPayment;
        }

        protected void UpdateApplicationPaymentStatus(int applicationId, PaymentStatusesEnum paymentStatus)
        {
            ApplicationPayment applicationPayment = (from applPayment in db.ApplicationPayments
                                                     where applPayment.ApplicationId == applicationId && applPayment.IsActive
                                                     select applPayment).Single();
            int paymentStatusId = this.GetPaymentStatusIdByCode(paymentStatus.ToString());
            applicationPayment.PaymentStatusId = paymentStatusId;

            Application application = this.GetApplication(applicationId);
            application.PaymentStatusId = paymentStatusId;
        }

        protected void UpdateDraftContentAndFiles(int applicationId, string draftContent, List<FileInfoDTO> files)
        {
            Application application = this.GetApplication(applicationId);
            application.ApplicationDraftContents = draftContent;
            if (files != null && files.Count != 0)
            {
                foreach (FileInfoDTO fileInfo in files)
                {
                    db.AddOrEditFile(application, application.ApplicationFiles, fileInfo);
                }
            }
        }

        protected void UpdateTicketRegisterApplicationStatus(int applicationId, TicketStatusEnum ticketStatus)
        {
            DateTime now = DateTime.Now;

            FishingTicket ticketApplication = (from ticket in db.FishingTickets
                                               where ticket.ApplicationId == applicationId
                                                     && ticket.IsActive
                                               select ticket).Single();

            int ticketStatusId = (from tStatus in db.NticketStatuses
                                  where tStatus.Code == ticketStatus.ToString() && tStatus.ValidFrom <= now && tStatus.ValidTo > now
                                  select tStatus.Id).Single();

            ticketApplication.TicketStatusId = ticketStatusId;
        }
        private void AddApplicationChangeHistory(Application application)
        {
            DateTime now = DateTime.Now;
            db.AddApplicationChangeHistory(application, now);
        }

        private NapplicationStatus ChangeStatus(int id, string statusReason = "")
        {
            Application application = this.GetApplication(id);

            NapplicationStatus newStatus = (from status in this.db.NapplicationStatuses
                                            where status.Code == this.NextStatus.ToString()
                                            select status).Single();

            // TODO check if newStatus has ExpectedRunTime and if so -> set some date for the start or something like that ???

            application.ApplicationStatusId = newStatus.Id;
            application.StatusReason = statusReason;

            this.AddApplicationChangeHistory(application);

            this.db.SaveChanges();

            return newStatus;
        }

        private IQueryable<TEntity> GetApplicationRegisterDbSet<TEntity>(Type entityType)
        {
            var dbSet = typeof(IARADbContext).GetMethod("Set")
                                             .MakeGenericMethod(entityType)
                                             .Invoke(db, new object[] { }) as IQueryable<TEntity>;
            return dbSet;
        }

        private int GetLastApplicationChangeHistoryId(int applicationId, DateTime now)
        {
            int id = (from applHist in db.ApplicationChangeHistories
                      where applHist.ApplicationId == applicationId && applHist.ValidFrom <= now && applHist.ValidTo > now
                      select applHist.Id).Single();

            return id;
        }

        private int GetPaymentStatusIdByCode(string paymentStatusCode)
        {
            int paymentStatusId = (from payStatus in db.NPaymentStatuses
                                   where payStatus.Code == paymentStatusCode
                                   select payStatus.Id).Single();

            return paymentStatusId;
        }
    }
}
