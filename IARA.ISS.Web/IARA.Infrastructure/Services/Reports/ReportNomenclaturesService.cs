using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Reports;

namespace IARA.Infrastructure.Services.Reports
{
    public class ReportNomenclaturesService : Service, IReportNomenclaturesService
    {
        public ReportNomenclaturesService(IARADbContext dbContext)
            : base(dbContext)
        {

        }

        public List<NomenclatureDTO> GetAvailableReportNParameterNomenclatures()
        {
            DateTime now = DateTime.Now;
            List<NomenclatureDTO> listOfAvailableNParameters = (from nParameter in Db.NReportParameters
                                                                where nParameter.ValidFrom <= now && nParameter.ValidTo >= now
                                                                select new NomenclatureDTO
                                                                {
                                                                    DisplayName = nParameter.Name,
                                                                    Value = nParameter.Id
                                                                }).ToList();
            return listOfAvailableNParameters;

        }

        public List<NomenclatureDTO> GetReportGroupsNomenclatures()
        {
            List<NomenclatureDTO> reportGroups = (from reportGroup in Db.ReportGroups
                                                  where reportGroup.IsActive
                                                  && reportGroup.GroupType != nameof(ReportTypesEnum.Cross)
                                                  select new NomenclatureDTO
                                                  {
                                                      DisplayName = reportGroup.Name,
                                                      Value = reportGroup.Id
                                                  }).ToList();
            return reportGroups;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
