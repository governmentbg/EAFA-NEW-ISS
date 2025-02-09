﻿using System;

namespace IARA.DomainModels.DTOModels.FishingTickets
{
    public class FishingTicketDTO
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public int PeriodId { get; set; }
        public string PeriodCode { get; set; }
        public string PeriodName { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string PersonFullName { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string PaymentStatus { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationStatusCode { get; set; }
        public string ApplicationStatusReason { get; set; }
    }
}
