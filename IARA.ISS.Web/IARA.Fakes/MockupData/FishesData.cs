using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class FishesData
    {
        public static List<NfishFamily> NfishFamilies
        {
            get
            {
                return new List<NfishFamily>
                {
                };
            }
        }

        public static List<NfishGroup> NfishGroups
        {
            get
            {
                return new List<NfishGroup>
                {
                };
            }
        }

        public static List<Nfish> Nfishes
        {
            get
            {
                return new List<Nfish>
                {
                    new Nfish
                    {
                        Id = 1,
                        Code = "SH",
                        Name = "Шаран"
                    },
                    new Nfish
                    {
                        Id = 2,
                        Code = "SK",
                        Name = "Скумрия"
                    },
                    new Nfish
                    {
                        Id = 3,
                        Code = "SM",
                        Name = "Сьомга"
                    }
                };
            }
        }
    }
}
