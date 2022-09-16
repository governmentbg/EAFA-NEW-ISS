using System;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionDto
    {
        public int Id { get; set; }
        public bool IsLocal { get; set; }
        public bool HasContentLocally { get; set; }
        public SubmitType SubmitType { get; set; }
        public InspectionState InspectionState { get; set; }
        public InspectionType Type { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Inspectors { get; set; }
        public string InspectionSubjects { get; set; }
        public DateTime StartDate { get; set; }
    }
}
