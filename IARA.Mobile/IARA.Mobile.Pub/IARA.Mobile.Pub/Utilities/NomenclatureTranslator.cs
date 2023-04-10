using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Domain.Enums;
using System;
using System.Collections.Generic;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Pub.Utilities
{
    public static class NomenclatureTranslator
    {
        public static List<FishingTicketDto> UpdateTicketTranslation(List<FishingTicketDto> tickets, IFishingTicketsTransaction FishingTicketsTransaction)
        {
            var ticketTypes = FishingTicketsTransaction.GetTicketTypes();

            foreach (var ticket in tickets)
            {
                string typeCode = ticketTypes.Find(x => x.Id == ticket.TypeId)?.Code;
                ticket.PeriodName = NomenclatureTranslator.MapTicketPeriodTranslation(ticket.PeriodCode);
                ticket.TypeName = NomenclatureTranslator.MapTicketTypeTranslation(typeCode);
            }
            return tickets;
        }
        public static void MapTicketTypeTranslation(this TicketTypeDto nomenclature)
        {
            if (Enum.TryParse(nomenclature.Code, out TicketTypeEnum type))
            {
                nomenclature.Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + NomenclatureTranslator.GetTicketTypeTranslationCode(type)];
            }
        }

        public static string MapTicketTypeTranslation(string code)
        {
            if (Enum.TryParse(code, out TicketTypeEnum type))
            {
                return TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + NomenclatureTranslator.GetTicketTypeTranslationCode(type)];
            }

            return string.Empty;
        }

        public static void MapTicketPeriodTranslation(this TicketPeriodDto nomenclature)
        {
            if (Enum.TryParse(nomenclature.Code, out TicketPeriodEnum type))
            {
                nomenclature.Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + NomenclatureTranslator.GetTicketPeriodTranslationCode(type)];
            }
        }

        public static string MapTicketPeriodTranslation(string code)
        {
            if (Enum.TryParse(code, out TicketPeriodEnum type))
            {
                return TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + NomenclatureTranslator.GetTicketPeriodTranslationCode(type)];
            }

            return string.Empty;
        }

        public static string GetTicketTypeTranslationCode(TicketTypeEnum type)
        {
            switch (type)
            {
                case TicketTypeEnum.STANDARD:
                    return "/ticket-type-standard-resource";
                case TicketTypeEnum.UNDER14:
                    return "/ticket-type-under14-resource";
                case TicketTypeEnum.BETWEEN14AND18:
                    return "/ticket-type-between14and18-resource";
                case TicketTypeEnum.DISABILITY:
                    return "/ticket-type-disability-resource";
                case TicketTypeEnum.ASSOCIATION:
                    return "/ticket-type-association-resource";
                case TicketTypeEnum.BETWEEN14AND18ASSOCIATION:
                    return "/ticket-type-between14and18association-resource";
                case TicketTypeEnum.ELDER:
                    return "/ticket-type-elder-resource";
                case TicketTypeEnum.ELDERASSOCIATION:
                    return "/ticket-type-elderassociation-resource";
                default:
                    return null;
            }
        }

        private static string GetTicketPeriodTranslationCode(TicketPeriodEnum type)
        {
            switch (type)
            {
                case TicketPeriodEnum.ANNUAL:
                    return "/ticket-period-annual-resource";
                case TicketPeriodEnum.HALFYEARLY:
                    return "/ticket-period-halfyearly-resource";
                case TicketPeriodEnum.MONTHLY:
                    return "/ticket-period-monthly-resource";
                case TicketPeriodEnum.WEEKLY:
                    return "/ticket-period-weekly-resource";
                case TicketPeriodEnum.UNTIL14:
                    return "/ticket-period-until14-resource";
                case TicketPeriodEnum.DISABILITY:
                    return "/ticket-period-disability-resource";
                case TicketPeriodEnum.NOPERIOD:
                    return "/ticket-period-noperiod-resource";
                default:
                    return null;
            }
        }
    }
}
