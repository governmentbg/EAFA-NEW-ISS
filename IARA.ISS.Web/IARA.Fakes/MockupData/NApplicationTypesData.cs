using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NApplicationTypesData
    {
        public static List<NapplicationType> ApplicationTypes
        {
            get
            {
                return new List<NapplicationType>
                {
                    new NapplicationType
                    {
                        Id = 39,
                        Code="a-2354-1",
                        Name = "№2354-1 - Участие в курс и изпит за правоспособност",
                        IsPaid = true,
                        IsEas = true,
                    },
                    new NapplicationType
                    {
                        Id = 51,
                        Code = "a-2354-2",
                        Name="№2354-2 - Издаване на дубликат за свидетелство за правоспособност",
                        IsPaid=true,
                        IsEas = true,
                    }
                };
            }
        }
    }
}
