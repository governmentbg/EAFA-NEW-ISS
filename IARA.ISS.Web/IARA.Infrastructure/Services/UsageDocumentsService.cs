using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services
{
    public class UsageDocumentsService : Service, IUsageDocumentsService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;

        public UsageDocumentsService(IARADbContext db,
                                     IPersonService personService,
                                     ILegalService legalService)
            : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
        }

        public UsageDocumentDTO GetUsageDocument(int id)
        {
            UsageDocumentDTO document = GetUsageDocumentHelper(id);
            return document;
        }

        public List<UsageDocumentDTO> GetUsageDocuments(List<int> ids)
        {
            List<UsageDocumentHelper> documents = GetUsageDocumentHelpers(ids);

            List<int> personIds = documents.Where(x => x.IsLessorPerson == true).Select(x => x.LessorPersonId.Value).ToList();
            List<int> legalIds = documents.Where(x => x.IsLessorPerson == false).Select(x => x.LessorLegalId.Value).ToList();

            Dictionary<int, RegixPersonDataDTO> persons = personService.GetRegixPersonsData(personIds);
            Dictionary<int, RegixLegalDataDTO> legals = legalService.GetRegixLegalsData(personIds);
            ILookup<int, AddressRegistrationDTO> personAddresses = personService.GetAddressRegistrations(personIds);
            ILookup<int, AddressRegistrationDTO> legalAddresses = legalService.GetAddressRegistrations(legalIds);

            List<UsageDocumentDTO> result = new List<UsageDocumentDTO>();

            foreach (UsageDocumentHelper document in documents)
            {
                LoadUsageDocumentLessor(document);
                result.Add(document);
            }

            return result;
        }

        public UsageDocumentRegixDataDTO GetUsageDocumentRegixData(int id)
        {
            UsageDocumentRegixDataDTO document = GetUsageDocumentRegixHelper(id);
            return document;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.UsageDocuments, id);
        }

        private UsageDocumentHelper GetUsageDocumentHelper(int id)
        {
            UsageDocumentHelper document = (from doc in Db.UsageDocuments
                                            where doc.Id == id
                                            select new UsageDocumentHelper
                                            {
                                                Id = doc.Id,
                                                DocumentTypeId = doc.DocumentTypeId,
                                                DocumentNum = doc.DocumentNum,
                                                IsDocumentIndefinite = doc.DocumentValidTo == null,
                                                DocumentValidFrom = doc.DocumentValidFrom,
                                                DocumentValidTo = doc.DocumentValidTo,
                                                IsLessorPerson = doc.IsLessorPerson,
                                                LessorPersonId = doc.LessorPersonId,
                                                LessorLegalId = doc.LessorLegalId,
                                                Comments = doc.Comments,
                                                IsActive = doc.IsActive,
                                            }).First();

            LoadUsageDocumentLessor(document);

            return document;
        }

        private UsageDocumentRegixHelper GetUsageDocumentRegixHelper(int id)
        {
            UsageDocumentRegixHelper document = (from doc in Db.UsageDocuments
                                                 where doc.Id == id
                                                 select new UsageDocumentRegixHelper
                                                 {
                                                     Id = doc.Id,
                                                     DocumentTypeId = doc.DocumentTypeId,
                                                     IsLessorPerson = doc.IsLessorPerson,
                                                     LessorPersonId = doc.LessorPersonId,
                                                     LessorLegalId = doc.LessorLegalId,
                                                     IsActive = doc.IsActive
                                                 }).First();

            LoadUsageDocumentLessor(document);

            return document;
        }

        private List<UsageDocumentHelper> GetUsageDocumentHelpers(List<int> ids)
        {
            List<UsageDocumentHelper> result = (from doc in Db.UsageDocuments
                                                where ids.Contains(doc.Id)
                                                orderby doc.DocumentValidTo descending
                                                select new UsageDocumentHelper
                                                {
                                                    Id = doc.Id,
                                                    DocumentTypeId = doc.DocumentTypeId,
                                                    DocumentNum = doc.DocumentNum,
                                                    IsDocumentIndefinite = doc.DocumentValidTo == null,
                                                    DocumentValidFrom = doc.DocumentValidFrom,
                                                    DocumentValidTo = doc.DocumentValidTo,
                                                    IsLessorPerson = doc.IsLessorPerson,
                                                    LessorPersonId = doc.LessorPersonId,
                                                    LessorLegalId = doc.LessorLegalId,
                                                    Comments = doc.Comments,
                                                    IsActive = doc.IsActive
                                                }).ToList();

            return result;
        }

        private void LoadUsageDocumentLessor(IUsageDocumentHelper document)
        {
            if (document.IsLessorPerson.HasValue)
            {
                if (document.IsLessorPerson.Value)
                {
                    document.LessorPerson = personService.GetRegixPersonData(document.LessorPersonId.Value);
                    document.LessorAddresses = personService.GetAddressRegistrations(document.LessorPersonId.Value);
                }
                else
                {
                    document.LessorLegal = legalService.GetRegixLegalData(document.LessorLegalId.Value);
                    document.LessorAddresses = legalService.GetAddressRegistrations(document.LessorLegalId.Value);
                }
            }
        }
    }

    internal interface IUsageDocumentHelper
    {
        public bool? IsLessorPerson { get; set; }
        public int? LessorPersonId { get; set; }
        public int? LessorLegalId { get; set; }
        public RegixPersonDataDTO LessorPerson { get; set; }
        public RegixLegalDataDTO LessorLegal { get; set; }
        public List<AddressRegistrationDTO> LessorAddresses { get; set; }
    }

    internal class UsageDocumentHelper : UsageDocumentDTO, IUsageDocumentHelper
    {
        public int? LessorPersonId { get; set; }
        public int? LessorLegalId { get; set; }
    }

    internal class UsageDocumentRegixHelper : UsageDocumentRegixDataDTO, IUsageDocumentHelper
    {
        public int? LessorPersonId { get; set; }
        public int? LessorLegalId { get; set; }
    }
}
