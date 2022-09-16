using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;
using TL.EDelivery;

namespace IARA.Infrastructure.Services.Buyers
{
    public partial class BuyersService : Service, IBuyersService
    {
        public BuyerApplicationEditDTO GetApplicationEntry(int applicationId)
        {
            BuyerApplicationEditDTO result = (from buyer in Db.BuyerRegisters
                                              join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                                              join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                              where buyer.ApplicationId == applicationId
                                                && buyer.RecordType == nameof(RecordTypesEnum.Application)
                                              select new BuyerApplicationEditDTO
                                              {
                                                  Id = buyer.Id,
                                                  ApplicationId = buyer.ApplicationId,
                                                  PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                                  HasUtility = buyer.HasUtility,
                                                  UtilityName = buyer.UtilityName,
                                                  HasVehicle = buyer.HasVehicle,
                                                  VehicleNumber = buyer.VehicleNumber,
                                                  Comments = buyer.Comments,
                                                  PremiseAddressId = buyer.UtilityAddressId,
                                                  SubmittedForLegalId = buyer.SubmittedForLegalId,
                                                  SubmittedForPersonId = buyer.SubmittedForPersonId,
                                                  AgentId = buyer.AgentId,
                                                  IsAgentSameAsSubmittedBy = buyer.IsAgentSameAsSubmittedBy,
                                                  IsAgentSameAsSubmittedForCustodianOfProperty = buyer.IsAgentSameAsSubmittedForCustodianOfProperty,
                                                  OrganizerPersonId = buyer.OrganizingPersonId,
                                                  OrganizerSameAsSubmittedBy = buyer.IsOrganizingPersonSameAsSubmittedBy
                                              }).SingleOrDefault();

            if (result == null)
            {
                string draftContent = (from appl in Db.Applications
                                       where appl.Id == applicationId
                                       select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draftContent))
                {
                    result = CommonUtils.Deserialize<BuyerApplicationEditDTO>(draftContent);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid.Value && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }

                }
                else
                {
                    result = new BuyerApplicationEditDTO
                    {
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid.Value)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                Application application = (from appl in Db.Applications
                                           where appl.Id == result.ApplicationId
                                           select appl).First();

                if (application.SubmittedByPersonId.HasValue)
                {
                    result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                }

                if (result.SubmittedForLegalId.HasValue || result.SubmittedForPersonId.HasValue)
                {
                    result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                }

                if (result.HasUtility.HasValue && result.HasUtility.Value)
                {
                    result.PremiseAddress = addressService.GetAddressRegistration(result.PremiseAddressId.Value);
                    result.BabhLawLicenseDocumnet = GetLicenseDocument(result.Id.Value, BuyerLicenseTypesEnum.Food);
                }

                if (result.HasVehicle.HasValue && result.HasVehicle.Value)
                {
                    result.VeteniraryVehicleRegLicenseDocument = GetLicenseDocument(result.Id.Value, BuyerLicenseTypesEnum.Vehicle);
                }

                if (result.PageCode == PageCodeEnum.RegFirstSaleBuyer)
                {
                    result.Agent = personService.GetRegixPersonData(result.AgentId.Value);
                }
                else if (result.PageCode == PageCodeEnum.RegFirstSaleCenter)
                {
                    result.Organizer = personService.GetRegixPersonData(result.OrganizerPersonId.Value);

                    if (result.HasUtility.Value)
                    {
                        result.PremiseUsageDocument = GetPremiseUsageDocument(result.Id.Value);
                    }
                }

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);

