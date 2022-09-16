namespace IARA.Mobile.Application
{
    public static class CommonConstants
    {
        /// <summary>
        /// Date format that should be send to the IARA API (only used for date)
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Date + time format that should be send to the IARA API (only used for date + time)
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Date + time format that should used to save dates locally (only used for date + time)
        /// </summary>
        public const string DateTimeSaveFormat = "yyyy-MM-dd HH:mm:ss.FFFFFFF";

        /// <summary>
        /// The default code used to detect when another nomenclature should be selected.
        /// </summary>
        public const string NomenclatureOther = "Other";

        /// <summary>
        /// The Bulgarian code.
        /// </summary>
        public const string NomenclatureBulgaria = "BGR";

        /// <summary>
        /// File type code of a photo.
        /// </summary>
        public const string PhotoFileType = "PHOTO";

        public const string NewLogin = nameof(NewLogin);
    }
}
