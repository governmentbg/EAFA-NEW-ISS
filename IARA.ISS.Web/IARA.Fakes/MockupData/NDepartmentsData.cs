using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class NDepartmentsData
    {
        public static List<Ndepartment> Departments
        {
            get
            {
                return new List<Ndepartment>
                {
                    new Ndepartment { Id = 1, Name = "Рибарство и контрол", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE },
                    new Ndepartment { Id = 2, Name = "Център за наблюдение на риболова", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE },
                    new Ndepartment { Id = 3, Name = "Рибарство и контрол – Черно море", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE },
                    new Ndepartment { Id = 4, Name = "Структурни фондове по рибарство", ValidFrom = new DateTime(2010, 5, 1), ValidTo = DefaultConstants.MAX_VALID_DATE }
                };
            }
        }
    }
}
