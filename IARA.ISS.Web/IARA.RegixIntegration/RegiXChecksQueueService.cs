using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.TransferCapacity;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.Interfaces;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;

namespace IARA.RegixIntegration
{
    public class RegiXChecksQueueService : IRegiXChecksQueueService
    {
        internal static readonly PersonRegisterCheckTypes RegisterCheckType = PersonRegisterCheckTypes.GRAO;

        private IRegixAdapterService regixAdapterService;
        private ScopedServiceProviderFactory scopedServiceProviderFactory;

        public RegiXChecksQueueService(IRegixAdapterService regixAdapterService, ScopedServiceProviderFactory scopedServiceProviderFactory)
        {
            this.regixAdapterService = regixAdapterService;
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
        }

        public Task<bool> EnqueueScientificPermitChecks(int applicationId, ScientificFishingPermitRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddRegixPersonDataCheck(context, ContextCheckType.RequesterPerson, applicationData.Requester);
            AddRegixLegalAddressCheck(context, ContextCheckType.ReceiverPerson, applicationData.Receiver, null);

            foreach (var holder in applicationData.Holders)
            {
                if (holder.RegixPersonData != null)
                {
                    AddRegixPersonDataCheck(context, ContextCheckType.HolderPerson, holder.RegixPersonData, holder.AddressRegistrations);
                }
                else
                {
                    throw new ArgumentNullException(nameof(holder), $"{nameof(holder.RegixPersonData)} must not be null");
                }
            }

            return context.EnqueueAndFlushAll();
        }

        private CheckContext GetCheckContext(int applicationId, IScopedServiceProvider scopedServiceProvider)
        {
            IARADbContext db = scopedServiceProvider.GetRequiredService<IARADbContext>();

            DateTime now = DateTime.Now;

            var result = (from appl in db.Applications
                          join applHist in db.ApplicationChangeHistories on appl.Id equals applHist.ApplicationId
                          join appType in db.NapplicationTypes on appl.ApplicationTypeId equals appType.Id
                          join assignedUser in db.Users on appl.AssignedUserId equals assignedUser.Id into left
                          from assignedUser in left.DefaultIfEmpty()
                          join person in db.Persons on assignedUser.PersonId equals person.Id into left1
                          from person in left1.DefaultIfEmpty()
                          where applHist.ApplicationId == applicationId
                                               && applHist.ValidFrom <= now
                                               && applHist.ValidTo > now
                          select new
                          {
                              ApplicationHistoryId = applHist.Id,
                              ServiceURI = appl.EventisNum,
                              ServiceType = appType.Code,
                              EGN = person != null ? person.EgnLnc : default,
                              EmployeeNames = person != null ? $"{person.FirstName} {person.MiddleName} {person.LastName}" : default,
                              EmployeeIdentifier = assignedUser != null ? assignedUser.Email : default
                          }).First();

            CheckContext context = new CheckContext(applicationId,
                                                    result.ApplicationHistoryId,
                                                    result.ServiceType,
                                                    result.ServiceURI,
                                                    result.EGN,
                                                    result.EmployeeNames,
                                                    result.EmployeeIdentifier,
                                                    scopedServiceProvider,
                                                    regixAdapterService);
            return context;
        }

