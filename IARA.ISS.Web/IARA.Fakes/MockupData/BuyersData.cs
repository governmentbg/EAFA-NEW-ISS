using System;
using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class BuyersData
    {
        public static List<BuyerRegister> Buyers
        {
            get
            {
                return new List<BuyerRegister>
                {
                    new BuyerRegister
                    {
                        ApplicationId = 1,
                        BuyerTypeId = 2,
                        Comments = "First test comment",
                        IsActive = true,
                        SubmittedForLegalId = 1,
                        OrganizingPersonId = 2,
                        RecordType = "Register",
                        RegisterApplicationId   = null,
                        RegistrationDate = new DateTime(2021,1,1),
                        RegistrationNum = "02000001",
                        UtilityName = "TestName"
                    }
                };
            }
        }
    }
}
