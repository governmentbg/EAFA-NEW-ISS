using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NTerritoryUnitsData
    {
        public static List<NterritoryUnit> TerritoryUnits
        {
            get
            {
                return new List<NterritoryUnit>
                {
                    new NterritoryUnit
                    {
                        Id = 1,
                        Name = "Рибарство и контрол – Черно море",
                        ValidFrom = new DateTime(2010, 5, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        Code =  "A"
                    },
                    new NterritoryUnit
                    {
                        Id = 2,
                        Name = "Рибарство и контрол – Централен Дунав",
                        ValidFrom = new DateTime(2010, 5, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        Code =  "B"
                    },
                    new NterritoryUnit
                    {
                        Id = 3,
                        Name = "Рибарство и контрол – Западна България",
                        ValidFrom = new DateTime(2010, 5, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        Code =  "C"
                    },
                    new NterritoryUnit
                    {
                        Id = 4,
                        Name = "Рибарство и контрол – Южна България",
                        ValidFrom = new DateTime(2010, 5, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        Code =  "D"
                    },
                };
            }
        }
    }
}
