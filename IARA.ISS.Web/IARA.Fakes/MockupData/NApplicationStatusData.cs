using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NApplicationStatusData
    {
        public static List<NapplicationStatus> ApplicationStatuses
        {
            get
            {
                return new List<NapplicationStatus>
                {
                    new NapplicationStatus
                    {
                        Id = 1,
                        Code = "INITIAL",
                        Name = "Начално състояние",
                    }
                };
            }
        }
    }
}
