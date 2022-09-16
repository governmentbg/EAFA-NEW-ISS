using IARA.Mobile.Domain.Enums;
using System.Collections.Generic;

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
