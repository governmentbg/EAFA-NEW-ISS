using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.StatisticalForms
{
    public class StatisticalFormDTO
    {
        public int Id { get; set; }

        public string RegistryNumber { get; set; }

        public string ProcessUser { get; set; }

        public int Year { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string FormObject { get; set; }

        public string FormTypeName { get; set; }

        public StatisticalFormTypesEnum FormType { get; set; }

        public bool IsActive { get; set; }
    }
}
