namespace IARA.Mobile.Pub
{
    /// <summary>
    /// Validation Groups
    /// </summary>
    public static class Group
    {
        public const string FOREIGNER = "Foreigner";
        public const string ADDRESS = "Address";
        public const string BULGARIAN_ADDRESS = "BulgarianAddress";
        public const string BULGARIAN_CITIZEN = "BulgarianCitizen";
        public const string NON_PERMANENT_EXPERTISE = "NonPermanentExpertise";
        public const string DATEOFBIRTH = "DateOfBirth";
        public const string Gender = "Gender";
        public const string CORRESPONDENCE = "Correspondence";
        public const string DOCUMENT = "DOCUMENT";
        public const string ACCEPT_AGREEMENT = "AcceptAgreement";
        public const string NEWS_DISTRICTS = "NewsNotificationsDistricts";
        public const string IS_UNDER_14 = "IsUnder14";
    }

    public static class ApplicationStatuses
    {
        public const string FILL_BY_APPL = nameof(FILL_BY_APPL);
        public const string CORR_BY_USR_NEEDED = nameof(CORR_BY_USR_NEEDED);
        public const string PAYMENT_PROCESSING = nameof(PAYMENT_PROCESSING);
        public const string CONFIRMED_ISSUED_TICKET = nameof(CONFIRMED_ISSUED_TICKET);
        public const string TICKET_ISSUED = nameof(TICKET_ISSUED);
        public const string INSP_CORR_FROM_EMP = nameof(INSP_CORR_FROM_EMP);
        public const string PAYMENT_ANNUL = nameof(PAYMENT_ANNUL);
    }

    public static class SystemParameters
    {
        public const string ELDER_TICKET_FEMALE_AGE = nameof(ELDER_TICKET_FEMALE_AGE);
        public const string ELDER_TICKET_MALE_AGE = nameof(ELDER_TICKET_MALE_AGE);
        public const string MAX_NUMBER_OF_UNDER14_TICKETS = nameof(MAX_NUMBER_OF_UNDER14_TICKETS);
    }

    public static class PaymentTypesConstants
    {
        public const string PAY_EGOV_BANK = "PayEGovBank";
        public const string PAY_EGOV_EPOS = "PayEGovePOS";
        public const string PAY_EGOV_EPAYBG = "PayEGovePayBG";
    }

    public static class Notifications
    {
        public const string IS_LOCAL = nameof(IS_LOCAL);
        public const string NEWS_ID = "NewsId";
    }
}
