//using System;
//using System.Collections.Generic;
//using IARA.FluxModels.Models;

//namespace IARA.Flux.Mockups
//{
//    public class FluxMockups
//    {
//        /// <summary>
//        /// Flux - FA - Departure without fish onboard - start of fishing trip
//        ///
//        /// FLUX_P1000-3_Fishing_Activities_domain_EU_Implementation_v2.5.1.pdf
//        ///
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Dep_wo_fish_2()
//        {
//            try
//            {
//                // Data structure
//                // Flux FA Report Msg consts:
//                // 1. Flux Report Document
//                // 2. FA Report Document

//                // FLUX Report Msg
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                // FLUX Report Document
//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // UUID generated in FVMS, ISS
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "5ee8be46-2efe-4a29-b2df-bdf2d3ed66a1"
//                    }
//                };
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // DB Nomenclature - MDR_FLUX_GP_Purpose
//                // Value 9 - Original
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-710302" };
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    // DB Nomenclature - MDR_FLUX_GP_Party
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                // FA Report Document
//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Report_Type
//                // Value - Declaration or Notification
//                fARepDoc.TypeCode = new CodeType()
//                {
//                    ListID = "FLUX_FA_REPORT_TYPE",
//                    Value = "DECLARATION"
//                };

//                // Datetime of acceptance in FVMS
//                fARepDoc.AcceptanceDateTime = new DateTimeType()
//                {
//                    Item = DateTime.Now
//                };

//                // With section - related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "UUID",
//                            Value  = "7712fe73-cef2-4646-97bb-d634fde00b07"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    // DB Nomenclature - MDR_FLUX_GP_Purpose
//                    // Value - 9 Original
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    // DB Nomenclature - MDR_FLUX_GP_Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // Type - code
//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Type
//                // Value - Departure
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "DEPARTURE";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                // Datetime of occurance/ creation of data msg on board of fishing vessel
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // Reason
//                // Reason of departure
//                // DB Nomenclature - FLUX_FA.MDR_FA_Reason_Departure
//                // Value - FIS -> fishing
//                CodeType ctypeReason = new CodeType();
//                ctypeReason.ListID = "FA_REASON_DEPARTURE";
//                ctypeReason.Value = "FIS";
//                FAct.ReasonCode = ctypeReason;

//                // Without section Specified FA Catch - catches on board

//                // Related Flux Location
//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                // DB Nomenclature - MDR_FLUX_Location_Type
//                fLUXLocation.TypeCode = new CodeType()
//                {
//                    ListID = "FLUX_LOCATION_TYPE",
//                    Value = "LOCATION"
//                };
//                // DB Nomenclature - MDR_Territory
//                fLUXLocation.CountryID = new IDType()
//                {
//                    SchemeID = "TERRITORY",
//                    Value = "BGR"
//                };
//                // DB Nomenclature - MDR_LOCATION
//                // Value  - BGBOJ - Burgas
//                fLUXLocation.ID = new IDType()
//                {
//                    SchemeID = "LOCATION",
//                    Value = "BGBOJ"
//                };
//                FAct.RelatedFLUXLocation = new FLUXLocationType[] { fLUXLocation };

//                // Specified Fishing Gear
//                // DB Nomenclature - MDR_Gear_Type
//                // Value - PS -
//                FishingGearType fishingGear = new FishingGearType();
//                fishingGear.TypeCode = new CodeType()
//                {
//                    ListID = "GEAR_TYPE",
//                    Value = "PS"
//                };
//                // DB Nomenclature - FLUX_FA.MDR_FA_Gear_Role
//                fishingGear.RoleCode = new CodeType[] {
//                    new CodeType() {
//                        ListID = "FA_GEAR_ROLE",
//                        Value = "ONBOARD"
//                    }
//                };
//                fishingGear.ApplicableGearCharacteristic = new GearCharacteristicType[] {
//                    new GearCharacteristicType() {
//                        // DB Nomenclature - FLUX_FA.MDR_FA_Gear_Characteristic
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC",
//                            Value = "ME"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MMT",
//                            Value = 140
//                        }
//                    } };
//                FAct.SpecifiedFishingGear = new FishingGearType[] { fishingGear };

//                // Trip information
//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        // DB Nomenclature - FLUX_FA.MDR_FA_Trip_Id_Type
//                        // Value - Unique ID Type
//                        // Check Trip Number Convention
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506193933176"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;
//                // Add fishing activity to the fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();
//                // Vessel Identification CFR, UVI, etc
//                // Check FLUX Fishing Activity Implementation Document in the EU – v 2.5.1
//                // page 57/ 7.1.15.1. Vessel_Transport_Means entity
//                vesselTransport.ID = new IDType[] {
//                    new IDType() { SchemeID  = "CFR", Value = "BGR123456789" },
//                    new IDType() { SchemeID = "UVI", Value = "1234567"}
//                };
//                // Vessel Name
//                vesselTransport.Name = new TextType[] {
//                    new TextType() { Value = "GOLF"}
//                };
//                // Registration Vessel Country
//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "CYR"
//                    }
//                };

//                // Specified Contact Party
//                // Data only for master/captain
//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        // DB Nomenclature - MDR_FLUX_Contact_Role
//                        RoleCode = new List<CodeType> {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "BGR"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] {
//                    fARepDoc
//                };

//                //// XML structure of FLUXFAReportMessageType
//                //// which is packing in Flux:ENV

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Departure with fish onboard from another fishing trip
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Dep_with_fish_2()
//        {
//            try
//            {
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "8826952f-b240-4570-a9dc-59f3a24c7bf1"
//                    }
//                };
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // DB Nomenclature - MDR_FLUX_GP_Purpose
//                // Value 9 - Original
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };

