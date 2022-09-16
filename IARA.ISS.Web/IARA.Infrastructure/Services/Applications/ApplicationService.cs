using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Infrastructure.FSM;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class ApplicationService : Service, IApplicationService
    {
        private readonly IUserService userService;
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IApplicationStateMachine stateMachine;

        public ApplicationService(IARADbContext db,
                                  IUserService userService,
                                  IPersonService personService,
                                  ILegalService legalService,
                                  IApplicationStateMachine stateMachine)
            : base(db)
        {
            this.userService = userService;
            this.personService = personService;
            this.legalService = legalService;
            this.stateMachine = stateMachine;
        }

        public List<ApplicationForChoiceDTO> GetApplicationsForChoice(PageCodeEnum[] pageCodes, int assignedUserId)
        {
            List<string> pageCodeString = pageCodes.Select(x => x.ToString()).ToList();
            List<ApplicationForChoiceDTO> result = (from application in Db.Applications
                                                    join submittedByPerson in Db.Persons on application.SubmittedByPersonId equals submittedByPerson.Id
                                                    join submittedForPerson in Db.Persons on application.SubmittedForPersonId equals submittedForPerson.Id into subPerson
                                                    from subForPerson in subPerson.DefaultIfEmpty()
                                                    join submittedForLegal in Db.Legals on application.SubmittedForLegalId equals submittedForLegal.Id into subLegal
                                                    from subForLegal in subLegal.DefaultIfEmpty()
                                                    join applicationType in Db.NapplicationTypes on application.ApplicationTypeId equals applicationType.Id
                                                    join applicationStatus in Db.NapplicationStatuses on application.ApplicationStatusId equals applicationStatus.Id
                                                    where application.AssignedUserId == assignedUserId
                                                          && applicationStatus.Code == nameof(ApplicationStatusesEnum.CAN_FILL_ADM_ACT)
                                                          && pageCodeString.Contains(applicationType.PageCode)
                                                          && application.IsActive
                                                    select new ApplicationForChoiceDTO
                                                    {
                                                        Id = application.Id,
                                                        Type = applicationType.Name,
                                                        PageCode = Enum.Parse<PageCodeEnum>(applicationType.PageCode),
                                                        EventisNumber = application.EventisNum,
                                                        SubmitDateTime = application.SubmitDateTime,
                                                        SubmittedBy = submittedByPerson.FirstName + " " + submittedByPerson.LastName,
                                                        SubmittedFor = subForPerson != null
                                                                        ? subForPerson.FirstName + " " + subForPerson.LastName
                                                                        : subForLegal != null
                                                                                ? subForLegal.Name
                                                                                : null
                                                    }).ToList();

            return result;
        }

        public List<ApplicationRegiXCheckDTO> GetLatestApplicationChangeHistoryRegiXChecks(int applicationChangeHistoryId)
        {
            int applicationId = (from applHist in Db.ApplicationChangeHistories
                                 where applHist.Id == applicationChangeHistoryId
                                 select applHist.ApplicationId).First();

            int latestApplicationHistoryRegixChecksId = (from applHist in Db.ApplicationChangeHistories
                                                         join regixCheck in Db.ApplicationRegiXchecks on applHist.Id equals regixCheck.ApplicationChangeHistoryId
                                                         where applHist.ApplicationId == applicationId
                                                         orderby applHist.ValidTo descending
                                                         select applHist.Id).FirstOrDefault();

            List<ApplicationRegiXCheckDTO> result = (from regixCheck in Db.ApplicationRegiXchecks
                                                     where regixCheck.ApplicationChangeHistoryId == latestApplicationHistoryRegixChecksId
                                                           && regixCheck.IsActive
                                                     orderby regixCheck.RequestDateTime descending
                                                     select new ApplicationRegiXCheckDTO
                                                     {
                                                         ID = regixCheck.Id,
                                                         ApplicationId = regixCheck.ApplicationId,
                                                         ApplicationChangeHistoryId = regixCheck.ApplicationChangeHistoryId,
                                                         WebServiceName = regixCheck.WebServiceName,
                                                         RequestDateTime = regixCheck.RequestDateTime,
                                                         ResponseStatus = regixCheck.ResponseStatus,
                                                         ResponseDateTime = regixCheck.ResponseDateTime,
                                                         ErrorLevel = regixCheck.ErrorLevel,
                                                         ErrorDescription = regixCheck.ErrorDescription,
                                                         Attempts = regixCheck.Attempts
                                                     }).ToList();

            return result;
        }

        public List<ApplicationTypeDTO> GetApplicationTypesForChoice(ApplicationHierarchyTypesEnum applicationHierarchyType)
        {
            DateTime now = DateTime.Now;

            var results = (from applType in Db.NapplicationTypes
                           join applTypeHierType in Db.NapplicationTypeHierTypes on applType.Id equals applTypeHierType.ApplicationTypeId
                           join applStatusHierType in Db.NapplicationStatusHierarchyTypes on applTypeHierType.ApplicationHierTypeId equals applStatusHierType.Id
                           join applTypeGroup in Db.NapplicationTypeGroups on applType.GroupId equals applTypeGroup.Id into tGroup
                           from typeGroup in tGroup.DefaultIfEmpty()
                           where applType.ValidFrom <= now && applType.ValidTo > now
                                 && (typeGroup == null || (typeGroup.ValidFrom <= now && typeGroup.ValidTo > now))
                                 && applTypeHierType.IsActive
                                 && applStatusHierType.ValidFrom <= now && applStatusHierType.ValidTo > now
                                 && applStatusHierType.Code == applicationHierarchyType.ToString()
                                 && applType.IsEas.HasValue && applType.IsEas.Value
                           orderby applType.OrderNum
                           select new
                           {
                               Id = applType.Id,
                               Code = applType.Code,
                               Name = applType.Name,
                               PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                               GroupId = typeGroup != null ? typeGroup.Id : -(applType.Id),
                               GroupName = typeGroup != null ? typeGroup.Name : "",
                               IsChecked = false,
                               ApplicationOrderNumber = applType.OrderNum,
                               GroupOrderNumber = typeGroup != null && typeGroup.OrderNum != null && typeGroup.OrderNum.HasValue ? typeGroup.OrderNum.Value : short.MaxValue,
                           }).AsEnumerable()
                           .OrderBy(x => x.GroupOrderNumber)
                           .ThenBy(x => x.ApplicationOrderNumber)
                           .Select(x => new ApplicationTypeDTO
                           {
                               Value = x.Id,
                               Code = x.Code,
                               DisplayName = x.Name,
                               PageCode = x.PageCode,
                               GroupId = x.GroupId,
                               GroupName = x.GroupName,
                               IsChecked = x.IsChecked
                           }).ToList();

            return results;
        }

        public ApplicationStatusesEnum GetApplicationCurrentStatusCode(int applicationId)
        {
            string currentStatus = (from appl in Db.Applications.AsSplitQuery().Include(x => x.ApplicationStatus)
                                    where appl.Id == applicationId
                                    select appl.ApplicationStatus.Code).First();

            return Enum.Parse<ApplicationStatusesEnum>(currentStatus);
        }

        public int GetLastGeneratedApplicationFileId(int applicationId)
        {
            string applicationPageCode = (from appl in Db.Applications
                                          join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                          where appl.Id == applicationId
                                          select applType.PageCode).First();

            PageCodeEnum pageCode = Enum.Parse<PageCodeEnum>(applicationPageCode);

            int fileTypeId = Db.NfileTypes
                   .Where(x => x.Code == FileTypeEnum.APPLICATION_PDF.ToString())
                   .Select(x => x.Id)
                   .First();

            int fileId = 0;

            switch (pageCode)
            {
                case PageCodeEnum.SciFi:
                    {
                        fileId = GetRegisterFileId<ScientificPermitRegister, ScientificPermitRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.LE:
                    {
                        fileId = GetRegisterFileId<Legal, LegalFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.PoundnetCommFish:
                case PageCodeEnum.RightToFishThirdCountry:
                case PageCodeEnum.CommFish:
                    {
                        fileId = GetRegisterFileId<PermitRegister, PermitRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.AquaFarmReg:
                    {
                        fileId = GetRegisterFileId<AquacultureFacilityRegister, AquacultureFacilityRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.PoundnetCommFishLic:
                case PageCodeEnum.RightToFishResource:
                case PageCodeEnum.CatchQuataSpecies:
                    {
                        fileId = GetRegisterFileId<PermitLicensesRegister, PermitLicensesRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.TransferFishCap:
                case PageCodeEnum.ReduceFishCap:
                case PageCodeEnum.IncreaseFishCap:
                case PageCodeEnum.CapacityCertDup:
                    {
                        fileId = GetApplicationFileId(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.RegVessel:
                    {
                        fileId = GetRegisterFileId<ShipRegister, ShipRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.CommFishLicense:
                    {
                        fileId = GetRegisterFileId<FishermenRegister, FishermenRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.RegFirstSaleCenter:
                case PageCodeEnum.RegFirstSaleBuyer:
                    {
                        fileId = GetRegisterFileId<BuyerRegister, BuyerRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.ShipRegChange:
                case PageCodeEnum.AquaFarmChange:
                case PageCodeEnum.AquaFarmDereg:
                case PageCodeEnum.DeregShip:
                case PageCodeEnum.ChangeFirstSaleBuyer:
                case PageCodeEnum.ChangeFirstSaleCenter:
                case PageCodeEnum.TermFirstSaleCenter:
                case PageCodeEnum.TermFirstSaleBuyer:
                    {
                        fileId = GetApplicationFileId(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.StatFormAquaFarm:
                case PageCodeEnum.StatFormFishVessel:
                case PageCodeEnum.StatFormRework:
                    {
                        fileId = GetRegisterFileId<StatisticalFormsRegister, StatisticalFormsRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                case PageCodeEnum.DupFirstSaleBuyer:
                case PageCodeEnum.DupFirstSaleCenter:
                case PageCodeEnum.DupCommFish:
                case PageCodeEnum.DupRightToFishThirdCountry:
                case PageCodeEnum.DupPoundnetCommFish:
                case PageCodeEnum.DupRightToFishResource:
                case PageCodeEnum.DupPoundnetCommFishLic:
                case PageCodeEnum.DupCatchQuataSpecies:
                case PageCodeEnum.CompetencyDup:
                    {
                        fileId = GetRegisterFileId<DuplicatesRegister, DuplicatesRegisterFile>(applicationId, fileTypeId);
                    }
                    break;
                default:
                    break;
            }

            return fileId;
        }

        public string GetApplicationAccessCode(int applicationId)
        {
            string accessCode = (from appl in Db.Applications
                                 where appl.Id == applicationId
                                 select appl.AccessCode).First();

            return accessCode;
        }

        public PaymentSummaryDTO GetApplicationPaymentSummary(int applicationId)
        {
            List<PaymentTariffDTO> appliedTariffs = GetApplicationAppliedTariffs(applicationId);

            PaymentSummaryDTO result = new PaymentSummaryDTO
            {
                Tariffs = appliedTariffs
            };

            result.TotalPrice = (from appl in Db.Applications
                                 join applPayment in Db.ApplicationPayments on appl.Id equals applPayment.ApplicationId
                                 where appl.Id == applicationId && applPayment.IsActive
                                 select applPayment.TotalAmountBgn).Single();

            return result;
        }

        public List<PaymentTariffDTO> GetApplicationAppliedTariffs(int applicationId)
        {
            List<PaymentTariffDTO> appliedTariffs = (from appl in Db.Applications
                                                     join applPayment in Db.ApplicationPayments on appl.Id equals applPayment.ApplicationId
                                                     join applPaymentTariff in Db.ApplicationPaymentTariffs on applPayment.Id equals applPaymentTariff.PaymentId
                                                     join tariff in Db.Ntariffs on applPaymentTariff.TariffId equals tariff.Id
                                                     where appl.Id == applicationId && applPayment.IsActive
                                                     select new PaymentTariffDTO
                                                     {
                                                         TariffId = tariff.Id,
                                                         Quantity = applPaymentTariff.Quantity,
                                                         Price = applPaymentTariff.AmountBgn,
                                                         UnitPrice = tariff.Price
                                                     }).ToList();

            return appliedTariffs;
        }

        public ApplicationContentDTO GetApplicationChangeHistoryContent(int applicationChangeHistoryId)
        {
            ApplicationContentDTO result = (from applHist in Db.ApplicationChangeHistories
                                            where applHist.Id == applicationChangeHistoryId
                                            select new ApplicationContentDTO
                                            {
                                                ApplicationId = applHist.ApplicationId,
                                                DraftContent = applHist.ApplicationDraftContents
                                            }).First();

            result.Files = Db.GetFiles(Db.ApplicationChangeHistoryFiles, applicationChangeHistoryId);

            return result;
        }

        public bool IsApplicationPaid(int applicationId)
        {
            bool isPaid = (from appl in Db.Applications
                           join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                           where appl.Id == applicationId
                           select applType.IsPaid).First();

            return isPaid;
        }

        public ApplicationPaymentInformationDTO GetApplicationPaymentInformation(int applicationId)
        {
            ApplicationPaymentInformationDTO result = (from applPayment in Db.ApplicationPayments
                                                       join paymentType in Db.NpaymentTypes on applPayment.PaymentTypeId equals paymentType.Id into payType
                                                       from paymentType in payType.DefaultIfEmpty()
                                                       join paymentStatus in Db.NPaymentStatuses on applPayment.PaymentStatusId equals paymentStatus.Id
                                                       where applPayment.ApplicationId == applicationId && applPayment.IsActive
                                                       select new ApplicationPaymentInformationDTO
                                                       {
                                                           Id = applPayment.Id,
                                                           PaymentDate = applPayment.PaymentDateTime,
                                                           PaymentType = paymentType != null ? paymentType.Name : "",
                                                           PaymentStatus = paymentStatus.Name,
                                                           ReferenceNumber = applPayment.PaymentRefNum,
                                                           LastUpdateDate = applPayment.UpdatedOn ?? applPayment.CreatedOn
                                                       }).Single();

            result.PaymentSummary = GetApplicationPaymentSummary(applicationId);

            return result;
        }

        public Tuple<int, string> AddApplication(int applicationTypeId,
                                                 ApplicationHierarchyTypesEnum applicationHierarchyType,
                                                 int currentUserId,
                                                 ApplicationSubmissionDTO applicationSubmission = null)
        {
            Application application = Db.AddApplication(applicationTypeId, applicationHierarchyType, currentUserId, applicationSubmission);

            Db.SaveChanges();

            application.AccessCode = ApplicationHelper.GenerateAccessCode(application.Id);

            Db.SaveChanges();

            return new Tuple<int, string>(application.Id, application.AccessCode);
        }

        public ApplicationStatusesEnum UpdateEventisNumber(int applicationId, EventisDataDTO eventisData)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, eventisData);

            return newStatus;
        }

        public AssignedApplicationInfoDTO AssignApplicationViaAccessCode(string accessCode, int userId, PageCodeEnum[] pageCodes)
        {
            Application application = (from appl in Db.Applications
                                                      .AsSplitQuery()
                                                      .Include(x => x.ApplicationType)
                                                      .Include(x => x.ApplicationStatus)
                                                      .Include(x => x.ApplicationChangeHistories)
                                                      .Include(x => x.ApplicationStatusHierType)
                                       where appl.AccessCode == accessCode.Trim()
                                       select appl).SingleOrDefault();

            if (application == null || !pageCodes.Contains(Enum.Parse<PageCodeEnum>(application.ApplicationType.PageCode)))
            {
                throw new ArgumentException("Invalid access code");
            }

            bool firstAssignment = application.AssignedUserId == null;

            application.AssignedUserId = userId;

            ApplicationHierarchyTypesEnum applicationHierarchyType = Enum.Parse<ApplicationHierarchyTypesEnum>(application.ApplicationStatusHierType.Code);

            if (firstAssignment && applicationHierarchyType == ApplicationHierarchyTypesEnum.OnPaper)
            {
                this.stateMachine.Act(application.Id, draftContent: null, files: null);
            }
            else
            {
                Db.AddApplicationChangeHistory(application, DateTime.Now);

                Db.SaveChanges();
            }

            return new AssignedApplicationInfoDTO
            {
                Id = application.Id,
                StatusCode = application.ApplicationStatus.Code,
                PageCode = Enum.Parse<PageCodeEnum>(application.ApplicationType.PageCode)
            };
        }

        public ApplicationStatusesEnum UpdateDraftContent(int applicationId, string draftContent, List<FileInfoDTO> files)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, draftContent, files);

            return newStatus;
        }

        public ApplicationStatusesEnum ApplicationAnnulment(int applicationId, string reason)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, ApplicationStatusesEnum.CANCELED_APPL, reason);

            return newStatus;
        }

        public string GetSubmittedByRoleCodeById(int id)
        {
            string code = (from role in Db.NsubmittedByRoles
                           where role.Id == id
                           select role.Code).First();

            return code;
        }

        public int GetSubmittedByRoleId(SubmittedByRolesEnum role)
        {
            int id = (from r in Db.NsubmittedByRoles
                      where r.Code == role.ToString()
                      select r.Id).Single();

            return id;
        }

        public ApplicationSubmittedByDTO GetUserAsSubmittedBy(int userId)
        {
            int personId = (from user in Db.Users
                            where user.Id == userId
                            select user.PersonId).First();

            ApplicationSubmittedByDTO result = new ApplicationSubmittedByDTO
            {
                Person = personService.GetRegixPersonData(personId),
                Addresses = personService.GetAddressRegistrations(personId)
            };

            return result;
        }

        public ApplicationSubmittedByDTO GetApplicationSubmittedBy(int applicationId)
        {
            int? submittedByPersonId = (from appl in Db.Applications
                                        where appl.Id == applicationId
                                        select appl.SubmittedByPersonId).First();

            if (submittedByPersonId.HasValue)
            {
                ApplicationSubmittedByDTO result = new ApplicationSubmittedByDTO
                {
                    Person = personService.GetRegixPersonData(submittedByPersonId.Value),
                    Addresses = personService.GetAddressRegistrations(submittedByPersonId.Value)
                };

                return result;
            }

            return null;
        }

        public ApplicationSubmittedByRegixDataDTO GetApplicationSubmittedByRegixData(int applicationId)
        {
            return GetApplicationSubmittedBy(applicationId);
        }

        public ApplicationSubmittedForDTO GetRegisterSubmittedFor(int applicationId, int? submittedForPersonId, int? submittedForLegalId)
        {
            SubmittedByRolesEnum submittedByRole = (from appl in Db.Applications
                                                    join role in Db.NsubmittedByRoles on appl.SubmittedByPersonRoleId.Value equals role.Id
                                                    where appl.Id == applicationId
                                                    select Enum.Parse<SubmittedByRolesEnum>(role.Code)).First();

            ApplicationSubmittedForDTO result = new ApplicationSubmittedForDTO();

            if (submittedForPersonId.HasValue)
            {
                result = new ApplicationSubmittedForDTO
                {
                    SubmittedByRole = submittedByRole,
                    SubmittedByLetterOfAttorney = GetLetterOfAttorney(applicationId),
                    Person = personService.GetRegixPersonData(submittedForPersonId.Value),
                    Addresses = personService.GetAddressRegistrations(submittedForPersonId.Value),
                };

                if (submittedByRole == SubmittedByRolesEnum.LegalRepresentative || submittedByRole == SubmittedByRolesEnum.PersonalRepresentative)
                {
                    result.SubmittedByLetterOfAttorney = GetLetterOfAttorney(applicationId);
                }
            }
            else if (submittedForLegalId.HasValue)
            {
                result = new ApplicationSubmittedForDTO
                {
                    SubmittedByRole = submittedByRole,
                    Legal = legalService.GetRegixLegalData(submittedForLegalId.Value),
                    Addresses = legalService.GetAddressRegistrations(submittedForLegalId.Value)
                };

                if (submittedByRole == SubmittedByRolesEnum.LegalRepresentative || submittedByRole == SubmittedByRolesEnum.PersonalRepresentative)
                {
                    result.SubmittedByLetterOfAttorney = GetLetterOfAttorney(applicationId);
                }
            }

            return result;
        }

        public ApplicationSubmittedForDTO GetApplicationSubmittedFor(int applicationId)
        {
            return GetApplicationSubmittedFor(applicationId, onlyRegixData: false);
        }

        public ApplicationSubmittedForRegixDataDTO GetApplicationSubmittedForRegixData(int applicationId)
        {
            return GetApplicationSubmittedFor(applicationId, onlyRegixData: true);
        }

        public LetterOfAttorneyDTO GetLetterOfAttorney(int applicationId)
        {
            LetterOfAttorneyDTO result = (from letter in Db.LetterOfAttorneys
                                          join application in Db.Applications on letter.Id equals application.SubmittedByLetterOfAttorneyId
                                          where application.Id == applicationId && letter.IsActive
                                          select new LetterOfAttorneyDTO
                                          {
                                              Id = letter.Id,
                                              LetterNum = letter.LetterNum,
                                              LetterValidFrom = letter.LetterValidFrom,
                                              LetterValidTo = letter.LetterValidTo,
                                              IsUnlimited = letter.IsUnlimited,
                                              NotaryName = letter.NotaryNames
                                          }).SingleOrDefault();

            return result;
        }

        public List<TariffNomenclatureDTO> GetApplicationTypeActiveTariffs(int applicationTypeId, bool onlyCalculated = false)
        {
            DateTime now = DateTime.Now;

            List<TariffNomenclatureDTO> tarrifs = (from applTariff in Db.NapplicationTypeTariffs
                                                   join tariff in Db.Ntariffs on applTariff.TariffId equals tariff.Id
                                                   where applTariff.ApplicationTypeId == applicationTypeId
                                                         && (!onlyCalculated || tariff.IsCalculated == onlyCalculated)
                                                         && applTariff.ValidFrom <= now
                                                         && applTariff.ValidTo > now
                                                   select new TariffNomenclatureDTO
                                                   {
                                                       Value = tariff.Id,
                                                       Code = tariff.Code,
                                                       DisplayName = tariff.Name,
                                                       Description = tariff.Description,
                                                       BasedOnPlea = tariff.BasedOnPlea,
                                                       IsCalculated = tariff.IsCalculated,
                                                       Price = tariff.Price,
                                                       IsActive = true
                                                   }).ToList();
            return tarrifs;
        }

        public PaymentStatusesEnum CalculatePaymentStatus(PaymentDataDTO paymentData, decimal price)
        {
            if (price == 0.0M)
            {
                return PaymentStatusesEnum.NotNeeded;
            }

            if (paymentData == null)
            {
                return PaymentStatusesEnum.Unpaid;
            }

            return PaymentStatusesEnum.PaidOK;
        }

        public PaymentStatusesEnum CalculatePaymentStatus(PaymentDataDTO paymentData, IEnumerable<decimal> prices)
        {
            if (paymentData == null)
            {
                return PaymentStatusesEnum.Unpaid;
            }

            if (prices.All(x => x == 0.0M))
            {
                return PaymentStatusesEnum.NotNeeded;
            }

            return PaymentStatusesEnum.PaidOK;
        }

        public ApplicationStatusesEnum InitiateApplicationCorrections(int applicationId)
        {
            string lastStatusReason = (from appl in Db.Applications
                                       where appl.Id == applicationId
                                       select appl.StatusReason).First();

            ApplicationStatusesEnum newStatus = this.stateMachine.Act(id: applicationId, statusReason: lastStatusReason);

            return newStatus;
        }

        public ApplicationStatusesEnum SendApplicationForUserCorrections(int applicationId, string statusReason)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, ApplicationStatusesEnum.CORR_BY_USR_NEEDED, statusReason);
            return newStatus;
        }

        public ApplicationStatusesEnum ConfirmNoErrorsForApplication(int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId);
            return newStatus;
        }

        public ApplicationStatusesEnum ConfirmApplicationDataIrregularity(int applicationId, string statusReason)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(id: applicationId, toState: ApplicationStatusesEnum.CORR_BY_USR_NEEDED, statusReason);
            return newStatus;
        }

        public ApplicationStatusesEnum ConfirmApplicationDataRegularity(int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId);
            return newStatus;
        }

        public ApplicationStatusesEnum EnterApplicationPaymentData(int applicationId, PaymentDataDTO paymentData)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, paymentData);
            return newStatus;
        }

        public void EnterOfflineTicketApplicationPaymentData(int applicationId, PaymentDataDTO paymentData)
        {
            int oldPaymentId = (from applPayment in Db.ApplicationPayments
                                where applPayment.ApplicationId == applicationId && applPayment.IsActive
                                select applPayment.Id).Single();

            Application application = (from appl in Db.Applications
                                       where appl.Id == applicationId
                                       select appl).First();

            ApplicationPayment applicationPayment = Db.AddOrEditApplicationPayment(application,
                                                                                   new ApplicationPaymentDTO
                                                                                   {
                                                                                       Id = oldPaymentId,
                                                                                       PaymentDateTime = paymentData.PaymentDateTime,
                                                                                       PaymentRefNumber = paymentData.PaymentRefNumber,
                                                                                       PaymentStatus = PaymentStatusesEnum.PaidOK,
                                                                                       PaymentType = paymentData.PaymentType
                                                                                   },
                                                                                   oldPaymentId: oldPaymentId);

            int paymentStatusId = (from payStatus in Db.NPaymentStatuses
                                   where payStatus.Code == nameof(PaymentStatusesEnum.PaidOK)
                                   select payStatus.Id).Single();

            application.PaymentStatusId = paymentStatusId;

            Db.SaveChanges();
        }

        public ApplicationStatusesEnum RenewApplicationPayment(int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId);
            return newStatus;
        }

        public ApplicationStatusesEnum ApplicationPaymentRefusal(int applicationId, string reason)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, ApplicationStatusesEnum.CANCELED_APPL, reason);
            return newStatus;
        }

        public ApplicationStatusesEnum UploadSignedApplication(int applicationId, FileInfoDTO fileInfo)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, fileInfo);
            return newStatus;
        }

        public ApplicationStatusesEnum StartRegixChecks(int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return newStatus;
        }

        public ApplicationStatusesEnum CompleteApplicationFillingByApplicant(int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, ApplicationStatusesEnum.APPL_FOR_SIGN);
            return newStatus;
        }

        public ApplicationStatusesEnum InitiateManualApplicationFillByApplicant(int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.stateMachine.Act(applicationId, ApplicationStatusesEnum.FILL_BY_APPL);
            return newStatus;
        }

        public bool IsApplicationSubmittedByUser(int userId, int applicationId)
        {
            List<int> submittedByUserIds = (from appl in Db.Applications
                                            where appl.Id == applicationId
                                            select appl.SubmittedByUserId).ToList();

            return submittedByUserIds.Contains(userId);
        }

        public bool IsApplicationSubmittedByUserOrPerson(int userId, int applicationId)
        {
            var ids = (from appl in Db.Applications
                       where appl.Id == applicationId
                       select new
                       {
                           appl.SubmittedByUserId,
                           appl.SubmittedByPersonId
                       }).First();

            if (ids.SubmittedByUserId == userId)
            {
                return true;
            }

            List<int> userPersonIds = userService.GetPersonIdsByUserId(userId);
            if (ids.SubmittedByPersonId.HasValue)
            {
                return userPersonIds.Contains(ids.SubmittedByPersonId.Value);
            }

            return false;
        }

        public bool AreApplicationsSubmittedByUser(int userId, List<int> applicationIds)
        {
            List<int> submittedByUserIds = (from appl in Db.Applications
                                            where applicationIds.Contains(appl.Id)
                                            select appl.SubmittedByUserId).ToList();

            return submittedByUserIds.Contains(userId);
        }

        public bool AreApplicationsSubmittedByUserOrPerson(int userId, List<int> applicationIds)
        {
            var ids = (from appl in Db.Applications
                       where applicationIds.Contains(appl.Id)
                       select new
                       {
                           appl.SubmittedByUserId,
                           appl.SubmittedByPersonId
                       }).ToList();

            List<int> userPersonIds = userService.GetPersonIdsByUserId(userId);

            return ids.All(x => x.SubmittedByUserId == userId || (x.SubmittedByPersonId.HasValue && userPersonIds.Contains(x.SubmittedByPersonId.Value)));
        }

        public bool IsApplicationHierarchyType(int applicationId, ApplicationHierarchyTypesEnum type)
        {
            ApplicationHierarchyTypesEnum applType = (from appl in Db.Applications
                                                      join hier in Db.NapplicationStatusHierarchyTypes on appl.ApplicationStatusHierTypeId equals hier.Id
                                                      where appl.Id == applicationId
                                                      select Enum.Parse<ApplicationHierarchyTypesEnum>(hier.Code)).First();

            return applType == type;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private ApplicationSubmittedForDTO GetApplicationSubmittedFor(int applicationId, bool onlyRegixData)
        {
            var ids = (from appl in Db.Applications
                       join role in Db.NsubmittedByRoles on appl.SubmittedByPersonRoleId.Value equals role.Id
                       where appl.Id == applicationId
                       select new
                       {
                           appl.SubmittedForPersonId,
                           appl.SubmittedForLegalId,
                           appl.SubmittedByLetterOfAttorneyId,
                           SubmittedByPersonRoleCode = Enum.Parse<SubmittedByRolesEnum>(role.Code)
                       }).First();

            ApplicationSubmittedForDTO result = new ApplicationSubmittedForDTO();

            if (ids.SubmittedForPersonId.HasValue)
            {
                result = new ApplicationSubmittedForDTO
                {
                    SubmittedByRole = ids.SubmittedByPersonRoleCode,
                    SubmittedByLetterOfAttorney = !onlyRegixData && ids.SubmittedByLetterOfAttorneyId.HasValue ? GetLetterOfAttorney(applicationId) : null,
                    Person = personService.GetRegixPersonData(ids.SubmittedForPersonId.Value),
                    Addresses = personService.GetAddressRegistrations(ids.SubmittedForPersonId.Value),
                };
            }
            else if (ids.SubmittedForLegalId.HasValue)
            {
                result = new ApplicationSubmittedForDTO
                {
                    SubmittedByRole = ids.SubmittedByPersonRoleCode,
                    SubmittedByLetterOfAttorney = !onlyRegixData && ids.SubmittedByLetterOfAttorneyId.HasValue ? GetLetterOfAttorney(applicationId) : null,
                    Legal = legalService.GetRegixLegalData(ids.SubmittedForLegalId.Value),
                    Addresses = legalService.GetAddressRegistrations(ids.SubmittedForLegalId.Value)
                };
            }

            return result;
        }

        private int GetApplicationFileId(int applicationId, int fileTypeId)
        {
            int fileId = (from application in Db.Applications
                          join file in Db.ApplicationFiles on application.Id equals file.RecordId
                          where application.Id == applicationId && file.FileTypeId == fileTypeId && file.IsActive
                          orderby file.FileId
                          select file.FileId).Last();

            return fileId;
        }

        private int GetRegisterFileId<TEntity, TFileEntity>(int? applicationId, int fileTypeId)
          where TFileEntity : class, IFileEntity<TEntity>, ISoftDeletable
          where TEntity : class, IBaseApplicationRegisterEntity, IApplicationNullableIdentifier
        {
            int fileId = (from record in Db.Set<TEntity>()
                          join file in Db.Set<TFileEntity>() on record.Id equals file.RecordId
                          join application in Db.Applications on record.ApplicationId equals application.Id
                          where application.Id == applicationId && file.FileTypeId == fileTypeId && file.IsActive
                          select file.FileId).First();

            return fileId;
        }

        private int GetRegisterFileId<TEntity, TFileEntity>(int applicationId, int fileTypeId)
            where TFileEntity : class, IFileEntity<TEntity>, ISoftDeletable
            where TEntity : class, IBaseApplicationRegisterEntity, IApplicationIdentifier
        {
            int fileId = (from record in Db.Set<TEntity>()
                          join file in Db.Set<TFileEntity>() on record.Id equals file.RecordId
                          join application in Db.Applications on record.ApplicationId equals application.Id
                          where application.Id == applicationId && file.FileTypeId == fileTypeId && file.IsActive
                          select file.FileId).First();

            return fileId;
        }
    }
}
