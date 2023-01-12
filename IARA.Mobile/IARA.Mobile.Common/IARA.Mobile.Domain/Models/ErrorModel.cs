using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Domain.Models
{
    public class ErrorModel
    {
        public int? Id { get; set; }
        public ErrorCode? Code { get; set; }
        public ErrorTypeEnum Type { get; set; }
        public List<string> Messages { get; set; }
    }
}
