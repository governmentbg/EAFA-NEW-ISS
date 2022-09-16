using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class ApplicationsData
    {
        public static List<Application> Applications
        {
            get
            {
                return new List<Application>
                {
                    new Application
                    {
                        Id = 1,
                        ApplicationTypeId = 39,
                        ApplicationStatusHierTypeId = 1,
                        ApplicationStatusId = 1,
                        PaymentStatusId = NpaymentStatuses[0].Id,
                        SubmitDateTime = new DateTime(2020,1,1),
                        SubmittedByUserId = 1,
                    }
                };
            }
        }

        public static List<NpaymentStatus> NpaymentStatuses
        {
            get
            {
                return new List<NpaymentStatus>
                {
                    new NpaymentStatus
                    {
                        Id = 1,
                        Code = "NotNeeded",
                        Name = "Не се изисква",
                        ValidFrom = DateTime.Now,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        OrderNum = 1,
                        CreatedBy = "SYSTEM",
                        CreatedOn = DateTime.Now
                    },
                    new NpaymentStatus
                    {
                        Id = 2,
                        Code = "Unpaid",
                        Name = "Неплатено",
                        ValidFrom = DateTime.Now,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        OrderNum = 2,
                        CreatedBy = "SYSTEM",
                        CreatedOn = DateTime.Now
                    },
                    new NpaymentStatus
                    {
                        Id = 3,
                        Code = "PaymentFail",
                        Name = "Неуспешно",
                        ValidFrom = DateTime.Now,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        OrderNum = 3,
                        CreatedBy = "SYSTEM",
                        CreatedOn = DateTime.Now
                    },
                    new NpaymentStatus
                    {
                        Id = 4,
                        Code = "PaidOK",
                        Name = "Платено",
                        ValidFrom = DateTime.Now,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        OrderNum = 4,
                        CreatedBy = "SYSTEM",
                        CreatedOn = DateTime.Now
                    }
                };
            }
        }
    }
}
