using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Utils;
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
using IARA.EntityModels.Entities;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;

namespace IARA.Infrastructure.RegiX
{
    public class RegixApplicationInterfaceService : BaseService, IRegixApplicationInterfaceService
    {
        internal static readonly PersonRegisterCheckTypes RegisterCheckType = PersonRegisterCheckTypes.GRAO;

        public RegixApplicationInterfaceService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public BuyerChangeOfCircumstancesRegixDataDTO GetBuyerChangeOfCircumstancesChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<BuyerChangeOfCircumstancesRegixDataDTO>(applicationId,
                                                                                            PageCodeEnum.ChangeFirstSaleBuyer,
                                                                                            out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);
            regixChecksData.Changes = GetChangeOfCircumstancesChecks(checks);

            return regixChecksData;
        }

        public BuyerChangeOfCircumstancesRegixDataDTO GetFirstSaleCenterChangeOfCircumstancesChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<BuyerChangeOfCircumstancesRegixDataDTO>(applicationId,
                                                                                    PageCodeEnum.ChangeFirstSaleCenter,
                                                                                    out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);
            regixChecksData.Changes = GetChangeOfCircumstancesChecks(checks);

            return regixChecksData;
        }

        public BuyerTerminationRegixDataDTO GetBuyerTerminationChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<BuyerTerminationRegixDataDTO>(applicationId,
                                                                                PageCodeEnum.TermFirstSaleBuyer,
                                                                                out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public BuyerTerminationRegixDataDTO GetFirstSaleCenterTerminationChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<BuyerTerminationRegixDataDTO>(applicationId,
                                                                                PageCodeEnum.TermFirstSaleCenter,
                                                                                out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public AquacultureChangeOfCircumstancesRegixDataDTO GetAquacultureChangeOfCircumstancesChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<AquacultureChangeOfCircumstancesRegixDataDTO>(applicationId,
                                                                                                  PageCodeEnum.AquaFarmChange,
                                                                                                  out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);
            regixChecksData.Changes = GetChangeOfCircumstancesChecks(checks);

            return regixChecksData;
        }

        public AquacultureRegixDataDTO GetAquacultureRegistrationChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<AquacultureRegixDataDTO>(applicationId,
                                                                             PageCodeEnum.AquaFarmReg,
                                                                             out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            var lessorPerson = GetRegixSinglePersonCheck(checks, ContextCheckType.LessorPerson);

            if (lessorPerson != null)
            {
                regixChecksData.UsageDocument = new UsageDocumentRegixDataDTO
                {
                    LessorPerson = lessorPerson.Person,
                    LessorAddresses = lessorPerson.Addresses,
                    IsLessorPerson = true
                };
            }
            else
            {
                var lessorLegal = GetRegixSingleLegalCheck(checks, ContextCheckType.LessorLegal);

                if (lessorLegal != null)
                {
                    regixChecksData.UsageDocument = new UsageDocumentRegixDataDTO
                    {
                        LessorLegal = lessorLegal.Legal,
                        LessorAddresses = lessorLegal.Addresses,
                        IsLessorPerson = false
                    };
                }
            }


            return regixChecksData;
        }

        public AquacultureDeregistrationRegixDataDTO GetAquacultureDeregistrationChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<AquacultureDeregistrationRegixDataDTO>(applicationId,
                                                                                           PageCodeEnum.AquaFarmDereg,
                                                                                           out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public BuyerRegixDataDTO GetBuyersChecks(int applicationId)
        {
            List<ApplicationRegiXcheck> checks = GetCurrentApplicationChecks(applicationId);

            var regixChecksData = new BuyerRegixDataDTO
            {
                ApplicationRegiXChecks = MapToRegixChecksDto(checks),
                ApplicationId = applicationId,
                PageCode = PageCodeEnum.Buyers
            };

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);
            regixChecksData.Organizer = GetRegixSinglePersonCheck(checks, ContextCheckType.OrganizerPerson)?.Person;
            regixChecksData.Agent = GetRegixSinglePersonCheck(checks, ContextCheckType.AgentPerson)?.Person;

            var lessorPerson = GetRegixSinglePersonCheck(checks, ContextCheckType.LessorPerson);

            if (lessorPerson != null)
            {
                regixChecksData.PremiseUsageDocument = new UsageDocumentRegixDataDTO
                {
                    LessorPerson = lessorPerson.Person,
                    LessorAddresses = lessorPerson.Addresses,
                    IsLessorPerson = true
                };
            }
            else
            {
                var lessorLegal = GetRegixSingleLegalCheck(checks, ContextCheckType.LessorLegal);

                if (lessorLegal != null)
                {
                    regixChecksData.PremiseUsageDocument = new UsageDocumentRegixDataDTO
                    {
                        LessorLegal = lessorLegal.Legal,
                        LessorAddresses = lessorLegal.Addresses,
                        IsLessorPerson = false
                    };
                }
            }

            return regixChecksData;
        }

        public CommercialFishingRegixDataDTO GetCommercialFishingChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<CommercialFishingRegixDataDTO>(applicationId, PageCodeEnum.CommFish, out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public StatisticalFormFishVesselRegixDataDTO GetStatisticalFormFishVesselChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<StatisticalFormFishVesselRegixDataDTO>(applicationId,
                                                                                           PageCodeEnum.StatFormRework,
                                                                                           out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public StatisticalFormReworkRegixDataDTO GetStatisticalFormReworkChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<StatisticalFormReworkRegixDataDTO>(applicationId,
                                                                                       PageCodeEnum.StatFormRework,
                                                                                       out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public StatisticalFormAquaFarmRegixDataDTO GetStatisticalFormAquaFarmChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<StatisticalFormAquaFarmRegixDataDTO>(applicationId,
                                                                                         PageCodeEnum.StatFormRework,
                                                                                         out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public List<ApplicationRegiXcheck> GetCurrentApplicationChecks(int applicationId)
        {
            var now = DateTime.Now;

            List<ApplicationRegiXcheck> result = (from check in Db.ApplicationRegiXchecks
                                                  join appHist in Db.ApplicationChangeHistories on check.ApplicationId equals appHist.ApplicationId
                                                  where check.ApplicationId == applicationId
                                                     && appHist.ValidFrom <= now
                                                     && appHist.ValidTo > now
                                                  orderby check.RequestDateTime descending
                                                  select check).ToList();

            return result;
        }

        public IncreaseFishingCapacityRegixDataDTO GetFishingCapacityIncreaseChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<IncreaseFishingCapacityRegixDataDTO>(applicationId,
                                                                                         PageCodeEnum.IncreaseFishCap,
                                                                                         out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            regixChecksData.RemainingCapacityAction = new FishingCapacityFreedActionsRegixDataDTO
            {
                Action = FishingCapacityRemainderActionEnum.Transfer,
                Holders = GetFishingCapacityHolders(checks)
            };

            return regixChecksData;
        }

        public ReduceFishingCapacityRegixDataDTO GetFishingCapacityReduceChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<ReduceFishingCapacityRegixDataDTO>(applicationId,
                                                                                       PageCodeEnum.ReduceFishCap,
                                                                                       out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            regixChecksData.FreedCapacityAction = new FishingCapacityFreedActionsRegixDataDTO
            {
                Action = FishingCapacityRemainderActionEnum.Transfer,
                Holders = GetFishingCapacityHolders(checks)
            };

            return regixChecksData;
        }

        public TransferFishingCapacityRegixDataDTO GetFishingCapacityTransferChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<TransferFishingCapacityRegixDataDTO>(applicationId,
                                                                                  PageCodeEnum.TransferFishCap,
                                                                                  out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);
            regixChecksData.Holders = GetFishingCapacityHolders(checks);

            return regixChecksData;
        }

        public CapacityCertificateDuplicateRegixDataDTO GetFishingCapacityDuplicateChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<CapacityCertificateDuplicateRegixDataDTO>(applicationId,
                                                                                              PageCodeEnum.CapacityCertDup,
                                                                                              out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public DuplicatesApplicationRegixDataDTO GetDuplicateApplicationChecks(int applicationId, PageCodeEnum pageCode)
        {
            var regixChecksData = GetRegixChecksDTO<DuplicatesApplicationRegixDataDTO>(applicationId,
                                                                                       pageCode,
                                                                                       out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            return regixChecksData;
        }

        public LegalEntityRegixDataDTO GetLegalChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<LegalEntityRegixDataDTO>(applicationId,
                                                                        PageCodeEnum.LE,
                                                                        out List<ApplicationRegiXcheck> checks);

            var requester = GetRegixSinglePersonCheck(checks, ContextCheckType.RequesterPerson);

            regixChecksData.Requester = requester.Person;
            regixChecksData.RequesterAddresses = requester.Addresses;

            var legal = GetRegixSingleLegalCheck(checks, ContextCheckType.Legal);

            regixChecksData.Legal = legal.Legal;
            regixChecksData.Addresses = legal.Addresses;

            var authorizedPersons = GetRegixPersonChecks(checks, ContextCheckType.LegalAuthorizedPerson);

            regixChecksData.AuthorizedPeople = new List<AuthorizedPersonRegixDataDTO>();

            foreach (var person in authorizedPersons)
            {
                regixChecksData.AuthorizedPeople.Add(new AuthorizedPersonRegixDataDTO
                {
                    Person = person.Person,
                    FullName = $"{person.Person.FirstName} {person.Person.MiddleName} {person.Person.LastName}"
                });
            }

            return regixChecksData;
        }

        public QualifiedFisherRegixDataDTO GetQualifiedFisherChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<QualifiedFisherRegixDataDTO>(applicationId,
                                                                       PageCodeEnum.QualiFi,
                                                                       out List<ApplicationRegiXcheck> checks);


            var submittedBy = GetApplicationSubmittedBy(checks);
            var submittedFor = GetApplicationSubmittedFor(checks);

            regixChecksData.SubmittedByRegixData = submittedBy.Person;
            regixChecksData.SubmittedByAddresses = submittedBy.Addresses;

            regixChecksData.SubmittedForRegixData = submittedFor.Person;
            regixChecksData.SubmittedForAddresses = submittedFor.Addresses;

            return regixChecksData;
        }

        public List<ApplicationRegiXcheck> GetRegixChecks(int applicationId, int applicationHistoryId)
        {
            var result = (from check in Db.ApplicationRegiXchecks
                          where check.ApplicationId == applicationId
                             && check.ApplicationChangeHistoryId == applicationHistoryId
                          orderby check.RequestDateTime descending
                          select check).ToList();

            return result;
        }

        public ScientificFishingPermitRegixDataDTO GetScientificFishingPermitChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<ScientificFishingPermitRegixDataDTO>(applicationId,
                                                         PageCodeEnum.SciFi,
                                                         out List<ApplicationRegiXcheck> checks);

            regixChecksData.Requester = GetRegixSinglePersonCheck(checks, ContextCheckType.RequesterPerson)?.Person;

            RegixLegalContext receiver = GetRegixSingleLegalCheck(checks, ContextCheckType.ReceiverPerson);
            if (receiver != null)
            {
                regixChecksData.Receiver = receiver.Legal;
            }

            regixChecksData.Holders = new List<ScientificFishingPermitHolderRegixDataDTO>();

            foreach (RegixPersonContext holder in GetRegixPersonChecks(checks, ContextCheckType.HolderPerson))
            {
                regixChecksData.Holders.Add(new ScientificFishingPermitHolderRegixDataDTO
                {
                    AddressRegistrations = holder.Addresses,
                    RegixPersonData = holder.Person
                });
            }

            return regixChecksData;
        }

        public ShipChangeOfCircumstancesRegixDataDTO GetShipChangeOfCircumstancesChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<ShipChangeOfCircumstancesRegixDataDTO>(applicationId,
                                                           PageCodeEnum.ShipRegChange,
                                                           out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);
            regixChecksData.Changes = GetChangeOfCircumstancesChecks(checks);

            return regixChecksData;
        }

        public ShipDeregistrationRegixDataDTO GetShipDeregistrationChecks(int applicationId)
        {
            var regixChecksData = GetRegixChecksDTO<ShipDeregistrationRegixDataDTO>(applicationId,
                                                                     PageCodeEnum.DeregShip,
                                                                     out List<ApplicationRegiXcheck> checks);

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            var holders = GetFishingCapacityHolders(checks);

            if (holders != null)
            {
                regixChecksData.FreedCapacityAction = new FishingCapacityFreedActionsRegixDataDTO
                {
                    Action = FishingCapacityRemainderActionEnum.Transfer,
                    Holders = holders,
                };
            }

            return regixChecksData;
        }

        public ShipRegisterRegixDataDTO GetShipRegisterChecks(int applicationId)
        {
            List<ApplicationRegiXcheck> checks = GetCurrentApplicationChecks(applicationId);

            var regixChecksData = new ShipRegisterRegixDataDTO
            {
                ApplicationRegiXChecks = MapToRegixChecksDto(checks),
                ApplicationId = applicationId,
                PageCode = PageCodeEnum.RegVessel
            };

            regixChecksData.SubmittedBy = GetApplicationSubmittedBy(checks);
            regixChecksData.SubmittedFor = GetApplicationSubmittedFor(checks);

            regixChecksData.Owners = new List<ShipOwnerRegixDataDTO>();

            foreach (var person in GetRegixPersonChecks(checks, ContextCheckType.OwnerPerson))
            {
                regixChecksData.Owners.Add(new ShipOwnerRegixDataDTO
                {
                    RegixPersonData = person.Person,
                    AddressRegistrations = person.Addresses,
                    IsOwnerPerson = true,
                    EgnLncEik = person.Person.EgnLnc.EgnLnc,
                    Names = $"{person.Person.FirstName} {person.Person.MiddleName} {person.Person.LastName}"
                });
            }

            foreach (var legal in GetRegixLegalChecks(checks, ContextCheckType.OwnerLegal))
            {
                regixChecksData.Owners.Add(new ShipOwnerRegixDataDTO
                {
                    RegixLegalData = legal.Legal,
                    AddressRegistrations = legal.Addresses,
                    IsOwnerPerson = false,
                    EgnLncEik = legal.Legal.EIK,
                    Names = legal.Legal.Name
                });
            }

            var vesselCheck = checks.Where(x => x.CheckType == (int)ContextCheckType.Vessel)
                                    .OrderBy(x => x.RequestDateTime)
                                    .LastOrDefault();

            if (vesselCheck != null && vesselCheck.ResponseContent != null)
            {
                var response = CommonUtils.Deserialize<VesselContext>(vesselCheck.ResponseContent);

                if (response != null && response.VesselData != null)
                {
                    var vessel = response.VesselData;

                    regixChecksData.BoardHeight = vessel.BoardHeight;
                    regixChecksData.TotalWidth = vessel.TotalWidth;
                    regixChecksData.CFR = vessel.CFR;
                    regixChecksData.GrossTonnage = vessel.GrossTonnage;
                    regixChecksData.HullNumber = vessel.HullNumber;
                    regixChecksData.LengthBetweenPerpendiculars = vessel.LengthBetweenPerpendiculars;
                    regixChecksData.MainEngineNum = vessel.MainEngineNum;
                    regixChecksData.Name = vessel.Name;
                    regixChecksData.NetTonnage = vessel.NetTonnage;
                    regixChecksData.RegLicenceNum = vessel.RegLicenceNum;
                    regixChecksData.RegLicencePublishPage = vessel.RegLicencePublishVolume;
                    regixChecksData.RegLicencePublishVolume = vessel.RegLicencePublishVolume;
                    regixChecksData.ShipDraught = vessel.ShipDraught;
                    regixChecksData.TotalLength = vessel.TotalLength;
                }
            }

            return regixChecksData;
        }

        public RecreationalFishingTicketBaseRegixDataDTO GetTicketChecks(int applicationId)
        {
            List<ApplicationRegiXcheck> checks = GetCurrentApplicationChecks(applicationId);

            var requestor = GetRegixSinglePersonCheck(checks, ContextCheckType.RequesterPerson);
            var representative = GetRegixSinglePersonCheck(checks, ContextCheckType.RepresentativePerson);

            var regixChecksData = new RecreationalFishingTicketBaseRegixDataDTO
            {
                RepresentativePerson = representative?.Person,
                ApplicationRegiXChecks = MapToRegixChecksDto(checks),
                ApplicationId = applicationId,
                Person = requestor.Person,
                PersonAddressRegistrations = requestor.Addresses,
                RepresentativePersonAddressRegistrations = representative?.Addresses,
            };

            return regixChecksData;
        }

        private ApplicationSubmittedByRegixDataDTO GetApplicationSubmittedBy(List<ApplicationRegiXcheck> checks)
        {
            var submittedByCheck = checks.Where(x => x.CheckType == (int)ContextCheckType.SubmittedByPerson).First();

            var personData = GetRegixSinglePersonCheck(checks, ContextCheckType.SubmittedByPerson);

            return new ApplicationSubmittedByRegixDataDTO
            {
                Person = personData.Person,
                Addresses = personData.Addresses
            };
        }

        private ApplicationSubmittedForRegixDataDTO GetApplicationSubmittedFor(List<ApplicationRegiXcheck> checks)
        {
            var personData = GetRegixSinglePersonCheck(checks, ContextCheckType.SubmittedForPerson);

            if (personData != null)
            {
                return new ApplicationSubmittedForRegixDataDTO
                {
                    Person = personData.Person,
                    Addresses = personData.Addresses
                };
            }
            else
            {
                var legaData = GetRegixSingleLegalCheck(checks, ContextCheckType.SubmittedForLegal);

                return new ApplicationSubmittedForRegixDataDTO
                {
                    Legal = legaData.Legal,
                    Addresses = legaData.Addresses
                };
            }
        }

        private List<ChangeOfCircumstancesDTO> GetChangeOfCircumstancesChecks(List<ApplicationRegiXcheck> checks)
        {
            var changes = new List<ChangeOfCircumstancesDTO>();

            foreach (var changeCheck in checks.Where(x => x.CheckType == (int)ContextCheckType.ChangePerson))
            {
                var response = CommonUtils.Deserialize<PersonalIdentityInfoResponseType>(changeCheck.ResponseContent);
                RegixPersonContext personData = RegixDataMappers.MapPersonIdentityInfoResponse(response);

                changes.Add(new ChangeOfCircumstancesDTO
                {
                    Person = personData.Person,
                    Address = personData.Addresses?.FirstOrDefault()
                });
            }

            foreach (var changeCheck in checks.Where(x => x.CheckType == (int)ContextCheckType.ChangeLegal))
            {
                var response = CommonUtils.Deserialize<ActualStateResponseType>(changeCheck.ResponseContent);
                RegixLegalContext personData = RegixDataMappers.MapActualStateResponse(response);

                changes.Add(new ChangeOfCircumstancesDTO
                {
                    Legal = personData.Legal,
                    Address = personData.Addresses?.FirstOrDefault()
                });
            }

            return changes;
        }

        private List<FishingCapacityHolderRegixDataDTO> GetFishingCapacityHolders(List<ApplicationRegiXcheck> checks)
        {
            List<FishingCapacityHolderRegixDataDTO> holders = new List<FishingCapacityHolderRegixDataDTO>();

            foreach (var check in GetRegixLegalChecks(checks, ContextCheckType.HolderLegal))
            {
                holders.Add(new FishingCapacityHolderRegixDataDTO
                {
                    Legal = check.Legal,
                    Addresses = check.Addresses,
                });
            }

            foreach (var check in GetRegixPersonChecks(checks, ContextCheckType.HolderPerson))
            {
                holders.Add(new FishingCapacityHolderRegixDataDTO
                {
                    Person = check.Person,
                    Addresses = check.Addresses
                });
            }

            return holders;
        }

        private T GetRegixChecksDTO<T>(int applicationId, PageCodeEnum pageCode, out List<ApplicationRegiXcheck> checks)
            where T : BaseRegixChecksDTO, new()
        {

            checks = GetCurrentApplicationChecks(applicationId);

            var regixChecksData = new T
            {
                ApplicationRegiXChecks = MapToRegixChecksDto(checks),
                ApplicationId = applicationId,
                PageCode = pageCode
            };

            return regixChecksData;
        }

        private List<RegixLegalContext> GetRegixLegalChecks(List<ApplicationRegiXcheck> checks, ContextCheckType type)
        {
            List<RegixLegalContext> legals = new List<RegixLegalContext>(Math.Min(checks.Count, 100));

            foreach (var legalCheck in checks.Where(x => x.CheckType == (int)type))
            {
                var response = CommonUtils.Deserialize<ActualStateResponseType>(legalCheck.ResponseContent);
                RegixLegalContext legalData = RegixDataMappers.MapActualStateResponse(response);
                legals.Add(legalData);
            }

            return legals;
        }

        private List<RegixPersonContext> GetRegixPersonChecks(List<ApplicationRegiXcheck> checks, ContextCheckType type)
        {
            List<RegixPersonContext> persons = new List<RegixPersonContext>(Math.Min(checks.Count, 100));

            foreach (var personCheck in checks.Where(x => x.CheckType == (int)type))
            {
                RegixPersonContext personData = null;
                string[] identifierParts = personCheck.AdditianalIdentifier.Split('|');
                IdentifierTypeEnum identifierType = Enum.Parse<IdentifierTypeEnum>(identifierParts[0]);

                string additionalIdentifier = identifierParts[1];

                switch (identifierType)
                {
                    case IdentifierTypeEnum.EGN:
                        {
                            switch (RegisterCheckType)
                            {
                                case PersonRegisterCheckTypes.MVR:
                                    {
                                        var response = CommonUtils.Deserialize<PersonalIdentityInfoResponseType>(personCheck.ResponseContent);
                                        personData = RegixDataMappers.MapPersonIdentityInfoResponse(response);
                                    }
                                    break;
                                case PersonRegisterCheckTypes.GRAO:
                                    {
                                        var personResponse = CommonUtils.Deserialize<PersonDataResponseType>(personCheck.ResponseContent);
                                        personData = RegixDataMappers.MapPersonDataSearchResponse(personResponse);

                                        var addresses = checks.Where(x => x.CheckType == ((int)type) + 1 && x.AdditianalIdentifier == personCheck.AdditianalIdentifier).ToList();

                                        if (addresses.Any())
                                        {
                                            personData.Addresses = new List<AddressRegistrationDTO>();

                                            foreach (var addressCheck in addresses)
                                            {
                                                var addressResponse = CommonUtils.Deserialize<PermanentAddressResponseType>(addressCheck.ResponseContent);
                                                var address = RegixDataMappers.MapPermanentAddressResponse(addressResponse);
                                                personData.Addresses.Add(address);
                                            }
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
                            var response = CommonUtils.Deserialize<ForeignIdentityInfoResponseType>(personCheck.ResponseContent);
                            personData = RegixDataMappers.MapForeignPersonIdentityResponse(response);
                        }
                        break;
                }

                persons.Add(personData);
            }

            return persons;
        }

        private RegixLegalContext GetRegixSingleLegalCheck(List<ApplicationRegiXcheck> checks, ContextCheckType type)
        {
            return GetRegixLegalChecks(checks, type).FirstOrDefault();
        }

        private RegixPersonContext GetRegixSinglePersonCheck(List<ApplicationRegiXcheck> checks, ContextCheckType type)
        {
            return GetRegixPersonChecks(checks, type).FirstOrDefault();
        }

        private List<ApplicationRegiXCheckDTO> MapToRegixChecksDto(List<ApplicationRegiXcheck> dbChecks)
        {
            return dbChecks.Select(x => new ApplicationRegiXCheckDTO
            {
                ID = x.Id,
                ApplicationChangeHistoryId = x.ApplicationChangeHistoryId,
                ApplicationId = x.ApplicationId,
                Attempts = x.Attempts,
                ErrorDescription = x.ErrorDescription,
                ErrorLevel = x.ErrorLevel,
                RequestDateTime = x.RequestDateTime,
                ResponseDateTime = x.ResponseDateTime,
                ResponseStatus = x.ResponseStatus,
                WebServiceName = x.WebServiceName
            }).ToList();
        }
    }
}
