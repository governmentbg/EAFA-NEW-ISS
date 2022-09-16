using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NBuyerTypes
    {
        public static List<NbuyerType> BuyerTypes
        {
            get
            {
                return new List<NbuyerType>
                {
                    new NbuyerType
                    {
                        Id = 1,
                        Code = "Buyer",
                        Name = "Регистриран купувач"
                    },
                    new NbuyerType
                    {
                        Id = 2,
                        Code = "CPP",
                        Name = "Център за първа продажба"
                    },
                };
            }
        }
    }
}
