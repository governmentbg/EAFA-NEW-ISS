using System.Collections.Generic;
using IARA.WebHelpers.Enums;

namespace IARA.WebHelpers
{
    public class ErrorModel
    {
        public int? ID { get; set; }
        public ErrorCode? Code { get; set; }
        public ErrorType Type { get; set; }
        public List<string> Messages { get; set; }
    }
}
