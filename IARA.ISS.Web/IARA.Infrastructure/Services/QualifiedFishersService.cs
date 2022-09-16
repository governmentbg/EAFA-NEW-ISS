using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;
using TL.EDelivery;

namespace IARA.Infrastructure.Services
{
    public class QualifiedFishersService : Service, IQualifiedFishersService
    {
        private readonly IApplicationService applicationService;
        private readonly IApplicationStateMachine applicationStateMachine;
        private readonly IPersonService personService;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IDeliveryService deliveryService;
        private readonly IJasperReportExecutionService jasperReportsService;
        private readonly IDuplicatesRegisterService duplicatesRegisterService;

        public QualifiedFishersService(IARADbContext db,
                                       IPersonService personService,
                                       IApplicationService applicationService,
                                       IApplicationStateMachine applicationStateMachine,
                                       IRegixApplicationInterfaceService regixApplicationService,
                                       IDeliveryService deliveryService,
                                       IJasperReportExecutionService jasperReportsService,
                                       IDuplicatesRegisterService duplicatesRegisterService)
                 : base(db)
        {
            this.personService = personService;
            this.applicationService = applicationService;
            this.applicationStateMachine = applicationStateMachine;
            this.regixApplicationService = regixApplicationService;
            this.deliveryService = deliveryService;
            this.jasperReportsService = jasperReportsService;
            this.duplicatesRegisterService = duplicatesRegisterService;
        }

        public IQueryable<QualifiedFisherDTO> GetAll(QualifiedFishersFilters filters)
        {
            IQueryable<QualifiedFisherDTO> result = Enumerable.Empty<QualifiedFisherDTO>().AsQueryable();

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool inactiveOnly = filters?.ShowInactiveRecords ?? false;
                result = GetAllFishers(inactiveOnly);
            }
            else
            {
                if (filters.HasFreeTextSearch())
                {
                    result = GetAllFishersByFreeText(filters);
                }
                else if (filters.HasAnyFilters(true))
                {
                    result = GetAllFishersByFilter(filters);
                }
            }

            return result;
        }

        public QualifiedFisherEditDTO GetRegisterEntry(int id)
        {
            var dbEntry = (from fisher in Db.FishermenRegisters
                           join person in Db.Persons on fisher.PersonId equals person.Id
                           where fisher.Id == id
                           select new
                           {
                               fisher.Id,
                               fisher.ApplicationId,
                               SubmittedForPersonId = fisher.PersonId,
                               fisher.RegistrationNum,
                               fisher.RegistrationDate,
                               Name = person.FirstName + " " + (string.IsNullOrEmpty(person.MiddleName) ? "" : person.MiddleName + " ") + person.LastName,
                               EGN = person.EgnLnc,
                               fisher.HasExamLicense,
                               fisher.ExamTerritoryUnitId,
                               fisher.ExamProtocolNum,
                               fisher.ExamDate,
                               fisher.HasPassedExam,
                               fisher.IsWithMaritimeEducation,
                               fisher.DiplomaNum,
                               fisher.DiplomaGraduationDate,
                               fisher.DiplomaIssuer,
                               fisher.Comments
                           }).First();

            QualifiedFisherEditDTO result = new QualifiedFisherEditDTO
            {
                Id = dbEntry.Id,
                ApplicationId = dbEntry.ApplicationId,
                RegistrationDate = dbEntry.RegistrationDate,
                RegistrationNum = dbEntry.RegistrationNum,
                Name = dbEntry.Name,
                EGN = dbEntry.EGN,
                HasExam = dbEntry.HasExamLicense,
                ExamTerritoryUnitId = dbEntry.ExamTerritoryUnitId,
                ExamProtocolNumber = dbEntry.ExamProtocolNum,
                ExamDate = dbEntry.ExamDate,
                HasPassedExam = dbEntry.HasPassedExam,
                IsWithMaritimeEducation = dbEntry.IsWithMaritimeEducation,
                DiplomaNumber = dbEntry.DiplomaNum,
                DiplomaDate = dbEntry.DiplomaGraduationDate,
                DiplomaIssuer = dbEntry.DiplomaIssuer,
                Comments = dbEntry.Comments
            };

            result.Files = Db.GetFiles(Db.FishermenRegisterFiles, id);

            if (dbEntry.ApplicationId.HasValue)
            {
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(dbEntry.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);
            }

            result.SubmittedForRegixData = personService.GetRegixPersonData(dbEntry.SubmittedForPersonId);
            result.SubmittedForAddresses = personService.GetAddressRegistrations(dbEntry.SubmittedForPersonId);

            result.DuplicateEntries = duplicatesRegisterService.GetDuplicateEntries(fisherId: result.Id.Value);

            return result;
        }

