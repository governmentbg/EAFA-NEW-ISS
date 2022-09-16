using System;
using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class ScientificPermitData
    {
        private static readonly DateTime now = DateTime.Now;

        public static List<NpermitStatus> NpermitStatuses
        {
            get
            {
                return new List<NpermitStatus>
                {
                    new NpermitStatus { Id = 1, Code = "Requested", Name = "Неодобрено" },
                    new NpermitStatus { Id = 2, Code = "Approved", Name = "Одобрено" },
                    new NpermitStatus { Id = 3, Code = "Canceled", Name = "Прекратено" },
                    new NpermitStatus { Id = 4, Code = "Expired", Name = "Изтекло" }
                };
            }
        }

        public static List<NpermitReason> NpermitReasons
        {
            get
            {
                return new List<NpermitReason>
                {
                    new NpermitReason { Id = 1, Name = "По чл. 40, ал. 1 от ЗРА" },
                    new NpermitReason { Id = 2, Name = "По чл. 40, ал. 2 от ЗРА" },
                    new NpermitReason { Id = 3, Name = "По чл. 49, ал. 1, т. 4, във връзка с чл. 44, ал. 1 и чл. 48, ал. 2, т. 5 от Закона за биологичното разнообразие" }
                };
            }
        }

        public static List<ScientificPermitRegister> ScientificPermitRegisters
        {
            get
            {
                return new List<ScientificPermitRegister>
                {
                    new ScientificPermitRegister
                    {
                        Id = 1,
                        PermitRegistrationDateTime = now,
                        PermitStatusId = NpermitStatuses[0].Id,
                        //RequestedByPersonId = PersonsData.Persons[0].Id,
                        PermitValidFrom = now.AddDays(15),
                        PermitValidTo = now.AddDays(15).AddYears(1),
                        //RequestedByOrganizationPosition = "Президент",
                        ResearchPeriodFrom = now.AddDays(30),
                        ResearchPeriodTo = now.AddDays(15).AddYears(1),
                        ResearchWaterAreas = "Река Дунав",
                        ResearchGoalsDesc = "Опазване дунавската есетра и други видове в река Дунав",
                        FishTypesDesc = "Естра",
                        FishTypesApp4Zbrdesc = null,
                        FishTypesCrayFish = "Речни раци",
                        FishingGearDescr = "Мрежа",
                        IsShipRegistered = false,
                        ShipId = null,
                        ShipName = "Лодка",
                        ShipExternalMark = "Няма",
                        ShipCaptainName = "Петко Петров",
                        IsActive = true,
                        //RequestedByOrganizationId = LegalsData.Legals[0].Id
                    },
                    new ScientificPermitRegister
                    {
                        Id = 2,
                        PermitRegistrationDateTime = now,
                        PermitStatusId = NpermitStatuses[1].Id,
                        //RequestedByPersonId = PersonsData.Persons[0].Id,
                        PermitValidFrom = now.AddDays(15),
                        PermitValidTo = now.AddDays(15).AddYears(1),
                        //RequestedByOrganizationPosition = "Софтуерен рибар",
                        ResearchPeriodFrom = now.AddDays(20),
                        ResearchPeriodTo = now.AddDays(15).AddYears(1),
                        ResearchWaterAreas = "Буркаски залив",
                        ResearchGoalsDesc = "Изхранване",
                        FishTypesDesc = "Скумрия",
                        FishTypesApp4Zbrdesc = null,
                        FishTypesCrayFish = "Морски раци",
                        FishingGearDescr = "Въдица",
                        IsShipRegistered = false,
                        ShipId = null,
                        ShipName = "Морена",
                        ShipExternalMark = "МР",
                        ShipCaptainName = "Тошко Африкански",
                        CoordinationCommittee = "Съгласуващ орган",
                        CoordinationLetterNo = "12345",
                        CoordinationDate = now.AddDays(15),
                        IsActive = true,
                        //RequestedByOrganizationId = LegalsData.Legals[1].Id
                    },
                    new ScientificPermitRegister
                    {
                        Id = 3,
                        PermitRegistrationDateTime = now,
                        PermitStatusId = NpermitStatuses[2].Id,
                        //RequestedByPersonId = PersonsData.Persons[0].Id,
                        PermitValidFrom = now.AddDays(20),
                        PermitValidTo = now.AddDays(20).AddYears(1),
                        //IsAllowedDuringMatingSeason = false,
                        //RequestedByOrganizationPosition = "Капитан",
                        ResearchPeriodFrom = now.AddDays(20),
                        ResearchPeriodTo = now.AddDays(20).AddYears(1),
                        ResearchWaterAreas = "Карибски острови",
                        ResearchGoalsDesc = "Плячка",
                        FishTypesDesc = "Кракени",
                        FishTypesApp4Zbrdesc = null,
                        FishTypesCrayFish = null,
                        FishingGearDescr = "Буркан с пръст",
                        IsShipRegistered = false,
                        ShipId = null,
                        ShipName = "Черната перла",
                        ShipExternalMark = "ЧП",
                        ShipCaptainName = "Джак Спароу",
                        CoordinationCommittee = "Тортуга",
                        CoordinationLetterNo = "555",
                        CoordinationDate = now.AddDays(25),
                        //CancellationDate = now.AddDays(100),
                        //CancellationReason = "Пиратство",
                        IsActive = true,
                        //RequestedByOrganizationId = LegalsData.Legals[2].Id
                    },
                    new ScientificPermitRegister
                    {
                        Id = 4,
                        PermitRegistrationDateTime = now.AddYears(-1),
                        PermitStatusId = NpermitStatuses[3].Id,
                        //RequestedByPersonId = PersonsData.Persons[0].Id,
                        PermitValidFrom = now.AddYears(-1).AddDays(30),
                        PermitValidTo = now.AddYears(-1).AddDays(30).AddDays(80),
                        //IsAllowedDuringMatingSeason = false,
                        //RequestedByOrganizationPosition = "Главен пазарджия",
                        ResearchPeriodFrom = now.AddYears(-1).AddDays(30),
                        ResearchPeriodTo = now.AddYears(-1).AddDays(30).AddDays(80),
                        ResearchWaterAreas = "Свищовски Дунав",
                        ResearchGoalsDesc = "Храна",
                        FishTypesDesc = "Щука, мряна, пъстърва, шаран, попче",
                        FishTypesApp4Zbrdesc = null,
                        FishTypesCrayFish = null,
                        FishingGearDescr = "Мрежа, динамит",
                        IsShipRegistered = false,
                        ShipId = null,
                        ShipName = "Лодка",
                        ShipExternalMark = "няма",
                        ShipCaptainName = "Наско",
                        CoordinationCommittee = "кому е нужен?",
                        CoordinationLetterNo = "123",
                        CoordinationDate = now.AddYears(-1).AddDays(25),
                        IsActive = false,
                        //RequestedByOrganizationId = LegalsData.Legals[3].Id
                    }
                };
            }
        }

        public static List<ScientificPermitReason> ScientificPermitReasons
        {
            get
            {
                return new List<ScientificPermitReason>
                {
                    new ScientificPermitReason
                    {
                        ScientificPermitId = ScientificPermitRegisters[0].Id,
                        ReasonId = NpermitReasons[0].Id
                    },
                    new ScientificPermitReason
                    {
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        ReasonId = NpermitReasons[0].Id
                    },
                    new ScientificPermitReason
                    {
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        ReasonId = NpermitReasons[1].Id
                    },
                    new ScientificPermitReason
                    {
                        ScientificPermitId = ScientificPermitRegisters[2].Id,
                        ReasonId = NpermitReasons[2].Id
                    },
                    new ScientificPermitReason
                    {
                        ScientificPermitId = ScientificPermitRegisters[3].Id,
                        ReasonId = NpermitReasons[0].Id
                    },
                };
            }
        }

        public static List<ScientificPermitOwner> ScientificPermitOwners
        {
            get
            {
                return new List<ScientificPermitOwner>
                {
                    new ScientificPermitOwner
                    {
                        Id = 1,
                        ScientificPermitId = ScientificPermitRegisters[0].Id,
                        OwnerId = PersonsData.Persons[0].Id,
                        RequestedByOrganizationPosition = "Рибар"
                    },
                    new ScientificPermitOwner
                    {
                        Id = 2,
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        OwnerId = PersonsData.Persons[1].Id,
                        RequestedByOrganizationPosition = "Главен изследовател"
                    },
                    new ScientificPermitOwner
                    {
                        Id = 3,
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        OwnerId = PersonsData.Persons[2].Id,
                        RequestedByOrganizationPosition = "Рибар"
                    },
                    new ScientificPermitOwner
                    {
                        Id = 4,
                        ScientificPermitId = ScientificPermitRegisters[2].Id,
                        OwnerId = PersonsData.Persons[3].Id,
                        RequestedByOrganizationPosition = "Философ"
                    },
                    new ScientificPermitOwner
                    {
                        Id = 5,
                        ScientificPermitId = ScientificPermitRegisters[3].Id,
                        OwnerId = PersonsData.Persons[2].Id,
                        RequestedByOrganizationPosition = "Акълодаващ"
                    },
                    new ScientificPermitOwner
                    {
                        Id = 6,
                        ScientificPermitId = ScientificPermitRegisters[3].Id,
                        OwnerId = PersonsData.Persons[3].Id,
                        RequestedByOrganizationPosition = "Работник"
                    }
                };
            }
        }

        public static List<ScientificPermitOuting> ScientificPermitOutings
        {
            get
            {
                return new List<ScientificPermitOuting>
                {
                    new ScientificPermitOuting
                    {
                        Id = 1,
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(20),
                        WaterAreaDesc = "Река Дунав край Русе"
                    },
                    new ScientificPermitOuting
                    {
                        Id = 2,
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(35),
                        WaterAreaDesc = "Река Дунав край Силистра"
                    },
                    new ScientificPermitOuting
                    {
                        Id = 3,
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(45),
                        WaterAreaDesc = "Река Дунав край Тутракан"
                    },
                    new ScientificPermitOuting
                    {
                        Id = 4,
                        ScientificPermitId = ScientificPermitRegisters[1].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(50),
                        WaterAreaDesc = "Река Дунав край Тутракан"
                    },
                    new ScientificPermitOuting
                    {
                        Id = 5,
                        ScientificPermitId = ScientificPermitRegisters[2].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(10),
                        WaterAreaDesc = "Бургаски залив Созопол"
                    },
                    new ScientificPermitOuting
                    {
                        Id = 6,
                        ScientificPermitId = ScientificPermitRegisters[2].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(23),
                        WaterAreaDesc = "Бургаски залив Несебър"
                    },
                    new ScientificPermitOuting
                    {
                        Id = 7,
                        ScientificPermitId = ScientificPermitRegisters[3].Id,
                        OutingDate = ScientificPermitRegisters[1].CoordinationDate.Value.AddDays(55),
                        WaterAreaDesc = "Река Дунав край Силистра"
                    }
                };
            }
        }

        public static List<ScientificPermitOutingCatch> ScientificPermitOutingCatches
        {
            get
            {
                return new List<ScientificPermitOutingCatch>
                {
                    new ScientificPermitOutingCatch
                    {
                        Id = 1,
                        ScientificPermitOutingId = ScientificPermitOutings[0].Id,
                        FishId = FishesData.Nfishes[0].Id,
                        CatchUnder100 = 100,
                        Catch100To500 = 200,
                        Catch500To1000 = 200,
                        CatchOver1000 = 100,
                        TotalKeptCount = 300
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 2,
                        ScientificPermitOutingId = ScientificPermitOutings[0].Id,
                        FishId = FishesData.Nfishes[1].Id,
                        CatchUnder100 = 200,
                        Catch100To500 = 200,
                        Catch500To1000 = 200,
                        CatchOver1000 = 0,
                        TotalKeptCount = 250
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 3,
                        ScientificPermitOutingId = ScientificPermitOutings[1].Id,
                        FishId = FishesData.Nfishes[2].Id,
                        CatchUnder100 = 100,
                        Catch100To500 = 50,
                        Catch500To1000 = 0,
                        CatchOver1000 = 0,
                        TotalKeptCount = 25
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 4,
                        ScientificPermitOutingId = ScientificPermitOutings[2].Id,
                        FishId = FishesData.Nfishes[0].Id,
                        CatchUnder100 = 0,
                        Catch100To500 = 0,
                        Catch500To1000 = 0,
                        CatchOver1000 = 1000,
                        TotalKeptCount = 10
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 5,
                        ScientificPermitOutingId = ScientificPermitOutings[3].Id,
                        FishId = FishesData.Nfishes[0].Id,
                        CatchUnder100 = 10,
                        Catch100To500 = 50,
                        Catch500To1000 = 0,
                        CatchOver1000 = 50,
                        TotalKeptCount = 15
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 6,
                        ScientificPermitOutingId = ScientificPermitOutings[4].Id,
                        FishId = FishesData.Nfishes[1].Id,
                        CatchUnder100 = 10,
                        Catch100To500 = 20,
                        Catch500To1000 = 30,
                        CatchOver1000 = 20,
                        TotalKeptCount = 25
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 7,
                        ScientificPermitOutingId = ScientificPermitOutings[5].Id,
                        FishId = FishesData.Nfishes[1].Id,
                        CatchUnder100 = 5,
                        Catch100To500 = 200,
                        Catch500To1000 = 10,
                        CatchOver1000 = 5,
                        TotalKeptCount = 50
                    },
                    new ScientificPermitOutingCatch
                    {
                        Id = 8,
                        ScientificPermitOutingId = ScientificPermitOutings[6].Id,
                        FishId = FishesData.Nfishes[2].Id,
                        CatchUnder100 = 150,
                        Catch100To500 = 350,
                        Catch500To1000 = 100,
                        CatchOver1000 = 50,
                        TotalKeptCount = 150
                    }
                };
            }
        }
    }
}
