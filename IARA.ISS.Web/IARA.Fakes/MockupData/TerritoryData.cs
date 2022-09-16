using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class TerritoryData
    {
        public static List<Nsector> Sectors
        {
            get
            {
                return new List<Nsector>
                {
                    new Nsector
                    {
                        Id=1,
                        Name = "Тестов Сектор 1",
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Nsector
                    {
                        Id=2,
                        Name = "Тестов Сектор 2",
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                };
            }
        }

        public static List<NterritoryUnit> TerritoryUnits
        {
            get
            {
                return new List<NterritoryUnit>
                {
                    new NterritoryUnit
                    {
                        Id = 1,
                        Name = "Тестова Територия 1",
                        Code = "A",
                        SectorId = 1,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new NterritoryUnit
                    {
                        Id = 2,
                        Name = "Тестова Територия 12",
                        Code = "B",
                        SectorId = 1,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new NterritoryUnit
                    {
                        Id = 3,
                        Name = "Тестова Територия 3",
                        Code = "C",
                        SectorId = 2,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                };
            }
        }
    }
}
