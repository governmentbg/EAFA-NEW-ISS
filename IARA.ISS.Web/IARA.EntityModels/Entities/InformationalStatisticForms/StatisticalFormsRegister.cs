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
    /// Регистър купувачи и центрове за първа продажба
    /// </summary>
    [Table("StatisticalFormsRegister", Schema = "RInfStat")]
    [Index(nameof(ApplicationId), Name = "IXFK_StatisticalFormsRegister_Applications")]
    [Index(nameof(SubmittedForLegalId), Name = "IXFK_StatisticalFormsRegister_Legals")]
    [Index(nameof(StatisticalFormTypeId), Name = "IXFK_StatisticalFormsRegister_NStatisticalFormTypes")]
    [Index(nameof(SubmittedForPersonId), Name = "IXFK_StatisticalFormsRegister_Persons")]
    [Index(nameof(RegisterApplicationId), Name = "IXFK_StatisticalFormsRegister_StatisticalFormsRegister")]
    [Index(nameof(RecordType), nameof(RegistrationNum), Name = "UK_RInfStat_StatisticalFormsRegister_RegNum", IsUnique = true)]
    [Index(nameof(SubmittedForLegalId), nameof(SubmittedForPersonId), nameof(ForYear), Name = "UK_RInfStat_StatisticalFormsRegister_SubmitByYear", IsUnique = true)]
    public partial class StatisticalFormsRegister
    {
        public StatisticalFormsRegister()
        {
            EmployeeStatCounts = new HashSet<EmployeeStatCount>();
            EmployeeStatNumericValues = new HashSet<EmployeeStatNumericValue>();
            InverseRegisterApplication = new HashSet<StatisticalFormsRegister>();
            StatisticalFormsRegisterFiles = new HashSet<StatisticalFormsRegisterFile>();
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Заявление
        /// </summary>
        [Column("ApplicationID")]
        public int ApplicationId { get; set; }
        /// <summary>
        /// Тип на запис - заявление или регистров :  Application / Register
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RecordType { get; set; }
        /// <summary>
        /// Заявление, от което е създаден регистровия запис - само при ApplicationStatus = &apos;Register&apos;
        /// </summary>
        [Column("RegisterApplicationID")]
        public int? RegisterApplicationId { get; set; }
        /// <summary>
        /// Титуляр - юридическо  лице
        /// </summary>
        [Column("SubmittedForLegalID")]
        public int? SubmittedForLegalId { get; set; }
        /// <summary>
        /// Титуляр, ако е физическо лице
        /// </summary>
        [Column("SubmittedForPersonID")]
        public int? SubmittedForPersonId { get; set; }
        /// <summary>
        /// Длъжност на подаващ формуляра
        /// </summary>
        [StringLength(200)]
        public string SubmitPersonWorkPosition { get; set; }
        /// <summary>
        /// Тип купувач (регистриран, ЦПП)
        /// </summary>
        [Column("StatisticalFormTypeID")]
        public int StatisticalFormTypeId { get; set; }
        /// <summary>
        /// Година на подаване
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime ForYear { get; set; }
        /// <summary>
        /// Регистрационен номер генериран по формата на наредбата
        /// </summary>
        [StringLength(50)]
        public string RegistrationNum { get; set; }
        /// <summary>
        /// Дата на регистрация
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? RegistrationDate { get; set; }
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
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        [InverseProperty("StatisticalFormsRegisters")]
        public virtual Application Application { get; set; }
        [ForeignKey(nameof(RegisterApplicationId))]
        [InverseProperty(nameof(StatisticalFormsRegister.InverseRegisterApplication))]
        public virtual StatisticalFormsRegister RegisterApplication { get; set; }
        [ForeignKey(nameof(StatisticalFormTypeId))]
        [InverseProperty(nameof(NstatisticalFormType.StatisticalFormsRegisters))]
        public virtual NstatisticalFormType StatisticalFormType { get; set; }
        [ForeignKey(nameof(SubmittedForLegalId))]
        [InverseProperty(nameof(Legal.StatisticalFormsRegisters))]
        public virtual Legal SubmittedForLegal { get; set; }
        [ForeignKey(nameof(SubmittedForPersonId))]
        [InverseProperty(nameof(Person.StatisticalFormsRegisters))]
        public virtual Person SubmittedForPerson { get; set; }
        [InverseProperty("StatisticalForm")]
        public virtual AquacutlureForm AquacutlureForm { get; set; }
        [InverseProperty("StatisticalForm")]
        public virtual FishVesselsForm FishVesselsForm { get; set; }
        [InverseProperty("StatisticalForm")]
        public virtual ReworkForm ReworkForm { get; set; }
        [InverseProperty(nameof(EmployeeStatCount.StatisticalForm))]
        public virtual ICollection<EmployeeStatCount> EmployeeStatCounts { get; set; }
        [InverseProperty(nameof(EmployeeStatNumericValue.StatisticalForm))]
        public virtual ICollection<EmployeeStatNumericValue> EmployeeStatNumericValues { get; set; }
        [InverseProperty(nameof(StatisticalFormsRegister.RegisterApplication))]
        public virtual ICollection<StatisticalFormsRegister> InverseRegisterApplication { get; set; }
        [InverseProperty(nameof(StatisticalFormsRegisterFile.Record))]
        public virtual ICollection<StatisticalFormsRegisterFile> StatisticalFormsRegisterFiles { get; set; }
    }
}