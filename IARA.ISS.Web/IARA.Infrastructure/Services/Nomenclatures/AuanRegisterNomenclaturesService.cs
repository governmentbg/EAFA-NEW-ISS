using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using IARA.Common.Enums;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class AuanRegisterNomenclaturesService : BaseService, IAuanRegisterNomenclaturesService
    {
        private static readonly string[] RelevantInspectionTypes = new string[]
        {
            nameof(InspectionTypesEnum.OTH),
            nameof(InspectionTypesEnum.OFS),
            nameof(InspectionTypesEnum.IBS),
            nameof(InspectionTypesEnum.IBP),
            nameof(InspectionTypesEnum.ITB),
            nameof(InspectionTypesEnum.IVH),
            nameof(InspectionTypesEnum.IFS),
            nameof(InspectionTypesEnum.IAQ),
            nameof(InspectionTypesEnum.IFP),
            nameof(InspectionTypesEnum.CWO),
            nameof(InspectionTypesEnum.IGM)
        };

        public AuanRegisterNomenclaturesService(IARADbContext db)
            : base(db)
        { }

        public List<NomenclatureDTO> GetAllInspectionReports()
        {
            List<NomenclatureDTO> reports = (from inspection in Db.InspectionsRegister
                                             join type in Db.NinspectionTypes on inspection.InspectionTypeId equals type.Id
                                             join state in Db.NinspectionStates on inspection.StateId equals state.Id
                                             where state.Code == nameof(InspectionStatesEnum.Submitted)
                                                && RelevantInspectionTypes.Contains(type.Code)
                                             orderby inspection.ReportNum
                                             select new NomenclatureDTO
                                             {
                                                 Value = inspection.Id,
                                                 DisplayName = inspection.ReportNum,
                                                 IsActive = inspection.IsActive
                                             }).ToList();

            return reports;
        }

        public List<AuanConfiscationActionsNomenclatureDTO> GetConfiscationActions()
        {
            DateTime now = DateTime.Now;

            var actions = (from action in Db.NconfiscationActions
                           orderby action.Name
                           select new AuanConfiscationActionsNomenclatureDTO
                           {
                               Value = action.Id,
                               DisplayName = action.Name,
                               ActionGroup = Enum.Parse<InspConfiscationActionGroupsEnum>(action.ActionGroup),
                               IsActive = action.ValidFrom <= now && action.ValidTo > now
                           }).ToList();

            return actions;
        }

        public List<InspDeliveryTypesNomenclatureDTO> GetInspDeliveryTypes()
        {
            DateTime now = DateTime.Now;

            List<InspDeliveryTypesNomenclatureDTO> result = (from type in Db.NinspDeliveryTypes
                                                             orderby type.OrderNo
                                                             select new InspDeliveryTypesNomenclatureDTO
                                                             {
                                                                 Value = type.Id,
                                                                 DisplayName = type.Name,
                                                                 Code = type.Code,
                                                                 Group = Enum.Parse<InspDeliveryTypeGroupsEnum>(type.Group),
                                                                 IsActive = type.ValidFrom <= now && type.ValidTo > now
                                                             }).ToList();

            return result;
        }

        public List<InspDeliveryTypesNomenclatureDTO> GetInspDeliveryConfirmationTypes()
        {
            DateTime now = DateTime.Now;

            List<InspDeliveryTypesNomenclatureDTO> result = (from type in Db.NinspDeliveryConfirmationTypes
                                                             orderby type.OrderNo
                                                             select new InspDeliveryTypesNomenclatureDTO
                                                             {
                                                                 Value = type.Id,
                                                                 DisplayName = type.Name,
                                                                 Code = type.Code,
                                                                 Group = Enum.Parse<InspDeliveryTypeGroupsEnum>(type.Group),
                                                                 IsActive = type.ValidFrom <= now && type.ValidTo > now
                                                             }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetAuanStatuses()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.Nauanstatuses);
            return result;
        }

        public List<NomenclatureDTO> GetConfiscatedAppliances()
        {
            List<NomenclatureDTO> result = GetNomenclature(Db.NconfiscatedAppliances);
            return result;
        }
    }
}