//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-710301" };
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    // DB Nomenclature - MDR_FLUX_GP_Party
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Report_Type
//                // Value - Declaration or Notification
//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "1e1bff95-dfff-4cc3-82d3-d72b46fda745"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    // DB Nomenclature - MDR_FLUX_GP_Purpose
//                    // Value 9 - Original
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    // DB Nomenclature - MDR_FLUX_GP_PARTY
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // Type - code
//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Type
//                // Value - Departure
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "DEPARTURE";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                // Datetime of occurance/ creation of data msg on board of fishing vessel
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // Reason
//                // Reason of departure
//                // DB Nomenclature - FLUX_FA.MDR_FA_Reason_Departure
//                // Value - FIS -> fishing
//                CodeType ctypeReason = new CodeType();
//                ctypeReason.ListID = "FA_REASON_DEPARTURE";
//                ctypeReason.Value = "FIS";
//                FAct.ReasonCode = ctypeReason;

//                // Without Specified FA Catch - catches on board

//                FAct.SpecifiedFACatch = new FACatchType[] {
//                    // first species on board
//                    new FACatchType(){
//                        // Species code
//                        // DB Nomenclature - MDR_FAO_species
//                        SpeciesCode  = new CodeType() {
//                            ListID = "FAO_SPECIES",
//                            Value = "COD"
//                        },
//                        // WeightMeasure
//                        // DB Nomenclature - MDR_FLUX_Unit
//                        WeightMeasure = new MeasureType() {
//                            unitCode = "KGM",
//                            Value = 50
//                        },
//                        // Weight Means Code
//                        // DB Nomenclature - MDR_Weight_Measure_Type
//                        WeighingMeansCode = new CodeType() {
//                            ListID = "WEIGHT_MEANS",
//                            Value = "ESTIMATED"
//                        },
//                        // Type code
//                        // DB Nomenclatures - FLUX_FA.MDR_FA_Catch_Type
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CATCH_TYPE",
//                            Value = "ONBOARD"
//                        },
//                        // Catches of another fishing trip - OPTIONAL
//                        RelatedFishingTrip = new FishingTripType[] {
//                            // first related fishing trip
//                            // DB Nomenclature - FLUX_FA.MDR_FA_Trip_Id_Type
//                           // Value - Unique ID Type
//                           // Check Trip Number Convention
//                            new FishingTripType() {
//                                ID = new IDType[] {
//                                    // first ID
//                                    new IDType() {
//                                        SchemeID = "EU_TRIP_ID",
//                                        Value = "SRC-TRP-T2T20200506193933176"
//                                    }
//                                }
//                            }
//                        },
//                        // Legally sized fish
//                        SpecifiedSizeDistribution = new SizeDistributionType(){
//                            ClassCode = new CodeType[] {
//                                // first size
//                                // DB Nomenclature - MDR_Fish_Size_Class
//                                new CodeType(){
//                                    ListID = "FISH_SIZE_CLASS",
//                                    Value = "LSC"
//                                }
//                            }
//                        },
//                        // Processing information - OPTIONAL
//                        AppliedAAPProcess = new AAPProcessType[] {
//                            // first process type
//                            new AAPProcessType() {
//                                // Presentation list
//                                // DB Nomeclature - MDR_Fish_Presentation
//                                TypeCode = new CodeType[] {
//                                    // first type of presentation
//                                    new CodeType() { ListID = "FISH_PRESENTATION", Value = "GUT"},
//                                    // second type of presentation
//                                    new CodeType() { ListID = "FISH_PRESENTATION", Value = "FRO"}
//                                },
//                                // Convertion Factor
//                                ConversionFactorNumeric = new NumericType() {
//                                    Value = new decimal(1.1)
//                                },
//                                // result AAP Product
//                                ResultAAPProduct = new AAPProductType[] {
//                                    // first product
//                                    new AAPProductType() {
//                                        // weight
//                                        WeightMeasure = new MeasureType() {
//                                            Value = new decimal(53.58)
//                                        },
//                                        // packaging unit quantity
//                                        // DB Nomeclature - MDR_FLUX_Unit
//                                        PackagingUnitQuantity = new QuantityType() {
//                                            unitCode = "C62",
//                                            Value = new decimal(5)
//                                        },
//                                        // packaging type code
//                                        // DB Nomeclature - FLUX_FA.MDR_Fish_Packaging
//                                        PackagingTypeCode = new CodeType() {
//                                            ListID = "FISH_PACKAGING",
//                                            Value = "BOX"
//                                        },
//                                        // packaging unit average weight
//                                        // DB Nomeclature - MDR_FLUX_Unit
//                                        PackagingUnitAverageWeightMeasure = new MeasureType() {
//                                            unitCode = "KGM",
//                                            Value = new decimal(10)
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                };

//                // Related Flux Location
//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                // DB Nomenclature - MDR_FLUX_Location_Type
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "LOCATION" };
//                // DB Nomenclature - MDR_Territory
//                fLUXLocation.CountryID = new IDType() { SchemeID = "TERRITORY", Value = "BGR" };
//                // DB Nomenclature - MDR_LOCATION
//                // Value  - BGBOJ - Burgas
//                fLUXLocation.ID = new IDType() { SchemeID = "LOCATION", Value = "BGBOJ" };
//                FAct.RelatedFLUXLocation = new FLUXLocationType[] { fLUXLocation };

//                // Specified Fishing Gear
//                FishingGearType fishingGear = new FishingGearType();
//                // DB Nomenclature - MDR_Gear_Type
//                // Value - PS
//                fishingGear.TypeCode = new CodeType() { ListID = "GEAR_TYPE", Value = "PS" };
//                // DB Nomenclature - FLUX_FA.MDR_FA_Gear_Role
//                fishingGear.RoleCode = new CodeType[] { new CodeType() { ListID = "FA_GEAR_ROLE", Value = "ONBOARD" } };
//                // DB Nomenclature - FLUX_FA.MDR_FA_Gear_Characteristic
//                fishingGear.ApplicableGearCharacteristic = new GearCharacteristicType[] {
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "ME"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MMT",
//                            Value = 140
//                        }
//                    } };
//                FAct.SpecifiedFishingGear = new FishingGearType[] { fishingGear };

