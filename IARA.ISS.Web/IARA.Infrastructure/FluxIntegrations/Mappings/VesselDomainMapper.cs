using System.Text;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Interfaces.FluxIntegrations.Ships;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Mappings.FluxVessels
{
    public class VesselDomainMapper : BaseService, IVesselDomainMapper
    {
        public const string BULGARIA_CODE = "BGR";
        public const string BULGARIA_NAME_EN = "Bulgaria";

        private readonly IPersonService personService;
        private readonly ILegalService legalService;

        private static readonly Dictionary<char, string> TransliterationTable = new Dictionary<char, string>
        {
            { 'А', "A" }, { 'Б', "B" }, { 'В', "V" }, { 'Г', "G" }, { 'Д', "D" }, { 'Е', "E" }, { 'Ж', "Zh" }, { 'З', "Z" }, { 'И', "I" }, { 'Й', "Y" },
            { 'К', "K" }, { 'Л', "L" }, { 'М', "M" }, { 'Н', "N" }, { 'О', "O" }, { 'П', "P" }, { 'Р', "R" }, { 'С', "S" }, { 'Т', "T" }, { 'У', "U" },
            { 'Ф', "F" }, { 'Х', "H" }, { 'Ц', "Ts" }, { 'Ч', "Ch" }, { 'Ш', "Sh" }, { 'Щ', "Sht" }, { 'Ъ', "A" }, { 'Ь', "Y" }, { 'Ю', "Yu" }, { 'Я', "Ya" },

            { 'а', "a" }, { 'б', "b" }, { 'в', "v" }, { 'г', "g" }, { 'д', "d" }, { 'е', "e" }, { 'ж', "zh" }, { 'з', "z" }, { 'и', "i" }, { 'й', "y" },
            { 'к', "k" }, { 'л', "l" }, { 'м', "m" }, { 'н', "n" }, { 'о', "o" }, { 'п', "p" }, { 'р', "r" }, { 'с', "s" }, { 'т', "t" }, { 'у', "u" },
            { 'ф', "f" }, { 'х', "h" }, { 'ц', "ts" }, { 'ч', "ch" }, { 'ш', "sh" }, { 'щ', "sht" }, { 'ъ', "a" }, { 'ь', "y" }, { 'ю', "yu" }, { 'я', "ya" }
        };

        public VesselDomainMapper(IARADbContext dbContext,
                                 IPersonService personService,
                                 ILegalService legalService)
            : base(dbContext)
        {
            this.personService = personService;
            this.legalService = legalService;
        }

        public FLUXReportVesselInformationType MapVesselToFluxSub(ShipRegisterEditDTO vessel, ReportPurposeCodes purpose)
        {
            return MapVesselToFluxSub(new List<ShipRegisterEditDTO> { vessel }, purpose);
        }

        public FLUXReportVesselInformationType MapVesselToFluxSubVcd(ShipRegisterEditDTO vessel, ReportPurposeCodes purpose)
        {
            return MapVesselToFluxSubVcd(new List<ShipRegisterEditDTO> { vessel }, purpose);
        }

        public FLUXReportVesselInformationType MapVesselToFluxVed(ShipRegisterEditDTO vessel, ReportPurposeCodes purpose)
        {
            return MapVesselToFluxVed(new List<ShipRegisterEditDTO> { vessel }, purpose);
        }

        public FLUXReportVesselInformationType MapVesselToFluxSub(List<ShipRegisterEditDTO> vessels, ReportPurposeCodes purpose)
        {
            FLUXReportVesselInformationType result = CreateReportVesselInformation(vessels, VesselReportTypes.SUB, purpose);
            return result;
        }

        public FLUXReportVesselInformationType MapVesselToFluxSubVcd(List<ShipRegisterEditDTO> vessels, ReportPurposeCodes purpose)
        {
            FLUXReportVesselInformationType result = CreateReportVesselInformation(vessels, VesselReportTypes.SUB_VCD, purpose);
            return result;
        }

        public FLUXReportVesselInformationType MapVesselToFluxVed(List<ShipRegisterEditDTO> vessels, ReportPurposeCodes purpose)
        {
            FLUXReportVesselInformationType result = CreateReportVesselInformation(vessels, VesselReportTypes.SUB_VED, purpose);
            return result;
        }

        public FLUXVesselQueryMessageType MapQuery(FluxVesselQueryRequestEditDTO query)
        {
            FLUXVesselQueryMessageType result = new()
            {
                VesselQuery = new VesselQueryType
                {
                    ID = IDType.CreateFromGuid(Guid.NewGuid()),
                    SubmittedDateTime = DateTime.Now,
                    TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_TYPE, VesselQueryTypes.Q_NR),
                    SubmitterFLUXParty = new FLUXPartyType
                    {
                        ID = new IDType[] { IDType.CreateParty("BGR") },
                        Name = new TextType[] { TextType.CreateText("Bulgaria") }
                    },
                    SubjectVesselIdentity = CreateQueryVesselIdentity(query),
                    SimpleVesselQueryParameter = CreateVesselQueryParameters(query),
                    SpecifiedDelimitedPeriod = new DelimitedPeriodType
                    {
                        StartDateTime = query.DateTimeFrom.Value,
                        EndDateTime = query.DateTimeTo.Value
                    }
                }
            };

            return result;
        }

        // Reports
        private FLUXReportVesselInformationType CreateReportVesselInformation(List<ShipRegisterEditDTO> vessels, string reportType, ReportPurposeCodes purpose)
        {
            FLUXReportVesselInformationType result = new FLUXReportVesselInformationType
            {
                FLUXReportDocument = CreateReportDocumentType(reportType, purpose),
                VesselEvent = CreateVesselEvents(vessels, reportType)
            };

            return result;
        }

        private FLUXReportDocumentType CreateReportDocumentType(string reportType, ReportPurposeCodes purpose)
        {
            FLUXReportDocumentType result = new FLUXReportDocumentType
            {
                ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) },
                TypeCode = CodeType.CreateVesselReportType(reportType),
                CreationDateTime = DateTime.Now,
                PurposeCode = CodeType.CreatePurpose(purpose),
                OwnerFLUXParty = new FLUXPartyType
                {
                    ID = new IDType[] { IDType.CreateParty(BULGARIA_CODE) },
                    Name = new TextType[] { TextType.CreateText(BULGARIA_NAME_EN) }
                }
            };

            return result;
        }

        private VesselEventType[] CreateVesselEvents(List<ShipRegisterEditDTO> vessels, string reportType)
        {
            List<VesselEventType> result = new List<VesselEventType>();

            foreach (ShipRegisterEditDTO vessel in vessels)
            {
                VesselEventType vesselEvent = CreateVesselEvent(vessel, reportType);
                result.Add(vesselEvent);
            }

            return result.ToArray();
        }

        private VesselEventType CreateVesselEvent(ShipRegisterEditDTO vessel, string reportType)
        {
            VesselEventType vesselEvent = new VesselEventType
            {
                OccurrenceDateTime = vessel.EventDate.Value,
                RelatedVesselTransportMeans = CreateRelatedVesselTransportMeans(vessel, reportType)
            };

            vesselEvent.TypeCode = reportType == VesselReportTypes.SUB_VED
                ? CodeType.CreateVesselEvent(nameof(ShipEventTypeEnum.MOD))
                : CodeType.CreateVesselEvent(vessel.EventType.Value.ToString());

            return vesselEvent;
        }

        private VesselTransportMeansType CreateRelatedVesselTransportMeans(ShipRegisterEditDTO vessel, string reportType)
        {
            VesselTransportMeansType result = new VesselTransportMeansType
            {
                // Идентификатори - CFR, UVI и т.н.
                ID = GetVesselIDs(vessel),
                // Име на кораба
                Name = new TextType[] { TextType.CreateText(vessel.Name) },
                // Пристанище на регистрация, държава на внос
                SpecifiedRegistrationEvent = CreateVesselRegistrationEvent(vessel, reportType),
                // Двигатели
                AttachedVesselEngine = CreateVesselEngines(vessel, reportType),
                // Технически характеристики (размери)
                SpecifiedVesselDimension = CreateVesselDimensions(vessel, reportType)
            };

            // Вид на кораба 
            if (vessel.VesselTypeId.HasValue)
            {
                string code = (from type in Db.NvesselTypes
                               where type.Id == vessel.VesselTypeId.Value
                               select type.Code).First();

                result.TypeCode = new CodeType[] { CodeType.CreateVesselType(code) };
            }
            else
            {
                string code = "FX"; // FISHING VESSELS NOT SPECIFIED
                result.TypeCode = new CodeType[] { CodeType.CreateVesselType(code) };
            }

            if (IsReportTypeVcd(reportType))
            {
                // Флаг
                string flagCountryCode = (from country in Db.Ncountries
                                          where country.Id == vessel.CountryFlagId.Value
                                          select country.Code).First();

                result.RegistrationVesselCountry = VesselCountryType.BuildVesselCountry(flagCountryCode);

                // Година на построяване (място?)
                result.SpecifiedConstructionEvent = new ConstructionEventType
                {
                    OccurrenceDateTime = new DateTime(vessel.BuildYear.Value, 1, 1)
                };

                // Уреди
                result.OnBoardFishingGear = CreateOnBoardFishingGear(vessel);

                // Индикатори за наличие на IRCS, VMS, ERS, AIS
                result.ApplicableVesselEquipmentCharacteristic = CreateVesselEquipment(vessel);

                // Публична помощ, сегмент и др.
                result.ApplicableVesselAdministrativeCharacteristic = CreateVesselAdministrativeCharacteristic(vessel);

                // Технически характеристики (материал на корпуса)
                result.ApplicableVesselTechnicalCharacteristic = CreateVesselTechnicalCharacteristic(vessel);

                // Собственици
                result.SpecifiedContactParty = CreateVesselContactParties(vessel);
            }

            if (IsReportTypeVed(reportType))
            {
                // Екипаж
                result.SpecifiedVesselCrew = CreateVesselCrew(vessel);
            }

            return result;
        }

        private IDType[] GetVesselIDs(ShipRegisterEditDTO vessel)
        {
            List<IDType> result = new List<IDType>();

            if (!string.IsNullOrEmpty(vessel.CFR))
            {
                result.Add(IDType.CreateCFR(vessel.CFR));
            }

            if (!string.IsNullOrEmpty(vessel.UVI))
            {
                result.Add(IDType.CreateUVI(vessel.UVI));
            }

            if (!string.IsNullOrEmpty(vessel.RegistrationNumber))
            {
                result.Add(IDType.CreateID(IDTypes.REG_NBR, vessel.RegistrationNumber));
            }

            if (!string.IsNullOrEmpty(vessel.ExternalMark))
            {
                result.Add(IDType.CreateID(IDTypes.EXT_MARK, vessel.ExternalMark));
            }

            if (!string.IsNullOrEmpty(vessel.IRCSCallSign))
            {
                result.Add(IDType.CreateID(IDTypes.IRCS, vessel.IRCSCallSign));
            }

            if (!string.IsNullOrEmpty(vessel.MMSI))
            {
                result.Add(IDType.CreateMMSI(vessel.MMSI));
            }

            return result.ToArray();
        }

        private RegistrationEventType[] CreateVesselRegistrationEvent(ShipRegisterEditDTO vessel, string reportType)
        {
            if (IsReportTypeVcd(reportType))
            {
                List<RegistrationEventType> result = new List<RegistrationEventType>();

                string portCode = (from port in Db.Nports
                                   where port.Id == vessel.PortId.Value
                                   select port.Code).First();

                result.Add(new RegistrationEventType
                {
                    OccurrenceDateTime = vessel.RegistrationDate,
                    RelatedRegistrationLocation = new RegistrationLocationType
                    {
                        ID = new IDType[] { IDType.CreateID(ListIDTypes.VESSEL_PORT, portCode) },
                        TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_REGSTR_TYPE, VesselRegistrationTypes.PORT)
                    }
                });

                if (vessel.ImportCountryId.HasValue)
                {
                    string countryCode = (from country in Db.Ncountries
                                          where country.Id == vessel.ImportCountryId.Value
                                          select country.Code).First();

                    result.Add(new RegistrationEventType
                    {
                        RelatedRegistrationLocation = new RegistrationLocationType
                        {
                            ID = new IDType[] { IDType.CreateID(ListIDTypes.TERRITORY, countryCode) },
                            TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_REGSTR_TYPE, VesselRegistrationTypes.MOVE)
                        }
                    });
                }

                return result.ToArray();
            }

            return null;
        }

        private VesselEngineType[] CreateVesselEngines(ShipRegisterEditDTO vessel, string reportType)
        {
            int count = vessel.AuxiliaryEnginePower.HasValue ? 2 : 1;

            VesselEngineType[] result = new VesselEngineType[count];

            result[0] = new VesselEngineType
            {
                RoleCode = CodeType.CreateVesselEngineType(VesselEngineTypes.MAIN),
                PowerMeasure = new MeasureType[] { MeasureType.CreatePowerKW(vessel.MainEnginePower.Value) }
            };

            if (IsReportTypeVed(reportType))
            {
                if (!string.IsNullOrEmpty(vessel.MainEngineModel))
                {
                    result[0].Model = TextType.CreateText(vessel.MainEngineModel);
                }
            }

            if (vessel.AuxiliaryEnginePower.HasValue)
            {
                result[1] = new VesselEngineType
                {
                    RoleCode = CodeType.CreateVesselEngineType(VesselEngineTypes.AUX),
                    PowerMeasure = new MeasureType[] { MeasureType.CreatePowerKW(vessel.AuxiliaryEnginePower.Value) }
                };
            }

            return result;
        }

        private VesselDimensionType[] CreateVesselDimensions(ShipRegisterEditDTO vessel, string reportType)
        {
            List<VesselDimensionType> result = new List<VesselDimensionType>();

            if (IsReportTypeVcd(reportType))
            {
                result.Add(VesselDimensionType.LengthOverall(vessel.TotalLength.Value));
                result.Add(VesselDimensionType.GrossTonnage(vessel.GrossTonnage.Value));

                if (vessel.NetTonnage.HasValue)
                {
                    result.Add(VesselDimensionType.NetTonnage(vessel.NetTonnage.Value));
                }

                if (vessel.OtherTonnage.HasValue)
                {
                    result.Add(VesselDimensionType.OtherGrossTonage(vessel.OtherTonnage.Value));
                }

                if (vessel.LengthBetweenPerpendiculars.HasValue)
                {
                    result.Add(VesselDimensionType.LengthBetweenPerpendiculars(vessel.LengthBetweenPerpendiculars.Value));
                }
            }

            if (IsReportTypeVed(reportType))
            {
                result.Add(VesselDimensionType.Draught(vessel.ShipDraught.Value));
                result.Add(VesselDimensionType.Breadth(vessel.TotalWidth.Value));
            }

            return result.ToArray();
        }

        private FishingGearType[] CreateOnBoardFishingGear(ShipRegisterEditDTO vessel)
        {
            List<FishingGearType> result = new List<FishingGearType>();

            string mainGearCode = (from gear in Db.NfishingGears
                                   where gear.Id == vessel.MainFishingGearId.Value
                                   select gear.Code).First();

            result.Add(FishingGearType.CreateFishingGear(mainGearCode, VesselGearRoles.MAIN));

            if (vessel.AdditionalFishingGearId.HasValue)
            {
                string auxGearCode = (from gear in Db.NfishingGears
                                      where gear.Id == vessel.AdditionalFishingGearId.Value
                                      select gear.Code).First();

                result.Add(FishingGearType.CreateFishingGear(auxGearCode, VesselGearRoles.AUX));
            }

            return result.ToArray();
        }

        private VesselEquipmentCharacteristicType[] CreateVesselEquipment(ShipRegisterEditDTO vessel)
        {
            List<VesselEquipmentCharacteristicType> result = new List<VesselEquipmentCharacteristicType>
            {
                VesselEquipmentCharacteristicType.CreateBooleanEquipment(VesselEquipmentTypes.IRCS_IND, !string.IsNullOrEmpty(vessel.IRCSCallSign)),
                VesselEquipmentCharacteristicType.CreateBooleanEquipment(VesselEquipmentTypes.ERS_IND, vessel.HasERS.Value),
                VesselEquipmentCharacteristicType.CreateBooleanEquipment(VesselEquipmentTypes.ERS_EXEMPT_IND, vessel.HasERSException.Value),
                VesselEquipmentCharacteristicType.CreateBooleanEquipment(VesselEquipmentTypes.VMS_IND, vessel.HasVMS.Value),
                VesselEquipmentCharacteristicType.CreateBooleanEquipment(VesselEquipmentTypes.AIS_IND, vessel.HasAIS.Value)
            };

            return result.ToArray();
        }

        private VesselAdministrativeCharacteristicType[] CreateVesselAdministrativeCharacteristic(ShipRegisterEditDTO vessel)
        {
            DateTime now = DateTime.Now;

            List<VesselAdministrativeCharacteristicType> result = new List<VesselAdministrativeCharacteristicType>();

            // флаг за разрешително
            result.Add(VesselAdministrativeCharacteristicType.CreateAdministrativeCharacteristic(VesselAdminTypes.LICENCE, vessel.HasFishingPermit));

            // сегмент на флота
            string fleetSegmentCode = (from segment in Db.Nsegments
                                       where segment.Id == vessel.FleetSegmentId.Value
                                       select segment.Code).First();

            result.Add(VesselAdministrativeCharacteristicType.CreateAdministrativeCharacteristic(VesselAdminTypes.SEG, fleetSegmentCode));

            // код на публична помощ
            string publicAidCode = (from aid in Db.NpublicAidTypes
                                    join mdr in Db.MdrVesselPublicAidTypes on aid.MdrVesselPublicAidTypeId equals mdr.Id
                                    where aid.Id == vessel.PublicAidTypeId.Value
                                    select mdr.Code).FirstOrDefault();

            if (!string.IsNullOrEmpty(publicAidCode))
            {
                result.Add(VesselAdministrativeCharacteristicType.CreateAdministrativeCharacteristic(VesselAdminTypes.AID, publicAidCode));
            }

            // Дата на влизане в експлоатация
            if (vessel.ExploitationStartDate.HasValue)
            {
                result.Add(VesselAdministrativeCharacteristicType.CreateAdministrativeCharacteristic(VesselAdminTypes.EIS, vessel.ExploitationStartDate.Value));
            }

            // вид износ
            if (vessel.EventType.Value == ShipEventTypeEnum.EXP)
            {
                if (vessel.ExportType.Value == ShipExportTypeEnum.ExportOrTransferInEU)
                {
                    result.Add(VesselAdministrativeCharacteristicType.CreateAdministrativeCharacteristic(VesselAdminTypes.EXPORT, VesselExportTypes.ExportOrTransfer));
                }
                else if (vessel.ExportType.Value == ShipExportTypeEnum.ExportJointVenture)
                {
                    result.Add(VesselAdministrativeCharacteristicType.CreateAdministrativeCharacteristic(VesselAdminTypes.EXPORT, VesselExportTypes.ExportationForAJoinEnterprise));
                }
            }

            return result.ToArray();
        }

        private VesselTechnicalCharacteristicType[] CreateVesselTechnicalCharacteristic(ShipRegisterEditDTO vessel)
        {
            string hullMaterialCode = (from hull in Db.NhullMaterials
                                       where hull.Id == vessel.HullMaterialId.Value
                                       select hull.Code).First();

            VesselTechnicalCharacteristicType[] result = new VesselTechnicalCharacteristicType[]
            {
                new VesselTechnicalCharacteristicType
                {
                    TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_TECH_TYPE, nameof(VesselTechTypes.HULL)),
                    ValueCode = CodeType.CreateCode(ListIDTypes.VESSEL_HULL_TYPE, hullMaterialCode)
                }
            };

            return result;
        }

        private VesselCrewType CreateVesselCrew(ShipRegisterEditDTO vessel)
        {
            return new VesselCrewType
            {
                MemberQuantity = new QuantityType
                {
                    unitCode = nameof(FluxUnits.C62),
                    Value = vessel.CrewCount.Value
                }
            };
        }

        private ContactPartyType[] CreateVesselContactParties(ShipRegisterEditDTO vessel)
        {
            List<ContactPartyType> result = new List<ContactPartyType>();

            // Add owners
            foreach (ShipOwnerDTO owner in vessel.Owners.Where(x => x.IsActive.Value))
            {
                ContactPartyType entry = CreateVesselContactParty(owner, FluxContactRoles.OWNER);
                result.Add(entry);
            }

            // Add operator(s)
            DateTime now = DateTime.Now;

            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == vessel.ShipUID.Value
                                 select ship.Id).ToList();

            var operatorIds = (from permitLicence in Db.CommercialFishingPermitLicensesRegisters
                               where shipIds.Contains(permitLicence.ShipId)
                                  && permitLicence.IsHolderShipOwner != true
                                  && permitLicence.PermitLicenseValidFrom <= now
                                  && permitLicence.PermitLicenseValidTo > now
                                  && !permitLicence.IsSuspended
                                  && permitLicence.IsActive
                               select new
                               {
                                   permitLicence.SubmittedForPersonId,
                                   permitLicence.SubmittedForLegalId
                               }).ToList();

            List<int> operatorPersonIds = operatorIds.Where(x => x.SubmittedForPersonId.HasValue).Select(x => x.SubmittedForPersonId.Value).ToList();
            List<int> operatorLegalIds = operatorIds.Where(x => x.SubmittedForLegalId.HasValue).Select(x => x.SubmittedForLegalId.Value).ToList();

            // ако няма удостоверения, то няма титуляри, то изпращаме собственик (титуляр на РСР) за оператор
            if (operatorPersonIds.Count == 0 && operatorLegalIds.Count == 0)
            {
                ShipOwnerDTO titular = vessel.Owners.Where(x => x.IsActive.Value && x.IsShipHolder.Value).Single();
                ContactPartyType entry = CreateVesselContactParty(titular, FluxContactRoles.OPERATOR);
                result.Add(entry);
            }
            else
            {
                Dictionary<int, RegixPersonDataDTO> persons = personService.GetRegixPersonsData(operatorPersonIds);
                Dictionary<int, RegixLegalDataDTO> legals = legalService.GetRegixLegalsData(operatorLegalIds);

                ILookup<int, AddressRegistrationDTO> personAddresses = personService.GetAddressRegistrations(operatorPersonIds);
                ILookup<int, AddressRegistrationDTO> legalAddresses = legalService.GetAddressRegistrations(operatorLegalIds);

                foreach (KeyValuePair<int, RegixPersonDataDTO> person in persons)
                {
                    ContactPartyType entry = CreateVesselContactPartyOperator(person.Value, personAddresses[person.Key].ToList());
                    result.Add(entry);
                }

                foreach (KeyValuePair<int, RegixLegalDataDTO> legal in legals)
                {
                    ContactPartyType entry = CreateVesselContactPartyOperator(legal.Value, legalAddresses[legal.Key].ToList());
                    result.Add(entry);
                }
            }

            return result.ToArray();
        }

        private ContactPartyType CreateVesselContactParty(ShipOwnerDTO owner, FluxContactRoles role)
        {
            ContactPartyType entry = new ContactPartyType
            {
                RoleCode = new CodeType[] { CodeType.CreateCode(ListIDTypes.FLUX_CONTACT_ROLE, role.ToString()) }
            };

            if (owner.IsOwnerPerson.Value)
            {
                ContactPersonType person = CreateContactPerson(owner.RegixPersonData);
                entry.SpecifiedContactPerson = new ContactPersonType[] { person };

                if (owner.RegixPersonData.CitizenshipCountryId.HasValue)
                {
                    string countryCode = (from country in Db.Ncountries
                                          where country.Id == owner.RegixPersonData.CitizenshipCountryId.Value
                                          select country.Code).First();

                    entry.NationalityCountryID = new IDType[] { IDType.CreateID(ListIDTypes.TERRITORY, countryCode) };
                }

                entry.SpecifiedStructuredAddress = CreateStructuredAddresses(owner.AddressRegistrations);
                entry.URIEmailCommunication = CreateEmailCommunications(new List<string> { owner.RegixPersonData.Email });
            }
            else
            {
                entry.Name = TextType.CreateText(owner.RegixLegalData.Name);
                entry.SpecifiedStructuredAddress = CreateStructuredAddresses(owner.AddressRegistrations);
                entry.URIEmailCommunication = CreateEmailCommunications(new List<string> { owner.RegixLegalData.Email });
            }

            return entry;
        }

        private ContactPartyType CreateVesselContactPartyOperator(RegixPersonDataDTO person, List<AddressRegistrationDTO> addresses)
        {
            ContactPartyType entry = new ContactPartyType
            {
                RoleCode = new CodeType[] { CodeType.CreateCode(ListIDTypes.FLUX_CONTACT_ROLE, nameof(FluxContactRoles.OPERATOR)) }
            };

            ContactPersonType contactPerson = CreateContactPerson(person);
            entry.SpecifiedContactPerson = new ContactPersonType[] { contactPerson };

            if (person.CitizenshipCountryId.HasValue)
            {
                string countryCode = (from country in Db.Ncountries
                                      where country.Id == person.CitizenshipCountryId.Value
                                      select country.Code).First();

                entry.NationalityCountryID = new IDType[] { IDType.CreateID(ListIDTypes.TERRITORY, countryCode) };
            }

            entry.SpecifiedStructuredAddress = CreateStructuredAddresses(addresses);
            entry.URIEmailCommunication = CreateEmailCommunications(new List<string> { person.Email });

            return entry;
        }

        private ContactPartyType CreateVesselContactPartyOperator(RegixLegalDataDTO legal, List<AddressRegistrationDTO> addresses)
        {
            ContactPartyType entry = new ContactPartyType
            {
                RoleCode = new CodeType[] { CodeType.CreateCode(ListIDTypes.FLUX_CONTACT_ROLE, nameof(FluxContactRoles.OPERATOR)) }
            };

            entry.Name = TextType.CreateText(legal.Name);
            entry.SpecifiedStructuredAddress = CreateStructuredAddresses(addresses);
            entry.URIEmailCommunication = CreateEmailCommunications(new List<string> { legal.Email });

            return entry;
        }

        private ContactPersonType CreateContactPerson(RegixPersonDataDTO person)
        {
            ContactPersonType result = new ContactPersonType
            {
                GivenName = TextType.CreateText(person.FirstName),
                FamilyName = TextType.CreateText(person.LastName)
            };

            if (!string.IsNullOrEmpty(person.MiddleName))
            {
                result.MiddleName = TextType.CreateText(person.MiddleName);
            }

            if (person.BirthDate.HasValue)
            {
                result.BirthDateTime = person.BirthDate.Value;
            }

            return result;
        }

        private StructuredAddressType[] CreateStructuredAddresses(List<AddressRegistrationDTO> addresses)
        {
            List<StructuredAddressType> result = new List<StructuredAddressType>();

            foreach (AddressRegistrationDTO address in addresses)
            {
                StructuredAddressType entry = CreateStructuredAddress(address);
                result.Add(entry);
            }

            return result.ToArray();
        }

        private StructuredAddressType CreateStructuredAddress(AddressRegistrationDTO address)
        {
            StructuredAddressType result = new StructuredAddressType();

            string countryCode = (from country in Db.Ncountries
                                  where country.Id == address.CountryId
                                  select country.Code).First();

            result.CountryID = IDType.CreateID(ListIDTypes.TERRITORY, countryCode);

            if (address.PopulatedAreaId.HasValue)
            {
                string populatedArea = (from pop in Db.NpopulatedAreas
                                        where pop.Id == address.PopulatedAreaId.Value
                                        select pop.Name).First();

                result.CityName = TextType.CreateText(populatedArea);
            }

            if (!string.IsNullOrEmpty(address.PostalCode))
            {
                result.PostalArea = TextType.CreateText(address.PostalCode);
            }

            if (!string.IsNullOrEmpty(address.Street))
            {
                result.StreetName = TextType.CreateText(address.Street);
            }

            return result;
        }

        private EmailCommunicationType[] CreateEmailCommunications(List<string> emails)
        {
            List<EmailCommunicationType> result = new List<EmailCommunicationType>();

            foreach (string email in emails)
            {
                EmailCommunicationType entry = new EmailCommunicationType
                {
                    URIID = IDType.CreateID(ListIDTypes.URI, email)
                };

                result.Add(entry);
            }

            return result.ToArray();
        }

        private static bool IsReportTypeVcd(string reportType)
        {
            return reportType == VesselReportTypes.SUB_VCD || reportType == VesselReportTypes.SUB;
        }

        private static bool IsReportTypeVed(string reportType)
        {
            return reportType == VesselReportTypes.SUB_VED || reportType == VesselReportTypes.SUB;
        }

        // Queries
        private VesselIdentityType CreateQueryVesselIdentity(FluxVesselQueryRequestEditDTO query)
        {
            VesselIdentityType result = new()
            {
                VesselID = CreateQueryVesselIdentification(query)
            };

            if (!string.IsNullOrEmpty(query.FlagStateCode))
            {
                result.VesselRegistrationCountryID = IDType.CreateID(ListIDTypes.TERRITORY, query.FlagStateCode);
            }

            return result;
        }

        private IDType[] CreateQueryVesselIdentification(FluxVesselQueryRequestEditDTO query)
        {
            List<IDType> result = new();

            if (!string.IsNullOrEmpty(query.CFR))
            {
                result.Add(IDType.CreateCFR(query.CFR));
            }
            else if (!string.IsNullOrEmpty(query.UVI))
            {
                result.Add(IDType.CreateUVI(query.UVI));
            }
            else if (!string.IsNullOrEmpty(query.IRCS))
            {
                result.Add(IDType.CreateID(IDTypes.IRCS, query.IRCS));
            }
            else if (!string.IsNullOrEmpty(query.MMSI))
            {
                result.Add(IDType.CreateMMSI(query.MMSI));
            }

            return result.ToArray();
        }

        private VesselQueryParameterType[] CreateVesselQueryParameters(FluxVesselQueryRequestEditDTO query)
        {
            List<VesselQueryParameterType> result = new();

            if (query.HistYes.Value && query.HistNo.Value)
            {
                throw new ArgumentException("Cannot have both HIST_YES and HIST_NO for vessel query.");
            }
            else if (query.HistYes.Value)
            {
                result.Add(new VesselQueryParameterType
                {
                    SearchTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_PARAM, VesselQueryParamTypes.HIST_YES)
                });
            }
            else if (query.HistNo.Value)
            {
                result.Add(new VesselQueryParameterType
                {
                    SearchTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_PARAM, VesselQueryParamTypes.HIST_NO)
                });
            }
            else
            {
                throw new ArgumentException("Either HIST_YES or HIST_NO must be true for vessel query.");
            }

            if (query.VesselActive.Value && query.VesselAll.Value)
            {
                throw new ArgumentException("Cannot have both VESSEL_ACTIVE and VESSEL_ALL for vessel query.");
            }
            else if (query.VesselActive.Value)
            {
                result.Add(new VesselQueryParameterType
                {
                    SearchTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_PARAM, VesselQueryParamTypes.VESSEL_ACTIVE)
                });
            }
            else if (query.VesselAll.Value)
            {
                result.Add(new VesselQueryParameterType
                {
                    SearchTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_PARAM, VesselQueryParamTypes.VESSEL_ALL)
                });
            }
            else
            {
                throw new ArgumentException("Either VESSEL_ACTIVE or VESSEL_ALL must be true for vessel query.");
            }

            if (query.DataVcd.Value && query.DataAll.Value)
            {
                throw new ArgumentException("Cannot have both DATA_VCD and DATA_ALL for vessel query.");
            }
            else if (query.DataVcd.Value)
            {
                result.Add(new VesselQueryParameterType
                {
                    SearchTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_PARAM, VesselQueryParamTypes.DATA_VCD)
                });
            }
            else if (query.DataAll.Value)
            {
                result.Add(new VesselQueryParameterType
                {
                    SearchTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_QUERY_PARAM, VesselQueryParamTypes.DATA_ALL)
                });
            }
            else
            {
                throw new ArgumentException("Either DATA_VCD or DATA_ALL must be true for vessel query.");
            }

            return result.ToArray();
        }

        // Common
        private static string TransliterateBgString(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length * 2);

            foreach (char ch in value)
            {
                if (TransliterationTable.TryGetValue(ch, out string transliterated))
                {
                    builder.Append(transliterated);
                }
                else
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }
    }
}