        public int AddRegisterEntry(QualifiedFisherEditDTO newFisherDto)
        {
            FishermenRegister dbFisher;

            using (TransactionScope scope = new TransactionScope())
            {
                Person person = Db.AddOrEditPerson(newFisherDto.SubmittedForRegixData, newFisherDto.SubmittedForAddresses);

                dbFisher = new FishermenRegister
                {
                    RecordType = nameof(RecordTypesEnum.Register),
                    RegistrationDate = newFisherDto.RegistrationDate.Value,
                    Person = person
                };

                dbFisher.IsWithMaritimeEducation = newFisherDto.IsWithMaritimeEducation;
                dbFisher.HasExamLicense = newFisherDto.HasExam.Value;

                if (newFisherDto.IsWithMaritimeEducation)
                {
                    dbFisher.DiplomaNum = newFisherDto.DiplomaNumber;
                    dbFisher.DiplomaGraduationDate = newFisherDto.DiplomaDate.Value;
                    dbFisher.DiplomaIssuer = newFisherDto.DiplomaIssuer;
                }
                else
                {
                    dbFisher.ExamTerritoryUnitId = newFisherDto.ExamTerritoryUnitId.Value;
                    dbFisher.ExamProtocolNum = newFisherDto.ExamProtocolNumber;
                    dbFisher.ExamDate = newFisherDto.ExamDate;
                    dbFisher.HasPassedExam = newFisherDto.HasPassedExam.Value;

                    dbFisher.ApplicationId = newFisherDto.ApplicationId.Value;

                    int registrationApplicationId = (from fisherApplication in Db.FishermenRegisters
                                                     where fisherApplication.ApplicationId == newFisherDto.ApplicationId
                                                            && fisherApplication.RecordType == nameof(RecordTypesEnum.Application)
                                                            && fisherApplication.IsActive
                                                     select fisherApplication.Id).Single();
                    dbFisher.RegisterApplicationId = registrationApplicationId;
                }

                dbFisher.Comments = newFisherDto.Comments;

                Db.FishermenRegisters.Add(dbFisher);

                if (newFisherDto.Files != null)
                {
                    foreach (FileInfoDTO file in newFisherDto.Files)
                    {
                        Db.AddOrEditFile(dbFisher, dbFisher.FishermenRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                if (!newFisherDto.IsWithMaritimeEducation)
                {
                    this.applicationStateMachine.Act(dbFisher.ApplicationId.Value);
                }

                scope.Complete();
            }

            return dbFisher.Id;
        }

        public int EditRegisterEntry(QualifiedFisherEditDTO fisher)
        {
            FishermenRegister dbFisher = (from fshr in Db.FishermenRegisters
                                                         .AsSplitQuery()
                                                         .Include(x => x.FishermenRegisterFiles)
                                          where fshr.Id == fisher.Id.Value
                                          select fshr).First();

            Person person = Db.AddOrEditPerson(fisher.SubmittedForRegixData, fisher.SubmittedForAddresses, dbFisher.PersonId);

            dbFisher.RegistrationDate = fisher.RegistrationDate.Value;
            dbFisher.Person = person;

            if (fisher.IsWithMaritimeEducation)
            {
                dbFisher.ExamTerritoryUnitId = null;
                dbFisher.ExamProtocolNum = null;
                dbFisher.ExamDate = null;
                dbFisher.HasPassedExam = null;
                dbFisher.DiplomaNum = fisher.DiplomaNumber;
                dbFisher.DiplomaGraduationDate = fisher.DiplomaDate.Value;
                dbFisher.DiplomaIssuer = fisher.DiplomaIssuer;
            }
            else
            {
                dbFisher.HasPassedExam = fisher.HasPassedExam.Value;
                dbFisher.ExamTerritoryUnitId = fisher.ExamTerritoryUnitId.Value;
                dbFisher.ExamProtocolNum = fisher.ExamProtocolNumber;
                dbFisher.ExamDate = fisher.HasPassedExam.Value ? fisher.ExamDate.Value : fisher.ExamDate;

                dbFisher.DiplomaNum = null;
                dbFisher.DiplomaGraduationDate = null;
                dbFisher.DiplomaIssuer = null;
            }

            dbFisher.Comments = fisher.Comments;

            if (fisher.Files != null)
            {
                foreach (FileInfoDTO file in fisher.Files)
                {
                    Db.AddOrEditFile(dbFisher, dbFisher.FishermenRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            return dbFisher.Id;
        }

        public void Delete(int id)
        {
            DeleteRecordWithId(Db.FishermenRegisters, id);
            Db.SaveChanges();
        }

        public void UndoDelete(int id)
        {
            UndoDeleteRecordWithId(Db.FishermenRegisters, id);
            Db.SaveChanges();
        }

        public QualifiedFisherApplicationEditDTO GetApplicationEntry(int applicationId)
        {
            QualifiedFisherApplicationEditDTO result = (from fisher in Db.FishermenRegisters
                                                        join person in Db.Persons on fisher.PersonId equals person.Id
                                                        where fisher.ApplicationId == applicationId
                                                              && fisher.RecordType == nameof(RecordTypesEnum.Application)
                                                              && fisher.IsActive
                                                        select new QualifiedFisherApplicationEditDTO
                                                        {
                                                            ApplicationId = fisher.ApplicationId,
                                                            Id = fisher.Id,
                                                            Name = person.FirstName + " " + (string.IsNullOrEmpty(person.MiddleName) ? "" : person.MiddleName + " ") + person.LastName,
                                                            EGN = person.EgnLnc,
                                                            HasExam = fisher.HasExamLicense,
                                                            ExamTerritoryUnitId = fisher.ExamTerritoryUnitId,
                                                            Comments = fisher.Comments
                                                        }).SingleOrDefault();

            if (result == null)
            {
                string draftContent = (from appl in Db.Applications
                                       where appl.Id == applicationId
                                       select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draftContent))
                {
                    result = CommonUtils.Deserialize<QualifiedFisherApplicationEditDTO>(draftContent);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
                else
                {
                    result = new QualifiedFisherApplicationEditDTO
                    {
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                var applicationData = (from appl in Db.Applications
                                       join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                       join role in Db.NsubmittedByRoles on appl.SubmittedByPersonRoleId equals role.Id
                                       where appl.Id == applicationId
                                       select new
                                       {
                                           DeliveryId = appl.DeliveryId,
                                           SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                           SubmittedForPersonId = appl.SubmittedForPersonId.Value,
                                           StatusReason = appl.StatusReason,
                                           SubmittedByPersonRoleId = appl.SubmittedByPersonRoleId,
                                           SubmittedByRole = Enum.Parse<SubmittedByRolesEnum>(role.Code)
                                       }).First();

                result.SubmittedByRegixData = personService.GetRegixPersonData(applicationData.SubmittedByPersonId);
                result.SubmittedByAddresses = personService.GetAddressRegistrations(applicationData.SubmittedByPersonId);

                result.SubmittedByRole = applicationData.SubmittedByRole;

                result.SubmittedForRegixData = personService.GetRegixPersonData(applicationData.SubmittedForPersonId);
                result.SubmittedForAddresses = personService.GetAddressRegistrations(applicationData.SubmittedForPersonId);

                result.LetterOfAttorney = applicationService.GetLetterOfAttorney(applicationId);

                result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);

                if (applicationData.DeliveryId != null)
                {
                    result.DeliveryData = deliveryService.GetDeliveryData(applicationData.DeliveryId.Value);
                }

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                result.Files = Db.GetFiles(Db.FishermenRegisterFiles, result.Id.Value);
            }

            return result;
        }

        public RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO> GetApplicationRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO> regixData = new RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO>
            {
                DialogDataModel = GetApplicationData(applicationId),
                RegiXDataModel = regixApplicationService.GetQualifiedFisherChecks(applicationId)
            };

            regixData.DialogDataModel.ApplicationRegiXChecks = regixData.RegiXDataModel.ApplicationRegiXChecks;

            return regixData;
        }

        public QualifiedFisherRegixDataDTO GetApplicationData(int applicationId)
        {
            QualifiedApplicationDataIds regixDataIds = GetRegixDataIds(applicationId);

            SubmittedByRolesEnum submmitedByRole = (from role in Db.NsubmittedByRoles
                                                    where role.Id == regixDataIds.SubmittedByPersonRoleId
                                                    select Enum.Parse<SubmittedByRolesEnum>(role.Code)).First();

            var regixData = new QualifiedFisherRegixDataDTO
            {
                Id = regixDataIds.FishermanId,
                ApplicationId = regixDataIds.ApplicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedByRegixData = personService.GetRegixPersonData(regixDataIds.SubmittedByPersonId),
                SubmittedByAddresses = personService.GetAddressRegistrations(regixDataIds.SubmittedByPersonId),
                SubmittedForRegixData = personService.GetRegixPersonData(regixDataIds.SubmittedForPersonId),
                SubmittedForAddresses = personService.GetAddressRegistrations(regixDataIds.SubmittedForPersonId),
                SubmittedByRole = submmitedByRole
            };

            return regixData;
        }

        public QualifiedFisherEditDTO GetEntryByApplicationId(int applicationId)
        {
            var dbEntry = (from fisher in Db.FishermenRegisters
                           join person in Db.Persons on fisher.PersonId equals person.Id
                           where fisher.ApplicationId == applicationId
                                          && fisher.RecordType == nameof(RecordTypesEnum.Application)
                                          && fisher.IsActive
                           select new
                           {
                               fisher.Id,
                               fisher.ApplicationId,
                               SubmittedForPersonId = person.Id,
                               Name = person.FirstName + " " + (string.IsNullOrWhiteSpace(person.MiddleName) ? "" : person.MiddleName + " ") + person.LastName,
                               EGN = person.EgnLnc,
                               fisher.HasExamLicense,
                               fisher.ExamTerritoryUnitId,
                               fisher.ExamProtocolNum,
                               fisher.ExamDate,
                               fisher.DiplomaNum,
                               fisher.DiplomaGraduationDate,
                               fisher.DiplomaIssuer,
                               fisher.Comments,
                               IsWithMaritimeEducation = fisher.IsWithMaritimeEducation,
                               DiplomaOrExamLabel = fisher.HasExamLicense ? "e" : "d"
                           }).Single();

            QualifiedFisherEditDTO result = new QualifiedFisherEditDTO
            {
                ApplicationId = dbEntry.ApplicationId,
                Name = dbEntry.Name,
                EGN = dbEntry.EGN,
                HasExam = dbEntry.HasExamLicense,
                ExamTerritoryUnitId = dbEntry.ExamTerritoryUnitId,
                ExamProtocolNumber = dbEntry.ExamProtocolNum,
                ExamDate = dbEntry.ExamDate,
                HasPassedExam = false,
                DiplomaNumber = dbEntry.DiplomaNum,
                DiplomaDate = dbEntry.DiplomaGraduationDate,
                DiplomaIssuer = dbEntry.DiplomaIssuer,
                Comments = dbEntry.Comments
            };

            result.Files = Db.GetFiles(Db.FishermenRegisterFiles, dbEntry.Id);

            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(dbEntry.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);
            result.SubmittedForRegixData = personService.GetRegixPersonData(dbEntry.SubmittedForPersonId);
            result.SubmittedForAddresses = personService.GetAddressRegistrations(dbEntry.SubmittedForPersonId);

            return result;
        }

        public QualifiedFisherEditDTO GetRegisterByApplicationId(int applicationId)
        {
            int id = (from fisher in Db.FishermenRegisters
                      where fisher.ApplicationId == applicationId
                        && fisher.RecordType == nameof(RecordTypesEnum.Register)
                      select fisher.Id).First();

            return GetRegisterEntry(id);
        }

        public int AddApplicationEntry(QualifiedFisherApplicationEditDTO newFisherApplicationDto, ApplicationStatusesEnum? nextManualStatus)
        {
            Application application;
            FishermenRegister dbFisher;

            using (TransactionScope scope = new TransactionScope())
            {
                application = (from appl in Db.Applications
                               where appl.Id == newFisherApplicationDto.ApplicationId
                               select appl).First();

                int submittedByRoleId = (from role in Db.NsubmittedByRoles
                                         where role.Code == newFisherApplicationDto.SubmittedByRole.ToString()
                                         select role.Id).Single();
                application.SubmittedByPersonRoleId = submittedByRoleId;

                if (newFisherApplicationDto.SubmittedByRole == SubmittedByRolesEnum.Personal)
                {
                    application.SubmittedByPerson = Db.AddOrEditPerson(newFisherApplicationDto.SubmittedByRegixData, newFisherApplicationDto.SubmittedByAddresses);
                    application.SubmittedForPerson = application.SubmittedByPerson;
                }
                else
                {
                    application.SubmittedByPerson = Db.AddOrEditPerson(newFisherApplicationDto.SubmittedByRegixData, newFisherApplicationDto.SubmittedByAddresses);
                    application.SubmittedByLetterOfAttorney = Db.AddOrEditLetterOfAttorney(newFisherApplicationDto.LetterOfAttorney, application.SubmittedByLetterOfAttorneyId);
                    application.SubmittedForPerson = Db.AddOrEditPerson(newFisherApplicationDto.SubmittedForRegixData, newFisherApplicationDto.SubmittedForAddresses);
                }

                Db.SaveChanges();

                dbFisher = new FishermenRegister
                {
                    ApplicationId = newFisherApplicationDto.ApplicationId.Value,
                    Person = application.SubmittedForPerson,
                    RecordType = nameof(RecordTypesEnum.Application)
                };

                dbFisher.IsWithMaritimeEducation = false;
                dbFisher.HasExamLicense = true;
                dbFisher.ExamTerritoryUnitId = newFisherApplicationDto.ExamTerritoryUnitId.Value;
                dbFisher.HasPassedExam = false;

                dbFisher.Comments = newFisherApplicationDto.Comments;

                if (newFisherApplicationDto.Files != null)
                {
                    foreach (FileInfoDTO file in newFisherApplicationDto.Files)
                    {
                        Db.AddOrEditFile(dbFisher, dbFisher.FishermenRegisterFiles, file);
                    }
                }

                ApplicationDelivery delivery = Db.AddDeliveryData(newFisherApplicationDto.DeliveryData);
                application.Delivery = delivery;

                Db.FishermenRegisters.Add(dbFisher);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> files = newFisherApplicationDto.Files;
            newFisherApplicationDto.Files = null;
            string serializedAppl = CommonUtils.Serialize(newFisherApplicationDto);
            this.applicationStateMachine.Act(application.Id, serializedAppl, newFisherApplicationDto.Files, nextManualStatus);

            return dbFisher.Id;
        }

        public int EditApplicationEntry(QualifiedFisherApplicationEditDTO applicationDto, ApplicationStatusesEnum? manualStatus = null)
        {
            FishermenRegister applicationDb;

            using (TransactionScope scope = new TransactionScope())
            {
                applicationDb = (from fisher in Db.FishermenRegisters.Include(x => x.Person).Include(x => x.FishermenRegisterFiles)
                                 where fisher.Id == applicationDto.Id
                                       && fisher.RecordType == nameof(RecordTypesEnum.Application)
                                 select fisher).AsSplitQuery().First();

                Application application = (from appl in Db.Applications
                                           where appl.Id == applicationDb.ApplicationId
                                           select appl).First();

                int submittedByRoleId = (from role in Db.NsubmittedByRoles
                                         where role.Code == applicationDto.SubmittedByRole.ToString()
                                         select role.Id).Single();
                application.SubmittedByPersonRoleId = submittedByRoleId;

                if (applicationDto.SubmittedByRole == SubmittedByRolesEnum.Personal)
                {
                    application.SubmittedByPerson = Db.AddOrEditPerson(applicationDto.SubmittedByRegixData, applicationDto.SubmittedByAddresses, application.SubmittedByPersonId);
                    application.SubmittedForPerson = application.SubmittedByPerson;
                }
                else
                {
                    application.SubmittedByPerson = Db.AddOrEditPerson(applicationDto.SubmittedByRegixData, applicationDto.SubmittedByAddresses, application.SubmittedByPersonId);
                    application.SubmittedByLetterOfAttorney = Db.AddOrEditLetterOfAttorney(applicationDto.LetterOfAttorney, application.SubmittedByLetterOfAttorneyId);
                    application.SubmittedForPerson = Db.AddOrEditPerson(applicationDto.SubmittedForRegixData, applicationDto.SubmittedForAddresses, application.SubmittedForPersonId);
                }

                Db.SaveChanges();

                applicationDb.Person = application.SubmittedForPerson;
                applicationDb.HasExamLicense = applicationDto.HasExam.Value;
                applicationDb.ExamTerritoryUnitId = applicationDto.ExamTerritoryUnitId.Value;
                applicationDb.HasExamLicense = applicationDto.HasExam.Value;
                applicationDb.Comments = applicationDb.Comments;

                if (applicationDto.Files != null)
                {
                    foreach (FileInfoDTO file in applicationDto.Files)
                    {
                        Db.AddOrEditFile(applicationDb, applicationDb.FishermenRegisterFiles, file);
                    }
                }

                Db.EditDeliveryData(applicationDto.DeliveryData, application.DeliveryId.Value);

                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> fisherFiles = applicationDto.Files;
            applicationDto.Files = null;
            string serializedAppl = CommonUtils.Serialize(applicationDto);
            this.applicationStateMachine.Act(applicationDb.ApplicationId.Value, serializedAppl, fisherFiles, manualStatus, applicationDto.StatusReason);

            return applicationDb.Id;
        }

        public ApplicationStatusesEnum EditApplicationRegixData(QualifiedFisherRegixDataDTO regixData)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                FishermenRegister dbEntry = (from entry in Db.FishermenRegisters
                                             where entry.Id == regixData.Id
                                             select entry).First();

                application = (from appl in Db.Applications
                               where appl.Id == dbEntry.ApplicationId
                               select appl).First();


                Db.AddOrEditPerson(regixData.SubmittedByRegixData, regixData.SubmittedByAddresses, application.SubmittedByPersonId);
                Db.SaveChanges();

                if (regixData.SubmittedForRegixData != null)
                {
                    Db.AddOrEditPerson(regixData.SubmittedForRegixData, regixData.SubmittedForAddresses, dbEntry.PersonId);
                    Db.SaveChanges();
                }

                scope.Complete();
            }

            ApplicationStatusesEnum newStatus = this.applicationStateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);

            return newStatus;
        }

        public async Task<DownloadableFileDTO> GetRegisterFileForDownload(int registerId, bool duplicate = false)
        {
            DownloadableFileDTO downloadableFile = new DownloadableFileDTO();
            downloadableFile.MimeType = "application/pdf";

            string sumittedForName = (from fisherman in Db.FishermenRegisters
                                      join person in Db.Persons on fisherman.PersonId equals person.Id
                                      where fisherman.Id == registerId
                                      select $"{person.FirstName} - {person.LastName} ({person.EgnLnc})").First();

            if (duplicate)
            {
                downloadableFile.FileName = $"Дубликат-на-свидетелство-за-правоспособност_{sumittedForName}.pdf".Replace("  ", "");
            }
            else
            {
                downloadableFile.FileName = $"Свидетелство-за-правоспособност_{sumittedForName}.pdf".Replace("  ", "");
            }

            downloadableFile.Bytes = await jasperReportsService.GetFishermanRegister(registerId, duplicate);

            return downloadableFile;
        }

        public bool PersonIsQualifiedFisherCheck(EgnLncDTO personIdentifier, int? entryId)
        {
            bool personIsQualifiedFisher = (from qualifiedFisher in Db.FishermenRegisters
                                            join person in Db.Persons on qualifiedFisher.PersonId equals person.Id
                                            where person.EgnLnc == personIdentifier.EgnLnc
                                                  && person.IdentifierType == personIdentifier.IdentifierType.ToString()
                                                  && qualifiedFisher.RecordType == nameof(RecordTypesEnum.Register)
                                                  && qualifiedFisher.IsActive
                                                  && (!entryId.HasValue || qualifiedFisher.Id != entryId.Value)
                                            select qualifiedFisher.Id).Any();

            return personIsQualifiedFisher;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.FishermenRegisters, id);
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            if (pageCode != PageCodeEnum.CommFishLicense && pageCode != PageCodeEnum.CompetencyDup)
            {
                throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
            }

            var data = (from fisher in Db.FishermenRegisters
                        join appl in Db.Applications on fisher.ApplicationId equals appl.Id
                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                        join submittedForPerson in Db.Persons on fisher.PersonId equals submittedForPerson.Id
                        join user in Db.Users on fisher.CreatedBy equals user.Username
                        join person in Db.Persons on user.PersonId equals person.Id
                        where fisher.ApplicationId == applicationId
                            && fisher.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            fisher.Id,
                            ApplicationType = applType.Name,
                            ApplicationTypeCode = applType.Code,
                            SubmittedForPersonId = submittedForPerson.Id,
                            CreatedByPersonEGN = person.EgnLnc
                        }).First();

            RegixPersonDataDTO subForPerson = personService.GetRegixPersonData(data.SubmittedForPersonId);

            DownloadableFileDTO pdf = await GetRegisterFileForDownload(data.Id, pageCode == PageCodeEnum.CompetencyDup);

            ApplicationEDeliveryInfo info = new ApplicationEDeliveryInfo
            {
                Subject = data.ApplicationType,
                DocBytes = pdf.Bytes,
                DocNameWithExtension = pdf.FileName,
                DocRegNumber = applicationId.ToString(),
                ReceiverType = eProfileType.LegalPerson,
                ReceiverUniqueIdentifier = subForPerson.EgnLnc.EgnLnc,
                ReceiverPhone = subForPerson.Phone,
                ReceiverEmail = subForPerson.Email,
                ServiceOID = data.ApplicationTypeCode,
                OperatorEGN = data.CreatedByPersonEGN
            };

            return info;
        }

        private IQueryable<QualifiedFisherDTO> GetAllFishers(bool inactiveOnly = false)
        {
            IQueryable<QualifiedFisherDTO> result = from fisher in Db.FishermenRegisters
                                                    join application in Db.Applications on fisher.ApplicationId equals application.Id into appl
                                                    from application in appl.DefaultIfEmpty()
                                                    join person in Db.Persons on fisher.PersonId equals person.Id
                                                    where fisher.IsActive == !inactiveOnly
                                                        && fisher.RecordType == nameof(RecordTypesEnum.Register)
                                                    orderby fisher.Id descending
                                                    select new QualifiedFisherDTO
                                                    {
                                                        Id = fisher.Id,
                                                        ApplicationId = application != null ? application.Id : default,
                                                        PageCode = PageCodeEnum.CommFishLicense,
                                                        RegistrationNum = fisher.RegistrationNum,
                                                        RegistrationDate = fisher.RegistrationDate,
                                                        Name = person.FirstName + " " + person.LastName,
                                                        IsWithMaritimeEducation = fisher.IsWithMaritimeEducation,
                                                        DiplomaOrExamLabel = fisher.HasExamLicense ? "e" : "d",
                                                        DiplomaOrExamNumber = fisher.HasExamLicense ? fisher.ExamProtocolNum : fisher.DiplomaNum,
                                                        DeliveryId = application != null ? application.DeliveryId : default,
                                                        IsActive = fisher.IsActive
                                                    };
            return result;
        }

        private IQueryable<QualifiedFisherDTO> GetAllFishersByFilter(QualifiedFishersFilters filters)
        {
            filters.RegistrationNum = filters.RegistrationNum?.Trim().ToLower();
            filters.DiplomaNr = filters.DiplomaNr?.Trim().ToLower();
            filters.Name = filters.Name?.Trim().ToLower();
            filters.EGN = filters.EGN?.Trim();

            IQueryable<QualifiedFisherDTO> result = from fisher in Db.FishermenRegisters
                                                    join application in Db.Applications on fisher.ApplicationId equals application.Id into appl
                                                    from application in appl.DefaultIfEmpty()
                                                    join person in Db.Persons on fisher.PersonId equals person.Id
                                                    where fisher.RecordType == nameof(RecordTypesEnum.Register)
                                                          && fisher.IsActive == !filters.ShowInactiveRecords
                                                          && (filters.RegistrationNum == null || fisher.RegistrationNum.ToLower().Contains(filters.RegistrationNum))
                                                          && (filters.RegisteredDateFrom == null || (fisher.RegistrationDate.HasValue && fisher.RegistrationDate.Value.Date >= filters.RegisteredDateFrom))
                                                          && (filters.RegisteredDateTo == null || (fisher.RegistrationDate.HasValue && fisher.RegistrationDate.Value.Date <= filters.RegisteredDateTo))
                                                          && (filters.Name == null || (person.FirstName + " " + person.LastName).ToLower().Contains(filters.Name))
                                                          && (filters.EGN == null || person.EgnLnc == filters.EGN)
                                                          && (filters.DiplomaNr == null || (fisher.DiplomaNum != null && fisher.DiplomaNum.Trim().ToLower() == filters.DiplomaNr))
                                                          && (!filters.PersonId.HasValue || (filters.PersonId.HasValue && fisher.PersonId == filters.PersonId))
                                                          && (!filters.TerritoryUnitId.HasValue || application.TerritoryUnitId == filters.TerritoryUnitId.Value)
                                                    orderby fisher.Id descending
                                                    select new QualifiedFisherDTO
                                                    {
                                                        Id = fisher.Id,
                                                        ApplicationId = application != null ? application.Id : default,
                                                        PageCode = PageCodeEnum.CommFishLicense,
                                                        RegistrationNum = fisher.RegistrationNum,
                                                        RegistrationDate = fisher.RegistrationDate,
                                                        Name = person.FirstName + " " + person.LastName,
                                                        IsWithMaritimeEducation = fisher.IsWithMaritimeEducation,
                                                        DiplomaOrExamLabel = fisher.HasExamLicense ? "e" : "d",
                                                        DiplomaOrExamNumber = fisher.HasExamLicense ? fisher.ExamProtocolNum : fisher.DiplomaNum,
                                                        DeliveryId = application != null ? application.DeliveryId : default,
                                                        IsActive = fisher.IsActive
                                                    };

            return result;
        }

        private IQueryable<QualifiedFisherDTO> GetAllFishersByFreeText(QualifiedFishersFilters filters)
        {
            filters.FreeTextSearch = filters.FreeTextSearch.ToLower();
            DateTime? searchDate = DateTimeUtils.TryParseDate(filters.FreeTextSearch);

            IQueryable<QualifiedFisherDTO> result = from fisher in Db.FishermenRegisters
                                                    join application in Db.Applications on fisher.ApplicationId equals application.Id into appl
                                                    from application in appl.DefaultIfEmpty()
                                                    join person in Db.Persons on fisher.PersonId equals person.Id
                                                    where fisher.RecordType == nameof(RecordTypesEnum.Register)
                                                    && fisher.IsActive == !filters.ShowInactiveRecords
                                                          && ((searchDate.HasValue && ((fisher.RegistrationDate.HasValue && fisher.RegistrationDate.Value.Date == searchDate.Value.Date)
                                                                                      || (!fisher.HasExamLicense && fisher.DiplomaGraduationDate != null && fisher.DiplomaGraduationDate.Value.Date == searchDate.Value.Date)
                                                                                      || (fisher.HasExamLicense && fisher.ExamDate != null && fisher.ExamDate.Value.Date == searchDate.Value.Date)))
                                                                    || fisher.RegistrationNum.ToLower().Contains(filters.FreeTextSearch)
                                                                    || (person.FirstName + " " + person.LastName).ToLower().Contains(filters.FreeTextSearch)
                                                                    || (person.EgnLnc != null && person.EgnLnc == filters.FreeTextSearch)
                                                                    || (fisher.DiplomaNum != null && fisher.DiplomaNum.ToLower().Contains(filters.FreeTextSearch))
                                                                    || (fisher.ExamProtocolNum != null && fisher.ExamProtocolNum.ToLower().Contains(filters.FreeTextSearch))
                                                             )
                                                          && (!filters.TerritoryUnitId.HasValue || (application != null && application.TerritoryUnitId == filters.TerritoryUnitId.Value))
                                                    orderby fisher.Id descending
                                                    select new QualifiedFisherDTO
                                                    {
                                                        Id = fisher.Id,
                                                        ApplicationId = application != null ? application.Id : default,
                                                        PageCode = PageCodeEnum.CommFishLicense,
                                                        RegistrationNum = fisher.RegistrationNum,
                                                        RegistrationDate = fisher.RegistrationDate,
                                                        Name = person.FirstName + " " + person.LastName,
                                                        IsWithMaritimeEducation = fisher.IsWithMaritimeEducation,
                                                        DiplomaOrExamLabel = fisher.HasExamLicense ? "e" : "d",
                                                        DiplomaOrExamNumber = fisher.HasExamLicense ? fisher.ExamProtocolNum : fisher.DiplomaNum,
                                                        DeliveryId = application != null ? application.DeliveryId : default,
                                                        IsActive = fisher.IsActive
                                                    };
            return result;
        }

        private QualifiedApplicationDataIds GetRegixDataIds(int applicationId)
        {
            var regixDataIds = (from fisherman in Db.FishermenRegisters
                                join appl in Db.Applications on fisherman.ApplicationId equals appl.Id
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where fisherman.ApplicationId == applicationId && fisherman.RecordType == nameof(RecordTypesEnum.Application)
                                select new QualifiedApplicationDataIds
                                {
                                    FishermanId = fisherman.Id,
                                    ApplicationId = fisherman.ApplicationId.Value,
                                    SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                    SubmittedByPersonRoleId = appl.SubmittedByPersonRoleId.Value,
                                    SubmittedForPersonId = appl.SubmittedForPersonId.Value,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                }).Single();

            return regixDataIds;
        }
    }
}