//                // Trip information
//                // DB Nomenclature - FLUX_FA.MDR_FA_Trip_Id_Type
//                // Value - Unique ID Type
//                // Check Trip Number Convention
//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-T2T20200506193933176"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;
//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();
//                // Vessel Identification CFR, UVI
//                vesselTransport.ID = new IDType[] {
//                    new IDType() { SchemeID  = "CFR", Value = "BGR123456789" },
//                    new IDType() { SchemeID = "UVI", Value = "1234567"}
//                };
//                // Vessel Name
//                vesselTransport.Name = new TextType[] {
//                    new TextType() { Value = "GOLF"}
//                };
//                // Registration Vessel Country
//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "CYR"
//                    }
//                };

//                // Specified Contact Party
//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                         // RoleCode
//                        // DB Nomenclature - MDR_FLUX_Contact_Role
//                        RoleCode = new List<CodeType> {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "BGR"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Daily fishing operation
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Daily_fishing_operation_2()
//        {
//            try
//            {
//                // Data structure
//                // Flux FA Report Msg consts:
//                // 1. Flux Report Document
//                // 2. FA Report Document

//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                // FLUX Report Document
//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID -   UUID generated in FVMS, ISS
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "196aca16-da66-4077-b340-ecad701be662"
//                    }
//                };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                // DB Nomenclature - MDR_FLUX_GP_Purpose
//                // Value 9 - Original
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-710501" };
//                // Owner flux party
//                // DB Nomenclature - MDR_FLUX_GP_Party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Report_Type
//                // Value - Declaration or Notification
//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                // Datetime of acceptance in FVMS/СНРК
//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with section related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "b2fca5fb-d1cd-4ec7-8a8c-645cecab6866"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    // DB Nomenclature - MDR_FLUX_GP_Purpose
//                    // Value - 9 Original
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    // DB Nomenclature - MDR_FLUX_GP_Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Type
//                // Value - Departure
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "FISHING_OPERATION";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                // Datetime of occurance/ creation of data msg on board of fishing vessel
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // DB Nomenclature - MDR_Vessel_Activity
//                // Value - FIS -> fishing
//                FAct.VesselRelatedActivityCode = new CodeType()
//                {
//                    ListID = "VESSEL_ACTIVITY",
//                    Value = "FIS"
//                };

//                // packaging unit quantity
//                // DB Nomeclature - MDR_FLUX_Unit
//                FAct.OperationsQuantity = new QuantityType()
//                {
//                    unitCode = "C62",
//                    Value = new decimal(1)
//                };

//                FAct.SpecifiedFACatch = new FACatchType[] {
//                    // first species on board
//                    new FACatchType(){
//                        // Species code
//                        // DB Nomenclature - MDR_FAO_species
//                        SpeciesCode  = new CodeType() {
//                            ListID = "FAO_SPECIES",
//                            Value = "COD"
//                        },
//                        // WeightMeasure
//                        // DB Nomenclature - MDR_FLUX_Unit
//                        WeightMeasure = new MeasureType() {
//                            unitCode = "KGM",
//                            Value = new decimal(1000)
//                        },

//                        // Type code
//                        // DB Nomenclatures - FLUX_FA.MDR_FA_Catch_Type
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CATCH_TYPE",
//                            Value = "ONBOARD"
//                        },
//                        // Legally sized fish
//                        // DB Nomenclature - MDR_Fish_Size_Class
//                        SpecifiedSizeDistribution = new SizeDistributionType(){
//                            ClassCode = new CodeType[] {
//                                // first size
//                                new CodeType(){
//                                    ListID = "FISH_SIZE_CLASS",
//                                    Value = "LSC"
//                                }
//                            }
//                        }
//                    }
//                };

//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                // DB Nomenclature - MDR_FLUX_Location_Type
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "AREA" };
//                // DB Nomenclature - FLUX_FA.MDR_Management_Area
//                fLUXLocation.ID = new IDType() { SchemeID = "MANAGEMENT_AREA", Value = "NEAFC_RA" };

//                // DB Nomenclature - MDR_FLUX_Location_Type
//                FLUXLocationType fLUXLocation2 = new FLUXLocationType();
//                fLUXLocation2.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "AREA" };
//                // DB Nomenclature - MDR_FAO_Fishing_Area
//                fLUXLocation2.ID = new IDType() { SchemeID = "FAO_AREA", Value = "27.8.e.1" };

//                FLUXLocationType fLUXLocation3 = new FLUXLocationType();
//                // DB Nomenclature - MDR_FLUX_Location_Type
//                fLUXLocation3.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "AREA" };
//                // DB Nomenclature - MDR_Stat_Rect
//                fLUXLocation3.ID = new IDType() { SchemeID = "STAT_RECTANGLE", Value = "21D5" };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation,
//                    fLUXLocation2,
//                    fLUXLocation3
//                };

//                FishingGearType fishingGear = new FishingGearType();
//                // DB Nomenclature - MDR_Gear_Type
//                fishingGear.TypeCode = new CodeType() { ListID = "GEAR_TYPE", Value = "TBB" };
//                // DB Nomenclature - FLUX_FA.MDR_FA_Gear_Characteristic
//                fishingGear.ApplicableGearCharacteristic = new GearCharacteristicType[] {
//                    // first characteristic for gear type TBB
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "ME"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MMT",
//                            Value = new decimal(140)
//                        }
//                    },
//                    // second characteristic for gear type TBB
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "GM"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(250)
//                        }
//                    },
//                    // third characteristic for gear type TBB
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "GM"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "C62",
//                            Value = new decimal(3)
//                        }
//                    }
//                };
//                FAct.SpecifiedFishingGear = new FishingGearType[] { fishingGear };

