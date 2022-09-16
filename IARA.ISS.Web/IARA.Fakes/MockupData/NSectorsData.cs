using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NSectorsData
    {
        public static List<Nsector> Sectors
        {
            get
            {
                return new List<Nsector>
                {
                    new Nsector {Id = 1, Name = "“Рибарство и контрол” гр. София", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE},
                    new Nsector {Id = 2, Name = "“Рибарство и контрол” Пловдив", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE},
                    new Nsector {Id = 3, Name = "“Рибарство и контрол” Благоевград", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE},
                    new Nsector {Id = 4, Name = "“Рибарство и контрол” Стара Загора", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE},
                    new Nsector {Id = 5, Name = "“Рибарство и контрол” Бургас", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE},
                    new Nsector {Id = 6, Name = "“Рибарство и контрол” Варна", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE},
                    new Nsector {Id = 7, Name = "“Рибарство и контрол” Русе", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE}
                };
            }
        }
    }
}
