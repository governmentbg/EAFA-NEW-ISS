// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    /// <summary>
    /// Прикачени файлове
    /// </summary>
    [Table("Files", Schema = "iss")]
    [Index(nameof(ContentLength), nameof(ContentHash), Name = "UK_ISS_Files", IsUnique = true)]
    public partial class File
    {
        public File()
        {
            AdmissionLogBookPageFiles = new HashSet<AdmissionLogBookPageFile>();
            ApplicationChangeHistoryFiles = new HashSet<ApplicationChangeHistoryFile>();
            ApplicationFiles = new HashSet<ApplicationFile>();
            AquacultureFacilityRegisterFiles = new HashSet<AquacultureFacilityRegisterFile>();
            AquacultureLogBookPageFiles = new HashSet<AquacultureLogBookPageFile>();
            AuanregisterFiles = new HashSet<AuanregisterFile>();
            BuyerLicenseFiles = new HashSet<BuyerLicenseFile>();
            BuyerRegisterFiles = new HashSet<BuyerRegisterFile>();
            CatchQuotaFiles = new HashSet<CatchQuotaFile>();
            DuplicatesRegisterFiles = new HashSet<DuplicatesRegisterFile>();
            FirstSaleLogBookPageFiles = new HashSet<FirstSaleLogBookPageFile>();
            FishermenRegisterFiles = new HashSet<FishermenRegisterFile>();
            FishingAssociationFiles = new HashSet<FishingAssociationFile>();
            FishingCatchRecordFiles = new HashSet<FishingCatchRecordFile>();
            FishingTicketFiles = new HashSet<FishingTicketFile>();
            InspectionRegisterFiles = new HashSet<InspectionRegisterFile>();
            LegalFiles = new HashSet<LegalFile>();
            NewsFiles = new HashSet<NewsFile>();
            PenalDecreesRegisterFiles = new HashSet<PenalDecreesRegisterFile>();
            PermitLicensesRegisterFiles = new HashSet<PermitLicensesRegisterFile>();
            PermitRegisterFiles = new HashSet<PermitRegisterFile>();
            PersonFiles = new HashSet<PersonFile>();
            ScientificPermitRegisterFiles = new HashSet<ScientificPermitRegisterFile>();
            ShipLogBookPageFiles = new HashSet<ShipLogBookPageFile>();
            ShipRegisterFiles = new HashSet<ShipRegisterFile>();
            StatisticalFormsRegisterFiles = new HashSet<StatisticalFormsRegisterFile>();
            TransportationLogBookPageFiles = new HashSet<TransportationLogBookPageFile>();
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Име на файла
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        ///  Mime тип на файла
        /// </summary>
        [Required]
        [StringLength(100)]
        public string MimeType { get; set; }
        /// <summary>
        /// Брой FK, които сочат към този ред. Когато стане 0, правим IsActive=False, преди това при триене намаляме counter-a.
        /// </summary>
        public int ReferenceCounter { get; set; }
        /// <summary>
        /// Хеш на съдържанието на файла по SHA-256
        /// </summary>
        [Required]
        [StringLength(64)]
        public string ContentHash { get; set; }
        /// <summary>
        /// Дължина на съдържанието в байтове
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// Съдържание на файла
        /// </summary>
        [Required]
        public byte[] Content { get; set; }
        public DateTime UploadedOn { get; set; }
        /// <summary>
        /// Коментари / Бележки
        /// </summary>
        [StringLength(4000)]
        public string Comments { get; set; }
        /// <summary>
        /// Флаг дали записът е активен или изтрит
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
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
        [StringLength(4000)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        [InverseProperty(nameof(AdmissionLogBookPageFile.File))]
        public virtual ICollection<AdmissionLogBookPageFile> AdmissionLogBookPageFiles { get; set; }
        [InverseProperty(nameof(ApplicationChangeHistoryFile.File))]
        public virtual ICollection<ApplicationChangeHistoryFile> ApplicationChangeHistoryFiles { get; set; }
        [InverseProperty(nameof(ApplicationFile.File))]
        public virtual ICollection<ApplicationFile> ApplicationFiles { get; set; }
        [InverseProperty(nameof(AquacultureFacilityRegisterFile.File))]
        public virtual ICollection<AquacultureFacilityRegisterFile> AquacultureFacilityRegisterFiles { get; set; }
        [InverseProperty(nameof(AquacultureLogBookPageFile.File))]
        public virtual ICollection<AquacultureLogBookPageFile> AquacultureLogBookPageFiles { get; set; }
        [InverseProperty(nameof(AuanregisterFile.File))]
        public virtual ICollection<AuanregisterFile> AuanregisterFiles { get; set; }
        [InverseProperty(nameof(BuyerLicenseFile.File))]
        public virtual ICollection<BuyerLicenseFile> BuyerLicenseFiles { get; set; }
        [InverseProperty(nameof(BuyerRegisterFile.File))]
        public virtual ICollection<BuyerRegisterFile> BuyerRegisterFiles { get; set; }
        [InverseProperty(nameof(CatchQuotaFile.File))]
        public virtual ICollection<CatchQuotaFile> CatchQuotaFiles { get; set; }
        [InverseProperty(nameof(DuplicatesRegisterFile.File))]
        public virtual ICollection<DuplicatesRegisterFile> DuplicatesRegisterFiles { get; set; }
        [InverseProperty(nameof(FirstSaleLogBookPageFile.File))]
        public virtual ICollection<FirstSaleLogBookPageFile> FirstSaleLogBookPageFiles { get; set; }
        [InverseProperty(nameof(FishermenRegisterFile.File))]
        public virtual ICollection<FishermenRegisterFile> FishermenRegisterFiles { get; set; }
        [InverseProperty(nameof(FishingAssociationFile.File))]
        public virtual ICollection<FishingAssociationFile> FishingAssociationFiles { get; set; }
        [InverseProperty(nameof(FishingCatchRecordFile.File))]
        public virtual ICollection<FishingCatchRecordFile> FishingCatchRecordFiles { get; set; }
        [InverseProperty(nameof(FishingTicketFile.File))]
        public virtual ICollection<FishingTicketFile> FishingTicketFiles { get; set; }
        [InverseProperty(nameof(InspectionRegisterFile.File))]
        public virtual ICollection<InspectionRegisterFile> InspectionRegisterFiles { get; set; }
        [InverseProperty(nameof(LegalFile.File))]
        public virtual ICollection<LegalFile> LegalFiles { get; set; }
        [InverseProperty(nameof(NewsFile.File))]
        public virtual ICollection<NewsFile> NewsFiles { get; set; }
        [InverseProperty(nameof(PenalDecreesRegisterFile.File))]
        public virtual ICollection<PenalDecreesRegisterFile> PenalDecreesRegisterFiles { get; set; }
        [InverseProperty(nameof(PermitLicensesRegisterFile.File))]
        public virtual ICollection<PermitLicensesRegisterFile> PermitLicensesRegisterFiles { get; set; }
        [InverseProperty(nameof(PermitRegisterFile.File))]
        public virtual ICollection<PermitRegisterFile> PermitRegisterFiles { get; set; }
        [InverseProperty(nameof(PersonFile.File))]
        public virtual ICollection<PersonFile> PersonFiles { get; set; }
        [InverseProperty(nameof(ScientificPermitRegisterFile.File))]
        public virtual ICollection<ScientificPermitRegisterFile> ScientificPermitRegisterFiles { get; set; }
        [InverseProperty(nameof(ShipLogBookPageFile.File))]
        public virtual ICollection<ShipLogBookPageFile> ShipLogBookPageFiles { get; set; }
        [InverseProperty(nameof(ShipRegisterFile.File))]
        public virtual ICollection<ShipRegisterFile> ShipRegisterFiles { get; set; }
        [InverseProperty(nameof(StatisticalFormsRegisterFile.File))]
        public virtual ICollection<StatisticalFormsRegisterFile> StatisticalFormsRegisterFiles { get; set; }
        [InverseProperty(nameof(TransportationLogBookPageFile.File))]
        public virtual ICollection<TransportationLogBookPageFile> TransportationLogBookPageFiles { get; set; }
    }
}