//                // DB Nomenclature - MDR_FLUX_Unit
//                FAct.SpecifiedDelimitedPeriod = new DelimitedPeriodType[] {
//                    new DelimitedPeriodType() {
//                        DurationMeasure = new MeasureType() {
//                            unitCode = "MIN",
//                            Value = new decimal(240)
//                        }
//                    }
//                };

//                // Trip information
//                // DB Nomenclature - FLUX_FA.MDR_FA_Trip_Id_Type
//                // Value - Unique ID Type
//                // Check Trip Number Convention
//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506193959462"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                // Vessel Identification CFR, UVI
//                vesselTransport.ID = new IDType[] {
//                    new IDType() { SchemeID  = "CFR", Value = "BGR123456789" }
//                };

//                // DB Nomenclature - MDR_Territory
//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                           // DB Nomenclature - MDR_FLUX_Contact_Role
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "XEU"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Prior notification of arrival to port, reason landing
//        /// </summary>
//        /// <returns></returns>
//        public static FLUXFAReportMessageType Gen_FA_Prior_Not_Arrival_Port_Landing_2()
//        {
//            try
//            {
//                // Data structure
//                // Flux FA Report Msg consts:
//                // 1. Flux Report Document
//                // 2. FA Report Document

//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                // FLUX Report Document
//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID -   UUID generated in FVMS, ISS
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "196aca16-da66-4077-b340-ecad701be662"
//                    }
//                };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                // DB Nomenclature - MDR_FLUX_GP_Purpose
//                // Value 9 - Original
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-710501" };
//                // Owner flux party
//                // DB Nomenclature - MDR_FLUX_GP_Party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Report_Type
//                // Value - Declaration or Notification
//                fARepDoc.TypeCode = new CodeType()
//                {
//                    ListID = "FLUX_FA_REPORT_TYPE",
//                    Value = "NOTIFICATION"
//                };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "UUID",
//                            Value  = "7ee30c6c-adf9-4f60-a4f1-f7f15ab92803"
//                        }
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    // DB Nomenclature - MDR_FLUX_GP_Purpose
//                    // Value 9 - Original
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    // DB Nomenclature - MDR_FLUX_GP_PARTY
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // Type - code
//                // DB Nomenclature - FLUX_FA.MDR_FLUX_FA_Type
//                // Value - ARRIVAL
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "ARRIVAL";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                // Datetime of occurance/ creation of data msg on board of fishing vessel
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // DB Nomenclature -  - FLUX_FA.MDR_FA_Reason_Arrival
//                CodeType ctypeReason = new CodeType();
//                ctypeReason.ListID = "FA_REASON_ARRIVAL";
//                ctypeReason.Value = "LAN";
//                FAct.ReasonCode = ctypeReason;

//                FAct.SpecifiedFACatch = new FACatchType[] {
//                    // first species on board
//                    new FACatchType(){
//                        // DB Nomenclature -  - MDR_FAO_species
//                        SpeciesCode  = new CodeType() {
//                            ListID = "FAO_SPECIES",
//                            Value = "GHL"
//                        }

//                        ,

//                        // DB Nomenclature - MDR_FLUX_Unit
//                        WeightMeasure = new MeasureType() {
//                            unitCode = "KGM",
//                            Value = new decimal(1500)
//                        }

//                        ,

//                        // DB Nomenclature - FLUX_FA.MDR_FA_Catch_Type
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CATCH_TYPE",
//                            Value = "ONBOARD"
//                        }

//                        ,

//                        // DB Nomenclature - MDR_Fish_Size_Class
//                        SpecifiedSizeDistribution = new SizeDistributionType(){
//                            ClassCode = new CodeType[] {
//                                // first size
//                                new CodeType(){
//                                    ListID = "FISH_SIZE_CLASS",
//                                    Value = "LSC"
//                                }
//                            }
//                        }
//                    }
//                    ,
//                    // second fa catch record - unloading declarated catch
//                    new FACatchType() {
//                        SpeciesCode = new CodeType() {
//                            ListID = "FAO_SPECIES",
//                            Value = "GHL"
//                        }
//                        ,
//                        WeightMeasure = new MeasureType() {
//                            unitCode = "KGM",
//                            Value = new decimal(1500)
//                        }
//                        ,
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CATCH_TYPE",
//                            Value = "UNLOADED"
//                        }
//                        ,
//                        SpecifiedSizeDistribution = new SizeDistributionType() {
//                            ClassCode = new CodeType[] {
//                                new CodeType()
//                                {
//                                    ListID = "FISH_SIZE_CLASS",
//                                    Value = "LSC"
//                                }
//                            }
//                        }
//                    }
//                };

//                // location
//                // DB Nomenclature - MDR_FLUX_Location_Type

//                FLUXLocationType fLUXLocation = new FLUXLocationType();

//                // DB Nomenclature - MDR_FLUX_Location_Type
//                fLUXLocation.TypeCode = new CodeType()
//                {
//                    ListID = "FLUX_LOCATION_TYPE",
//                    Value = "LOCATION"
//                };

//                // DB Nomenclature - MDR_Territory
//                fLUXLocation.CountryID = new IDType()
//                {
//                    SchemeID = "TERRITORY",
//                    Value = "BGR"
//                };

