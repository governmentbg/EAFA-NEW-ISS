using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Tests.FATests
{
    public static class FaTripConstants
    {
        // Owners
        public static readonly FLUXPartyType BgrOwnerParty = new()
        {
            ID = new IDType[] { new IDType { schemeID = "FLUX_GP_PARTY", Value = "BGR" } },
            Name = new TextType[] { TextType.CreateText("Bulgaria") },
        };

        public static readonly ContactPartyType VesselOwner1 = new()
        {
            RoleCode = new CodeType[] { CodeType.CreateCode("FLUX_CONTACT_ ROLE", "OWNER") },
            SpecifiedContactPerson = new ContactPersonType[] { new ContactPersonType { GivenName = "OwnerGivenName1", FamilyName = "OwnerFamilyName1" } },
            SpecifiedStructuredAddress = new StructuredAddressType[]
            {
                new StructuredAddressType { CityName = TextType.CreateText("City1"), CountryID = IDType.CreateID("TERRITORY", "BGR"), StreetName = TextType.CreateText("Street1") }
            }
        };

        public static readonly ContactPartyType VesselOwner2 = new()
        {
            RoleCode = new CodeType[] { CodeType.CreateCode("FLUX_CONTACT_ ROLE", "OWNER") },
            SpecifiedContactPerson = new ContactPersonType[] { new ContactPersonType { GivenName = "OwnerGivenName2", FamilyName = "OwnerFamilyName2" } },
            SpecifiedStructuredAddress = new StructuredAddressType[]
            {
                new StructuredAddressType { CityName = TextType.CreateText("City2"), CountryID = IDType.CreateID("TERRITORY", "BGR"), StreetName = TextType.CreateText("Street2") }
            }
        };

        public static readonly ContactPartyType VesselOwner3 = new()
        {
            RoleCode = new CodeType[] { CodeType.CreateCode("FLUX_CONTACT_ ROLE", "OWNER") },
            SpecifiedContactPerson = new ContactPersonType[] { new ContactPersonType { GivenName = "OwnerGivenName3", FamilyName = "OwnerFamilyName3" } },
            SpecifiedStructuredAddress = new StructuredAddressType[]
            {
                new StructuredAddressType { CityName = TextType.CreateText("City3"), CountryID = IDType.CreateID("TERRITORY", "BGR"), StreetName = TextType.CreateText("Street3") }
            }
        };

        public static readonly ContactPartyType VesselMaster1 = new()
        {
            RoleCode = new CodeType[] { CodeType.CreateCode("FLUX_CONTACT_ ROLE", "MASTER") },
            SpecifiedContactPerson = new ContactPersonType[] { new ContactPersonType { GivenName = "MasterGivenName1", FamilyName = "MasterFamilyName1" } },
            SpecifiedStructuredAddress = new StructuredAddressType[]
            {
                new StructuredAddressType { CityName = TextType.CreateText("City4"), CountryID = IDType.CreateID("TERRITORY", "BGR"), StreetName = TextType.CreateText("Street4") }
            }
        };

        public static readonly ContactPartyType VesselMaster2 = new()
        {
            RoleCode = new CodeType[] { CodeType.CreateCode("FLUX_CONTACT_ ROLE", "MASTER") },
            SpecifiedContactPerson = new ContactPersonType[] { new ContactPersonType { GivenName = "MasterGivenName2", FamilyName = "MasterFamilyName2" } },
            SpecifiedStructuredAddress = new StructuredAddressType[]
            {
                new StructuredAddressType { CityName = TextType.CreateText("City5"), CountryID = IDType.CreateID("TERRITORY", "BGR"), StreetName = TextType.CreateText("Street5") }
            }
        };

        public static readonly ContactPartyType VesselMaster3 = new()
        {
            RoleCode = new CodeType[] { CodeType.CreateCode("FLUX_CONTACT_ ROLE", "MASTER") },
            SpecifiedContactPerson = new ContactPersonType[] { new ContactPersonType { GivenName = "MasterGivenName3", FamilyName = "MasterFamilyName3" } },
            SpecifiedStructuredAddress = new StructuredAddressType[]
            {
                new StructuredAddressType { CityName = TextType.CreateText("City6"), CountryID = IDType.CreateID("TERRITORY", "BGR"), StreetName = TextType.CreateText("Street6") }
            }
        };

        // Locations
        public static readonly FLUXLocationType LocationPort1 = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "LOCATION"),
            CountryID = IDType.CreateID("TERRITORY", "BGR"),
            ID = IDType.CreateID("LOCATION", "BGMCH")
        };

        public static readonly FLUXLocationType LocationPort2 = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "LOCATION"),
            CountryID = IDType.CreateID("TERRITORY", "BGR"),
            ID = IDType.CreateID("LOCATION", "BGNES")
        };

        public static readonly FLUXLocationType LocationStatRectangle1 = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "AREA"),
            ID = IDType.CreateID("STAT_RECTANGLE", "M25G7")
        };

        public static readonly FLUXLocationType LocationStatRectangle2 = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "AREA"),
            ID = IDType.CreateID("STAT_RECTANGLE", "M24G7")
        };

        public static readonly FLUXLocationType LocationArea = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "AREA"),
            ID = IDType.CreateID("FAO_AREA", "37.4.2")
        };

        public static readonly FLUXLocationType LocationPosition1 = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "POSITION"),
            SpecifiedPhysicalFLUXGeographicalCoordinate = new FLUXGeographicalCoordinateType
            {
                LatitudeMeasure = new MeasureType { Value = 42.5576333333333M },
                LongitudeMeasure = new MeasureType { Value = 27.69165M }
            }
        };

        public static readonly FLUXLocationType LocationPosition2 = new FLUXLocationType
        {
            TypeCode = CodeType.CreateCode("FLUX_LOCATION_TYPE", "POSITION"),
            SpecifiedPhysicalFLUXGeographicalCoordinate = new FLUXGeographicalCoordinateType
            {
                LatitudeMeasure = new MeasureType { Value = 42.4464166666667M },
                LongitudeMeasure = new MeasureType { Value = 27.7948333333333M }
            }
        };

        // Vessels
        public static readonly VesselTransportMeansType Vessel1 = new()
        {
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000001") },
            Name = new TextType[] { TextType.CreateText("SCORTEL001-VN") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner1, VesselMaster1 }
        };

        public static readonly VesselTransportMeansType Vessel2 = new()
        {
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000002") },
            Name = new TextType[] { TextType.CreateText("SCORTEL002-BS") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner2, VesselMaster2 }
        };

        public static readonly VesselTransportMeansType Vessel3 = new()
        {
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000003") },
            Name = new TextType[] { TextType.CreateText("SCORTEL003-TST") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner3, VesselMaster3 }
        };

        public static readonly VesselTransportMeansType VesselDonor = new()
        {
            RoleCode = CodeType.CreateCode("FA_VESSEL_ROLE", "DONOR"),
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000002") },
            Name = new TextType[] { TextType.CreateText("SCORTEL002-BS") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner2, VesselMaster2 }
        };

        public static readonly VesselTransportMeansType VesselReceiver = new()
        {
            RoleCode = CodeType.CreateCode("FA_VESSEL_ROLE", "RECEIVER"),
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000001") },
            Name = new TextType[] { TextType.CreateText("SCORTEL001-VN") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner1, VesselMaster1 }
        };

        public static readonly VesselTransportMeansType VesselCatching = new()
        {
            RoleCode = CodeType.CreateCode("FA_VESSEL_ROLE", "CATCHING_VESSEL"),
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000002") },
            Name = new TextType[] { TextType.CreateText("SCORTEL002-BS") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner2, VesselMaster2 }
        };

        public static readonly VesselTransportMeansType VesselParticipating = new()
        {
            RoleCode = CodeType.CreateCode("FA_VESSEL_ROLE", "PARTICIPATING_VESSEL"),
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000001") },
            Name = new TextType[] { TextType.CreateText("SCORTEL001-VN") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner1, VesselMaster1 }
        };

        public static readonly VesselTransportMeansType VesselPairFishingPartner = new()
        {
            RoleCode = CodeType.CreateCode("FA_VESSEL_ROLE", "PAIR_FISHING_PARTNER"),
            ID = new IDType[] { IDType.CreateID("CFR", "SUR000000003") },
            Name = new TextType[] { TextType.CreateText("SCORTEL003-TST") },
            RegistrationVesselCountry = VesselCountryType.BuildVesselCountry("SVN"),
            SpecifiedContactParty = new ContactPartyType[] { VesselOwner3, VesselMaster3 }
        };

        // Fishing gear characteristics
        public static readonly GearCharacteristicType[] FishingGearCharacteristics11 = new GearCharacteristicType[]
        {
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "ME"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MMT, 20) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GM"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 200) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GD"), Value = TextType.CreateText("SCORTEL1-002") },
        };

        public static readonly GearCharacteristicType[] FishingGearCharacteristics12 = new GearCharacteristicType[]
        {
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "ME"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MMT, 400) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GM"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 2) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GD"), Value = TextType.CreateText("SCORTEL1-002") },
        };

        public static readonly GearCharacteristicType[] FishingGearCharacteristics21 = new GearCharacteristicType[]
        {
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "ME"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MMT, 20) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GM"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 2) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GD"), Value = TextType.CreateText("SCORTEL2-002") },
        };

        public static readonly GearCharacteristicType[] FishingGearCharacteristics22 = new GearCharacteristicType[]
        {
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "ME"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MMT, 20) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "MT"), Value = TextType.CreateText("Трал-2") },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GD"), Value = TextType.CreateText("SCORTEL2-002") },
        };

        public static readonly GearCharacteristicType[] FishingGearCharacteristics31 = new GearCharacteristicType[]
        {
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "ME"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MMT, 2) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GM"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 2) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "HE"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 2) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GD"), Value = TextType.CreateText("SCORTEL11-002") },
        };

        public static readonly GearCharacteristicType[] FishingGearCharacteristics32 = new GearCharacteristicType[]
        {
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "ME"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MMT, 20) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GM"), ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 200) },
            new GearCharacteristicType{ TypeCode = CodeType.CreateCode("FA_GEAR_CHARACTERISTIC", "GD"), Value = TextType.CreateText("SCORTEL11-002") },
        };

        // Fishing gears
        public static readonly FishingGearType FishingGearOnboard11 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "TBB"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "ONBOARD") },
            ApplicableGearCharacteristic = FishingGearCharacteristics11
        };

        public static readonly FishingGearType FishingGearOnboard12 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "GND"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "ONBOARD") },
            ApplicableGearCharacteristic = FishingGearCharacteristics12
        };

        public static readonly FishingGearType FishingGearOnboard21 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "GNS"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "ONBOARD") },
            ApplicableGearCharacteristic = FishingGearCharacteristics21
        };

        public static readonly FishingGearType FishingGearOnboard22 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "OTM"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "ONBOARD") },
            ApplicableGearCharacteristic = FishingGearCharacteristics22
        };

        public static readonly FishingGearType FishingGearOnboard31 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "GNS"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "ONBOARD") },
            ApplicableGearCharacteristic = FishingGearCharacteristics31
        };

        public static readonly FishingGearType FishingGearOnboard32 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "TBB"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "ONBOARD") },
            ApplicableGearCharacteristic = FishingGearCharacteristics32
        };

        public static readonly FishingGearType FishingGearDeployed11 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "TBB"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "DEPLOYED") },
            ApplicableGearCharacteristic = FishingGearCharacteristics11
        };

        public static readonly FishingGearType FishingGearDeployed12 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "GND"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "DEPLOYED") },
            ApplicableGearCharacteristic = FishingGearCharacteristics12
        };

        public static readonly FishingGearType FishingGearDeployed21 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "GNS"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "DEPLOYED") },
            ApplicableGearCharacteristic = FishingGearCharacteristics21
        };

        public static readonly FishingGearType FishingGearDeployed22 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "OTM"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "DEPLOYED") },
            ApplicableGearCharacteristic = FishingGearCharacteristics22
        };

        public static readonly FishingGearType FishingGearDeployed31 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "GNS"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "DEPLOYED") },
            ApplicableGearCharacteristic = FishingGearCharacteristics31
        };

        public static readonly FishingGearType FishingGearDeployed32 = new()
        {
            TypeCode = CodeType.CreateCode("GEAR_TYPE", "TBB"),
            RoleCode = new CodeType[] { CodeType.CreateCode("FA_GEAR_ROLE", "DEPLOYED") },
            ApplicableGearCharacteristic = FishingGearCharacteristics32
        };
    }
}
