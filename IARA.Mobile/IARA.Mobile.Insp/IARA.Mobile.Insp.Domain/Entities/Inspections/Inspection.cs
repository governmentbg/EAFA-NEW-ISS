using System;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Insp.Domain.Enums;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class Inspection : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Identifier { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool IsLocal { get; set; }
        public SubmitType SubmitType { get; set; }
        public InspectionType InspectionType { get; set; }
        public int InspectionTypeId { get; set; }
        public InspectionState InspectionState { get; set; }
        public int InspectionStateId { get; set; }
        public string ReportNr { get; set; }
        public DateTime StartDate { get; set; }
        public bool CreatedByCurrentUser { get; set; } = true;
        public string Inspectors { get; set; }
        public string InspectionSubjects { get; set; }
        public bool IsStatusChanged { get; set; }
        public bool HasJsonContent { get; set; }
        public string JsonContent { get; set; }
    }
}
