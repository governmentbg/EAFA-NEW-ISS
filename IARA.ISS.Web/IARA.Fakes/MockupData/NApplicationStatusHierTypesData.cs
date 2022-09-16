using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NApplicationStatusHierTypesData
    {
        public static List<NapplicationStatusHierarchyType> ApplicationStatusHierTypesData
        {
            get
            {
                return new List<NapplicationStatusHierarchyType>
                {
                    new NapplicationStatusHierarchyType
                    {
                        Id = 1,
                        Code = "Online",
                        Name = "Онлайн подадено заявление",
                    },
                    new NapplicationStatusHierarchyType
                    {
                        Id = 2,
                        Code = "OnPaper",
                        Name = "Подадено на хартия заявление",
                    }
                };
            }
        }
    }
}