                if (result.IsPaid.Value)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery.Value)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }

                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
                result.Files = Db.GetFiles(Db.BuyersRegisterFiles, result.Id.Value);
            }
            return result;
        }

        public BuyerEditDTO GetRegisterByApplicationId(int applicationId)
        {
            int buyerId = (from buyer in Db.BuyerRegisters
                           where buyer.ApplicationId == applicationId
                                 && buyer.RecordType == nameof(RecordTypesEnum.Register)
                           select buyer.Id).Single();

            return GetRegisterEntry(buyerId);
        }

        public BuyerEditDTO GetRegisterByChangeOfCircumstancesApplicationId(int applicationId)
        {
            int buyerId = (from change in Db.ApplicationChangeOfCircumstances
                           where change.ApplicationId == applicationId
                                 && change.IsActive
                           select change.BuyerId.Value).First();

            return GetRegisterEntry(buyerId);
        }

        public BuyerEditDTO GetEntryByApplicationId(int applicationId)
        {
            int buyerId = (from buyer in Db.BuyerRegisters
                           join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                           join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                           where buyer.ApplicationId == applicationId
                               && buyer.RecordType == nameof(RecordTypesEnum.Application)
                           select buyer.Id).Single();

            return GetRegisterEntry(buyerId);
        }

        public RegixChecksWrapperDTO<BuyerRegixDataDTO> GetRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<BuyerRegixDataDTO> regixData = new RegixChecksWrapperDTO<BuyerRegixDataDTO>
            {
                DialogDataModel = GetApplicationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetBuyersChecks(applicationId)
            };

            regixData.DialogDataModel.ApplicationRegiXChecks = regixData.RegiXDataModel.ApplicationRegiXChecks;

            return regixData;
        }

        public BuyerRegixDataDTO GetApplicationRegixData(int applicationId)
        {
            BuyersApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            BuyerRegixDataDTO regixData = new BuyerRegixDataDTO
            {
                Id = regixDataIds.BuyerId,
                ApplicationId = regixDataIds.ApplicationId,
                PageCode = regixDataIds.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(regixDataIds.ApplicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(regixDataIds.ApplicationId);

            if (regixDataIds.PageCode == PageCodeEnum.RegFirstSaleBuyer)
            {
                regixData.Agent = personService.GetRegixPersonData(regixDataIds.AgentPersonId.Value);
            }
            else if (regixDataIds.PageCode == PageCodeEnum.RegFirstSaleCenter)
            {
                if (regixDataIds.HasUtility)
                {
                    regixData.PremiseUsageDocument = GetPremiseUsageDocumentRegixData(regixDataIds.BuyerId);
                }

                regixData.Organizer = personService.GetRegixPersonData(regixDataIds.OrganizerPersonId.Value);
            }

            return regixData;
        }

        public int AddApplicationEntry(BuyerApplicationEditDTO buyerApplication, ApplicationStatusesEnum? nextManualStatus = null)
        {
            BuyerRegister dbBuyer;
            Application application;
            DateTime now = DateTime.Now;

            using (TransactionScope scope = new TransactionScope())
            {
                application = (from appl in Db.Applications
                               where appl.Id == buyerApplication.ApplicationId
                               select appl).First();

                int buyerTypeId = GetTypeIdByPageCode(buyerApplication.PageCode);
                int buyerStatusId = GetBuyerStatusId(BuyerStatusesEnum.Appl);

                dbBuyer = new BuyerRegister
                {
                    ApplicationId = buyerApplication.ApplicationId.Value,
                    BuyerTypeId = buyerTypeId,
                    RecordType = nameof(RecordTypesEnum.Application),
                    Comments = buyerApplication.Comments,
                    BuyerStatusId = buyerStatusId,
                    IsActive = true
                };

                Db.AddOrEditApplicationSubmittedBy(application, buyerApplication.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, buyerApplication.SubmittedFor);
                Db.SaveChanges();

                dbBuyer.SubmittedForLegalId = application.SubmittedForLegalId;
                dbBuyer.SubmittedForPersonId = application.SubmittedForPersonId;

                Db.BuyerRegisters.Add(dbBuyer);
                Db.SaveChanges();

                dbBuyer.HasUtility = buyerApplication.HasUtility.Value;
                dbBuyer.HasVehicle = buyerApplication.HasVehicle.Value;

                if (buyerApplication.HasUtility.Value)
                {
                    dbBuyer.UtilityName = buyerApplication.UtilityName;

                    Address premiseAddress = Db.AddOrEditAddress(buyerApplication.PremiseAddress, true);
                    dbBuyer.UtilityAddress = premiseAddress;

                    AddLicenseDocument(buyerApplication.BabhLawLicenseDocumnet, dbBuyer.Id, BuyerLicenseTypesEnum.Food);
                }
                else
                {
                    dbBuyer.UtilityName = null;
                }

                if (buyerApplication.HasVehicle.Value)
                {
                    dbBuyer.VehicleNumber = buyerApplication.VehicleNumber;
                    AddLicenseDocument(buyerApplication.VeteniraryVehicleRegLicenseDocument, dbBuyer.Id, BuyerLicenseTypesEnum.Vehicle);
                }
                else
                {
                    dbBuyer.VehicleNumber = null;
                }

                if (buyerApplication.PageCode == PageCodeEnum.RegFirstSaleBuyer)
                {
                    Person agent = Db.AddOrEditPerson(buyerApplication.Agent);
                    dbBuyer.Agent = agent;
                    Db.SaveChanges();

                    dbBuyer.IsAgentSameAsSubmittedBy = buyerApplication.IsAgentSameAsSubmittedBy;
                    dbBuyer.IsAgentSameAsSubmittedForCustodianOfProperty = buyerApplication.IsAgentSameAsSubmittedForCustodianOfProperty;
                }
                else if (buyerApplication.PageCode == PageCodeEnum.RegFirstSaleCenter)
                {
                    Person organizer = Db.AddOrEditPerson(buyerApplication.Organizer);
                    dbBuyer.OrganizingPerson = organizer;
                    Db.SaveChanges();

                    dbBuyer.IsOrganizingPersonSameAsSubmittedBy = buyerApplication.OrganizerSameAsSubmittedBy;

                    if (buyerApplication.HasUtility.Value)
                    {
                        AddPremiseUsageDocuments(dbBuyer, new List<UsageDocumentDTO> { buyerApplication.PremiseUsageDocument });
                    }
                }

                AddOrEditApplicationDeliveryData(application, buyerApplication);

                if (buyerApplication.Files != null)
                {
                    foreach (FileInfoDTO file in buyerApplication.Files)
                    {
                        Db.AddOrEditFile(dbBuyer, dbBuyer.BuyerRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> files = buyerApplication.Files;
            buyerApplication.Files = null;
            applicationStateMachine.Act(application.Id, CommonUtils.Serialize(buyerApplication), files, nextManualStatus);

            return dbBuyer.Id;
        }

        public int EditApplicationEntry(BuyerApplicationEditDTO buyerApplication, ApplicationStatusesEnum? manualStatus = null)
        {
            BuyerRegister dbBuyer;

            using (TransactionScope scope = new TransactionScope())
            {
                dbBuyer = (from b in Db.BuyerRegisters
                                .AsSplitQuery()
                                .Include(x => x.BuyerRegisterFiles)
                           where b.Id == buyerApplication.Id.Value
                           select b).First();

                dbBuyer.Comments = buyerApplication.Comments;


                Application application = (from appl in Db.Applications
                                           where appl.Id == dbBuyer.ApplicationId
                                           select appl).First();

                Db.AddOrEditApplicationSubmittedFor(application, buyerApplication.SubmittedFor);
                Db.SaveChanges();

                dbBuyer.SubmittedForPersonId = application.SubmittedForPersonId;
                dbBuyer.SubmittedForLegalId = application.SubmittedForLegalId;

                dbBuyer.HasUtility = buyerApplication.HasUtility.Value;
                dbBuyer.HasVehicle = buyerApplication.HasVehicle.Value;

                if (buyerApplication.HasUtility.Value)
                {
                    dbBuyer.UtilityName = buyerApplication.UtilityName;
                    dbBuyer.UtilityAddress = Db.AddOrEditAddress(buyerApplication.PremiseAddress, true, dbBuyer.UtilityAddressId);

                    AddOrEditLicenseDocument(buyerApplication.BabhLawLicenseDocumnet, dbBuyer.Id, BuyerLicenseTypesEnum.Food);
                }
                else
                {
                    dbBuyer.UtilityName = null;
                }

                if (buyerApplication.HasVehicle.Value)
                {
                    dbBuyer.VehicleNumber = buyerApplication.VehicleNumber;
                    AddOrEditLicenseDocument(buyerApplication.VeteniraryVehicleRegLicenseDocument, dbBuyer.Id, BuyerLicenseTypesEnum.Vehicle);
                }
                else
                {
                    dbBuyer.VehicleNumber = null;
                }

                if (buyerApplication.PageCode == PageCodeEnum.RegFirstSaleBuyer)
                {
                    dbBuyer.Agent = Db.AddOrEditPerson(buyerApplication.Agent, null, dbBuyer.AgentId);
                    Db.SaveChanges();

                    dbBuyer.IsAgentSameAsSubmittedBy = buyerApplication.IsAgentSameAsSubmittedBy;
                    dbBuyer.IsAgentSameAsSubmittedForCustodianOfProperty = buyerApplication.IsAgentSameAsSubmittedForCustodianOfProperty;
                }
                else if (buyerApplication.PageCode == PageCodeEnum.RegFirstSaleCenter)
                {
                    dbBuyer.OrganizingPerson = Db.AddOrEditPerson(buyerApplication.Organizer, null, dbBuyer.OrganizingPersonId);
                    Db.SaveChanges();
                    dbBuyer.IsOrganizingPersonSameAsSubmittedBy = buyerApplication.OrganizerSameAsSubmittedBy;

                    if (buyerApplication.HasUtility.Value)
                    {
                        AddOrEditPremiseUsageDocuments(dbBuyer, new List<UsageDocumentDTO> { buyerApplication.PremiseUsageDocument });
                    }
                }

                AddOrEditApplicationDeliveryData(application, buyerApplication);

                if (buyerApplication.Files != null)
                {
                    foreach (FileInfoDTO file in buyerApplication.Files)
                    {
                        Db.AddOrEditFile(dbBuyer, dbBuyer.BuyerRegisterFiles, file);
                    }
                }

                Db.AddOrEditApplicationSubmittedBy(application, buyerApplication.SubmittedBy);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> buyerFiles = null;

            if (buyerApplication.Files != null)
            {
                buyerFiles = buyerApplication.Files.ToList();
            }

            buyerApplication.Files = null;
            applicationStateMachine.Act(dbBuyer.ApplicationId, CommonUtils.Serialize(buyerApplication), buyerFiles, manualStatus, buyerApplication.StatusReason);

            return dbBuyer.Id;
        }

        public ApplicationStatusesEnum EditApplicationRegixData(BuyerRegixDataDTO buyer)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                BuyerRegister dbBuyer = (from b in Db.BuyerRegisters
                                         where b.Id == buyer.Id
                                         select b).First();

                application = (from appl in Db.Applications
                               where appl.Id == dbBuyer.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(application, buyer.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(application, buyer.SubmittedFor);
                Db.SaveChanges();

                dbBuyer.SubmittedForPersonId = application.SubmittedForPersonId;
                dbBuyer.SubmittedForLegalId = application.SubmittedForLegalId;

                if (buyer.PageCode == PageCodeEnum.RegFirstSaleBuyer)
                {
                    dbBuyer.Agent = Db.AddOrEditPerson(buyer.Agent, null, dbBuyer.AgentId);
                    Db.SaveChanges();
                }
                else if (buyer.PageCode == PageCodeEnum.RegFirstSaleCenter)
                {
                    dbBuyer.OrganizingPerson = Db.AddOrEditPerson(buyer.Organizer, null, dbBuyer.OrganizingPersonId);

                    if (dbBuyer.HasUtility.Value)
                    {
                        EditUsageDocuments(dbBuyer, new List<UsageDocumentRegixDataDTO> { buyer.PremiseUsageDocument });
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            ApplicationStatusesEnum newStatus = applicationStateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
            return newStatus;
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            PageCodeEnum[] buyers = new PageCodeEnum[] { PageCodeEnum.RegFirstSaleBuyer, PageCodeEnum.DupFirstSaleBuyer };
            PageCodeEnum[] centers = new PageCodeEnum[] { PageCodeEnum.RegFirstSaleCenter, PageCodeEnum.DupFirstSaleCenter };

            if (!buyers.Contains(pageCode) && !centers.Contains(pageCode))
            {
                throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
            }

            var data = (from aqua in Db.BuyerRegisters
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

            DownloadableFileDTO pdf = await GetRegisterFileForDownload(data.Id,
                                                                       buyers.Contains(pageCode) ? BuyerTypesEnum.Buyer : BuyerTypesEnum.CPP,
                                                                       pageCode == PageCodeEnum.DupFirstSaleBuyer || pageCode == PageCodeEnum.DupFirstSaleCenter);

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

        private BuyersApplicationDataIds GetApplicationDataIds(int applicationId)
        {
            var regixDataIds = (from buyer in Db.BuyerRegisters
                                join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where buyer.ApplicationId == applicationId
                                      && buyer.RecordType == nameof(RecordTypesEnum.Application)
                                select new BuyersApplicationDataIds
                                {
                                    ApplicationId = buyer.ApplicationId,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                    SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                    SubmittedByPersonRoleId = appl.SubmittedByPersonRoleId.Value,
                                    SubmittedForPersonId = appl.SubmittedForPersonId.HasValue ? appl.SubmittedForPersonId.Value : default,
                                    SubmittedForLegalId = appl.SubmittedForLegalId.HasValue ? appl.SubmittedForLegalId.Value : default,
                                    OrganizerPersonId = buyer.OrganizingPersonId,
                                    AgentPersonId = buyer.AgentId,
                                    BuyerId = buyer.Id,
                                    HasUtility = buyer.HasUtility.Value
                                }).Single();

            return regixDataIds;
        }

        private CommonDocumentDTO GetLicenseDocument(int buyerId, BuyerLicenseTypesEnum buyerLicenseType)
        {
            CommonDocumentDTO document = (from license in Db.BuyerLicenses
                                          join licenseType in Db.NbuyerLicenseTypes on license.BuyerLicenceTypeId equals licenseType.Id
                                          where license.BuyerId == buyerId && licenseType.Code == buyerLicenseType.ToString() && license.IsActive
                                          select new CommonDocumentDTO
                                          {
                                              Id = license.Id,
                                              IssueDate = license.IssueDate,
                                              Issuer = license.DocIssuer,
                                              Num = license.DocNum,
                                              ValidFrom = license.DocValidFrom,
                                              ValidTo = license.DocValidTo,
                                              IsIndefinite = license.IsUnlimited,
                                              Comments = license.Comment,
                                              IsActive = license.IsActive
                                          }).Single();

            return document;
        }

        private UsageDocumentDTO GetPremiseUsageDocument(int buyerId)
        {
            int id = (from buyerDoc in Db.BuyerPremiseUsageDocuments
                      where buyerDoc.BuyerId == buyerId
                         && buyerDoc.IsActive
                      select buyerDoc.UsageDocumentId).Single();

            UsageDocumentDTO document = usageDocumentsService.GetUsageDocument(id);
            return document;
        }

        private UsageDocumentRegixDataDTO GetPremiseUsageDocumentRegixData(int buyerId)
        {
            int id = (from buyerDoc in Db.BuyerPremiseUsageDocuments
                      where buyerDoc.BuyerId == buyerId
                         && buyerDoc.IsActive
                      select buyerDoc.UsageDocumentId).Single();

            UsageDocumentRegixDataDTO document = usageDocumentsService.GetUsageDocumentRegixData(id);
            return document;
        }

        private void EditUsageDocuments(BuyerRegister dbBuyer, List<UsageDocumentRegixDataDTO> documents)
        {
            List<BuyerPremiseUsageDocument> currentUsageDocuments = (from usgDoc in Db.BuyerPremiseUsageDocuments
                                                                     where usgDoc.BuyerId == dbBuyer.Id
                                                                     select usgDoc).ToList();

            if (documents != null)
            {
                foreach (UsageDocumentRegixDataDTO document in documents)
                {
                    BuyerPremiseUsageDocument dbDoc = currentUsageDocuments.Where(x => x.UsageDocumentId == document.Id).Single();
                    dbDoc.UsageDocument = Db.EditUsageDocument(document, dbBuyer);
                    dbDoc.IsActive = document.IsActive;
                }
            }
            else
            {
                foreach (BuyerPremiseUsageDocument document in currentUsageDocuments)
                {
                    document.IsActive = false;
                }
            }
        }

        private void AddOrEditApplicationDeliveryData(Application application, BuyerApplicationEditDTO buyer)
        {
            if (buyer.HasDelivery.Value)
            {
                if (application.DeliveryId.HasValue)
                {
                    application.Delivery = Db.EditDeliveryData(buyer.DeliveryData, application.DeliveryId.Value);
                }
                else
                {
                    application.Delivery = Db.AddDeliveryData(buyer.DeliveryData);
                }
            }
        }
    }
}
