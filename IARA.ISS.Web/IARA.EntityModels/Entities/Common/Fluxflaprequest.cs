using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    /// <summary>
    /// FLAP заявки
    /// </summary>
    [Table("FLUXFLAPRequests", Schema = "iss")]
    [Index(nameof(FluxfvmsrequestId), Name = "IXFK_FLUXFLAPRequests_FLUXFVMSRequests")]
    [Index(nameof(MdrFlapRequestPurposeId), Name = "IXFK_FLUXFLAPRequests_MDR_FLAP_Request_Purpose")]
    [Index(nameof(ShipId), Name = "IXFK_FLUXFLAPRequests_ShipRegister")]
    [Index(nameof(FluxfvmsrequestId), nameof(ValidTo), Name = "UK_ISS_FLUXFLAPRequests", IsUnique = true)]
    public partial class Fluxflaprequest
    {
        public Fluxflaprequest()
        {

        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// FLUXFVMSRequest запис
        /// </summary>
        [Column("FLUXFVMSRequestID")]
        public int FluxfvmsrequestId { get; set; }
        /// <summary>
        /// Причина за заявлението
        /// </summary>
        [Column("MdrFlapRequestPurposeID")]
        public int MdrFlapRequestPurposeId { get; set; }
        /// <summary>
        /// Съдържание на заявката
        /// </summary>
        [Required]
        public string RequestContent { get; set; }
        /// <summary>
        /// Съдържание на отговора
        /// </summary>
        public string ResponseContent { get; set; }
        /// <summary>
        /// Идентификатор на кораб при изходящи заявки
        /// </summary>
        [Column("ShipID")]
        public int? ShipId { get; set; }
        /// <summary>
        /// Тип идентификатор на кораб
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ShipIdentifierType { get; set; }
        /// <summary>
        /// Идентификатор на кораб
        /// </summary>
        [Required]
        [StringLength(30)]
        public string ShipIdentifier { get; set; }
        /// <summary>
        /// Име на кораб
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ShipName { get; set; }
        /// <summary>
        /// Начална дата на валидност на записа
        /// </summary>
        public DateTime ValidFrom { get; set; }
        /// <summary>
        /// Крайна дата на валидност на записа
        /// </summary>
        public DateTime ValidTo { get; set; }
        /// <summary>
        /// Потребител създал записа
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Дата и час на създаване на записа
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Потребител последно актуализирал записа
        /// </summary>
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