//                // DB Nomenclature - MDR_LOCATION
//                // Value  - BGBOJ - Burgas
//                fLUXLocation.ID = new IDType()
//                {
//                    SchemeID = "LOCATION",
//                    Value = "BGBOJ"
//                };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                // Trip information
//                // DB Nomenclature - FLUX_FA.MDR_FA_Trip_Id_Type
//                // Value - Unique ID Type
//                // Check Trip Number Convention
//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194103340"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                // Vessel Identification CFR, UVI, etc.
//                vesselTransport.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID  = "CFR",
//                        Value = "BGR123456789"
//                    }
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    // DB Nomenclature - MDR_Territory
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        // DB Nomenclature - MDR_FLUX_Contact_Role
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                // DB Nomenclature - MDR_Territory
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "BGR"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                       // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Prior notification of arrival to port, reason shelter
//        /// </summary>
//        /// <returns></returns>
//        public static FLUXFAReportMessageType Gen_FA_Prior_Not_Arrival_Port_Shelter_2()
//        {
//            try
//            {
//                // Data structure
//                // Flux FA Report Msg consts:
//                // 1. Flux Report Document
//                // 2. FA Report Document

//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID - UUID
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "7cfcdde3-286c-4713-8460-2ed82a59be34"
//                    }
//                };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-711002" };
//                // Owner flux party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                // DB - FLUX_FA.MDR_FLUX_FA_Report_Type
//                fARepDoc.TypeCode = new CodeType()
//                {
//                    ListID = "FLUX_FA_REPORT_TYPE",
//                    Value = "NOTIFICATION"
//                };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "UUID",
//                            Value  = "fc16ea8a-3148-44b2-977f-de2a2ae550b9"
//                        }
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    // DB - MDR_FLUX_GP_Purpose
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // DB - FLUX_FA.MDR_FLUX_FA_Type
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "ARRIVAL";
//                FAct.TypeCode = codeType;

//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // DB - FLUX_FA.MDR_FA_Reason_Arrival
//                CodeType ctypeReason = new CodeType();
//                ctypeReason.ListID = "FA_REASON_ARRIVAL";
//                ctypeReason.Value = "SHE";
//                FAct.ReasonCode = ctypeReason;

//                // #################################################
//                // Without Specified FA Catch
//                // #################################################

//                // location of Discard operation
//                // DB - MDR_FLUX_Location_Type

//                FLUXLocationType fLUXLocation = new FLUXLocationType();

//                fLUXLocation.TypeCode = new CodeType()
//                {
//                    ListID = "FLUX_LOCATION_TYPE",
//                    Value = "LOCATION"
//                };

//                fLUXLocation.CountryID = new IDType()
//                {
//                    SchemeID = "TERRITORY",
//                    Value = "GBR"
//                };

//                fLUXLocation.ID = new IDType()
//                {
//                    SchemeID = "LOCATION",
//                    Value = "BGBOJ"
//                };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                // DB - FLUX_FA.MDR_FA_Trip_Id_Type
//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194109200"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                vesselTransport.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID  = "CFR",
//                        Value = "BGR123456789"
//                    }
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "CYR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "BGR"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Arrival Declaration - Refilling
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Arrival_Declaration_Refilling_2()
//        {
//            try
//            {
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID - UUID
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "4f971076-e6c6-48f6-b87e-deae90fe4705"
//                    }
//                };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-711101" };
//                // Owner flux party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "cc45063f-2d3c-4cda-ac0c-8381e279e150"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // Type - code
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "ARRIVAL";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // Reason for port call - REFUELLING
//                FAct.ReasonCode = new CodeType()
//                {
//                    ListID = "FA_REASON_ARRIVAL",
//                    Value = "REF"
//                };

//                // Operatioins Quantity
//                FAct.OperationsQuantity = new QuantityType()
//                {
//                    unitCode = "C62",
//                    Value = new decimal(1)
//                };

