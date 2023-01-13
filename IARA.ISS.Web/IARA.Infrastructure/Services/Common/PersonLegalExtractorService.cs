using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Interfaces.Common;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services.Common
{
    public class PersonLegalExtractorService : Service, IPersonLegalExtractorService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;

        public PersonLegalExtractorService(IARADbContext dbContext,
                                           IPersonService personService,
                                           ILegalService legalService)
            : base(dbContext)
        {
            this.personService = personService;
            this.legalService = legalService;
        }

        public PersonFullDataDTO TryGetPerson(IdentifierTypeEnum identifierType, string identifier)
        {
            List<int> personIds = (from person in Db.Persons
                                   where person.IdentifierType == identifierType.ToString()
                                      && person.EgnLnc == identifier
                                   orderby person.ValidTo descending
                                   select person.Id).ToList();

            if (personIds.Any())
            {
                RegixPersonDataDTO person = personService.GetRegixPersonData(personIds[0]);
                List<AddressRegistrationDTO> addresses = personService.GetAddressRegistrations(personIds[0]);

                FileInfoDTO pic = (from personFile in Db.PersonFiles
                                   join per in Db.Persons on personFile.RecordId equals per.Id
                                   join photo in Db.Files on personFile.FileId equals photo.Id
                                   join fileType in Db.NfileTypes on personFile.FileTypeId equals fileType.Id
                                   where personIds.Contains(per.Id)
                                        && fileType.Code == nameof(FileTypeEnum.PHOTO)
                                        && personFile.IsActive
                                        && photo.IsActive
                                   orderby per.ValidTo descending
                                   select new FileInfoDTO
                                   {
                                       Id = photo.Id
                                   }).FirstOrDefault();

                PersonFullDataDTO result = new()
                {
                    Person = person,
                    Addresses = addresses,
                    Photo = pic
                };

                return result;
            }

            return null;
        }

        public LegalFullDataDTO TryGetLegal(string eik)
        {
            int? legalId = (from legal in Db.Legals
                            where legal.Eik == eik
                            orderby legal.ValidTo descending
                            select (int?)legal.Id).FirstOrDefault();

            if (legalId.HasValue)
            {
                RegixLegalDataDTO legal = legalService.GetRegixLegalData(legalId.Value);
                List<AddressRegistrationDTO> addresses = legalService.GetAddressRegistrations(legalId.Value);

                LegalFullDataDTO result = new()
                {
                    Legal = legal,
                    Addresses = addresses
                };

                return result;
            }

            return null;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
