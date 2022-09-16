using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils.Hashing;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Security;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class ApplicationHelper
    {
        public static Application AddApplication(this IARADbContext db, int applicationTypeId,
                                                 ApplicationHierarchyTypesEnum applicationHierarchyType,
                                                 int currentUserId,
                                                 ApplicationSubmissionDTO applicationSubmission = null)
        {
            DateTime now = DateTime.Now;

            int initialStatusId = (from status in db.NapplicationStatuses
                                   where status.Code == nameof(ApplicationStatusesEnum.INITIAL)
                                      && status.ValidFrom <= now && status.ValidTo > now
                                   select status.Id).Single();

            int applicationStatusHierarchyTypeId = (from statusHierarchy in db.NapplicationStatusHierarchyTypes
                                                    where statusHierarchy.Code == applicationHierarchyType.ToString()
                                                        && statusHierarchy.ValidFrom <= now && statusHierarchy.ValidTo > now
                                                    select statusHierarchy.Id).Single();

            bool isPaid = (from applType in db.NapplicationTypes
                           where applType.Id == applicationTypeId
                           select applType.IsPaid).Single();

            int paymentStatusId = GetPaymentStatusId(db, isPaid ? PaymentStatusesEnum.Unpaid : PaymentStatusesEnum.NotNeeded);

            Application application = new Application
            {
                ApplicationTypeId = applicationTypeId,
                ApplicationStatusHierTypeId = applicationStatusHierarchyTypeId,
                ApplicationStatusId = initialStatusId,
                PaymentStatusId = paymentStatusId,
                SubmitDateTime = now,
                SubmittedByUserId = currentUserId
            };

            // Add ApplicationPayment + isCalculated = false tariffs for paid online/paper applications only
            if (isPaid && applicationHierarchyType != ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
            {
                List<PaymentTariffDTO> notCalculatedPaymentTariffs = (from applTariff in db.NapplicationTypeTariffs
                                                                      join tariff in db.Ntariffs on applTariff.TariffId equals tariff.Id
                                                                      where applTariff.ApplicationTypeId == applicationTypeId
                                                                            && !tariff.IsCalculated
                                                                            && applTariff.ValidFrom <= now
                                                                            && applTariff.ValidTo > now
                                                                      select new PaymentTariffDTO
                                                                      {
                                                                          Quantity = 1,
                                                                          TariffId = tariff.Id,
                                                                          UnitPrice = tariff.Price
                                                                      }).ToList();

                ApplicationPayment applicationPayment = AddOrEditApplicationPayment(db,
                                                                                    application,
                                                                                    new ApplicationPaymentDTO
                                                                                    {
                                                                                        PaymentStatus = PaymentStatusesEnum.Unpaid
                                                                                    },
                                                                                    null);

                foreach (var paymentTariff in notCalculatedPaymentTariffs)
                {
                    AddOrEditApplicationPaymentTariff(db, applicationPayment, paymentTariff, null);
                }
            }

            if (applicationSubmission != null)
            {
                application.ApplicationDraftContents = applicationSubmission.ApplicationDraft;

                if (applicationSubmission.SubmittedForPerson != null)
                {
                    application.SubmittedForPerson = db.AddOrEditPerson(applicationSubmission.SubmittedForPerson,
                                                                        applicationSubmission.SubmittedForAddresses,
                                                                        null,
                                                                        applicationSubmission.SubmittedForPersonPhoto);
                }
                else if (applicationSubmission.SubmittedForLegal != null)
                {
                    application.SubmittedForLegal = db.AddOrEditLegal(
                        new ApplicationRegisterDataDTO
                        {
                            RecordType = RecordTypesEnum.Application,
                            ApplicationId = application.Id
                        },
                        applicationSubmission.SubmittedForLegal,
                        applicationSubmission.SubmittedForAddresses
                    );
                }

                if (applicationSubmission.SubmittedByPerson != null)
                {
                    if (applicationSubmission.SubmittedByPerson.EgnLnc.EgnLnc == applicationSubmission.SubmittedForPerson.EgnLnc.EgnLnc)
                    {
                        application.SubmittedByPerson = application.SubmittedForPerson;
                    }
                    else
                    {
                        application.SubmittedByPerson = db.AddOrEditPerson(applicationSubmission.SubmittedByPerson,
                                                                           applicationSubmission.SubmittedByPersonAddresses,
                                                                           null,
                                                                           applicationSubmission.SubmittedByPersonPhoto);
                    }

                    if (applicationSubmission.SubmittedByLetterOfAttorney != null)
                    {
                        application.SubmittedByLetterOfAttorney = db.AddOrEditLetterOfAttorney(applicationSubmission.SubmittedByLetterOfAttorney);
                    }
                }
            }

            db.Applications.Add(application);

            db.AddApplicationChangeHistory(application, now);

            return application;
        }

        public static string GenerateAccessCode(int applicationId)
        {
            string accessCode = Hasher.Encode(applicationId);
            return accessCode;
        }

        public static ApplicationChangeHistory AddApplicationChangeHistory(this IARADbContext db, Application application, DateTime now)
        {
            ApplicationChangeHistory lastChangeHistory = (from applHist in application.ApplicationChangeHistories
                                                          where applHist.ValidFrom <= now && applHist.ValidTo > now
                                                          select applHist).SingleOrDefault();

            return db.AddApplicationChangeHistory(application, now, lastChangeHistory);
        }

        public static ApplicationChangeHistory AddApplicationChangeHistory(this IARADbContext db, Application application, DateTime now, ApplicationChangeHistory lastChangeHistory = null)
        {
            int currentUserId = Thread.CurrentPrincipal.GetUserId() ?? DefaultConstants.SYSTEM_USER_ID;
            int? territoryUnitId = application.AssignedUserId.HasValue ? (from userInfo in db.UserInfos
                                                                          where userInfo.UserId == application.AssignedUserId.Value
                                                                          select userInfo.TerritoryUnitId).Single()
                                                                       : null;
            application.TerritoryUnitId = territoryUnitId;

            if (lastChangeHistory != null)
            {
                lastChangeHistory.ValidTo = now;
            }

            ApplicationChangeHistory applicationChangeHistory = new ApplicationChangeHistory
            {
                Application = application,
                ValidFrom = now,
                ValidTo = DefaultConstants.MAX_VALID_DATE,
                ApplicationStatusId = application.ApplicationStatusId,
                StatusReason = application.StatusReason,
                AssignedUserId = application.AssignedUserId,
                TerritoryUnitId = territoryUnitId,
                PaymentStatusId = application.PaymentStatusId,
                ApplicationDraftContents = application.ApplicationDraftContents,
                ModifiedByUserId = currentUserId,
                ModifiedDateTime = now
            };

            if (!string.IsNullOrEmpty(application.ApplicationDraftContents))
            {
                List<FileInfoDTO> files = db.GetFiles(db.ApplicationFiles, application.Id);
                foreach (FileInfoDTO file in files)
                {
                    db.AddOrEditFile(applicationChangeHistory, applicationChangeHistory.ApplicationChangeHistoryFiles, file);
                }
            }

            db.ApplicationChangeHistories.Add(applicationChangeHistory);

            return applicationChangeHistory;
        }

        public static void AddOrEditApplicationSubmittedBy(this IARADbContext db, Application application, ApplicationSubmittedByDTO submittedBy)
        {
            db.AddOrEditApplicationSubmittedByRegixData(application, submittedBy);
        }

        public static void AddOrEditApplicationSubmittedByRegixData(this IARADbContext db, Application application, ApplicationSubmittedByRegixDataDTO submittedBy)
        {
            application.SubmittedByPerson = db.AddOrEditPerson(submittedBy.Person, submittedBy.Addresses, application.SubmittedByPersonId);
        }

        public static void AddOrEditApplicationSubmittedFor(this IARADbContext db, Application application, ApplicationSubmittedForDTO submittedFor)
        {
            int submittedByRoleId = (from role in db.NsubmittedByRoles
                                     where role.Code == submittedFor.SubmittedByRole.ToString()
                                     select role.Id).Single();

            db.AddOrEditApplicationSubmittedForRegixData(application, submittedFor, submittedByRoleId);

            if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.PersonalRepresentative || submittedFor.SubmittedByRole == SubmittedByRolesEnum.LegalRepresentative)
            {
                application.SubmittedByLetterOfAttorney = db.AddOrEditLetterOfAttorney(submittedFor.SubmittedByLetterOfAttorney, application.SubmittedByLetterOfAttorneyId);
            }
            else
            {
                application.SubmittedByLetterOfAttorney = db.AddOrEditLetterOfAttorney(null, application.SubmittedByLetterOfAttorneyId);
            }
        }

        public static void AddOrEditApplicationSubmittedForRegixData(this IARADbContext db, Application application, ApplicationSubmittedForRegixDataDTO submittedFor)
        {
            int submittedByRoleId = (from role in db.NsubmittedByRoles
                                     where role.Code == submittedFor.SubmittedByRole.ToString()
                                     select role.Id).Single();

            db.AddOrEditApplicationSubmittedForRegixData(application, submittedFor, submittedByRoleId);
        }

        public static void AddOrEditRegisterSubmittedFor(this IARADbContext db, IApplicationEntity entity, ApplicationSubmittedForDTO submittedFor)
        {
            if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                entity.SubmittedForLegalId = null;
                entity.SubmittedForPerson = db.AddOrEditPerson(submittedFor.Person, submittedFor.Addresses, entity.SubmittedForPersonId);
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.PersonalRepresentative)
            {
                entity.SubmittedForLegalId = null;
                entity.SubmittedForPerson = db.AddOrEditPerson(submittedFor.Person, submittedFor.Addresses, entity.SubmittedForPersonId);
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.LegalOwner)
            {
                entity.SubmittedForPersonId = null;

                entity.SubmittedForLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                {
                    ApplicationId = submittedFor.Legal.Id.HasValue ? entity.ApplicationId : default(int?),
                    RecordType = RecordTypesEnum.Register
                }, submittedFor.Legal, submittedFor.Addresses, entity.SubmittedForLegalId);

                // should we even allow this to be edited in frontend?
                Application application = db.Applications.Single(x => x.Id == entity.ApplicationId);
                application.SubmittedByLetterOfAttorney = db.AddOrEditLetterOfAttorney(submittedFor.SubmittedByLetterOfAttorney, application.SubmittedByLetterOfAttorneyId);
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.LegalRepresentative)
            {
                entity.SubmittedForPersonId = null;

                entity.SubmittedForLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                {
                    ApplicationId = submittedFor.Legal.Id.HasValue ? entity.ApplicationId : default(int?),
                    RecordType = RecordTypesEnum.Register
                }, submittedFor.Legal, submittedFor.Addresses, entity.SubmittedForLegalId);

                // should we even allow this to be edited in frontend?
                Application application = db.Applications.Single(x => x.Id == entity.ApplicationId);
                application.SubmittedByLetterOfAttorney = db.AddOrEditLetterOfAttorney(submittedFor.SubmittedByLetterOfAttorney, application.SubmittedByLetterOfAttorneyId);
            }
        }

        public static LetterOfAttorney AddOrEditLetterOfAttorney(this IARADbContext db, LetterOfAttorneyDTO letter, int? oldLoaId = null)
        {
            LetterOfAttorney result;

            if (oldLoaId.HasValue)
            {
                result = (from loa in db.LetterOfAttorneys
                          where loa.Id == oldLoaId.Value
                          select loa).Single();

                if (letter == null)
                {
                    result.IsActive = false;
                }
                else
                {
                    result.LetterNum = letter.LetterNum;
                    result.LetterValidFrom = letter.LetterValidFrom.Value;
                    result.LetterValidTo = letter.IsUnlimited ? default(DateTime?) : letter.LetterValidTo.Value;
                    result.IsUnlimited = letter.IsUnlimited;
                    result.NotaryNames = letter.NotaryName;
                    result.IsActive = true;
                }
            }
            else
            {
                if (letter == null)
                {
                    return null;
                }

                result = new LetterOfAttorney
                {
                    LetterNum = letter.LetterNum,
                    LetterValidFrom = letter.LetterValidFrom.Value,
                    LetterValidTo = letter.IsUnlimited ? default(DateTime?) : letter.LetterValidTo.Value,
                    IsUnlimited = letter.IsUnlimited,
                    NotaryNames = letter.NotaryName
                };

                db.LetterOfAttorneys.Add(result);
            }

            return result;
        }

        public static ApplicationPayment AddOrEditApplicationPayment(this IARADbContext db,
                                                                     Application application,
                                                                     ApplicationPaymentDTO payment,
                                                                     int? oldPaymentId = null)
        {
            ApplicationPayment result;

            if (oldPaymentId != null)
            {
                result = (from applPayment in db.ApplicationPayments.AsSplitQuery().Include(x => x.Application)
                          where applPayment.Id == oldPaymentId
                          select applPayment).Single();

                if (payment == null)
                {
                    result.IsActive = false;
                }
                else
                {
                    result.PaymentTypeId = payment.PaymentType.HasValue ? GetPaymentTypeId(db, payment.PaymentType.ToString()) : default(int?);
                    result.PaymentDateTime = payment.PaymentDateTime;
                    result.PaymentRefNum = payment.PaymentRefNumber;
                    result.PaymentStatusId = GetPaymentStatusId(db, payment.PaymentStatus.Value);

                    result.Application.PaymentStatusId = result.PaymentStatusId;
                }
            }
            else
            {
                if (payment == null)
                {
                    return null;
                }

                result = new ApplicationPayment
                {
                    PaymentTypeId = payment.PaymentType.HasValue ? GetPaymentTypeId(db, payment.PaymentType.ToString()) : default(int?),
                    PaymentDateTime = payment.PaymentDateTime,
                    PaymentStatusId = GetPaymentStatusId(db, payment.PaymentStatus.Value),
                    TotalAmountBgn = 0,
                    PaymentRefNum = payment.PaymentRefNumber
                };

                application.PaymentStatusId = result.PaymentStatusId;
                application.ApplicationPayment = result;
            }

            return result;
        }

        public static ApplicationPaymentTariff AddOrEditApplicationPaymentTariff(this IARADbContext db,
                                                                                 ApplicationPayment applicationPayment,
                                                                                 PaymentTariffDTO paymentTariff,
                                                                                 int? oldPaymentTariffId)
        {
            ApplicationPaymentTariff applicationPaymentTariff;

            if (oldPaymentTariffId.HasValue)
            {
                applicationPaymentTariff = (from applPaymentTariff in db.ApplicationPaymentTariffs
                                            where applPaymentTariff.Id == oldPaymentTariffId.Value
                                            select applPaymentTariff).First();

                if (paymentTariff == null)
                {
                    applicationPaymentTariff.IsActive = false;
                }
                else
                {
                    applicationPaymentTariff.Quantity = paymentTariff.Quantity;
                    // applicationPaymentTariff.Comments // TODO това ще може ли да се променя отнякъде ???

                    decimal oldAmountBgn = applicationPaymentTariff.AmountBgn;
                    applicationPaymentTariff.AmountBgn = paymentTariff.Quantity * paymentTariff.UnitPrice;

                    applicationPayment.TotalAmountBgn = applicationPayment.TotalAmountBgn - oldAmountBgn + applicationPaymentTariff.AmountBgn;
                }
            }
            else
            {
                if (paymentTariff == null)
                {
                    return null;
                }

                applicationPaymentTariff = new ApplicationPaymentTariff
                {
                    TariffId = paymentTariff.TariffId,
                    AmountBgn = paymentTariff.Quantity * paymentTariff.UnitPrice,
                    Quantity = paymentTariff.Quantity
                    // Comments // TODO ???
                };

                applicationPayment.TotalAmountBgn += applicationPaymentTariff.AmountBgn;
                applicationPayment.ApplicationPaymentTariffs.Add(applicationPaymentTariff);
            }

            return applicationPaymentTariff;
        }

        private static int GetPaymentTypeId(IARADbContext db, string code)
        {
            DateTime now = DateTime.Now;

            int paymentTypeId = (from paymentType in db.NpaymentTypes
                                 where paymentType.Code == code
                                        && paymentType.ValidFrom <= now
                                        && paymentType.ValidTo > now
                                 select paymentType.Id).Single();

            return paymentTypeId;
        }

        private static int GetPaymentStatusId(IARADbContext db, PaymentStatusesEnum paymentStatus)
        {
            DateTime now = DateTime.Now;

            int paymentStatusId = (from status in db.NPaymentStatuses
                                   where status.Code == paymentStatus.ToString()
                                        && status.ValidFrom <= now
                                        && status.ValidTo > now
                                   select status.Id).Single();

            return paymentStatusId;
        }

        private static void AddOrEditApplicationSubmittedForRegixData(this IARADbContext db, Application application, ApplicationSubmittedForRegixDataDTO submittedFor, int submittedByRoleId)
        {
            if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                application.SubmittedByPersonRoleId = submittedByRoleId;
                application.SubmittedForLegalId = null;
                application.SubmittedForPerson = application.SubmittedByPerson;
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.PersonalRepresentative)
            {
                application.SubmittedByPersonRoleId = submittedByRoleId;
                application.SubmittedForLegalId = null;
                application.SubmittedForPerson = db.AddOrEditPerson(submittedFor.Person, submittedFor.Addresses, application.SubmittedForPersonId);
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.LegalOwner)
            {
                application.SubmittedByPersonRoleId = submittedByRoleId;
                application.SubmittedForPersonId = null;
                application.SubmittedForLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                {
                    ApplicationId = application.Id,
                    RecordType = RecordTypesEnum.Application
                }, submittedFor.Legal, submittedFor.Addresses, application.SubmittedForLegalId);
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.LegalRepresentative)
            {
                application.SubmittedByPersonRoleId = submittedByRoleId;
                application.SubmittedForPersonId = null;
                application.SubmittedForLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                {
                    ApplicationId = application.Id,
                    RecordType = RecordTypesEnum.Application
                }, submittedFor.Legal, submittedFor.Addresses, application.SubmittedForLegalId);
            }
        }
    }
}
