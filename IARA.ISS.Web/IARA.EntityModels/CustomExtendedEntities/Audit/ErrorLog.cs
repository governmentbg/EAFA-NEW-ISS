using System;
using System.ComponentModel.DataAnnotations.Schema;
using TL.Logging.Abstractions.Interfaces.Models.Error;

namespace IARA.EntityModels.Entities
{
    public partial class ErrorLog : IErrorLog, IErrorClient, IErrorIdentity
    {
        [NotMapped]
        public Guid ErrorId { get => this.ExceptionUid; set => this.ExceptionUid = value; }
    }
}
