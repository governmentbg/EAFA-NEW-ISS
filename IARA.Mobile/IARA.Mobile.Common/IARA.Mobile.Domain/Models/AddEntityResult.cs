using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Domain.Models
{
    public class AddEntityResult
    {
        public int Id { get; }
        public AddEntityResultEnum Result { get; }

        public AddEntityResult(int id, AddEntityResultEnum result)
        {
            Id = id;
            Result = result;
        }
    }
}
