using System;
using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class PoundnetRegisterData
    {
        public static List<PoundNetRegister> PoundnetRegister
        {
            get
            {
                return new List<PoundNetRegister>
                {
                   new PoundNetRegister { Id = 1, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = true, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[0].Id, CategoryTypeId = PoundnetCategoryTypes[0].Id },
                   new PoundNetRegister { Id = 2, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = true, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[1].Id, CategoryTypeId = PoundnetCategoryTypes[1].Id },
                   new PoundNetRegister { Id = 3, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = false, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[2].Id, CategoryTypeId = PoundnetCategoryTypes[2].Id },
                   new PoundNetRegister { Id = 4, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = true, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[0].Id, CategoryTypeId = PoundnetCategoryTypes[3].Id },
                   new PoundNetRegister { Id = 5, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = true, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[1].Id, CategoryTypeId = PoundnetCategoryTypes[0].Id },
                   new PoundNetRegister { Id = 6, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = true, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[2].Id, CategoryTypeId = PoundnetCategoryTypes[1].Id },
                   new PoundNetRegister { Id = 7, Name = "gaegaeg", PoundNetNum = "tegtwre", IsActive = true, RegistrationDate = DateTime.Now, SeasonTypeId = PoundnetSeasonTypes[0].Id, CategoryTypeId = PoundnetCategoryTypes[2].Id },
                };
            }
        }

        public static List<NpoundNetCategoryType> PoundnetCategoryTypes
        {
            get
            {
                return new List<NpoundNetCategoryType>
                {
                     new NpoundNetCategoryType { Id = 1, Name = "Първа", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  },
                     new NpoundNetCategoryType { Id = 2, Name = "Втора", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  },
                     new NpoundNetCategoryType { Id = 3,  Name = "Друга", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  },
                     new NpoundNetCategoryType { Id = 4,  Name = "Няма категория", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  }
                };
            }
        }

        public static List<NpoundNetSeasonType> PoundnetSeasonTypes
        {
            get
            {
                return new List<NpoundNetSeasonType>
                {
                      new NpoundNetSeasonType { Id = 1,  Name = "Целогодишен", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  },
                      new NpoundNetSeasonType { Id = 2,  Name = "Пролетен", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  },
                      new NpoundNetSeasonType { Id = 3,  Name = "Есенен", CreatedBy = "SYSTEM", CreatedOn = DateTime.Now  }
                };
            }
        }
    }
}