        public Task<bool> EnqueueShipChangeOfCircumstancesChecks(int applicationId, ShipChangeOfCircumstancesRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);
            AddChangeOfCircumstancesChecks(context, applicationData.Changes);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueShipDeregistrationChecks(int applicationId, ShipDeregistrationRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            if (applicationData.FreedCapacityAction != null && applicationData.FreedCapacityAction.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                AddFishingCapacityHoldersChecks(context, applicationData.FreedCapacityAction.Holders);
            }

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueRegisterShipChecks(int applicationId, ShipRegisterRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            foreach (var owner in applicationData.Owners)
            {
                if (owner.RegixPersonData != null)
                {
                    AddRegixPersonDataCheck(context, ContextCheckType.OwnerPerson, owner.RegixPersonData, owner.AddressRegistrations);
                }
                else if (owner.RegixLegalData != null)
                {
                    AddRegixLegalAddressCheck(context, ContextCheckType.OwnerLegal, owner.RegixLegalData, owner.AddressRegistrations);
                }
                else
                {
                    throw new ArgumentNullException(nameof(owner), $"{nameof(owner.RegixPersonData)} or {nameof(owner.RegixLegalData)} must not be null");
                }
            }

            if (applicationData.RemainingCapacityAction?.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                AddFishingCapacityHoldersChecks(context, applicationData.RemainingCapacityAction.Holders);
            }

            //AddVesselChecks(context, applicationData);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueQualifiedFisherChecks(int applicationId, QualifiedFisherRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            var submittedBy = new ApplicationSubmittedByRegixDataDTO
            {
                Person = applicationData.SubmittedByRegixData,
                Addresses = applicationData.SubmittedByAddresses
            };

            var submittedFor = new ApplicationSubmittedForRegixDataDTO
            {
                Person = applicationData.SubmittedForRegixData,
                Addresses = applicationData.SubmittedForAddresses
            };

            AddChecksForSubmittedForSubmittedBy(context, submittedBy, submittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueTicketChecks(int applicationId, RecreationalFishingTicketBaseRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddRegixPersonDataCheck(context,
                                 ContextCheckType.RequesterPerson,
                                 applicationData.Person,
                                 applicationData.PersonAddressRegistrations);

            if (applicationData.RepresentativePerson != null)
            {
                AddRegixPersonDataCheck(context,
                               ContextCheckType.RepresentativePerson,
                               applicationData.RepresentativePerson,
                               applicationData.RepresentativePersonAddressRegistrations);

            }

            if (applicationData.TelkData != null)
            {
                context.AddLastExpertDecisionCheck(new RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO>(context)
                {
                    AdditionalIdentifier = applicationData.TelkData.Num.ToString(),
                    Context = new GetLastExpertDecisionByIdentifierRequest
                    {
                        Identifier = applicationData.Person.EgnLnc.EgnLnc
                    },
                    Type = ContextCheckType.DisabledPerson,
                    CompareWithObject = applicationData.TelkData
                });
            }

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueLegalEntitiesChecks(int applicationId, LegalEntityRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddRegixPersonDataCheck(context,
                                    ContextCheckType.RequesterPerson,
                                    applicationData.Requester,
                                    applicationData.RequesterAddresses);


            AddRegixLegalAddressCheck(context, ContextCheckType.Legal, applicationData.Legal, applicationData.Addresses);

            foreach (var authorizedPerson in applicationData.AuthorizedPeople)
            {
                AddRegixPersonDataCheck(context, ContextCheckType.LegalAuthorizedPerson, authorizedPerson.Person);
            }

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueFishingCapacityTransferChecks(int applicationId, TransferFishingCapacityRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);
            AddFishingCapacityHoldersChecks(context, applicationData.Holders);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueFishingCapacityDuplicateChecks(int applicationId, CapacityCertificateDuplicateRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueFishingCapacityReduceChecks(int applicationId, ReduceFishingCapacityRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            if (applicationData.FreedCapacityAction.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                AddFishingCapacityHoldersChecks(context, applicationData.FreedCapacityAction.Holders);
            }

            return context.EnqueueAndFlushAll(); ;
        }

        public Task<bool> EnqueueFishingCapacityIncreseChecks(int applicationId, IncreaseFishingCapacityRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            if (applicationData.RemainingCapacityAction.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                AddFishingCapacityHoldersChecks(context, applicationData.RemainingCapacityAction.Holders);
            }

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueAquacultureRegistrationChecks(int applicationId, AquacultureRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);
            AddUsageDocumentChecks(context, applicationData.UsageDocument);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueAquacultureDeregistrationChecks(int applicationId, AquacultureDeregistrationRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueAquacultureChangeOfCircumstancesChecks(int applicationId, AquacultureChangeOfCircumstancesRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            AddChangeOfCircumstancesChecks(context, applicationData.Changes);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueCommercialFishingCheck(int applicationId, CommercialFishingBaseRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueBuyersChecks(int applicationId, BuyerRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            if (applicationData.Agent != null)
            {
                AddRegixPersonDataCheck(context, ContextCheckType.AgentPerson, applicationData.Agent);
            }

            if (applicationData.Organizer != null)
            {
                AddRegixPersonDataCheck(context, ContextCheckType.OrganizerPerson, applicationData.Organizer);
            }

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            AddUsageDocumentChecks(context, applicationData.PremiseUsageDocument);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueBuyersChangeChecks(int applicationId, BuyerChangeOfCircumstancesRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueBuyersTerminationChecks(int applicationId, BuyerTerminationRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueStatisticalFormAquaFarmChecks(int applicationId, StatisticalFormAquaFarmRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueStatisticalFormFishVesselChecks(int applicationId, StatisticalFormFishVesselRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueStatisticalFormReworkChecks(int applicationId, StatisticalFormReworkRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        public Task<bool> EnqueueDuplicateApplicationChecks(int applicationId, DuplicatesApplicationRegixDataDTO applicationData)
        {
            using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();

            CheckContext context = GetCheckContext(applicationId, scopedServiceProvider);

            AddChecksForSubmittedForSubmittedBy(context, applicationData.SubmittedBy, applicationData.SubmittedFor);

            return context.EnqueueAndFlushAll();
        }

        private void AddVesselChecks(CheckContext context, ShipRegisterRegixDataDTO applicationData)
        {
            context.AddVesselCheck(new RegixContextData<VesselRequest, VesselContext>(context)
            {
                AdditionalIdentifier = Guid.NewGuid().ToString(),//RANDOM Identifier
                Context = new VesselRequest
                {
                    OwnersBulstatEGN = applicationData.Owners.Select(x => x.EgnLncEik).ToList(),
                    VesselType = applicationData.VesselTypeId,
                    TotalLength = applicationData.TotalLength,
                    LengthBetweenPerpendiculars = applicationData.LengthBetweenPerpendiculars,
                    RegistrationPage = applicationData.RegLicencePublishPage,
                    RegistrationVolume = applicationData.RegLicencePublishVolume,
                    MainEngineNumber = applicationData.MainEngineNum,
                    HullNumber = applicationData.HullNumber
                },
                Type = ContextCheckType.Vessel,
                CompareWithObject = new VesselContext
                {
                    VesselData = applicationData,
                    Owners = applicationData.Owners
                }
            });
        }

        private void AddUsageDocumentChecks(CheckContext context, UsageDocumentRegixDataDTO usageDocument)
        {
            if (usageDocument != null && usageDocument.IsLessorPerson != null)
            {
                if (usageDocument.IsLessorPerson.Value)
                {
                    AddRegixPersonDataCheck(context,
                                            ContextCheckType.LessorPerson,
                                            usageDocument.LessorPerson,
                                            usageDocument.LessorAddresses);
                }
                else
                {
                    AddRegixLegalAddressCheck(context,
                                              ContextCheckType.LessorLegal,
                                              usageDocument.LessorLegal,
                                              usageDocument.LessorAddresses);
                }
            }
        }

        private void AddChangeOfCircumstancesChecks(CheckContext context, List<ChangeOfCircumstancesDTO> changes)
        {
            if (changes != null)
            {
                foreach (var change in changes.Where(x => x.DataType == ChangeOfCircumstancesDataTypeEnum.Person))
                {
                    List<AddressRegistrationDTO> addresses = null;

                    if (change.Address != null)
                    {
                        addresses = new List<AddressRegistrationDTO> { change.Address };
                    }

                    AddRegixPersonDataCheck(context, ContextCheckType.ChangePerson, change.Person, addresses);
                }

                foreach (var change in changes.Where(x => x.DataType == ChangeOfCircumstancesDataTypeEnum.Legal))
                {
                    List<AddressRegistrationDTO> addresses = null;

                    if (change.Address != null)
                    {
                        addresses = new List<AddressRegistrationDTO> { change.Address };
                    }

                    AddRegixLegalAddressCheck(context, ContextCheckType.ChangeLegal, change.Legal, addresses);
                }
            }
        }

        private void AddFishingCapacityHoldersChecks(CheckContext context, List<FishingCapacityHolderRegixDataDTO> holders)
        {
            foreach (var holder in holders)
            {
                if (holder.Person != null)
                {
                    AddRegixPersonDataCheck(context, ContextCheckType.HolderPerson, holder.Person, holder.Addresses);
                }
                else if (holder.Legal != null)
                {
                    AddRegixLegalAddressCheck(context, ContextCheckType.HolderLegal, holder.Legal, holder.Addresses);
                }
                else
                {
                    throw new ArgumentNullException(nameof(holder), $"{nameof(holder.Person)} or {nameof(holder.Legal)} must not be null");
                }
            }
        }

        private void AddChecksForSubmittedForSubmittedBy(CheckContext context, ApplicationSubmittedByRegixDataDTO submittedBy, ApplicationSubmittedForRegixDataDTO submittedFor)
        {

            if (submittedBy != null)
            {
                AddRegixPersonDataCheck(context, ContextCheckType.SubmittedByPerson, submittedBy.Person, submittedBy.Addresses);
            }

            if (submittedFor != null)
            {
                if (submittedFor.Person != null)
                {
                    AddRegixPersonDataCheck(context, ContextCheckType.SubmittedForPerson, submittedFor.Person, submittedFor.Addresses);
                }
                else if (submittedFor.Legal != null)
                {
                    AddRegixLegalAddressCheck(context, ContextCheckType.SubmittedForLegal, submittedFor.Legal, submittedFor.Addresses);
                }
                else
                {
                    throw new ArgumentNullException(nameof(submittedFor), $"{nameof(submittedFor.Person)} or {nameof(submittedFor.Legal)} must not be null");
                }
            }
        }

        private void AddRegixLegalAddressCheck(CheckContext context, ContextCheckType checkType, RegixLegalDataDTO legal, List<AddressRegistrationDTO> addresses = null)
        {
            context.AddActualStateCheck(new RegixContextData<ActualStateRequestType, RegixLegalContext>(context)
            {
                AdditionalIdentifier = legal.EIK,
                Type = checkType,
                CompareWithObject = new RegixLegalContext
                {
                    Legal = legal,
                    Addresses = addresses
                },
                Context = new ActualStateRequestType
                {
                    UIC = legal.EIK
                }
            });
        }

        private void AddRegixPersonDataCheck(CheckContext context, ContextCheckType type, RegixPersonDataDTO person, List<AddressRegistrationDTO> addresses = null)
        {
            switch (person.EgnLnc.IdentifierType)
            {
                case IdentifierTypeEnum.EGN:
                    {
                        switch (RegisterCheckType)
                        {
                            case PersonRegisterCheckTypes.MVR:
                                {
                                    context.AddPersonIdentityCheck(new RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext>(context)
                                    {
                                        AdditionalIdentifier = person.EgnLnc.ToString(),
                                        Type = type,
                                        Context = new PersonalIdentityInfoRequestType
                                        {
                                            EGN = person.EgnLnc.EgnLnc,
                                            IdentityDocumentNumber = person?.Document?.DocumentNumber
                                        },
                                        CompareWithObject = new RegixPersonContext
                                        {
                                            Person = person,
                                            Addresses = addresses
                                        }
                                    });
                                }
                                break;
                            case PersonRegisterCheckTypes.GRAO:
                                {
                                    context.AddPersonDataCheck(new RegixContextData<PersonDataRequestType, RegixPersonContext>(context)
                                    {
                                        AdditionalIdentifier = person.EgnLnc.ToString(),
                                        Type = type,
                                        Context = new PersonDataRequestType
                                        {
                                            EGN = person.EgnLnc.EgnLnc
                                        },
                                        CompareWithObject = new RegixPersonContext
                                        {
                                            Person = person,
                                            Addresses = addresses
                                        }
                                    });

                                    if (addresses != null)
                                    {
                                        context.AddPersonPermanentAddressCheck(new RegixContextData<PermanentAddressRequestType, RegixPersonContext>(context)
                                        {
                                            AdditionalIdentifier = person.EgnLnc.ToString(),
                                            Type = type + 1,
                                            Context = new PermanentAddressRequestType
                                            {
                                                EGN = person.EgnLnc.EgnLnc,
                                                SearchDate = DateTime.Now
                                            },
                                            CompareWithObject = new RegixPersonContext
                                            {
                                                Person = person,
                                                Addresses = addresses
                                            }
                                        });
                                    }
                                }
                                break;
                            default:
                                throw new NotImplementedException($"Person check of type: {RegisterCheckType} is not implemented");
                        }
                    }
                    break;
                case IdentifierTypeEnum.LNC:
                    {
                        context.AddForeignPersonCheck(new RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext>(context)
                        {
                            AdditionalIdentifier = person.EgnLnc.ToString(),
                            Type = type,
                            Context = new ForeignIdentityInfoRequestType
                            {
                                IdentifierType = IdentifierType.LNCh,
                                Identifier = person.EgnLnc.EgnLnc
                            },
                            CompareWithObject = new RegixPersonContext
                            {
                                Person = person,
                                Addresses = addresses
                            }
                        });
                    }
                    break;
            }
        }

    }
}
