﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    /// <summary>
    /// Заявки за комуникация с FLUX/СНРК
    /// </summary>
    [Table("FLUXFVMSRequests", Schema = "iss")]
    [Index(nameof(RequestUuid), Name = "UK_Appl_FLUXFVMSRequests", IsUnique = true)]
    public partial class Fluxfvmsrequest
    {         public Fluxfvmsrequest()
        {
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Флаг дали заявката е изходяща за ИСС или е входяща към ИСС.
        /// </summary>
        public bool IsOutgoing { get; set; }
        /// <summary>
        /// Име на FLUX домейн
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DomainName { get; set; }
        /// <summary>
        /// Име на извиканата услуга 
        /// </summary>
        [Required]
        [StringLength(500)]
        public string WebServiceName { get; set; }
        [Column("RequestUUID")]
        public Guid RequestUuid { get; set; }
        /// <summary>
        /// Дата и час на подаване на заявката
        /// </summary>
        public DateTime RequestDateTime { get; set; }
        /// <summary>
        /// Съдържание на заявката
        /// </summary>
        [Required]
        public string RequestContent { get; set; }
        /// <summary>
        /// Статус на отговора (OK, Error)
        /// </summary>
        [StringLength(50)]
        public string ResponseStatus { get; set; }
        [Column("ResponseUUID")]
        public Guid? ResponseUuid { get; set; }
        /// <summary>
        /// Дата и час на отговора
        /// </summary>
        public DateTime? ResponseDateTime { get; set; }
        /// <summary>
        /// Съдържание на отговора
        /// </summary>
        public string ResponseContent { get; set; }
        /// <summary>
        /// Описание на намерените грешки
        /// </summary>
        [StringLength(4000)]
        public string ErrorDescription { get; set; }
        /// <summary>
        /// Брой направени опити
        /// </summary>
        public short Attempts { get; set; }
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
    }
}
