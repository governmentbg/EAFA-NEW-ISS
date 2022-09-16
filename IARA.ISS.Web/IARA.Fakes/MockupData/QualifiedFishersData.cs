using System;
using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class QualifiedFishersData
    {
        public static List<FishermenRegister> Fishers
        {
            get
            {
                return new List<FishermenRegister>
                {
                    new FishermenRegister
                    {
                        Id = 1,
                        PersonId = 1,
                        RegistrationDate = new DateTime(2021,4,25),
                        HasExamLicense = true,
                        ExamProtocolNum = "55643251",
                        ExamDate = new DateTime(2021,3,3),
                        DiplomaNum = null,
                        DiplomaGraduationDate = null,
                        RecordType = "Register",
                        IsActive = true
                    },
                    new FishermenRegister
                    {
                        Id = 2,
                        PersonId = 6,
                        RegistrationDate = new DateTime(1980, 11, 22),
                        HasExamLicense = false,
                        ExamProtocolNum = null,
                        ExamDate = null,
                        DiplomaNum = "7306290672839",
                        DiplomaGraduationDate = new DateTime(1980,7,1),
                        RecordType = "Register",
                        IsActive = true
                    },
                    new FishermenRegister
                    {
                        Id = 3,
                        PersonId = 3,
                        RegistrationDate = new DateTime(2020,12,31),
                        HasExamLicense = false,
                        ExamProtocolNum = null,
                        ExamDate = null,
                        DiplomaNum = "testestest",
                        DiplomaGraduationDate = new DateTime(2011,9,13),
                        RecordType = "Register",
                        IsActive = false
                    },
                };
            }
        }
    }
}
