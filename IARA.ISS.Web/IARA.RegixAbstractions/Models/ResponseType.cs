using IARA.RegixAbstractions.Enums;

namespace IARA.RegixAbstractions.Models
{
    public class ResponseType<TResponse>
    {
        public TResponse Response { get; set; }
        public RegixResponseStatusEnum Type { get; set; }
    }
}
