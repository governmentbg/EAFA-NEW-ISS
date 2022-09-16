using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using IARA.Common.Enums;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class PenalDecreesNomenclaturesService : BaseService, IPenalDecreesNomenclaturesService
    {
        public PenalDecreesNomenclaturesService(IARADbContext db)
            : base(db)
        { }

        public List<NomenclatureDTO> GetAllAuans()
        {
            List<NomenclatureDTO> auans = (from auan in Db.AuanRegister
                                           orderby auan.AuanNum
                                           select new NomenclatureDTO
                                           {
                                               Value = auan.Id,
                                               DisplayName = auan.AuanNum,
                                               IsActive = auan.IsActive
                                           }).ToList();

            return auans;
        }

        public List<NomenclatureDTO> GetDecreeInspDeliveryTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from type in Db.NinspDeliveryTypes
                                            where type.Group == nameof(InspDeliveryTypeGroupsEnum.PD)
                                            orderby type.OrderNo
                                            select new NomenclatureDTO
                                            {
                                                Value = type.Id,
                                                DisplayName = type.Name,
                                                Code = type.Code,
                                                IsActive = type.ValidFrom <= now && type.ValidTo > now
                                            }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetPenalDecreeStatusTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NpenalDecreeStatusTypes);
            return result;
        }

        public List<NomenclatureDTO> GetPenalDecreeAuthorityTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NpenalAuthorityTypes);
            return result;
        }

        public List<NomenclatureDTO> GetCourts()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.Ncourts);
            return result;
        }

        public List<NomenclatureDTO> GetPenalDecreeTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NpenalDecreeTypes);
            return result;
        }

        public List<NomenclatureDTO> GetPenalDecreeSanctionTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NpenalDecreeSanctionTypes);
            return result;
        }

        public List<NomenclatureDTO> GetConfiscationInstitutions()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NconfiscationInstitutions);
            return result;
        }
    }
}
