using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;
using TL.EDelivery;

namespace IARA.Infrastructure.Services
{
    public partial class AquacultureFacilitiesService : Service, IAquacultureFacilitiesService
    {
        public AquacultureApplicationEditDTO GetAquacultureApplication(int applicationId)
        {
            AquacultureFacilityRegister dbAquaculture = (from aquaculture in Db.AquacultureFacilitiesRegister
                                                         where aquaculture.ApplicationId == applicationId
                                                            && aquaculture.RecordType == nameof(RecordTypesEnum.Application)
                                                         select aquaculture).SingleOrDefault();

            AquacultureApplicationEditDTO result = null;

            if (dbAquaculture == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<AquacultureApplicationEditDTO>(draft);
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
                    result = new AquacultureApplicationEditDTO
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
                result = new AquacultureApplicationEditDTO
                {
                    Id = dbAquaculture.Id,
                    ApplicationId = dbAquaculture.ApplicationId,
                    Name = dbAquaculture.Name,
                    TerritoryUnitId = dbAquaculture.TerritoryUnitId,
                    WaterAreaTypeId = dbAquaculture.WaterAreaTypeId,
                    PopulatedAreaId = dbAquaculture.PopulatedAreaId,
                    LocationDescription = dbAquaculture.LocationDescription,
                    WaterSalinity = Enum.Parse<AquacultureSalinityEnum>(dbAquaculture.WaterSalinity),
                    WaterTemperature = Enum.Parse<AquacultureTemperatureEnum>(dbAquaculture.WaterTemperature),
                    System = Enum.Parse<AquacultureSystemEnum>(dbAquaculture.System),
                    PowerSupplyTypeId = dbAquaculture.PowerSupplyTypeId,
                    PowerSupplyName = dbAquaculture.PowerSupplyName,
                    PowerSupplyDebit = dbAquaculture.PowerSupplyDebit,
                    TotalWaterArea = dbAquaculture.TotalWaterArea,
                    TotalProductionCapacity = dbAquaculture.TotalProductionCapacity,
                    Comments = dbAquaculture.Comments
                };

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                result.Coordinates = GetAquacultureCoordinates(dbAquaculture.Id);
                result.AquaticOrganismIds = GetAquacultureAquaticOrganismIds(dbAquaculture.Id);
                result.Installations = GetAquacultureInstallations(dbAquaculture.Id);
                result.UsageDocument = GetAquacultureUsageDocument(dbAquaculture.Id);

                if (result.System == AquacultureSystemEnum.FullSystem)
                {
                    result.HatcheryCapacity = dbAquaculture.HatcheryCapacity.Value;
                    result.HatcheryEquipment = GetAquacultureHatcheryEquipment(dbAquaculture.Id);
                }

                List<AquacultureWaterLawCertificateDTO> waterLawCertificates = GetAquacultureWaterLawCertificates(dbAquaculture.Id);
                List<CommonDocumentDTO> ovosCertificates = GetAquacultureOvosCertificates(dbAquaculture.Id);
                List<CommonDocumentDTO> babhCertificates = GetAquacultureBabhCertificates(dbAquaculture.Id);

                if (waterLawCertificates.Count == 1)
                {
                    result.WaterLawCertificate = waterLawCertificates[0];
                }

                if (ovosCertificates.Count == 1)
                {
                    result.OvosCertificate = ovosCertificates[0];
                }

                if (babhCertificates.Where(x => x.IsActive.Value).Count() == 1)
                {
                    result.HasBabhCertificate = true;
                    result.BabhCertificate = babhCertificates.Where(x => x.IsActive.Value).Single();
                }
                else
                {
                    result.HasBabhCertificate = false;
                }

                result.Files = Db.GetFiles(Db.AquacultureFacilityFiles, dbAquaculture.Id);

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<AquacultureRegixDataDTO> GetAquacultureRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<AquacultureRegixDataDTO> result = new RegixChecksWrapperDTO<AquacultureRegixDataDTO>
            {
                DialogDataModel = GetApplicationRegistrationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetAquacultureRegistrationChecks(applicationId),
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public AquacultureRegixDataDTO GetApplicationRegistrationRegixData(int applicationId)
        {
            AquacultureApplicationDataIds data = GetApplicationDataIds(applicationId);

            AquacultureRegixDataDTO regixData = new AquacultureRegixDataDTO
            {
                Id = data.AquacultureId,
                ApplicationId = applicationId,
                PageCode = data.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);
            regixData.UsageDocument = GetAquacultureUsageDocumentRegixData(data.AquacultureId);

            return regixData;
        }

        public AquacultureFacilityEditDTO GetApplicationDataForRegister(int applicationId)
        {
            AquacultureFacilityRegister dbAquaculture = (from aquaculture in Db.AquacultureFacilitiesRegister
                                                         where aquaculture.ApplicationId == applicationId
                                                            && aquaculture.RecordType == nameof(RecordTypesEnum.Application)
                                                         select aquaculture).Single();

            AquacultureFacilityEditDTO result = new AquacultureFacilityEditDTO
            {
                Id = dbAquaculture.Id,
                ApplicationId = dbAquaculture.ApplicationId,
                Name = dbAquaculture.Name,
                TerritoryUnitId = dbAquaculture.TerritoryUnitId,
                WaterAreaTypeId = dbAquaculture.WaterAreaTypeId,
                PopulatedAreaId = dbAquaculture.PopulatedAreaId,
                LocationDescription = dbAquaculture.LocationDescription,
                WaterSalinity = Enum.Parse<AquacultureSalinityEnum>(dbAquaculture.WaterSalinity),
                WaterTemperature = Enum.Parse<AquacultureTemperatureEnum>(dbAquaculture.WaterTemperature),
                System = Enum.Parse<AquacultureSystemEnum>(dbAquaculture.System),
                PowerSupplyTypeId = dbAquaculture.PowerSupplyTypeId,
                PowerSupplyName = dbAquaculture.PowerSupplyName,
                PowerSupplyDebit = dbAquaculture.PowerSupplyDebit,
                TotalWaterArea = dbAquaculture.TotalWaterArea,
                TotalProductionCapacity = dbAquaculture.TotalProductionCapacity,
                Comments = dbAquaculture.Comments
            };

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(applicationId, dbAquaculture.SubmittedForPersonId, dbAquaculture.SubmittedForLegalId);
            result.Coordinates = GetAquacultureCoordinates(dbAquaculture.Id);
            result.AquaticOrganismIds = GetAquacultureAquaticOrganismIds(dbAquaculture.Id);
            result.Installations = GetAquacultureInstallations(dbAquaculture.Id);
            result.UsageDocuments = GetAquacultureUsageDocuments(dbAquaculture.Id);
            result.WaterLawCertificates = GetAquacultureWaterLawCertificates(dbAquaculture.Id);
            result.OvosCertificates = GetAquacultureOvosCertificates(dbAquaculture.Id);
            result.BabhCertificates = GetAquacultureBabhCertificates(dbAquaculture.Id);
            result.Files = Db.GetFiles(Db.AquacultureFacilityFiles, dbAquaculture.Id);

            if (result.System == AquacultureSystemEnum.FullSystem)
            {
                result.HatcheryCapacity = dbAquaculture.HatcheryCapacity.Value;
                result.HatcheryEquipment = GetAquacultureHatcheryEquipment(dbAquaculture.Id);
            }

            return result;
        }

        public AquacultureFacilityEditDTO GetRegisterByApplicationId(int applicationId)
        {
            int id = (from aqua in Db.AquacultureFacilitiesRegister
                      where aqua.ApplicationId == applicationId
                            && aqua.RecordType == nameof(RecordTypesEnum.Register)
                      select aqua.Id).First();

            return GetAquaculture(id);
        }

        public AquacultureFacilityEditDTO GetRegisterByChangeOfCircumstancesApplicationId(int applicationId)
        {
            int id = (from change in Db.ApplicationChangeOfCircumstances
                      where change.ApplicationId == applicationId
                            && change.IsActive
                      select change.AquacultureFacilityId.Value).First();

            return GetAquaculture(id);
        }

        public int AddAquacultureApplication(AquacultureApplicationEditDTO aquaculture, ApplicationStatusesEnum? nextManualStatus = null)
        {
            AquacultureFacilityRegister entry;
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                entry = new AquacultureFacilityRegister
                {
                    ApplicationId = aquaculture.ApplicationId.Value,
                    RecordType = nameof(RecordTypesEnum.Application),
                    AquacultureStatusId = GetAquacultureStatusId(AquacultureStatusEnum.Application),
                    RegistrationDate = DateTime.Now,
                    Name = aquaculture.Name,
                    TerritoryUnitId = aquaculture.TerritoryUnitId.Value,
                    WaterAreaTypeId = aquaculture.WaterAreaTypeId.Value,
                    PopulatedAreaId = aquaculture.PopulatedAreaId,
                    LocationDescription = aquaculture.LocationDescription,
                    WaterSalinity = aquaculture.WaterSalinity.Value.ToString(),
                    WaterTemperature = aquaculture.WaterTemperature.Value.ToString(),
                    System = aquaculture.System.Value.ToString(),
                    PowerSupplyTypeId = aquaculture.PowerSupplyTypeId.Value,
                    PowerSupplyName = aquaculture.PowerSupplyName,
                    PowerSupplyDebit = aquaculture.PowerSupplyDebit,
                    TotalWaterArea = aquaculture.TotalWaterArea.Value,
                    TotalProductionCapacity = aquaculture.TotalProductionCapacity.Value,
                    Comments = aquaculture.Comments
                };

                application = (from appl in Db.Applications
                               where appl.Id == entry.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedBy(application, aquaculture.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, aquaculture.SubmittedFor);
                entry.SubmittedForPerson = application.SubmittedForPerson;
                entry.SubmittedForLegal = application.SubmittedForLegal;
                Db.SaveChanges();

                AddAquacultureCoordinates(entry, aquaculture.Coordinates);
                AddAquaticOrganisms(entry, aquaculture.AquaticOrganismIds);
                AddAquacultureInstallations(entry, aquaculture.Installations);
                AddUsageDocuments(entry, new List<UsageDocumentDTO> { aquaculture.UsageDocument });
                AddWaterLawCertificates(entry, new List<AquacultureWaterLawCertificateDTO> { aquaculture.WaterLawCertificate });
                AddOvosCertificates(entry, new List<CommonDocumentDTO> { aquaculture.OvosCertificate });

                if (aquaculture.HasBabhCertificate.Value)
                {
                    AddBabhCertificates(entry, new List<CommonDocumentDTO> { aquaculture.BabhCertificate });
                }

                if (aquaculture.System == AquacultureSystemEnum.FullSystem)
                {
                    entry.HatcheryCapacity = aquaculture.HatcheryCapacity.Value;
                    AddAquacultureHatcheryEquipment(entry, aquaculture.HatcheryEquipment);
                }

                if (aquaculture.Files != null)
                {
                    foreach (FileInfoDTO file in aquaculture.Files)
                    {
                        Db.AddOrEditFile(entry, entry.AquacultureFacilityRegisterFiles, file);
                    }
                }

                Db.AddDeliveryData(application, aquaculture);

                Db.AquacultureFacilitiesRegister.Add(entry);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = aquaculture.Files;
            aquaculture.Files = null;
            stateMachine.Act(entry.ApplicationId, CommonUtils.Serialize(aquaculture), aquacultureFiles, nextManualStatus);

            return entry.Id;
        }

        public void EditAquacultureApplication(AquacultureApplicationEditDTO aquaculture, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                AquacultureFacilityRegister dbAquaculture = (from aq in Db.AquacultureFacilitiesRegister
                                                                .AsSplitQuery()
                                                                .Include(x => x.AquacultureFacilityRegisterFiles)
                                                             where aq.Id == aquaculture.Id
                                                             select aq).First();

                dbAquaculture.Name = aquaculture.Name;
                dbAquaculture.TerritoryUnitId = aquaculture.TerritoryUnitId.Value;
                dbAquaculture.WaterAreaTypeId = aquaculture.WaterAreaTypeId.Value;
                dbAquaculture.PopulatedAreaId = aquaculture.PopulatedAreaId;
                dbAquaculture.LocationDescription = aquaculture.LocationDescription;
                dbAquaculture.WaterSalinity = aquaculture.WaterSalinity.Value.ToString();
                dbAquaculture.WaterTemperature = aquaculture.WaterTemperature.Value.ToString();
                dbAquaculture.System = aquaculture.System.Value.ToString();
                dbAquaculture.PowerSupplyTypeId = aquaculture.PowerSupplyTypeId.Value;
                dbAquaculture.PowerSupplyName = aquaculture.PowerSupplyName;
                dbAquaculture.PowerSupplyDebit = aquaculture.PowerSupplyDebit;
                dbAquaculture.TotalWaterArea = aquaculture.TotalWaterArea.Value;
                dbAquaculture.TotalProductionCapacity = aquaculture.TotalProductionCapacity.Value;
                dbAquaculture.Comments = aquaculture.Comments;

                EditAquacultureCoordinates(dbAquaculture.Id, aquaculture.Coordinates);
                EditAquacultureAquaticOrganisms(dbAquaculture.Id, aquaculture.AquaticOrganismIds);
                EditAquacultureInstallations(dbAquaculture, aquaculture.Installations);
                EditAquacultureHatcheryEquipment(dbAquaculture.Id, aquaculture.HatcheryEquipment, aquaculture.System.Value);
                EditUsageDocuments(dbAquaculture, new List<UsageDocumentDTO> { aquaculture.UsageDocument });
                EditWaterLawCertificates(dbAquaculture.Id, new List<AquacultureWaterLawCertificateDTO> { aquaculture.WaterLawCertificate });
                EditOvosCertificates(dbAquaculture.Id, new List<CommonDocumentDTO> { aquaculture.OvosCertificate });

                if (aquaculture.HasBabhCertificate.Value)
                {
                    EditBabhCertificates(dbAquaculture.Id, new List<CommonDocumentDTO> { aquaculture.BabhCertificate });
                }
                else
                {
                    EditBabhCertificates(dbAquaculture.Id, null);
                }

                if (aquaculture.System.Value == AquacultureSystemEnum.FullSystem)
                {
                    dbAquaculture.HatcheryCapacity = aquaculture.HatcheryCapacity.Value;
                }
                else
                {
                    dbAquaculture.HatcheryCapacity = null;
                }

                application = (from appl in Db.Applications
                               where appl.Id == dbAquaculture.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedBy(application, aquaculture.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, aquaculture.SubmittedFor);
                Db.SaveChanges();

                if (aquaculture.Files != null)
                {
                    foreach (FileInfoDTO file in aquaculture.Files)
                    {
                        Db.AddOrEditFile(dbAquaculture, dbAquaculture.AquacultureFacilityRegisterFiles, file);
                    }
                }

                Db.EditDeliveryData(application, aquaculture);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = aquaculture.Files;
            aquaculture.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(aquaculture), aquacultureFiles, manualStatus, application.StatusReason);
        }

        public void EditAquacultureApplicationRegixData(AquacultureRegixDataDTO aquaculture)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                AquacultureFacilityRegister dbAquaculture = (from aq in Db.AquacultureFacilitiesRegister
                                                             where aq.Id == aquaculture.Id
                                                             select aq).First();

                EditUsageDocuments(dbAquaculture, new List<UsageDocumentRegixDataDTO> { aquaculture.UsageDocument });

                application = (from appl in Db.Applications
                               where appl.Id == dbAquaculture.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(application, aquaculture.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(application, aquaculture.SubmittedFor);
                Db.SaveChanges();

                scope.Complete();
            }

            stateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            if (pageCode == PageCodeEnum.AquaFarmReg)
            {
                var data = (from aqua in Db.AquacultureFacilitiesRegister
                            join appl in Db.Applications on aqua.ApplicationId equals appl.Id
                            join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                            join submittedForLegal in Db.Legals on aqua.SubmittedForLegalId equals submittedForLegal.Id into leg
                            from submittedForLegal in leg.DefaultIfEmpty()
                            join submittedForPerson in Db.Persons on aqua.SubmittedForPersonId equals submittedForPerson.Id into per
                            from submittedForPerson in per.DefaultIfEmpty()
                            join user in Db.Users on aqua.CreatedBy equals user.Username
                            join person in Db.Persons on user.PersonId equals person.Id
                            where aqua.ApplicationId == applicationId
                                && aqua.RecordType == nameof(RecordTypesEnum.Register)
                            select new
                            {
                                aqua.Id,
                                ApplicationType = applType.Name,
                                ApplicationTypeCode = applType.Code,
                                SubmittedForLegalId = submittedForLegal != null ? (int?)submittedForLegal.Id : null,
                                SubmittedForPersonId = submittedForPerson != null ? (int?)submittedForPerson.Id : null,
                                CreatedByPersonEGN = person.EgnLnc
                            }).First();

                RegixPersonDataDTO subForPerson = null;
                RegixLegalDataDTO subForLegal = null;

                if (data.SubmittedForPersonId.HasValue)
                {
                    subForPerson = personService.GetRegixPersonData(data.SubmittedForPersonId.Value);
                }
                else
                {
                    subForLegal = legalService.GetRegixLegalData(data.SubmittedForLegalId.Value);
                }

                string submittedForName = subForPerson != null
                    ? $"{subForPerson.FirstName}-{subForPerson.LastName}-{subForPerson.EgnLnc.EgnLnc}"
                    : $"{subForLegal.Name}-{subForLegal.EIK}";

                byte[] pdf = await DownloadAquacultureFacility(data.Id);

                ApplicationEDeliveryInfo info = new ApplicationEDeliveryInfo
                {
                    Subject = data.ApplicationType,
                    DocBytes = pdf,
                    DocNameWithExtension = $"Аквакултурно-стопанство_{submittedForName}.pdf".Replace(" ", ""),
                    DocRegNumber = applicationId.ToString(),
                    ReceiverType = eProfileType.LegalPerson,
                    ReceiverUniqueIdentifier = subForPerson != null ? subForPerson.EgnLnc.EgnLnc : subForLegal.EIK,
                    ReceiverPhone = subForPerson != null ? subForPerson.Phone : subForLegal.Phone,
                    ReceiverEmail = subForPerson != null ? subForPerson.Email : subForLegal.Email,
                    ServiceOID = data.ApplicationTypeCode,
                    OperatorEGN = data.CreatedByPersonEGN
                };

                return info;
            }

            throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
        }

        private AquacultureApplicationDataIds GetApplicationDataIds(int applicationId)
        {
            AquacultureApplicationDataIds data = (from aquaculture in Db.AquacultureFacilitiesRegister
                                                  join appl in Db.Applications on aquaculture.ApplicationId equals appl.Id
                                                  join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                  where aquaculture.ApplicationId == applicationId
                                                      && aquaculture.RecordType == nameof(RecordTypesEnum.Application)
                                                  select new AquacultureApplicationDataIds
                                                  {
                                                      AquacultureId = aquaculture.Id,
                                                      PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                                  }).Single();

            return data;
        }

        private UsageDocumentDTO GetAquacultureUsageDocument(int aquacultureId)
        {
            int id = (from aquaDoc in Db.AquacultureUsageDocuments
                      where aquaDoc.AquacultureFacilityId == aquacultureId
                         && aquaDoc.IsActive
                      select aquaDoc.UsageDocumentId).Single();

            UsageDocumentDTO document = usageDocumentsService.GetUsageDocument(id);
            return document;
        }

        private UsageDocumentRegixDataDTO GetAquacultureUsageDocumentRegixData(int aquacultureId)
        {
            int id = (from aquaDoc in Db.AquacultureUsageDocuments
                      where aquaDoc.AquacultureFacilityId == aquacultureId
                         && aquaDoc.IsActive
                      select aquaDoc.UsageDocumentId).Single();

            UsageDocumentRegixDataDTO document = usageDocumentsService.GetUsageDocumentRegixData(id);
            return document;
        }

        private void EditUsageDocuments(AquacultureFacilityRegister aquaculture, List<UsageDocumentRegixDataDTO> documents)
        {
            List<AquacultureUsageDocument> currentUsageDocuments = (from usgDoc in Db.AquacultureUsageDocuments
                                                                    where usgDoc.AquacultureFacilityId == aquaculture.Id
                                                                    select usgDoc).ToList();

            if (documents != null)
            {
                foreach (UsageDocumentRegixDataDTO document in documents)
                {
                    AquacultureUsageDocument dbDoc = currentUsageDocuments.Where(x => x.UsageDocumentId == document.Id).Single();
                    dbDoc.UsageDocument = Db.EditUsageDocument(document, aquaculture);
                    dbDoc.IsActive = document.IsActive;
                }
            }
            else
            {
                foreach (AquacultureUsageDocument document in currentUsageDocuments)
                {
                    document.IsActive = false;
                }
            }
        }
    }
}
