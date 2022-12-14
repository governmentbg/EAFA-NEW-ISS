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
    /// Видове файлове за прикачване
    /// </summary>
    [Table("NFileTypes", Schema = "iss")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_ISS_NFileTypes", IsUnique = true)]
    public partial class NfileType
    {
        public NfileType()
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
            NrequiredFileTypes = new HashSet<NrequiredFileType>();
            PenalDecreesRegisterFiles = new HashSet<PenalDecreesRegisterFile>();
            PermitLicensesRegisterFiles = new HashSet<PermitLicensesRegisterFile>();
            PermitRegisterFiles = new HashSet<PermitRegisterFile>();
            PersonFiles = new HashSet<PersonFile>();
            ScientificPermitRegisterFiles = new HashSet<ScientificPermitRegisterFile>();
            ShipLogBookPageFiles = new HashSet<ShipLogBookPageFile>();
            ShipRegisterFiles = new HashSet<ShipRegisterFile>();
            StatisticalFormsRegisterFiles = new HashSet<StatisticalFormsRegisterFile>();
            TransportationLogBookPageFiles = new HashSet<TransportationLogBookPageFile>();
        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Име
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(4000)]
        public string Description { get; set; }
        /// <summary>
        /// Флаг дали този тип файл може само да се качи и повече не може да се трие и подменя, само да се сваля (използва се за качени подписани PDFи)
        /// </summary>
        public bool IsReadOnly { get; set; }
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

        [InverseProperty(nameof(AdmissionLogBookPageFile.FileType))]
        public virtual ICollection<AdmissionLogBookPageFile> AdmissionLogBookPageFiles { get; set; }
        [InverseProperty(nameof(ApplicationChangeHistoryFile.FileType))]
        public virtual ICollection<ApplicationChangeHistoryFile> ApplicationChangeHistoryFiles { get; set; }
        [InverseProperty(nameof(ApplicationFile.FileType))]
        public virtual ICollection<ApplicationFile> ApplicationFiles { get; set; }
        [InverseProperty(nameof(AquacultureFacilityRegisterFile.FileType))]
        public virtual ICollection<AquacultureFacilityRegisterFile> AquacultureFacilityRegisterFiles { get; set; }
        [InverseProperty(nameof(AquacultureLogBookPageFile.FileType))]
        public virtual ICollection<AquacultureLogBookPageFile> AquacultureLogBookPageFiles { get; set; }
        [InverseProperty(nameof(AuanregisterFile.FileType))]
        public virtual ICollection<AuanregisterFile> AuanregisterFiles { get; set; }
        [InverseProperty(nameof(BuyerLicenseFile.FileType))]
        public virtual ICollection<BuyerLicenseFile> BuyerLicenseFiles { get; set; }
        [InverseProperty(nameof(BuyerRegisterFile.FileType))]
        public virtual ICollection<BuyerRegisterFile> BuyerRegisterFiles { get; set; }
        [InverseProperty(nameof(CatchQuotaFile.FileType))]
        public virtual ICollection<CatchQuotaFile> CatchQuotaFiles { get; set; }
        [InverseProperty(nameof(DuplicatesRegisterFile.FileType))]
        public virtual ICollection<DuplicatesRegisterFile> DuplicatesRegisterFiles { get; set; }
        [InverseProperty(nameof(FirstSaleLogBookPageFile.FileType))]
        public virtual ICollection<FirstSaleLogBookPageFile> FirstSaleLogBookPageFiles { get; set; }
        [InverseProperty(nameof(FishermenRegisterFile.FileType))]
        public virtual ICollection<FishermenRegisterFile> FishermenRegisterFiles { get; set; }
        [InverseProperty(nameof(FishingAssociationFile.FileType))]
        public virtual ICollection<FishingAssociationFile> FishingAssociationFiles { get; set; }
        [InverseProperty(nameof(FishingCatchRecordFile.FileType))]
        public virtual ICollection<FishingCatchRecordFile> FishingCatchRecordFiles { get; set; }
        [InverseProperty(nameof(FishingTicketFile.FileType))]
        public virtual ICollection<FishingTicketFile> FishingTicketFiles { get; set; }
        [InverseProperty(nameof(InspectionRegisterFile.FileType))]
        public virtual ICollection<InspectionRegisterFile> InspectionRegisterFiles { get; set; }
        [InverseProperty(nameof(LegalFile.FileType))]
        public virtual ICollection<LegalFile> LegalFiles { get; set; }
        [InverseProperty(nameof(NewsFile.FileType))]
        public virtual ICollection<NewsFile> NewsFiles { get; set; }
        [InverseProperty(nameof(NrequiredFileType.FileType))]
        public virtual ICollection<NrequiredFileType> NrequiredFileTypes { get; set; }
        [InverseProperty(nameof(PenalDecreesRegisterFile.FileType))]
        public virtual ICollection<PenalDecreesRegisterFile> PenalDecreesRegisterFiles { get; set; }
        [InverseProperty(nameof(PermitLicensesRegisterFile.FileType))]
        public virtual ICollection<PermitLicensesRegisterFile> PermitLicensesRegisterFiles { get; set; }
        [InverseProperty(nameof(PermitRegisterFile.FileType))]
        public virtual ICollection<PermitRegisterFile> PermitRegisterFiles { get; set; }
        [InverseProperty(nameof(PersonFile.FileType))]
        public virtual ICollection<PersonFile> PersonFiles { get; set; }
        [InverseProperty(nameof(ScientificPermitRegisterFile.FileType))]
        public virtual ICollection<ScientificPermitRegisterFile> ScientificPermitRegisterFiles { get; set; }
        [InverseProperty(nameof(ShipLogBookPageFile.FileType))]
        public virtual ICollection<ShipLogBookPageFile> ShipLogBookPageFiles { get; set; }
        [InverseProperty(nameof(ShipRegisterFile.FileType))]
        public virtual ICollection<ShipRegisterFile> ShipRegisterFiles { get; set; }
        [InverseProperty(nameof(StatisticalFormsRegisterFile.FileType))]
        public virtual ICollection<StatisticalFormsRegisterFile> StatisticalFormsRegisterFiles { get; set; }
        [InverseProperty(nameof(TransportationLogBookPageFile.FileType))]
        public virtual ICollection<TransportationLogBookPageFile> TransportationLogBookPageFiles { get; set; }
    }
}