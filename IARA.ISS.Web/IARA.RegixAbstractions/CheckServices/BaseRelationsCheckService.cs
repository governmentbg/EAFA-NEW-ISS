using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.RelationsSearch;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BaseRelationsCheckService : BaseRegiXCheckService<RelationsRequestType, RelationsResponseType, List<RegixPersonDataDTO>>
    {
        protected BaseRelationsCheckService(ConnectionStrings connectionString,
                                            IRegiXClientService regixService,
                                            IRegixConclusionsService regixConclusionsService,
                                            ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.RelationsSearch),
                   Operations.RELATIONS_SEARCH)
        { }


        protected abstract override Task<ResponseType<RelationsResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> data);


        protected override RegixCheckStatus CompareApplicationData(List<RegixPersonDataDTO> response, List<RegixPersonDataDTO> compare, BaseContextData context)
        {
            if (compare != null && compare.Any() && response != null && response.Any())
            {
                var representativePerson = compare.First();
                bool result = response.Where(x => x.EgnLnc.EgnLnc == representativePerson.EgnLnc.EgnLnc).Any();

                if (result)
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
                }
            }

            return new RegixCheckStatus(RegixCheckStatusesEnum.ERROR, ErrorResources.msgRepresentativeMissing);
        }

        protected override List<RegixPersonDataDTO> MapToLocalData(RelationsResponseType response, RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> requestContext)
        {
            List<RegixPersonDataDTO> relations = new List<RegixPersonDataDTO>();

            if (response != null && response.PersonRelations != null && response.PersonRelations.Length > 0)
            {
                var personRelations = response.PersonRelations.Where(x => x.RelationCode == RelationType.Баща
                                                                       || x.RelationCode == RelationType.Майка
                                                                       || x.RelationCode == RelationType.Осиновител
                                                                       || x.RelationCode == RelationType.Осиновителка);
                foreach (var person in personRelations)
                {
                    relations.Add(new RegixPersonDataDTO
                    {
                        FirstName = person.FirstName,
                        BirthDate = person.BirthDate,
                        LastName = person.FamilyName,
                        MiddleName = person.SurName,
                        CitizenshipCountryName = person.Nationality?.NationalityName,
                        GenderName = person?.Gender?.GenderName.ToString()
                    });
                }
            }

            return relations;
        }
    }
}
