namespace IARA.Mobile.Insp
{
    public static class Constants
    {
        public const string IARAInstitution = "ИАРА";

        public const string SignatureBase = "INSP-SIGN";
        public const string InspectorSignature = "INSP-SIGN-Inspector";
        public const string InspectedPersonSignature = "INSP-SIGN-InspectedPerson";
        public const string SignedReport = "SIGNED_REPORT";

        public static string InspectionFileTypeCode { get; set; }

        public const string ShipBase = "Ship-";
        public const string CatchBase = "Catch-";
        public const string ObjectCheckBase = "CheckObject-";
        public const string FishingGearBase = "FishingGear-";

        public const string UnloadingDeclarationType = "CheckObject-UnloadingDeclaration";
        public const string PreliminaryNoticeType = "CheckObject-PreliminaryNotice";
        public const string OPMembershipType = "CheckObject-OPMembership";

        public const string TicketReportCode = "fishingTicketReport";

        public const string EgnReportParameter = "EGNParam";
        public const string TicketNumReportParameter = "ticketNum";
    }
}
