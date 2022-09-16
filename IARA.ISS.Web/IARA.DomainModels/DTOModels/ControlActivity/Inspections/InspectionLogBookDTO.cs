using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionLogBookDTO
    {
        public int? Id { get; set; }
        public InspectionToggleTypesEnum? CheckValue { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public string PageNum { get; set; }
        public int? LogBookId { get; set; }
        public int? PageId { get; set; }
        public DateTime? From { get; set; }
        public long? StartPage { get; set; }
        public long? EndPage { get; set; }

        /// <summary>
        /// Used only for the inline grid
        /// </summary>
        public List<NomenclatureDTO> Pages { get; set; }
    }
}
