using System;
using IARA.EntityModels.Entities;

namespace IARA.EntityModels.Interfaces
{
    public interface ICommercialFishingRegister
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int ShipId { get; set; }
        public int QualifiedFisherId { get; set; }
        public bool? IsQualifiedFisherSameAsSubmittedFor { get; set; }
        public int WaterTypeId { get; set; }
        public int? SubmittedForLegalId { get; set; }
        public int? SubmittedForPersonId { get; set; }
        public Person SubmittedForPerson { get; set; }
        public Legal SubmittedForLegal { get; set; }
        public int? PoundNetId { get; set; }
        public DateTime? IssueDate { get; set; }
        public HolderGroundsForUse ShipGroundsForUse { get; set; }
        public int? ShipGroundsForUseId { get; set; }
        public bool? IsHolderShipOwner { get; set; }
    }
}
