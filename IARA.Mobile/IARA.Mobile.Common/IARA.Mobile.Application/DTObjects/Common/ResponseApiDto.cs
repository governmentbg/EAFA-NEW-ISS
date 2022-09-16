using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Common
{
    public class ResponseApiDto
    {
        public bool IsSuccessful { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