//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "LOCATION" };
//                fLUXLocation.CountryID = new IDType() { SchemeID = "TERRITORY", Value = "BGR" };
//                fLUXLocation.ID = new IDType() { SchemeID = "LOCATION", Value = "BGBOJ" };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194115013"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                vesselTransport.ID = new IDType[] {
//                    new IDType() { SchemeID  = "CFR", Value = "BGR123456789" },
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "BGR"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Arrival Declaration - Landing
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Arrival_Declaration_Landing_2()
//        {
//            try
//            {
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID - UUID
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType {
//                        SchemeID = "UUID",
//                        Value = "8f06061e-e723-4b89-8577-3801a61582a2"
//                    }
//                };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-711102" };
//                // Owner flux party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "dde5df56-24c2-4a2e-8afb-561f32113256"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // Type - code
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "ARRIVAL";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // Reason for port call - REFUELLING
//                FAct.ReasonCode = new CodeType()
//                {
//                    ListID = "FA_REASON_ARRIVAL",
//                    Value = "LAN"
//                };

//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "LOCATION" };
//                fLUXLocation.CountryID = new IDType() { SchemeID = "TERRITORY", Value = "BGR" };
//                fLUXLocation.ID = new IDType() { SchemeID = "LOCATION", Value = "BGBOJ" };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                FAct.SpecifiedFLUXCharacteristic = new FLUXCharacteristicType[] {
//                    new FLUXCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CHARACTERISTIC",
//                            Value = "START_DATETIME_LANDING"
//                        },
//                        ValueDateTime = new DateTimeType() {
//                            Item = DateTime.Now
//                        }
//                    }
//                };

//                // NO FISHING GEAR ONBOARD

//                FishingGearType fishingGear = new FishingGearType();
//                // Type Code
//                fishingGear.TypeCode = new CodeType() { ListID = "GEAR_TYPE", Value = "GN" };

//                // Role Code
//                fishingGear.RoleCode = new CodeType[]  {
//                    new CodeType() {
//                        ListID = "FA_GEAR_ROLE",
//                        Value = "ONBOARD"
//                    }
//                };

//                // Applicable Gear Charaacteristic
//                fishingGear.ApplicableGearCharacteristic = new GearCharacteristicType[] {
//                    // first characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "ME"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MMT",
//                            Value = new decimal(140)
//                        }
//                    },
//                    // second characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "GM"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(1000)
//                        }
//                    },
//                    // third characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "HE"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(25)
//                        }
//                    },
//                    // forth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "NL"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(100)
//                        }
//                    },
//                    // fifth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "NN"
//                        },
//                        ValueQuantity = new QuantityType() {
//                            Value = new decimal(10)
//                        }
//                    },
//                    // sixth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "QG"
//                        },
//                        ValueQuantity = new QuantityType() {
//                            Value = new decimal(1)
//                        }
//                    }
//                };
//                FAct.SpecifiedFishingGear = new FishingGearType[] { fishingGear };

//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194120712"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                vesselTransport.ID = new IDType[] {
//                    new IDType() { SchemeID  = "CFR", Value = "BGR123456789" },
//                    new IDType() { SchemeID = "IRCS", Value = "IRCS6"},
//                    new IDType() { SchemeID = "EXT_MARK", Value = "XR006"}
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "XEU"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                       SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Landing Declaration
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Landing_Declaration_2()
//        {
//            try
//            {
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID - UUID
//                fLUXRepDoc.ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "UUID",
//                            Value = "8f06061e-e723-4b89-8577-3801a61582a2"
//                        }
//                    };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-711102" };
//                // Owner flux party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "dde5df56-24c2-4a2e-8afb-561f32113256"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // Type - code
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "ARRIVAL";
//                FAct.TypeCode = codeType;

//                // Occurrence - Datetime
//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // Reason for port call - REFUELLING
//                FAct.ReasonCode = new CodeType()
//                {
//                    ListID = "FA_REASON_ARRIVAL",
//                    Value = "LAN"
//                };

//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "LOCATION" };
//                fLUXLocation.CountryID = new IDType() { SchemeID = "TERRITORY", Value = "BGR" };
//                fLUXLocation.ID = new IDType() { SchemeID = "LOCATION", Value = "BGBOJ" };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                FAct.SpecifiedFLUXCharacteristic = new FLUXCharacteristicType[] {
//                    new FLUXCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CHARACTERISTIC",
//                            Value = "START_DATETIME_LANDING"
//                        },
//                        ValueDateTime = new DateTimeType() {
//                            Item = DateTime.Now
//                        }
//                    }
//                };

//                // NO FISHING GEAR ONBOARD

//                FishingGearType fishingGear = new FishingGearType();
//                // Type Code
//                fishingGear.TypeCode = new CodeType() { ListID = "GEAR_TYPE", Value = "GN" };

//                // Role Code
//                fishingGear.RoleCode = new CodeType[]  {
//                    new CodeType() {
//                        ListID = "FA_GEAR_ROLE",
//                        Value = "ONBOARD"
//                    }
//                };

//                // Applicable Gear Charaacteristic
//                fishingGear.ApplicableGearCharacteristic = new GearCharacteristicType[] {
//                    // first characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "ME"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MMT",
//                            Value = new decimal(140)
//                        }
//                    },
//                    // second characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "GM"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(1000)
//                        }
//                    },
//                    // third characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "HE"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(25)
//                        }
//                    },
//                    // forth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "NL"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(100)
//                        }
//                    },
//                    // fifth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "NN"
//                        },
//                        ValueQuantity = new QuantityType() {
//                            Value = new decimal(10)
//                        }
//                    },
//                    // sixth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "QG"
//                        },
//                        ValueQuantity = new QuantityType() {
//                            Value = new decimal(1)
//                        }
//                    }
//                };
//                FAct.SpecifiedFishingGear = new FishingGearType[] { fishingGear };

//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194120712"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                vesselTransport.ID = new IDType[] {
//                    new IDType() { SchemeID  = "CFR", Value = "BGR123456789" },
//                    new IDType() { SchemeID = "IRCS", Value = "IRCS6"},
//                    new IDType() { SchemeID = "EXT_MARK", Value = "XR006"}
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "XEU"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                       SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - Fishing Operation NIL Catches
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Nil_Catches_2()
//        {
//            try
//            {
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID - UUID
//                fLUXRepDoc.ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "UUID",
//                            Value = "251db84c-1d8b-49be-b426-f70bb2c68a2d"
//                        }
//                    };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-710521" };
//                // Owner flux party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                };

//                //##########################################################

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                // DB - FLUX_FA.MDR_FLUX_FA_Report_Type
//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "fe7acdb9-ff2e-4cfa-91a9-fd2e06b556e1"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // DB - FLUX_FA.MDR_FLUX_FA_Type
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "FISHING_OPERATION";
//                FAct.TypeCode = codeType;

//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // DB - MDR_Vessel_Activity
//                FAct.VesselRelatedActivityCode = new CodeType()
//                {
//                    ListID = "VESSEL_ACTIVITY",
//                    Value = "STE"
//                };

//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "AREA" };
//                fLUXLocation.ID = new IDType() { SchemeID = "TERRITORY", Value = "ESP" };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                FAct.SpecifiedFLUXCharacteristic = new FLUXCharacteristicType[] {
//                    new FLUXCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CHARACTERISTIC",
//                            Value = "START_DATETIME_LANDING"
//                        },
//                        ValueDateTime = new DateTimeType() {
//                            Item = DateTime.Now
//                        }
//                    }
//                };

//                // NO FISHING GEAR ONBOARD

//                FishingGearType fishingGear = new FishingGearType();
//                // Type Code
//                fishingGear.TypeCode = new CodeType() { ListID = "GEAR_TYPE", Value = "GN" };

//                // Role Code
//                fishingGear.RoleCode = new CodeType[]  {
//                    new CodeType() {
//                        ListID = "FA_GEAR_ROLE",
//                        Value = "ONBOARD"
//                    }
//                };

//                // Applicable Gear Charaacteristic
//                fishingGear.ApplicableGearCharacteristic = new GearCharacteristicType[] {
//                    // first characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "ME"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MMT",
//                            Value = new decimal(140)
//                        }
//                    },
//                    // second characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "GM"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(1000)
//                        }
//                    },
//                    // third characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "HE"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(25)
//                        }
//                    },
//                    // forth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "NL"
//                        },
//                        ValueMeasure = new MeasureType(){
//                            unitCode = "MTR",
//                            Value = new decimal(100)
//                        }
//                    },
//                    // fifth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "NN"
//                        },
//                        ValueQuantity = new QuantityType() {
//                            Value = new decimal(10)
//                        }
//                    },
//                    // sixth characteristic for gear type GN
//                    new GearCharacteristicType() {
//                        TypeCode = new CodeType() {
//                            ListID = "FA_GEAR_CHARACTERISTIC", Value = "QG"
//                        },
//                        ValueQuantity = new QuantityType() {
//                            Value = new decimal(1)
//                        }
//                    }
//                };
//                FAct.SpecifiedFishingGear = new FishingGearType[] { fishingGear };

//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194011291"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                vesselTransport.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID  = "CFR",
//                        Value = "BGR123456789"
//                    }
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "XEU"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Discard
//        /// </summary>
//        public static FLUXFAReportMessageType Gen_FA_Discard_2()
//        {
//            try
//            {
//                FLUXFAReportMessageType fLUXFARepMessage = new FLUXFAReportMessageType();

//                FLUXReportDocumentType fLUXRepDoc = new FLUXReportDocumentType();
//                // ID - UUID
//                fLUXRepDoc.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID = "UUID",
//                        Value = "0e1ea2b6-f4f5-4958-bc48-cfb016a22f58"
//                    }
//                };
//                // Creatioin Time
//                fLUXRepDoc.CreationDateTime = new DateTimeType() { Item = DateTime.Now };
//                // Purpose code
//                fLUXRepDoc.PurposeCode = new CodeType() { ListID = "FLUX_GP_PURPOSE", Value = "9" };
//                // Purpose
//                fLUXRepDoc.Purpose = new TextType() { Value = "FLUX-FA-EU-710701" };
//                // Owner flux party
//                fLUXRepDoc.OwnerFLUXParty = new FLUXPartyType()
//                {
//                    ID = new IDType[] {
//                        new IDType() {
//                            SchemeID = "FLUX_GP_PARTY",
//                            Value = "BGR"
//                        }
//                    }
//                };

//                FAReportDocumentType fARepDoc = new FAReportDocumentType();

//                fARepDoc.TypeCode = new CodeType() { ListID = "FLUX_FA_REPORT_TYPE", Value = "DECLARATION" };

//                fARepDoc.AcceptanceDateTime = new DateTimeType() { Item = DateTime.Now };

//                // with related report document

//                fARepDoc.RelatedFLUXReportDocument = new FLUXReportDocumentType()
//                {
//                    // IDs
//                    ID = new IDType[] {
//                        new IDType() { SchemeID = "UUID", Value  = "a913a52e-5e66-4f40-8c64-148f90fa8cd9"}
//                    },
//                    // Creation DateTime
//                    CreationDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    },
//                    // Purpose Code
//                    PurposeCode = new CodeType()
//                    {
//                        ListID = "FLUX_GP_PURPOSE",
//                        Value = "9"
//                    },
//                    // Owner Flux Party
//                    OwnerFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }
//                };

//                FishingActivityType FAct = new FishingActivityType();

//                // DB - FLUX_FA.MDR_FLUX_FA_Type
//                CodeType codeType = new CodeType();
//                codeType.ListID = "FLUX_FA_TYPE";
//                codeType.Value = "DISCARD";
//                FAct.TypeCode = codeType;

//                DateTimeType dateTimeType = new DateTimeType();
//                dateTimeType.Item = DateTime.Now;
//                FAct.OccurrenceDateTime = dateTimeType;

//                // DB - FLUX_FA.MDR_FA_Reason_Discard
//                CodeType ctypeReason = new CodeType();
//                ctypeReason.ListID = "FA_REASON_DISCARD";
//                ctypeReason.Value = "PDM";
//                FAct.ReasonCode = ctypeReason;

//                FAct.SpecifiedFACatch = new FACatchType[] {
//                    // first species on board
//                    new FACatchType(){
//                        // DB - MDR_FAO_species
//                        SpeciesCode  = new CodeType() {
//                            ListID = "FAO_SPECIES",
//                            Value = "COD"
//                        }

//                        ,

//                        WeightMeasure = new MeasureType() {
//                            unitCode = "KGM",
//                            Value = new decimal(100)
//                        }

//                        ,

//                        // DB - FLUX_FA.MDR_FA_Catch_Type
//                        TypeCode = new CodeType() {
//                            ListID = "FA_CATCH_TYPE",
//                            Value = "DISCARDED"
//                        }

//                        ,

//                        // DB - MDR_Fish_Size_Class
//                        SpecifiedSizeDistribution = new SizeDistributionType(){
//                            ClassCode = new CodeType[] {
//                                // first size
//                                new CodeType(){
//                                    ListID = "FISH_SIZE_CLASS",
//                                    Value = "LSC"
//                                }
//                            }
//                        }
//                    }
//                };

//                // location of Discard operation

//                FLUXLocationType fLUXLocation = new FLUXLocationType();
//                fLUXLocation.TypeCode = new CodeType() { ListID = "FLUX_LOCATION_TYPE", Value = "POSITION" };
//                fLUXLocation.SpecifiedPhysicalFLUXGeographicalCoordinate = new FLUXGeographicalCoordinateType()
//                {
//                    LongitudeMeasure = new MeasureType()
//                    {
//                        Value = new decimal(-14.516)
//                    },
//                    LatitudeMeasure = new MeasureType()
//                    {
//                        Value = new decimal(46.758)
//                    }
//                };

//                FAct.RelatedFLUXLocation = new FLUXLocationType[] {
//                    fLUXLocation
//                };

//                // DB - FLUX_FA.MDR_FA_Trip_Id_Type
//                FishingTripType fishingTrip = new FishingTripType()
//                {
//                    ID = new IDType[]  {
//                        new IDType() {
//                            SchemeID = "EU_TRIP_ID",
//                            Value = "SRC-TRP-TTT20200506194034449"
//                        }
//                    }
//                };

//                FAct.SpecifiedFishingTrip = fishingTrip;

//                // add fishing activity to fa report document
//                fARepDoc.SpecifiedFishingActivity = new FishingActivityType[] { FAct };

//                VesselTransportMeansType vesselTransport = new VesselTransportMeansType();

//                vesselTransport.ID = new IDType[] {
//                    new IDType() {
//                        SchemeID  = "CFR",
//                        Value = "BGR123456789"
//                    }
//                };

//                vesselTransport.RegistrationVesselCountry = new VesselCountryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "TERRITORY",
//                        Value = "BGR"
//                    }
//                };

//                vesselTransport.SpecifiedContactParty = new ContactPartyType[] {
//                    new ContactPartyType(){
//                        // RoleCode
//                        RoleCode = new CodeType[] {
//                            new CodeType() {
//                                ListID = "FLUX_CONTACT_ROLE",
//                                Value = "MASTER"
//                            }
//                        },
//                        // Specified Structured Address
//                        SpecifiedStructuredAddress = new StructuredAddressType[] {
//                            new StructuredAddressType() {
//                                StreetName = new TextType() { Value = "ABC"},
//                                CityName = new TextType() {Value = "BURGAS"},
//                                CountryID = new IDType() {
//                                    SchemeID = "TERRITORY",
//                                    Value = "XEU"
//                                },
//                                PlotIdentification = new TextType() { Value = "17"},
//                                PostalArea = new TextType() { Value = "14390"}
//                            }
//                        },
//                        // Specified Contact Person
//                        SpecifiedContactPerson = new ContactPersonType[] {
//                            new ContactPersonType() {
//                                GivenName = new TextType() { Value = "Ivan"},
//                                FamilyName = new TextType() { Value = "Petrov"},
//                                Alias = new TextType() { Value = "Morski vylk"}
//                            }
//                        }
//                    }
//                };

//                fARepDoc.SpecifiedVesselTransportMeans = vesselTransport;

//                fLUXFARepMessage.FLUXReportDocument = fLUXRepDoc;
//                fLUXFARepMessage.FAReportDocument = new FAReportDocumentType[] { fARepDoc };

//                //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fLUXFARepMessage.GetType());
//                //x.Serialize(Console.Out, fLUXFARepMessage);

//                return fLUXFARepMessage;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Flux - FA - Query with parameters - VesselID, Period of time
//        /// </summary>
//        /// <returns></returns>
//        public static FLUXFAQueryMessageType Gen_FA_Query_VesselID_And_Period_2()
//        {
//            try
//            {
//                FLUXFAQueryMessageType fLUXFAQueryMessage = new FLUXFAQueryMessageType();

//                fLUXFAQueryMessage.FAQuery = new FAQueryType()
//                {
//                    ID = new IDType()
//                    {
//                        SchemeID = "UUID",
//                        Value = "735a3633-ab2b-435a-b9a3-65e05ca2b774"
//                    }

//                    ,

//                    SubmittedDateTime = new DateTimeType()
//                    {
//                        Item = DateTime.Now
//                    }

//                    ,

//                    // DB - FLUX_FA.MDR_FA_Query_Type
//                    TypeCode = new CodeType()
//                    {
//                        ListID = "FA_QUERY_TYPE",
//                        Value = "VESSEL"
//                    }

//                    ,

//                    // Change start and end period of query
//                    SpecifiedDelimitedPeriod = new DelimitedPeriodType()
//                    {
//                        StartDateTime = new DateTimeType()
//                        {
//                            Item = DateTime.Now.AddDays(-1)
//                        },
//                        EndDateTime = new DateTimeType()
//                        {
//                            Item = DateTime.Now
//                        }
//                    }

//                    ,

//                    SubmitterFLUXParty = new FLUXPartyType()
//                    {
//                        ID = new IDType[] {
//                            new IDType() {
//                                // DB - MDR_FLUX_GP_Party
//                                SchemeID = "FLUX_GP_PARTY",
//                                Value = "BGR"
//                            }
//                        }
//                    }

//                    ,

//                    SimpleFAQueryParameter = new FAQueryParameterType[] {
//                        // first parameter
//                        new FAQueryParameterType() {
//                            // DB - FLUX_FA.MDR_FA_Query_Parameter
//                            TypeCode = new CodeType() {
//                                ListID = "FA_QUERY_PARAMETER",
//                                Value = "VESSELID"
//                            }
//                            ,
//                            ValueID = new IDType() {
//                                SchemeID = "CFR",
//                                Value = "BGR123456789"
//                            }
//                        }
//                        ,
//                        // second parameter
//                         new FAQueryParameterType() {
//                            // DB - FLUX_FA.MDR_FA_Query_Parameter
//                            TypeCode = new CodeType() {
//                                ListID = "FA_QUERY_PARAMETER",
//                                Value = "CONSOLIDATED"
//                            }
//                            ,
//                            ValueID = new IDType() {
//                                SchemeID = "BOOLEAN_TYPE",
//                                Value = "Y"
//                            }
//                        }
//                    }
//                };

//                return fLUXFAQueryMessage;
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }
//    }
//}
