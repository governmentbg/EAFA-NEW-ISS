using System.ComponentModel.DataAnnotations.Schema;

namespace IARA.DomainModels.DTOModels.Common
{
    public class SystemPropertiesDTO
    {
        public SystemPropertiesDTO()
        { }

        [Column("ELDER_TICKET_FEMALE_AGE")]
        public int ElderTicketFemaleAge { get; set; }

        [Column("ELDER_TICKET_MALE_AGE")]
        public int ElderTicketMaleAge { get; set; }

        [Column("MAX_NUMBER_OF_UNDER14_TICKETS")]
        public int MaxNumberOfUnder14Tickets { get; set; }

        [Column("MAX_NUMBER_FISHING_GEARS")]
        public int MaxNumberFishingGears { get; set; }

        [Column("ACDR_USER_ID")]
        public string ACDRUserID { get; set; }

        [Column("ACDR_USER_NAME")]
        public string ACDRUserName { get; set; }
    }
}
