using System;
using System.Collections.Generic;
using System.Text;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Reports
{
    public interface IReportNomenclaturesService
    {
        List<NomenclatureDTO> GetAvailableReportNParameterNomenclatures();
        List<NomenclatureDTO> GetReportGroupsNomenclatures();
    }
}
