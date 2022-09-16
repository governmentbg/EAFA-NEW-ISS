using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class RecreationalFishingNomenclaturesService : Service, IRecreationalFishingNomenclaturesService
    {
        public RecreationalFishingNomenclaturesService(IARADbContext db)
            : base(db)
        { }

        public List<NomenclatureDTO> GetTicketPeriods()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> periods = (from period in Db.NticketPeriods
                                             orderby period.OrderNo
                                             select new NomenclatureDTO
                                             {
                                                 Value = period.Id,
                                                 Code = period.Code,
                                                 DisplayName = period.Name,
                                                 IsActive = period.ValidFrom <= now && period.ValidTo > now
                                             }).ToList();
            return periods;
        }

        public List<NomenclatureDTO> GetTicketTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> types = (from type in Db.NticketTypes
                                           orderby type.OrderNo
                                           select new NomenclatureDTO
                                           {
                                               Value = type.Id,
                                               Code = type.Code,
                                               DisplayName = type.Name,
                                               IsActive = type.ValidFrom <= now && type.ValidTo > now
                                           }).ToList();
            return types;
        }

        public List<RecreationalFishingTicketPriceDTO> GetTicketPrices()
        {
            DateTime now = DateTime.Now;

            List<RecreationalFishingTicketPriceDTO> prices = (from tariff in Db.Ntariffs
                                                              where tariff.Code.StartsWith("Ticket_")
                                                                    && tariff.Code != nameof(TariffCodesEnum.Ticket_duplicate)
                                                                    && tariff.ValidFrom <= now
                                                                    && tariff.ValidTo > now
                                                              select new RecreationalFishingTicketPriceDTO
                                                              {
                                                                  TicketType = ExtractTicketTypeFromTariffCode(tariff.Code),
                                                                  TicketPeriod = ExtractTicketPeriodFromTariffCode(tariff.Code),
                                                                  Price = tariff.Price
                                                              }).ToList();
            return prices;
        }

        public List<NomenclatureDTO> GetAllFishingAssociations()
        {
            List<NomenclatureDTO> result = (from assoc in Db.FishingAssociations
                                            join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                                            where assoc.IsActive
                                            orderby legal.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = assoc.Id,
                                                DisplayName = legal.Name
                                            }).ToList();
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private static TicketTypeEnum ExtractTicketTypeFromTariffCode(string code)
        {
            if (code.Contains("standard"))
            {
                return TicketTypeEnum.STANDARD;
            }

            if (code.Contains("elderassociation"))
            {
                return TicketTypeEnum.ELDERASSOCIATION;
            }

            if (code.Contains("between14and18association"))
            {
                return TicketTypeEnum.BETWEEN14AND18ASSOCIATION;
            }

            if (code.Contains("elder"))
            {
                return TicketTypeEnum.ELDER;
            }

            if (code.Contains("between14and18"))
            {
                return TicketTypeEnum.BETWEEN14AND18;
            }

            if (code.Contains("association"))
            {
                return TicketTypeEnum.ASSOCIATION;
            }

            if (code.Contains("under14"))
            {
                return TicketTypeEnum.UNDER14;
            }

            if (code.Contains("disability"))
            {
                return TicketTypeEnum.DISABILITY;
            }

            throw new ArgumentException("Unexpected ticket tariff: " + code);
        }

        private static TicketPeriodEnum ExtractTicketPeriodFromTariffCode(string code)
        {
            if (code.Contains("weekly"))
            {
                return TicketPeriodEnum.WEEKLY;
            }

            if (code.Contains("monthly"))
            {
                return TicketPeriodEnum.MONTHLY;
            }

            if (code.Contains("halfyearly"))
            {
                return TicketPeriodEnum.HALFYEARLY;
            }

            if (code.Contains("annual"))
            {
                return TicketPeriodEnum.ANNUAL;
            }

            if (code.Contains("noperiod"))
            {
                return TicketPeriodEnum.NOPERIOD;
            }

            if (code.Contains("until14"))
            {
                return TicketPeriodEnum.UNTIL14;
            }

            if (code.Contains("disability"))
            {
                return TicketPeriodEnum.DISABILITY;
            }

            throw new ArgumentException("Unexpected ticket tariff: " + code);
        }
    }
}
